using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface IConvolutionalLayer : ILayer, IRandomize, IMatrixCompute
    {
        int NeuronsCount { get; }
        int KernelSize { get; }
        ConvolutionalNeuron[] Neurons { get; set; }
        double[][,] Outputs { get; }
        ConvolutionalNeuron this[int index] { get; }
        
        double[] GetLinereOutput();
    }
}
