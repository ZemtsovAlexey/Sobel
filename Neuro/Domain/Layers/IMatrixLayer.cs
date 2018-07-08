namespace Neuro.Layers
{
    public interface IMatrixLayer : ILayer
    {
        double[][,] Outputs { get; }
     
        int OutputWidht { get; }
        
        int OutputHeight { get; }
        
        double[][,] Compute(double[][,] input);
    }
}
