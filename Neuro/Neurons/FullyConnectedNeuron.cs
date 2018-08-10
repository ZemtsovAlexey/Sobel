using System;
using System.Collections.Generic;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Extensions;

namespace Neuro.Neurons
{
    public class FullyConnectedNeuron
    {
        public float[] Weights { get; set; }
        public float Output { get; private set; }
        public IActivationFunction Function { get; }
        
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public FullyConnectedNeuron(int inputsCount, IActivationFunction function)
        {
            inputsCount = Math.Max(1, inputsCount);
            Weights = new float[inputsCount];
            Function = function;
        }

        public void Randomize()
        {
            for (var i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
            }
        }

        public float Compute(IEnumerable<float> input)
        {
            var e = input.Select((xn, n) => Weights[n] * xn).Sum();
            var output = Function.Activation(e);

            Output = output;

            return output;
        }
    }
}