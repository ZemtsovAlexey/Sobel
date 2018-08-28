using System;

namespace Neuro.ActivationFunctions
{
    public class Tanh : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            double e2x = (double)Math.Exp(2 * x);
            return (e2x - 1f) / (e2x + 1f);
        }

        public double Derivative(double x)
        {
            double
                eminus2x = (double)Math.Exp(-x),
                e2x = (double)Math.Exp(x),
                sum = eminus2x + e2x,
                square = sum * sum,
                div = 4 / square;
            
            return div;
        }
    }
}