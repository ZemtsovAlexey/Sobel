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
        private int shape = 2;
        private int shapeOut = 2;
        private int shapeDiff = 0;
//        private Func<float, float> eventLog;
        public event EventHandler<float> EventHandler;

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
                //new DropoutLayer(0.5),
                new FullyConnectedLayer(100, ActivationType.BipolarSigmoid),
                new FullyConnectedLayer(shapeOut * shapeOut, ActivationType.BipolarSigmoid)
            );
            
            Network.Randomize();
        }
        
        public Bitmap Restore(Bitmap actual)
        {
            var actualMap = actual.GetMatrix(delimetr: 255, invert: false, optimize: false);
            var expectedMap = new float[actualMap.GetLength(0), actualMap.GetLength(1)];
            var height = expectedMap.GetLength(0) - shape;
            var width = expectedMap.GetLength(1) - shape;
           
            for (var y = 0; y < height; y = y + shapeOut)
            {
                for (var x = 0; x < width; x = x + shapeOut)
                {
                    var input = new float[shape, shape];
                    float inputSum = 0;
                    float maxInput = 0;

                    for (var h = 0; h < shape; h++)
                    {
                        for (var w = 0; w < shape; w++)
                        {
                            var value = actualMap[y + h, x + w];

                            input[h, w] = value;
                            inputSum += value;
                            maxInput = value > maxInput ? value : maxInput;
                        }
                    }

                    /*if (maxInput - (inputSum / (shape * 2)) < 0.3)
                        continue;*/

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

        public Bitmap Big(Bitmap actual)
        {
            var actualMap = actual.GetMatrix(invert: false, optimize: false);
            var resultMap = new float[actualMap.GetLength(0) * 2, actualMap.GetLength(1) * 2];
            var height = actualMap.GetLength(0);
            var width = actualMap.GetLength(1);

            for (var y = 0; y < height / shape; y += shape)
            {
                for (var x = 0; x < width / shape; x += shape)
                {
                    var input = new float[shape, shape];
                    float inputSum = 0;

                    for (var h = 0; h < shape; h++)
                    {
                        for (var w = 0; w < shape; w++)
                        {
                            var value = actualMap[y + h, x + w];

                            input[h, w] = value;
                            inputSum += value;
                        }
                    }

                    if (inputSum / (shape * 2) < 30)
                        continue;

                    var result = Network.Compute(input);

                    for (var h = 0; h < shapeOut; h++)
                    {
                        for (var w = 0; w < shapeOut; w++)
                        {
                            resultMap[y * 2 + h, x * 2 + w] = result[h * shapeOut + w];
                        }
                    }

                }
            }

            return resultMap.ToBitmap();
        }

        public void LearnBig(Bitmap actual, Bitmap expected, float learningRate)
        {
            Running = true;

            float totalTime = 0;
            float totalError = 0;
            var teacher = new Neuro.Learning.BackPropagationLearning(Network)
            {
                LearningRate = learningRate
            };

            var expectedMap = expected.GetMatrix(invert: false, optimize: false);
            var actualMap = actual.GetMatrix(invert: false, optimize: false);

            var st = new Stopwatch();
            var i = 0;

            while (Running)
            {
                for (var y = 0; y < actualMap.GetLength(0) / shape; y += shape)
                {
                    for (var x = 0; x < actualMap.GetLength(1) / shape; x += shape)
                    {
                        if (!Running)
                            return;

                        var input = new float[shape, shape];
                        float inputSum = 0;

                        for (var h = 0; h < shape; h++)
                        {
                            for (var w = 0; w < shape; w++)
                            {
                                var value = actualMap[y + h, x + w];

                                input[h, w] = value;
                                inputSum += value;
                            }
                        }

                        if (inputSum / (shape * 2) < 0.3)
                            continue;

                        var expectedOutput = new float[shapeOut * shapeOut];

                        for (var h = 0; h < shapeOut; h++)
                        {
                            for (var w = 0; w < shapeOut; w++)
                            {
                                expectedOutput[h * shapeOut + w] = expectedMap[y * 2 + h, x * 2 + w];
                            }
                        }

                        Network.Compute(input);
                        totalError += teacher.Run(input, expectedOutput);

                        i++;
                        //                    this.eventLog.Invoke(totalError);
                        if (i % 20 == 0)
                            EventHandler.Invoke(this, totalError / i);
                    }
                }
            }

            Running = false;
        }

        public void Learn(Bitmap actual, Bitmap expected, float learningRate)
        {
            Running = true;
            
            float totalTime = 0;
            float totalError = 0;
            var teacher = new Neuro.Learning.BackPropagationLearning(Network)
            {
                LearningRate = learningRate
            };

            var expectedMap = expected.GetMatrix(delimetr: 255, invert: false, optimize: false);
            var actualMap = actual.GetMatrix(delimetr: 255, invert: false, optimize: false);

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
                        
                        var input = new float[shape, shape];
                        float inputSum = 0;
                        float maxInput = 0;

                        for (var h = 0; h < shape; h++)
                        {
                            for (var w = 0; w < shape; w++)
                            {
                                var value = actualMap[y + h, x + w];

                                input[h, w] = value;
                                inputSum += value;
                                maxInput = value > maxInput ? value : maxInput;
                            }
                        }

                        if (maxInput - (inputSum / (shape * 2)) < 0.3)
                            continue;

                        var expectedOutput = new float[shapeOut * shapeOut];
                    
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