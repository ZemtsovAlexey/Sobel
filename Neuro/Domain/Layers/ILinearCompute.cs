using Neuro.Neurons;

namespace Neuro.Layers
{
    public interface ILinearCompute
    {
        float[] Outputs { get; set; }

        float[] Compute(float[] inputs);
        
        FullyConnectedNeuron[] Neurons { get; }
        
        FullyConnectedNeuron this[int index] { get; }
    }
}
