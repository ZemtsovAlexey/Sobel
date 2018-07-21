using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IFullyConnectedLayer : ILayer, IRandomize, ILinearCompute
    {
        ActivationType ActivationFunctionType { get; }
        
        int NeuronsCount { get; }
        
        FullyConnectedNeuron[] Neurons { get; }
        
        double[] Outputs { get; }
        
        IActivationFunction Function { get; }
        
        FullyConnectedNeuron this[int index] { get; }

        void Init(int inputsCount);
        
        double[] Compute(double[] inputs);
    }
}
