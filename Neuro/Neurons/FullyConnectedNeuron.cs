﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Neuro.ActivationFunctions;
using Neuro.Extensions;

namespace Neuro.Neurons
{
    public class FullyConnectedNeuron
    {
        public double[] Weights { get; set; }
        public double Output { get; private set; }
        public IActivationFunction Function { get; }

        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        public FullyConnectedNeuron(int inputsCount, IActivationFunction function)
        {
            inputsCount = Math.Max(1, inputsCount);
            Weights = new double[inputsCount];
            Function = function;
        }

        public void Randomize()
        {
            for (var i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.NextDouble() * (Function.MaxRange - Function.MinRange) + Function.MinRange;
            }
        }

        public unsafe double Compute(double[] input)
        {
            double e = 0;

            fixed (double* w = Weights, i = input)
                for (var n = 0; n < input.Length; n++)
                    e += w[n] * i[n];

            var output = Function.Activation(e);

            Output = output;

            return output;
        }
    }
}