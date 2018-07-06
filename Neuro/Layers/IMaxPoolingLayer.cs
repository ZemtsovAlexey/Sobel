using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface IMaxPoolingLayer : ILayer
    {
        double[][,] Outputs { get; }
        MaxPoolingNeuron[] Neurons { get; set; }
    }
}