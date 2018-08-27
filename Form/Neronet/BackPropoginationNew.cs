using System.Drawing;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Networks;
using Neuro.Neurons;
using ScannerNet.Extensions;

namespace Sobel.Neronet
{
    public class BackPropoginationNew
    {
        public ConvolutionalNetwork Network;

        public int Iterations = 2000;
        public float LearningRate = 0.01f;
        public float ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = ActivationType.BipolarSigmoid;
            var relu = ActivationType.ReLu;

            Network = new ConvolutionalNetwork();

            Network.InitLayers(26, 26,
                new ConvolutionalLayer(relu, 5, 3),//24
                new MaxPoolingLayer(2),//12
                new ConvolutionalLayer(relu, 10, 3),//10
                new MaxPoolingLayer(2),//5
                //new FullyConnectedLayer(100, activation),
                new FullyConnectedLayer(50, activation),
                new FullyConnectedLayer(50, activation),
                new FullyConnectedLayer(1, activation)
                );
            
            Network.Randomize();
        }
        
        public float[] Compute(Bitmap bmp)
        {
            return Network.Compute(bmp.GetDoubleMatrix());
        }

        public void SearchSolution(Bitmap bmp, float[] outputs)
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
