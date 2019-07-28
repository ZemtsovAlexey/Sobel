using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IConvolutionLayer : IRandomize, IMatrixLayer
    {
        ActivationType ActivationFunctionType { get; }
        int KernelSize { get; }
        ConvolutionNeuron[] Neurons { get; set; }
        bool UseReferences { get; }
        ConvolutionNeuron this[int index] { get; }

        void Init(int index, int inputWidth, int inputHeight, int linksCount);
    }
}
