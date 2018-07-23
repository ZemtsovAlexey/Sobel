namespace Neuro.Layers
{
    public interface IMatrixLayer : ILayer
    {
        float[][,] Outputs { get; }
     
        int OutputWidht { get; }
        
        int OutputHeight { get; }
        
        float[][,] Compute(float[][,] input);
    }
}
