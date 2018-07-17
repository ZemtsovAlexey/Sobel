﻿using System.Drawing;
using Neuro.ActivationFunctions;
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
        public double LearningRate = 0.01;
        public double ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = ActivationType.BipolarSigmoid;
            var relu = ActivationType.ReLu;

            Network = new ConvolutionalNetwork();

            Network.InitLayers(28, 28,
                new ConvolutionalLayer(relu, 30, 5),
                new MaxPoolingLayer(2),
                new ConvolutionalLayer(relu, 40, 3),
                new MaxPoolingLayer(2),
//                new FullyConnectedLayer(200, activation),
                new FullyConnectedLayer(150, activation),
//                new FullyConnectedLayer(100, activation),
                new FullyConnectedLayer(100, activation),
                new FullyConnectedLayer(75, activation),
                new FullyConnectedLayer(50, activation),
                new FullyConnectedLayer(25, activation),
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
