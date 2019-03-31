using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScannerNet;
using Sobel.Models;

namespace Sobel
{
    public static class Utils
    {
        private static int[,] GX = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        private static int[,] GY = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        private static int[] mX = new int[9] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
        private static int[] mY = new int[9] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };

        private static ConvMatrix cX = new ConvMatrix { Height = 3, Width = 3, Size = 9, Arr = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } } };
        private static ConvMatrix cY = new ConvMatrix { Height = 3, Width = 3, Size = 9, Arr = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } } };

        public static Bitmap SobolNew(Bitmap bmp)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            int sumX, sumY;
            byte SUM = 0;

            for (int Y = 1; Y < bmp.Height - 1; Y++)
            {
                for (int X = 1; X < bmp.Width - 1; X++)
                {
                    sumX = 0;
                    sumY = 0;

                    Color pix1 = bmp.GetPixel(X - 1, Y - 1);
                    Color pix2 = bmp.GetPixel(X, Y - 1);
                    Color pix3 = bmp.GetPixel(X + 1, Y - 1);

                    Color pix4 = bmp.GetPixel(X - 1, Y);
                    Color pix5 = bmp.GetPixel(X, Y);
                    Color pix6 = bmp.GetPixel(X + 1, Y);

                    Color pix7 = bmp.GetPixel(X - 1, Y + 1);
                    Color pix8 = bmp.GetPixel(X, Y + 1);
                    Color pix9 = bmp.GetPixel(X + 1, Y + 1);

                    var b = new Bitmap(3, 3);

                    b.SetPixel(0, 0, pix1);
                    b.SetPixel(1, 0, pix2);
                    b.SetPixel(2, 0, pix3);

                    b.SetPixel(0, 1, pix4);
                    b.SetPixel(1, 1, pix5);
                    b.SetPixel(2, 1, pix6);

                    b.SetPixel(0, 2, pix7);
                    b.SetPixel(1, 2, pix8);
                    b.SetPixel(2, 2, pix9);

                    var rX = Conv3x3(b, cX);
                    var rY = Conv3x3(b, cY);

                    sumX = (rX.GetPixel(2, 0).V() + rX.GetPixel(2, 1).V() + rX.GetPixel(2, 2).V()) - (rX.GetPixel(0, 0).V() + rX.GetPixel(0, 1).V() + rX.GetPixel(0, 2).V());
                    sumY = (rY.GetPixel(0, 2).V() + rY.GetPixel(1, 2).V() + rY.GetPixel(2, 2).V()) - (rY.GetPixel(0, 0).V() + rY.GetPixel(1, 0).V() + rY.GetPixel(2, 0).V());

                    //var gx = (pix7.V() * mX[6] + pix7.V() * mX[7] + pix7.V() * mX[8]) - (pix1.V() * mX[0] + pix2.V() * mX[1] + pix3.V() * mX[2]);
                    //var gy = (pix3.V() * mX[2] + pix6.V() * mX[5] + pix9.V() * mX[8]) - (pix1.V() * mX[0] + pix4.V() * mX[3] + pix7.V() * mX[6]);

                    SUM = (byte)Math.Sqrt((sumX * sumX + sumY * sumY));

                    if (SUM > 255) SUM = 255;
                    if (SUM < 0) SUM = 0;

                    byte newPixel = (byte)(255 - SUM);

                    Color newPixCol = Color.FromArgb(newPixel, newPixel, newPixel);
                    newBmp.SetPixel(X, Y, newPixCol);
                }
            }

            return newBmp;
        }

        public static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        {
            if (0 == m.Factor)
                return b;

            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                               ImageLockMode.ReadWrite,
                               PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;
                int nOffset = stride - b.Width * m.Width;
                int nWidth = b.Width - (m.Size - 1);
                int nHeight = b.Height - (m.Size - 2);

                int nPixel = 0;

                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        for (int r = 0; r < m.Height; r++)
                        {
                            nPixel = 0;
                            for (int i = 0; i < m.Width; i++)
                                for (int j = 0; j < m.Height; j++)
                                {
                                    nPixel += (pSrc[(m.Width * (i + 1)) - 1 - r + stride * j] * m.Arr[j, i]);
                                }

                            nPixel /= m.Factor;
                            nPixel += m.Offset;

                            if (nPixel < 0) nPixel = 0;
                            if (nPixel > 255) nPixel = 255;
                            p[(m.Width * (m.Height / 2 + 1)) - 1 - r + stride * (m.Height / 2)] = (byte)nPixel;
                        }
                        p += m.Width;
                        pSrc += m.Width;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return b;
        }

        public static Bitmap ShowDifferent(Image image, int yPos, int xPos)
        {
            var bmp = new Bitmap(image);

            var gY = ShowDifferentVert(bmp, yPos);
            //var gX = ShowDifferentHor(bmp, xPos);

            gY.Flush();
            //gX.Flush();

            gY.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            //gX.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);

            return bmp;
        }

        public static Bitmap TestSearch(Bitmap bmp)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            
            var resultX = new List<List<PixelBright>>();
            var resultY = new List<List<PixelBright>>();

//            for (int X = 0; X < bmp.Width; X++)
//            {
//                var pr = new List<PixelBright>();
//
//                for (var Y = 1; Y < bmp.Height - 1; Y++)
//                {
//                    Color pix1 = bmp.GetPixel(X, Y - 1);
//                    Color pix3 = bmp.GetPixel(X, Y + 1);
//
//                    var dif = Math.Abs((pix1.V() - pix3.V()));
//                    var curPos = Math.Max(0, dif / 2);
//
//                    //result[X, Y] = curPos;
//                    pr.Add(new PixelBright(X, Y, curPos));
////                    newBmp.SetPixel(X, Y, Color.FromArgb(curPos, curPos, curPos));
//                }
//
//                resultY.Add(pr);
//            }

//            for (int Y = 0; Y < bmp.Height; Y++)
//            {
//                var pr = new List<PixelBright>();
//
//                for (var X = 1; X < bmp.Width - 1; X++)
//                {
//                    Color pix1 = bmp.GetPixel(X - 1, Y);
//                    Color pix3 = bmp.GetPixel(X + 1, Y);
//
//                    var dif = Math.Abs((pix1.V() - pix3.V()));
//                    var curPos = Math.Max(0, dif / 2);
//
//                    //result[X, Y] = (result[X, Y] + curPos) / 2;
//                    pr.Add(new PixelBright(X, Y, curPos));
//                    //newBmp.SetPixel(X, Y, Color.FromArgb(curPos, curPos, curPos));
//                }
//
//                resultX.Add(pr);
//            }
            
            for (int Y = 1; Y < bmp.Height - 1; Y++)
            {
                var pr = new List<PixelBright>();

                for (var X = 1; X < bmp.Width - 1; X++)
                {
                    Color pixX1 = bmp.GetPixel(X - 1, Y);
                    Color pixX3 = bmp.GetPixel(X, Y);
                    
                    Color pixY1 = bmp.GetPixel(X, Y - 1);
                    Color pixY3 = bmp.GetPixel(X, Y);

                    var difX = Math.Max(pixX1.V(), pixX3.V()) - Math.Min(pixX1.V(), pixX3.V());// Math.Abs((pixX1.V() - pixX3.V()));
//                    var difY = Math.Max(pixY1.V(), pixY3.V()) - Math.Min(pixY1.V(), pixY3.V());
                    
                    var curPos = Math.Min(255, (difX) / 2);

                    pr.Add(new PixelBright(X, Y, curPos));
                    newBmp.SetPixel(X, Y, Color.FromArgb(curPos, curPos, curPos));
                }

                resultX.Add(pr);
            }

//            var sumListX = resultX.Select(t => t.Sum(x => x.Bright)).ToList();
//            var averSumX = (sumListX.Max() - sumListX.Min()) / sumListX.Count;
////
//            var sumListY = resultY.Select(t => t.Sum(x => x.Bright)).ToList();
//            var averSumY = (sumListY.Max() - sumListY.Min()) / sumListY.Count;
//
//            for (var y = 0; y < sumListX.Count; y++)
//            {
//                if (sumListX[y] > averSumX)
//                    for (int x = 0; x < bmp.Width; x++)
//                    {
//                        if (sumListY[x] > averSumY)
//                            newBmp.SetPixel(x, y, Color.FromArgb(30, bmp.GetPixel(x, y)));
//                    }
//            }


//            for (var x = 0; x < sumListY.Count; x++)
//            {
//                if (sumListY[x] > averSumY)
//                    for (int y = 0; y < bmp.Height; y++)
//                    {
//                        newBmp.SetPixel(x, y, Color.FromArgb(30, bmp.GetPixel(x, y)));
//                    }
//            }
            
//            for (var y = 0; y < sumListX.Count; y++)
//            {
//                if (sumListX[y] > averSumX)
//                    for (int x = 0; x < bmp.Width; x++)
//                    {
//                        newBmp.SetPixel(x, y, Color.FromArgb(30, bmp.GetPixel(x, y)));
//                    }
//            }

//            var l = GetLines(resultX);
//
//            for (var y = 0; y < l.Count; y++)
//            {
//                foreach (var line in l[y])
//                {
//                    var lineLenght = line.To - line.From;
//                    
//                    for (int i = 0; i < lineLenght; i++)
//                    {
//                        var x = line.From + i;
//                        newBmp.SetPixel(x, y, Color.Black);
//                    }
//                }
//            }

            return newBmp;
        }
        
        private static List<List<LinePosition>> GetLines(List<List<PixelBright>> input)
        {
            const int border = 1;
            var result = new List<List<LinePosition>>();
            
            //var sumBrightList = input.Select(t => t.Sum(x => x.Bright)).ToList();
            var averBright = 5;// (sumBrightList.Max() - sumBrightList.Min()) / sumBrightList.Count;
            
            foreach (var brightLine in input)
            {
                int? from = null;

                var line = new List<LinePosition>();
                
                for (var i = 0; i < brightLine.Count; i++)
                {
                    if (!from.HasValue && brightLine[i].Bright > averBright && i >= border && !brightLine.Skip(i - border).Take(border).Any(p => p.Bright > averBright))
                    {
                        from = brightLine[i].X;
                    }

                    if (from.HasValue && (i >= brightLine.Count - border ||
                                          brightLine.Skip(i).Take(border).All(b => b.Bright < averBright)))
                    {
                        line.Add(new LinePosition { From = from.Value, To = brightLine[i].X });
                        from = null;
                    }
                }
                
                result.Add(line);
            }
            
            return result;
        }

        public static Bitmap DrawString(this Bitmap mapBitmap, string text, float fontSize = 100, double rotate = 0, Random random = null)
        {
            Graphics g = Graphics.FromImage(mapBitmap);
//            var rndColorValue = random.Next(100, 255);
//            Color randomColor = Color.FromArgb(rndColorValue, rndColorValue, rndColorValue);
//            g.FillRectangle((Brush) new SolidBrush(randomColor), 0, 0, mapBitmap.Width, mapBitmap.Height);
            g.FillRectangle(Brushes.White, 0, 0, mapBitmap.Width, mapBitmap.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            var family = new List<string>{ "Calibri", "Arial", "Times New Roman" };
//            var family = new List<string>{ "Times New Roman" };
            var fontFamily = family[random.Next(family.Count)];
            
            var font = new Font(fontFamily, fontSize);

            if (random.Next(2) == 1)
            {
                font = new Font(fontFamily, fontSize, FontStyle.Bold);   
            }

            TextRenderer.DrawText(g, text, font, new Point(20, 20), Color.Black);

            g.Flush();

            if (rotate != 0)
            {
                var angle = random.NextDouble() * (-rotate - rotate) + rotate;
                PointF offset = new PointF((float)mapBitmap.Width / 2, (float)mapBitmap.Height / 2);
                g.TranslateTransform(offset.X, offset.Y);
                g.RotateTransform((float)angle);
                g.TranslateTransform(-offset.X, -offset.Y);
                g.DrawImage(mapBitmap, new Point(0, 0));
            }

            return mapBitmap;
        }

        public static Bitmap ResizeImage(this Bitmap source, RectangleF destinationBounds)
        {
            if (source == null)
            {
                return null;
            }
            RectangleF sourceBounds = new RectangleF(0.0f, 0.0f, (float)source.Width, (float)source.Height);

            Bitmap destinationImage = new Bitmap((int)destinationBounds.Width, (int)destinationBounds.Height);
            Graphics graph = Graphics.FromImage(destinationImage);
            graph.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Fill with background color
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), destinationBounds);

            float resizeRatio, sourceRatio;
            float scaleWidth, scaleHeight;

            sourceRatio = (float)source.Width / (float)source.Height;

            if (sourceRatio >= 1.0f)
            {
                //landscape
                resizeRatio = destinationBounds.Width / sourceBounds.Width;
                scaleWidth = destinationBounds.Width;
                scaleHeight = sourceBounds.Height * resizeRatio;
                float trimValue = destinationBounds.Height - scaleHeight;
                graph.DrawImage(source, 0, (trimValue / 2), destinationBounds.Width, scaleHeight);
            }
            else
            {
                //portrait
                resizeRatio = destinationBounds.Height / sourceBounds.Height;
                scaleWidth = sourceBounds.Width * resizeRatio;
                scaleHeight = destinationBounds.Height;
                float trimValue = destinationBounds.Width - scaleWidth;
                graph.DrawImage(source, (trimValue / 2), 0, scaleWidth, destinationBounds.Height);
            }

            return destinationImage;

        }
        
        public static Bitmap ResizeImage(this Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static double[] ToDoubles(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            
            var result = new double[bitmap.Height * bitmap.Width];
            
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var i = y * bitmap.Width + x;
                    result[i] = bitmap.GetPixel(x, y).V();
                }
            }

            return result;
        }

        public static float[] ToFloat(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            var result = new float[bitmap.Height * bitmap.Width];

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var i = y * bitmap.Width + x;
                    result[i] = bitmap.GetPixel(x, y).V();
                }
            }

            return result;
        }

        public static Bitmap ToBitmap(this double[] vector, int windth, int height)
        {
            if (vector.Length != windth * height)
            {
                throw new Exception($"vector lenght mast be {windth * height}");
            }
            
            var bitmap = new Bitmap(windth, height);
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < windth; x++)
                {
                    var i = y * windth + x;
                    var color = ToColor(vector[i]);
                    
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static (string symble, int position) RandomSymble(this Random random, string chars = null)
        {
            //            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //            var chars = " ЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯСМИТЬБЮ0123456789ёйцукенгшщзхъфывапролджэячсмитьбю/.,\"";
            chars = string.IsNullOrEmpty(chars) ? "0123456789ЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯСМИТЬБЮёйцукенгшщзхъфывапролджэячсмитьбю" : chars;
            //            var chars = "0123456789 ёйцукенгшщзхъфывапролджэячсмитьбю/.,\"";
            //            var chars = "аоеиы";
            var number = random.Next(chars.Length);
            var finalString = new String(new[] { chars[number] });

            return (finalString, number);
        }

        private static Graphics ShowDifferentVert(Bitmap bmp, int pixelPositiov)
        {
            Pen pen = new Pen(Color.DarkRed, 0.5f);
            Graphics g = Graphics.FromImage(bmp);

            int X = Math.Min(bmp.Width - 1, pixelPositiov);

            List<Point> p = new List<Point>();
            List<int> vv = new List<int>();

            for (var Y = 1; Y < bmp.Height - 1; Y++)
            {
                Color pix1 = bmp.GetPixel(X, Y - 1);
                Color pix2 = bmp.GetPixel(X, Y);
                Color pix3 = bmp.GetPixel(X, Y + 1);

                var dif = Math.Abs((pix1.V() - pix3.V()));
                var curPos = Math.Max(0, X + dif);

                vv.Add(curPos);
            }

            var aver = Math.Abs(vv.Sum() / vv.Count());
            var averDiff = Math.Abs(X - aver);

            for (int y = 0; y < vv.Count; y++)
            {
                bmp.SetPixel(aver - averDiff, y, Color.Blue);
                p.Add(new Point(vv[y] - averDiff, y));
            }

            g.DrawCurve(pen, p.ToArray());

            return g;
        }

        private static Graphics ShowDifferentHor(Bitmap bmp, int pixelPositiov)
        {
            Pen pen = new Pen(Color.Green, 0.5f);
            Graphics g = Graphics.FromImage(bmp);

            int Y = Math.Min(bmp.Height - 1, pixelPositiov);

            List<Point> p = new List<Point>();
            List<int> vv = new List<int>();

            for (var X = 1; X < bmp.Width - 1; X++)
            {
                Color pix1 = bmp.GetPixel(X - 1, Y);
                Color pix3 = bmp.GetPixel(X + 1, Y);

                var dif = Math.Abs((pix1.V() - pix3.V()));
                var curPos = Math.Max(0, Y + dif);

                vv.Add(curPos);
            }

            var aver = Math.Abs(vv.Sum() / vv.Count());
            var averDiff = Math.Abs(Y - aver);

            int x = 1;
            foreach (var pos in vv)
            {
                //bmp.SetPixel(x, aver - averDiff, Color.Blue);
                p.Add(new Point(x, pos - averDiff));

                x++;
            }

            g.DrawCurve(pen, p.ToArray());

            return g;
        }

        private static int V(this Color color)
        {
            int R = color.R;
            int G = color.G;
            int B = color.B;

            return (R + G + B) / 3;
        }
        
        private static Color ToColor(double value)
        {
            int R = Convert.ToByte(value);

            return Color.FromArgb(R, R, R);
        }






        public static Bitmap Sobol(Bitmap bmp)
        {
            int sumX, sumY;
            byte SUM = 0;

            for (int Y = 0; Y < bmp.Height; Y++)
            {
                for (int X = 0; X < bmp.Width; X++)
                {
                    sumX = 0;
                    sumY = 0;
                    if (Y == 0 || Y == bmp.Height - 1)
                        SUM = 0;
                    else if (X == 0 || X == bmp.Width - 1)
                        SUM = 0;
                    else
                    {
                        for (int I = -1; I <= 1; I++)
                        {
                            for (int J = -1; J <= 1; J++)
                            {
                                int piX = J + X;
                                int piY = I + Y;

                                Color pixVal = bmp.GetPixel(piX, piY);

                                int R = pixVal.R;
                                int G = pixVal.G;
                                int B = pixVal.B;

                                int NC = (R + G + B) / 3;

                                sumX = sumX + (NC) * GX[J + 1, I + 1];
                                sumY = sumY + (NC) * GY[J + 1, I + 1];
                            }
                        }

                        SUM = (byte)(Math.Abs(sumX) + Math.Abs(sumY));
                    }
                    if (SUM > 255) SUM = 255;
                    if (SUM < 0) SUM = 0;
                    byte newPixel = (byte)(255 - SUM);


                    Color newPixCol = Color.FromArgb(newPixel, newPixel, newPixel);
                    bmp.SetPixel(X, Y, newPixCol);
                }
            }

            return bmp;
        }
    }

    public class ConvMatrix
    {
        public int Factor = 1;
        public int Height, Width;
        public int Offset = 0;
        public int Size;
        public int[,] Arr;
    }
}
