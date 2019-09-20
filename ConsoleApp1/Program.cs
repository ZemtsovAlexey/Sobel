using ManagedCuda;
using ManagedCuda.BasicTypes;
using System;
using System.Diagnostics;
using System.Linq;
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
        //unsafe public extern static void matrixSum(float* b, float* a, int x, int y, int z);

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
        public extern static void ConvolutionGPU(
            float[] output,
            float[] input,
            float[] kernel,
            int inputWidth,
            int inputHeight,
            int kernelWidth,
            int kernelHeight,
            int outWidth,
            int outHeight);

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
        public extern static void ConvolutionGPU2(
            float[] output,
            float[] input,
            float[] kernel,
            int inputWidth,
            int inputHeight,
            int kernelWidth,
            int kernelHeight,
            int outWidth,
            int outHeight);

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
        public extern static void ConvolutionGPU3(
            float[] output,
            float[] input,
            float[] kernel,
            int inputWidth,
            int inputHeight,
            int kernelWidth,
            int kernelHeight,
            int outWidth,
            int outHeight);

        [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
        public extern static void Multiply2GPU(float[] output, float[] input, float[] weights, int len, int wlen, int nlen);

        /* static float[] MatrixSum(float[][,] matrix)
         {
             int z = matrix.Length;
             int y = matrix[0].GetLength(0);
             int x = matrix[0].GetLength(1);

             var result = new float[y * x];
             var input = ToLinearArray(matrix);

             matrixSum(result, input, x, y, z);

             return result;
         }*/

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

        //static float[] MatrixSum(float[][,] matrix)
        //{
        //    unsafe
        //    {
        //        int z = matrix.Length;
        //        int y = matrix[0].GetLength(0);
        //        int x = matrix[0].GetLength(1);

        //        var result = new float[y * x];
        //        var input = ToLinearArray(matrix);

        //        fixed (float* aA = input, bB = result)
        //        {
        //            matrixSum(bB, aA, x, y, z);
        //            Marshal.Copy((IntPtr)bB, result, 0, y * x);
        //        }

        //        return result;
        //    }
        //}

        //static float[] MatrixSum(float[][,] matrix)
        //{
        //    int z = matrix.Length;
        //    int y = matrix[0].GetLength(0);
        //    int x = matrix[0].GetLength(1);

        //    var result = new float[y * x];
        //    var input = ToLinearArray(matrix);

        //    unsafe
        //    {
        //        fixed (float* Result = result, Input = input)
        //        {
        //            matrixSum(Result, Input, x, y, z);
        //            Marshal.Copy((IntPtr)Result, result, 0, (y * x));
        //        }
        //    }

        //    return result;
        //}

        static void Main(string[] args)
        {
            TestConvolution(args);

            var key = Console.ReadKey();

            Main(args);
        }

        static void TestRotate(string[] args)
        {
            const int size = 2;
            const int deep = 2;

            var input = new Matrix(GetMatrixes(1, 200)[0]);
            var kernel = GetMatrixes(1, 3)[0];

            Console.WriteLine();

            var st = new Stopwatch();
            st.Start();

            var output = input.Rot180GPU();//.To1DArray();
            //var output = Convolution(input.Value, kernel);
            var gpuTime = st.Elapsed;

            st.Restart();

            var outputCPU = input.Rot180().To1DArray();
            var cpuTime = st.Elapsed;

            st.Stop();

            Console.WriteLine(string.Join(",", output.Skip(output.Length - 11).Take(10)));
            Console.WriteLine($"GPU - {gpuTime}");

            Console.WriteLine(string.Join(",", outputCPU.Skip(output.Length - 11).Take(10)));
            Console.WriteLine($"CPU - {cpuTime}");
        }

        static void TestConvolution(string[] args)
        {
            //InitKernels();
            const int size = 2;
            const int deep = 2;

            var input = new Matrix(GetMatrixes(1, 200)[0]);
            var kernel = new Matrix(GetMatrixes(1, 3)[0]);

            var inputs = ToLinearArray(GetMatrixes(10, 1000));
            var weights = ToLinearArray(GetMatrixes(10, 1000));
            /*var kernel2 = GetMatrixes(1, 3, 0.003f)[0];
            var kernels = GetMatrixes(8, 3, 0.003f);*/

            Console.WriteLine();

            var st = new Stopwatch();
            st.Start();

            //float[] output = null;
            /*Parallel.For(0, kernels.Length, (int i) => {
                output = Convolution(input, kernels[i]); 
            });*/

            /*for (var i = 0; i < kernels.Length; i++)
            {
                output = Convolution(input, kernels[i]);
            }*/

            var output = input.BackConvolutionGPU(kernel);

            /*float[] output = new float[10];
            Multiply2GPU(output, inputs, weights, inputs.Length, weights.Length, output.Length);*/
            var gpuTime = st.Elapsed;

            st.Restart();

            /*var output2 = Convolution2(input, kernel);
            var gpuTime2 = st.Elapsed;

            st.Restart();*/

            //float[] outputCPU = null;
            /*Parallel.For(0, kernels.Length, (int i) => {
                outputCPU = ConvolutionCPU(input, kernels[i]);
            });*/

            /*for (var i = 0; i < kernels.Length; i++)
            {
                outputCPU = ConvolutionCPU(input, kernels[i]);
            }*/

            var outputCPU = input.BackConvolution(kernel).To1DArray();
            /*float[] outputCPU = new float[10];
            Parallel.For(0, outputCPU.Length, (int i) => {
                outputCPU[i] = Compute(inputs, weights);
            });*/
            var cpuTime = st.Elapsed;

            st.Stop();

            Console.WriteLine(string.Join(",", output.Skip(output.Length - 11).Take(10)));
            Console.WriteLine($"GPU - {gpuTime}");

            /* Console.WriteLine(string.Join(",", output2.Skip(output.Length - 11).Take(10)));
             Console.WriteLine($"GPU2 - {gpuTime2}");*/

            Console.WriteLine(string.Join(",", outputCPU.Skip(output.Length - 11).Take(10)));
            Console.WriteLine($"CPU - {cpuTime}");
        }

        public static float Compute(float[] input, float[] Weights)
        {
            float e = 0;
            unsafe
            {
                fixed (float* w = Weights, i = input)
                    for (var n = 0; n < input.Length; n++)
                        e += w[n] * i[n];
            }

            return e;
        }

        static float[][,] GetMatrixes(int deep, int width, float s = 0f)
        {
            var c = new float[deep][,];

            for (var i = 0; i < deep; i++)
            {
                c[i] = new float[width, width];

                for (var y = 0; y < c[i].GetLength(0); y++)
                    for (var x = 0; x < c[i].GetLength(1); x++)
                        c[i][y, x] = y + x + i + s;
            }

            return c;
        }

        static float[][,] GetMatrixesFloat(int deep, int width)
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

        static float[] SumMatrixCpu(float[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new float[Y * X];
            var lm = ToLinearArray(matrix);

            for (var i = 0; i < Z; i++)
            {
                for (var y = 0; y < Y; y++)
                    for (var x = 0; x < X; x++)
                        result[y * X + x] += lm[(i * Y * X) + (y * X + x)];
            }

            return result;
        }

        /*static float[,] SumMatrixCpu(float[][,] matrix)
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
        }*/

        static float[] SumMatrixManagedCuda(float[][,] matrix)
        {
            int Z = matrix.Length;
            int Y = matrix[0].GetLength(0);
            int X = matrix[0].GetLength(1);

            var result = new float[Y * X];
            var lm = ToLinearArray(matrix);
            int N = lm.Length;

            matrixSumCude.SetComputeSize((uint)X, (uint)Y);
            //matrixSumCude.BlockDimensions = 128;
            //matrixSumCude.GridDimensions = (N + 127) / 128;

            var da = cntxt.AllocateMemory(N * sizeof(float));
            var db = cntxt.AllocateMemory(result.Length * sizeof(float));

            cntxt.CopyToDevice(da, lm);
            cntxt.CopyToDevice(db, result);

            //CudaDeviceVariable<int> dA = a;
            //CudaDeviceVariable<int> dB = b;
            //CudaDeviceVariable<int> dC = new CudaDeviceVariable<int>(N);

            // Invoke kernel
            //kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);
            matrixSumCude.Run(db, da, X, Y, Z);

            cntxt.CopyToHost<float>(result, db);

            return result;
        }

        static float[,] SumMatrixCuda(float[][,] matrix)
        {
            //matrixSumCude.GridDimensions = new dim3(784, 1, 1);
            //matrixSumCude.BlockDimensions = new dim3(1, 1, 1);
            //kernel.SetComputeSize(49, 1, 1);

            int dimZ = matrix.Length;
            int dimX = matrix[0].GetLength(0);
            int dimY = matrix[0].GetLength(1);

            matrixSumCude.SetComputeSize((uint)(dimX * dimY), 1, 1);

            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<float> dA = ToLinearArray(matrix);
            CudaDeviceVariable<float> dC = new CudaDeviceVariable<float>(dimX * dimY);

            // Invoke kernel
            //kernel.Run(dA.DevicePointer, dC.DevicePointer, dimX, dimY, dimZ);
            matrixSumCude.Run(dA.DevicePointer, dC.DevicePointer, dimZ);

            // Copy result from device memory to host memory
            float[] c = dC;

            var result = ToMultyArray(c, dimX);

            return result;
        }

        static float[,] ToMultyArray(float[] array, int stride)
        {
            var arrayLegth = array.Length;
            var result = new float[(array.Length / stride), stride];

            for (var i = 0; i < arrayLegth; i++)
            {
                result[i / stride, i % stride] = array[i];
            }

            return result;
        }

        static float[] To1DArray(float[,] input)
        {
            int dimX = input.GetLength(0);
            int dimY = input.GetLength(1);
            var result = new float[dimX * dimY];

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

        /*static float[] ToLinearArray(float[][,] outputs)
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
        }*/

        static IntPtr Array3DToIntPtr(float[][,] Val)
        {
            IntPtr ret = Marshal.AllocHGlobal((Val.GetLength(0) + Val[0].GetLength(0) + Val[0].GetLength(1)) * sizeof(float));

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
                        offset += sizeof(float);
                    }
                }
            }

            return ret;
        }

        static IntPtr Array2DToIntPtr(float[,] Val)
        {
            IntPtr ret = Marshal.AllocHGlobal((Val.GetLength(0) + Val.GetLength(1)) * sizeof(float));
            int offset = 0;

            for (int j = 0; j < Val.GetLength(0); j++)
            {
                for (int k = 0; k < Val.GetLength(1); k++)
                {
                    byte[] byteDouble = BitConverter.GetBytes(Val[j, k]);
                    Marshal.Copy(byteDouble, 0, ret, byteDouble.Length);

                    //Marshal.WriteInt64(ret, offset, Val[i][j, k]);
                    offset += sizeof(float);
                }
            }

            return ret;
        }

        static float[] Convolution(float[,] matrix, float[,] kernel, int step = 1)
        {
            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var kernelHeight = kernel.GetLength(0);
            var kernelWidth = kernel.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;
            var output = new float[outputHeight * outputWidth];

            ConvolutionGPU2(output, To1DArray(matrix), To1DArray(kernel), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

            return output;
        }

        static float[] Convolution2(float[,] matrix, float[,] kernel, int step = 1)
        {
            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var kernelHeight = kernel.GetLength(0);
            var kernelWidth = kernel.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;
            var output = new float[outputHeight * outputWidth];

            ConvolutionGPU2(output, To1DArray(matrix), To1DArray(kernel), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

            return output;
        }

        static unsafe float[] ConvolutionCPU(float[,] matrix, float[,] kernel, int step = 1)
        {
            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var kernelHeight = kernel.GetLength(0);
            var kernelWidth = kernel.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;

            var output = new float[outputHeight, outputWidth];

            fixed (float* m = matrix, k = kernel, o = output)
                for (var y = 0; y < outputHeight; y++)
                    for (var x = 0; x < outputWidth; x++)
                        for (var h = 0; h < kernelHeight; h++)
                            for (var w = 0; w < kernelWidth; w++)
                                o[y * outputWidth + x] += (m[((y + h) * matrixWidth + x + w)] * k[h * kernelWidth + w]);

            return To1DArray(output);
        }
    }
}

public struct Test
{
    //public Test(float[,] cord)
    //{
    //    Cord = cord;
    //}

    public float[][] Cord { get; set; }
}

public class Matrix
{
    public float[,] Value { get; }

    public Matrix(float[,] value)
    {
        Value = value;
    }

    public unsafe Matrix(float[] value, int stride)
    {
        var imageHeight = value.Length / stride;
        var imageWidth = stride;
        var result = new float[imageHeight, imageWidth];

        fixed (float* r = result)
        fixed (float* v = value)
        {
            for (var h = 0; h < imageHeight; h++)
            {
                for (var w = 0; w < imageWidth; w++)
                {
                    r[h * imageWidth + w] = v[h * imageWidth + w];
                }
            }
        }

        Value = result;
    }

    /* public unsafe Matrix(float[] value, int stride)
     {
         var imageHeight = value.Length / stride;
         var imageWidth = stride;
         var result = new float[imageHeight, imageWidth];

         fixed (float* r = result, v = value)
         {
             for (var h = 0; h < imageHeight; h++)
             {
                 for (var w = 0; w < imageWidth; w++)
                 {
                     r[h * imageWidth + w] = v[h * imageWidth + w];
                 }
             }
         }

         Value = result;
     }*/

    public float this[int y, int x]
    {
        get => Value[y, x];
        set => Value[y, x] = value;
    }

    public int Length => Value.Length;

    public int GetLength(int dimension) => Value.GetLength(dimension);

    public static Matrix operator +(Matrix a, Matrix b)
    {
#if DEBUG

        if (a.Value == null || a.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

        if (b.Value == null || b.Value?.Length == 0)
            throw new ArgumentNullException(nameof(b));

        if (a.Value.Length != b.Value.Length)
            throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

#endif

        var height = a.Value.GetLength(0);
        var width = a.Value.GetLength(1);
        var result = new float[height, width];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < height; x++)
                result[y, x] = a.Value[y, x] + b.Value[y, x];

        return new Matrix(result);
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
#if DEBUG

        if (a.Value == null || a.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

        if (b.Value == null || b.Value?.Length == 0)
            throw new ArgumentNullException(nameof(b));

#endif

        if (a.Value.Length != b.Value.Length)
            throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

        var height = a.Value.GetLength(0);
        var width = a.Value.GetLength(1);
        var result = new float[height, width];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < height; x++)
                result[y, x] = a.Value[y, x] - b.Value[y, x];

        return new Matrix(result);
    }

    public static Matrix operator -(Matrix a, float value)
    {
#if DEBUG

        if (a.Value == null || a.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

#endif

        var height = a.Value.GetLength(0);
        var width = a.Value.GetLength(1);
        var result = new float[height, width];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < height; x++)
                result[y, x] = a.Value[y, x] - value;

        return new Matrix(result);
    }

    public static unsafe Matrix operator +(Matrix a, float value)
    {
#if DEBUG

        if (a.Value == null || a.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

#endif

        var height = a.Value.GetLength(0);
        var width = a.Value.GetLength(1);
        var result = new float[height, width];

        fixed (float* v = a.Value, r = result)
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    r[y * width + x] = v[y * width + x] + value;

        return new Matrix(result);
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
#if DEBUG

        if (a.Value == null || a.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

        if (b.Value == null || b.Value?.Length == 0)
            throw new ArgumentNullException(nameof(a));

        if (a.Value.Length != b.Value.Length)
            throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

#endif

        var height = a.Value.GetLength(0);
        var width = a.Value.GetLength(1);
        var result = new float[height, width];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < height; x++)
                result[y, x] = a.Value[y, x] * b.Value[y, x];

        return new Matrix(result);
    }

    public static unsafe Matrix operator *(Matrix matrix, Func<float, float> func)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

        if (func == null)
            throw new ArgumentNullException(nameof(func));

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var output = new float[matrixHeight, matrixWidth];

        fixed (float* v = output, m = matrix.Value)
            for (var y = 0; y < matrixHeight; y++)
                for (var x = 0; x < matrixWidth; x++)
                    v[y * matrixWidth + x] = func.Invoke(m[y * matrixWidth + x]);

        return new Matrix(output);
    }

    public static Matrix operator *(Matrix matrix, float value)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var output = new float[matrixHeight, matrixWidth];

        for (var y = 0; y < matrixHeight; y++)
            for (var x = 0; x < matrixWidth; x++)
                output[y, x] = matrix[y, x] * value;

        return new Matrix(output);
    }

    public static Matrix operator /(Matrix matrix, float value)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var output = new float[matrixHeight, matrixWidth];

        for (var y = 0; y < matrixHeight; y++)
            for (var x = 0; x < matrixWidth; x++)
                output[y, x] = matrix[y, x] / value;

        return new Matrix(output);
    }
}

public static class MatrixExtensions
{
    [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
    public extern static void Rot180GPU(float[] output, float[] input, int width, int length);

    [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
    public extern static void ConvolutionGPU(
        float[] output,
        float[] input,
        float[] kernel,
        int inputWidth,
        int inputHeight,
        int kernelWidth,
        int kernelHeight,
        int outWidth,
        int outHeight);

    [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
    public extern static void ConvolutionGPU3(
        float[] output,
        float[] input,
        float[] kernel,
        int inputWidth,
        int inputHeight,
        int kernelWidth,
        int kernelHeight,
        int outWidth,
        int outHeight);

    [DllImport("C:\\git_my\\Sobel\\x64\\Debug\\Neuro.Extensions.dll")]
    public extern static void BackConvolutionGPU2(
        float[] output,
        float[] input,
        float[] kernel,
        int inputWidth,
        int inputHeight,
        int kernelWidth,
        int kernelHeight,
        int outWidth,
        int outHeight);

    public static unsafe Matrix Rot180(this Matrix input)
    {
        if (input.Value == null || input.Value.Length == 0)
            return input;

        var height = input.Value.GetLength(0);
        var width = input.Value.GetLength(1);

        /*if (height > 199 && width > 199)
            return Rot180f(input);*/

        var result = new float[height, width];

        fixed (float* v = input.Value, r = result)
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    r[y * width + x] = v[(height - 1 - y) * width + (width - 1 - x)];

        return new Matrix(result);
    }

    public static unsafe float[] Rot180GPU(this Matrix input)
    {
        if (input.Value == null || input.Value.Length == 0)
            return null;// input;

        var height = input.Value.GetLength(0);
        var width = input.Value.GetLength(1);
        var length = height * width;
        var output = new float[length];
        var i = input.To1DArray();

        Rot180GPU(output, i, width, length);
        return output;
        /*var result = new float[height, width];

        fixed (float* v = output, r = result)
            for (var x = 0; x < length; x++)
                r[x] = v[x];

        return new Matrix(result);*/
    }

    public static unsafe Matrix Rot90(this Matrix input)
    {
        if (input.Value == null || input.Value.Length == 0)
            return input;

        var height = input.Value.GetLength(0);
        var width = input.Value.GetLength(1);
        var result = new float[width, height];
        var resultHeight = input.Value.GetLength(0);
        var resultWidth = input.Value.GetLength(1);

        fixed (float* v = input.Value, r = result)
            for (var y = 0; y < resultHeight; y++)
                for (var x = 0; x < resultWidth; x++)
                    r[y * resultWidth + x] = v[(height - 1 - x) * width + (y)];

        return new Matrix(result);
    }

    public static unsafe Matrix Sum(this Matrix[] matrixes)
    {
#if DEBUG

        if (matrixes == null || matrixes.Length < 1)
            throw new ArgumentNullException(nameof(matrixes));

#endif

        if (matrixes.Length == 1)
            return matrixes[0];

        var height = matrixes[0].Value.GetLength(0);
        var width = matrixes[0].Value.GetLength(1);
        var sum = new float[height, width];

        foreach (var matrix in matrixes)
            fixed (float* v = matrix.Value, r = sum)
                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                        r[y * width + x] += v[y * width + x];

        return new Matrix(sum);
    }

    public static unsafe float Sum(this Matrix matrix)
    {
#if DEBUG

        if (matrix == null || matrix.Length < 1)
            throw new ArgumentNullException(nameof(matrix));

#endif

        var height = matrix.Value.GetLength(0);
        var width = matrix.Value.GetLength(1);
        var sum = 0f;

        fixed (float* v = matrix.Value)
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    sum += v[y * width + x];

        return sum;
    }

    public static unsafe Matrix Convolution(this Matrix matrix, Matrix kernel, int step = 1)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

        if (kernel.Value == null || kernel.Value.Length == 0)
            throw new ArgumentNullException(nameof(kernel));

        if (matrix.Value.Length < kernel.Value.Length)
            throw new Exception("Kernel size more then size of matrix");

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var kernelHeight = kernel.Value.GetLength(0);
        var kernelWidth = kernel.Value.GetLength(1);
        var outputHeight = matrixHeight - kernelHeight + step;
        var outputWidth = matrixWidth - kernelWidth + step;

        var output = new float[outputHeight, outputWidth];

        fixed (float* m = matrix.Value, k = kernel.Value, o = output)
            for (var y = 0; y < outputHeight; y++)
                for (var x = 0; x < outputWidth; x++)
                    for (var h = 0; h < kernelHeight; h++)
                        for (var w = 0; w < kernelWidth; w++)
                            o[y * outputWidth + x] += (m[((y + h) * matrixWidth + x + w)] * k[h * kernelWidth + w]);

        return new Matrix(output);
    }

    public static unsafe Matrix Convolution2(this Matrix matrix, Matrix kernel, int step = 1)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

        if (kernel.Value == null || kernel.Value.Length == 0)
            throw new ArgumentNullException(nameof(kernel));

        if (matrix.Value.Length < kernel.Value.Length)
            throw new Exception("Kernel size more then size of matrix");

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var kernelHeight = kernel.Value.GetLength(0);
        var kernelWidth = kernel.Value.GetLength(1);
        var outputHeight = matrixHeight - kernelHeight + step;
        var outputWidth = matrixWidth - kernelWidth + step;
        var output = new float[outputHeight * outputWidth];

        ConvolutionGPU3(output, matrix.To1DFloatArray(), kernel.To1DFloatArray(), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

        return new Matrix(output, outputWidth);
    }

    public static unsafe Matrix BackConvolution(this Matrix matrix, Matrix kernel, int step = 1)
    {
#if DEBUG

        if (matrix.Value == null || matrix.Value.Length == 0)
            throw new ArgumentNullException(nameof(matrix));

        if (kernel.Value == null || kernel.Value.Length == 0)
            throw new ArgumentNullException(nameof(kernel));

        if (matrix.Value.Length < kernel.Value.Length)
            throw new Exception("Kernel size more then size of matrix");

#endif

        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var kernelHeight = kernel.Value.GetLength(0);
        var kernelWidth = kernel.Value.GetLength(1);
        var outputHeight = matrixHeight + kernelHeight - step;
        var outputWidth = matrixWidth + kernelWidth - step;

        var output = new float[outputHeight, outputWidth];

        var padY = kernelHeight - step;
        var padX = kernelWidth - step;

        fixed (float* m = matrix.Value, k = kernel.Value, o = output)
            for (var y = -(padY); y < matrixHeight + padY - step; y++)
                for (var x = -(padX); x < matrixWidth + padX - step; x++)
                    for (var h = y < 0 ? 0 - y : 0; h < (y + kernelHeight > matrixHeight ? (matrixHeight - (y + kernelHeight)) + kernelHeight : kernelHeight); h++)
                        for (var w = x < 0 ? 0 - x : 0; w < (x + kernelWidth > matrixWidth ? (matrixWidth - (x + kernelWidth)) + kernelWidth : kernelWidth); w++)
                            o[(y + padY) * outputWidth + (x + padX)] += m[((y + h) * matrixWidth + x + w)] * k[h * kernelWidth + w];

        return new Matrix(output);
    }

    public static float[] BackConvolutionGPU(this Matrix matrix, Matrix kernel, int step = 1)
    {
        var matrixHeight = matrix.Value.GetLength(0);
        var matrixWidth = matrix.Value.GetLength(1);
        var kernelHeight = kernel.Value.GetLength(0);
        var kernelWidth = kernel.Value.GetLength(1);
        var outputHeight = matrixHeight + kernelHeight - step;
        var outputWidth = matrixWidth + kernelWidth - step;

        var output = new float[outputHeight * outputWidth];

        BackConvolutionGPU2(output, matrix.To1DArray(), kernel.To1DArray(), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

        return output;
    }

    public static unsafe float[] To1DArray(this Matrix[] outputs)
    {
        var imageHeight = outputs[0].GetLength(0);
        var imageWidth = outputs[0].GetLength(1);
        var result = new float[outputs.Length * imageHeight * imageWidth];

        fixed (float* r = result)
            for (var i = 0; i < outputs.Length; i++)
                fixed (float* oValue = outputs[i].Value)
                    for (var h = 0; h < imageHeight; h++)
                        for (var w = 0; w < imageWidth; w++)
                            r[(i * (imageWidth * imageHeight)) + (h * imageWidth + w)] = oValue[h * imageWidth + w];

        return result;
    }

    public static unsafe float[] To1DArray(this Matrix output)
    {
        var imageHeight = output.GetLength(0);
        var imageWidth = output.GetLength(1);
        var result = new float[imageHeight * imageWidth];

        fixed (float* r = result)
                fixed (float* oValue = output.Value)
                    for (var h = 0; h < imageHeight; h++)
                        for (var w = 0; w < imageWidth; w++)
                            r[h * imageWidth + w] = oValue[h * imageWidth + w];

        return result;
    }

    public static float[] To1DFloatArray(this Matrix outputs)
    {
        var imageHeight = outputs.GetLength(0);
        var imageWidth = outputs.GetLength(1);
        var result = new float[imageHeight * imageWidth];

        for (var h = 0; h < imageHeight; h++)
        {
            for (var w = 0; w < imageWidth; w++)
            {
                result[h * imageWidth + w] = (float)outputs[h, w];
            }
        }

        return result;
    }
}