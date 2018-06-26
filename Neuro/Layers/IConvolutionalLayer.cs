namespace Neuro.Layers
{
    public interface IConvolutionalLayer : ILayer
    {
        double[][,] Compute(double[][,] input);
    }
}
