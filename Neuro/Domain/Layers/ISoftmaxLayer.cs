using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface ISoftmaxLayer : ILayer, IRandomize, ILinearCompute
    {
        FullyConnectedNeuron[] Neurons { get; }

        double[] Outputs { get; }

        FullyConnectedNeuron this[int index] { get; }

        void Init(int inputsCount);

        double Derivative(int neuron);
    }
}
