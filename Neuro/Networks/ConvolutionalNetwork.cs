using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork// : Network
    {
        public ConvolutionalLayer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
        public ConvolutionalLayer this[int index] => Layers[index];
        
        public ConvolutionalNetwork(IActivationFunction function, int inputWidth, int inputHeitght, int kernelZize, int neuronsCount)
        {
            for (var i = 0; i < neuronsCount; i++)
            {
                Layers[i] = new ConvolutionalLayer(
                    function, 
                    neuronsCount, 
                    i == 0 ? inputWidth : Layers[i - 1].OutputWidht,
                    i == 0 ? inputHeitght : Layers[i - 1].OutputHeight,
                    kernelZize
                    );
            }
        }
        
        public ConvolutionalNetwork()
        {
        }

        public void Randomize()
        {
            foreach (var layer in Layers)
            {
                layer.Randomize();
            }
        }
        
        //public double[,] Compute(double[,] input)
        //{
        //    Output = input;

        //    for (var i = 0; i < Layers.Length; i++)
        //    {
        //        Output = Layers[i].Compute(i == 0 ? input : Output);
        //    }

        //    return Output;
        //}
    }
}