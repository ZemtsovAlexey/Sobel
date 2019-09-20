namespace Neuro.ActivationFunctions
{
    public class LeakyReLU : IActivationFunction
    {
        public float Alpha { get; set; } = 0.5f;
        public float MinRange { get; set; } = 0;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            return x > 0 ? x : 0.01f * x;
        }

        public float Derivative(float x)
        {
            return x > 0 ? 1 : 0.01f;
        }
    }
}