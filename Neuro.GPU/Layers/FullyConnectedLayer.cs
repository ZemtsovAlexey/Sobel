﻿using System;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class FullyConnectedLayer : IFullyConnectedLayer
    {
        public LayerType Type { get; set; } = LayerType.FullyConnected;
        public ActivationType ActivationFunctionType { get; }
        public FullyConnectedNeuron[] Neurons { get; }
        public double[] Outputs { get; }
        public int NeuronsCount => Neurons.Length;
        public FullyConnectedNeuron this[int index] => Neurons[index];
        public IActivationFunction Function { get; }
        
        public FullyConnectedLayer(int neuronsCount, ActivationType activationType)
        {
            ActivationFunctionType = activationType;
            Function = activationType.Get();
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            Outputs = new double[neuronsCount];

        }
        
        public FullyConnectedLayer(int neuronsCount, int inputsCount, IActivationFunction activationFunction)
        {
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            
            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, activationFunction);
            }

            Outputs = new double[neuronsCount];

        }

        public void Init(int inputsCount)
        {
            for (var i = 0; i < NeuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, Function);
            }
        }
        
        public void Randomize()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Randomize();
            }
        }

        public double[] Compute(double[] inputs)
        {
            Neurons
                .Select((neuron, i) => new {i, neuron})
                .AsParallel()
                .ForAll((item) => { Outputs[item.i] = item.neuron.Compute(inputs); });

            return Outputs;
        }
    }
}