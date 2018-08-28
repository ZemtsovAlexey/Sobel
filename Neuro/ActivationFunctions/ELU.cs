using System;

namespace Neuro.ActivationFunctions
{
    public class ELU : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = 0;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return x >= 0 ? x : (double)Math.Exp(x) - 1;;
        }

        public double Derivative(double x)
        {
            return x >= 0 ? 1 : (double)Math.Exp(x);
        }
    }
}