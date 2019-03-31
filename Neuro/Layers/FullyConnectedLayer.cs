using System;
using System.Diagnostics;
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
        public LayerType Type { get; set; } = LayerType.FullyConnected;
        public ActivationType ActivationFunctionType { get; }
        public FullyConnectedNeuron[] Neurons { get; }
        public double[] Outputs { get; private set; }
        public int NeuronsCount => Neurons.Length;
        public FullyConnectedNeuron this[int index] => Neurons[index];
        public IActivationFunction Function { get; }

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\CudaNeuroSDK.dll")]
        public extern static void Multiply2GPU(double[] output, double[] input, double[] weights, int len, int wlen, int nlen);

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
            //var st = Stopwatch.StartNew();
            //return ComputeGPU(inputs);

            //st.Stop();
            //Console.WriteLine($"gpu - {st.Elapsed}");
            //st.Restart();

            var outputs = Neurons.AsParallel().Select(n => n.Compute(inputs)).ToArray();

            //st.Stop();
            //Console.WriteLine($"cpu - {st.Elapsed}");

            Outputs = outputs;
            
            return outputs;
        }

        public double[] ComputeGPU(double[] inputs)
        {
            var weights = Neurons.SelectMany(x => x.Weights).ToArray();
            var outputs = new double[Neurons.Length];

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