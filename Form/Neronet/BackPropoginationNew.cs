using System.Drawing;
using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Networks;
using ScannerNet.Extensions;
using Network = Neuro.Networks.Network;

namespace Sobel.Neronet
{
    public class BackPropoginationNew
    {
        public ConvolutionalNetwork Network;

        public int Iterations = 2000;
        public double LearningRate = 0.05;
        public double ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = new BipolarSigmoidFunction();

            Network = new ConvolutionalNetwork(
                new IConvolutionalLayer[]
                {
                    new ConvolutionalLayer(activation, 2, 20, 20),
                    new ConvolutionalLayer(activation, 3, 18, 18),
                    new ConvolutionalLayer(activation, 4, 16, 16),
                    new ConvolutionalLayer(activation, 4, 14, 14),
                    new ConvolutionalLayer(activation, 4, 12, 12),
                    new ConvolutionalLayer(activation, 4, 10, 10),
//                    new MaxPoolingLayer(5, 18, 18)
                },
                new IFullyConnectedLayer[]
                {
                    new ActivationLayer(15, 400, activation),
                    new ActivationLayer(15, 15, activation),
                    new ActivationLayer(15, 15, activation),
                    new ActivationLayer(1, 15, activation)
                });

            Network.Randomize();
        }
        
        public double[] Compute(Bitmap bmp)
        {
            return Network.Compute(bmp.GetDoubleMatrix());
        }

        public void SearchSolution(Bitmap bmp, double[] outputs)
        {
            var teacher = new ConvolutionalBackPropagationLearning(Network)
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
