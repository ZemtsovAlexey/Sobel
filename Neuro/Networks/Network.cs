using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Networks
{
    public class Network
    {
        protected double[,] Output;

        public Layer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
        public Layer this[int index] => Layers[index];

        protected Network()
        {
        }
        
        public Network(params Layer[] layers)
        {
            Layers = new Layer[layers.Length];

            for (var i = 0; i < layers.Length; i++)
            {
                Layers[i] = layers[i];
            }
        }
        
        protected Network(int layersCount)
        {
            layersCount = Math.Max(1, layersCount);
            Layers = new Layer[layersCount];
        }

        public virtual void Randomize()
        {
            foreach (var layer in Layers)
            {
                layer.Randomize();
            }
        }

        public double[,] Compute(double[,] input)
        {
            Output = input;

            for (var i = 0; i < Layers.Length; i++)
            {
                Output = Layers[i].Compute(i == 0 ? input : Output);
            }

            return Output;
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

        public virtual Network Load(byte[] data)
        {
            return null;
        }
    }
}