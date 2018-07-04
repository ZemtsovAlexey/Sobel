using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface IConvolutionalLayer : ILayer, IWithWeightsLayer
    {
        int NeuronsCount { get; }
        int KernelSize { get; }
        ConvolutionalNeuron[] Neurons { get; set; }
        double[][,] Outputs { get; }
        ConvolutionalNeuron this[int index] { get; }
        
        double[][,] Compute(double[][,] input);
        double[] GetLinereOutput();
    }
}
