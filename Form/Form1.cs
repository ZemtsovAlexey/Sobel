using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        public ConvolutionalNetwork Network = new ConvolutionalNetwork();
        private int ImageWidth = 20;
        private int ImageHeight = 20;
        private string lastImgPath = null;
        private (int x, int y) pictureSize = (28, 28);
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
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed loading image");
            }
        }

        private void sobelFilterButton_Click(object sender, EventArgs e)
        {
            var bmp = new Bitmap(workImage);
            workImage = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format1bppIndexed);
//            workImage = Segmentation.ToBlackWite(new Bitmap(pictureBox1.Image));
            pictureBox1.Image = workImage;
//            pictureBox1.BackgroundImage = Segmentation.Sobel(new Bitmap(lastImgPath));

            //Bitmap a = AForge.Imaging.Image.Clone(new Bitmap(pictureBox1.ImageLocation), PixelFormat.Format8bppIndexed);
            //AForge.Imaging.Image.SetGrayscalePalette(a);

            //// create filter
            //SobelEdgeDetector filter = new SobelEdgeDetector();
            //// apply the filter
            //filter.ApplyInPlace(a);

            //pictureBox1.BackgroundImage = a;
        }

        private void findTextButton_Click(object sender, EventArgs e)
        {
            byte difMin = (byte)findMinNumeric.Value;
            var result = Segmentation.ShowTextCord2(new Bitmap(workImage), difMin);
            workImage = result.img;
            pictureBox1.Image = result.img; //Utils.TestSearch(new Bitmap(pictureBox1.BackgroundImage));
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
        
        private void nextVertPos_Click(object sender, EventArgs e)
        {
            Y++;
            var result = Segmentation.ShowTextCordDebug(new Bitmap(workImage), Y);
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
            //pictureBox1.Image = Utils.TestSearch(new Bitmap(pictureBox1.Image));
            horPosition.Value = horPosition.Value + 1;
            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.BackgroundImage, (int)vertPos.Value, (int)horPosition.Value);
        }

        private void prevHorPos_Click(object sender, EventArgs e)
        {
            horPosition.Value = horPosition.Value - 1;
            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.Image, (int)vertPos.Value, (int)horPosition.Value);
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
//                var bitmap = bmp.DrawString(text, (int)nwFontSizeNumeric.Value, fontFamilyTextBox.Text).ResizeImage(new RectangleF(0, 0, (float)ImageWidth, (float)ImageHeight));
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
            AvrBrightLabel.Text = Segmentation.GetAverBright(new Bitmap(pictureBox1.Image)).ToString();
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Segmentation.RotateImage(new Bitmap(lastImgPath), (float)PictureAngleNumeric.Value);
            AvrBrightLabel.Text = Segmentation.GetAverBright(new Bitmap(pictureBox1.Image)).ToString();
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
            var result = Segmentation.Test(new Bitmap(workImage));
            workImage = result.img;
            cords = result.cords;
            pictureBox1.Image = workImage;
        }

        private void cannyApplyButton_Click(object sender, EventArgs e)
        {
            //var c = new Canny(new Bitmap(pictureBox1.Image), (float)cannyThNumeric.Value, (float)cannyTlNumeric.Value, (int)kernelNumeric.Value, (int)sigmaNumeric.Value);
            //pictureBox1.Image = c.DisplayImage(c.EdgeMap);
            workImage = new Bitmap(pictureBox1.Image).Canny((double)cannyThNumeric.Value, (double)cannyTlNumeric.Value, (int)kernelNumeric.Value);
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
            panel2.Controls.Clear();
            var picture = new Bitmap(pictureBox1.Image);
            var picture2 = new Bitmap(lastImgPath);
            //var cannyPicture = Segmentation.Test(picture);

            //var result = Segmentation.ShowTextCord2(cannyPicture);
            //cords = result.cords;
            var i = 0;

            foreach (var cord in cords.Where(x => (x.Right - x.Left > 6) && (x.Right - x.Left < 100)).OrderBy(x => x.Top).ThenBy(x => x.Left).Take(500))
            {
                var width = cord.Right - cord.Left + 4;
                var height = cord.Bottom - cord.Top + 4;

                if (width < 6 || height < 6)
                {
                    continue;
                }

                try
                {
                    var cloneRect = new Rectangle(cord.Left - 2, cord.Top - 2, width, height);
//                    var cloneRect = new Rectangle(cord.Left, cord.Top, width, height);
                    var cloneBitmap = picture.Clone(cloneRect, picture.PixelFormat);
                    var cloneBitmap2 = picture2.Clone(cloneRect, picture2.PixelFormat);//.ToBlackWite();

                    var bitmap = cloneBitmap.ResizeImage(pictureSize.x, pictureSize.y);
                    var bitmap2 = cloneBitmap2.ResizeImage(pictureSize.x, pictureSize.y);

                    var netResult = Network.Compute(bitmap.GetDoubleMatrix());

                    var box = new PictureBox
                    {
                        Location = new System.Drawing.Point(10, 4 + (i * (pictureSize.y + 3))),
                        Name = $"pictureBoxResult{i}",
                        Size = new System.Drawing.Size(pictureSize.x, pictureSize.y),
                        BackColor = Color.Black,
                        Image = bitmap2,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var label = new Label
                    {
                        Location = new System.Drawing.Point(10 + pictureSize.x, 8 + (i * (pictureSize.y + 3))),
                        Name = $"labelResult{i}",
                        Size = new System.Drawing.Size(120, pictureSize.y),
                        Text = netResult[0].ToString(),
                        ForeColor = netResult[0] > 0.5 ? Color.DarkGreen : Color.Black
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

        private void loadNetButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Network Files(*.nw)|*.nw";

            if (open.ShowDialog() == DialogResult.OK)
            {
                var setting = File.ReadAllBytes(open.FileName);
                Network.Load(setting);
            }
        }
    }
}
