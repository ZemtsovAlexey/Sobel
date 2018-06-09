using System.Linq;
using System.Numerics;
using Neuro.ActivationFunctions;
using Neuro.Models;
using Neuro.Models.Outputs;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class ConvolutionalLayer : ILayer
    {
        public LayerType Type { get; set; } = LayerType.ConvolutionWithMaxpooling;
        public ConvolutionalNeuron[] Neurons { get; set; }
        public double[][,] Outputs { get; set; }
        public int OutputWidht { get; }
        public int OutputHeight { get; }

        public ConvolutionalLayer(IActivationFunction activationFunction, int neuronsCount, int inputWidth, int inputHeitght, int kernelSize = 3)
        {
            OutputWidht = inputWidth - kernelSize + 1;
            OutputHeight = inputHeitght - kernelSize + 1;
            Neurons = new ConvolutionalNeuron[neuronsCount];

            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new ConvolutionalNeuron(activationFunction, inputWidth, inputHeitght, kernelSize);
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
            var outputs = Neurons.Select(n => n.Compute(input)).ToArray();

            Outputs = outputs;

            return Outputs;
        }
    }
}
