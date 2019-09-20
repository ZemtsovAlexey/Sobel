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
        public float LearningRate = 0.01f;
        public float ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = ActivationType.BipolarSigmoid;
            var relu = ActivationType.ReLu;

            Network = new Network();

            Network.InitLayers((12, 12),
                new DropoutLayer(0.3f),
                new ConvolutionLayer(activation, 4, 5, false),//24
                new ConvolutionLayer(activation, 8, 3, false),//24
//                new ConvolutionLayer(activation, 4, 3, true),
                new MaxPoolingLayer(2),
                new FullyConnectedLayer(50, activation),
                new SoftmaxLayer(34)
                );

            Network.Randomize();
        }
        
        public float[] Compute(Matrix bmp)
        {
            return Network.Compute(bmp.Value);
        }
        
        public float[] TeachCompute(Matrix bmp)
        {
            return Network.TeachCompute(bmp.Value);
        }

        public void SearchSolution(Bitmap bmp, float[] outputs)
        {
            var teacher = new BackPropagationLearning(Network)
            {
                LearningRate = LearningRate
            };

            teacher.Run(bmp.GetMatrix(), outputs);
        }

        public void SearchSolutionStop()
        {
            _needToStop = true;
        }
    }
}
