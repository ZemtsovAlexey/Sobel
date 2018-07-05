using System.Linq;
using Neuro.Extensions;
using Neuro.Layers;
using Neuro.Models;

namespace Neuro.Networks
{
    public class ConvolutionalNetwork
    {
        //public IConvolutionalLayer[] ConvLayers { get; set; }
        //public IFullyConnectedLayer[] FullyConnectedLayers { get; set; }
        public ILayer[] Layers;
        public double[] Output;
        private double[][,] _output;
        
        public ConvolutionalNetwork(){}
        
        public ConvolutionalNetwork(IConvolutionalLayer[] convLayers, IFullyConnectedLayer[] fullyConnectedLayers)
        {
            //ConvLayers = convLayers;
            //FullyConnectedLayers = fullyConnectedLayers;
        }
        
        public void InitLayers(params ILayer[] layers) 
        {
            Layers = new ILayer[layers.Length];
            
            for (var i = 0; i < layers.Length; i++)
            {
                Layers[i] = layers[i];
            }
        }

        public void Randomize()
        {
            foreach (IRandomize layer in Layers.Where(l => l.Type == LayerType.Convolution || l.Type == LayerType.FullyConnected))
            {
                layer.Randomize();
            }
            
            /*foreach (var layer in ConvLayers)
            {
                layer.Randomize();
            }
            
            foreach (var layer in FullyConnectedLayers)
            {
                layer.Randomize();
            }*/
        }
        
        public double[] Compute(double[,] input)
        {
            _output = new[] {input};

            foreach (IMatrixCompute layer in Layers.Where(l => l.Type == LayerType.Convolution || l.Type == LayerType.MaxPoolingLayer))
            {
                _output = layer.Compute(_output);
            }

            Output = _output.ToLinearArray();

            foreach (ILinearCompute layer in Layers.Where(l => l.Type == LayerType.FullyConnected))
            {
                Output = layer.Compute(Output);
            }
            
            return Output;
        }
    }
}