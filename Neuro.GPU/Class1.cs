using System;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace Neuro.GPU
{
    public class Class1
    {
        public const int N = 33 * 1024;
        public const int threadsPerBlock = 256;
        public const int blocksPerGrid = 32;
        
        public void Test()
        {
            CudafyModule km = CudafyTranslator.Cudafy();

            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            gpu.LoadModule(km);
            
            int iterations = 1000000;
            double Value;
            double[] dev_Value = gpu.Allocate<double>(iterations * sizeof(double));
            gpu.Launch(blocksPerGrid, threadsPerBlock).SumOfSines(iterations, dev_Value);
            gpu.CopyFromDevice(dev_Value, out Value);

            var a = Value;
        }
        
        [Cudafy]
        public static void SumOfSines(GThread thread, int _iterations, double[] Value)
        {
            int threadID = thread.threadIdx.x + thread.blockIdx.x * thread.blockDim.x;
            int numThreads = thread.blockDim.x * thread.gridDim.x;
            if (threadID < _iterations){
                for (int i = threadID; i < _iterations; i += numThreads)
                {
                    double _degAsRad = Math.PI / 180;
                    Value[i] = 0.0;
                    for (int a = 0; a < 100; a++)
                    {
                        double angle = (double)a * _degAsRad;
                        Value[i] += Math.Sin(angle);
                    }
                }
            }
        }
    }
}