using System;

namespace Neuro.ActivationFunctions
{
    public class ReLU : IActivationFunction
    {
        public float Alpha { get; set; } = 2;
        public float MinRange { get; set; } = 0;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x > 0 ? x : 0;// Math.Max(0.01, x);
        }

        public float Derivative(float x)
        {
            return x <= 0 ? 0 : 1;
//            var random = new Random((int)DateTime.Now.Ticks);
//            return  x > 0 ? x : random.NextDouble() * (0.05 - 0.01) + 0.01;
        }
    }
}