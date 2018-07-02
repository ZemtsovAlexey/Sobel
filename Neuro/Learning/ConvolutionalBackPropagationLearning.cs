using System.Linq;
using System.Threading.Tasks;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;

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
                convNeuronErrors[i] = new double[convLayers[i].NeuronsCount][,];
            }
        }

        public double Run(double[,] input, double[] output)
        {
            network.Compute(input);
            CalculateError(output);
//            UpdateWeightsParallel(input);

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
            
            CalculateConvLayersError();
        }

        private void CalculateConvLayersError()
        {
            IConvolutionalLayer layer, layerNext;
            double[,] errorsNext;
            double[][,] layerError = new double[network.ConvLayers.Length][,];
            
            layer = network.ConvLayers.Last();
            var firstFullyConnectedLayer = network.FullyConnectedLayers[0];
            
            //расчет ошибки на слое соединия с полносвязной сетью
            for (int nIndex = 0; nIndex < layer.NeuronsCount; nIndex++)
            {
                var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);

                convNeuronErrors[network.ConvLayers.Length - 1][nIndex] = new double[outputHeight, outputWidth];

                for (int y = 0; y < outputHeight; y++)
                {
                    for (int x = 0; x < outputWidth; x++)
                    {
                        var linerNeuronIndex = (nIndex * layer.NeuronsCount) + (y * layer.Neurons[nIndex].Output.GetLength(1) + x);
                        var sum = firstFullyConnectedLayer.Neurons.Select((neuron, ni) => new { neuron, ni }).Sum(j => j.neuron.Weights[linerNeuronIndex] * fullyConnectedNeuronErrors[0][j.ni]);

                        convNeuronErrors[network.ConvLayers.Length - 1][nIndex][x, y] = layer.Neurons[nIndex].Function.Derivative(layer.Neurons[nIndex].Output[y, x]) * sum;
                    }
                }
            }

            for (int lIndex = network.ConvLayers.Length - 2; lIndex >= 0; lIndex--)
            {
                layer = network.ConvLayers[lIndex];
                layerNext = network.ConvLayers[lIndex + 1];
                layerError[lIndex] = new double[layer.Outputs[0].GetLength(0), layer.Outputs[0].GetLength(1)];

                for (int nIndex = 0; nIndex < layerNext.NeuronsCount; nIndex++)
                {
                    errorsNext = convNeuronErrors[lIndex + 1][nIndex];

                    var errorHeight = errorsNext.GetLength(0);
                    var errorWidth = errorsNext.GetLength(1);
                    
                    convNeuronErrors[lIndex][nIndex] = new double[errorHeight + 2, errorWidth + 2];

                    //сканируем карту ошибок предыдущего слоя перевернутым ядром
                    for (var y = -1; y < errorHeight; y++)
                    {
                        for (var x = -1; x < errorWidth; x++)
                        {
                            //rotate kernel to 180 degrees
                            for (var h = layerNext.KernelSize - 1; h >= 0; h--)
                            {
                                for (var w = layerNext.KernelSize - 1; w >= 0; w--)
                                {
                                    if (y + h < 0 || y + h >= errorHeight || x + w < 0 || x + w >= errorWidth)
                                        continue;
						
                                    convNeuronErrors[lIndex][nIndex][y + 1, x + 1] += errorsNext[y + h, x + w] * layerNext.Neurons[nIndex].Weights[h, w];
                                }
                            }
                        }
                    }
                }
                
                //суммируем ошибки
                for (int nIndex = 0; nIndex < convNeuronErrors[lIndex].Length; nIndex++)
                {
                    for (int y = 0; y < convNeuronErrors[lIndex][nIndex].GetLength(0); y++)
                    {
                        for (int x = 0; x < convNeuronErrors[lIndex][nIndex].GetLength(1); x++)
                        {
                            layerError[lIndex][y, x] += convNeuronErrors[lIndex][nIndex][y, x];
                        }
                    }
                }

                for (var nIndex = 0; nIndex < layer.NeuronsCount; nIndex++)
                {
                    var outputHeight = layer.Neurons[nIndex].Output.GetLength(0);
                    var outputWidth = layer.Neurons[nIndex].Output.GetLength(1);
                    
                    convNeuronErrors[lIndex][nIndex] = convNeuronErrors[network.ConvLayers.Length - 1][nIndex] = new double[outputHeight, outputWidth];

                    for (int y = 0; y < convNeuronErrors[lIndex][nIndex].GetLength(0); y++)
                    {
                        for (int x = 0; x < convNeuronErrors[lIndex][nIndex].GetLength(1); x++)
                        {
                            convNeuronErrors[lIndex][nIndex][y, x] = layer.Neurons[nIndex].Function.Derivative(layer.Neurons[nIndex].Output[y, x]) * layerError[lIndex][y, x]; 
                        }
                    }
                }
            }
        }
        
        private void UpdateWeightsParallel(double[] input)
        {
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

        private static double GetValueOrZero(double[,] data, int y, int x)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);

            return y <= 0 || y >= height || x <= 0 || x >= width ? 0 : data[y, x];
        }
    }
}