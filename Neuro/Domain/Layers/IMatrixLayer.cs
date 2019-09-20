using Neuro.Models;

namespace Neuro.Layers
{
    public interface IMatrixLayer : ILayer
    {
        Matrix[] Outputs { get; set; }
     
        int OutputWidht { get; }
        
        int OutputHeight { get; }

        Matrix[] Compute(Matrix[] input);
    }
}
