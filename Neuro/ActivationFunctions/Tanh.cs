using System;

namespace Neuro.ActivationFunctions
{
    public class Tanh : IActivationFunction
    {
        public float Alpha { get; set; }
        public float MinRange { get; set; } = -1;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            float e2x = (float)Math.Exp(2 * x);
            return (e2x - 1f) / (e2x + 1f);
        }

        public float Derivative(float x)
        {
            float
                eminus2x = (float)Math.Exp(-x),
                e2x = (float)Math.Exp(x),
                sum = eminus2x + e2x,
                square = sum * sum,
                div = 4 / square;
            
            return div;
        }
    }
}