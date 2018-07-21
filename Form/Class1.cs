/*
using ManagedCuda;
using ManagedCuda.BasicTypes;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static CudaKernel matrixSumCude;
        static CudaContext cntxt;

        static void InitKernels()
        {
            cntxt = new CudaContext();
            CUmodule cumodule = cntxt.LoadModulePTX(@"C:\work\Sobel\CudaTest\x64\Debug\kernel.ptx");
            matrixSumCude = new CudaKernel("_Z9addKernelPiPKiS1_", cumodule, cntxt);
        }

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        unsafe public extern static int* addWithCuda(int* c, int* a, int* b, int size);

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        public extern static void matrixSum(double[] b, double[] a, int x, int y, int z);

        static int[] TestPinvoke(int[] a, int[] b)
        {
            unsafe
            {
                int size = a.Length;
                var c = new int[size];

                fixed (int* aA = a, bB = b, cC = c)
                {
                    addWithCuda(cC, aA, bB, size);
                    Marshal.Copy((IntPtr)cC, c, 0, size);
                }

                return c;
            }
        }

        //static double[] MatrixSum(double[][,] matrix)
        //{
        //    unsafe
        //    {
        //        int z = matrix.Length;
        //        int y = matrix[0].GetLength(0);
        //        int x = matrix[0].GetLength(1);

        //        var result = new double[y * x];
        //        var input = ToLinearArray(matrix);

        //        fixed (double* aA = input, bB = result)
        //        {
        //            matrixSum(bB, aA, x, y, z);
        //            Marshal.Copy((IntPtr)bB, result, 0, y * x);
        //        }

        //        return result;
        //    }
        //}

        static double[] MatrixSum(double[][,] matrix)
        {
            int z = matrix.Length;
            int y = matrix[0].GetLength(0);
            int x = matrix[0].GetLength(1);

            var result = new double[y * x];
            var input = ToLinearArray(matrix);

            matrixSum(result, input, x, y, z);

            return result;
        }

        static void Main(string[] args)
        {
            const int size = 5;

            var a = new int[size] { 2, 2, 2, 2, 2 };
            var b = new int[size] { 2, 2, 2, 2, 2 };
            TestPinvoke(a, b);

            var c = new float[2][,]
            {
               new float[size, size]
               {
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 }
               },
               new float[size, size]
               {
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 },
                    { 2, 2, 2, 2, 2 }
               }
            };

            var aa = ToLinearArray(new float[1][,] { c[0] });
            var bb = ToLinearArray(new float[1][,] { c[1] });

            //Console.WriteLine(string.Join(",", MatrixSum(c)));
            TestPinvoke(a, b);

            var st = new Stopwatch();
            st.Start();

            SumMatrixCpu(c);

            Console.WriteLine($"CPU - {st.Elapsed}");
            st.Restart();

            //Console.WriteLine(string.Join(",", MatrixSum(c)));
            //MatrixSum(c);

            TestPinvoke(a, b);

            Console.WriteLine($"GPU - {st.Elapsed}");

            //var matrix = new double[1][,];
            //int N = 1000;
            //var size = ((((N * (double)N) * sizeof(double)) / 8) / 1024) / 1024;
            //matrix[0] = new double[N, N];

            //var st = new Stopwatch();

            //InitKernels();
            //SumMatrixCuda(matrix);

            //st.Start();

            //for (var i = 0; i < 100; i++)
            //    SumMatrixCpu(matrix);

            //st.Stop();
            //Console.WriteLine(st.Elapsed);
            //st.Restart();

            //for (var i = 0; i < 100; i++)
            //    SumMatrixCuda(matrix);

            //st.Stop();
            //Console.WriteLine(st.Elapsed);
        }

        static double[,] SumMatrixCpu(double[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new double[Y, X];
            var lm = ToLinearArray(matrix);

            for (var i = 0; i < Z; i++)
            {
                for (var y = 0; y < Y; y++)
                    for (var x = 0; x < X; x++)
                        result[y, x] += lm[(i * Y * X) + (y * X + x)];
            }

            return result;
        }

        static float[,] SumMatrixCpu(float[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new float[Y, X];
            var lm = ToLinearArray(matrix);

            for (var i = 0; i < Z; i++)
            {
                for (var y = 0; y < Y; y++)
                    for (var x = 0; x < X; x++)
                        result[y, x] += lm[(i * Y * X) + (y * X + x)];
            }

            return result;
        }

        static int[] SumMatrixManagedCuda(int[] a, int[] b)
        {
            int N = a.Length;
            var result = new int[N];

            //matrixSumCude.SetComputeSize((uint)(N + 127) / 512, 512, 1);
            matrixSumCude.BlockDimensions = 128;
            matrixSumCude.GridDimensions = (N + 127) / 128;

            CudaDeviceVariable<int> dA = a;
            CudaDeviceVariable<int> dB = b;
            CudaDeviceVariable<int> dC = new CudaDeviceVariable<int>(N);

            // Invoke kernel
            //kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);
            matrixSumCude.Run(dC.DevicePointer, dA.DevicePointer, dB.DevicePointer);

            result = dC;

            return result;
        }

        static double[,] SumMatrixCuda(double[][,] matrix)
        {
            //matrixSumCude.GridDimensions = new dim3(784, 1, 1);
            //matrixSumCude.BlockDimensions = new dim3(1, 1, 1);
            //kernel.SetComputeSize(49, 1, 1);

            int dimZ = matrix.Length;
            int dimX = matrix[0].GetLength(0);
            int dimY = matrix[0].GetLength(1);

            matrixSumCude.SetComputeSize((uint)(dimX * dimY), 1, 1);

            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<double> dA = ToLinearArray(matrix);
            CudaDeviceVariable<double> dC = new CudaDeviceVariable<double>(dimX * dimY);

            // Invoke kernel
            //kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);
            matrixSumCude.Run(dA.DevicePointer, dC.DevicePointer, dimZ);

            // Copy result from device memory to host memory
            double[] c = dC;

            var result = ToMultyArray(c, dimX);

            return result;
        }

        static double[,] ToMultyArray(double[] array, int stride)
        {
            var arrayLegth = array.Length;
            var result = new double[(array.Length / stride), stride];

            for (var i = 0; i < arrayLegth; i++)
            {
                result[i / stride, i % stride] = array[i];
            }

            return result;
        }

        static double[] ToLinearArray(double[,] input)
        {
            int dimX = input.GetLength(0);
            int dimY = input.GetLength(1);
            double[] result = new double[dimX * dimY];

            for (int y = 0; y < dimY; y++)
            {
                for (int x = 0; x < dimX; x++)
                {
                    result[y * dimX + x] = input[y, x];
                }
            }

            return result;
        }

        static float[] ToLinearArray(float[,] input)
        {
            int dimX = input.GetLength(0);
            int dimY = input.GetLength(1);
            float[] result = new float[dimX * dimY];

            for (int y = 0; y < dimY; y++)
            {
                for (int x = 0; x < dimX; x++)
                {
                    result[y * dimX + x] = input[y, x];
                }
            }

            return result;
        }

        static double[] ToLinearArray(double[][,] outputs)
        {
            var imageHeight = outputs[0].GetLength(0);
            var imageWidth = outputs[0].GetLength(1);
            var result = new double[outputs.Length * imageHeight * imageWidth];

            for (var i = 0; i < outputs.Length; i++)
            {
                for (var h = 0; h < imageHeight; h++)
                {
                    for (var w = 0; w < imageWidth; w++)
                    {
                        var position = (i * (imageWidth * imageHeight)) + (h * imageWidth + w);
                        result[position] = outputs[i][h, w];
                    }
                }
            }

            return result;
        }

        static float[] ToLinearArray(float[][,] outputs)
        {
            var imageHeight = outputs[0].GetLength(0);
            var imageWidth = outputs[0].GetLength(1);
            var result = new float[outputs.Length * imageHeight * imageWidth];

            for (var i = 0; i < outputs.Length; i++)
            {
                for (var h = 0; h < imageHeight; h++)
                {
                    for (var w = 0; w < imageWidth; w++)
                    {
                        var position = (i * (imageWidth * imageHeight)) + (h * imageWidth + w);
                        result[position] = outputs[i][h, w];
                    }
                }
            }

            return result;
        }

        static IntPtr Array3DToIntPtr(double[][,] Val)
        {
            IntPtr ret = Marshal.AllocHGlobal((Val.GetLength(0) + Val[0].GetLength(0) + Val[0].GetLength(1)) * sizeof(double));

            int offset = 0;
            for (int i = 0; i < Val.GetLength(0); i++)
            {

                for (int j = 0; j < Val[i].GetLength(0); j++)
                {
                    for (int k = 0; k < Val[i].GetLength(1); k++)
                    {
                        byte[] byteDouble = BitConverter.GetBytes(Val[i][j, k]);
                        Marshal.Copy(byteDouble, 0, ret, byteDouble.Length);

                        //Marshal.WriteInt64(ret, offset, Val[i][j, k]);
                        offset += sizeof(double);
                    }
                }
            }

            return ret;
        }

        static IntPtr Array2DToIntPtr(double[,] Val)
        {
            IntPtr ret = Marshal.AllocHGlobal((Val.GetLength(0) + Val.GetLength(1)) * sizeof(double));
            int offset = 0;

            for (int j = 0; j < Val.GetLength(0); j++)
            {
                for (int k = 0; k < Val.GetLength(1); k++)
                {
                    byte[] byteDouble = BitConverter.GetBytes(Val[j, k]);
                    Marshal.Copy(byteDouble, 0, ret, byteDouble.Length);

                    //Marshal.WriteInt64(ret, offset, Val[i][j, k]);
                    offset += sizeof(double);
                }
            }

            return ret;
        }
    }
}

public struct Test
{
    //public Test(double[,] cord)
    //{
    //    Cord = cord;
    //}

    public double[][] Cord { get; set; }
}
*/
