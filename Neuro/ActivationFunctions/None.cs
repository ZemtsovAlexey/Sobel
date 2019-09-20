using System;

namespace Neuro.ActivationFunctions
{
    public class None : IActivationFunction
    {
        public float Alpha { get; set; }
        public float MinRange { get; set; } = -1;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x;
        }

        public float Derivative(float x)
        {
            return 1;
        }
    }
}