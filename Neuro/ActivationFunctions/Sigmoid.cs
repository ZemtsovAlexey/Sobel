using System;

namespace Neuro.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public float Alpha { get; set; } = 1;
        public float MinRange { get; set; } = 0;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return 1 / (1 + (float)Math.Exp(-Alpha * x));
        }

        public float Derivative(float x)
        {
            return  Alpha * x * (1 - x);
        }
    }
}