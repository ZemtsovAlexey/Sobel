using System;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class SoftmaxLayer : ISoftmaxLayer
    {
        public int Index { get; private set; }
        public LayerType Type { get; set; } = LayerType.Softmax;
        public FullyConnectedNeuron[] Neurons { get; }
        public double[] Outputs { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public FullyConnectedNeuron this[int index] => Neurons[index];
        private IActivationFunction Function { get; }

        public SoftmaxLayer(int neuronsCount)
        {
            Function = new None();
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            Outputs = new double[neuronsCount];
        }

        public SoftmaxLayer(int neuronsCount, int inputsCount)
        {
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];

            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, Function);
            }

            Outputs = new double[neuronsCount];
        }

        public void Init(int index, int inputsCount)
        {
            Index = index;

            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, Function);
            }
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
            var computed = Neurons.AsParallel().Select(n => n.Compute(inputs)).ToArray();
            var inputsExp = computed.Select(x => Math.Exp(x)).ToArray();
            var denominator = inputsExp.Sum();
            var outputs = computed.Select((x, i) => inputsExp[i] / denominator).ToArray();

            Outputs = outputs;

            return outputs;
        }

        public double Derivative(int neuron)
        {
            double y = Outputs[neuron];
            return y * (1 - y);
        }
    }
}