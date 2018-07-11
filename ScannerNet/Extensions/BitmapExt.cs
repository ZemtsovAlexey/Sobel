using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ScannerNet.Extensions
{
    public static class BitmapExt
    {
        public static Bitmap ToBitmap(this int[,] data)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            const int step = 4;

            unsafe
            {
                for (var y = 0; y < data.GetLength(0); y++)
                {
                    var row = (byte*)bData.Scan0 + y * bData.Stride;
                    var offset = 0;
                    
                    for (var x = 0; x < data.GetLength(1); x++)
                    {
                        var bright = (byte) Math.Max(0, Math.Min(255, data[y, x]));
                        row[offset + 3] = 255;
                        row[offset + 2] = bright;
                        row[offset + 1] = bright;
                        row[offset] = bright;

                        offset += step;
                    }
                }   
            }
            
            bitmap.UnlockBits(bData);

            return bitmap;
        }
        
        public static Bitmap ToBitmap(this double[,] data)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            const int step = 4;

            unsafe
            {
                for (var y = 0; y < data.GetLength(0); y++)
                {
                    var row = (byte*)bData.Scan0 + y * bData.Stride;
                    var offset = 0;
                    
                    for (var x = 0; x < data.GetLength(1); x++)
                    {
                        var bright = (byte) Math.Max(0, Math.Min(255, data[y, x]));
                        row[offset + 3] = 255;
                        row[offset + 2] = bright;
                        row[offset + 1] = bright;
                        row[offset] = bright;

                        offset += step;
                    }
                }   
            }
            
            bitmap.UnlockBits(bData);

            return bitmap;
        }
        
        public static int[,] GetGrayMap(this Bitmap bitmap)
        {
            var result = new int[bitmap.Height, bitmap.Width];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var step = bitmap.GetStep();

            unsafe
            {
                int imageHeight = bitmap.Height;
                int imageWidth = bitmap.Width;
                
                Parallel.For(0, imageHeight, (int y) =>
                {
                    var pRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;
                    
                    Parallel.For(0, imageWidth, (int x) =>
                    {
                        var offset = x * step;
                        result[y, x] = step == 1 ? pRow[offset] : (pRow[offset + 2] + pRow[offset + 1] + pRow[offset]) / 3;
                    });
                });
            }
            
            bitmap.UnlockBits(bitmapData);

            return result;
        }
        
        public static double[,] GetDoubleMatrix(this Bitmap bitmap)
        {
            var result = new double[bitmap.Height, bitmap.Width];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var step = bitmap.GetStep();

            unsafe
            {
                int imageHeight = bitmap.Height;
                int imageWidth = bitmap.Width;
                
                Parallel.For(0, imageHeight, (int y) =>
                {
                    var pRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;
                    
                    Parallel.For(0, imageWidth, (int x) =>
                    {
                        var offset = x * step;
                        result[y, x] = step == 1 ? pRow[offset] : (pRow[offset + 2] + pRow[offset + 1] + pRow[offset]) / 3;
                    });
                });
            }
            
            bitmap.UnlockBits(bitmapData);

            return result;
        }
        
        public static Bitmap Contrast(this Bitmap image, float value)
        {
            value = (100.0f + value) / 100.0f;
            value *= value;
            
            var newBitmap = (Bitmap)image.Clone();
            var data = newBitmap.LockBits(
                new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                ImageLockMode.ReadWrite,
                newBitmap.PixelFormat);
            var height = newBitmap.Height;
            var width = newBitmap.Width;

            unsafe
            {
                for (var y = 0; y < height; ++y)
                {
                    var row = (byte*)data.Scan0 + y * data.Stride;
                    var columnOffset = 0;
                    
                    for (var x = 0; x < width; ++x)
                    {
                        var b = row[columnOffset];
                        var g = row[columnOffset + 1];
                        var r = row[columnOffset + 2];
                        
                        var red = (((r / 255.0f - 0.5f) * value) + 0.5f) * 255.0f;
                        var green = (((g / 255.0f - 0.5f) * value) + 0.5f) * 255.0f;
                        var blue = (((b / 255.0f - 0.5f) * value) + 0.5f) * 255.0f;

                        row[columnOffset] = (byte)Math.Max(0, Math.Min(255, blue));
                        row[columnOffset + 1] = (byte)Math.Max(0, Math.Min(255, green));
                        row[columnOffset + 2] = (byte)Math.Max(0, Math.Min(255, red));

                        columnOffset += 4;
                    }
                }
            }

            newBitmap.UnlockBits(data);

            return newBitmap;
        }
        
        private static void GenerateGaussianKernel(int N, float S ,out int Weight, out int [,] GaussianKernel)
        {
            float Sigma = S ;
            float pi;
            pi = (float)Math.PI;
            int i, j;
            int SizeofKernel=N;
            
            float [,] Kernel = new float [N,N];
            GaussianKernel = new int [N,N];
            float[,] OP = new float[N, N];
            float D1,D2;


            D1= 1/(2*pi*Sigma*Sigma);
            D2= 2*Sigma*Sigma;
            
            float min=1000;

            for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
            {
               for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                {
                    Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = ((1 / D1) * (float)Math.Exp(-(i * i + j * j) / D2));
                    if (Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] < min)
                        min = Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];

                 }
            }
            int mult = (int)(1 / min);
            int sum = 0;
            if ((min > 0) && (min < 1))
            {

                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] * mult, 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
            else
            {
                sum = 0;
                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] , 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
          //Normalizing kernel Weight
          Weight= sum;
         
         return;
        }

        public static int[,] GaussianFilter(this int[,] Data, int KernelSize = 5, int Sigma = 1)
        {
            GenerateGaussianKernel(KernelSize, Sigma, out int KernelWeight, out int[,] GaussianKernel);

            var Width = Data.GetLength(1);
            var Height = Data.GetLength(0);
            int[,] Output = new int[Height, Width];
            int i, j,k,l;
            int Limit = KernelSize /2;

            float Sum=0;


            Output = Data; // Removes Unwanted Data Omission due to kernel bias while convolution

         
            for (i = Limit; i <= ((Width - 1) - Limit); i++)
            {
                for (j = Limit; j <= ((Height - 1) - Limit); j++)
                {
                    Sum = 0;
                    for (k = -Limit; k <= Limit; k++)
                    {

                        for (l = -Limit; l <= Limit; l++)
                        {
                            Sum = Sum + ((float)Data[j + l, i + k] * GaussianKernel [Limit + k, Limit + l]); 
                            
                        }
                    }
                    Output[j, i] = (int)(Math.Round(Sum/ (float)KernelWeight));
                }

            }


            return Output;
        }

        public static int GetStep(this Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                    case PixelFormat.Format24bppRgb:
                        return 3;
                    
                    case PixelFormat.Format8bppIndexed:
                        return 1;
                    
                    default:
                        return 4;
            }
        }
    }
}