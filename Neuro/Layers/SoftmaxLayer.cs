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
        public float[] Outputs { get; set; }
        public int NeuronsCount => Neurons.Length;
        public FullyConnectedNeuron this[int index] => Neurons[index];
        private IActivationFunction Function { get; }

        public SoftmaxLayer(int neuronsCount)
        {
            Function = new None();
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            Outputs = new float[neuronsCount];
        }

        public SoftmaxLayer(int neuronsCount, int inputsCount)
        {
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];

            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, Function);
            }

            Outputs = new float[neuronsCount];
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

        public float[] Compute(float[] inputs)
        {
            var computed = Neurons.AsParallel().Select(n => n.Compute(inputs)).ToArray();
            var inputsExp = computed.Select(x => (float)Math.Exp(x)).ToArray();
            var denominator = inputsExp.Sum();
            var outputs = computed.Select((x, i) => inputsExp[i] / denominator).ToArray();

            Outputs = outputs;

            return outputs;
        }

        public float Derivative(int neuron)
        {
            float y = Outputs[neuron];
            return y * (1 - y);
        }
    }
}