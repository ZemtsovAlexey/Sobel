using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IConvolutionalLayer : IRandomize, IMatrixLayer
    {
        ActivationType ActivationFunctionType { get; }
        int NeuronsCount { get; }
        int KernelSize { get; }
        ConvolutionalNeuron[] Neurons { get; set; }
        ConvolutionalNeuron this[int index] { get; }

        void Init(int inputWidth, int inputHeitght);
    }
}
