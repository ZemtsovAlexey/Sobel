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
        }

        private void CalculateConvLayersError(double[,] desiredOutput)
        {
            IConvolutionalLayer layer, layerNext;
            double[] output, errors, errorsNext;

            layer = network.ConvLayers.Last();

            for (var j = network.ConvLayers.Length - 1; j >= 0; j--)
            {
                layer = network.ConvLayers[j];
            }

            Parallel.For(0, layer.NeuronsCount, (int i) =>
            {
                var kernelHeight = layer.Neurons[i].Weights.GetLength(0);
                var kernelWidth = layer.Neurons[i].Weights.GetLength(1);
                var inputHeight = layer.Neurons[i].Input.GetLength(0) - kernelHeight;
                var inputWidth = layer.Neurons[i].Input.GetLength(1) - kernelWidth;
                
                for (var y = 0; y < inputHeight; y++)
                {
                    for (var x = 0; x < inputWidth; x++)
                    {
                        for (int h = 0; h < kernelHeight; h++)
                        {
                            for (int w = 0; w < kernelWidth; w++)
                            {
                                convNeuronErrors[network.ConvLayers.Length - 1][i][h, w] += layer.Neurons[i].Weights[h, w] * desiredOutput[h, w];
                            }
                        }
                    }
                }
            });
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
    }
}