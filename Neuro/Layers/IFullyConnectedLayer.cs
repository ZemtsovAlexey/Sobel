namespace Neuro.Layers
{
    public interface IFullyConnectedLayer : ILayer
    {
        double[] Compute(double[] inputs);
    }
}
