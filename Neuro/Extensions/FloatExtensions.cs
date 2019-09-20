using System;

namespace Neuro.Extensions
{
    public static class FloatExtensions
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble(); // range 0.0 to 1.0
        }

        public static float[,] Sum(this float[][,] inputs)
        {
            if (inputs == null || inputs.Length < 1)
                return null;

            var inputHeight = inputs[0].GetLength(0);
            var inputWidth = inputs[0].GetLength(1);
            var sum = new float[inputHeight, inputWidth];

            foreach (var input in inputs)
            {
                for (var y = 0; y < inputHeight; y++)
                {
                    for (var x = 0; x < inputWidth; x++)
                    {
                        sum[y, x] += input[y, x];
                    }
                }
            }

            return sum;
        }

        public static float Sum(this float[,] inputs)
        {
            if (inputs == null || inputs.Length < 1)
                return 0;

            var inputHeight = inputs.GetLength(0);
            var inputWidth = inputs.GetLength(1);
            float sum = 0;

            for (var y = 0; y < inputHeight; y++)
            {
                for (var x = 0; x < inputWidth; x++)
                {
                    sum += inputs[y, x];
                }
            }

            return sum;
        }

        public static float[,] Rot180(this float[,] input)
        {
            if (input == null || input.Length == 0)
                return input;

            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var result = new float[height, width];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < height; x++)
                    result[y, x] = input[height - 1 - y, width - 1 - x];

            return result;
        }

        public static float[,] Сonvolution(this float[,] matrix, float[,] kernel, int step = 1)
        {
            if (matrix == null || matrix.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            if (kernel == null || kernel.Length == 0)
                throw new ArgumentNullException("Kernel is empty");

            if (matrix.Length < kernel.Length)
                throw new Exception("Kernel size more then size of matrix");

            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var kernelHeight = kernel.GetLength(0);
            var kernelWidth = kernel.GetLength(1);
            var outputHeight = matrixHeight - kernelHeight + step;
            var outputWidth = matrixWidth - kernelWidth + step;

            var output = new float[outputHeight, outputWidth];

            for (var y = 0; y < outputHeight; y++)
                for (var x = 0; x < outputWidth; x++)
                    for (var h = 0; h < kernelHeight; h++)
                        for (var w = 0; w < kernelWidth; w++)
                            output[y, x] += matrix[y + h, x + w] * kernel[h, w];

            return output;
        }

        public static float[,] Multiply(this float[,] matrix, float value)
        {
            if (matrix == null || matrix.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var output = new float[matrixHeight, matrixWidth];

            for (var y = 0; y < matrixHeight; y++)
                for (var x = 0; x < matrixWidth; x++)
                    output[y, x] = matrix[y, x] * value;

            return output;
        }

        public static float[,] Multiply(this float[,] matrix, Func<float, float> func)
        {
            if (matrix == null || matrix.Length == 0)
                throw new ArgumentNullException("Matrix is empty");

            if (func == null)
                throw new ArgumentNullException("Func is null");

            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            var output = new float[matrixHeight, matrixWidth];

            for (var y = 0; y < matrixHeight; y++)
                for (var x = 0; x < matrixWidth; x++)
                    output[y, x] = func.Invoke(matrix[y, x]);

            return output;
        }
    }
}
