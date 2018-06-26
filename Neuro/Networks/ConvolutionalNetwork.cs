using System.Threading.Tasks;
using Neuro.Layers;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork
    {
        public IConvolutionalLayer[] ConvLayers { get; set; }
        public IFullyConnectedLayer[] FullyConnectedLayers { get; set; }
        public double[] Output;
        private double[][,] _output;
        
        public ConvolutionalNetwork(IConvolutionalLayer[] convLayers, IFullyConnectedLayer[] fullyConnectedLayers)
        {
            ConvLayers = convLayers;
            FullyConnectedLayers = fullyConnectedLayers;
        }

        public void Randomize()
        {
            foreach (var layer in ConvLayers)
            {
                layer.Randomize();
            }
        }
        
        public double[] Compute(double[,] input)
        {
            _output = new[] {input};

            foreach (var layer in ConvLayers)
            {
                _output = layer.Compute(_output);
            }

            Output = MapToArray(_output);

            foreach (var layer in FullyConnectedLayers)
            {
                Output = layer.Compute(Output);
            }
            
            return Output;
        }
        
        private static double[] MapToArray(double[][,] outputs)
        {
            var imageHeight = outputs[0].GetLength(0);
            var imageWidth = outputs[0].GetLength(1);
            var result = new double[outputs.Length * imageHeight * imageWidth];

            Parallel.For(0, outputs.Length, (int i) =>
            {
                Parallel.For(0, imageHeight, (int h) =>
                {
                    Parallel.For(0, imageWidth, (int w) =>
                    {
                        var position = (i * imageWidth * imageHeight) + (h * imageWidth + w);
                        result[position] = outputs[i][h, w];
                    });
                });
            });

            return result;
        }
    }
}