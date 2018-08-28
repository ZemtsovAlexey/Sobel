using Neuro.Models;
using System;

namespace Neuro.Neurons
{
    public class MaxPoolingNeuron
    {
        private int inputHeight;
        private int inputWidth;

        public int KernelSize = 2;
        public Matrix Outputs { get; private set; }
        public bool[,] OutputCords { get; private set; }

        public MaxPoolingNeuron(int inWidth, int inHeight, int kernelSize = 2)
        {
            if (inWidth % kernelSize != 0 || inHeight % kernelSize != 0)
            {
                throw new ArgumentException("Размер входного изображения должен быть кратным размеру ядра");
            }

            KernelSize = kernelSize;
            inputHeight = inHeight;
            inputWidth = inWidth;
            Outputs = new Matrix(new double[inHeight / kernelSize, inWidth / kernelSize]);
            OutputCords = new bool[inHeight, inWidth];
        }
        
        public Matrix Compute(Matrix input)
        {
            int y, x, h, w, iy, ix;
            var outputHeight = Outputs.GetLength(0);
            var outputWidth = Outputs.GetLength(1);
            
            var outputs = new Matrix(new double[outputHeight, outputWidth]);
            var outputCords = new bool[inputHeight, inputWidth];
            
            //сканируем изображение ядром
            for (y = 0; y < outputHeight; y++)
            {
                for (x = 0; x < outputWidth; x++)
                {
                    iy = y * KernelSize;
                    ix = x * KernelSize;
                    (int y, int x) maxCord = (0, 0);

                    for (h = 0; h < KernelSize; h++)
                    {
                        for (w = 0; w < KernelSize; w++)
                        {
                            if (outputs[y, x] < input[iy + h, ix + w])
                            {
                                outputs[y, x] = input[iy + h, ix + w];
                                maxCord = (iy + h, ix + w);
                            }
                        }
                    }

                    outputCords[maxCord.y, maxCord.x] = true;
                }
            }

            Outputs = outputs;
            OutputCords = outputCords;

            return outputs;
        }
    }
}
