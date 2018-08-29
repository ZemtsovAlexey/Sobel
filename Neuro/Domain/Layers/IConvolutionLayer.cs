using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IConvolutionLayer : IRandomize, IMatrixLayer
    {
        ActivationType ActivationFunctionType { get; }
        int KernelSize { get; }
        ConvolutionNeuron[] Neurons { get; set; }
        ConvolutionNeuron this[int index] { get; }

        void Init(int inputWidth, int inputHeight);
    }
}
