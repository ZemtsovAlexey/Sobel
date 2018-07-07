using System;
using System.Collections.Generic;
using System.Linq;
using Neuro.ActivationFunctions;

namespace Neuro.Neurons
{
    public class FullyConnectedNeuron
    {
        public double[] Weights { get; }
        public double Output { get; private set; }
        public IActivationFunction Function { get; }
        
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public FullyConnectedNeuron(int inputsCount, IActivationFunction function)
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

        public double Compute(IEnumerable<double> input)
        {
            var e = input.Select((xn, n) => Weights[n] * xn).Sum();

            Output = Function.Activation(e);

            return Output;
        }
    }
}