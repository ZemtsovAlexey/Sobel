namespace Neuro.Layers
{
    public interface IConvolutionalLayer : ILayer
    {
        double[][,] Outputs { get; }
        
        double[][,] Compute(double[][,] input);
    }
}
