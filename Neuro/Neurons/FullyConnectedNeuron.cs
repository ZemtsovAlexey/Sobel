using System;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Extensions;

namespace Neuro.Neurons
{
    public class FullyConnectedNeuron
    {
        public float[] Weights { get; set; }
        public float Output { get; set; }
        public IActivationFunction Function { get; }
        public float Bias { get; set; } = 0;
        
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public FullyConnectedNeuron(int inputsCount, IActivationFunction function)
        {
            inputsCount = Math.Max(1, inputsCount);
            Weights = new float[inputsCount];
            Function = function;
        }

        public void Randomize()
        {
            Bias = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
            Bias = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;

            for (var i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
            }
        }

        public float Compute(float[] input)
        {
            float e = 0;
            unsafe
            {
                fixed (float* w = Weights, i = input)
                    for (var n = 0; n < input.Length; n++)
                        e += w[n] * i[n];
            }

            var output = Function.Activation(e + Bias);

            Output = output;

            return output;
        }
    }
}