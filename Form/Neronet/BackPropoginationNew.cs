using System.Drawing;
using Neuro.Extensions;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;
using ScannerNet.Extensions;

namespace Sobel.Neronet
{
    public class BackPropoginationNew
    {
        public Network Network;

        public int Iterations = 2000;
        public double LearningRate = 0.01f;
        public double ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var function = ActivationType.Sigmoid;

            Network = new Network();

            Network.InitLayers(28, 28,
                //new ConvolutionLayer(function, 8, 5),//24
                //new MaxPoolingLayer(2),
                //new ConvolutionLayer(function, 16, 3),//10
                //new MaxPoolingLayer(2),//10
                new FullyConnectedLayer(50, function),
                new FullyConnectedLayer(20, function),
                new FullyConnectedLayer(10, function)
                );

            Network.Randomize();
        }
        
        public double[] Compute(Bitmap bmp)
        {
            return Network.Compute(bmp.GetDoubleMatrix());
        }

        public double[] Compute(Matrix bmp)
        {
            return Network.Compute(bmp.Value);
        }

        public void SearchSolution(Bitmap bmp, double[] outputs)
        {
            var teacher = new BackPropagationLearning(Network)
            {
                LearningRate = LearningRate
            };

            teacher.Run(bmp.GetDoubleMatrix(), outputs);
        }

        public void SearchSolutionStop()
        {
            _needToStop = true;
        }
    }
}
