using System.Linq;
using System.Threading.Tasks;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Extensions;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class ConvolutionalLayer : IConvolutionalLayer
    {
        public LayerType Type { get; set; } = LayerType.Convolution;
        public ConvolutionalNeuron[] Neurons { get; set; }
        public double[][,] Outputs { get; private set; }
        public int OutputWidht { get; private set; }
        public int OutputHeight { get; private set; }
        public int KernelSize { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public ConvolutionalNeuron this[int index] => Neurons[index];

        private IActivationFunction _function;
        
        public ConvolutionalLayer(IActivationFunction activationFunction, int neuronsCount, int kernelSize = 3)
        {
            _function = activationFunction;
            KernelSize = kernelSize;
            Neurons = new ConvolutionalNeuron[neuronsCount];
            Outputs = new double[neuronsCount][,];
        }

        public void Init(int inputWidth, int inputHeitght)
        {
            OutputWidht = inputWidth - KernelSize + 1;
            OutputHeight = inputHeitght - KernelSize + 1;
            
            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new ConvolutionalNeuron(_function, inputWidth, inputHeitght, KernelSize);
            }
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
            var outputs = Neurons.AsParallel().Select(n => n.Compute(input)).ToArray();

            Outputs = outputs;

            return Outputs;
        }
        
        public double[] GetLinereOutput()
        {
            return Outputs.ToLinearArray();
        }
    }
}
