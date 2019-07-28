using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Neuro.Domain.Layers;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class Network
    {
        public List<ILayer> Layers;
        public int InputWidth { get; private set; }
        public int InputHeight { get; private set; }

        public void InitLayers((int X, int Y) shape, params ILayer[] layers)
        {
            InputWidth = shape.X;
            InputHeight = shape.Y;
            Layers = new List<ILayer>();
            
            var neuronsCount = 0;
            var shapeX = shape.X;
            var shapeY = shape.Y;
            int inputLength = shapeX * shapeY;
            
            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i].Type == LayerType.FullyConnected)
                {
                    var layer = (IFullyConnectedLayer) layers[i];
                    
                    layer.Init(i, inputLength);

                    inputLength = layer.Outputs.Length;
                    neuronsCount = layer.NeuronsCount;
                }

                if (layers[i].Type == LayerType.Softmax)
                {
                    var layer = (ISoftmaxLayer)layers[i];

                    layer.Init(i, inputLength);

                    inputLength = layer.Outputs.Length;
                    neuronsCount = layer.NeuronsCount;
                }

                if (layers[i].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionLayer) layers[i];
                    var linksCount = neuronsCount > 0 ? layer.NeuronsCount / neuronsCount + 1 : 0;

                    layer.Init(i, shapeX, shapeY, linksCount);
                    
                    inputLength = layer.OutputHeight * layer.OutputWidht * layer.NeuronsCount;
                    shapeX = layer.OutputWidht;
                    shapeY = layer.OutputHeight;
                    neuronsCount = layer.NeuronsCount;
                }
                
                if (layers[i].Type == LayerType.MaxPoolingLayer)
                {
                    var layer = (IMaxPoolingLayer) layers[i];

                    if (shapeX % layer.KernelSize != 0 || shapeY % layer.KernelSize != 0)
                    {
                        throw new ArgumentException($"Слой №{i}. Размер входного изображения ({shapeX}x{shapeY}) должен быть кратным размеру ядра ({layer.KernelSize})");
                    }
                    
                    layer.Init(i, neuronsCount, shapeX, shapeY);
                    
                    inputLength = layer.OutputHeight * layer.OutputWidht * layer.NeuronsCount;
                    shapeX = layer.OutputWidht;
                    shapeY = layer.OutputHeight;
                    neuronsCount = layer.NeuronsCount;
                }

                if (layers[i].Type == LayerType.Dropout)
                {
                    var layer = (IDropoutLayer)layers[i];

                    layer.Init(i);
                }

                Layers.Add(layers[i]);
            }
        }

        public void Randomize()
        {
            foreach (var layer in Layers.Where(l => l is IRandomize))
            {
                (layer as IRandomize)?.Randomize();
            }
        }
        
        public double[] Compute(double[,] input)
        {
            var output = new[] {new Matrix(input)};

            foreach (var layer in Layers.Where(l => l.Type == LayerType.Convolution || l.Type == LayerType.MaxPoolingLayer))
            {
                output = (layer as IMatrixLayer)?.Compute(output);
            }
            
            var outputLinear = output.To1DArray();

            foreach (var layer in Layers.Where(l => l.Type == LayerType.FullyConnected || l.Type == LayerType.Softmax))
            {
                outputLinear = (layer as ILinearCompute)?.Compute(outputLinear);
            }

            return outputLinear;
        }
        
        public byte[] Save()
        {
            var data = new SaveNetworkModel
            {
                InputWidth = InputWidth,
                InputHeight = InputHeight,
                Layers = new List<LayerSaveData>()
            };

            foreach (var layer in Layers)
            {
                var layerSaveData = new LayerSaveData
                {
                    Type = layer.Type,
                    OutputLength = layer.NeuronsCount,
                    FullyConnectedNeurons= new List<FullyConnectedNeuronSaveData>(),
                    ConvNeurons = new List<ConvNeuronSaveData>(),
                };

                switch (layer.Type)
                {
                    case LayerType.FullyConnected:
                    {
                        var fullyConnectedLayer = (FullyConnectedLayer) layer;

                        layerSaveData.ActivationType = fullyConnectedLayer.ActivationFunctionType;

                        foreach (var neuron in fullyConnectedLayer.Neurons)
                        {
                            var weights = new double[neuron.Weights.Length];

                            for (var i = 0; i < neuron.Weights.Length; i++)
                            {
                                weights[i] = neuron.Weights[i];
                            }
                    
                            layerSaveData.FullyConnectedNeurons.Add(new FullyConnectedNeuronSaveData
                            {
                                Weights = weights,
                                Bias = neuron.Bias
                            });
                        }

                        break;
                    }
                    case LayerType.Softmax:
                    {
                        var fullyConnectedLayer = (SoftmaxLayer) layer;

                        foreach (var neuron in fullyConnectedLayer.Neurons)
                        {
                            var weights = new double[neuron.Weights.Length];

                            for (var i = 0; i < neuron.Weights.Length; i++)
                            {
                                weights[i] = neuron.Weights[i];
                            }
                    
                            layerSaveData.FullyConnectedNeurons.Add(new FullyConnectedNeuronSaveData
                            {
                                Weights = weights,
                                Bias = neuron.Bias
                            });
                        }

                        break;
                    }
                    case LayerType.Convolution:
                    {
                        var fullyConnectedLayer = (ConvolutionLayer) layer;
                    
                        layerSaveData.ActivationType = fullyConnectedLayer.ActivationFunctionType;
                        layerSaveData.KernelSize = fullyConnectedLayer.KernelSize;
                    
                        foreach (var neuron in fullyConnectedLayer.Neurons)
                        {
                            layerSaveData.ConvNeurons.Add(new ConvNeuronSaveData
                            {
                                KernelSize = neuron.Weights.GetLength(0),
                                Weights = neuron.Weights.Value,
                                Bias = neuron.Bias
                            });
                        }

                        break;
                    }
                    case LayerType.MaxPoolingLayer:
                    {
                        var fullyConnectedLayer = (MaxPoolingLayer) layer;

                        layerSaveData.KernelSize = fullyConnectedLayer.KernelSize;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                data.Layers.Add(layerSaveData);
            }
            
            if(!data.Layers.Any())
                return null;

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();

                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }
        
        public void Load(byte[] data)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            
            var obj = (SaveNetworkModel) binForm.Deserialize(memStream);
            var layers = new List<ILayer>();

            Layers = null;
            
            foreach (var layer in obj.Layers)
            {
                switch (layer.Type)
                {
                    case LayerType.Convolution:
                        layers.Add(new ConvolutionLayer(layer.ActivationType, layer.OutputLength, layer.KernelSize, true));
                        break;
                    case LayerType.MaxPoolingLayer:
                        layers.Add(new MaxPoolingLayer(layer.KernelSize));
                        break;
                    case LayerType.FullyConnected:
                        layers.Add(new FullyConnectedLayer(layer.OutputLength, layer.ActivationType));
                        break;
                    case LayerType.Softmax:
                        layers.Add(new SoftmaxLayer(layer.OutputLength));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            InitLayers((obj.InputWidth, obj.InputHeight), layers.ToArray());
            
            for (var l = 0; l < Layers.Count; l++)
            {
                switch (Layers[l].Type)
                {
                    case LayerType.Convolution:
                    {
                        var layer = (ConvolutionLayer) Layers[l];

                        for (var n = 0; n < layer.NeuronsCount; n++)
                        {
                            layer.Neurons[n].Weights = new Matrix(obj.Layers[l].ConvNeurons[n].Weights);
                            layer.Neurons[n].Bias = obj.Layers[l].ConvNeurons[n].Bias;
                        }
                        
                        break;
                    }
                    case LayerType.FullyConnected:
                    {
                        var layer = (FullyConnectedLayer) Layers[l];

                        for (var n = 0; n < layer.NeuronsCount; n++)
                        {
                            layer.Neurons[n].Weights = obj.Layers[l].FullyConnectedNeurons[n].Weights;
                            layer.Neurons[n].Bias = obj.Layers[l].FullyConnectedNeurons[n].Bias;
                        }
                        
                        break;
                    }
                    case LayerType.Softmax:
                    {
                        var layer = (SoftmaxLayer) Layers[l];

                        for (var n = 0; n < layer.NeuronsCount; n++)
                        {
                            layer.Neurons[n].Weights = obj.Layers[l].FullyConnectedNeurons[n].Weights;
                            layer.Neurons[n].Bias = obj.Layers[l].FullyConnectedNeurons[n].Bias;
                        }
                        
                        break;
                    }
                    case LayerType.MaxPoolingLayer:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public static class NetworkExtensions
    {
        public static Network AddConvolutionLayer(this Network network, ActivationType activationType, int neuronsCount, int kernelSize = 3, bool useReferences = false)
        {
            network.Layers.Add(new ConvolutionLayer(activationType, neuronsCount, kernelSize, useReferences));

            return network;
        }
    }
}