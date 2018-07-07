using System.Linq;
using Neuro.Layers;
using Neuro.Networks;
using Neuro.Neurons;

namespace Neuro.Learning
{
    public class BackPropagationLearning : ILearning
    {
        private ActivationNetwork network;
        private double[][] neuronErrors;

        public double LearningRate { get; set; } = 0.05f;

        public BackPropagationLearning(ActivationNetwork network)
        {
            this.network = network;

            neuronErrors = new double[network.LayersCount][];

            for (var i = 0; i < network.LayersCount; i++)
            {
                neuronErrors[i] = new double[network[i].NeuronsCount];
            }
        }

        public double Run(double[] input, double[] output)
        {
            network.Compute(input);
            CalculateError(output);
            UpdateWeightsParallel(input);

            return 0;
        }

        private void CalculateError(double[] desiredOutput)
        {        
            FullyConnectedLayer layer, layerNext;
            double[] output, errors, errorsNext;

            layer = network[network.LayersCount - 1];
            errors = neuronErrors[network.LayersCount - 1];
            output = layer.Outputs;
            
            for (var i = 0; i < layer.NeuronsCount; i++)
            {
                errors[i] = (desiredOutput[i] - output[i]) * layer[i].Function.Derivative(output[i]);
            }

            for (var j = network.LayersCount - 2; j >= 0; j--)
            {
                layer = network[j];
                layerNext = network[j + 1];
                errors = neuronErrors[j];
                errorsNext = neuronErrors[j + 1];
                
                for (var i = 0; i < layer.NeuronsCount; i++)
                {
                    var sum = layerNext.Neurons.Select((neuron, nIndex) => new {neuron, nIndex}).Sum(x => x.neuron.Weights[i] * errorsNext[x.nIndex]);
                    errors[i] = layer[i].Function.Derivative(layer[i].Output) * sum;
                } 
            }
        }
        
        private void UpdateWeights(double[] input)
        {
            int lIndex, nIndex, wIndex;
            FullyConnectedNeuron[] neurons;
            double[] weights;
            double[] outputs = null;

            for (lIndex = 0; lIndex < network.Layers.Length; lIndex++)
            {
                neurons = network.Layers[lIndex].Neurons;
                outputs = lIndex == 0 ? input : network.Layers[lIndex - 1].Neurons.Select(x => x.Output).ToArray();

                for (nIndex = 0; nIndex < neurons.Length; nIndex++)
                {
                    weights = neurons[nIndex].Weights;

                    for (wIndex = 0; wIndex < weights.Length; wIndex++)
                    {
                        weights[wIndex] += LearningRate * neuronErrors[lIndex][nIndex] * outputs[wIndex];
                    }
                }
            }
        }
        
        private void UpdateWeightsParallel(double[] input)
        {
            var layers = network.Layers.Select((layer, i) => new { layer, Index = i }).AsParallel();
            
            layers.ForAll(layerEl =>
            {
                var outputs = layerEl.Index == 0 ? input : network.Layers[layerEl.Index - 1].Neurons.Select(x => x.Output).ToArray();
                
                layerEl.layer.Neurons
                    .Select((neuron, i) => new {neuron, Index = i})
                    .AsParallel()
                    .ForAll(neuronEl =>
                    {
                        var weights = neuronEl.neuron.Weights;

                        for (var wIndex = 0; wIndex < weights.Length; wIndex++)
                        {
                            weights[wIndex] += LearningRate * neuronErrors[layerEl.Index][neuronEl.Index] * outputs[wIndex];
                        }
                    });
            });
        }
    }
}