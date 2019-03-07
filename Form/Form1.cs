using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mono.Cecil.Cil;
using Neuro.Models;
using Neuro.Networks;
using ScannerNet;
using ScannerNet.Extensions;
using ScannerNet.Models;
using Sobel.UI;

namespace Sobel
{
    public partial class Form1 : Form
    {
        public Network Network = new Network();
        private int ImageWidth = 16;
        private int ImageHeight = 16;
        private string lastImgPath = null;
        private (int x, int y) pictureSize = (16, 16);
        private Bitmap workImage;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void loadImgButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    lastImgPath = open.FileName;
                    Bitmap bit = new Bitmap(open.FileName);
                    workImage = bit;

                    mainPicturePanel.AutoScrollPosition = new Point(0, 0);
                    pictureBox1.Location = new Point(3, 3);
                    pictureBox1.SizeMode = bit.Width < pictureBox1.Width && bit.Height < pictureBox1.Height ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.AutoSize;
                    pictureBox1.Dock = bit.Width < pictureBox1.Width && bit.Height < pictureBox1.Height ? DockStyle.Fill : DockStyle.None;
                    //mainPicturePanel.AutoScroll = bit.Width >= pictureBox1.Width && bit.Height >= pictureBox1.Height;
                    pictureBox1.Image = bit;
                    Y = 0;
                    X = 0;
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed loading image");
            }
        }

        private void sobelFilterButton_Click(object sender, EventArgs e)
        {
            if (averageNum.Value == 0)
            {
                averageNum.Value = (decimal)workImage.GetAverBright();
            }

            var kernel = (int)findMinNumeric.Value;
            var averege = (double)averageNum.Value;
            workImage = ((Bitmap) workImage.Clone()).ToBlackWite(averege);
//            workImage = ((Bitmap) workImage.Clone()).To1bpp(kernel, averege);
            pictureBox1.Image = workImage;
        }

        private void findTextButton_Click(object sender, EventArgs e)
        {
            byte difMin = (byte)findMinNumeric.Value;
//            var bitmap = workImage.ScaleImage(1000, 1000);
            /*var cords = Segmentation.FindQRCornersCords(new Bitmap(workImage));
            pictureBox1.Image = cords.DrawCords(workImage, Color.Red);
            
            cords = Segmentation.FindQRCornersCordsY(new Bitmap(workImage));
            pictureBox1.Image = cords.DrawCords(new Bitmap(pictureBox1.Image), Color.Blue);*/
            

            var result = Segmentation.ShowTextCord2(new Bitmap(workImage), difMin, difMin);
            workImage = result.img;
            pictureBox1.Image = result.cords.DrawCords(workImage); //Utils.TestSearch(new Bitmap(pictureBox1.BackgroundImage));
            cords = result.cords;
        }

        private void prevVertPos_Click(object sender, EventArgs e)
        {
            Y--;
            var result = Segmentation.ShowTextCordDebug(new Bitmap(workImage), Y);
//            workImage = result.img;
            pictureBox1.Image = result.img; //Utils.TestSearch(new Bitmap(pictureBox1.BackgroundImage));
            cords = result.cords;
            
            
//            vertPos.Value = vertPos.Value - 1;
//            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.Image, (int)vertPos.Value, (int)horPosition.Value);
        }

        public Bitmap PrevHorPosImage = null;

        private int Y = 0;
        private int X = 0;
        
        private void nextVertPos_Click(object sender, EventArgs e)
        {
//            Y++;
            var result = Segmentation.ShowTextCordDebug(new Bitmap(workImage), Y, (byte)averageNum.Value);
//            workImage = result.img;
            pictureBox1.Image = result.img; //Utils.TestSearch(new Bitmap(pictureBox1.BackgroundImage));
            cords = result.cords;
//            
//            vertPos.Value = vertPos.Value + 1;
//            PrevHorPosImage = PrevHorPosImage ?? new Bitmap(pictureBox1.Image);
//            pictureBox1.Image = Utils.ShowDifferent(PrevHorPosImage, (int)vertPos.Value, (int)horPosition.Value);
        }

        private void nextHorPos_Click(object sender, EventArgs e)
        {
            X++;
            var result = Segmentation.ShowTextCordHDebug(new Bitmap(workImage), X);
            pictureBox1.Image = result.img;
            cords = result.cords;
            
            //pictureBox1.Image = Utils.TestSearch(new Bitmap(pictureBox1.Image));
//            horPosition.Value = horPosition.Value + 1;
//            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.BackgroundImage, (int)vertPos.Value, (int)horPosition.Value);
        }

        private void prevHorPos_Click(object sender, EventArgs e)
        {
            X--;
            var result = Segmentation.ShowTextCordHDebug(new Bitmap(workImage), X);
            pictureBox1.Image = result.img;
            cords = result.cords;
            
//            horPosition.Value = horPosition.Value - 1;
//            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.Image, (int)vertPos.Value, (int)horPosition.Value);
        }

        private void applyContrast_Click(object sender, EventArgs e)
        {
            workImage = new Bitmap(workImage).Contrast((int)contrastValue.Value);
            pictureBox1.Image = workImage;
        }

        private void SearchSolutionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
//            UpdateNwParams();
//
//            NWStartButton.Enabled = false;
//
//            var vectors = new double[_som.Iterations][];
//
//            for (var i = 0; i < _som.Iterations; i++)
//            {
//                var bmp = new Bitmap(neouronetPictureBox.Width, neouronetPictureBox.Height);
//                var text = _random.RandomSymble();
//                var bitmap = bmp.DrawString(text, (int)nwFontSizeNumeric.Value, fontFamilyTextBox.Text).ResizeImage(new RectangleF(0, 0, (double)ImageWidth, (double)ImageHeight));
//                var vector = bitmap.ToDoubles();
//
//                vectors[i] = vector;
//            }
//
//            _som.SearchSolution(vectors);
//
//            Monitor.Enter(this);
//            CurrenIterationTextBox.Text = _som.Iteration.ToString();
//            //            nwSearchLog.AppendText(text);
//            Monitor.Exit(this);
//
//            NWStartButton.Enabled = true;
        }

        private void GetAvrBrightButton_Click(object sender, EventArgs e)
        {
            averageNum.Value = (decimal)workImage.GetAverBright();
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Segmentation.RotateImage(new Bitmap(lastImgPath), (float)PictureAngleNumeric.Value);
        }

        private void reloadImgButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lastImgPath))
            {
                return;
            }

            Bitmap bit = new Bitmap(lastImgPath);
            pictureBox1.Image = bit;
            workImage = bit;
        }

        private void sobelFilter2_Click(object sender, EventArgs e)
        {
//            var c = new Canny(new Bitmap(lastImgPath), 80F, 35F, 5, 1);
//            pictureBox1.Image = c.DisplayImage(c.EdgeMap);
//            pictureBox1.BackgroundImage = new Bitmap(pictureBox1.BackgroundImage).GetGrayMap().ToBitmap();
            
//            var result = Segmentation.Test(new Bitmap(workImage));
//            workImage = result.img;
//            cords = result.cords;
//            pictureBox1.Image = workImage;
            
            var kernel = (int)findMinNumeric.Value;
            var averege = (double)averageNum.Value;
//            workImage = ((Bitmap) workImage.Clone()).ToBlackWite(averege);
            workImage = ((Bitmap) workImage.Clone()).To1bpp2(kernel, averege);
            pictureBox1.Image = workImage;
        }

        private void cannyApplyButton_Click(object sender, EventArgs e)
        {
            //var c = new Canny(new Bitmap(pictureBox1.Image), (double)cannyThNumeric.Value, (double)cannyTlNumeric.Value, (int)kernelNumeric.Value, (int)sigmaNumeric.Value);
            //pictureBox1.Image = c.DisplayImage(c.EdgeMap);
            workImage = new Bitmap(workImage).Canny((double)cannyThNumeric.Value, (double)cannyTlNumeric.Value, (int)kernelNumeric.Value);
            pictureBox1.Image = workImage;
        }

        private void gaussianFilterButton_Click(object sender, EventArgs e)
        {
            workImage = new Bitmap(pictureBox1.Image).GetGrayMap()
                .GaussianFilter((int) kernelNumeric.Value, (int) sigmaNumeric.Value).ToBitmap();

            pictureBox1.Image = workImage;
        }

        private void autoRotateButton_Click(object sender, EventArgs e)
        {
            workImage = Segmentation.AutoRotate(new Bitmap(pictureBox1.Image));
            pictureBox1.Image = workImage;
        }

        private void grayFilterButton_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            pictureBox1.Image = new Bitmap(pictureBox1.Image).GetGrayMap().ToBitmap();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
            sw.Stop();
        }

        private void BackPropoginationOpenButton_Click(object sender, EventArgs e)
        {
            var backPropoginationForm = new BackPropoginationForm();
            backPropoginationForm.Show();
        }

        private List<Cord> cords = new List<Cord>();

        private void recognizeButton_Click(object sender, EventArgs e)
        {
            //searchText();
            ShowResult2();
            return;

            panel2.Controls.Clear();
            var i = 0;

            cords = cords.Where(x => (x.Right - x.Left > 6) && (x.Right - x.Left < 100)).OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
            var results = new (Bitmap img, Cord cord, double answer)[cords.Count];
            Exception error = null;

            var imageMap = workImage.GetDoubleMatrix(1);

            Parallel.For(0, cords.Count, (int c) =>
            //for (var c = 0; c < cords.Count; c++)
            {
                try
                {
                    if (cords[c].Right - cords[c].Left > 6 && cords[c].Bottom - cords[c].Top > 6)
                    {
                        var width = cords[c].Right - cords[c].Left + 4;
                        var height = cords[c].Bottom - cords[c].Top + 4;
                        var mapPart = imageMap.GetMapPart(cords[c].Left - 2, cords[c].Top - 2, width, height);
                        var bitmap = mapPart.ToBitmap().ScaleImage(pictureSize.x, pictureSize.y);
                        var netResult = Network.Compute(bitmap.GetDoubleMatrix());

                        results[c] = (bitmap, cords[c], netResult[0]);
                    }
                }
                catch (Exception exception)
                {
                    error = exception;
                }
            });

            if (error != null)
                MessageBox.Show($"{error.Message}\n{error.InnerException}\n{error.StackTrace}");

            var viewedCords = results.Where(x => x.answer > 0).ToList();
            var picture = new Bitmap(pictureBox1.Image);

            pictureBox1.Image = viewedCords.Select(x => x.cord).ToList().DrawCords(picture);
            
            foreach (var cord in viewedCords)
            {
                try
                {
                    var box = new PictureBox
                    {
                        Location = new System.Drawing.Point(10, 4 + (i * (pictureSize.y + 3))),
                        Name = $"pictureBoxResult{i}",
                        Size = new System.Drawing.Size(pictureSize.x, pictureSize.y),
                        BackColor = Color.Black,
                        Image = cord.img,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var label = new Label
                    {
                        Location = new System.Drawing.Point(10 + pictureSize.x, 8 + (i * (pictureSize.y + 3))),
                        Name = $"labelResult{i}",
                        Size = new System.Drawing.Size(120, pictureSize.y),
                        Text = cord.answer.ToString(),
                        ForeColor = cord.answer > 0.5 ? Color.DarkGreen : Color.Black
                    };

                    panel2.Controls.Add(box);
                    panel2.Controls.Add(label);
                }
                catch
                {
                    continue;
                }
                
                i++;
            }
        }

        public byte[,] CreateImageThumbnail(byte[,] input, int width = 50, int height = 50)
        {
            var image = ToLinearArray(input);
            
            using (var stream = new System.IO.MemoryStream(image))
            {
                var img = Image.FromStream(stream);
                var thumbnail = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

                using (var thumbStream = new System.IO.MemoryStream())
                {
                    thumbnail.Save(thumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    var result = new byte[height,width];
                    var byteArray = thumbStream.GetBuffer();
                    
                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            result[y, x] = byteArray[y * width + x];
                        }
                    }
                    
                    return result;
                }
            }
        }  
        
        public static T[] ToLinearArray<T>(T[,] outputs) where T : struct
        {
            var imageHeight = outputs.GetLength(0);
            var imageWidth = outputs.GetLength(1);
            var result = new T[imageHeight * imageWidth];

            for (var h = 0; h < imageHeight; h++)
            {
                for (var w = 0; w < imageWidth; w++)
                {
                    result[h * imageWidth + w] = outputs[h, w];
                }
            }

            return result;
        }
        
        private void loadNetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lastNetsPath))
            {
                MessageBox.Show("Path is empty");
                return;
            }

            var filesPaths = Directory.GetFiles(lastNetsPath, "*.nw");

            foreach (var path in filesPaths)
            {
                var setting = File.ReadAllBytes(path);
                var symbleMatch = Regex.Match(path, @"\\_?(?<symble>\w+)\.nw");
                var symble = symbleMatch.Groups["symble"].Value;

                if (symble.Equals("point"))
                {
                    symble = ".";
                }

                var net = new Network();

                net.Load(setting);
                networks.Add((symble, net));
            }
        }

        private void vertPos_ValueChanged(object sender, EventArgs e)
        {
            Y = (int) vertPos.Value;
        }

        private void ShowResult()
        {
            cords = cords.Where(x => (x.Right - x.Left > 6) && (x.Right - x.Left < 100)).OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
            var imageMap = workImage.GetDoubleMatrix(invert: false);

            var matrixes = cords
                .Where(c => c.Right - c.Left > 6 && c.Bottom - c.Top > 6)
                .Select(cord => new MatrixWithCords
                {
                    cord = cord,
                    matrix = imageMap
                        .GetMapPart(cord.Left - 2, cord.Top - 2, cord.Right - cord.Left + 4, cord.Bottom - cord.Top + 4)
                        .ToBitmap()
                        .ScaleImage(pictureSize.x, pictureSize.y)
                        .GetDoubleMatrix()
                })
                .ToList();

            var res = matrixes
                .AsParallel()
                .Select(m => new
                {
                    m.cord,
                    result = networks
                        //.AsParallel()
                        .Select(n => new Result {answer = n.Value.Compute(m.matrix)[0], result = n.Key})
                        .OrderByDescending(x => x.answer).First()
                })
                .ToArray()
                .Select(m => new Result
                {
                    cord = m.cord,
                    result = m.result.result,
                    answer = m.result.answer
                })
                .ToList();

            var resultBitmap = new Bitmap(workImage.Width, workImage.Height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(resultBitmap);
            g.FillRectangle(Brushes.White, 0, 0, resultBitmap.Width, resultBitmap.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Flush();
            
            foreach (var result in res.Where(x => x.answer > 0.9d))
            {
                DrawSymbol(resultBitmap, result.cord, result.result, null);
            }

            pictureBox1.Image = resultBitmap;

            //var k = 0;
            //foreach (var cord in results)
            //{
            //    try
            //    {
            //        var box = new PictureBox
            //        {
            //            Location = new System.Drawing.Point(10, 4 + (k * (pictureSize.y + 3))),
            //            Name = $"pictureBoxResult{k}",
            //            Size = new System.Drawing.Size(pictureSize.x, pictureSize.y),
            //            BackColor = Color.Black,
            //            Image = cord.bitmap,
            //            BorderStyle = BorderStyle.FixedSingle
            //        };

            //        var label = new Label
            //        {
            //            Location = new System.Drawing.Point(10 + pictureSize.x, 8 + (k * (pictureSize.y + 3))),
            //            Name = $"labelResult{k}",
            //            Size = new System.Drawing.Size(120, pictureSize.y),
            //            Text = cord.answerValue.ToString(),
            //            ForeColor = cord.answerValue > 0.5 ? Color.DarkGreen : Color.Black
            //        };

            //        panel2.Controls.Add(box);
            //        panel2.Controls.Add(label);
            //    }
            //    catch
            //    {
            //        continue;
            //    }

            //    k++;
            //}
        }
        
        private void ShowResult2()
        {
            cords = cords.Where(x => (x.Right - x.Left > 6) && (x.Right - x.Left < 100)).OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
            var imageMap = workImage.GetDoubleMatrix(invert: false, optimize: false);

            var matrixes = cords
                .Where(c => c.Right - c.Left > 6 && c.Bottom - c.Top > 6)
                .Select(cord => new MatrixWithCords
                {
                    cord = cord,
                    matrix = imageMap
                        .GetMapPart(cord.Left - 2, cord.Top - 2, cord.Right - cord.Left + 4, cord.Bottom - cord.Top + 4)
                        .ToBitmap()
                        .ScaleImage(pictureSize.x, pictureSize.y)
//                        .ToBlackWite()
                        .GetDoubleMatrix()
                })
                .ToList();

            var chars = "ёйцукенгшщзхъфывапролджэячсмитьбюЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯСМИТЬБЮ0123456789";
//            var chars = "ёйцукенгшщзхъфывапролджэячсмитьбю";
//            var chars = "0123456789";
            var network = networks.FirstOrDefault();
            var matrixesCount = matrixes.Count;
            var res = new List<Result>();

            Parallel.For(0, matrixesCount, i =>
            {
                var m = matrixes[i];
                var computed = network.Value.Compute(m.matrix);
                var maxResult = GetMaxNeuron(computed);
                
                res.Add(new Result
                {
                    cord = m.cord,
                    result = chars[maxResult.index].ToString(),
                    answer = maxResult.value
                });
            });

            var resultBitmap = new Bitmap(workImage.Width, workImage.Height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(resultBitmap);
            g.FillRectangle(Brushes.White, 0, 0, resultBitmap.Width, resultBitmap.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Flush();
            
            foreach (var result in res.Where(x => x.answer > 0.1d))
            {
                var color = result.answer > 0.85d 
                    ? Color.Green 
                    : result.answer > 0.65d
                        ? Color.Yellow
                        : result.answer > 0.4d
                            ? Color.Red
                            : Color.SaddleBrown;
                
                DrawSymbol(resultBitmap, result.cord, result.result, null, color);
            }

            pictureBox1.Image = resultBitmap;
        }

        private (int index, double value) GetMaxNeuron(double[] neurons)
        {
            return neurons.Select((r, i) => (i, r)).OrderByDescending(x => x.r).First();
        }
        
        private void DrawSymbol(Bitmap mapBitmap, Cord cord, string symbol, Bitmap bitmap, Color? color = null)
        {
            color = color ?? Color.Black;
            
            using (var g = Graphics.FromImage(mapBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                g.DrawImage(bitmap, new Point(cord.Left, cord.Top));
                TextRenderer.DrawText(g, symbol, new Font("Calibri", cord.Bottom - cord.Top), new Point(cord.Left, cord.Top), color.Value);
            }
            
//            TextRenderer.DrawText(g, symbol, new Font("Calibri", cord.Bottom - cord.Top), new Point(cord.Left, cord.Top), Color.Black);
            
//            g.Flush();
//            g.FillRectangle(Brushes.Black, cord.Left, cord.Bottom, cord.Right - cord.Left, cord.Bottom - cord.Top);
//            g.SmoothingMode = SmoothingMode.AntiAlias;
//            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
//            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private List<(string Key, Network Value)> networks = new List<(string Key, Network Value)>();

        private string lastNetsPath = "";

        private void loadNamedNetBtn_Click(object sender, EventArgs e)
        {
            networks = new List<(string Key, Network Value)>();
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();

            browserDialog.SelectedPath = netsPathTextBox.Text;

            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                lastNetsPath = browserDialog.SelectedPath;
                var filesPaths = Directory.GetFiles(browserDialog.SelectedPath, "*.nw");

                foreach (var path in filesPaths)
                {
                    var setting = File.ReadAllBytes(path);
                    var symbleMatch = Regex.Match(path, @"\\_?(?<symble>\w+)\.nw");
                    var symble = symbleMatch.Groups["symble"].Value;
                    var net = new Network();

                    if (symble.Equals("point"))
                    {
                        symble = ".";
                    }

                    net.Load(setting);
                    networks.Add((symble, net));
                }

                MessageBox.Show("All nets loaded");
            }
        }

        private void openRestoreNetFormButton_Click(object sender, EventArgs e)
        {
            var form = new RestoreNetForm();
            form.Show();
        }
    }

    public class Result
    {
        public Cord cord { get; set; }
        public double answer { get; set; }
        public string result { get; set; }
    }

    public class MatrixWithCords
    {
        public Cord cord { get; set; }
        public double[,] matrix { get; set; }
    }
}
