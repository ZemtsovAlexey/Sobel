using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface IFullyConnectedLayer : ILayer, IRandomize, ILinearCompute
    {
        int NeuronsCount { get; }
        
        ActivationNeuron[] Neurons { get; }
        
        double[] Outputs { get; }
        
        ActivationNeuron this[int index] { get; }
        
        double[] Compute(double[] inputs);
    }
}
