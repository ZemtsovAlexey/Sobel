using System.Linq;
using System.Threading.Tasks;
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

        public double LearningRate { get; set; } = 0.05f;

        public ConvolutionalBackPropagationLearning(ConvolutionalNetwork network)
        {
            this.network = network;

            var convLayers = network.ConvLayers.Where(x => x.Type == LayerType.Convolution).ToList();

            fullyConnectedNeuronErrors = new double[network.FullyConnectedLayers.Length][];
            convNeuronErrors = new double[convLayers.Count][][,];

            for (var i = 0; i < network.FullyConnectedLayers.Length; i++)
            {
                fullyConnectedNeuronErrors[i] = new double[network.FullyConnectedLayers[i].NeuronsCount];
            }
            
            for (var i = 0; i < convLayers.Count; i++)
            {
                convNeuronErrors[i] = new double[convLayers[i == convLayers.Count - 1 ? i : i + 1].Outputs.Length][,];
            }
        }

        public double Run(double[,] input, double[] output)
        {
            network.Compute(input);
            CalculateError(output);
            UpdateWeightsParallel(input);

            return 0;
        }
        
        private void CalculateError(double[] desiredOutput)
        {        
            IFullyConnectedLayer layer, layerNext;
            double[] output, errors, errorsNext;

            layer = network.FullyConnectedLayers[network.FullyConnectedLayers.Length - 1];
            errors = fullyConnectedNeuronErrors[network.FullyConnectedLayers.Length - 1];
            output = layer.Outputs;
            
            //расчитываем ошибку на последнем слое
            for (var i = 0; i < layer.NeuronsCount; i++)
            {
                errors[i] = (desiredOutput[i] - output[i]) * layer[i].Function.Derivative(output[i]);
            }

            //расчитываем ошибку на скрытых слоях
            for (var j = network.FullyConnectedLayers.Length - 2; j >= 0; j--)
            {
                layer = network.FullyConnectedLayers[j];
                layerNext = network.FullyConnectedLayers[j + 1];
                errors = fullyConnectedNeuronErrors[j];
                errorsNext = fullyConnectedNeuronErrors[j + 1];
                
                for (var i = 0; i < layer.NeuronsCount; i++)
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new {neuron, nIndex}).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    errors[i] = layer[i].Function.Derivative(layer[i].Output) * sum;
                } 
            }

            CalculateConvLayersErrorParallel();
        }

        private void CalculateConvLayersErrorParallel()
        {
            IConvolutionalLayer layer, layerNext;
            double[,] errorsNext;

            layer = network.ConvLayers.Last();
            var firstFullyConnectedLayer = network.FullyConnectedLayers[0];

            //расчет ошибки на слое соединия с полносвязной сетью
            var layer1 = layer;
            Parallel.For(0, layer.NeuronsCount, (int nIndex) => {
                var outputHeight = layer1.Neurons[nIndex].Output.GetLength(0);
                var outputWidth = layer1.Neurons[nIndex].Output.GetLength(1);

                convNeuronErrors[network.ConvLayers.Length - 1][nIndex] = new double[outputHeight, outputWidth];

                Parallel.For(0, outputHeight, (int y) => {
                    Parallel.For(0, outputWidth, (int x) => {
                        var linerNeuronIndex = (nIndex * outputHeight * outputWidth) + (y * layer1.Neurons[nIndex].Output.GetLength(1) + x);
                        var sum = firstFullyConnectedLayer.Neurons.Select((neuron, ni) => new { neuron, ni }).Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                        convNeuronErrors[network.ConvLayers.Length - 1][nIndex][x, y] = layer1.Neurons[nIndex].Function.Derivative(layer1.Neurons[nIndex].Output[y, x]) * sum;
                    });
                });
            });

            for (int lIndex = network.ConvLayers.Length - 2; lIndex >= 0; lIndex--)
            {
                layer = network.ConvLayers[lIndex];
                layerNext = network.ConvLayers[lIndex + 1];

                Parallel.For(0, layerNext.NeuronsCount, (int nIndex) => {
                    errorsNext = convNeuronErrors[lIndex + 1][nIndex];

                    var weights = layerNext.Neurons[nIndex].Weights;
                    var errorHeight = errorsNext.GetLength(0);
                    var errorWidth = errorsNext.GetLength(1);
                    var outStepY = weights.GetLength(0) - 1;
                    var outStepX = weights.GetLength(1) - 1;

                    convNeuronErrors[lIndex][nIndex] = new double[errorHeight + weights.GetLength(0) - 1, errorWidth + weights.GetLength(1) - 1];

                    //сканируем карту ошибок предыдущего слоя перевернутым ядром
                    Parallel.For(-(outStepY), errorHeight + outStepY - 1, (int y) => {
                        Parallel.For(-(outStepX), errorWidth + outStepX - 1, (int x) => {
                            //rotate kernel to 180 degrees
                            for (var h = y < 0 ? 0 - y : 0; h < (y + weights.GetLength(0) > errorHeight ? (errorHeight - (y + weights.GetLength(0))) + weights.GetLength(0) : weights.GetLength(0)); h++)
                            {
                                for (var w = x < 0 ? 0 - x : 0; w < (x + weights.GetLength(1) > errorWidth ? (errorWidth - (x + weights.GetLength(1))) + weights.GetLength(1) : weights.GetLength(1)); w++)
                                {
                                    convNeuronErrors[lIndex][nIndex][y + outStepY, x + outStepY] += errorsNext[y + h, x + w] * layerNext.Neurons[nIndex].Weights[layerNext.Neurons[nIndex].Weights.GetLength(0) - 1 - h, layerNext.Neurons[nIndex].Weights.GetLength(1) - 1 - w];
                                }
                            }
                        });
                    });
                });

                //применяем функцию активации
                Parallel.For(0, layer.NeuronsCount, (int nIndex) => {
                    var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                    var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);

                    convNeuronErrors[lIndex][nIndex] = new double[outputHeight, outputWidth];

                    Parallel.For(0, convNeuronErrors[lIndex][nIndex].GetLength(0), (int y) =>
                    {
                        Parallel.For(0, convNeuronErrors[lIndex][nIndex].GetLength(1), (int x) => {
                            convNeuronErrors[lIndex][nIndex][y, x] = layer.Neurons[nIndex].Function.Derivative(layer.Neurons[nIndex].Output[y, x]) * convNeuronErrors[lIndex][nIndex][y, x];
                        });
                    });
                });
            }
        }

        private void UpdateWeightsParallel(double[,] matrix)
        {
            var convLayers = network.ConvLayers;
            
            //проходимся по всем слоям сверточной сети
//            Parallel.For(0, convLayers.Length, (int l) =>
//            {
//                Parallel.For(0, convNeuronErrors[l].Length, (int e) =>
//                {
//                    //проход по ошибкам
//                    Parallel.For(0, convLayers[l].NeuronsCount, (int n) =>
//                    {
//                        Parallel.For(0, convNeuronErrors[l][e].GetLength(0), (int y) =>
//                        {
//                            Parallel.For(0, convNeuronErrors[l][e].GetLength(1), (int x) =>
//                            {
//                                //проход по весам
//                                Parallel.For(0, convLayers[l].Neurons[n].Weights.GetLength(0), (int h) =>
//                                {
//                                    Parallel.For(0, convLayers[l].Neurons[n].Weights.GetLength(1), (int w) =>
//                                    {
//                                        convLayers[l].Neurons[n].Weights[h, w] +=
//                                            LearningRate *
//                                            convNeuronErrors[l][e][y, x] *
//                                            convLayers[l].Neurons[n].Output[y, x];
//                                    });
//                                });
//                            });
//                        });
//                    });
//                });
//            });
            
            for (var l = 0; l < convLayers.Length; l++)
            {
                for (var e = 0; e < convNeuronErrors[l].Length; e++)
                {
                    //проход по ошибкам
                    for (var n = 0; n < convLayers[l].NeuronsCount; n++)
                    {
                        for (int y = 0; y < convNeuronErrors[l][e].GetLength(0); y++)
                        {
                            for (int x = 0; x < convNeuronErrors[l][e].GetLength(1); x++)
                            {
                                //проход по весам
                                for (var h = 0; h < convLayers[l].Neurons[n].Weights.GetLength(0); h++)
                                {
                                    for (var w = 0; w < convLayers[l].Neurons[n].Weights.GetLength(0); w++)
                                    {
                                        convLayers[l].Neurons[n].Weights[h, w] += LearningRate * convNeuronErrors[l][e][y, x] * convLayers[l].Neurons[n].Output[y, x];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            var input = convLayers.Last().GetLinereOutput();
            var layers = network.FullyConnectedLayers.Select((layer, i) => new { layer, Index = i }).AsParallel();
            
            layers.ForAll(layerEl =>
            {
                var outputs = layerEl.Index == 0 ? input : network.FullyConnectedLayers[layerEl.Index - 1].Neurons.Select(x => x.Output).ToArray();
                
                layerEl.layer.Neurons
                    .Select((neuron, i) => new {neuron, Index = i})
                    .AsParallel()
                    .ForAll(neuronEl =>
                    {
                        var weights = neuronEl.neuron.Weights;

                        for (var wIndex = 0; wIndex < weights.Length; wIndex++)
                        {
                            weights[wIndex] += LearningRate * fullyConnectedNeuronErrors[layerEl.Index][neuronEl.Index] * outputs[wIndex];
                        }
                    });
            });
        }
    }
}