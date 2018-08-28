namespace Neuro.ActivationFunctions
{
    public interface IActivationFunction
    {
        double Alpha { get; set; }
        
        double MinRange { get; set; }
        
        double MaxRange { get; set; }
        
        double Activation(double x);

        double Derivative(double x);
    }
}