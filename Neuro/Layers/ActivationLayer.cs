using System;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class ActivationLayer : IFullyConnectedLayer
    {
        public LayerType Type { get; set; } = LayerType.FullyConnected;
        public ActivationNeuron[] Neurons;
        public double[] Outputs;
        public int NeuronsCount => Neurons.Length;
        public ActivationNeuron this[int index] => Neurons[index];
        
        public ActivationLayer(int neuronsCount, int inputsCount, IActivationFunction activationFunction)
        {
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new ActivationNeuron[neuronsCount];
            
            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new ActivationNeuron(inputsCount, activationFunction);
            }

            Outputs = new double[neuronsCount];

        }

        public void Randomize()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Randomize();
            }
        }

        public double[] Compute(double[] inputs)
        {
            Neurons
                .Select((neuron, i) => new {i, neuron})
                .AsParallel()
                .ForAll((item) => { Outputs[item.i] = item.neuron.Compute(inputs); });

            return Outputs;
        }
    }
}