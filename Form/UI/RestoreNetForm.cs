using Neuro.Domain.Layers;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;
using ScannerNet.Extensions;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sobel.UI
{
    public partial class RestoreNetForm : Form
    {
        private RestoredNet net = new RestoredNet();
        private Bitmap result = null;
        public Network Network;
        public bool Running;
        public event EventHandler<ShowLogModel> EventHandler;
        private int ImageSize = 202;

        public RestoreNetForm()
        {
            InitializeComponent();
            net.InitNet();
            //net.EventHandler += ShowLog;
            EventHandler += ShowLog;

            Network = new Network();

            Network.InitLayers((ImageSize, ImageSize),
                //new DropoutLayer(0.2f),
                new ConvolutionLayer(ActivationType.BipolarSigmoid, 4, 3, true, false),
                new MaxPoolingLayer(2),
                /*new ConvolutionLayer(ActivationType.BipolarSigmoid, 16, 3, true, true),
                new MaxPoolingLayer(2),*/
                //new ConvolutionLayer(ActivationType.LeakyReLu, 8, 3, true, false),
                //new MaxPoolingLayer(2),
                new FullyConnectedLayer(30, ActivationType.BipolarSigmoid),
                new DropoutLayer(0.2f),
                new FullyConnectedLayer(10, ActivationType.BipolarSigmoid),
                new DropoutLayer(0.3f),
                /*new FullyConnectedLayer(20, ActivationType.BipolarSigmoid),
                new DropoutLayer(0.4f),*/
                new SoftmaxLayer(2)
            );

            /*Network
                .AddConvolutionLayer(ActivationType.BipolarSigmoid, 4, 3, true, false)
                .AddMaxPoolingLayer(2)
                .AddFullyConnectedLayer(ActivationType.BipolarSigmoid, 30)
                .AddDropoutLayer(0.2f)
                .AddFullyConnectedLayer(ActivationType.BipolarSigmoid, 10)
                .AddDropoutLayer(0.3f)
                .AddSoftmaxLayer(2);*/

            Network.Randomize();
        }

        private void teachButton_Click(object sender, EventArgs e)
        {
            var actualPicture = (Bitmap)this.actualPicture.Image;
            var expectedPicture = (Bitmap)this.expectedPicture.Image;
            //net.Learn(actualPicture, expectedPicture, 0.02f);
            Task.Factory.StartNew(() => { net.Learn(actualPicture, expectedPicture, 0.02f); });
        }

        private void ShowLog(object sender, ShowLogModel model)
        {
            errorLabel.Text = model.TotalError.ToString(CultureInfo.InvariantCulture);
            timeLabel.Text = model.TotalTime.ToString(CultureInfo.InvariantCulture);
            iterationLabel.Text = model.Iteration.ToString(CultureInfo.InvariantCulture);
        }

        private void loadActualPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.pdf)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.pdf";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bit;

                    if (Regex.IsMatch(open.FileName, @"\.pdf$", RegexOptions.IgnoreCase))
                    {
                        var doc = new PdfDocument(open.FileName);
                        bit = new Bitmap(doc.Pages[0].ExtractImages().FirstOrDefault());
                    }
                    else
                    {
                        bit = new Bitmap(open.FileName);
                    }

                    actualPicture.Image = bit;
                    actualPicture.BackgroundImage = bit;
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed loading image");
            }
        }

        private void loadExpectedPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bit = new Bitmap(open.FileName);
                    expectedPicture.Image = bit;
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed loading image");
            }
        }

        private void restorePictureButton_Click(object sender, EventArgs e)
        {
            var image = (Bitmap)this.actualPicture.Image;
            result = net.Restore(image);

            this.actualPicture.Image = null;
            this.actualPicture.Image = result;
        }

        private void stopTeachButton_Click(object sender, EventArgs e)
        {
            net.Running = false;
        }

        private void SavePictureBtn_Click(object sender, EventArgs e)
        {
            var open = new SaveFileDialog();
            open.Filter = "Image File(*.png)|*.png";

            if (result != null && open.ShowDialog() == DialogResult.OK)
            {
                result.Save(open.FileName);
            }
        }

        private Random _random = new Random();

        private void TeachPhotoButton_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => { TeachPhoto(); });
        }

        private void TeachPhoto()
        {
            Running = true;
            var falseDirectoryPath = @"C:\Users\zemtsov\Pictures\печать 2\false";
            var trueDirectoryPath = @"C:\Users\zemtsov\Pictures\печать 2\true";
            List<Bitmap> falseImages = new List<Bitmap>();
            List<Bitmap> trueImages = new List<Bitmap>();
            
            try
            {
                var falseFilesPath = Directory.GetFiles(falseDirectoryPath, "*.*", SearchOption.AllDirectories);

                foreach (var path in falseFilesPath)
                {
                    Bitmap image;

                    if (Regex.IsMatch(path, @"\.pdf$", RegexOptions.IgnoreCase))
                    {
                        try
                        {
                            var doc = new PdfDocument(path);
                            image = new Bitmap(doc.Pages[0].ExtractImages().FirstOrDefault());
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        image = new Bitmap(path);
                    }

                    image = image.ScaleImage(ImageSize, ImageSize);

                    falseImages.Add(image);
                }

                var trueFilesPath = Directory.GetFiles(trueDirectoryPath, "*.*", SearchOption.AllDirectories);

                foreach (var path in trueFilesPath)
                {
                    Bitmap image;

                    if (Regex.IsMatch(path, @"\.pdf$", RegexOptions.IgnoreCase))
                    {
                        try
                        {
                            var doc = new PdfDocument(path);
                            image = new Bitmap(doc.Pages[0].ExtractImages().FirstOrDefault());
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        image = new Bitmap(path);
                    }

                    image = image.ScaleImage(ImageSize, ImageSize);

                    trueImages.Add(image);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Failed loading image", e);
            }

            var falseAnswerCount = 0;
            var trueAnswerCount = 0;
            var teacher = new BackPropagationLearning(Network)
            {
                LearningRate = (float)learningRateNumericUpDown.Value
            };
            double totalError = 0;
            int iteration = 1;
            var teachedTrue = new List<int>();
            var teachedFalse = new List<int>();
            var st = new Stopwatch();
            double totalTime = 0;

            while (Running)
            {
                teacher.LearningRate = (float)learningRateNumericUpDown.Value;
                var output = new float[2];

                st.Start();

                if (trueAnswerCount < 2)
                {
                    var i = _random.Next(trueImages.Count);

                    if (teachedTrue.Count == trueImages.Count)
                        teachedTrue.Clear();

                    if (teachedTrue.Any(x => x == i))
                        continue;

                    teachedTrue.Add(i);

                    var input = trueImages[i].GetMatrix(invert: true, optimize: false);

                    Network.TeachCompute(input);
                    output[0] = 1f;

                    totalError += teacher.Run(input, output);
                    trueAnswerCount++;
                    falseAnswerCount = 0;
                }
                else
                {
                    var i = _random.Next(falseImages.Count);

                    if (teachedFalse.Count == falseImages.Count)
                        teachedFalse.Clear();

                    if (teachedFalse.Any(x => x == i))
                        continue;

                    teachedFalse.Add(i);

                    var input = falseImages[i].GetMatrix(invert: true, optimize: false);

                    Network.TeachCompute(input);
                    output[1] = 1f;

                    totalError += teacher.Run(input, output);
                    falseAnswerCount++;
                    trueAnswerCount = 0;
                }

                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                //if (iteration % 20 == 0)
                EventHandler.Invoke(this, new ShowLogModel(totalError / iteration, totalTime / (iteration + 1), iteration));

                st.Reset();
                iteration++;
            }
        }

        private void TeachPhotoStopButton_Click(object sender, EventArgs e)
        {
            Running = false;
        }

        private void TestPhotoButton_Click(object sender, EventArgs e)
        {
            var input = ((Bitmap)this.actualPicture.Image).ScaleImage(ImageSize, ImageSize).GetMatrix(invert: true, optimize: false);
            var result = Network.Compute(input);

            photoResultLabel.Text = result[0] > result[1] ? "true" : "false";
        }

        private void SavePhotoNetButton_Click(object sender, EventArgs e)
        {
            var open = new SaveFileDialog();
            open.Filter = "Network Files(*.nw)|*.nw";

            if (open.ShowDialog() == DialogResult.OK)
            {
                var data = Network.Save();
                using (var file = open.OpenFile())
                {
                    file.Write(data, 0, data.Length);
                }
            }
        }

        private void LoadPhotoNetButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Network Files(*.nw)|*.nw";

            if (open.ShowDialog() == DialogResult.OK)
            {
                var a = File.ReadAllBytes(open.FileName);
                Network.Load(a);
            }
        }

        private void GetTrueResultsButton_Click(object sender, EventArgs e)
        {
            var docTypeName = "waybills";
            var saveDirectory = @"C:\Users\zemtsov\Pictures\печать 2\test";
            var directoryPath = $@"D:\documents types\доки\доки\{docTypeName}";
            var filesPath = "*.jpg|*.png|*.jpeg".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(directoryPath, filter, SearchOption.AllDirectories)).ToArray();
            var i = 100;
            var findedResults = 0;

            var trueFiles = Directory.GetFiles($@"{saveDirectory}\true").ToList();
            var falseFiles = Directory.GetFiles($@"{saveDirectory}\false").ToList();

            foreach (var file in trueFiles)
            {
                File.Delete(file);
            }

            foreach (var file in falseFiles)
            {
                File.Delete(file);
            }

            foreach (var filePath in filesPath.Skip(i).Take(100))
            {
                try
                {
                    var bitmap = new Bitmap(filePath);
                    var input = bitmap.ScaleImage(ImageSize, ImageSize).GetMatrix(invert: true, optimize: false);
                    var result = Network.Compute(input);

                    bitmap.Save($"{saveDirectory}\\{(result[0] > result[1] ? "true" : "false")}\\{docTypeName}_{i}.png", ImageFormat.Png);
                    i++;
                    /*if (result[0] > result[1])
                    {
                        bitmap.Save($"{saveDirectory}\\{i}.png", ImageFormat.Png);
                        findedResults++;

                        if (findedResults == 100)
                            return;
                    }

                    */
                }
                catch
                {
                    continue;
                }
            }
        }

        private void TestConvButton_Click(object sender, EventArgs e)
        {
            /*var input = new Matrix(new Matrix(((Bitmap)this.actualPicture.Image).ScaleImage(ImageSize, ImageSize).GetMatrix(invert: true, optimize: false)).To1DFloatArray(), ImageSize);
            var Weights = new Matrix(GetMatrixes(1, 3, 1)[0]);

            if (Network.Layers[1] is IConvolutionLayer)
            {
                Weights = ((IConvolutionLayer)Network.Layers[1]).Neurons[0].Weights;
            }

            var c = input.Convolution(Weights, 1);
            var c2 = input.Convolution2(Weights, 1);

            this.actualPicture.Image = c.Value.ToBitmap();
            this.expectedPicture.Image = c2.Value.ToBitmap();*/

            var layers = Network.Layers.Where(x => x is IConvolutionLayer).ToList();

            if (layers.Count > 0)
                this.actualPicture.Image = ((IConvolutionLayer)layers[0]).Neurons[1].Output.Value.ToBitmap();

            if (layers.Count > 1)
                this.expectedPicture.Image = ((IConvolutionLayer)layers[1]).Neurons[1].Output.Value.ToBitmap();
        }

        private static float[][,] GetMatrixes(int deep, int width, float s = 0f)
        {
            var c = new float[deep][,];

            for (var i = 0; i < deep; i++)
            {
                c[i] = new float[width, width];

                for (var y = 0; y < c[i].GetLength(0); y++)
                    for (var x = 0; x < c[i].GetLength(1); x++)
                        c[i][y, x] = y + x + i + s;
            }

            return c;
        }
    }

    public class ShowLogModel
    {
        public ShowLogModel(double totalError, double totalTime, int iteration)
        {
            TotalError = totalError;
            TotalTime = totalTime;
            Iteration = iteration;
        }

        public double TotalError { get; set; }
        public double TotalTime { get; set; }
        public int Iteration { get; set; }
    }
}
