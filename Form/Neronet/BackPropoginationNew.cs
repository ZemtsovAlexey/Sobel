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
                    new ConvolutionalLayer(activation, 5, 20, 20),
//                    new MaxPoolingLayer(5, 18, 18)
                },
                new IFullyConnectedLayer[]
                {
                    new ActivationLayer(15, 405, activation),
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

        public void SearchSolution(double[,] inputs, double[] outputs)
        {
            var teacher = new ConvolutionalBackPropagationLearning(Network)
            {
                LearningRate = LearningRate
            };

            teacher.Run(inputs, outputs);
        }

        public void SearchSolutionStop()
        {
            _needToStop = true;
        }
    }
}
