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

        public double LearningRate { private get; set; } = 0.05f;

        public BackPropagationLearning(Network network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();

            fullyConnectedNeuronErrors = new double[fullyConnectedLayers.Length][];
            convNeuronErrors = new Matrix[matrixLayers.Length][];

            for (var i = 0; i < fullyConnectedLayers.Length; i++)
                fullyConnectedNeuronErrors[i] = new double[fullyConnectedLayers[i].NeuronsCount];
            
            for (var i = 0; i < matrixLayers.Length; i++)
                convNeuronErrors[i] = new Matrix[matrixLayers[i].Outputs.Length];
        }

        public double Run(double[,] input, double[] output)
        {
//            network.Compute(input);
            CalculateFullyConnectedLayersError(output);
            CalculateConvLayersErrorParallel();
            UpdateWeightsParallel(new Matrix(input));

            var lastErrors = fullyConnectedNeuronErrors.Last();

            return lastErrors.Sum(Math.Abs) / lastErrors.Length;
        }
        
        private void CalculateFullyConnectedLayersError(IReadOnlyList<double> desiredOutput)
        {
            var layer = fullyConnectedLayers[fullyConnectedLayers.Length - 1];
            var errors = fullyConnectedNeuronErrors[fullyConnectedLayers.Length - 1];
            var output = layer.Outputs;
            
            //расчитываем ошибку на последнем слое
            for (var i = 0; i < layer.NeuronsCount; i++)
                errors[i] = (desiredOutput[i] - output[i]) * layer[i].Function.Derivative(layer[i].Output);

            //расчитываем ошибку на скрытых слоях
            for (var j = fullyConnectedLayers.Length - 2; j >= 0; j--)
            {
                layer = fullyConnectedLayers[j];
                errors = fullyConnectedNeuronErrors[j];
                
                var layerNext = fullyConnectedLayers[j + 1];
                var errorsNext = fullyConnectedNeuronErrors[j + 1];

                Parallel.For(0, layer.NeuronsCount, i =>
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new { neuron, nIndex }).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    errors[i] = layer[i].Function.Derivative(layer[i].Output) * sum;
                });
            }
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
                        var Eb = b.Sum();

                        Parallel.For(0, prevLayer.NeuronsCount, n =>
                        {
                            // b[l-1] = f'(u[l-1]) * sum(b[l]) * rot180(k)
                            prevB[n] = prevLayer.Outputs[n] * Eb.BackConvolution(layer.Neurons[n].Weights.Rot180());

                            if (prevLayer is IConvolutionLayer convolutionLayer)
                                prevB[n] *= convolutionLayer.Neurons[n].Function.Derivative;
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

                            prevB[n] *= prevLayer.Outputs[n];

                            if (prevLayer is IConvolutionLayer convolutionLayer)
                                prevB[n] *= convolutionLayer.Neurons[n].Function.Derivative;
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
                    {
                        var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                        var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);
                        var outputs = layer.Neurons[nIndex].Output;
                        var activationFunction = layer.Neurons[nIndex].Function;

                        convNeuronErrors[convLayers.Count - 1][nIndex] = new Matrix(new double[outputHeight, outputWidth]);
                        var errors = convNeuronErrors[convLayers.Count - 1][nIndex];

                        for (var y = 0; y < outputHeight; y++)
                        {
                            for (var x = 0; x < outputWidth; x++)
                            {
                                var i = (nIndex * outputHeight * outputWidth) + (y * outputs.GetLength(1) + x);
                                var sum = fullyConnectedLayers[0].Neurons
                                    .Select((neuron, ni) => new {neuron, ni})
                                    .Sum(j => j.neuron.Weights[i] * fullyConnectedNeuronErrors[0][j.ni]);

                                errors[x, y] = activationFunction.Derivative(outputs[y, x]) * sum;
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
                    var error = convNeuronErrors[l][e];
                    var weights = layer.Neurons[e].Weights;

                    layer.Neurons[e].Weights = weights + inputs.Convolution(error.Rot180() * LearningRate).Rot180();
                    layer.Neurons[e].Bias +=
                        layer.Neurons[e].Function is ActivationFunctions.BipolarSigmoid
                            ? 0
                            : layer.Neurons[e].Function.Derivative(error.Sum()) * LearningRate;
                }
            });

            var input = convLayers.Length > 0 ? convLayers.Last().Outputs.To1DArray() : matrix.To1DArray();
            var layersCount = fullyConnectedLayers.Count();

            Parallel.For(0, layersCount, l =>
            {
                var outputs = l == 0 ? input : fullyConnectedLayers[l - 1].Neurons.Select(x => x.Output).ToArray();
                var layer = fullyConnectedLayers[l];

                Parallel.For(0, layer.NeuronsCount, n =>
                {
                    var neuron = layer.Neurons[n];
                    var weightsLength = neuron.Weights.Length;
                    var error = fullyConnectedNeuronErrors[l][n];

                    for (var wIndex = 0; wIndex < weightsLength; wIndex++)
                    {
                        neuron.Weights[wIndex] +=LearningRate * error * outputs[wIndex];
                    }
                });
            });
        }
    }
}