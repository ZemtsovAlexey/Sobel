using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IMaxPoolingLayer : IMatrixLayer
    {
        MaxPoolingNeuron[] Neurons { get; }

        int KernelSize { get; set; }
        
        void Init(int neuronsCount, int inputWidth, int inputHeitght);
    }
}