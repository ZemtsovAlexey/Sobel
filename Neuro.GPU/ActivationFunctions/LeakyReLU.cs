namespace Neuro.ActivationFunctions
{
    public class LeakyReLU : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = 0;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            return x > 0 ? x : 0.01f * x;
        }

        public double Derivative(double x)
        {
            return x > 0 ? 1 : 0.01f;
        }
    }
}