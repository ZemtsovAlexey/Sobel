using System;
using System.Linq;
using Neuro.ActivationFunctions;

namespace Neuro.Neurons
{
    public class ActivationNeuron
    {
        public double[] Weights { get; set; }
        public double Output { get; set; }
        public IActivationFunction Function { get; set; }
        
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public ActivationNeuron(int inputsCount, IActivationFunction function)
        {
            inputsCount = Math.Max(1, inputsCount);
            Weights = new double[inputsCount];
            Function = function;
        }

        public void Randomize()
        {
            for (var i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.NextDouble() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
            }
        }

        public double Compute(double[] input)
        {
            var e = input.Select((Xn, n) => Weights[n] * Xn).Sum();

            Output = Function.Activation(e);

            return Output;
        }
    }
}