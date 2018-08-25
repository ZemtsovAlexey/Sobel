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
        private Matrix[][] convNeuronErrors;
        private IFullyConnectedLayer[] fullyConnectedLayers;

        public float LearningRate { get; set; } = 0.05f;

        public ConvolutionalBackPropagationLearning(ConvolutionalNetwork network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();

            fullyConnectedNeuronErrors = new float[fullyConnectedLayers.Length][];
            convNeuronErrors = new Matrix[matrixLayers.Length][];

            for (var i = 0; i < fullyConnectedLayers.Length; i++)
            {
                fullyConnectedNeuronErrors[i] = new float[fullyConnectedLayers[i].NeuronsCount];
            }
            
            for (var i = 0; i < matrixLayers.Length; i++)
            {
                convNeuronErrors[i] = new Matrix[matrixLayers[i == matrixLayers.Length - 1 ? i : i + 1].Outputs.Length];
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
            
            //расчет ошибки на слое соединия с полносвязной сетью
            CalcLastConvErrors(convLayers);

            for (int l = convLayers.Length - 1; l > 0; l--)
            {
                if (convLayers[l - 1].Type == LayerType.Convolution)
                {
                    Parallel.For(0, layerNext.NeuronsCount, (int n) => {
                        var function = layer.Neurons[n].Function;

                        currError[n] = layer.Neurons[n].Output * function.Derivative * nextError.Sum().BackСonvolution(layerNext.Neurons[n].Weights.Rot180());
                    });
                }
            }

            //расчет ошибки на внутренних слоях включая первый
            for (int l = convLayers.Length - 2; l >= 0; l--)
            {
                var currError = convNeuronErrors[l];
                var nextError = convNeuronErrors[l + 1];

                if (convLayers[l + 1].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];
                    var layerNext = (IConvolutionalLayer)convLayers[l + 1];

                    Parallel.For(0, layerNext.NeuronsCount, (int n) => {
                        var function = layer.Neurons[n].Function;

                        currError[n] = layer.Neurons[n].Output * function.Derivative * nextError.Sum().BackСonvolution(layerNext.Neurons[n].Weights.Rot180());
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

                        convNeuronErrors[l][n] = new Matrix(new float[outputHeight, outputWidth]);

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
                
                //if (convLayers[l].Type == LayerType.Convolution)
                //{
                //    var layer = (IConvolutionalLayer)convLayers[l];
                    
                //    Parallel.For(0, layer.NeuronsCount, (int nIndex) =>
                //    {
                //        var function = layer.Neurons[nIndex].Function;
                //        var outputs = layer.Neurons[nIndex].Output;
                //        var errors = convNeuronErrors[l][nIndex];

                //        convNeuronErrors[l][nIndex] = (outputs * function.Derivative) * errors;
                //    });
                //}
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

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new Matrix(new float[outputHeight, outputWidth]);
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

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new Matrix(new float[outputHeight, outputWidth]);
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
                    var inputs = l == 0 ? new Matrix(matrix) : convLayers[l - 1].Outputs.Sum();

                    for (var e = 0; e < convNeuronErrors[l].Length; e++)
                    {
                        var error = convNeuronErrors[l][e];
                        var weights = layer.Neurons[e].Weights;

                        layer.Neurons[e].Weights = weights + (inputs.Сonvolution(error.Rot180(), 1) * LearningRate);
                    }
                }
            }

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