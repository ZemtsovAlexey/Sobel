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
        private int shapeOut = 3;
        private int shapeDiff = 0;
//        private Func<double, double> eventLog;
        public event EventHandler<double> EventHandler;

        public void InitNet()
        {
//            this.eventLog = eventLog;
            shapeDiff = (shape - shapeOut) / 2;
            Network = new Network();
            
            Network.InitLayers(shape, shape,
                new ConvolutionLayer(ActivationType.BipolarSigmoid, 18, 3, true),
//                new MaxPoolingLayer(2),
//                new ConvolutionLayer(ActivationType.BipolarSigmoid, 16, 3, true),
//                new ConvolutionLayer(ActivationType.BipolarSigmoid, 18, 3, true),
                new FullyConnectedLayer(32, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(20, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(15, ActivationType.BipolarSigmoid),
//                new FullyConnectedLayer(10, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(shapeOut * shapeOut, ActivationType.BipolarSigmoid)
            );
            
            Network.Randomize();
        }
        
        public Bitmap Restore(Bitmap actual)
        {
            var actualMap = actual.GetDoubleMatrix(invert: false, optimize: false);
            var height = actualMap.GetLength(0);// - shape;
            var width = actualMap.GetLength(1);// - shape;
            var resultMap = new double[height, width];

            for (var y = 0; y < height - shape; y = y + shapeOut)
            {
                for (var x = 0; x < width - shape; x = x + shapeOut)
                {
//                    if (actualMap[y + shapeDiff, x + shapeDiff] >= 255d)
//                        continue;
                    
                    var input = new double[shape, shape];
                    
                    for (var h = 0; h < shape; h++)
                    {
                        for (var w = 0; w < shape; w++)
                        {
                            var value = actualMap[y + h, x + w];
                            input[h, w] = value;
                        }
                    }
                    
                    var result = Network.Compute(input);
                    
                    for (var h = 0; h < shapeOut; h++)
                    {
                        for (var w = 0; w < shapeOut; w++)
                        {
                            resultMap[y + shapeDiff + h, x + shapeDiff + w] = result[h * shapeOut + w];
                        }
                    }

                }
            }
           
            /*for (var y = 0; y < height; y = y + shapeOut)
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
            }*/

            return resultMap.ToBitmap();
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
                        
//                        if (actualMap[y + shapeDiff, x + shapeDiff] >= 255d)
//                            continue;
                        
                        var input = new double[shape, shape];
                    
                        for (var h = 0; h < shape; h++)
                        {
                            for (var w = 0; w < shape; w++)
                            {
                                var value = actualMap[y + h, x + w];
                                input[h, w] = value;
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