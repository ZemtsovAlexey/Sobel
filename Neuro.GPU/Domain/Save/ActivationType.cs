using System;

namespace Neuro.Neurons
{
    [Serializable]
    public enum ActivationType
    {
        BipolarSigmoid,
        Sigmoid,
        ELU,
        LeakyReLu,
        ReLu,
        LeCunTanh,
        AbsoluteReLU
    }
}