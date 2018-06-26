using System.Linq;
using System.Threading.Tasks;
using Neuro.Layers;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork
    {
        public ILayer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
        public double[] Output;
        private double[][,] _output;
        
        public ConvolutionalNetwork(params ILayer[] layers)
        {
            Layers = layers;
        }

        public void Randomize()
        {
            foreach (var layer in Layers)
            {
                layer.Randomize();
            }
        }
        
        public double[] Compute(double[,] input)
        {
            _output = new[] {input};

            foreach (var layer in Layers.Where(l => l.Type == Models.LayerType.ConvolutionWithMaxpooling || l.Type == Models.LayerType.MaxPoolingLayer))
            {
                _output = ((IConvolutionalLayer)layer).Compute(_output);
            }

            var imageHeight = _output[0].GetLength(0);
            var imageWidth = _output[0].GetLength(1);

            Output = new double[_output.Length * imageHeight * imageWidth];

            var imageNumber = 0;
            
            foreach (var image in _output)
            {
                Parallel.For(0, imageHeight, (int h) =>
                {
                    Parallel.For(0, imageWidth, (int w) =>
                    {
                        Output[imageNumber * (h * imageWidth + w)] = image[h, w];
                    });
                });

                imageNumber++;
            }

            foreach (var layer in Layers.Where(l => l.Type == Models.LayerType.FullyConnected))
            {
                Output = ((IFullyConnectedLayer)layer).Compute(Output);
            }
            
            return Output;
        }
    }
}