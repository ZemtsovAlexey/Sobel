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
            matrixSumCude = new CudaKernel("_Z15matrixSumKernelPdPKdiii", cumodule, cntxt);
        }

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        unsafe public extern static int* addWithCuda(int* c, int* a, int* b, int size);

        //[DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        //unsafe public extern static void matrixSum(double* b, double* a, int x, int y, int z);

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        public extern static void matrixSum(double[] b, double[] a, int x, int y, int z);

        //[DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        //unsafe public extern static void transpose(double* b, double* a, int n);

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        public extern static void Rot90(double[] output, double[] iniput, int width, int length);

        [DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        public extern static void ConvolutionGPU(double[] output, double[] iniput, double[] kernel, int inputWidth, int inputHeight, int kernelWidth, int kernelHeight, int outWidth, int outHeight);

        //[DllImport("C:\\work\\Sobel\\x64\\Debug\\CudaTest.dll")]
        //public extern static void Rot180(double[] output, double[] iniput, int width, int length);

        double[] MatrixSum(double[][,] matrix)
        {
            int z = matrix.Length;
            int y = matrix[0].GetLength(0);
            int x = matrix[0].GetLength(1);

            var result = new double[y * x];
            var input = ToLinearArray(matrix);

            matrixSum(result, input, x, y, z);

            return result;
        }

        static double[] Rot90Wrapper(double[,] matrix)
        {
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);

            var result = new double[y * x];
            var input = ToLinearArray(matrix);

            Rot90(result, input, x, matrix.Length);

            return result;
        }

        static double[,] Convolution(double[,] input, double[,] kernel, int step = 1)
        {
            int inputWidth = input.GetLength(1);
            int inputHeight = input.GetLength(0);
            int kernelWidth = kernel.GetLength(1);
            int kernelHeight = kernel.GetLength(0);
            var outHeight = inputHeight - kernelHeight + step;
            var outWidth = inputWidth - kernelWidth + step;

            var output = new double[outHeight * outWidth];
            var input2 = ToLinearArray(input);
            var kernel2 = ToLinearArray(kernel);

            ConvolutionGPU(output, input2, kernel2, inputWidth, inputHeight, kernelWidth, kernelHeight, outWidth, outHeight);

            return ToMultyArray(output, outWidth);
        }

        //static double[] Rot180Wrapper(double[,] matrix)
        //{
        //    int y = matrix.GetLength(0);
        //    int x = matrix.GetLength(1);

        //    var result = new double[y * x];
        //    var input = ToLinearArray(matrix);

        //    Rot180(result, input, x, matrix.Length);

        //    return result;
        //}

        //unsafe static double[] Transpose(double[,] matrix)
        //{
        //    int y = matrix.GetLength(0);
        //    int x = matrix.GetLength(1);

        //    var result = new double[y * x];
        //    var input = ToLinearArray(matrix);

        //    fixed (double* host_a = input, host_b = result)
        //    {
        //        transpose(host_b, host_a, x);
        //        Marshal.Copy((IntPtr)host_b, result, 0, y * x);
        //    }

        //    return result;
        //}

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

        //static double[] MatrixSum(double[][,] matrix)
        //{
        //    int z = matrix.Length;
        //    int y = matrix[0].GetLength(0);
        //    int x = matrix[0].GetLength(1);

        //    var result = new double[y * x];
        //    var input = ToLinearArray(matrix);

        //    unsafe
        //    {
        //        fixed (double* Result = result, Input = input)
        //        {
        //            matrixSum(Result, Input, x, y, z);
        //            Marshal.Copy((IntPtr)Result, result, 0, (y * x));
        //        }
        //    }

        //    return result;
        //}

        static void Main(string[] args)
        {
            //InitKernels();
            Run(args);
        }

        static void Run(string[] args)
        {
            const int size = 3;
            const int deep = 1;

            //var kernel = new double[,] {
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d },
            //    { 2d, 2d, 2d, 2d, 2d, 2d, 2d }
            //};

            var kernel = new double[,] {
                { 2d, 2d },
                { 2d, 2d },
            };

            var c = GetMatrixes(deep, size);
            //var kernel = GetMatrixes(1, 2)[0];

            Console.WriteLine();
            //var aa = ToLinearArray(new float[1][,] { c[0] });
            //var bb = ToLinearArray(new float[1][,] { c[1] });
            //SumMatrixManagedCuda(c);
            //Console.WriteLine(string.Join(",", MatrixSum(c)));
            //TestPinvoke(a, b);

            var st = new Stopwatch();
            st.Start();

            //Parallel.For(0, 1000, i =>
            //{
            //    ConvolutionCPU(c[0], kernel);
            //});
            //SumMatrixCpu(c);
            //Console.WriteLine(string.Join(",", SumMatrixCpu(c)));

            //var res1 = Rot90(c[0]);
            //res1 = Rot90(res1);

            //Parallel.For(0, 1000, i => { res1 = Rot90(c[0]); });
            var res1 = ConvolutionCPU(c[0], kernel);

            Console.WriteLine($"CPU - {st.Elapsed}");
            //st.Restart();

            ////SumMatrixManagedCuda(c);

            //Console.WriteLine($"CUD - {st.Elapsed}");


            st.Restart();
            //var res = Rot90Wrapper(c[0]);
            var res = Convolution(c[0], kernel);

            //Parallel.For(0, 1000, i => { Convolution(c[0], kernel); });

            //for (var i= 0; i < 1000; i++)
            //    res = Transpose2(c[0]);

            //MatrixSum(c);

            Console.WriteLine($"GPU - {st.Elapsed}");

            Show2dArray(res1);
            //Show2dArray(c[0]);
            Console.WriteLine();
            Show2dArray(res);
            //Console.WriteLine(string.Join(",", res));

            var key = Console.ReadKey();

            Run(args);
        }

        static void Show2dArray(double[] arr, int stride)
        {
            var height = arr.Length / stride;
            var width = stride;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Console.Write($" {arr[y * stride + x]}");
                }

                Console.WriteLine();
            }
        }

        static void Show2dArray(double[,] arr)
        {
            var height = arr.GetLength(0);
            var width = arr.GetLength(1);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Console.Write($" {arr[y, x]}");
                }

                Console.WriteLine();
            }
        }

        static double[][,] GetMatrixes(int deep, int width)
        {
            var c = new double[deep][,];
            var value = 0;

            for (var i = 0; i < deep; i++)
            {
                c[i] = new double[width, width];

                for (var y = 0; y < c[i].GetLength(0); y++)
                    for (var x = 0; x < c[i].GetLength(1); x++)
                        c[i][y, x] = i + (++value);

                value = 0;
            }

            return c;
        }

        public static unsafe double[,] Rot90(double[,] input)
        {
            if (input == null || input.Length == 0)
                return null;

            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var result = new double[width, height];
            var resultHeight = input.GetLength(0);
            var resultWidth = input.GetLength(1);

            fixed (double* v = input, r = result)
                for (var y = 0; y < resultHeight; y++)
                    for (var x = 0; x < resultWidth; x++)
                        r[y * resultWidth + x] = v[(height - 1 - x) * width + (y)];

            return result;
        }

        float[][,] GetMatrixesFloat(int deep, int width)
        {
            var c = new float[deep][,];

            for (var i = 0; i < deep; i++)
            {
                c[i] = new float[width, width];

                for (var y = 0; y < c[i].GetLength(0); y++)
                    for (var x = 0; x < c[i].GetLength(1); x++)
                        c[i][y, x] = i + 1;
            }

            return c;
        }

        double[] SumMatrixCpu(double[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new double[Y * X];
            var lm = ToLinearArray(matrix);

            for (var i = 0; i < Z; i++)
            {
                for (var y = 0; y < Y; y++)
                    for (var x = 0; x < X; x++)
                        result[y * X + x] += lm[(i * Y * X) + (y * X + x)];
            }

            return result;
        }

        float[,] SumMatrixCpu(float[][,] matrix)
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

        double[] SumMatrixManagedCuda(double[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new double[Y * X];
            var lm = ToLinearArray(matrix);
            int N = lm.Length;

            matrixSumCude.SetComputeSize((uint)X, (uint)Y);
            //matrixSumCude.BlockDimensions = 128;
            //matrixSumCude.GridDimensions = (N + 127) / 128;

            var da = cntxt.AllocateMemory(N * sizeof(double));
            var db = cntxt.AllocateMemory(result.Length * sizeof(double));

            cntxt.CopyToDevice(da, lm);
            cntxt.CopyToDevice(db, result);

            //CudaDeviceVariable<int> dA = a;
            //CudaDeviceVariable<int> dB = b;
            //CudaDeviceVariable<int> dC = new CudaDeviceVariable<int>(N);

            // Invoke kernel
            //kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);
            matrixSumCude.Run(db, da, X, Y, Z);

            cntxt.CopyToHost<double>(result, db);

            return result;
        }

        double[,] SumMatrixCuda(double[][,] matrix)
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

        float[] ToLinearArray(float[,] input)
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

        double[] ToLinearArray(double[][,] outputs)
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

        float[] ToLinearArray(float[][,] outputs)
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

        IntPtr Array3DToIntPtr(double[][,] Val)
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

        IntPtr Array2DToIntPtr(double[,] Val)
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

        public static unsafe double[,] ConvolutionCPU(double[,] matrix, double[,] kernel, int step = 1)
        {
            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var kernelHeight = kernel.GetLength(0);
            var kernelWidth = kernel.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;

            var output = new double[outputHeight, outputWidth];

            fixed (double* m = matrix, k = kernel, o = output)
                for (var y = 0; y < outputHeight; y++)
                    for (var x = 0; x < outputWidth; x++)
                        for (var h = 0; h < kernelHeight; h++)
                            for (var w = 0; w < kernelWidth; w++)
                                o[y * outputWidth + x] += (m[((y + h) * matrixWidth + x + w)] * k[h * kernelWidth + w]);

            return output;
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
