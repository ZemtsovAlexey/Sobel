using System;

namespace Neuro.ActivationFunctions
{
    public class BipolarSigmoidFunction : IActivationFunction
    {
        public double Alpha { get; set; } = 2;
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return (2 / (1 + Math.Exp(-Alpha * x))) - 1;
        }

        public double Derivative(double y)
        {
            return Alpha * (1 - y * y) / 2;
        }
    }
}