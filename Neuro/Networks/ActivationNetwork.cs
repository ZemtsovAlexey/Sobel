using System;
using Neuro.ActivationFunctions;
using Neuro.Layers;

namespace Neuro.Networks
{
    public class ActivationNetwork// : Network
    {
        public float[] Output;
        public FullyConnectedLayer[] Layers { get; set; }
        public int LayersCount => Layers.Length;
        public FullyConnectedLayer this[int index] => Layers[index];
        
        public ActivationNetwork(IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            var layersCount = Math.Max(1, neuronsCount.Length);
            Layers = new FullyConnectedLayer[layersCount];
            
            for (var i = 0; i < neuronsCount.Length; i++)
            {
                Layers[i] = new FullyConnectedLayer(neuronsCount[i], i == 0 ? inputsCount : neuronsCount[i - 1], function);
            }
        }

        public ActivationNetwork(params FullyConnectedLayer[] layers)
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
        
        public float[] Compute(float[] input)
        {
            Output = input;

            for (var i = 0; i < Layers.Length; i++)
            {
                Output = Layers[i].Compute(i == 0 ? input : Output);
            }

            return Output;
        }
        
        /*public ActivationNetwork Load(byte[] data)
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
        }*/
        
        /*public byte[] Save()
        {
            var data = new SaveNetworkModel {Layers = new List<LayerSaveData>()};

            foreach (var layer in Layers)
            {
                var layerSaveData = new LayerSaveData {Neurons = new List<FullyConnectedNeuronSaveData>()};

                foreach (var neuron in layer.Neurons)
                {
                    var weights = new float[neuron.Weights.Length];

                    for (var i = 0; i < neuron.Weights.Length; i++)
                    {
                        weights[i] = neuron.Weights[i];
                    }
                    
                    layerSaveData.Neurons.Add(new FullyConnectedNeuronSaveData { Weights = weights });
                }
                
                data.Layers.Add(layerSaveData);
            }
            
            if(!data.Layers.Any())
                return null;

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            
            bf.Serialize(ms, data);

            return ms.ToArray();
        }*/
    }
}