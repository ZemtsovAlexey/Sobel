﻿using System.Linq;
using System.Threading.Tasks;
using Neuro.ActivationFunctions;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class ConvolutionalLayer : IConvolutionalLayer
    {
        public LayerType Type { get; set; } = LayerType.Convolution;
        public ConvolutionalNeuron[] Neurons { get; set; }
        public double[][,] Outputs { get; private set; }
        public int OutputWidht { get; }
        public int OutputHeight { get; }
        public int KernelSize { get; private set; }

        public int NeuronsCount => Neurons.Length;
        public ConvolutionalNeuron this[int index] => Neurons[index];
        
        public ConvolutionalLayer(IActivationFunction activationFunction, int neuronsCount, int inputWidth, int inputHeitght, int kernelSize = 3)
        {
            KernelSize = kernelSize;
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
        
        public double[] GetLinereOutput()
        {
            return MapToArray(Outputs);
        }
        
        public double[] MapToArray(double[][,] outputs)
        {
            var imageHeight = outputs[0].GetLength(0);
            var imageWidth = outputs[0].GetLength(1);
            var result = new double[outputs.Length * imageHeight * imageWidth];

            Parallel.For(0, outputs.Length, (int i) =>
            {
                Parallel.For(0, imageHeight, (int h) =>
                {
                    Parallel.For(0, imageWidth, (int w) =>
                    {
                        var position = (i * imageWidth * imageHeight) + (h * imageWidth + w);
                        result[position] = outputs[i][h, w];
                    });
                });
            });

            return result;
        }
    }
}
