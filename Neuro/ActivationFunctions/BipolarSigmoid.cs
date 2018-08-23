using System;

namespace Neuro.ActivationFunctions
{
    public class BipolarSigmoid : IActivationFunction
    {
        public float Alpha { get; set; } = 1;
        public float MinRange { get; set; } = -1;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return (2 / (1 + (float)Math.Exp(-Alpha * x))) - 1;
        }

        public float Derivative(float x)
        {
            return Alpha * (1 - x * x) / 2;
        }
    }
}