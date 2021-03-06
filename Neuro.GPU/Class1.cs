﻿using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;
using Neuro.Extensions;

namespace Neuro.GPU
{
    public static class Class1
    {
        //private CudaKernel kernel1;

        //public Class1()
        //{
        //    //int deviceID = 0;

        //    //CudaContext ctx = new CudaContext(deviceID);
        //    //CUmodule cumodule = ctx.LoadModulePTX(@"C:\work\Sobel\TestCuda\x64\Debug\kernel.ptx");
        //    //kernel1 = new CudaKernel("_Z9matrixSumPdS_iii", cumodule, ctx);
        //}

        public static double[,] Test(double[][,] a)
        {
            using (CudaContext ctx = new CudaContext(0))
            {
                CUmodule cumodule = ctx.LoadModule(@"C:\work\Sobel\TestCuda\x64\Debug\kernel.ptx");
                var kernel = new CudaKernel("_Z9matrixSumPdS_iii", cumodule, ctx);

                int dimZ = a.Length;
                int dimX = a[0].GetLength(0);
                int dimY = a[0].GetLength(1);

                kernel.GridDimensions = new dim3(28, 28, 1);
                kernel.BlockDimensions = new dim3(1, 1, 1);
                //kernel.BlockDimensions = new dim3(dimX, dimY, 1);

                // Allocate vectors in device memory and copy vectors from host memory to device memory 
                CudaDeviceVariable<double> dA = a.ToLinearArray();
                //CudaDeviceVariable<double> dB = ToLinearArray(b);
                CudaDeviceVariable<double> dC = new CudaDeviceVariable<double>(dimX * dimY);

                // Invoke kernel
                kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);

                // Copy result from device memory to host memory
                double[] c = dC;

                //ctx.FreeMemory(dC.DevicePointer);
                //ctx.FreeMemory(dA.DevicePointer);
                //ctx.Dispose();

                return ToMultyArray(c, dimX);
            }
        }

        private static double[,] ToMultyArray(double[] array, int stride)
        {
            var arrayLegth = array.Length;
            var result = new double[(array.Length / stride), stride];

            for (var i = 0; i < arrayLegth; i++)
            {
                result[i / stride, i % stride] = array[i];
            }

            return result;
        }
    }

    public struct Test
    {
        public double[,] Cord { get; set; }
    }

}
