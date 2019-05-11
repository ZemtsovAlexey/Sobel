//using System;

//namespace Neuro.Models
//{
//    public class Matrix
//    {
//        private int Width { get; }

//        private int Height { get; }

//        public Memory<double> Value { get; }

//        public Matrix(double[,] value)
//        {
//            var imageHeight = value.GetLength(0);
//            var imageWidth = value.GetLength(1);
//            var result = new double[value.Length * imageHeight * imageWidth];

//            for (var h = 0; h < imageHeight; h++)
//            {
//                for (var w = 0; w < imageWidth; w++)
//                {
//                    result[h * imageWidth + w] = value[h, w];
//                }
//            }

//            Value = result;
//            Width = imageWidth;
//            Height = imageHeight;
//        }

//        public double this[int y, int x]
//        {
//            get => Value.Span[y * Width + x];
//            set => Value.Span[y * Width + x] = value;
//        }

//        public int Length => Value.Length;

//        public int GetLength(int dimension)
//        {
//            switch (dimension)
//            {
//                case 0: return Height;
//                case 1: return Width;

//                default: throw new ArgumentOutOfRangeException();
//            }
//        }

//        public static Matrix operator +(Matrix a, Matrix b)
//        {
//            //#if DEBUG
            
//            //if (a.Value. == null || a.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //if (b.Value == null || b.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(b));

//            //if (a.Value.Length != b.Value.Length)
//            //    throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

//            //#endif
            
//            var height = a.GetLength(0);
//            var width = a.GetLength(1);
//            var result = new double[height, width];

//            for (var y = 0; y < height; y++)
//                for (var x = 0; x < height; x++)
//                    result[y, x] = a[y, x] + b[y, x];

//            return new Matrix(result);
//        }

//        public static Matrix operator -(Matrix a, Matrix b)
//        {
//            //#if DEBUG
            
//            //if (a.Value == null || a.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //if (b.Value == null || b.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(b));

//            //#endif
            
//            if (a.Value.Length != b.Value.Length)
//                throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

//            var height = a.GetLength(0);
//            var width = a.GetLength(1);
//            var result = new double[height, width];

//            for (var y = 0; y < height; y++)
//                for (var x = 0; x < height; x++)
//                    result[y, x] = a[y, x] - b[y, x];

//            return new Matrix(result);
//        }

//        public static Matrix operator -(Matrix a, double value)
//        {
//            //#if DEBUG
            
//            //if (a.Value == null || a.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //#endif
            
//            var height = a.GetLength(0);
//            var width = a.GetLength(1);
//            var result = new double[height, width];

//            for (var y = 0; y < height; y++)
//                for (var x = 0; x < height; x++)
//                    result[y, x] = a[y, x] - value;

//            return new Matrix(result);
//        }

//        public static unsafe Matrix operator +(Matrix a, double value)
//        {
//            //#if DEBUG
            
//            //if (a.Value == null || a.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //#endif
            
//            var height = a.GetLength(0);
//            var width = a.GetLength(1);
//            var result = new double[height, width];

//            fixed (double* r = result)
//                for (var y = 0; y < height; y++)
//                for (var x = 0; x < width; x++)
//                    r[y * width + x] = a[y, x] + value;

//            return new Matrix(result);
//        }

//        public static Matrix operator *(Matrix a, Matrix b)
//        {
//            //#if DEBUG
            
//            //if (a.Value == null || a.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //if (b.Value == null || b.Value?.Length == 0)
//            //    throw new ArgumentNullException(nameof(a));

//            //if (a.Value.Length != b.Value.Length)
//            //    throw new Exception("Matrix size of \"a\" not equal to size of \"b\"");

//            //#endif
            
//            var height = a.GetLength(0);
//            var width = a.GetLength(1);
//            var result = new double[height, width];

//            for (var y = 0; y < height; y++)
//                for (var x = 0; x < height; x++)
//                    result[y, x] = a[y, x] * b[y, x];

//            return new Matrix(result);
//        }

//        public static unsafe Matrix operator *(Matrix matrix, Func<double, double> func)
//        {
//            //#if DEBUG
            
//            //if (Matrix.Value == null || Matrix.Value.Length == 0)
//            //    throw new ArgumentNullException(nameof(Matrix));

//            //if (func == null)
//            //    throw new ArgumentNullException(nameof(func));

//            //#endif
            
//            var MatrixHeight = matrix.GetLength(0);
//            var MatrixWidth = matrix.GetLength(1);
//            var output = new double[MatrixHeight, MatrixWidth];

//            fixed (double* v = output)
//                for (var y = 0; y < MatrixHeight; y++)
//                for (var x = 0; x < MatrixWidth; x++)
//                    v[y * MatrixWidth + x] = func.Invoke(matrix[y, x]);

//            return new Matrix(output);
//        }

//        public static Matrix operator *(Matrix matrix, double value)
//        {
//            //#if DEBUG
            
//            //if (Matrix.Value == null || Matrix.Value.Length == 0)
//            //    throw new ArgumentNullException(nameof(Matrix));

//            //#endif
            
//            var MatrixHeight = matrix.GetLength(0);
//            var MatrixWidth = matrix.GetLength(1);
//            var output = new double[MatrixHeight, MatrixWidth];

//            for (var y = 0; y < MatrixHeight; y++)
//                for (var x = 0; x < MatrixWidth; x++)
//                    output[y, x] = matrix[y, x] * value;

//            return new Matrix(output);
//        }

//        public static Matrix operator /(Matrix matrix, double value)
//        {
//            //#if DEBUG
            
//            //if (Matrix.Value == null || Matrix.Value.Length == 0)
//            //    throw new ArgumentNullException(nameof(Matrix));

//            //#endif
            
//            var MatrixHeight = matrix.GetLength(0);
//            var MatrixWidth = matrix.GetLength(1);
//            var output = new double[MatrixHeight, MatrixWidth];

//            for (var y = 0; y < MatrixHeight; y++)
//                for (var x = 0; x < MatrixWidth; x++)
//                    output[y, x] = matrix[y, x] / value;

//            return new Matrix(output);
//        }
//    }

//    internal static class MatrixExtensions
//    {
//        public static unsafe Matrix Rot180(this Matrix input)
//        {
//            if (input.Value.Length == 0)
//                return input;

//            var height = input.GetLength(0);
//            var width = input.GetLength(1);
//            var result = new double[height, width];

//            fixed (double* r = result)
//                for (var y = 0; y < height; y++)
//                for (var x = 0; x < width; x++)
//                    r[y * width + x] = input[(height - 1 - y), (width - 1 - x)];

//            return new Matrix(result);
//        }

//        public static unsafe Matrix Sum(this Matrix[] Matrixes)
//        {
//            #if DEBUG
            
//            if (Matrixes == null || Matrixes.Length < 1)
//                throw new ArgumentNullException(nameof(Matrixes));

//            #endif
            
//            var height = Matrixes[0].GetLength(0);
//            var width = Matrixes[0].GetLength(1);
//            var sum = new double[height, width];

//            foreach (var Matrix in Matrixes)
//                fixed (double* r = sum)
//                    for (var y = 0; y < height; y++)
//                    for (var x = 0; x < width; x++)
//                        r[y * width + x] += Matrix[y , x];

//            return new Matrix(sum);
//        }

//        public static unsafe double Sum(this Matrix Matrix)
//        {
//            #if DEBUG
            
//            if (Matrix == null || Matrix.Length < 1)
//                throw new ArgumentNullException(nameof(Matrix));

//            #endif
            
//            var height = Matrix.GetLength(0);
//            var width = Matrix.GetLength(1);
//            var sum = 0d;

//                for (var y = 0; y < height; y++)
//                for (var x = 0; x < width; x++)
//                    sum += Matrix[y, x];

//            return sum;
//        }

//        public static unsafe Matrix Convolution(this Matrix Matrix, Matrix kernel, int step = 1)
//        {
//            #if DEBUG
            
//            if (Matrix.Value.Length == 0)
//                throw new ArgumentNullException(nameof(Matrix));

//            if (kernel.Value.Length == 0)
//                throw new ArgumentNullException(nameof(kernel));

//            if (Matrix.Value.Length < kernel.Value.Length)
//                throw new Exception("Kernel size more then size of Matrix");

//            #endif
            
//            var MatrixHeight = Matrix.GetLength(0);
//            var MatrixWidth = Matrix.GetLength(1);
//            var kernelHeight = kernel.GetLength(0);
//            var kernelWidth = kernel.GetLength(1);
//            var outputHeight = MatrixHeight - kernelHeight + step;
//            var outputWidth = MatrixWidth - kernelWidth + step;

//            var output = new double[outputHeight, outputWidth];

//            fixed (double* o = output)
//                for (var y = 0; y < outputHeight; y++)
//                for (var x = 0; x < outputWidth; x++)
//                for (var h = 0; h < kernelHeight; h++)
//                for (var w = 0; w < kernelWidth; w++)
//                    o[y * outputWidth + x] += (Matrix[(y + h), x + w] * kernel[h, w]);

//            return new Matrix(output);
//        }

//        public static Matrix BackConvolution(this Matrix Matrix, Matrix kernel, int step = 1)
//        {
//            #if DEBUG
            
//            if (Matrix.Value.Length == 0)
//                throw new ArgumentNullException(nameof(Matrix));

//            if (kernel.Value.Length == 0)
//                throw new ArgumentNullException(nameof(kernel));

//            if (Matrix.Value.Length < kernel.Value.Length)
//                throw new Exception("Kernel size more then size of Matrix");
            
//            #endif

//            var MatrixHeight = Matrix.GetLength(0);
//            var MatrixWidth = Matrix.GetLength(1);
//            var kernelHeight = kernel.GetLength(0);
//            var kernelWidth = kernel.GetLength(1);
//            var outputHeight = MatrixHeight + kernelHeight - step;
//            var outputWidth = MatrixWidth + kernelWidth - step;

//            var output = new double[outputHeight, outputWidth];

//            var padY = kernelHeight - step;
//            var padX = kernelWidth - step;

//            for (var y = -(padY); y < MatrixHeight + padY - step; y++)
//                for (var x = -(padX); x < MatrixWidth + padX - step; x++)
//                    for (var h = y < 0 ? 0 - y : 0; h < (y + kernelHeight > MatrixHeight ? (MatrixHeight - (y + kernelHeight)) + kernelHeight : kernelHeight); h++)
//                        for (var w = x < 0 ? 0 - x : 0; w < (x + kernelWidth > MatrixWidth ? (MatrixWidth - (x + kernelWidth)) + kernelWidth : kernelWidth); w++)
//                            output[y + padY, x + padX] += Matrix[y + h, x + w] * kernel[h, w];

//            return new Matrix(output);
//        }

//        public static unsafe double[] To1DArray(this Matrix[] outputs)
//        {
//            var imageHeight = outputs[0].GetLength(0);
//            var imageWidth = outputs[0].GetLength(1);
//            var result = new double[outputs.Length * imageHeight * imageWidth];

//            fixed (double* r = result)
//                for (var i = 0; i < outputs.Length; i++)
//                        for (var h = 0; h < imageHeight; h++)
//                        for (var w = 0; w < imageWidth; w++)
//                            r[(i * (imageWidth * imageHeight)) + (h * imageWidth + w)] = outputs[i][h, w];

//            return result;
//        }

//        public static double[] To1DArray(this Matrix outputs)
//        {
//            var imageHeight = outputs.GetLength(0);
//            var imageWidth = outputs.GetLength(1);
//            var result = new double[outputs.Length * imageHeight * imageWidth];

//            for (var h = 0; h < imageHeight; h++)
//            {
//                for (var w = 0; w < imageWidth; w++)
//                {
//                    result[h * imageWidth + w] = outputs[h, w];
//                }
//            }

//            return result;
//        }
//    }
//}
