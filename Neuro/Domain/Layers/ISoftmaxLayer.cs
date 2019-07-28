using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface ISoftmaxLayer : ILayer, IRandomize, ILinearCompute
    {
        void Init(int inputsCount);

        double Derivative(int neuron);
    }
}
