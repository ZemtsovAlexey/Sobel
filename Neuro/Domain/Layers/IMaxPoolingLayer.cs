using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IMaxPoolingLayer : ILayer
    {
        double[][,] Outputs { get; }
        MaxPoolingNeuron[] Neurons { get; set; }
    }
}