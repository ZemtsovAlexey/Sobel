﻿using System;
using Neuro.ActivationFunctions;

namespace Neuro.Neurons
{
    public class ConvolutionalNeuron //: Neuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        public IActivationFunction Function { get; set; }
        private int _kernelSize = 3;

//        public IActivationFunction Function { get; set; }
        public new double[,] Weights { get; set; }
        public new double[,] Output { get; set; }

        public ConvolutionalNeuron(IActivationFunction function, int inWidth, int inHeight, int kernelSize = 3)
        {
            if (kernelSize % 2 == 0)
            {
                throw new ArgumentException("Размер ядра должен быть не четным");
            }

            this._kernelSize = kernelSize;
            Weights = new double[kernelSize, kernelSize];
            Output = new double[inHeight - kernelSize + 1, inWidth - kernelSize + 1];
            Function = function;
        }

        public void Randomize()
        {
            int y, x;

            for (y = 0; y < _kernelSize; y++)
            {
                for (x = 0; x < _kernelSize; x++)
                {
                    Weights[y, x] = Random.NextDouble() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
                }
            }
        }
        
        public double[,] Compute(double[][,] input)
        {
            int i, y, x, h, w;
            var outputHeight = Output.GetLength(0);
            var outputWidth = Output.GetLength(1);
            var inputHeight = input[0].GetLength(0);
            var inputWidth = input[0].GetLength(1);
            double[,] inputSum = new double[inputHeight, inputWidth];

            //суммируем все входные изображения
            for (i = 0; i < input.Length; i++)
            {
                for (y = 0; y < inputHeight; y++)
                {
                    for (x = 0; x < inputWidth; x++)
                    {
                        inputSum[y, x] += input[i][y, x];
                    }
                }
            }
            
            //сканируем изображение ядром
            for (y = 0; y < outputHeight; y++)
            {
                for (x = 0; x < outputWidth; x++)
                {
                    for (h = 0; h < _kernelSize; h++)
                    {
                        for (w = 0; w < _kernelSize; w++)
                        {
                            Output[y, x] += inputSum[y + h, x + w] * Weights[h, w];
                        }
                    }

                    Output[y, x] = Function.Activation(Output[y, x]);
                }
            }

            return Output;
        }
    }
}
