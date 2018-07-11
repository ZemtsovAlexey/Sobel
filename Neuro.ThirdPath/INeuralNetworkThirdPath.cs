using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using NeuralNetworkNET.APIs.Structs;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace Neuro.ThirdPath
{
    public class INeuralNetworkThirdPath
    {
        private INeuralNetwork network;

        public void Init()
        {
            INeuralNetwork network = NetworkManager.NewSequential(TensorInfo.Image<Alpha8>(20, 20),
                CuDnnNetworkLayers.Convolutional((5, 5), 20, ActivationType.Identity),
                CuDnnNetworkLayers.Pooling(ActivationType.LeakyReLU),
                CuDnnNetworkLayers.Convolutional((3, 3), 40, ActivationType.LeakyReLU),
                CuDnnNetworkLayers.Pooling(ActivationType.LeakyReLU),
                CuDnnNetworkLayers.FullyConnected(125, ActivationType.LeakyReLU),
                CuDnnNetworkLayers.FullyConnected(64, ActivationType.LeakyReLU),
                CuDnnNetworkLayers.Softmax(10));

        }

        public void Train(IEnumerable<(float[] x, float[] u)> trainData)
        {
            ITrainingDataset dataset = DatasetLoader.Training(trainData, 100);

            TrainingSessionResult result = NetworkManager.TrainNetwork(
                network,                                // The network instance to train
                dataset,                                // The ITrainingDataset instance   
                TrainingAlgorithms.AdaDelta(),          // The training algorithm to use
                60,                                     // The expected number of training epochs to run
                0.5f,                                   // Dropout probability
                p => { },                               // Optional training epoch progress callback
                null,                                   // Optional callback to monitor the training dataset accuracy
                null);
        }

        public float[] Compute(float[] x)
        {
            return network.Forward(x);
        }
    }
}
