﻿using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Domain.Layers
{
    public interface IFullyConnectedLayer : ILayer, IRandomize, ILinearCompute
    {
        ActivationType ActivationFunctionType { get; }
        
        FullyConnectedNeuron[] Neurons { get; }
        
        IActivationFunction Function { get; }
        
        FullyConnectedNeuron this[int index] { get; }

        void Init(int index, int inputsCount);
    }
}
