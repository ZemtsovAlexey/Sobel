using System;

namespace Neuro.Neurons
{
    public class MaxPoolingNeuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private int _kernelSize = 2;

        public double[,] Weights { get; set; }
        public double[,] Output { get; set; }

        public MaxPoolingNeuron(int inWidth, int inHeight, int kernelSize = 2)
        {
            if (inWidth % kernelSize != 0 || inHeight % kernelSize != 0)
            {
                throw new ArgumentException("Размер входного изображения должен быть кратным размеру ядра");
            }

            this._kernelSize = kernelSize;
            Weights = new double[kernelSize, kernelSize];
            Output = new double[inHeight / kernelSize, inWidth / kernelSize];
        }

        public void Randomize()
        {
            int y, x;

            for (y = 0; y < _kernelSize; y++)
            {
                for (x = 0; x < _kernelSize; x++)
                {
                    Weights[y, x] = Random.NextDouble() * 255;
                }
            }
        }
        
        public double[,] Compute(double[,] input)
        {
            int i, y, x, h, w;
            var outputHeight = Output.GetLength(0);
            var outputWidth = Output.GetLength(1);
            
            //сканируем изображение ядром
            for (y = 0; y < outputHeight; y += _kernelSize)
            {
                for (x = 0; x < outputWidth; x += _kernelSize)
                {
                    for (h = 0; h < _kernelSize; h++)
                    {
                        for (w = 0; w < _kernelSize; w++)
                        {
                            Output[y, x] = Output[y, x] > input[y + h, x + w] ? Output[y, x] : input[y + h, x + w];
                        }
                    }
                }
            }

            return Output;
        }
    }
}
