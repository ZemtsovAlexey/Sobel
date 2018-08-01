using System;
using Neuro.Neurons;

namespace Neuro.ActivationFunctions
{
    internal static class ActivationFunctionProvider
    {
        public static IActivationFunction Get(this ActivationType type)
        {
            switch (type)
            {
                case ActivationType.Sigmoid: return new Sigmoid();
                case ActivationType.BipolarSigmoid: return new BipolarSigmoid();
                case ActivationType.LeCunTanh: return new LeCunTanh();
                case ActivationType.ReLu: return new ReLU();
                case ActivationType.LeakyReLu: return new LeakyReLU();
                case ActivationType.AbsoluteReLU: return new AbsoluteReLU();
                case ActivationType.ELU: return new ELU();
                case ActivationType.None: return new None();
                default:
                    throw new ArgumentOutOfRangeException(nameof(ActivationType), "Unsupported activation function");
            }
        }
    }
}