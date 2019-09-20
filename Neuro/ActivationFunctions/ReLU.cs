using Neuro.Extensions;
using System;

namespace Neuro.ActivationFunctions
{
    public class ReLU : IActivationFunction
    {
        public float Alpha { get; set; } = 0.5f;
        public float MinRange { get; set; } = 0;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x > 0 ? x : 0;// Math.Max(0.01, x);
        }

        public float Derivative(float x)
        {
            return x <= 0 ? 0 : 1;
            var random = new Random((int)DateTime.Now.Ticks);
            return x > 0 ? x : random.NextFloat() * (0.05f - 0.01f) + 0.01f;
        }
    }
}