namespace Neuro.ActivationFunctions
{
    public class AbsoluteReLU : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return x >= 0 ? x : -x;
        }

        public double Derivative(double x)
        {
            return x >= 0 ? 1 : -1;
        }
    }
}