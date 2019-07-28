using Neuro.Layers;

namespace Neuro.Domain.Layers
{
    public interface IDropoutLayer : ILayer
    {
        double DropProbability { get; }

        void Init(int index);

        double[] Derivative(double[] inputs);
    }
}
