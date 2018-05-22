using System;

namespace Neuro.ActivationFunctions
{
    public class ReluFunction : IActivationFunction
    {
        public double Alpha { get; set; } = 2;
        public double MinRange { get; set; } = 0;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return Math.Max(0.01, x);
        }

        public double Derivative(double y)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            return  y > 0 ? 1 : random.NextDouble() * (0.05 - 0.01) + 0.01;
        }
    }
}