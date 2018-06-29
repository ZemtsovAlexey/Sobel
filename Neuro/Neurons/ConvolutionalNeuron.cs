using System;
using Neuro.ActivationFunctions;

namespace Neuro.Neurons
{
    public class ConvolutionalNeuron //: Neuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private readonly int _kernelSize;

        public IActivationFunction Function { get; set; }
        public double[,] Input { get; private set; }
        public double[,] Weights { get; set; }
        public double[,] Output { get; set; }

        public ConvolutionalNeuron(IActivationFunction function, int inWidth, int inHeight, int kernelSize = 3)
        {
            if (kernelSize % 2 == 0)
            {
                throw new ArgumentException("Размер ядра должен быть не четным");
            }

            _kernelSize = kernelSize;
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
            var outputHeight = Output.GetLength(0) - _kernelSize;
            var outputWidth = Output.GetLength(1) - _kernelSize;
            var inputHeight = input[0].GetLength(0);
            var inputWidth = input[0].GetLength(1);

            Input = new double[inputHeight, inputWidth];

            //суммируем все входные изображения
            for (i = 0; i < input.Length; i++)
            {
                for (y = 0; y < inputHeight; y++)
                {
                    for (x = 0; x < inputWidth; x++)
                    {
                        Input[y, x] += input[i][y, x];
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
                            Output[y, x] += Input[y + h, x + w] * Weights[h, w];
                        }
                    }

                    Output[y, x] = Function.Activation(Output[y, x]);
                }
            }

            return Output;
        }
    }
}
