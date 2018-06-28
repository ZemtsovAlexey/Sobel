using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface IConvolutionalLayer : ILayer
    {
        int NeuronsCount { get; }
        
        ConvolutionalNeuron[] Neurons { get; set; }
        
        double[][,] Outputs { get; }
        
        double[][,] Compute(double[][,] input);
        
        ConvolutionalNeuron this[int index] { get; }
    }
}
