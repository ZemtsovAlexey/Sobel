using System;
using System.Collections.Generic;

namespace Neuro.Neurons
{
    [Serializable]
    internal class SaveNetworkModel
    {
        public int InputCount { get; set; } = 1;
        
        public List<LayerSaveData> Layers { get; set; }
    }

    [Serializable]
    internal class LayerSaveData
    {
        public List<NeuronSaveData> Neurons { get; set; }
    }

    [Serializable]
    internal class NeuronSaveData
    {
        public double[] Weights { get; set; } 
    }
}