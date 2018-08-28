using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Extensions;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork
    {
        public ILayer[] Layers;
        private int InputWidth;
        private int InputHeight;
        
        public ConvolutionalNetwork(){}
        
        public void InitLayers(int inputWidth, int inputHeitght, params ILayer[] layers)
        {
            InputWidth = inputWidth;
            InputHeight = inputHeitght;
            var neuronsCount = 0;
            Layers = new ILayer[layers.Length];

            int inputLegth = inputWidth * inputHeitght;
            
            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i].Type == LayerType.FullyConnected)
                {
                    var layer = (IFullyConnectedLayer) layers[i];
                    
                    layer.Init(inputLegth);

                    inputLegth = layer.Outputs.Length;
                    neuronsCount = layer.NeuronsCount;
                }
                
                if (layers[i].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer) layers[i];
                    
                    layer.Init(inputWidth, inputHeitght);
                    
                    inputLegth = layer.OutputHeight * layer.OutputWidht * layer.NeuronsCount;
                    inputWidth = layer.OutputWidht;
                    inputHeitght = layer.OutputHeight;
                    neuronsCount = layer.NeuronsCount;
                }
                
                if (layers[i].Type == LayerType.MaxPoolingLayer)
                {
                    var layer = (IMaxPoolingLayer) layers[i];

                    if (inputWidth % layer.KernelSize != 0 || inputHeitght % layer.KernelSize != 0)
                    {
                        throw new ArgumentException($"Слой №{i}. Размер входного изображения ({inputWidth}x{inputHeitght}) должен быть кратным размеру ядра ({layer.KernelSize})");
                    }
                    
                    layer.Init(neuronsCount, inputWidth, inputHeitght);
                    
                    inputLegth = layer.OutputHeight * layer.OutputWidht * layer.NeuronsCount;
                    inputWidth = layer.OutputWidht;
                    inputHeitght = layer.OutputHeight;
                    neuronsCount = layer.NeuronsCount;
                }
                
                Layers[i] = layers[i];
            }
        }

        public void Randomize()
        {
            foreach (IRandomize layer in Layers.Where(l => l is IRandomize))
            {
                layer.Randomize();
            }
        }
        
        public double[] Compute(double[,] input)
        {
            var output = new[] {new Matrix(input)};

            foreach (IMatrixLayer layer in Layers.Where(l => l.Type == LayerType.Convolution || l.Type == LayerType.MaxPoolingLayer))
            {
                output = layer.Compute(output);
            }
            
            var outputLinear = output.To1DArray();

            foreach (ILinearCompute layer in Layers.Where(l => l.Type == LayerType.FullyConnected))
            {
                outputLinear = layer.Compute(outputLinear);
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

                if (layer.Type == LayerType.FullyConnected)
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
                            Weights = weights
                        });
                    }
                }
                
                if (layer.Type == LayerType.Convolution)
                {
                    var fullyConnectedLayer = (ConvolutionalLayer) layer;
                    
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
                }
                
                if (layer.Type == LayerType.MaxPoolingLayer)
                {
                    var fullyConnectedLayer = (MaxPoolingLayer) layer;

                    layerSaveData.KernelSize = fullyConnectedLayer.KernelSize;
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
                if (layer.Type == LayerType.Convolution)
                {
                    layers.Add(new ConvolutionalLayer(layer.ActivationType, layer.OutputLength, layer.KernelSize));
                }
                
                if (layer.Type == LayerType.MaxPoolingLayer)
                {
                    layers.Add(new MaxPoolingLayer(layer.KernelSize));
                }
                
                if (layer.Type == LayerType.FullyConnected)
                {
                    layers.Add(new FullyConnectedLayer(layer.OutputLength, layer.ActivationType));
                }
            }
            
            InitLayers(obj.InputWidth, obj.InputHeight, layers.ToArray());
            
            for (var l = 0; l < Layers.Length; l++)
            {
                if (Layers[l].Type == LayerType.Convolution)
                {
                    var layer = (ConvolutionalLayer) Layers[l];

                    for (var n = 0; n < layer.NeuronsCount; n++)
                    {
                        layer.Neurons[n].Weights = new Matrix(obj.Layers[l].ConvNeurons[n].Weights);
                        layer.Neurons[n].Bias = obj.Layers[l].ConvNeurons[n].Bias;
                    }
                }
                
                if (Layers[l].Type == LayerType.FullyConnected)
                {
                    var layer = (FullyConnectedLayer) Layers[l];

                    for (var n = 0; n < layer.NeuronsCount; n++)
                    {
                        layer.Neurons[n].Weights = obj.Layers[l].FullyConnectedNeurons[n].Weights;
                    }
                }
            }
        }
    }
}