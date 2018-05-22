using System;

namespace Neuro.ActivationFunctions
{
    public class SigmoidFunction : IActivationFunction
    {
        public double Alpha { get; set; } = 2;
        public double MinRange { get; set; } = 0;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return 1 / (1 + Math.Exp(-Alpha * x));
        }

        public double Derivative(double y)
        {
            return  Alpha * y * (1 - y);
        }
    }
}