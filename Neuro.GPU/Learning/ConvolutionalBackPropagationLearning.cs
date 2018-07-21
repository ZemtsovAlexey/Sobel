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
        private double[][][,] convNeuronErrors;
        private IFullyConnectedLayer[] fullyConnectedLayers;

        public double LearningRate { get; set; } = 0.05f;

        public ConvolutionalBackPropagationLearning(ConvolutionalNetwork network)
        {
            this.network = network;

            var matrixLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            fullyConnectedLayers = network.Layers.Where(x => x.Type == LayerType.FullyConnected).Select(x => x as IFullyConnectedLayer).ToArray();

            fullyConnectedNeuronErrors = new double[fullyConnectedLayers.Length][];
            convNeuronErrors = new double[matrixLayers.Length][][,];

            for (var i = 0; i < fullyConnectedLayers.Length; i++)
            {
                fullyConnectedNeuronErrors[i] = new double[fullyConnectedLayers[i].NeuronsCount];
            }
            
            for (var i = 0; i < matrixLayers.Length; i++)
            {
                convNeuronErrors[i] = new double[matrixLayers[i == matrixLayers.Length - 1 ? i : i + 1].Outputs.Length][,];
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
                errors[i] = (desiredOutput[i] - output[i]) * layer[i].Function.Derivative(output[i]);
            }

            //расчитываем ошибку на скрытых слоях
            for (var j = fullyConnectedLayers.Length - 2; j >= 0; j--)
            {
                layer = fullyConnectedLayers[j];
                layerNext = fullyConnectedLayers[j + 1];
                errors = fullyConnectedNeuronErrors[j];
                errorsNext = fullyConnectedNeuronErrors[j + 1];

                Parallel.For(0, layer.NeuronsCount, (int i) =>
                //for (var i = 0; i < layer.NeuronsCount; i++)
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new {neuron, nIndex}).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    errors[i] = layer[i].Function.Derivative(layer[i].Output) * sum;
                }); 
            }

            CalculateConvLayersErrorParallel();
        }

        private void CalculateConvLayersErrorParallel()
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();
            double[,] errorsNext;
            
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
                        var outStepY = weights.GetLength(0) - 1;
                        var outStepX = weights.GetLength(1) - 1;

                        currError[n] = new double[errorHeight + weights.GetLength(0) - 1, errorWidth + weights.GetLength(1) - 1];
    
                        //сканируем карту ошибок предыдущего слоя перевернутым ядром
                        for (var y = -(outStepY); y < errorHeight + outStepY - 1; y++)
                        {
                            for (var x = -(outStepX); x < errorWidth + outStepX - 1; x++)
                            {
                                //rotate kernel to 180 degrees
                                for (var h = y < 0 ? 0 - y : 0; h < (y + weights.GetLength(0) > errorHeight ? (errorHeight - (y + weights.GetLength(0))) + weights.GetLength(0) : weights.GetLength(0)); h++)
                                {
                                    for (var w = x < 0 ? 0 - x : 0; w < (x + weights.GetLength(1) > errorWidth ? (errorWidth - (x + weights.GetLength(1))) + weights.GetLength(1) : weights.GetLength(1)); w++)
                                    {
                                        currError[n][y + outStepY, x + outStepY] += errorsNext[y + h, x + w] * layerNext.Neurons[n].Weights[layerNext.Neurons[n].Weights.GetLength(0) - 1 - h, layerNext.Neurons[n].Weights.GetLength(1) - 1 - w];
                                    }
                                }
                            }
                        }
                        
                        /*Parallel.For(-(outStepY), errorHeight + outStepY - 1, (int y) => 
                        {
                            Parallel.For(-(outStepX), errorWidth + outStepX - 1, (int x) =>
                            {
                                //rotate kernel to 180 degrees
                                Parallel.For(y < 0 ? 0 - y : 0, (y + weights.GetLength(0) > errorHeight ? (errorHeight - (y + weights.GetLength(0))) + weights.GetLength(0) : weights.GetLength(0)), (int h) =>
                                {
                                    Parallel.For(x < 0 ? 0 - x : 0, (x + weights.GetLength(1) > errorWidth ? (errorWidth - (x + weights.GetLength(1))) + weights.GetLength(1) : weights.GetLength(1)), (int w) =>
                                    {
                                        convNeuronErrors[l][n][y + outStepY, x + outStepY] += errorsNext[y + h, x + w] * layerNext.Neurons[n].Weights[layerNext.Neurons[n].Weights.GetLength(0) - 1 - h, layerNext.Neurons[n].Weights.GetLength(1) - 1 - w];
                                    });
                                });
                            });
                        });*/
                    });
                }

                if (convLayers[l + 1].Type == LayerType.MaxPoolingLayer)
                {
                    var layer1 = (IMaxPoolingLayer)convLayers[l + 1];

                    //for (var nIndex = 0; nIndex < layer1.Neurons.Length; nIndex++)
                    /*Parallel.For(0, layer1.NeuronsCount, nIndex =>
                    {
                        var outputHeight = layer1.Neurons[nIndex].Outputs.GetLength(0);
                        var outputWidth = layer1.Neurons[nIndex].Outputs.GetLength(1);

                        convNeuronErrors[l][nIndex] = new double[outputHeight, outputWidth];

                        for (var y = 0; y < outputHeight; y++)
                        {
                            for (var x = 0; x < outputWidth; x++)
                            {
                                if (layer1.Neurons[nIndex].OutputCords[y, x])
                                    convNeuronErrors[l][nIndex][y, x] = convNeuronErrors[l + 1][nIndex][y, x];
                            }
                        }
                    });*/

                    //for (var nIndex = 0; nIndex < layer1.Neurons.Length; nIndex++)
                    //{
                    //    var outputHeight = layer1.Neurons[nIndex].Outputs.GetLength(0);
                    //    var outputWidth = layer1.Neurons[nIndex].Outputs.GetLength(1);

                    //    currError[nIndex] = new double[outputHeight, outputWidth];

                    //    for (var y = 0; y < outputHeight; y++)
                    //    {
                    //        for (var x = 0; x < outputWidth; x++)
                    //        {
                    //            if (layer1.Neurons[nIndex].OutputCords[y, x])
                    //                currError[nIndex][y, x] = nextError[nIndex][y, x];
                    //        }
                    //    }
                    //}
                    //Parallel.For(0, layer1.NeuronsCount, (int n) =>
                    for (var n = 0; n < layer1.NeuronsCount; n++)
                    {
                        var outputHeight = layer1.Neurons[n].Outputs.GetLength(0);
                        var outputWidth = layer1.Neurons[n].Outputs.GetLength(1);
                        var nextNeuronError = nextError[n];
                        var outputCords = layer1.Neurons[n].OutputCords;

                        convNeuronErrors[l][n] = new double[outputHeight, outputWidth];

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
                    
                    //применяем функцию активации
                    //Parallel.For(0, layer.NeuronsCount, (int nIndex) => 
                    //{
                    //    var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                    //    var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);
        
                    //    convNeuronErrors[l][nIndex] = new double[outputHeight, outputWidth];

                    //    for (var y = 0; y < convNeuronErrors[l][nIndex].GetLength(0); y++)
                    //    {
                    //        for (var x = 0; x < convNeuronErrors[l][nIndex].GetLength(1); x++)
                    //        {
                    //            convNeuronErrors[l][nIndex][y, x] = layer.Neurons[nIndex].Function.Derivative(layer.Neurons[nIndex].Output[y, x]) * convNeuronErrors[l][nIndex][y, x];
                    //        }
                    //    }
                    //});

                    Parallel.For(0, layer.NeuronsCount, (int nIndex) =>
                    {
                        var function = layer.Neurons[nIndex].Function;
                        var outputs = layer.Neurons[nIndex].Output;
                        var errors = convNeuronErrors[l][nIndex];
                        var errorsHeight = errors.GetLength(0);
                        var errorsWidth = errors.GetLength(1);
                        var outputHeight = outputs.GetLength(0);
                        var outputWidth = outputs.GetLength(1);

                        convNeuronErrors[l][nIndex] = new double[outputHeight, outputWidth];

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
            var layerIndex = convLayers.Length - 1;
            
            if (convLayers[layerIndex].Type == LayerType.MaxPoolingLayer)
            {
                var layer1 = (IMaxPoolingLayer) convLayers[layerIndex];

                //for (var nIndex = 0; nIndex < layer1.Neurons.Length; nIndex++)
                Parallel.For(0, layer1.NeuronsCount, (int nIndex) =>
                {
                    var outputHeight = layer1.Neurons[nIndex].Outputs.GetLength(0);
                    var outputWidth = layer1.Neurons[nIndex].Outputs.GetLength(1);

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new double[outputHeight, outputWidth];

                    Parallel.For(0, outputHeight, y =>
                    {
                        Parallel.For(0, outputWidth, x =>
                        {
                            if (layer1.Neurons[nIndex].OutputCords[y, x])
                            {
                                var linerNeuronIndex = (nIndex * outputHeight * outputWidth) + (y * layer1.Neurons[nIndex].Outputs.GetLength(1) + x);
                                var sum = fullyConnectedLayers[0].Neurons.Select((neuron, ni) => new { neuron, ni }).Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                                convNeuronErrors[convLayers.Length - 1][nIndex][y, x] = sum;
                            }
                        });
                    });
                });
            }
            
            if (convLayers[layerIndex].Type == LayerType.Convolution)
            {
                var layer1 = (IConvolutionalLayer) convLayers[layerIndex];

                Parallel.For(0, layer1.NeuronsCount, (int nIndex) =>
                {
                    var outputHeight = layer1.Neurons[nIndex].Output.GetLength(0);
                    var outputWidth = layer1.Neurons[nIndex].Output.GetLength(1);

                    convNeuronErrors[convLayers.Length - 1][nIndex] = new double[outputHeight, outputWidth];

                    Parallel.For(0, outputHeight, y =>
                    {
                        Parallel.For(0, outputWidth, x =>
                        {
                            var linerNeuronIndex = (nIndex * outputHeight * outputWidth) + (y * layer1.Neurons[nIndex].Output.GetLength(1) + x);
                            var sum = fullyConnectedLayers[0].Neurons.Select((neuron, ni) => new {neuron, ni}).Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                            convNeuronErrors[convLayers.Length - 1][nIndex][x, y] = layer1.Neurons[nIndex].Function.Derivative(layer1.Neurons[nIndex].Output[y, x]) * sum;
                        });
                    });
                });
            }
        }
        
        private void UpdateWeightsParallel(double[,] matrix)
        {
            var convLayers = network.Layers.Where(x => x.Type == LayerType.Convolution || x.Type == LayerType.MaxPoolingLayer).Select(x => x as IMatrixLayer).ToArray();

            //проходимся по всем слоям сверточной сети
            //Parallel.For(0, convLayers.Length, (int l) =>
            //{
            //    if (convLayers[l].Type == LayerType.Convolution)
            //    {
            //        var layer = (IConvolutionalLayer)convLayers[l];

            //        Parallel.For(0, convNeuronErrors[l].Length, (int e) =>
            //        {
            //            //проход по ошибкам
            //            Parallel.For(0, layer.NeuronsCount, (int n) =>
            //            {
            //                Parallel.For(0, convNeuronErrors[l][e].GetLength(0), (int y) =>
            //                {
            //                    Parallel.For(0, convNeuronErrors[l][e].GetLength(1), (int x) =>
            //                    {
            //                        //проход по весам
            //                        Parallel.For(0, layer.Neurons[n].Weights.GetLength(0), (int h) =>
            //                        {
            //                            Parallel.For(0, layer.Neurons[n].Weights.GetLength(1), (int w) =>
            //                            {
            //                                layer.Neurons[n].Weights[h, w] +=
            //                                    LearningRate *
            //                                    convNeuronErrors[l][e][y, x] *
            //                                    layer.Neurons[n].Output[y, x];
            //                            });
            //                        });
            //                    });
            //                });
            //            });
            //        });
            //    }
            //});

            Parallel.For(0, convLayers.Length, (int l) =>
            {
                int e, n, x, y, h, w;

                if (convLayers[l].Type == LayerType.Convolution)
                {
                    var layer = (IConvolutionalLayer)convLayers[l];

                    //Parallel.For(0, convNeuronErrors[l].Length, (int e) =>
                    for (e = 0; e < convNeuronErrors[l].Length; e++)
                    {
                        var error = convNeuronErrors[l][e];
                        var errorHeigth = error.GetLength(0);
                        var errorWidth = error.GetLength(1);

                        //проход по ошибкам
                        //Parallel.For(0, layer.NeuronsCount, (int n) =>
                        for (n = 0; n < layer.NeuronsCount; n++)
                        {
                            var outputs = layer.Neurons[n].Output;
                            var weights = layer.Neurons[n].Weights;
                            var weightsHeight = weights.GetLength(0);
                            var weightsWidth = weights.GetLength(1);

                            for (y = 0; y < errorHeigth; y++)
                            {
                                for (x = 0; x < errorWidth; x++)
                                {
                                    //проход по весам
                                    for (h = 0; h < weightsHeight; h++)
                                    {
                                        for (w = 0; w < weightsWidth; w++)
                                        {
                                            weights[h, w] += LearningRate * error[y, x] * outputs[y, x];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            var input = convLayers.Last().Outputs.ToLinearArray();

            Parallel.For(0, fullyConnectedLayers.Count(), l =>
            {
                var outputs = l == 0 ? input : fullyConnectedLayers[l - 1].Neurons.Select(x => x.Output).ToArray();

                Parallel.For(0, fullyConnectedLayers[l].Neurons.Count(), n =>
                {
                    var weights = fullyConnectedLayers[l].Neurons[n].Weights;

                    for (var wIndex = 0; wIndex < weights.Length; wIndex++)
                    {
                        weights[wIndex] += LearningRate * fullyConnectedNeuronErrors[l][n] * outputs[wIndex];
                    }
                });
            });

            //var layers = fullyConnectedLayers.Select((layer, i) => new { layer, Index = i }).AsParallel();

            //layers.ForAll(layerEl =>
            //{
            //    var outputs = layerEl.Index == 0 ? input : fullyConnectedLayers[layerEl.Index - 1].Neurons.Select(x => x.Output).ToArray();

            //    layerEl.layer.Neurons
            //        .AsParallel()
            //        .Select((neuron, i) => new { neuron, Index = i })
            //        .ForAll(neuronEl =>
            //        {
            //            var weights = neuronEl.neuron.Weights;

            //            for (var wIndex = 0; wIndex < weights.Length; wIndex++)
            //            {
            //                weights[wIndex] += LearningRate * fullyConnectedNeuronErrors[layerEl.Index][neuronEl.Index] * outputs[wIndex];
            //            }
            //        });
            //});
        }
    }
}