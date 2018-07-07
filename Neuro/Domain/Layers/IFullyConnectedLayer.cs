using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IFullyConnectedLayer : ILayer, IRandomize, ILinearCompute
    {
        int NeuronsCount { get; }
        
        FullyConnectedNeuron[] Neurons { get; }
        
        double[] Outputs { get; }
        
        FullyConnectedNeuron this[int index] { get; }
        
        double[] Compute(double[] inputs);
    }
}
