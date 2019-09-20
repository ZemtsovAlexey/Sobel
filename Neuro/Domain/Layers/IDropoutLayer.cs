using Neuro.Layers;

namespace Neuro.Domain.Layers
{
    public interface IDropoutLayer : ILayer
    {
        float DropProbability { get; set; }

        void Init(int index);

        float[] Derivative(float[] inputs);
    }
}
