namespace Neuro.ActivationFunctions
{
    public interface IActivationFunction
    {
        float Alpha { get; set; }
        
        float MinRange { get; set; }
        
        float MaxRange { get; set; }
        
        float Activation(float x);

        float Derivative(float x);
    }
}