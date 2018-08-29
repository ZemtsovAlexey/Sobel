using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class ConvolutionLayer : IConvolutionLayer
    {
        public LayerType Type { get; set; } = LayerType.Convolution;
        public ActivationType ActivationFunctionType { get; }
        public ConvolutionNeuron[] Neurons { get; set; }
        public Matrix[] Outputs { get; private set; }
        public int OutputWidht { get; private set; }
        public int OutputHeight { get; private set; }
        public int KernelSize { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public ConvolutionNeuron this[int index] => Neurons[index];

        private IActivationFunction _function;
        
        public ConvolutionLayer(ActivationType activationType, int neuronsCount, int kernelSize = 3)
        {
            ActivationFunctionType = activationType;
            _function = activationType.Get();
            KernelSize = kernelSize;
            Neurons = new ConvolutionNeuron[neuronsCount];
            Outputs = new Matrix[neuronsCount];
        }

        public void Init(int inputWidth, int inputHeight)
        {
            OutputWidht = inputWidth - KernelSize + 1;
            OutputHeight = inputHeight - KernelSize + 1;
            
            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new ConvolutionNeuron(_function, inputWidth, inputHeight, KernelSize);
            }
        }

        public void Randomize()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Randomize();
            }
        }

        public Matrix[] Compute(Matrix[] input)
        {
            var outputs = Neurons.AsParallel().Select(n => n.Compute(input)).ToArray();

            Outputs = outputs;

            return outputs;
        }
    }
}
