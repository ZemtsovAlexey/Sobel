﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public ActivationNetwork Network = new ActivationNetwork();
        private int ImageWidth = 20;
        private int ImageHeight = 20;
        private string lastImgPath = null;
        
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
            pictureBox1.Image = Segmentation.Sobel(new Bitmap(pictureBox1.Image));
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
            var result = Segmentation.ShowTextCord2(new Bitmap(pictureBox1.Image), difMin);
            pictureBox1.Image = result.img; //Utils.TestSearch(new Bitmap(pictureBox1.BackgroundImage));
            cords = result.cords;
        }

        private void prevVertPos_Click(object sender, EventArgs e)
        {
            vertPos.Value = vertPos.Value - 1;
            pictureBox1.Image = Utils.ShowDifferent(pictureBox1.Image, (int)vertPos.Value, (int)horPosition.Value);
        }

        public Bitmap PrevHorPosImage = null;

        private void nextVertPos_Click(object sender, EventArgs e)
        {
            vertPos.Value = vertPos.Value + 1;
            PrevHorPosImage = PrevHorPosImage ?? new Bitmap(pictureBox1.Image);
            pictureBox1.Image = Utils.ShowDifferent(PrevHorPosImage, (int)vertPos.Value, (int)horPosition.Value);
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
            pictureBox1.Image = new Bitmap(pictureBox1.Image).Contrast((int)contrastValue.Value);
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
        }

        private void sobelFilter2_Click(object sender, EventArgs e)
        {
//            var c = new Canny(new Bitmap(lastImgPath), 80F, 35F, 5, 1);
//            pictureBox1.Image = c.DisplayImage(c.EdgeMap);
//            pictureBox1.BackgroundImage = new Bitmap(pictureBox1.BackgroundImage).GetGrayMap().ToBitmap();
            pictureBox1.Image = Segmentation.Test(new Bitmap(pictureBox1.Image));
        }

        private void cannyApplyButton_Click(object sender, EventArgs e)
        {
            var c = new Canny(new Bitmap(pictureBox1.Image), (float)cannyThNumeric.Value, (float)cannyTlNumeric.Value, (int)kernelNumeric.Value, (int)sigmaNumeric.Value);
            pictureBox1.Image = c.DisplayImage(c.EdgeMap);
        }

        private void gaussianFilterButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Image).GetGrayMap()
                .GaussianFilter((int) kernelNumeric.Value, (int) sigmaNumeric.Value).ToBitmap();
        }

        private void autoRotateButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Segmentation.AutoRotate(new Bitmap(pictureBox1.Image));
        }

        private void grayFilterButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Image).GetGrayMap().ToBitmap();
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
            //var cannyPicture = Segmentation.Test(picture);

            //var result = Segmentation.ShowTextCord2(cannyPicture);
            //cords = result.cords;
            var i = 0;

            foreach (var cord in cords)
            {
                var width = cord.Right - cord.Left;
                var height = cord.Bottom - cord.Top;

                if (width < 6 || height < 6)
                {
                    continue;
                }

                var cloneRect = new Rectangle(cord.Left, cord.Top, width, height);
                var cloneBitmap = picture.Clone(cloneRect, picture.PixelFormat);

                var bitmap = cloneBitmap.ResizeImage(new RectangleF(0, 0, (float)20, (float)20));
                var vector = bitmap.ToDoubles().Select(x => x / 255).ToArray();

                var netResult = Network.Compute(vector);

                var box = new PictureBox
                {
                    Location = new System.Drawing.Point(10, 4 + (i * 23)),
                    Name = $"pictureBoxResult{i}",
                    Size = new System.Drawing.Size(20, 20),
                    BackColor = Color.Black,
                    Image = bitmap,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var label = new Label
                {
                    Location = new System.Drawing.Point(30, 8 + (i * 23)),
                    Name = $"labelResult{i}",
                    Size = new System.Drawing.Size(100, 20),
                    Text = netResult[0].ToString(),
                    ForeColor = netResult[0] > 0.7 ? Color.DarkGreen : Color.Black
                };

                panel2.Controls.Add(box);
                panel2.Controls.Add(label);
                i++;
            }
        }

        private void loadNetButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                var setting = File.ReadAllBytes(open.FileName);
                Network = (ActivationNetwork)Network.Load(setting);
            }
        }
    }
}
