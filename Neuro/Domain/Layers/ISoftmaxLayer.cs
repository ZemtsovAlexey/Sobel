using Neuro.Layers;

namespace Neuro.Domain.Layers
{
    public interface ISoftmaxLayer : ILayer, IRandomize, ILinearCompute
    {
        void Init(int index, int inputsCount);

        double Derivative(int neuron);
    }
}
