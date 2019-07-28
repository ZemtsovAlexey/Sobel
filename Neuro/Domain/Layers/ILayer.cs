using Neuro.Models;

namespace Neuro.Layers
{
    public interface ILayer
    {
        int Index { get; }

        LayerType Type { get; }
        
        int NeuronsCount { get; }
    }
}
