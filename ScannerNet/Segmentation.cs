using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ScannerNet.Extensions;
using ScannerNet.Models;

namespace ScannerNet
{
    public static class Segmentation
    {
        private static int[,] GX = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        private static int[,] GY = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        
        public static Bitmap VerticalSobel(Bitmap bitmap)
        {
            var newBitmap = (Bitmap) bitmap.Clone();
            var height = newBitmap.Height;
            var width = newBitmap.Width;
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

            unsafe
            {
                for (var y = 0; y < height; y++)
                {
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var newRow = (byte*)newBitmapData.Scan0 + (y * newBitmapData.Stride);
                    int columnOffset = 0;
                    
                    for (int x = 0; x < width; ++x)
                    {
                        if (columnOffset > 3)
                        {
                            var pixCur = GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                            var pixPrev = GetBright(row[columnOffset - 2], row[columnOffset - 3], row[columnOffset - 4]);
                       
                            var dif = Math.Max(pixCur, pixPrev) - Math.Min(pixCur, pixPrev);
                            var curPos = (byte)Math.Min(255, dif / 2);

                            newRow[columnOffset] = curPos;
                            newRow[columnOffset + 1] = curPos;
                            newRow[columnOffset + 2] = curPos;
                        }
                        
                        columnOffset += 4;
                    }
                }
            }
            
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }
        
        public static Bitmap Sobel(Bitmap bitmap)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            var height = newBitmap.Height;
            var width = newBitmap.Width;
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            var step = bitmapData.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            unsafe
            {
                for (var y = 1; y < height - 1; y++)
                {
                    var rowPrev = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    
                    var newRow = (byte*)newBitmapData.Scan0 + (y * newBitmapData.Stride);
                    int columnOffset = 0;
                    
                    for (int x = 0; x < width; ++x)
                    {
                        if (columnOffset > 3)
                        {
                            var rowPrevPix = GetBright(rowPrev[columnOffset + 2], rowPrev[columnOffset + 1], rowPrev[columnOffset]);
                            var rowPix = GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                            var rowPixPrev = GetBright(row[columnOffset - 2], row[columnOffset - 3], row[columnOffset - 4]);
                       
                            var rowPrevDif = Math.Max(rowPix, rowPixPrev) - Math.Min(rowPix, rowPixPrev);
                            var rowDif = Math.Max(rowPix, rowPrevPix) - Math.Min(rowPix, rowPrevPix);
                            var curPos = (byte)Math.Min(255, (rowDif + rowPrevDif) / 2);

                            newRow[columnOffset] = curPos;
                            newRow[columnOffset + 1] = curPos;
                            newRow[columnOffset + 2] = curPos;
                            
                            if (step > 3)
                                newRow[columnOffset + 3] = 255;
                        }
                        
                        columnOffset += step;
                    }
                }
            }
            
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }
        
        public static Bitmap Sobel2(Bitmap bitmap)
        {
//            var newBitmap = (Bitmap) bitmap.Clone();
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            var height = newBitmap.Height;
            var width = newBitmap.Width;
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            var step = bitmapData.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            unsafe
            {
                for (var y = 1; y < height - 1; y++)
                {
                    var rowPrev = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    
                    var newRowPrev = (byte*)newBitmapData.Scan0 + ((y - 1) * newBitmapData.Stride);
                    var newRow = (byte*)newBitmapData.Scan0 + (y * newBitmapData.Stride);
                    int columnOffset = 0;
                    
                    for (int x = 0; x < width; ++x)
                    {
                        if (columnOffset > 3)
                        {
                            var rowPrevPix = GetBright(rowPrev[columnOffset + 2], rowPrev[columnOffset + 1], rowPrev[columnOffset]);
                            var rowPix = GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                            var rowPixPrev = GetBright(row[columnOffset - 2], row[columnOffset - 3], row[columnOffset - 4]);
                       
                            var rowPrevDif = Math.Max(rowPix, rowPixPrev) - Math.Min(rowPix, rowPixPrev);
                            var rowDif = Math.Max(rowPix, rowPrevPix) - Math.Min(rowPix, rowPrevPix);
                            var curPos = (byte)Math.Min(255, (rowDif + rowPrevDif) / 2);

//                            newRow[columnOffset] = curPos;
//                            newRow[columnOffset + 1] = curPos;
//                            newRow[columnOffset + 2] = curPos;
//                            
//                            if (step > 3)
//                                newRow[columnOffset + 3] = 255;
                            
                            if (curPos > 0)
                            {
                                var curOffset = columnOffset;
                                var curRow = newRow;
                                
                                if (rowPix < rowPixPrev)
                                {
                                    curOffset -= step;
                                }

                                if (rowPix < rowPrevPix)
                                {
                                    curRow = newRowPrev;
                                    
//                                    newRowPrev[curOffset] = curPos;
//                                    newRowPrev[curOffset + 1] = curPos;
//                                    newRowPrev[curOffset + 2] = curPos;
                                }
                                
                                curRow[curOffset] = curPos;
                                curRow[curOffset + 1] = curPos;
                                curRow[curOffset + 2] = curPos;
                            
                                if (step > 3)
                                    curRow[curOffset + 3] = 255;
                            }
                            
                            
                            
//                            if (curPos > 10)
//                            {
//                                newRow[columnOffset] = 0;
//                                newRow[columnOffset + 1] = 0;
//                                newRow[columnOffset + 2] = 0;
//                                newRow[columnOffset + 3] = 255;
//                            }
//                            else
//                            {
//                                newRow[columnOffset] = 255;
//                                newRow[columnOffset + 1] = 255;
//                                newRow[columnOffset + 2] = 255;
//                                newRow[columnOffset + 3] = 255;
//                            }
//                            else
//                            {
//                                newRow[columnOffset - 4] = (byte)Math.Min(255, rowDif);;
//                                newRow[columnOffset - 4 + 1] = (byte)Math.Min(255, rowDif);;
//                                newRow[columnOffset - 4 + 2] = (byte)Math.Min(255, rowDif);;
//                            }
                        }
                        
                        columnOffset += step;
                    }
                }
            }
            
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }
        
        public static Bitmap Sobel3(Bitmap bitmap)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            var height = newBitmap.Height;
            var width = newBitmap.Width;
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            var step = bitmapData.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            unsafe
            {
                for (var y = 1; y < height - 1; y++)
                {
                    var Y1 = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
                    var Y2 = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var Y3 = (byte*)bitmapData.Scan0 + ((y + 1) * bitmapData.Stride);
                    
                    var newRow = (byte*)newBitmapData.Scan0 + (y * newBitmapData.Stride);
                    int columnOffset = step;
                    var m = new int[3, 3];
                    
                    for (int x = 1; x < width - 1; x++)
                    {
                        m[0, 0] = GetBright(Y1[columnOffset + 2 - step], Y1[columnOffset + 1 - step], Y1[columnOffset - step]);
                        m[0, 1] = GetBright(Y1[columnOffset + 2], Y1[columnOffset + 1], Y1[columnOffset]);
                        m[0, 2] = GetBright(Y1[columnOffset + 2 + step], Y1[columnOffset + 1 + step], Y1[columnOffset + step]);
                        
                        m[1, 0] = GetBright(Y2[columnOffset + 2 - step], Y2[columnOffset + 1 - step], Y2[columnOffset - step]);
                        m[1, 1] = GetBright(Y2[columnOffset + 2], Y2[columnOffset + 1], Y2[columnOffset]);
                        m[1, 2] = GetBright(Y2[columnOffset + 2 + step], Y2[columnOffset + 1 + step], Y2[columnOffset + step]);
                        
                        m[2, 0] = GetBright(Y3[columnOffset + 2 - step], Y3[columnOffset + 1 - step], Y3[columnOffset - step]);
                        m[2, 1] = GetBright(Y3[columnOffset + 2], Y3[columnOffset + 1], Y3[columnOffset]);
                        m[2, 2] = GetBright(Y3[columnOffset + 2 + step], Y3[columnOffset + 1 + step], Y3[columnOffset + step]);

                        var dx = m.Sum(GX);
                        var dy = m.Sum(GY);
                        
                        var r = Math.Sqrt(dx * dx + dy * dy);
                        var p = (byte) Math.Max(0, Math.Min(255, r));
                        
                        newRow[columnOffset] = p;
                        newRow[columnOffset + 1] = p;
                        newRow[columnOffset + 2] = p;
                            
                        if (step > 3)
                            newRow[columnOffset + 3] = 255;
                        
                        columnOffset += step;
                    }
                }
            }
            
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }
        
        private static int[,] Multiplication(int[,] a, int[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0))
            {
                throw new Exception("Матрицы нельзя перемножить");
            }
            
            int[,] r = new int[a.GetLength(0), b.GetLength(1)];
            
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i,j] += a[i,k] * b[k,j];
                    }
                }
            }
            
            return r;
        }
        
        private static int Sum(this int[,] a, int[,] b)
        {
            int r = 0;
            
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    r += a[i, j] * b[i, j];
                }
            }
            
            return r;
        }

        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }
        
        public static float GetAverBright(Bitmap bitmap)
        {
            var filterdBitmap = bitmap.Contrast(50);
            filterdBitmap = Sobel(filterdBitmap);
            var height = filterdBitmap.Height;
            var width = filterdBitmap.Width;
            var bitmapData = filterdBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, filterdBitmap.PixelFormat);

            var linesBright = new float[height];
            var docBright = new float[height * width];
            
            unsafe
            {
                for (var y = 0; y < height; y++)
                {
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var columnOffset = 0;
                    var lineBright = 0;

                    for (var x = 0; x < width; ++x)
                    {
                        lineBright += GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                        docBright[y * width + x] += GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                        columnOffset += 4;
                    }

                    linesBright[y] = (float)1 / width * lineBright;
                }
            }
            
            filterdBitmap.UnlockBits(bitmapData);

            var imgAvrBright = 0.4f * ((float)1 / height * linesBright.Sum());
            var docAvrBright = ((float)1 / (docBright.Sum() / (height * width)));

            return docAvrBright;
        }
        
        public static Bitmap Canny(this Bitmap bitmap)
        {
            Image<Gray, byte> gray = new Image<Gray, byte>(bitmap);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1, 1, BorderType.Default);
            CvInvoke.Canny(gray, gray, 70, 130);

            var grayMap = gray.ToBitmap();

            return grayMap;
        }

        public static Bitmap ShowTextCord(Bitmap bitmap, byte min = 5)
        {
            var newBitmap = (Bitmap)bitmap.Clone();
//            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            bitmap = bitmap.Contrast(25);
            
//            var canny = new Canny(bitmap);
//            bitmap = canny.DisplayImage(canny.GreyImage);
            
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

            var map = GetMapBySobel(bitmapData);
            
            var step = newBitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
            var cordList = new List<Cord>();
            
            for (var y = 5; y < bitmap.Height; y++)
            {
                int? leftCord = null;

                for (var x = 5; x < bitmap.Width; x++)
                {
                    var point = map[x + y * bitmap.Width];

                    if (leftCord == null && point > min)
                    {
                        leftCord = x;
                    }
                    else if (leftCord != null && point == 0)
                    {
                        var prevCords = cordList.Where(c => c.Bottom < y && c.Bottom > y - 2 && leftCord <= c.Right + 0 && x >= c.Left - 0).ToList();

                        if (!prevCords.Any())
                        {
                            cordList.Add(new Cord(y, y, leftCord.Value, x));
                        }
                        else
                        {
                            var newCord = new Cord
                            {
                                Top = prevCords.Min(c => c.Top),
                                Bottom = y,
                                Left = Math.Min(leftCord.Value, prevCords.Min(c => c.Left)),
                                Right = Math.Max(x, prevCords.Max(c => c.Right)),
                            };

                            cordList.RemoveAll(c => c.Bottom < y && c.Bottom > y - 2 && leftCord <= c.Right + 0 && x >= c.Left - 0);
                            
                            cordList.Add(newCord);
                        }
                        
                        leftCord = null;
                    }
                }
                
                if (leftCord != null)
                {
                    cordList.Add(new Cord(y, y, leftCord.Value, bitmap.Width - 1));
                }
            }

            Random rnd = new Random();
            
            unsafe
            {
                foreach (var cord in cordList.Where(x => x.Bottom - x.Top > 5 && x.Right - x.Left > 5))
                {
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    
                    for (var y = cord.Top; y < cord.Bottom; y++)
                    {
                        var pRow = (byte*)newBitmapData.Scan0 + y * newBitmapData.Stride;
                        var offset = cord.Left * step;

                        for (var x = cord.Left; x < cord.Right; x++)
                        {
                            if (x == cord.Left || x == cord.Right - 1 || y == cord.Top || y == cord.Bottom - 1)
                            {
                                pRow[offset + 2] = randomColor.R;
                                pRow[offset + 1] = randomColor.G;
                                pRow[offset] = randomColor.B;
                            }

                            offset += step;
                        }
                    }
                }
            }
            
//            unsafe
//            {
//                foreach (var cord in cordList.Where(x => x.Bottom - x.Top > 5 && x.Right - x.Left > 5))
//                {
//                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
//                    
//                    for (var y = cord.Top; y < cord.Bottom; y++)
//                    {
//                        var sRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;
//                        var pRow = (byte*)newBitmapData.Scan0 + y * newBitmapData.Stride;
//                        var offset = cord.Left * step;
//
//                        for (var x = cord.Left; x < cord.Right; x++)
//                        {
//                            pRow[offset + 3] = 255;
//                            pRow[offset + 2] = sRow[offset + 2];
//                            pRow[offset + 1] = sRow[offset + 1];
//                            pRow[offset] = sRow[offset];
//                            
//                            if (x == cord.Left || x == cord.Right - 1 || y == cord.Top || y == cord.Bottom - 1)
//                            {
//                                pRow[offset + 2] = randomColor.R;
//                                pRow[offset + 1] = randomColor.G;
//                                pRow[offset] = randomColor.B;
//                            }
//
//                            offset += step;
//                        }
//                    }
//                }
//            }

            bitmap.UnlockBits(bitmapData);
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }
        
        public static (Bitmap img, List<Cord> cords) ShowTextCord2(Bitmap bitmap, byte min = 5)
        {
            //var newBitmap = (Bitmap)bitmap.Clone();

            Image<Gray, byte> gray = new Image<Gray, byte>(bitmap);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1, 1, BorderType.Default);
            CvInvoke.Canny(gray, gray, 70, 130);

            var grayMap = gray.ToBitmap().GetGrayMap();
            var map = grayMap;//GetMapBySobel(grayMap);

            var newBitmap = (Bitmap)gray.ToBitmap().Clone();
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            
            var step = newBitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
            var cordList = new List<Cord>();
            
            for (var y = 5; y < bitmap.Height; y++)
            {
                int? leftCord = null;

                for (var x = 5; x < bitmap.Width; x++)
                {
                    var point = map[y, x];

                    if (leftCord == null && point > min)
                    {
                        leftCord = x;
                    }
                    else if (leftCord != null && point == 0)
                    {
                        var prevCords = cordList.Where(c => c.Bottom < y && c.Bottom > y - 2 && leftCord <= c.Right + 0 && x >= c.Left - 0).ToList();

                        if (!prevCords.Any())
                        {
                            cordList.Add(new Cord(y, y, leftCord.Value, x));
                        }
                        else
                        {
                            var newCord = new Cord
                            {
                                Top = prevCords.Min(c => c.Top),
                                Bottom = y,
                                Left = Math.Min(leftCord.Value, prevCords.Min(c => c.Left)),
                                Right = Math.Max(x, prevCords.Max(c => c.Right)),
                            };

                            cordList.RemoveAll(c => c.Bottom < y && c.Bottom > y - 2 && leftCord <= c.Right + 0 && x >= c.Left - 0);
                            
                            cordList.Add(newCord);
                        }
                        
                        leftCord = null;
                    }
                }
                
                if (leftCord != null)
                {
                    cordList.Add(new Cord(y, y, leftCord.Value, bitmap.Width - 1));
                }
            }

            Random rnd = new Random();
            
//            unsafe
//            {
//                foreach (var cord in cordList.Where(x => x.Bottom - x.Top > 5 && x.Right - x.Left > 5))
//                {
//                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
////                    Color randomColor = Color.Blue;
                    
//                    for (var y = cord.Top; y < cord.Bottom; y++)
//                    {
//                        var pRow = (byte*)newBitmapData.Scan0 + y * newBitmapData.Stride;
//                        var offset = cord.Left * step;

//                        for (var x = cord.Left; x < cord.Right; x++)
//                        {
//                            if (x == cord.Left || x == cord.Right - 1 || y == cord.Top || y == cord.Bottom - 1)
//                            {
//                                pRow[offset + 2] = randomColor.R;
//                                pRow[offset + 1] = randomColor.G;
//                                pRow[offset] = randomColor.B;
//                            }

//                            offset += step;
//                        }
//                    }
//                }
//            }
            
            bitmap.UnlockBits(bitmapData);
            newBitmap.UnlockBits(newBitmapData);

            return (newBitmap, cordList);
        }

        public static Bitmap ShowCord(this Bitmap bitmap, int X = 0, int Y = 0, int width = 1, int height = 1)
        {
            var newBitmap = (Bitmap)bitmap.Clone();
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            var step = newBitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            unsafe
            {
                Color randomColor = Color.Blue;

                for (var y = Y; y < Y + height; y++)
                {
                    var pRow = (byte*)newBitmapData.Scan0 + y * newBitmapData.Stride;
                    var offset = X * step;

                    for (var x = X; x < X + width; x++)
                    {
                        if (x == X || x == X + width - 1 || y == Y || y == Y + height - 1)
                        {
                            pRow[offset + 2] = randomColor.R;
                            pRow[offset + 1] = randomColor.G;
                            pRow[offset] = randomColor.B;
                        }

                        offset += step;
                    }
                }
            }

            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }

        public static Bitmap CutSymbol(this Bitmap bitmap, (int V, int H) padding, byte min = 0)
        {
            var procBitmap = (Bitmap)bitmap.Clone(new RectangleF(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
            Image<Gray, byte> gray = new Image<Gray, byte>(procBitmap);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1, 1, BorderType.Default);
            CvInvoke.Canny(gray, gray, 70, 130);

            var map = gray.ToBitmap().GetGrayMap();
            var cordList = new List<Cord>();
            
            for (var y = 1; y < bitmap.Height; y++)
            {
                int? leftCord = null;

                for (var x = 1; x < bitmap.Width; x++)
                {
                    var point = map[y, x];

                    if (leftCord == null && point > min)
                    {
                        leftCord = x;
                    }
                    else if (leftCord != null && point == 0)
                    {
                        var prevCords = cordList.Where(c => c.Bottom <= y && c.Bottom > y - 10 && leftCord <= c.Right + 10 && x >= c.Left - 10).ToList();

                        if (!prevCords.Any())
                        {
                            cordList.Add(new Cord(y, y, leftCord.Value, x));
                        }
                        else
                        {
                            var newCord = new Cord
                            {
                                Top = prevCords.Min(c => c.Top),
                                Bottom = y,
                                Left = Math.Min(leftCord.Value, prevCords.Min(c => c.Left)),
                                Right = Math.Max(x, prevCords.Max(c => c.Right)),
                            };

                            cordList.RemoveAll(c => c.Bottom <= y && c.Bottom > y - 10 && leftCord <= c.Right + 10 && x >= c.Left - 10);
                            
                            cordList.Add(newCord);
                        }
                        
                        leftCord = null;
                    }
                }
                
                if (leftCord != null)
                {
                    cordList.Add(new Cord(y, y, leftCord.Value, bitmap.Width - 1));
                }
            }

            var cord = cordList.FirstOrDefault(x => x.Bottom - x.Top > 5 && x.Right - x.Left > 5);

            if (cord == null)
            {
                return null;
            }

            Random random = new Random((int) DateTime.Now.Ticks);
            
            var H = random.Next(-padding.H, padding.H);
            var V = random.Next(-padding.V, padding.V);

            cord.Left = Math.Min(bitmap.Width, Math.Max(0, cord.Left + H));
            cord.Right = Math.Min(bitmap.Width, Math.Max(0, cord.Right + H));
            cord.Top = Math.Min(bitmap.Height, Math.Max(0, cord.Top + V));
            cord.Bottom = Math.Min(bitmap.Height, Math.Max(0, cord.Bottom + V));

            var newBitmap = new Bitmap(cord.Right - cord.Left, cord.Bottom - cord.Top, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            var sStep = bitmap.GetStep();
            var pStep = newBitmap.GetStep();
            
            unsafe
            {
                for (var y = cord.Top; y < cord.Bottom; y++)
                {
                    var sRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;
                    var pRow = (byte*)newBitmapData.Scan0 + (y - cord.Top) * newBitmapData.Stride;
                    var sOffset = cord.Left * sStep;
                    var pOffset = 0;

                    for (var x = cord.Left; x < cord.Right; x++)
                    {
                        pRow[pOffset + 3] = 255;
                        pRow[pOffset + 2] = sRow[sOffset + 2];
                        pRow[pOffset + 1] = sRow[sOffset + 1];
                        pRow[pOffset] = sRow[sOffset];

                        sOffset += sStep;
                        pOffset += pStep;
                    }
                }
            }
            
            bitmap.UnlockBits(bitmapData);
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }

        public static Bitmap Test(Bitmap bitmap)
        {
            var newBitmap = (Bitmap)bitmap.Clone();
            Image<Gray, byte> gray = new Image<Gray, byte>(newBitmap);
            var cImage = new Image<Gray, byte>(newBitmap);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1, 1, BorderType.Default);
            CvInvoke.Canny(gray, gray, 70, 130);

            //return gray.ToBitmap();

            //Image<Gray, byte> res = new Image<Gray, byte>(newBitmap);
            //var a = FindLargestContour(gray, res);
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            Random rnd = new Random();
            var step = newBitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
            //var d = a.ToArray();


            //var r = CvInvoke.MinAreaRect(a);
            //var ad =  res.Rotate(r.Angle, r.Center, Inter.Area, new Gray(), false);

            var cordList = FindContourList(gray);

            unsafe
            {
                //foreach (var cord in cordList.Where(x => x.Bottom - x.Top > 5 && x.Right - x.Left > 5))
                foreach (var cord in cordList)
                {
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    //                    Color randomColor = Color.Blue;

                    for (var y = cord.Top; y < cord.Bottom; y++)
                    {
                        var pRow = (byte*)newBitmapData.Scan0 + y * newBitmapData.Stride;
                        var offset = cord.Left * step;

                        for (var x = cord.Left; x < cord.Right; x++)
                        {
                            if (x == cord.Left || x == cord.Right - 1 || y == cord.Top || y == cord.Bottom - 1)
                            {
                                pRow[offset + 2] = randomColor.R;
                                pRow[offset + 1] = randomColor.G;
                                pRow[offset] = randomColor.B;
                            }

                            offset += step;
                        }
                    }
                }
            }

            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }

        public static List<Cord> FindContourList(IInputOutputArray cannyEdges)
        {
            List<Cord> resultCords = new List<Cord>();

            using (Mat hierachy = new Mat())
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, hierachy, RetrType.List, ChainApproxMethod.ChainApproxTc89Kcos);

                for (int i = 0; i < contours.Size; i++)
                {
                    if (CvInvoke.ContourArea(contours[i]) > 8)
                        for (int j = 0; j < contours[i].Size; j++)
                    {
                        var cArray = contours[i].ToArray();
                        var cord = new Cord
                        {
                            Left = cArray.Min(x => x.X),
                            Right = cArray.Max(x => x.X),
                            Top = cArray.Min(x => x.Y),
                            Bottom = cArray.Max(x => x.Y)
                        };

                        if (cord.Right - cord.Left > 5 && cord.Bottom - cord.Top > 5)
                            resultCords.Add(cord);
                    }
                }
            }

            return resultCords;
        }

        public static VectorOfPoint FindLargestContour(IInputOutputArray cannyEdges, IInputOutputArray result)
        {
            int largest_contour_index = 0;
            double largest_area = 0;
            VectorOfPoint largestContour;
            VectorOfVectorOfPoint r;

            using (Mat hierachy = new Mat())
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                IOutputArray hirarchy;

                CvInvoke.FindContours(cannyEdges, contours, hierachy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                //r = contours;
                for (int i = 0; i < contours.Size; i++)
                {
                    MCvScalar color = new MCvScalar(0, 0, 255);

                    double a = CvInvoke.ContourArea(contours[i], false);  //  Find the area of contour
                    if (a > largest_area)
                    {
                        largest_area = a;
                        largest_contour_index = i;                //Store the index of largest contour
                    }

                    CvInvoke.DrawContours(result, contours, largest_contour_index, new MCvScalar(255, 0, 0));
                }

                CvInvoke.DrawContours(result, contours, largest_contour_index, new MCvScalar(0, 0, 255), 3, LineType.EightConnected, hierachy);
                largestContour = new VectorOfPoint(contours[largest_contour_index].ToArray());
            }

            return largestContour;
        }

        public static Bitmap AutoRotate(Bitmap bitmap)
        {
            var newBitmap = (Bitmap)bitmap.Clone();
            Image<Gray, byte> gray = new Image<Gray, byte>(newBitmap);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1, 1, BorderType.Default);
            CvInvoke.Canny(gray, gray, 130, 150);

            var angle = SomeMethode(gray);
            var r = RotateImage(bitmap, (float)angle);

            return r;
        }
        
        private static double SomeMethode(Image<Gray, byte> gray)
        {
            int D = (int)(Math.Sqrt(gray.Width * gray.Width + gray.Height * gray.Height));
            Image<Gray, int> houghSpace = new Image<Gray, int>(181, ((int)(1.414213562 * D) * 2) + 1);
            int xpoint = 0;
            double maxT = 0;
            double[,] table = CreateTable();
            for (int xi = 0; xi < gray.Width; xi++)
            for (int yi = 0; yi < gray.Height; yi++)
            {
                if (gray[yi, xi].Intensity == 0) continue;
                for (int i = 0; i < 181; i++)
                {
                    int rho = (int)((xi * table[0, i] + yi * table[1, i])) + (houghSpace.Height / 2);
                    Gray g = new Gray(houghSpace[rho, i].Intensity + 1);
                    if (g.Intensity > maxT)
                    {
                        maxT = g.Intensity;
                        xpoint = i;
                    }
                    houghSpace [rho, i] = g;
                }
            }
            
            double thetaHotPoint = ((Math.PI / 180) * -90d) + (Math.PI / 180) * xpoint;
            return (90 - Math.Abs(thetaHotPoint) * (180 / Math.PI)) * (thetaHotPoint< 0 ? -1 : 1);
        }
        
        private static double[,] CreateTable()
        {
            double[,] table = new double[2, 181]; // 0 - cos, 1 - sin;
            double rad = (Math.PI / 180);
            double theta = rad * -90;
            for (int i = 0; i < 181; i++)
            {
                table[0, i] = Math.Cos(theta);
                table[1, i] = Math.Sin(theta);
                theta += rad;
            }
            return table;
        }

        public static Bitmap FindRows(Bitmap bitmap)
        {
            var newBitmap = (Bitmap) bitmap.Clone();
            var height = newBitmap.Height;
            var width = newBitmap.Width;
            var delimiter = width;// / 25;

            for (int part = 0; part < (width / delimiter); part++)
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
                //var rowsCordList = GetRowsCord(bitmapData, new LineCord {Top = 0, Bottom = height - 1});
                var rowsCordList = GetRowsCordSobel(bitmapData, null,null);

                var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

                unsafe
                {
                    foreach (var rowCord in rowsCordList)
                    {
                        var newRowCord = new LineCord
                        {
                            Top = rowCord.Top > 0 ? rowCord.Top - 1 : 0,
                            Bottom = rowCord.Bottom < bitmapData.Height - 1 ? rowCord.Bottom + 1 : rowCord.Bottom
                        };

                        var columnsCordList = GetColumnsCordSobel(bitmapData, null, newRowCord);
                        columnsCordList = columnsCordList.Any() ? columnsCordList : new List<LineCord> { new LineCord { Top = 0, Bottom = bitmapData.Width - 1 } };


                        foreach (var columnCord in columnsCordList)
                        {
                            var columnLeft = (byte*) newBitmapData.Scan0 + (columnCord.Top * 4);
                            var columnRight = (byte*) newBitmapData.Scan0 + (columnCord.Bottom * 4);
                            var columnOffset = rowCord.Top * bitmapData.Stride;

                            for (int y = rowCord.Top; y < rowCord.Bottom; ++y)
                            {
                                columnLeft[columnOffset] = 0;
                                columnLeft[columnOffset + 1] = 200;
                                columnLeft[columnOffset + 2] = 0;

                                columnRight[columnOffset] = 0;
                                columnRight[columnOffset + 1] = 0;
                                columnRight[columnOffset + 2] = 200;

                                columnOffset += bitmapData.Stride;
                            }
                            
                            var rowTop = (byte*) newBitmapData.Scan0 + (rowCord.Top * newBitmapData.Stride);
                            var rowBottom = (byte*) newBitmapData.Scan0 + (rowCord.Bottom * newBitmapData.Stride);
                            var rowOffset = columnCord.Top * 4;

                            for (int x = columnCord.Top; x < columnCord.Bottom; ++x)
                            {
                                rowTop[rowOffset] = 0;
                                rowTop[rowOffset + 1] = 200;
                                rowTop[rowOffset + 2] = 0;

                                rowBottom[rowOffset] = 0;
                                rowBottom[rowOffset + 1] = 0;
                                rowBottom[rowOffset + 2] = 200;

                                rowOffset += 4;
                            }
                        }
                    }
                }

                bitmap.UnlockBits(bitmapData);
                newBitmap.UnlockBits(newBitmapData);
            }

            return newBitmap;
        }
        
        public static Bitmap FindColumns(Bitmap bitmap, int delimiter = 0)
        {
            var newBitmap = (Bitmap) bitmap.Clone();
            var filterdBitmap = bitmap.Contrast(50);
            filterdBitmap = Sobel(filterdBitmap);
            var height = filterdBitmap.Height;
            var width = filterdBitmap.Width;
            
            delimiter = delimiter > 0 ? height / delimiter : height;

            for (int part = 0; part < (height / delimiter); part++)
            {
                var bitmapData = filterdBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, filterdBitmap.PixelFormat);
                var columnsBright = new float[width];

                unsafe
                {
                    for (var x = 0; x < width; ++x)
                    {
                        var pData = (byte*) bitmapData.Scan0 + x * 4;
                        var offset = delimiter * part * bitmapData.Stride;
                        var lineBright = 0;

                        for (var y = 0; y < delimiter; y++)
                        {
                            lineBright += GetBright(pData[offset + 2], pData[offset + 1], pData[offset]);
                            offset += bitmapData.Stride;
                        }

                        columnsBright[x] = (float) 1 / height * lineBright;
                    }
                }

                filterdBitmap.UnlockBits(bitmapData);

                var columnsAvrBright = 0.4f * ((float) 1 / width * columnsBright.Sum());
                var columnsCordList = GetLinesCord(columnsBright, columnsAvrBright);

                var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

                unsafe
                {
                    foreach (var columnCord in columnsCordList)
                    {
                        var columnLeft = (byte*) newBitmapData.Scan0 + (columnCord.Top * 4);
                        var columnRight = (byte*) newBitmapData.Scan0 + (columnCord.Bottom * 4);
                        var columnOffset = delimiter * part * bitmapData.Stride;

                        for (int y = 0; y < delimiter; ++y)
                        {
                            columnLeft[columnOffset] = 0;
                            columnLeft[columnOffset + 1] = 200;
                            columnLeft[columnOffset + 2] = 0;

                            columnRight[columnOffset] = 0;
                            columnRight[columnOffset + 1] = 0;
                            columnRight[columnOffset + 2] = 200;

                            columnOffset += bitmapData.Stride;
                        }
                    }
                }

                newBitmap.UnlockBits(newBitmapData);
            }

            return newBitmap;
        }
        
        public static Bitmap FindLines(Bitmap bitmap)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var linesBright = new float[height];
            
            unsafe
            {
                for (var y = 0; y < height; y++)
                {
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var columnOffset = 0;
                    var lineBright = 0;

                    for (var x = 0; x < width; ++x)
                    {
                        lineBright += GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                        columnOffset += 4;
                    }

                    linesBright[y] = (float)1 / width * lineBright;
                }
            }

            bitmap.UnlockBits(bitmapData);

            var imgAvrBright = 0.4f * ((float)1 / height * linesBright.Sum());
            var lineCordList = GetLinesCord(linesBright, imgAvrBright);
            
            var newBitmap = (Bitmap) bitmap.Clone();
            var newBitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

            unsafe
            {
                foreach (var lineCord in lineCordList)
                {
                    var rowTop = (byte*)newBitmapData.Scan0 + (lineCord.Top * newBitmapData.Stride);
                    var rowBottom = (byte*)newBitmapData.Scan0 + (lineCord.Bottom * newBitmapData.Stride);
                    int columnOffset = 0;

                    for (int x = 0; x < width; ++x)
                    {
                        rowTop[columnOffset] = 0;
                        rowTop[columnOffset + 1] = 200;
                        rowTop[columnOffset + 2] = 0;
                        
                        rowBottom[columnOffset] = 0;
                        rowBottom[columnOffset + 1] = 0;
                        rowBottom[columnOffset + 2] = 200;

                        columnOffset += 4;
                    }
                }
            }
            
            newBitmap.UnlockBits(newBitmapData);
            
            return newBitmap;
        }

        private static List<LineCord> GetRowsCord(BitmapData bitmapData, LineCord rowCord = null)
        {
            var height = bitmapData.Height;
            var width = bitmapData.Width;
            
            rowCord = rowCord ?? new LineCord {Top = 0, Bottom = height - 1};
            
            var rowsBright = new float[rowCord.Bottom - rowCord.Top];

            unsafe
            {
                for (var y = rowCord.Top; y < rowCord.Bottom; y++)
                {
                    var row = (byte*) bitmapData.Scan0 + (y * bitmapData.Stride);
                    var columnOffset = 0;
                    var lineBright = 0;

                    for (var x = 0; x < width; ++x)
                    {
                        lineBright += GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
                        columnOffset += 4;
                    }

                    rowsBright[y] = (float) 1 / width * lineBright;
                }
            }

            var rowsAvrBright = (float) 1 / height * rowsBright.Sum();
            
            return GetLinesCord(rowsBright, rowsAvrBright);
        }
        
        private static List<LineCord> GetColumnsCord(BitmapData bitmapData, LineCord columnCord = null, LineCord rowCord = null)
        {
            var height = bitmapData.Height;
            var width = bitmapData.Width;

            columnCord = columnCord ?? new LineCord {Top = 0, Bottom = width - 1};
            rowCord = rowCord ?? new LineCord {Top = 0, Bottom = height - 1};
            
            var columnsBright = new float[columnCord.Bottom - columnCord.Top];

            unsafe
            {
                for (var x = columnCord.Top; x < columnCord.Bottom; ++x)
                {
                    var pData = (byte*) bitmapData.Scan0 + x * 4;
                    var offset = rowCord.Top * bitmapData.Stride;
                    var lineBright = 0;

                    for (var y = rowCord.Top; y < rowCord.Bottom; y++)
                    {
                        lineBright += GetBright(pData[offset + 2], pData[offset + 1], pData[offset]);
                        offset += bitmapData.Stride;
                    }

                    columnsBright[x] = (float) 1 / height * lineBright;
                }
            }

            var columnsAvrBright = ((float) 1 / width * columnsBright.Sum());
            
            return GetLinesCord(columnsBright, columnsAvrBright);
        }

        private static List<LineCord> GetRowsCordSobel(BitmapData bitmapData, LineCord columnCord = null, LineCord rowCord = null)
        {
            columnCord = columnCord ?? new LineCord { Top = 0, Bottom = bitmapData.Width - 1 };
            rowCord = rowCord ?? new LineCord { Top = 0, Bottom = bitmapData.Height - 1 };

            var height = rowCord.Bottom - rowCord.Top;
            var width = columnCord.Bottom - columnCord.Top;

            var columnsBright = new float[height];

            unsafe
            {
                for (var y = rowCord.Top + 1; y < rowCord.Bottom; y++)
                {
                    var rowPrev = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var lineBright = 0;
                    int columnOffset = 0;

                    for (var x = columnCord.Top; x < columnCord.Bottom; ++x)
                    {
                        var rowPrevPix = GetBright(rowPrev[columnOffset + 2], rowPrev[columnOffset + 1], rowPrev[columnOffset], false);
                        var rowPix = GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset], false);

                        var rowDif = Math.Max(rowPix, rowPrevPix) - Math.Min(rowPix, rowPrevPix);
                        var curPos = (byte)Math.Min(255, rowDif);

                        lineBright += curPos;

                        columnOffset += 4;
                    }

                    columnsBright[y] = (float)1 / height * lineBright;
                }
            }

            var columnsAvrBright = ((float)1 / width * columnsBright.Sum());

            return GetLinesCord(columnsBright, columnsAvrBright);
        }

        private static List<LineCord> GetColumnsCordSobel(BitmapData bitmapData, LineCord columnCord = null, LineCord rowCord = null)
        {
            columnCord = columnCord ?? new LineCord {Top = 0, Bottom = bitmapData.Width - 1};
            rowCord = rowCord ?? new LineCord {Top = 0, Bottom = bitmapData.Height - 1};

            var height = rowCord.Bottom - rowCord.Top;
            var width = columnCord.Bottom - columnCord.Top;

            var columnsBright = new float[width];

            unsafe
            {
                for (var x = columnCord.Top; x < columnCord.Bottom; ++x)
                {
                    var pData = (byte*)bitmapData.Scan0 + x * 4;
                    var offset = (rowCord.Top + 1) * bitmapData.Stride;
                    var lineBright = 0;

                    for (var y = rowCord.Top + 1; y < rowCord.Bottom; y++)
                    {
                        var rowPrevPix = GetBright(pData[offset - bitmapData.Stride + 2], pData[offset - bitmapData.Stride + 1], pData[offset - bitmapData.Stride], false);
                        var rowPix = GetBright(pData[offset + 2], pData[offset + 1], pData[offset], false);

                        var rowDif = Math.Max(rowPix, rowPrevPix) - Math.Min(rowPix, rowPrevPix);
                        var curPos = (byte)Math.Min(255, rowDif);

                        lineBright += curPos;
                        offset += bitmapData.Stride;
                    }

                    columnsBright[x] = (float)1 / width * lineBright;
                }
            }

            var columnsAvrBright = ((float)1 / height * columnsBright.Sum());

            return GetLinesCord(columnsBright, columnsAvrBright);
        }

        private static int[] GetMapBySobel(BitmapData bitmapData, LineCord columnCord = null, LineCord rowCord = null)
        {
            columnCord = columnCord ?? new LineCord { Top = 0, Bottom = bitmapData.Width };
            rowCord = rowCord ?? new LineCord { Top = 0, Bottom = bitmapData.Height };

            var height = rowCord.Bottom - rowCord.Top;
            var width = columnCord.Bottom - columnCord.Top;
            var step = bitmapData.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            var map = new int[height * width];

            unsafe
            {
                for (var y = 1; y < height - 1; y++)
                {
                    var Y1 = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
                    var Y2 = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    var Y3 = (byte*)bitmapData.Scan0 + ((y + 1) * bitmapData.Stride);
                    
                    int columnOffset = step;
                    var m = new int[3, 3];
                    
                    for (int x = 1; x < width - 1; x++)
                    {
                        m[0, 0] = GetBright(Y1[columnOffset + 2 - step], Y1[columnOffset + 1 - step], Y1[columnOffset - step]);
                        m[0, 1] = GetBright(Y1[columnOffset + 2], Y1[columnOffset + 1], Y1[columnOffset]);
                        m[0, 2] = GetBright(Y1[columnOffset + 2 + step], Y1[columnOffset + 1 + step], Y1[columnOffset + step]);
                        
                        m[1, 0] = GetBright(Y2[columnOffset + 2 - step], Y2[columnOffset + 1 - step], Y2[columnOffset - step]);
                        m[1, 1] = GetBright(Y2[columnOffset + 2], Y2[columnOffset + 1], Y2[columnOffset]);
                        m[1, 2] = GetBright(Y2[columnOffset + 2 + step], Y2[columnOffset + 1 + step], Y2[columnOffset + step]);
                        
                        m[2, 0] = GetBright(Y3[columnOffset + 2 - step], Y3[columnOffset + 1 - step], Y3[columnOffset - step]);
                        m[2, 1] = GetBright(Y3[columnOffset + 2], Y3[columnOffset + 1], Y3[columnOffset]);
                        m[2, 2] = GetBright(Y3[columnOffset + 2 + step], Y3[columnOffset + 1 + step], Y3[columnOffset + step]);

                        var dx = m.Sum(GX);
                        var dy = m.Sum(GY);
                        
                        var r = Math.Sqrt(dx * dx + dy * dy);
                        var p = (byte) Math.Max(0, Math.Min(255, r));
                        
                        map[x + y * width] = p;
                        
                        columnOffset += step;
                    }
                }
                
                
//                for (var y = 1; y < height - 1; y++)
//                {
//                    var rowPrev = (byte*)bitmapData.Scan0 + ((y - 1) * bitmapData.Stride);
//                    var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
//                    int columnOffset = 0;
//                    
//                    for (int x = 0; x < width; ++x)
//                    {
//                        if (columnOffset > 3)
//                        {
//                            var rowPrevPix = GetBright(rowPrev[columnOffset + 2], rowPrev[columnOffset + 1], rowPrev[columnOffset]);
//                            var rowPix = GetBright(row[columnOffset + 2], row[columnOffset + 1], row[columnOffset]);
//                            var rowPixPrev = GetBright(row[columnOffset - 2], row[columnOffset - 3], row[columnOffset - 4]);
//                       
//                            var rowPrevDif = Math.Max(rowPix, rowPixPrev) - Math.Min(rowPix, rowPixPrev);
//                            var rowDif = Math.Max(rowPix, rowPrevPix) - Math.Min(rowPix, rowPrevPix);
//                            var curPos = Math.Min(255, (rowDif + rowPrevDif) / 2);
//
//                            map[x + y * width] = Math.Max(0, Math.Min(255, curPos));
//                            
////                            if (curPos > 0)
////                            {
////                                var curOffset = x;
////                                var curRow = y;
////                                
////                                if (rowPix < rowPixPrev)
////                                {
////                                    curOffset -= 1;
////                                }
////
////                                if (rowPix < rowPrevPix)
////                                {
////                                    map[curOffset + curRow * width] = Math.Max(0, Math.Min(255, curPos));
////                                    
////                                    curRow -= 1;
////                                }
////                                
////                                map[curOffset + curRow * width] = Math.Max(0, Math.Min(255, curPos));
////                            }
//                        }
//                        
//                        columnOffset += step;
//                    }
//                }
            }

            return map;
        }

        private static int[,] GetMapBySobel(int[,] data)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            var map = new int[height, width];

            for (var y = 1; y < height - 1; y++)
            {
                var m = new int[3, 3];

                for (int x = 1; x < width - 1; x++)
                {
                    m[0, 0] = data[y - 1, x - 1];
                    m[0, 1] = data[y - 1, x];
                    m[0, 2] = data[y - 1, x + 1];

                    m[1, 0] = data[y, x - 1];
                    m[1, 1] = data[y, x];
                    m[1, 2] = data[y, x + 1];

                    m[2, 0] = data[y + 1, x - 1];
                    m[2, 1] = data[y + 1, x];
                    m[2, 2] = data[y + 1, x + 1];

                    var dx = m.Sum(GX);
                    var dy = m.Sum(GY);

                    var r = Math.Sqrt(dx * dx + dy * dy);
                    var p = (byte) Math.Max(0, Math.Min(255, r));

                    map[y, x] = p;
                }
            }

            return map;
        }

        private static List<LineCord> GetLinesCord(IReadOnlyList<float> linesBright, float imgAvrBright)
        {
            var linesCord = new List<LineCord>();
            int? top = null;
            
            for (var i = 2; i < linesBright.Count - 3; i++)
            {
                var t =
                    !top.HasValue &&
                    (linesBright[i - 2] < imgAvrBright) &&
                    (linesBright[i - 1] < imgAvrBright) &&
                    (linesBright[i] > imgAvrBright) &&
                    (linesBright[i + 1] > imgAvrBright) &&
                    (linesBright[i + 2] > imgAvrBright) &&
                    (linesBright[i + 3] > imgAvrBright);

                top = t ? i : top;
                
                var b = 
                    top.HasValue &&
                    ((linesBright[i] > imgAvrBright) && (linesBright[i + 1] < imgAvrBright)) ||
                    ((linesBright[i + 1] < imgAvrBright) && (linesBright[i + 2] < imgAvrBright) && (linesBright[i + 3] < imgAvrBright));

                if (top.HasValue && b)
                {
                    var cord = new LineCord
                    {
//                        Top = Math.Max(0, (int) (top.Value - (top.Value * 0.3))),
//                        Bottom = Math.Min(linesBright.Count - 1, (int) (i + i * 0.3))
                        Top = top.Value,
                        Bottom = i
                    };
                    
                    linesCord.Add(cord);
                    top = null;
                }
            }
            
            return linesCord;
        }

        private static byte GetBright(byte red, byte green, byte blue, bool revert = true)
        {
            var bright = (red + green + blue) / 3;
            bright = revert ? 255 - bright : bright;
            return (byte)Math.Max(0, Math.Min(255, bright));
        }

        public static Bitmap ConvertTo1Bit(Bitmap input)
        {
            var masks = new byte[] {0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
            var data = new sbyte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                var scanLine = inputData.Scan0;
                var line = new byte[inputData.Stride];
                for (var y = 0; y < inputData.Height; y++, scanLine += inputData.Stride)
                {
                    Marshal.Copy(scanLine, line, 0, line.Length);
                    for (var x = 0; x < input.Width; x++)
                    {
                        data[x, y] =
                            (sbyte) (64 * (GetGreyLevel(line[x * 3 + 2], line[x * 3 + 1], line[x * 3 + 0]) - 0.5));
                    }
                }
            }
            finally
            {
                input.UnlockBits(inputData);
            }

            var outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format1bppIndexed);
            try
            {
                var scanLine = outputData.Scan0;
                for (var y = 0; y < outputData.Height; y++, scanLine += outputData.Stride)
                {
                    var line = new byte[outputData.Stride];
                    for (var x = 0; x < input.Width; x++)
                    {
                        var j = data[x, y] > 0;
                        if (j) line[x / 8] |= masks[x % 8];
                        var error = (sbyte) (data[x, y] - (j ? 32 : -32));
                        if (x < input.Width - 1) data[x + 1, y] += (sbyte) (7 * error / 16);
                        if (y < input.Height - 1)
                        {
                            if (x > 0) data[x - 1, y + 1] += (sbyte) (3 * error / 16);
                            data[x, y + 1] += (sbyte) (5 * error / 16);
                            if (x < input.Width - 1) data[x + 1, y + 1] += (sbyte) (1 * error / 16);
                        }
                    }

                    Marshal.Copy(line, 0, scanLine, outputData.Stride);
                }
            }
            finally
            {
                output.UnlockBits(outputData);
            }

            return output;
        }

        public static byte GetGreyLevel(byte r, byte g, byte b)
        {
            var res = (r * 0.299 + g * 0.587 + b * 0.114) / 255;
            
            return (byte)Math.Max(0, Math.Min(255, res));
        }
    }
}