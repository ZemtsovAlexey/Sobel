using System;
using System.Collections.Generic;
using Neuro.Models;

namespace Neuro.Neurons
{
    [Serializable]
    internal class SaveNetworkModel
    {
        public int InputWidth { get; set; }
        
        public int InputHeight { get; set; }
        
        public List<LayerSaveData> Layers { get; set; }
    }

    [Serializable]
    internal class LayerSaveData
    {
        public LayerType Type { get; set; }
        
        public ActivationType ActivationType { get; set; }
        
        public int OutputLength { get; set; }
        
        public int KernelSize { get; set; }
        
        public List<FullyConnectedNeuronSaveData> FullyConnectedNeurons { get; set; }
        
        public List<ConvNeuronSaveData> ConvNeurons { get; set; }
    }

    [Serializable]
    internal class FullyConnectedNeuronSaveData
    {
        public float[] Weights { get; set; } 
    }
    
    [Serializable]
    internal class ConvNeuronSaveData
    {
        public int KernelSize { get; set; }

        public float[,] Weights { get; set; } 
    }
}