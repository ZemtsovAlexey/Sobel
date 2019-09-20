using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Neuro.ActivationFunctions;
using Neuro.Domain.Layers;
using Neuro.Models;
using Neuro.Neurons;

namespace Neuro.Layers
{
    public class FullyConnectedLayer : IFullyConnectedLayer
    {
        public int Index { get; private set; }
        public LayerType Type { get; set; } = LayerType.FullyConnected;
        public ActivationType ActivationFunctionType { get; }
        public FullyConnectedNeuron[] Neurons { get; }
        public float[] Outputs { get; set; }
        public int NeuronsCount => Neurons.Length;
        public FullyConnectedNeuron this[int index] => Neurons[index];
        public IActivationFunction Function { get; }

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
        public extern static void Multiply2GPU(float[] output, float[] input, float[] weights, int len, int wlen, int nlen);

        public FullyConnectedLayer(int neuronsCount, ActivationType activationType)
        {
            ActivationFunctionType = activationType;
            Function = activationType.Get();
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            Outputs = new float[neuronsCount];

        }
        
        public FullyConnectedLayer(int neuronsCount, int inputsCount, IActivationFunction activationFunction)
        {
            neuronsCount = Math.Max(1, neuronsCount);
            Neurons = new FullyConnectedNeuron[neuronsCount];
            
            for (var i = 0; i < neuronsCount; i++)
            {
                Neurons[i] = new FullyConnectedNeuron(inputsCount, activationFunction);
            }

            Outputs = new float[neuronsCount];

        }

        public void Init(int index, int inputsCount)
        {
            Index = index;

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

        public float[] Compute(float[] inputs)
        {
            var outputs = Neurons.AsParallel().Select(n => n.Compute(inputs)).ToArray();
            //var o = Compute2(inputs);

            Outputs = outputs;
            
            return outputs;
        }

        public float[] Compute2(float[] inputs)
        {
            var weights = Neurons.SelectMany(x => x.Weights).ToArray();
            var outputs = new float[Neurons.Length];

            Multiply2GPU(outputs, inputs, weights, inputs.Length, weights.Length, Neurons.Length);
            var a = Neurons[0].Function;

            for (var i = 0; i < outputs.Length; i++)
            {
                outputs[i] = a.Activation(outputs[i]);
                Neurons[i].Output = outputs[i];
            }

            Outputs = outputs;

            return outputs;
        }
    }
}