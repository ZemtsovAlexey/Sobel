using Neuro.Models;
using Neuro.Neurons;

namespace ScannerNet.Models
{
    public class NetworkSettings
    {
        public NetworkSettings()
        {
            
        }

        public NetworkSettings(LayerType type, ActivationType? activation, int? neuronsCount, int? kernelSize)
        {
            Type = type;
            Activation = activation;
            NeuronsCount = neuronsCount;
            KernelSize = kernelSize;

            if (type == LayerType.MaxPoolingLayer)
            {
                ActivationDisable = true;
            }
        }
        
        public LayerType Type { get; set; }
        
        public ActivationType? Activation { get; set; }
        
        public int? NeuronsCount { get; set; }
        
        public int? KernelSize { get; set; }
        
        public bool ActivationDisable { get; set; }
    }
}