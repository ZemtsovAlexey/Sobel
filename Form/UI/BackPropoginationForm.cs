using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Neuro;
using Neuro.ThirdPath;
using ScannerNet;
using ScannerNet.Extensions;
using Sobel.Neronet;

namespace Sobel.UI
{
    public partial class BackPropoginationForm : Form
    {
        private BackPropoginationNew _networkNew = new BackPropoginationNew();
        private INeuralNetworkThirdPath networkThirdPath = new INeuralNetworkThirdPath();
        private Random _random = new Random();
        private Series _seriesStop;
        private Series _seriesSuccess;
        private int _succeses = 0;
        private bool _neadToStopLearning;
        private (int x, int y) pictureSize = (28, 28);

        public BackPropoginationForm()
        {
            InitializeComponent();
            InitLerningChart();

            //var a = new Class1();
            //a.Test();

            //networkThirdPath.Init();
        }

        private void InitLerningChart()
        {
            chart1.Series.Clear();

            _seriesStop = new Series
            {
                Name = "Stop",
                Color = Color.Blue,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Spline,
                BorderWidth = 1
            };

            _seriesSuccess = new Series
            {
                Name = "Success",
                Color = Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Spline,
                BorderWidth = 1
            };

            this.chart1.Series.Add(_seriesStop);
            this.chart1.Series.Add(_seriesSuccess);
        }

        private void ShowGraffic(long iteration, int success)
        {
            _succeses = success > _succeses
                ? success
                : _succeses <= 0
                    ? 0
                    : success == 0
                        ? _succeses - 3
                        : _succeses;

            if (_seriesStop.Points.Count > 60)
            {
                _seriesStop.Points.Remove(_seriesStop.Points.First());
                _seriesSuccess.Points.Remove(_seriesSuccess.Points.First());
            }

            _seriesStop.Points.AddXY(iteration, (int)learningStopNumeric.Value);
            _seriesSuccess.Points.AddXY(iteration, _succeses);
        }

        private void ShowLogs(object sender, LogEventArgs e)
        {
            ShowGraffic(e.I, e.Success);
            totalTimeText.Text = e.Time.ToString();
            resultErrorText.Text = e.Error.ToString();
        }

        private async void startLearnButton_Click(object sender, EventArgs e)
        {
            var st = new Stopwatch();
            st.Start();

            _succeses = 0;
            _neadToStopLearning = false;
            InitLerningChart();

            //LearnNew();
            await Task.Run(() => LearnNew());
            //            LearnThirdPath();
            //            await Task.Run(() => LearnAnyNeurons());
            //            LearnAnyNeurons();

            //            totalTimeText.Text = st.ElapsedMilliseconds.ToString();
        }

        private void stopLearnButton_Click(object sender, EventArgs e)
        {
            _neadToStopLearning = true;
        }

        private void regnizeButton_Click(object sender, EventArgs e)
        {
            textViewPicture.SizeMode = PictureBoxSizeMode.StretchImage;
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.White);

            var rotateImage = (double)textRotateNumeric.Value;
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);

            var bitmap = new Bitmap(b, textViewPicture.Width, textViewPicture.Height)
                .DrawString(recognizedText.Text, 50, rotateImage, random: _random)
                .CutSymbol(padding)
                .ResizeImage(pictureSize.x, pictureSize.y);
            textViewPicture.Image = bitmap;

            var st = new Stopwatch();
            st.Start();

            var result = _networkNew.Compute(bitmap);

            var time = st.ElapsedMilliseconds;
            
            int maxIter = 0;
            double maxRes = 0;
            var k = 0;

            foreach (var neuron in result)
            {
                if (maxRes < neuron)
                {
                    maxRes = neuron;
                    maxIter = k;
                }

                k++;
            }

            realAnswerText.Text = $@"{maxIter} - {string.Join(" | ", result)}";
        }

        private void Learn()
        {
            startLearnButton.Enabled = false;

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            var falseAnswerCount = 0;
            double totalTime = 0;
            double totalError = 0;
            var rotateImage = (double)textRotateNumeric.Value;
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);

            long i = 0;

            var teacher = new Neuro.Learning.ConvolutionalBackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (float)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            
            while (iterations == 0 || i < iterations)
            {
                if (_neadToStopLearning) break;

                teacher.LearningRate = (float)learningRateNumeric.Value;
                st.Start();
                var text = _random.RandomSymble();

                Bitmap bitmap;
                float[] output;
                
                if (!text.symble.Equals(trueAnswerText.Text) && falseAnswerCount < 1)
                {
                    bitmap = bmp.DrawString(text.symble, 50, rotateImage, random: _random).CutSymbol(padding).ResizeImage(pictureSize.x, pictureSize.y);
                    output = new float[] { 0 };
                    falseAnswerCount++;
                }
                else
                {
                    bitmap = bmp.DrawString(trueAnswerText.Text, 50, rotateImage, random: _random).CutSymbol(padding).ResizeImage(pictureSize.x, pictureSize.y);
                    output = new float[] { 0.71f };
                    falseAnswerCount = 0;
                }
                
                totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                
                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, 0, totalTime / (i + 1), totalError / (i + 1)));

                st.Reset();
                i++;
            }

            startLearnButton.Enabled = true;
        }
        
        private void LearnNew()
        {
            startLearnButton.Enabled = false;

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            (string symble, int position) text;
            Bitmap bitmap;
            int falseAnswerCount = 0;
            float[] output;
            float error = 0;
            int succeses = 0;
            float totalTime = 0;
            float totalError = 0;
            var rotateImage = (float)textRotateNumeric.Value;
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);

            long i = 0;

            var teacher = new Neuro.Learning.ConvolutionalBackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (float)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            
            while ((totalError > 0.01f || iterations < 20) && (iterations == 0 ||i < iterations))
            {
                if (_neadToStopLearning) break;

                teacher.LearningRate = (float)learningRateNumeric.Value;
                
//                st.Start();

                text = _random.RandomSymble();

                if (!text.symble.Equals(trueAnswerText.Text) && falseAnswerCount < 1)
                {
                    bitmap = bmp.DrawString(text.symble, 50, rotateImage, random: _random).CutSymbol(padding).ResizeImage(pictureSize.x, pictureSize.y);
                    output = new float[] { 0f };
                    var computed = _networkNew.Compute(bitmap)[0];

                    st.Start();

                    if (computed >= 0.5f)
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                        
                        succeses = 0;
                    }
                    else
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), new float[] { computed - teacher.LearningRate });
                        succeses++;
                    }

                    st.Stop();
                    totalTime += st.ElapsedMilliseconds;
                    falseAnswerCount++;
                }
                else
                {
                    bitmap = bmp.DrawString(trueAnswerText.Text, 50, rotateImage, random: _random).CutSymbol(padding).ResizeImage(pictureSize.x, pictureSize.y);
                    output = new float[] { 1 };
                    var computed = _networkNew.Compute(bitmap)[0];

                    st.Start();

                    if (computed < 0.5f)
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                        succeses = 0;
                    }
                    else
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), new float[] { computed + teacher.LearningRate });
                        succeses++;
                    }

                    st.Stop();
                    totalTime += st.ElapsedMilliseconds;
                    falseAnswerCount = 0;
                }
                
//                st.Stop();
//                totalTime += st.ElapsedMilliseconds;
//                var delimiter = Math.Min(999, i);

                BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, succeses, totalTime / (i + 1), totalError / (i + 1)));

                st.Reset();
                i++;
            }

            startLearnButton.Enabled = true;
        }
        
        private void LearnAnyNeurons()
        {
            startLearnButton.Enabled = false;

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            (string symble, int position) text;
            Bitmap bitmap;
            float[] output;
            int succeses = 0;
            float totalTime = 0;
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);

            long i = 0;

            var teacher = new Neuro.Learning.ConvolutionalBackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (float)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            
            while (succeses < (int)learningStopNumeric.Value && i < iterations)
            {
                if (_neadToStopLearning) break;
                
                st.Start();

                text = _random.RandomSymble();
                
                int maxIter = 0;
                double maxRes = 0;
                var k = 0;

                bitmap = bmp.DrawString(text.symble, 70, random: _random).CutSymbol(padding).ResizeImage(new RectangleF(0, 0, (float)20, (float)20));
                var result = _networkNew.Compute(bitmap);
                
                foreach (var neuron in result)
                {
                    if (maxRes < neuron)
                    {
                        maxRes = neuron;
                        maxIter = k;
                    }

                    k++;
                }

                if (text.position != maxIter)
                {
                    output = new float[5];

                    for (var j = 0; j < 5; j++)
                    {
                        output[j] = j == text.position ? 0.75f : 0.1f;
                    }
                    
                    teacher.Run(bitmap.GetDoubleMatrix(), output);
                    succeses = 0;
                }
//                else if (text.position == maxIter && maxRes < 0.7)
//                {
//                    output = new double[5];
//
//                    for (var j = 0; j < 5; j++)
//                    {
//                        output[j] = j == text.position ? 0.75 : 0.1;
//                    }
//                    
//                    teacher.Run(bitmap.GetDoubleMatrix(), output);
//                    succeses = 0;
//                }
                else
                {
                    succeses++;
                }
                
                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, succeses, totalTime / (i + 1), 0));

                st.Reset();
                i++;
            }

            startLearnButton.Enabled = true;
        }

        private void LearnThirdPath()
        {
            startLearnButton.Enabled = false;

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            (string symble, int position) text;
            Bitmap bitmap;
            int falseAnswerCount = 0;
            double[] input;
            float[] output;
            double error = 0;
            int succeses = 0;
            double totalTime = 0;
            List<(float[] x, float[] u)> trainBatch = new List<(float[] x, float[] u)>();
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);

            long i = 0;

            var st = new Stopwatch();

            while (succeses < (int)learningStopNumeric.Value && (iterations == 0 || i < iterations))
            {
                if (_neadToStopLearning) break;

                st.Start();

                text = _random.RandomSymble();


                if (!text.symble.Equals(trueAnswerText.Text) && falseAnswerCount < 1)
                {
                    bitmap = bmp.DrawString(text.symble, 70, random: _random).CutSymbol(padding).ResizeImage(new RectangleF(0, 0, pictureSize.x, pictureSize.y)).Canny();
                    output = new float[] { 0 };
                    trainBatch.Add((bitmap.ToFloat(), output));
                    falseAnswerCount++;
                }
                else
                {
                    bitmap = bmp.DrawString(trueAnswerText.Text, 70, random: _random).CutSymbol(padding).ResizeImage(new RectangleF(0, 0, pictureSize.x, pictureSize.y)).Canny();
                    output = new float[] { 0 };
                    trainBatch.Add((bitmap.ToFloat(), output));
                    falseAnswerCount = 0;
                }

                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                //BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, succeses, totalTime / (i + 1)));

                st.Reset();
                i++;
            }

            networkThirdPath.Train(trainBatch);

            startLearnButton.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var open = new SaveFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                var data = _networkNew.Network.Save();
                using (var file = open.OpenFile())
                {
                    file.Write(data, 0, data.Length);
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                var a = File.ReadAllBytes(open.FileName);
                _networkNew.Network.Load(a);
            }
        }
    }
}
