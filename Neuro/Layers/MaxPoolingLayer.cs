using System.Linq;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class MaxPoolingLayer : IMaxPoolingLayer
    {
        public int Index { get; private set; }
        public LayerType Type { get; } = LayerType.MaxPoolingLayer;
        public MaxPoolingNeuron[] Neurons { get; private set; }
        public Matrix[] Outputs { get; private set; }
        public int OutputWidht { get; private set; }
        public int OutputHeight { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public int KernelSize { get; set; }

        public MaxPoolingLayer(int kernelSize = 2)
        {
            KernelSize = kernelSize;
        }

        public void Init(int index, int neuronsCount, int inputWidth, int inputHeitght)
        {
            Index = index;
            Neurons = new MaxPoolingNeuron[neuronsCount];
            Outputs = new Matrix[neuronsCount];
            OutputHeight = inputHeitght / KernelSize;
            OutputWidht = inputWidth / KernelSize;
            
            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new MaxPoolingNeuron(inputWidth, inputHeitght, KernelSize);
            }
        }

        public Matrix[] Compute(Matrix[] input)
        {
            var outputs = Neurons.AsParallel().Select((n, i) => n.Compute(input[i])).ToArray();

            Outputs = outputs;

            return outputs;
        }
    }
}
