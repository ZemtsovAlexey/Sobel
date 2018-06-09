using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork
    {
        public ConvolutionalLayer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
//        public ConvolutionalLayer this[int index] => Layers[index];
        public double[] Output;
        private double[][,] _output;
        
//        public ConvolutionalNetwork(IActivationFunction function, int inputWidth, int inputHeight, int kernelSize, int neuronsCount)
//        {
//            for (var i = 0; i < neuronsCount; i++)
//            {
//                Layers[i] = new ConvolutionalLayer(
//                    function, 
//                    neuronsCount, 
//                    i == 0 ? inputWidth : Layers[i - 1].OutputWidht,
//                    i == 0 ? inputHeight : Layers[i - 1].OutputHeight,
//                    kernelSize
//                    );
//            }
//        }
        
        public ConvolutionalNetwork(params ConvolutionalLayer[] layers)
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

            foreach (var layer in Layers)
            {
                _output = layer.Compute(_output);
            }

            var imageHeight = _output[0].GetLength(0);
            var imageWidth = _output[0].GetLength(1);

            Output = new double[_output.Length * imageHeight * imageWidth];
            
            foreach (var image in _output)
            {
                Parallel.For(0, imageHeight, (int h) =>
                {
                    Parallel.For(0, imageWidth, (int w) =>
                    {
                        Output.Append(image[h, w]);
                    });
                });
            }

            return Output;
        }
    }
}