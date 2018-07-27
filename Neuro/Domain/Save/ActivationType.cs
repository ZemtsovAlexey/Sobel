using System;

namespace Neuro.Neurons
{
    [Serializable]
    public enum ActivationType
    {
        None,
        BipolarSigmoid,
        Sigmoid,
        ELU,
        LeakyReLu,
        ReLu,
        LeCunTanh,
        AbsoluteReLU
    }
}