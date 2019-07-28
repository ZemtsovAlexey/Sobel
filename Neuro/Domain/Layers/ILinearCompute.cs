using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface ILinearCompute
    {
        double[] Outputs { get; }

        double[] Compute(double[] inputs);
        
        FullyConnectedNeuron[] Neurons { get; }
        
        FullyConnectedNeuron this[int index] { get; }
    }
}
