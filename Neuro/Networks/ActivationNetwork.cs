using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class ActivationNetwork// : Network
    {
        public double[] Output;
        public ActivationLayer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
        public ActivationLayer this[int index] => Layers[index];
        
        public ActivationNetwork(IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            var layersCount = Math.Max(1, neuronsCount.Length);
            Layers = new ActivationLayer[layersCount];
            
            for (var i = 0; i < neuronsCount.Length; i++)
            {
                Layers[i] = new ActivationLayer(neuronsCount[i], i == 0 ? inputsCount : neuronsCount[i - 1], function);
            }
        }

        public ActivationNetwork(params ActivationLayer[] layers)
        {
            Layers = layers;
        }

        public ActivationNetwork()
        {
        }

        public void Randomize()
        {
            foreach (var layer in Layers)
            {
                layer.Randomize();
            }
        }
        
        public double[] Compute(double[] input)
        {
            Output = input;

            for (var i = 0; i < Layers.Length; i++)
            {
                Output = Layers[i].Compute(i == 0 ? input : Output);
            }

            return Output;
        }
        
        public ActivationNetwork Load(byte[] data)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            
            var obj = (SaveNetworkModel) binForm.Deserialize(memStream);

            var inputCount = obj.Layers[0].Neurons[0].Weights.Length;
            var neuronsCount = obj.Layers.Select(x => x.Neurons.Count).ToArray();

            var network = new ActivationNetwork(new BipolarSigmoidFunction(), inputCount, neuronsCount);

            for (var lIndex = 0; lIndex < obj.Layers.Count; lIndex++)
            {
                var neurons = obj.Layers[lIndex].Neurons;
                
                for (var nIndex = 0; nIndex < neurons.Count; nIndex++)
                {
                    var weigths = neurons[nIndex].Weights;

                    for (var wIndex = 0; wIndex < weigths.Length; wIndex++)
                    {
                        network[lIndex][nIndex].Weights[wIndex] = weigths[wIndex];
                    }
                }
            }
            
            return network;
        }
        
        public byte[] Save()
        {
            var data = new SaveNetworkModel {Layers = new List<LayerSaveData>()};

            foreach (var layer in Layers)
            {
                var layerSaveData = new LayerSaveData {Neurons = new List<NeuronSaveData>()};

                foreach (var neuron in layer.Neurons)
                {
                    var weights = new double[neuron.Weights.Length];

                    for (var i = 0; i < neuron.Weights.Length; i++)
                    {
                        weights[i] = neuron.Weights[i];
                    }
                    
                    layerSaveData.Neurons.Add(new NeuronSaveData { Weights = weights });
                }
                
                data.Layers.Add(layerSaveData);
            }
            
            if(!data.Layers.Any())
                return null;

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            
            bf.Serialize(ms, data);

            return ms.ToArray();
        }
    }
}