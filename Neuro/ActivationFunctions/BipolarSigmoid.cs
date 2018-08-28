using System;

namespace Neuro.ActivationFunctions
{
    public class BipolarSigmoid : IActivationFunction
    {
        public double Alpha { get; set; } = 1;
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return (2 / (1 + (double)Math.Exp(-Alpha * x))) - 1;
        }

        public double Derivative(double x)
        {
            return Alpha * (1 - x * x) / 2;
        }
    }
}