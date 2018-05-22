using System.Linq;
using System.Numerics;
using Neuro.ActivationFunctions;
using Neuro.ConvolutionalNetworks;
using Neuro.Models;
using Neuro.Models.Outputs;

namespace Neuro.Layers
{
    public class ConvolutionalLayer// : Layer
    {
        public new ConvolutionalNeuron[] Neurons { get; set; }
        public new ConvInputVarible Outputs { get; set; }
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
            
            Outputs = new ConvInputVarible { Value = new double[neuronsCount][,] };
        }

        public void Randomize()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Randomize();
            }
        }

        public InputValue Compute(InputValue input)
        {
            var convInput = (ConvInputVarible) input;
            var outputs = Neurons.Select(n => n.Compute(convInput.Value)).ToArray();

            Outputs = new ConvInputVarible {Value = outputs};

            return Outputs;
        }
    }
}
