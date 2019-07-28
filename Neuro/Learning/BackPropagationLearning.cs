using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuro.Domain.Layers;
using Neuro.Extensions;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;

namespace Neuro.Learning
{
    public class BackPropagationLearning
    {
        private readonly Network network;
        private readonly double[][] fullyConnectedNeuronErrors;
        private readonly Matrix[][] convNeuronErrors;
        private readonly IFullyConnectedLayer[] fullyConnectedLayers;

        public double LearningRate { get; set; } = 0.01f;

        public BackPropagationLearning(Network network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();
            var softmaxAny = network.Layers.Any(x => x.Type == LayerType.Softmax);

            fullyConnectedNeuronErrors = new double[fullyConnectedLayers.Length + (softmaxAny ? 1 : 0)][];
            convNeuronErrors = new Matrix[matrixLayers.Length][];
            
            for (var i = 0; i < fullyConnectedLayers.Length; i++)
                fullyConnectedNeuronErrors[i] = new double[fullyConnectedLayers[i].NeuronsCount];
            
            if (softmaxAny)
                fullyConnectedNeuronErrors[fullyConnectedNeuronErrors.Length - 1] = new double[network.Layers.Last().NeuronsCount];
            
            for (var i = 0; i < matrixLayers.Length; i++)
                convNeuronErrors[i] = new Matrix[matrixLayers[i].Outputs.Length];
        }

        public double Run(double[,] input, double[] output)
        {
//            network.Compute(input);
            var error = CalculateFullyConnectedLayersError(output);
            CalculateConvLayersErrorParallel();
            UpdateWeightsParallel(new Matrix(input));

            return error;
        }
        
        private double CalculateFullyConnectedLayersError(IReadOnlyList<double> desiredOutput)
        {
            var lastLayer = network.Layers.Last();
            var errors = fullyConnectedNeuronErrors[fullyConnectedNeuronErrors.Length - 1];
            var output = (lastLayer as ILinearCompute)?.Outputs;
            var error = 0d;
            
            //расчитываем ошибку на последнем слое
            for (var i = 0; i < lastLayer.NeuronsCount; i++)
            {
                var e = desiredOutput[i] - output[i];

                //function.Alpha = layer[i].Bias;
                double derivatived = lastLayer.Type == LayerType.Softmax 
                    ? ((ISoftmaxLayer)lastLayer).Derivative(i) 
                    : ((IFullyConnectedLayer)lastLayer).Function.Derivative(((IFullyConnectedLayer)lastLayer)[i].Output);

                errors[i] = e * derivatived;
                error += e * e;
            }

            var layerIndex = fullyConnectedLayers.Length - (lastLayer.Type == LayerType.Softmax ? 1 : 2);

            //расчитываем ошибку на скрытых слоях
            for (var j = layerIndex; j >= 0; j--)
            {
                var layer = fullyConnectedLayers[j];
                errors = fullyConnectedNeuronErrors[j];
                
                var layerNext = j + 1 > fullyConnectedLayers.Length - 1 && lastLayer.Type == LayerType.Softmax ? (ILinearCompute)lastLayer : (ILinearCompute)fullyConnectedLayers[j + 1];
                var errorsNext = fullyConnectedNeuronErrors[j + 1];

                Parallel.For(0, layer.NeuronsCount, i =>
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new { neuron, nIndex }).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    var function = layer[i].Function;

                    //function.Alpha = layer[i].Bias;
                    errors[i] = function.Derivative(layer[i].Output) * sum;
                });

                errors = network.Layers[layer.Index + 1].Type == LayerType.Dropout ? ((IDropoutLayer)network.Layers[layer.Index + 1]).Derivative(errors) : errors;
            }

            return error;
        }

        private void CalculateConvLayersErrorParallel()
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            
            if (convLayers.Length == 0)
                return;

            //расчет ошибки на слое соединия с полносвязной сетью
            CalcLastConvErrors(convLayers);

            for (var l = convLayers.Length - 1; l > 0; l--)
            {
                var b = convNeuronErrors[l];
                var prevB = convNeuronErrors[l - 1];

                switch (convLayers[l].Type)
                {
                    case LayerType.Convolution:
                    {
                        var layer = (IConvolutionLayer)convLayers[l];
                        var prevLayer = convLayers[l - 1];

                        // b[l-1] = f'(u[l-1]) * sum(b[l]) * rot180(k)
                        Parallel.For(0, prevLayer.NeuronsCount, n =>
                        {
                            var fU = prevLayer is IConvolutionLayer convolutionLayer
                                ? prevLayer.Outputs[n] * convolutionLayer.Neurons[n].Function.Derivative
                                : prevLayer.Outputs[n];

                            var error = new Matrix(new double[prevLayer.Outputs[n].GetLength(0),prevLayer.Outputs[n].GetLength(1)]);
                            var neurons = layer.Neurons.Select((neuron, i) => new {neuron, index = i});
                                
                            if (layer.UseReferences)
                                neurons = neurons.Where(x => x.neuron.ParentId.Contains(n)).ToList();

                            foreach (var neuron in neurons)
                            {
                                error += (b[neuron.index].BackConvolution(neuron.neuron.Weights.Rot180())) * fU;
                            }
                            
                            prevB[n] = error;
                        });
                        break;
                    }

                    case LayerType.MaxPoolingLayer:
                    {
                        var layer = (IMaxPoolingLayer)convLayers[l];
                        var prevLayer = convLayers[l - 1];

                        Parallel.For(0, layer.NeuronsCount, (int n) =>
                        {
                            var outputCords = layer.Neurons[n].OutputCords;
                            var kernelSize = layer.Neurons[n].KernelSize;

                            prevB[n] = new Matrix(new double[prevLayer.OutputHeight, prevLayer.OutputWidht]);

                            // b[l-1] = upsample(b[l]) * f'(u[l-1])
                            for (var y = 0; y < prevLayer.OutputHeight; y++)
                            {
                                for (var x = 0; x < prevLayer.OutputWidht; x++)
                                {
                                    if (outputCords[y, x])
                                    {
                                        prevB[n][y, x] = b[n][y / kernelSize, x / kernelSize];
                                    }
                                }
                            }

                            var fU = prevLayer is IConvolutionLayer convolutionLayer
                                ? prevLayer.Outputs[n] * convolutionLayer.Neurons[n].Function.Derivative
                                : prevLayer.Outputs[n];

                            prevB[n] *= fU;
                        });
                        
                        break;
                    }
                }
            }
        }

        private void CalcLastConvErrors(IReadOnlyList<IMatrixLayer> convLayers)
        {
            if (convLayers.Count == 0)
                return;
            
            var layerIndex = convLayers.Count - 1;
            
            switch (convLayers[layerIndex].Type)
            {
                case LayerType.MaxPoolingLayer:
                {
                    var layer = (IMaxPoolingLayer) convLayers[layerIndex];

                    Parallel.For(0, layer.NeuronsCount, nIndex =>
                    {
                        var outputs = layer.Neurons[nIndex].Outputs;
                        var outputHeight = outputs.GetLength(0);
                        var outputWidth = outputs.GetLength(1);

                        convNeuronErrors[convLayers.Count - 1][nIndex] = new Matrix(new double[outputHeight, outputWidth]);
                        var errors = convNeuronErrors[convLayers.Count - 1][nIndex];

                        for (var y = 0; y < outputHeight; y++)
                        {
                            for (var x = 0; x < outputWidth; x++)
                            {
                                if (layer.Neurons[nIndex].OutputCords[y, x])
                                {
                                    var linerNeuronIndex = (nIndex * outputHeight * outputWidth) + (y * outputWidth + x);
                                    var sum = fullyConnectedLayers[0].Neurons
                                        .Select((neuron, ni) => new { neuron, ni })
                                        .Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                                    errors[y, x] = sum;
                                }
                            }
                        }
                    });
                    break;
                }
                case LayerType.Convolution:
                {
                    var layer = (IConvolutionLayer) convLayers[layerIndex];

                    Parallel.For(0, layer.NeuronsCount, nIndex =>
//                    for (var nIndex = 0; nIndex < layer.NeuronsCount; nIndex++)
                    {
                        var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                        var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);
                        var outputs = layer.Neurons[nIndex].Output;
                        var activationFunction = layer.Neurons[nIndex].Function;
                        var stride = nIndex * outputHeight * outputWidth;

                        convNeuronErrors[convLayers.Count - 1][nIndex] = new Matrix(new double[outputHeight, outputWidth]);
                        var errors = convNeuronErrors[convLayers.Count - 1][nIndex];

                        for (var y = 0; y < outputHeight; y++)
                        {
                            for (var x = 0; x < outputWidth; x++)
                            {
                                var i = stride + (y * outputWidth + x);
                                var sum = fullyConnectedLayers[0].Neurons
                                    .Select((neuron, ni) => new {neuron, ni})
                                    .Sum(j => j.neuron.Weights[i] * fullyConnectedNeuronErrors[0][j.ni]);

                                errors[y, x] = activationFunction.Derivative(outputs[y, x]) * sum;
                            }
                        }
                    });
                    
                    break;
                }
                case LayerType.FullyConnected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void UpdateWeightsParallel(Matrix matrix)
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();

            Parallel.For(0, convLayers.Length, l =>
            {
                if (convLayers[l].Type != LayerType.Convolution) return;
                
                var layer = (IConvolutionLayer)convLayers[l];
                var inputs = l == 0 ? matrix : convLayers[l - 1].Outputs.Sum();

                for (var e = 0; e < convNeuronErrors[l].Length; e++)
                {
                    var parentId = layer.Neurons[e].ParentId;
                    inputs = l > 0 && layer.UseReferences && parentId != null && parentId.Any() ? convLayers[l - 1].Outputs.Where((x, i) => parentId.Contains(i)).ToArray().Sum(): inputs;
                    
                    var error = convNeuronErrors[l][e] * LearningRate;
                    var weights = layer.Neurons[e].Weights;
                    var correction = inputs.Convolution(error.Rot180());

                    layer.Neurons[e].Weights = weights + correction;
                    layer.Neurons[e].Bias += correction.Sum();
                }
            });

            var input = convLayers.Length > 0 ? convLayers.Last().Outputs.To1DArray() : (new [] { matrix }).To1DArray();
            var layersCount = fullyConnectedLayers.Count();

            Parallel.For(0, layersCount, l =>
            {
                var layer = fullyConnectedLayers[l];
                var inputs = l == 0 ? input : fullyConnectedLayers[l - 1].Neurons.Select(x => x.Output).ToArray();

                unsafe
                {
                    Parallel.For(0, layer.NeuronsCount, n =>
                    {
                        var neuron = layer.Neurons[n];
                        var weightsLength = neuron.Weights.Length;
                        var error = fullyConnectedNeuronErrors[l][n];

                        fixed (double* weights = neuron.Weights, x = inputs)
                            for (var i = 0; i < weightsLength; i++)
                            {
                                weights[i] += LearningRate * error * x[i];
                                neuron.Bias += LearningRate * error * x[i];
                            }
                    });
                }
            });

            var lastLayer = network.Layers.Last();

            if (lastLayer.Type == LayerType.Softmax)
            {
                var layer = network.Layers.Last() as ISoftmaxLayer;
                var inputs = fullyConnectedLayers.Last().Outputs;
                
                unsafe
                {
                    Parallel.For(0, layer.NeuronsCount, n =>
                    {
                        var neuron = layer.Neurons[n];
                        var weightsLength = neuron.Weights.Length;
                        var error = fullyConnectedNeuronErrors.Last()[n];

                        fixed (double* weights = neuron.Weights, x = inputs)
                            for (var i = 0; i < weightsLength; i++)
                            {
                                weights[i] += LearningRate * error * x[i];
                                neuron.Bias += LearningRate * error * x[i];
                            }
                    });
                }
            }
        }
    }
}