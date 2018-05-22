using System;
using Neuro.ActivationFunctions;

namespace Neuro.Neurons
{
    public abstract class Neuron
    {
        public double[] Weights { get; set; }
        
        public double Output { get; set; }
        
        public IActivationFunction Function { get; set; }

        protected Neuron()
        {
        }
        
        protected Neuron(int inputsCount)
        {
            inputsCount = Math.Max(1, inputsCount);
            Weights = new double[inputsCount];
        }
        
        public abstract void Randomize();

        public abstract double Compute(double[] input);
        
        public abstract double[,] Compute(double[][,] input);
    }
}