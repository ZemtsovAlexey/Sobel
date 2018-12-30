using System.Drawing;
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
            var activation = ActivationType.BipolarSigmoid;
            var relu = ActivationType.ReLu;

            Network = new Network();

            Network.InitLayers(16, 16,
                new ConvolutionLayer(activation, 4, 3),//24
                new ConvolutionLayer(activation, 8, 3),//24
                new ConvolutionLayer(activation, 16, 3),//10
                new MaxPoolingLayer(2),//10
                //new FullyConnectedLayer(100, activation),
                new FullyConnectedLayer(43, activation),
                new FullyConnectedLayer(43, activation),
                new FullyConnectedLayer(43, activation)
                );
            
            Network.Randomize();
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
