using Neuro.Domain.Layers;
using Neuro.Extensions;
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

        public float DropProbability { get; set; }

        private Random Random = new Random((int)DateTime.Now.Ticks);

        public DropoutLayer(float dropProbability)
        {
            DropProbability = dropProbability;
        }

        public void Init(int index)
        {
            Index = index;
        }

        public float[] Derivative(float[] inputs)
        {
            if (DropProbability <= 0)
            {
                return inputs;
            }
            
            return inputs.Select((x, i) =>
            {
                var nextfloat = Random.NextFloat();

                if (nextfloat < DropProbability)
                {
                    return 0;
                }

                return x / (1 - DropProbability);
            }).ToArray();
        }
    }
}