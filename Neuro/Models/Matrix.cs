using System;
using System.Runtime.InteropServices;

namespace Neuro.Models
{
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
                fixed(float* v = value)
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

        public static unsafe Matrix Rot180f(this Matrix input)
        {
            if (input.Value == null || input.Value.Length == 0)
                return input;

            var height = input.Value.GetLength(0);
            var width = input.Value.GetLength(1);
            var length = height * width;
            var output = new float[length];

            Rot180GPU(output, input.To1DArray(), width, length);

            var result = new float[height, width];

            fixed (float* v = output, r = result)
                for (var x = 0; x < length; x++)
                    r[x] = v[x];

            return new Matrix(result);
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

            ConvolutionGPU2(output, matrix.To1DArray(), kernel.To1DArray(), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

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

        public static Matrix BackConvolutionGPU(this Matrix matrix, Matrix kernel, int step = 1)
        {
            var matrixHeight = matrix.Value.GetLength(0);
            var matrixWidth = matrix.Value.GetLength(1);
            var kernelHeight = kernel.Value.GetLength(0);
            var kernelWidth = kernel.Value.GetLength(1);
            var outputHeight = matrixHeight + kernelHeight - step;
            var outputWidth = matrixWidth + kernelWidth - step;

            var output = new float[outputHeight * outputWidth];

            BackConvolutionGPU2(output, matrix.To1DArray(), kernel.To1DArray(), matrixWidth, matrixHeight, kernelWidth, kernelHeight, outputWidth, outputHeight);

            return new Matrix(output, outputWidth);
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
    }
}
