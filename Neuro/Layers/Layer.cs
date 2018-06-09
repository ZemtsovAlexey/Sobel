using System;
using Neuro.Models;
using Neuro.Models.Outputs;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public abstract class Layer : ILayer
    {
        public InputValue Outputs { get; set; }
        public Neuron[] Neurons { get; set; }
        public int NeuronsCount => Neurons.Length;
        public Neuron this[int index] => Neurons[index];
        
        protected Layer()
        {
        }
        
        protected Layer(int neuronsCount)
        {
            neuronsCount = Math.Max(1, neuronsCount);

            Neurons = new Neuron[neuronsCount];
//            Outputs = new double[neuronsCount];
        }

        public LayerType Type { get; set; }
        public abstract void Randomize();

        public abstract double[,] Compute(double[,] input);
    }
}