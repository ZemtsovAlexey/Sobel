using System;
using System.Linq;
using System.Threading.Tasks;
using Neuro.Domain.Layers;
using Neuro.Extensions;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;

namespace Neuro.Learning
{
    public class ConvolutionalBackPropagationLearning
    {
        private ConvolutionalNetwork network;
        private double[][] fullyConnectedNeuronErrors;
        private Matrix[][] convNeuronErrors;
        private IFullyConnectedLayer[] fullyConnectedLayers;

        public double LearningRate { get; set; } = 0.05f;

        public ConvolutionalBackPropagationLearning(ConvolutionalNetwork network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();

            fullyConnectedNeuronErrors = new double[fullyConnectedLayers.Length][];
            convNeuronErrors = new Matrix[matrixLayers.Length][];

            for (var i = 0; i < fullyConnectedLayers.Length; i++)
            {
                fullyConnectedNeuronErrors[i] = new double[fullyConnectedLayers[i].NeuronsCount];
            }
            
            for (var i = 0; i < matrixLayers.Length; i++)
            {
                //convNeuronErrors[i] = new Matrix[matrixLayers[i == matrixLayers.Length - 1 ? i : i + 1].Outputs.Length];
                convNeuronErrors[i] = new Matrix[matrixLayers[i].Outputs.Length];
            }
        }

        public double Run(double[,] input, double[] output)
        {
//            network.Compute(input);
            CalculateFullyConnectedLayersError(output);
            UpdateWeightsParallel(input);

            var lastErrors = fullyConnectedNeuronErrors.Last();

            return lastErrors.Sum(x => Math.Abs(x)) / lastErrors.Length;
        }
        
        private void CalculateFullyConnectedLayersError(double[] desiredOutput)
        {        
            IFullyConnectedLayer layer, layerNext;
            double[] output, errors, errorsNext;

            layer = fullyConnectedLayers[fullyConnectedLayers.Length - 1];
            errors = fullyConnectedNeuronErrors[fullyConnectedLayers.Length - 1];
            output = layer.Outputs;
            
            //расчитываем ошибку на последнем слое
            for (var i = 0; i < layer.NeuronsCount; i++)
            {
                errors[i] = (desiredOutput[i] - output[i]) * layer[i].Function.Derivative(layer[i].Output);
            }

            //расчитываем ошибку на скрытых слоях
            for (var j = fullyConnectedLayers.Length - 2; j >= 0; j--)
            {
                layer = fullyConnectedLayers[j];
                layerNext = fullyConnectedLayers[j + 1];
                errors = fullyConnectedNeuronErrors[j];
                errorsNext = fullyConnectedNeuronErrors[j + 1];

                Parallel.For(0, layer.NeuronsCount, (int i) =>
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new { neuron, nIndex }).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    errors[i] = layer[i].Function.Derivative(layer[i].Output) * sum;
                });
            }

            CalculateConvLayersErrorParallel();
        }

        private void CalculateConvLayersErrorParallel()
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            
            if (convLayers.Length == 0)
                return;
            
            //расчет ошибки на слое соединия с полносвязной сетью
            CalcLastConvErrors(convLayers);

            for (int l = convLayers.Length - 1; l > 0; l--)
            {
                var b = convNeuronErrors[l];
                var prevB = convNeuronErrors[l - 1];

                //расчет ошибки на входе сверточного слоя
                if (convLayers[l].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];
                    var prevLayer = convLayers[l - 1];
                    var Eb = b.Sum();// / b.Length;

                    Parallel.For(0, prevLayer.NeuronsCount, n =>
                    //for (var n = 0; n < prevLayer.NeuronsCount; n++)
                    {
                        // b[l-1] = f'(u[l-1]) * sum(b[l]) * rot180(k)
                        prevB[n] = prevLayer.Outputs[n] * Eb.BackСonvolution(layer.Neurons[n].Weights.Rot180());

                        if (prevLayer is IConvolutionalLayer)
                        {
                            prevB[n] *= (prevLayer as IConvolutionalLayer).Neurons[n].Function.Derivative;
                        }
                    });
                }

                //расчет ошибки на входе pooling слоя
                if (convLayers[l].Type == LayerType.MaxPoolingLayer)
                {
                    var layer = (convLayers[l] as IMaxPoolingLayer);
                    var prevLayer = convLayers[l - 1];

                    Parallel.For(0, layer.NeuronsCount, (int n) =>
                    //for (var n = 0; n < layer.NeuronsCount; n++)
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

                        if (prevLayer is IConvolutionalLayer)
                        {
                            prevB[n] *= (prevLayer as IConvolutionalLayer).Neurons[n].Function.Derivative;
                        }
                    });
                }
            }
        }

        private void CalcLastConvErrors(IMatrixLayer[] convLayers)
        {
            if (convLayers.Length == 0)
                return;
            
            var layerIndex = convLayers.Length - 1;
            
            if (convLayers[layerIndex].Type == LayerType.MaxPoolingLayer)
            {
                var curLayer = (IMaxPoolingLayer) convLayers[layerIndex];

                Parallel.For(0, curLayer.NeuronsCount, nIndex =>
                {
                    var outputs = curLayer.Neurons[nIndex].Outputs;
                    var outputHeight = outputs.GetLength(0);
                    var outputWidth = outputs.GetLength(1);

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new Matrix(new double[outputHeight, outputWidth]);
                    var errors = convNeuronErrors[convLayers.Length - 1][nIndex];

                    for (var y = 0; y < outputHeight; y++)
                    {
                        for (var x = 0; x < outputWidth; x++)
                        {
                            if (curLayer.Neurons[nIndex].OutputCords[y, x])
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
            }
            
            if (convLayers[layerIndex].Type == LayerType.Convolution)
            {
                var layer1 = (IConvolutionalLayer) convLayers[layerIndex];

                Parallel.For(0, layer1.NeuronsCount, nIndex =>
                {
                    var outputHeight = layer1.Neurons[nIndex].Output.GetLength(0);
                    var outputWidth = layer1.Neurons[nIndex].Output.GetLength(1);
                    var outputs = layer1.Neurons[nIndex].Output;
                    var activationFunction = layer1.Neurons[nIndex].Function;

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new Matrix(new double[outputHeight, outputWidth]);
                    var errors = convNeuronErrors[convLayers.Length - 1][nIndex];

                    for (var y = 0; y < outputHeight; y++)
                    {
                        for (var x = 0; x < outputWidth; x++)
                        {
                            var linerNeuronIndex = (nIndex * outputHeight * outputWidth) + (y * outputs.GetLength(1) + x);
                            var sum = fullyConnectedLayers[0].Neurons
                                .Select((neuron, ni) => new {neuron, ni})
                                .Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                            errors[x, y] = activationFunction.Derivative(outputs[y, x]) * sum;
                        }
                    }
                });
            }
        }
        
        private void UpdateWeightsParallel(double[,] matrix)
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();

            //for (int l = 0; l < convLayers.Length; l++)
            Parallel.For(0, convLayers.Length, l =>
            {
                if (convLayers[l].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];
                    var inputs = l == 0 ? new Matrix(matrix) : convLayers[l - 1].Outputs.Sum();// / convLayers[l - 1].Outputs.Length;

                    for (var e = 0; e < convNeuronErrors[l].Length; e++)
                    {
                        var error = convNeuronErrors[l][e];
                        var weights = layer.Neurons[e].Weights;

                        layer.Neurons[e].Weights = weights + inputs.Сonvolution(error.Rot180() * LearningRate).Rot180();// * layer.Neurons[e].Function.Derivative;
                        layer.Neurons[e].Bias +=
                            layer.Neurons[e].Function is ActivationFunctions.BipolarSigmoid
                                ? 0
                                : layer.Neurons[e].Function.Derivative(error.Sum()) * LearningRate;
                    }
                }
            });

            var input = convLayers.Length > 0 ? convLayers.Last().Outputs.To1DArray() : matrix.ToLinearArray();
            var layersCount = fullyConnectedLayers.Count();

            Parallel.For(0, layersCount, l =>
            {
                var outputs = l == 0 ? input : fullyConnectedLayers[l - 1].Neurons.Select(x => x.Output).ToArray();
                var layer = fullyConnectedLayers[l];

                Parallel.For(0, layer.NeuronsCount, n =>
                {
                    var neuron = layer.Neurons[n];
                    var weightsLegth = neuron.Weights.Length;
                    var error = fullyConnectedNeuronErrors[l][n];

                    for (var wIndex = 0; wIndex < weightsLegth; wIndex++)
                    {
                        neuron.Weights[wIndex] +=LearningRate * error * outputs[wIndex];
                    }
                });
            });
        }
    }
}