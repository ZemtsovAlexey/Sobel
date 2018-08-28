using System;

namespace Neuro.ActivationFunctions
{
    public class None : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return x;
        }

        public double Derivative(double x)
        {
            return 1;
        }
    }
}