using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IAvgPoolingLayer : IMatrixLayer
    {
        AvgPoolingNeuron[] Neurons { get; }

        int KernelSize { get; set; }
        
        void Init(int neuronsCount, int inputWidth, int inputHeitght);
    }
}