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
            var e2x = Math.Exp(2 * x);
            return (e2x - 1) / (e2x + 1);
        }

        public double Derivative(double x)
        {
            double
                eminus2x = Math.Exp(-x),
                e2x = Math.Exp(x),
                sum = eminus2x + e2x,
                square = sum * sum,
                div = 4 / square;
            
            return div;
        }
    }
}