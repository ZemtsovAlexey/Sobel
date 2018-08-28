using System;

namespace Neuro.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public double Alpha { get; set; } = 1;
        public double MinRange { get; set; } = 0;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return 1 / (1 + (double)Math.Exp(-Alpha * x));
        }

        public double Derivative(double x)
        {
            return  Alpha * x * (1 - x);
        }
    }
}