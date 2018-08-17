﻿using System;
using System.Threading.Tasks;
using Neuro.ActivationFunctions;
using Neuro.Extensions;

namespace Neuro.Neurons
{
    public class ConvolutionalNeuron //: Neuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private readonly int _kernelSize;
        private readonly int _inWidth;
        private readonly int _inHeight;

        public IActivationFunction Function { get; set; }
        public float[,] Weights { get; set; }
        public float[,] Output { get; set; }

        public ConvolutionalNeuron(IActivationFunction function, int inWidth, int inHeight, int kernelSize = 3)
        {
            //if (kernelSize % 2 == 0)
            //{
            //    throw new ArgumentException("Размер ядра должен быть не четным");
            //}

            _inWidth = inWidth;
            _inHeight = inHeight;
            _kernelSize = kernelSize;
            Weights = new float[kernelSize, kernelSize];
            Output = new float[inHeight - kernelSize + 1, inWidth - kernelSize + 1];
            Function = function;
        }

        public void Randomize()
        {
            int y, x;

            for (y = 0; y < _kernelSize; y++)
            {
                for (x = 0; x < _kernelSize; x++)
                {
                    Weights[y, x] = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
                }
            }
        }
        
        public float[,] Compute(float[][,] input)
        {
            int y, x, h, w;
            var outputHeight = Output.GetLength(0);
            var outputWidth = Output.GetLength(1);

            var output = new float[_inHeight - _kernelSize + 1, _inWidth - _kernelSize + 1];
            var Input = input.Sum();
            
            //сканируем изображение ядром
            for (y = 0; y < outputHeight; y++)
            {
                for (x = 0; x < outputWidth; x++)
                {
                    for (h = 0; h < _kernelSize; h++)
                    {
                        for (w = 0; w < _kernelSize; w++)
                        {
                            output[y, x] += Input[y + h, x + w] * Weights[h, w];
                        }
                    }

                    output[y, x] = Function.Activation(output[y, x]);
                }
            }

            Output = output;
            
            return output;
        }
    }
}
