using System.Drawing;
using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Networks;
using ScannerNet.Extensions;

namespace Sobel.Neronet
{
    public class BackPropoginationNew
    {
        public ConvolutionalNetwork Network;

        public int Iterations = 2000;
        public double LearningRate = 0.01;
        public double ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = new BipolarSigmoidFunction();
            var relu = new ReluFunction();
            var sigmoid = new SigmoidFunction();

            Network = new ConvolutionalNetwork();

            Network.InitLayers(20, 20,
                new ConvolutionalLayer(activation, 5, 5),
                new MaxPoolingLayer(2),
                new ConvolutionalLayer(activation, 50, 3),
                new MaxPoolingLayer(2),
                new FullyConnectedLayer(20, activation),
                new FullyConnectedLayer(20, activation),
                new FullyConnectedLayer(1, activation)
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
