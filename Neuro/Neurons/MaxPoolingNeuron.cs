using System;

namespace Neuro.Neurons
{
    public class MaxPoolingNeuron
    {
        private int _kernelSize = 2;

        public float[,] Outputs { get; private set; }
        public bool[,] OutputCords { get; private set; }

        public MaxPoolingNeuron(int inWidth, int inHeight, int kernelSize = 2)
        {
            if (inWidth % kernelSize != 0 || inHeight % kernelSize != 0)
            {
                throw new ArgumentException("Размер входного изображения должен быть кратным размеру ядра");
            }

            _kernelSize = kernelSize;
            Outputs = new float[inHeight / kernelSize, inWidth / kernelSize];
            OutputCords = new bool[inHeight / kernelSize, inWidth / kernelSize];
        }
        
        public float[,] Compute(float[,] input)
        {
            int y, x, h, w, iy, ix;
            var outputHeight = Outputs.GetLength(0);
            var outputWidth = Outputs.GetLength(1);
            
            var outputs = new float[outputHeight, outputWidth];
            OutputCords = new bool[outputHeight, outputWidth];
            
            //сканируем изображение ядром
            for (y = 0; y < outputHeight; y++)
            {
                for (x = 0; x < outputWidth; x++)
                {
                    for (h = 0; h < _kernelSize; h++)
                    {
                        for (w = 0; w < _kernelSize; w++)
                        {
                            iy = y * _kernelSize;
                            ix = x * _kernelSize;
					
                            if (outputs[y, x] < input[iy + h, ix + w])
                            {
                                outputs[y, x] = input[iy + h, ix + w];
                                OutputCords[y, x] = true;
                            }
                        }
                    }
                }
            }

            Outputs = outputs;

            return outputs;
        }
    }
}
