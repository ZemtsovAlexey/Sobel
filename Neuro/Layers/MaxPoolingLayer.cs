using System.Linq;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class MaxPoolingLayer// : IConvolutionalLayer
    {
        public LayerType Type { get; set; } = LayerType.MaxPoolingLayer;
        public MaxPoolingNeuron[] Neurons { get; set; }
        public double[][,] Outputs { get; set; }
        
        public int NeuronsCount => Neurons.Length;

        public MaxPoolingLayer(int neuronsCount, int inputWidth, int inputHeitght, int kernelSize = 2)
        {
            Neurons = new MaxPoolingNeuron[neuronsCount];

            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new MaxPoolingNeuron(inputWidth, inputHeitght, kernelSize);
            }
            
            Outputs = new double[neuronsCount][,];
        }

        public void Randomize()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Randomize();
            }
        }

        public double[][,] Compute(double[][,] input)
        {
            var outputs = Neurons.Select((n, i) => n.Compute(input[i])).ToArray();

            Outputs = outputs;

            return Outputs;
        }
    }
}
