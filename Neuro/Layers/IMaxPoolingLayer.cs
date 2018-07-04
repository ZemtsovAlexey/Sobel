using Neuro.Models;

namespace Neuro.Layers
{
    public interface IMaxPoolingLayer : ILayer
    {
        double[][,] Outputs { get; }
    }
}