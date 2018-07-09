using System.Linq;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class MaxPoolingLayer : IMaxPoolingLayer
    {
        public LayerType Type { get; } = LayerType.MaxPoolingLayer;
        public MaxPoolingNeuron[] Neurons { get; }
        public double[][,] Outputs { get; private set; }
        public int OutputWidht { get; private set; }
        public int OutputHeight { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public int KernelSize { get; set; }

        public MaxPoolingLayer(int neuronsCount, int kernelSize = 2)
        {
            KernelSize = kernelSize;
            Neurons = new MaxPoolingNeuron[neuronsCount];
            Outputs = new double[neuronsCount][,];
        }

        public void Init(int inputWidth, int inputHeitght)
        {
            OutputHeight = inputHeitght / KernelSize;
            OutputWidht = inputWidth / KernelSize;
            
            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new MaxPoolingNeuron(inputWidth, inputHeitght, KernelSize);
            }
        }

        public double[][,] Compute(double[][,] input)
        {
            var outputs = Neurons.AsParallel().Select((n, i) => n.Compute(input[i])).ToArray();

            Outputs = outputs;

            return Outputs;
        }
    }
}
