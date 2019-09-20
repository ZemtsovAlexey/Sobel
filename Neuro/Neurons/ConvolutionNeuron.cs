using System;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Extensions;
using Neuro.Models;

namespace Neuro.Neurons
{
    public class ConvolutionNeuron
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private readonly int _kernelSize;

        public IActivationFunction Function { get; set; }
        public Matrix Weights { get; set; }
        public float Bias { get; set; } = 0;
        public Matrix Output { get; set; }
        public Matrix Output2 { get; set; }
        public int Padding { get; } = 1;
        public int[] ParentId { get; }

        public ConvolutionNeuron(IActivationFunction function, int inWidth, int inHeight, int kernelSize = 3, int[] parentId = null)
        {
            _kernelSize = kernelSize;
            Weights = new Matrix(new float[kernelSize, kernelSize]);
            Output = new Matrix(new float[inHeight - kernelSize + Padding, inWidth - kernelSize + Padding]);
            Output2 = new Matrix(new float[inHeight - kernelSize + Padding, inWidth - kernelSize + Padding]);
            Function = function;
            ParentId = parentId;
        }

        public void Randomize()
        {
            int y, x;
            var weight = Random.NextFloat() * (Function.MaxRange - 0) + 0;
            for (y = 0; y < _kernelSize; y++)
            {
                for (x = 0; x < _kernelSize; x++)
                {
                    Weights[y, x] = weight;// Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
                }
            }

            Bias = Random.NextFloat() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
        }
        
        public Matrix Compute(Matrix[] inputs, bool useGpu = false)
        {
            var input = ParentId != null && ParentId.Any() ? inputs.Where((x, i) => ParentId.Contains(i)).ToArray().Sum() : inputs.Sum();
            var output = (useGpu ? (input.Convolution2(Weights, Padding) + Bias) : (input.Convolution(Weights, Padding) + Bias)) * Function.Activation;

            Output = output;
            
            return output;
        }
    }
}
