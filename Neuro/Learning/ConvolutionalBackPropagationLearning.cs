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
        private float[][] fullyConnectedNeuronErrors;
        private float[][][,] convNeuronErrors;
        private IFullyConnectedLayer[] fullyConnectedLayers;

        public float LearningRate { get; set; } = 0.05f;

        public ConvolutionalBackPropagationLearning(ConvolutionalNetwork network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();

            fullyConnectedNeuronErrors = new float[fullyConnectedLayers.Length][];
            convNeuronErrors = new float[matrixLayers.Length][][,];

            for (var i = 0; i < fullyConnectedLayers.Length; i++)
            {
                fullyConnectedNeuronErrors[i] = new float[fullyConnectedLayers[i].NeuronsCount];
            }
            
            for (var i = 0; i < matrixLayers.Length; i++)
            {
                convNeuronErrors[i] = new float[matrixLayers[i == matrixLayers.Length - 1 ? i : i + 1].Outputs.Length][,];
            }
        }

        public float Run(float[,] input, float[] output)
        {
//            network.Compute(input);
            CalculateFullyConnectedLayersError(output);
            UpdateWeightsParallel(input);

            var lastErrors = fullyConnectedNeuronErrors.Last();

            return lastErrors.Sum(x => Math.Abs(x)) / lastErrors.Length;
        }
        
        private void CalculateFullyConnectedLayersError(float[] desiredOutput)
        {        
            IFullyConnectedLayer layer, layerNext;
            float[] output, errors, errorsNext;

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
            
            float[,] errorsNext;
            
            //расчет ошибки на слое соединия с полносвязной сетью
            CalcLastConvErrors(convLayers);

            //расчет ошибки на внутренних слоях включая первый
            for (int l = convLayers.Length - 2; l >= 0; l--)
            {
                var currError = convNeuronErrors[l];
                var nextError = convNeuronErrors[l + 1];

                if (convLayers[l + 1].Type == LayerType.Convolution)
                {
                    var layerNext = (IConvolutionalLayer)convLayers[l + 1];

                    Parallel.For(0, layerNext.NeuronsCount, (int n) => {
                        errorsNext = nextError[n];
    
                        var weights = layerNext.Neurons[n].Weights;
                        var errorHeight = errorsNext.GetLength(0);
                        var errorWidth = errorsNext.GetLength(1);
                        var weightsHeight = weights.GetLength(0);
                        var weightsWidth = weights.GetLength(1);
                        var outStepY = weightsHeight - 1;
                        var outStepX = weightsWidth - 1;

                        currError[n] = new float[errorHeight + weightsHeight - 1, errorWidth + weightsWidth - 1];
                        var error = currError[n];

                        //сканируем карту ошибок предыдущего слоя перевернутым ядром
                        for (var y = -(outStepY); y < errorHeight + outStepY - 1; y++)
                        {
                            for (var x = -(outStepX); x < errorWidth + outStepX - 1; x++)
                            {
                                //rotate kernel to 180 degrees
                                for (var h = y < 0 ? 0 - y : 0; h < (y + weightsHeight > errorHeight ? (errorHeight - (y + weightsHeight)) + weightsHeight : weightsHeight); h++)
                                {
                                    for (var w = x < 0 ? 0 - x : 0; w < (x + weightsWidth > errorWidth ? (errorWidth - (x + weightsWidth)) + weightsWidth : weightsWidth); w++)
                                    {
                                        error[y + outStepY, x + outStepX] += errorsNext[y + h, x + w] * weights[weightsHeight - 1 - h, weightsWidth - 1 - w];
                                    }
                                }
                            }
                        }
                    });
                }

                if (convLayers[l + 1].Type == LayerType.MaxPoolingLayer)
                {
                    var layer1 = (IMaxPoolingLayer)convLayers[l + 1];

                    //Parallel.For(0, layer1.NeuronsCount, (int n) =>
                    for (var n = 0; n < layer1.NeuronsCount; n++)
                    {
                        var outputHeight = layer1.Neurons[n].Outputs.GetLength(0);
                        var outputWidth = layer1.Neurons[n].Outputs.GetLength(1);
                        var nextNeuronError = nextError[n];
                        var outputCords = layer1.Neurons[n].OutputCords;

                        convNeuronErrors[l][n] = new float[outputHeight, outputWidth];

                        for (var y = 0; y < outputHeight; y++)
                        {
                            for (var x = 0; x < outputWidth; x++)
                            {
                                if (outputCords[y, x])
                                    currError[n][y, x] = nextNeuronError[y, x];
                            }
                        }
                    }
                }
                
                if (convLayers[l].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];
                    
                    Parallel.For(0, layer.NeuronsCount, (int nIndex) =>
                    {
                        var function = layer.Neurons[nIndex].Function;
                        var outputs = layer.Neurons[nIndex].Output;
                        var errors = convNeuronErrors[l][nIndex];
                        var errorsHeight = errors.GetLength(0);
                        var errorsWidth = errors.GetLength(1);
                        var outputHeight = outputs.GetLength(0);
                        var outputWidth = outputs.GetLength(1);

                        convNeuronErrors[l][nIndex] = new float[outputHeight, outputWidth];

                        for (var y = 0; y < errorsHeight; y++)
                        {
                            for (var x = 0; x < errorsWidth; x++)
                            {
                                errors[y, x] = function.Derivative(outputs[y, x]) * errors[y, x];
                            }
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

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new float[outputHeight, outputWidth];
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

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new float[outputHeight, outputWidth];
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
        
        private void UpdateWeightsParallel(float[,] matrix)
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();

            for (int l = 0; l < convLayers.Length; l++)
            {
                if (convLayers[l].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];
                    var inputs = l == 0 ? matrix : convLayers[l - 1].Outputs.Sum();

                    for (var e = 0; e < convNeuronErrors[l].Length; e++)
                    {
                        var error = convNeuronErrors[l][e];
                        var errorHeight = error.GetLength(0);
                        var errorWidth = error.GetLength(1);

                        var weights = layer.Neurons[e].Weights;
                        var weightsHeight = weights.GetLength(0);
                        var weightsWidth = weights.GetLength(1);
                        var function = layer.Neurons[e].Function;

                        var newWeights = new float[weightsHeight, weightsWidth];

                        for (var y = 0; y < weightsHeight; y++)
                        {
                            for (var x = 0; x < weightsWidth; x++)
                            {
                                for (var h = 0; h < errorHeight; h++)
                                {
                                    for (var w = 0; w < errorWidth; w++)
                                    {
                                        weights[y, x] += inputs[y + h, x + w] * error[errorHeight - 1 - h, errorWidth - 1 - w] * LearningRate;
//                                        newWeights[y, x] += inputs[y + h, x + w] * error[h, w];
                                    }
                                }

//                                newWeights[y, x] *= (LearningRate / network.Layers.Length);
                            }
                        }
//
//                        for (var h = 0; h < weightsHeight; h++)
//                        {
//                            for (var w = 0; w < weightsWidth; w++)
//                            {
//                                newWeights[h, w] = weights[h, w] + newWeights[h, w] * LearningRate;
//                            }
//                        }

//                        layer.Neurons[e].Weights = newWeights;
                    }
                }
            }

            var input = convLayers.Length > 0 ? convLayers.Last().Outputs.ToLinearArray() : matrix.ToLinearArray();
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