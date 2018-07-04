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
            var relu = new ReluFunction();
            var sigmoid = new SigmoidFunction();

            /*Network = new ConvolutionalNetwork(
                new IConvolutionalLayer[]
                {
                    new ConvolutionalLayer(relu, 5, 20, 20, 7),
                    new ConvolutionalLayer(relu, 8, 14, 14, 3),
//                    new ConvolutionalLayer(relu, 10, 10, 10, 5),
//                    new ConvolutionalLayer(relu, 15, 6, 6),
//                    new ConvolutionalLayer(relu, 20, 4, 4, 3),
//                    new MaxPoolingLayer(5, 18, 18)
                },
                new IFullyConnectedLayer[]
                {
                    new ActivationLayer(13, 1152, activation),
                    new ActivationLayer(13, 13, activation),
//                    new ActivationLayer(15, 15, activation),
                    new ActivationLayer(1, 13, activation)
                });*/

            Network = new ConvolutionalNetwork();

            Network.InitLayers(
                new ConvolutionalLayer(relu, 5, 20, 20, 7),
                new ConvolutionalLayer(relu, 8, 14, 14, 3),
                new ActivationLayer(13, 1152, activation),
                new ActivationLayer(13, 13, activation),
                new ActivationLayer(1, 13, activation)
                );
            
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
