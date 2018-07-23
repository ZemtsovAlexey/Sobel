using System;

namespace Neuro.ActivationFunctions
{
    public class ELU : IActivationFunction
    {
        public float Alpha { get; set; }
        public float MinRange { get; set; } = 0;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x >= 0 ? x : (float)Math.Exp(x) - 1;;
        }

        public float Derivative(float x)
        {
            return x >= 0 ? 1 : (float)Math.Exp(x);
        }
    }
}