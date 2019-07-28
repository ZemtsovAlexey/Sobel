using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;
using ScannerNet;
using ScannerNet.Extensions;

namespace Sobel.UI
{
    public class RestoredNet
    {
        public Network Network;
        public bool Running;
        private int shape = 5;
        private int shapeOut = 5;
        private int shapeDiff = 0;
//        private Func<double, double> eventLog;
        public event EventHandler<double> EventHandler;

        public void InitNet()
        {
//            this.eventLog = eventLog;
            shapeDiff = (shape - shapeOut) / 2;
            Network = new Network();
            
            Network.InitLayers((shape, shape),
//                new ConvolutionLayer(ActivationType.BipolarSigmoid, 8, 3, true),
//                new MaxPoolingLayer(2),
//                new ConvolutionLayer(ActivationType.BipolarSigmoid, 16, 3, true),
//                new ConvolutionLayer(ActivationType.ReLu, 16, 3),
                new FullyConnectedLayer(36, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(36, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(shapeOut * shapeOut, ActivationType.BipolarSigmoid)
            );
            
            Network.Randomize();
        }
        
        public Bitmap Restore(Bitmap actual)
        {
            var actualMap = actual.GetDoubleMatrix(invert: false, optimize: false);
            var expectedMap = new double[actualMap.GetLength(0), actualMap.GetLength(1)];
            var height = expectedMap.GetLength(0) - shape;
            var width = expectedMap.GetLength(1) - shape;

            /*Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    var input = new double[shape, shape];
                    
                    for (var h = 0; h < shape; h++)
                    {
                        for (var w = 0; w < shape; w++)
                        {
                            input[h, w] = actualMap[y + h, x + w];
                        }
                    }
                    
                    var result = Network.Compute(input);
                    
                    for (var h = 0; h < shapeOut; h++)
                    {
                        for (var w = 0; w < shapeOut; w++)
                        {
                            actualMap[y + shapeDiff + h, x + shapeDiff + w] = result[h * shapeOut + w];
                        }
                    }
                });
            });*/
           
            for (var y = 0; y < height; y = y + shapeOut)
            {
                for (var x = 0; x < width; x = x + shapeOut)
                {
                    var input = new double[shape, shape];
                    
                    for (var h = 0; h < shape; h++)
                    {
                        for (var w = 0; w < shape; w++)
                        {
                            input[h, w] = actualMap[y + h, x + w];
                        }
                    }
                    
                    var result = Network.Compute(input);
                    
                    for (var h = 0; h < shapeOut; h++)
                    {
                        for (var w = 0; w < shapeOut; w++)
                        {
                            actualMap[y + shapeDiff + h, x + shapeDiff + w] = result[h * shapeOut + w];
                        }
                    }

                }
            }

            return actualMap.ToBitmap();
        }
        
        public void Learn(Bitmap actual, Bitmap expected, double learningRate)
        {
            Running = true;
            
            double totalTime = 0;
            double totalError = 0;
            var teacher = new Neuro.Learning.BackPropagationLearning(Network)
            {
                LearningRate = learningRate
            };

            var expectedMap = expected.GetDoubleMatrix(invert: false, optimize: false);
            var actualMap = actual.GetDoubleMatrix(invert: false, optimize: false);

            var st = new Stopwatch();
            var i = 0;

            while (Running)
            {
                for (var y = 0; y < expectedMap.GetLength(0) - shape; y = y + shapeOut)
                {
                    for (var x = 0; x < expectedMap.GetLength(1) - shape; x = x + shapeOut)
                    {
                        if (!Running)
                            return;
                        
                        var input = new double[shape, shape];
                    
                        for (var h = 0; h < shape; h++)
                        {
                            for (var w = 0; w < shape; w++)
                            {
                                input[h, w] = actualMap[y + h, x + w];
                            }
                        }
                    
                        var expectedOutput = new double[shapeOut * shapeOut];
                    
                        for (var h = 0; h < shapeOut; h++)
                        {
                            for (var w = 0; w < shapeOut; w++)
                            {
                                expectedOutput[h * shapeOut + w] = expectedMap[y + shapeDiff + h, x + shapeDiff + w];
                            }
                        }

                        Network.Compute(input);
                        totalError += teacher.Run(input, expectedOutput);

                        i++;
//                    this.eventLog.Invoke(totalError);
                        if (i % 20 == 0)
                            EventHandler.Invoke(this, totalError/i);
                    }
                }
            }
            
            Running = false;
        }
    }
}