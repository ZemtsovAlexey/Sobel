namespace Neuro.ActivationFunctions
{
    public class AbsoluteReLU : IActivationFunction
    {
        public float Alpha { get; set; }
        public float MinRange { get; set; } = -1;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x >= 0 ? x : -x;
        }

        public float Derivative(float x)
        {
            return x >= 0 ? 1 : -1;
        }
    }
}