using Neuro.Domain.Layers;
using Neuro.Models;
using System;
using System.Linq;

namespace Neuro.Layers
{
    public class DropoutLayer : IDropoutLayer
    {
        public int Index { get; private set; }

        public LayerType Type => LayerType.Dropout;

        public int NeuronsCount { get; }

        public double DropProbability { get; }

        private Random Random = new Random((int)DateTime.Now.Ticks);

        public DropoutLayer(double dropProbability)
        {
            DropProbability = dropProbability;
        }

        public void Init(int index)
        {
            Index = index;
        }

        public double[] Derivative(double[] inputs)
        {
            return inputs.Select((x, i) =>
            {
                var nextDouble = Random.NextDouble();

                if (nextDouble < DropProbability)
                {
                    return 0;
                }

                return x / (1 - DropProbability);
            }).ToArray();
        }
    }
}