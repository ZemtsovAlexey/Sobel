using System;

namespace Neuro.Models
{
    public class Matrix
    {
        public float[,] Value { get; set; }

        public Matrix(float[,] value)
        {
            Value = value;
        }

        public float this[int y, int x]
        {
            get { return Value[y, x]; }
            set { Value[y, x] = value; }
        }

        public int Length => Value.Length;

        public int GetLength(int demension) => Value.GetLength(demension);

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Value == null || a.Value?.Length == 0)
                throw new ArgumentNullException("Matrix \"a\" is empty");

            if (b.Value == null || b.Value?.Length == 0)
                throw new ArgumentNullException("Matrix \"b\" is empty");

            if (a.Value.Length != b.Value.Length)
                throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

            var height = a.Value.GetLength(0);
            var width = a.Value.GetLength(1);
            var result = new float[height, width];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < height; x++)
                    result[y, x] = a.Value[y, x] + b.Value[y, x];

            return new Matrix(result);
        }

        public static Matrix operator +(Matrix a, float value)
        {
            if (a.Value == null || a.Value?.Length == 0)
                throw new ArgumentNullException("Matrix \"a\" is empty");

            var height = a.Value.GetLength(0);
            var width = a.Value.GetLength(1);
            var result = new float[height, width];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < height; x++)
                    result[y, x] = a.Value[y, x] + value;

            return new Matrix(result);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Value == null || a.Value?.Length == 0)
                throw new ArgumentNullException("Matrix \"a\" is empty");

            if (b.Value == null || b.Value?.Length == 0)
                throw new ArgumentNullException("Matrix \"b\" is empty");

            if (a.Value.Length != b.Value.Length)
                throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

            var height = a.Value.GetLength(0);
            var width = a.Value.GetLength(1);
            var result = new float[height, width];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < height; x++)
                    result[y, x] = a.Value[y, x] * b.Value[y, x];

            return new Matrix(result);
        }

        public static Matrix operator *(Matrix matrix, Func<float, float> func)
        {
            if (matrix.Value == null || matrix.Value.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            if (func == null)
                throw new ArgumentNullException("Func is null");

            var matrixHeight = matrix.Value.GetLength(0);
            var matrixWidth = matrix.Value.GetLength(1);
            var output = new float[matrixHeight, matrixWidth];

            for (var y = 0; y < matrixHeight; y++)
                for (var x = 0; x < matrixWidth; x++)
                    output[y, x] = func.Invoke(matrix.Value[y, x]);

            return new Matrix(output);
        }

        public static Matrix operator *(Matrix matrix, float value)
        {
            if (matrix.Value == null || matrix.Value.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            var matrixHeight = matrix.Value.GetLength(0);
            var matrixWidth = matrix.Value.GetLength(1);
            var output = new float[matrixHeight, matrixWidth];

            for (var y = 0; y < matrixHeight; y++)
                for (var x = 0; x < matrixWidth; x++)
                    output[y, x] = matrix[y, x] * value;

            return new Matrix(output);
        }
    }

    internal static class MatrixExtensions
    {
        public static Matrix Rot180(this Matrix input)
        {
            if (input.Value == null || input.Value.Length == 0)
                return input;

            var height = input.Value.GetLength(0);
            var width = input.Value.GetLength(1);
            var result = new float[height, width];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < height; x++)
                    result[y, x] = input.Value[height - 1 - y, width - 1 - x];

            return new Matrix(result);
        }

        public static Matrix Sum(this Matrix[] matrixs)
        {
            if (matrixs == null || matrixs.Length < 1)
                throw new ArgumentNullException("Matrixs is null");

            var inputHeight = matrixs[0].Value.GetLength(0);
            var inputWidth = matrixs[0].Value.GetLength(1);
            var sum = new float[inputHeight, inputWidth];

            foreach (var matrix in matrixs)
            {
                for (var y = 0; y < inputHeight; y++)
                {
                    for (var x = 0; x < inputWidth; x++)
                    {
                        sum[y, x] += matrix.Value[y, x];
                    }
                }
            }

            return new Matrix(sum);
        }

        public static Matrix Сonvolution(this Matrix matrix, Matrix kernel, int step = 1)
        {
            if (matrix.Value == null || matrix.Value.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            if (kernel.Value == null || kernel.Value.Length == 0)
                throw new ArgumentNullException("Kernel is empty");

            if (matrix.Value.Length < kernel.Value.Length)
                throw new Exception("Kernel size more then size of matrix");

            var matrixHeight = matrix.Value.GetLength(0);
            var matrixWidth = matrix.Value.GetLength(1);
            var kernelHeight = kernel.Value.GetLength(0);
            var kernelWidth = kernel.Value.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;

            var output = new float[outputHeight, outputWidth];

            for (var y = 0; y < outputHeight; y++)
                for (var x = 0; x < outputWidth; x++)
                    for (var h = 0; h < kernelHeight; h++)
                        for (var w = 0; w < kernelWidth; w++)
                            output[y, x] += matrix.Value[y + h, x + w] * kernel.Value[h, w];

            return new Matrix(output);
        }

        public static Matrix BackСonvolution(this Matrix matrix, Matrix kernel, int step = 1)
        {
            if (matrix.Value == null || matrix.Value.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            if (kernel.Value == null || kernel.Value.Length == 0)
                throw new ArgumentNullException("Kernel is empty");

            if (matrix.Value.Length < kernel.Value.Length)
                throw new Exception("Kernel size more then size of matrix");

            var matrixHeight = matrix.Value.GetLength(0);
            var matrixWidth = matrix.Value.GetLength(1);
            var kernelHeight = kernel.Value.GetLength(0);
            var kernelWidth = kernel.Value.GetLength(1);
            var outputHeight = matrixHeight + kernelHeight - step;
            var outputWidth = matrixWidth + kernelWidth - step;

            var output = new float[outputHeight, outputWidth];

            var paddY = kernelHeight - step;
            var paddX = kernelWidth - step;

            for (var y = -(paddY); y < matrixHeight + paddY - step; y++)
                for (var x = -(paddX); x < matrixWidth + paddX - step; x++)
                    for (var h = y < 0 ? 0 - y : 0; h < (y + kernelHeight > matrixHeight ? (matrixHeight - (y + kernelHeight)) + kernelHeight : kernelHeight); h++)
                        for (var w = x < 0 ? 0 - x : 0; w < (x + kernelWidth > matrixWidth ? (matrixWidth - (x + kernelWidth)) + kernelWidth : kernelWidth); w++)
                            output[y + paddY, x + paddX] += matrix[y + h, x + w] * kernel[h, w];

            return new Matrix(output);
        }

        public static float[] To1DArray(this Matrix[] outputs)
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

        public static float[] To1DArray(this Matrix outputs)
        {
            var imageHeight = outputs.GetLength(0);
            var imageWidth = outputs.GetLength(1);
            var result = new float[outputs.Length * imageHeight * imageWidth];

            for (var h = 0; h < imageHeight; h++)
            {
                for (var w = 0; w < imageWidth; w++)
                {
                    result[h * imageWidth + w] = outputs[h, w];
                }
            }

            return result;
        }
    }
}
