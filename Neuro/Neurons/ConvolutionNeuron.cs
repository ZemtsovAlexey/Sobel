using System;
using Neuro.ActivationFunctions;
using Neuro.Extensions;
using Neuro.Models;

namespace Neuro.Neurons
{
    public class ConvolutionNeuron //: Neuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private readonly int _kernelSize;

        public IActivationFunction Function { get; set; }
        public Matrix Weights { get; set; }
        public double Bias { get; set; } = 0;
        public Matrix Output { get; set; }
        public int Padding { get; } = 1;

        public ConvolutionNeuron(IActivationFunction function, int inWidth, int inHeight, int kernelSize = 3)
        {
            _kernelSize = kernelSize;
            Weights = new Matrix(new double[kernelSize, kernelSize]);
            Output = new Matrix(new double[inHeight - kernelSize + Padding, inWidth - kernelSize + Padding]);
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
        
        public Matrix Compute(Matrix[] input)
        {
            var output = (input.Sum().Convolution(Weights, Padding) + Bias) * Function.Activation;

            Output = output;
            
            return output;
        }
    }
}
