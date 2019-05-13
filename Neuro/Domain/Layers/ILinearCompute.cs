namespace Neuro.Layers
{
    public interface ILinearCompute
    {
        double[] Outputs { get; }

        double[] Compute(double[] inputs);
    }
}
