using System;

namespace Neuro.Models
{
    [Serializable]
    public enum LayerType
    {
        Convolution = 0,
        FullyConnected = 1,
        MaxPoolingLayer = 2,
        Softmax = 3,
        Dropout = 4
    }
}