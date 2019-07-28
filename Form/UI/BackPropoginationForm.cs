﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Neuro.Domain.Layers;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;
using Neuro.ThirdPath;
using ScannerNet;
using ScannerNet.Extensions;
using ScannerNet.Models;
using Sobel.Models;
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
        private (int x, int y) pictureSize = (16, 16);
        private BindingSource bindingSource1 = new BindingSource();
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        
        private const string BadResults = " /\\*(){}-+&?#DFGIJLQRSUVWYZbdfghijklqrstuvwyz0123456789";
//            var trueResults = "0123456789ЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯСМИТЬБЮёйцукенгшщзхъфывапролджэячсмитьбю";
        private const string TrueResults = "ёйцукенгшщзхъфывапролджэячсмитьбю"; //ЁЕАБО
        
        public BackPropoginationForm()
        {
            InitializeComponent();
            InitLerningChart();
//            InitNetworkSettingsPanel();
            InitNetworkSettings();
//            Test();

            //var a = new Class1();
            //a.Test();

            //networkThirdPath.Init();
        }

        private void Test()
        {
            var net = new Network();
            var activation = ActivationType.None;

            net.InitLayers((3, 3),
                new ConvolutionLayer(activation, 1, 2),//24
                new FullyConnectedLayer(1, activation)
            );

            var convNeuron = (net.Layers[0] as ConvolutionLayer).Neurons[0];

            convNeuron.Weights = new Matrix(new [,]{{0.1d, 0.2d}, {0.3d, 0.4d}});

            var firstFullConLayer = (net.Layers[1] as FullyConnectedLayer);

            firstFullConLayer[0].Weights[0] = 0.1d;
            firstFullConLayer[0].Weights[1] = 0.2d;
            firstFullConLayer[0].Weights[2] = 0.3d;
            firstFullConLayer[0].Weights[3] = 0.4d;

            var input = new double[3, 3]
            {
                {0.1d, 0.2d, 0.3d},
                {0.4d, 0.5d, 0.6d},
                {0.7d, 0.8d, 0.9d}
            };
            
            var output = net.Compute(input);

            var teacher = new Neuro.Learning.BackPropagationLearning(net)
            {
                LearningRate = 1f
            };
            
            teacher.Run(input, new []{ 0.60d });
            
            var output2 = net.Compute(input);
            
            var a = output;
        }
        
        private void InitNetworkSettings()
        {
            bindingSource1 = new BindingSource();
            var dataGridView1 = netSettingsDataGridView;

            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            foreach (var layer in _networkNew.Network.Layers)
            {
                if (layer.Type == LayerType.Convolution)
                {
                    var convLayer = (IConvolutionLayer)layer;
                    bindingSource1.Add(new NetworkSettings(layer.Type, convLayer.ActivationFunctionType, convLayer.NeuronsCount, convLayer.KernelSize));
                }
                
                if (layer.Type == LayerType.MaxPoolingLayer)
                {
                    var convLayer = (IMaxPoolingLayer)layer;
                    bindingSource1.Add(new NetworkSettings(layer.Type, null, null, convLayer.KernelSize));
                }
                
                if (layer.Type == LayerType.FullyConnected)
                {
                    var convLayer = (IFullyConnectedLayer)layer;
                    bindingSource1.Add(new NetworkSettings(layer.Type, convLayer.ActivationFunctionType, convLayer.NeuronsCount, null));
                }

                if (layer.Type == LayerType.Softmax)
                {
                    var convLayer = (ISoftmaxLayer)layer;
                    bindingSource1.Add(new NetworkSettings(layer.Type, ActivationType.None, convLayer.NeuronsCount, null));
                }
            }
            
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSize = true;
            
            dataGridView1.Columns.Add(CreateComboBoxLayerType());
            dataGridView1.Columns.Add(CreateComboBoxActivationType());
            
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "NeuronsCount";
            column.Name = "NeuronsCount";
            column.Frozen = false;
            dataGridView1.Columns.Add(column);
            
            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
            column2.DataPropertyName = "KernelSize";
            column2.Name = "KernelSize";
            column2.Frozen = false;
            dataGridView1.Columns.Add(column2);
            
            dataGridView1.EditingControlShowing += _DataGridView_EditingControlShowing;
            dataGridView1.BindingContextChanged += new EventHandler(BindingContext_Changed);

            dataGridView1.DataSource = bindingSource1;
        }

        private DataGridViewComboBoxColumn CreateComboBoxLayerType()
        {
            return new DataGridViewComboBoxColumn
                {
                    DataSource = Enum.GetValues(typeof(LayerType)),
                    DataPropertyName = "Type",
                    Name = "Type",
                    Frozen = false
                };
        }
        
        private DataGridViewComboBoxColumn CreateComboBoxActivationType()
        {
            var types = Enum.GetValues(typeof(ActivationType)).Cast<ActivationType>().ToList();

            return new DataGridViewComboBoxColumn
            {
                ValueType = typeof(ActivationType?),
                DataSource = types,
                DataPropertyName = "Activation",
                Name = "Activation",
                Frozen = false
            };
        }
        
        private void BindingContext_Changed(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = netSettingsDataGridView.Rows;

            foreach (DataGridViewRow row in rows)
            {
                var type = row.Cells[0];

                row.Cells[1].ReadOnly = type.Value != null && (LayerType) type.Value == LayerType.MaxPoolingLayer;
            }
        }
        
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            try
            {
                var selectedValue = (sender as ComboBox)?.SelectedValue;
                
                if (selectedValue?.ToString() == "MaxPoolingLayer")
                {
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].ReadOnly = true;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].ReadOnly = true;
                    
                    netSettingsDataGridView.EndEdit();
                    
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].Value = ActivationType.None;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value = null;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[0].Value = LayerType.MaxPoolingLayer;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value = netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value ?? 2;
                }
                else if (selectedValue?.ToString() == "FullyConnected")
                {
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].ReadOnly = true;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].ReadOnly = false;
                    
                    netSettingsDataGridView.EndEdit();
                    
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].Value = ActivationType.BipolarSigmoid;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value = null;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value = netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value ?? 1;;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[0].Value = LayerType.FullyConnected;
                }
                else if (selectedValue?.ToString() == "Softmax")
                {
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].ReadOnly = true;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].ReadOnly = false;
                    
                    netSettingsDataGridView.EndEdit();
                    
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].Value = ActivationType.BipolarSigmoid;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value = null;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value = netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value ?? 1;;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[0].Value = LayerType.Softmax;
                }
                else
                {
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].ReadOnly = false;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].ReadOnly = false;
                    
                    netSettingsDataGridView.EndEdit();
                    
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[1].Value = ActivationType.ReLu;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value = netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[2].Value ?? 1;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value = netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[3].Value ?? 3;
                    netSettingsDataGridView.Rows[netSettingsDataGridView.CurrentCell.RowIndex].Cells[0].Value = selectedValue;
                }

            }
            catch { }
        }
        
        private void _DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((sender as DataGridView).SelectedCells[0].ColumnIndex == 0)
            {
                if ((e.Control as ComboBox) != null)
                {
                    (e.Control as ComboBox).SelectionChangeCommitted -= ComboBox_SelectedIndexChanged;
                    (e.Control as ComboBox).SelectionChangeCommitted += ComboBox_SelectedIndexChanged;
                }
            }
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    DragDropEffects dropEffect = netSettingsDataGridView.DoDragDrop(netSettingsDataGridView.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            rowIndexFromMouseDown = netSettingsDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (rowIndexFromMouseDown != -1)
            {
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
            {
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = netSettingsDataGridView.PointToClient(new Point(e.X, e.Y));
            rowIndexOfItemUnderMouseToDrop = netSettingsDataGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                var r = bindingSource1[rowIndexFromMouseDown];
                bindingSource1.Insert(rowIndexOfItemUnderMouseToDrop, r);
                bindingSource1.RemoveAt(rowIndexFromMouseDown);

                netSettingsDataGridView.DataSource = bindingSource1;
            }
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
                BorderWidth = 1, 
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

            _succeses = Math.Max(0, _succeses);
            
            if (_seriesSuccess.Points.Count > 50)
            {
//                _seriesSuccess.Points.Clear();
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
            
            _succeses = e.Success > _succeses
                ? e.Success
                : _succeses <= 0
                    ? 0
                    : e.Success == 0
                        ? _succeses - 3
                        : _succeses;

            _succeses = Math.Max(0, _succeses);

            //textBoxAnswResult.Text = _succeses.ToString();
        }

        private async void startLearnButton_Click(object sender, EventArgs e)
        {
            var st = new Stopwatch();
            st.Start();

            _succeses = 0;
            _neadToStopLearning = false;
            InitLerningChart();

//            LearnAnyNeurons();
            await Task.Run(() => LearnAnyNeurons());
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
            var scale = ((int)scaleFromNum.Value, (int)scaleToNum.Value);
            
            var H = _random.Next(-padding.Item1, padding.Item1);
            var V = _random.Next(-padding.Item2, padding.Item2);
            padding.Item1 = H;
            padding.Item2 = V;

            var fontSize = _random.Next(16, 50);
            
            var bitmap = new Bitmap(b, textViewPicture.Width, textViewPicture.Height)
                .DrawString(recognizedText.Text, fontSize, rotateImage, random: _random)
                .CutSymbol(padding, scale)
                .ResizeImage1(pictureSize.x, pictureSize.y);
            textViewPicture.Image = bitmap;

            var st = new Stopwatch();
            st.Start();

            var result = _networkNew.Compute(new Matrix(bitmap.GetDoubleMatrix(optimize: false)));

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

            var outputs = _networkNew.Network.Layers.Last().NeuronsCount;
            var trueData = outputs > 2 ? TrueResults : trueAnswerText.Text;

            realAnswerText.Text = $@"{(maxIter >= outputs - 1 ? '*' : trueData[maxIter])} - {result[maxIter]}";
        }

        private void Learn()
        {
            startLearnButton.Enabled = false;

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            (string symble, int position) text;
            Bitmap bitmap;
            int falseAnswerCount = 0;
            int trueAnswerCount = 0;
            double[] output;
            double error = 0;
            int succeses = 0;
            double totalTime = 0;
            double totalError = 0;
            var rotateImage = (double)textRotateNumeric.Value;
            (int V, int H) padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);
            var scale = ((int)scaleFromNum.Value, (int)scaleToNum.Value);

            long i = 0;

            var teacher = new Neuro.Learning.BackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (double)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            
            while ((totalError > 0.01f || iterations < 20) && (iterations == 0 ||i < iterations))
            {
                if (_neadToStopLearning) break;

                teacher.LearningRate = (double)learningRateNumeric.Value;
                var fontSize = _random.Next(16, 50);
                
                st.Start();

                text = _random.RandomSymble();

                padding.H = _random.Next((-((int)paddingHNumeric.Value)), ((int)paddingHNumeric.Value));
                padding.V = _random.Next((-((int)paddingVNumeric.Value)), ((int)paddingVNumeric.Value));

                Matrix matrix;
                
                if (!text.symble.Equals(trueAnswerText.Text) && falseAnswerCount < 2)
                {
                    matrix = new Matrix(bmp.DrawString(text.symble, fontSize, rotateImage, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y).GetDoubleMatrix(1));
                    var computed = _networkNew.Compute(matrix)[0];
                    output = new double[] { -1f };

//                    st.Start();
                    
                    succeses = computed < -0.1d ? succeses + 1 : 0;

                    falseAnswerCount++;
                    trueAnswerCount = 0;
                }
                else
                {
                    matrix = new Matrix(bmp.DrawString(trueAnswerText.Text, fontSize, 0, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y).GetDoubleMatrix(1));
                    var computed = _networkNew.Compute(matrix)[0];
                    output = new double[] { 1f };

//                    st.Start();

                    succeses = computed > 0.9d ? succeses + 1 : 0;

                    falseAnswerCount=0;
                    trueAnswerCount++;
                }
                
                totalError += teacher.Run(matrix.Value, output);
                
                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, succeses, totalTime / (i + 1), totalError / (i + 1)));

                st.Reset();
                i++;
            }

            startLearnButton.Enabled = true;
        }

        private List<Symble> LoadTeachDataSet(string name)
        {
            var image = new Bitmap($@"C:\Users\zemtsov\Pictures\test acts\{name}.jpg");
//            var imageMap = image.GetDoubleMatrix(invert: false, optimize: false);
            var document = XDocument.Load($@"C:\Users\zemtsov\Pictures\test acts\{name}.xml");
            
            var root = document?.Root;

            if (root == null)
                return null;
            
            var ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("a", "http://www.abbyy.com/FineReader_xml/FineReader10-schema-v1.xml");

            var chars = root.Document
                .XPathSelectElements("//a:formatting//a:charParams", ns)
                .Where(x => x.Value.Length > 0)
                .Select(x => 
                    new Symble(
                        image
                            .GetPart(int.Parse(x.Attribute("l").Value), int.Parse(x.Attribute("t").Value), int.Parse(x.Attribute("r").Value), int.Parse(x.Attribute("b").Value))
                            .ResizeImage1(pictureSize.x, pictureSize.y)
                            .GetDoubleMatrix(optimize: false),
                        x.Value));
		
            return chars.ToList();
        }

        private double[,] GetSymbolMap(IReadOnlyList<Symble> symbles, string symble)
        {
            var targetSymbles = symbles.Where(x => x.Value == symble).ToList();

            if (!targetSymbles.Any())
                return null;
            
            var index = _random.Next(targetSymbles.Count - 1);

            return targetSymbles[index].Img;
        }

        private List<Symble> Symbles = null;
        
        private void LearnAnyNeurons()
        {
            startLearnButton.Enabled = false;

            /*List<Symble> symbols = null;

            if (Symbles == null)
            {
                Symbles = LoadTeachDataSet("30");
                Symbles.AddRange(LoadTeachDataSet("52"));
                Symbles.AddRange(LoadTeachDataSet("51"));
                
                symbols = Symbles;
            }
            else
            {
                symbols = Symbles;
            }*/

            var bmp = new Bitmap(textViewPicture.Width, textViewPicture.Height);
            var iterations = (long)learnIterationsNumeric.Value;
            (string symble, int position) text;
            Bitmap bitmap;
            double[,] symbleMap;
            int succeses = 0;
            double totalTime = 0;
            double totalError = 0;
            var rotateImage = (double)textRotateNumeric.Value;
            (int V, int H) padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);
            var scale = ((int)scaleFromNum.Value, (int)scaleToNum.Value);
            var outputs = _networkNew.Network.Layers.Last().NeuronsCount;

            var teacher = new Neuro.Learning.BackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (double)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            var teachedList = new List<int>();
            long i = 0;
            var trueAnswerCount = 0;
            var badChars = outputs > 2 ? BadResults.ToCharArray() : BadResults.Where(x => x != trueAnswerText.Text[0]).Select(x => x).ToArray();
            var trueChars = outputs > 2 ? TrueResults : trueAnswerText.Text;

            while ((iterations == 0 ||i < iterations))
            {
                if (_neadToStopLearning) break;
                
                st.Start();
                
                teacher.LearningRate = (double)learningRateNumeric.Value;
                padding.H = _random.Next((-((int)paddingHNumeric.Value)), ((int)paddingHNumeric.Value));
                padding.V = _random.Next((-((int)paddingVNumeric.Value)), ((int)paddingVNumeric.Value));
                var output = new double[outputs];
                
                var fontSize = _random.Next(16, 50);
//                var fontSize = 50;

                if (trueAnswerCount < trueChars.Length / 2)
                {
                    if (teachedList.Count == outputs - 1)
                        teachedList.Clear();
//
                    text = _random.RandomSymble(trueChars);
//                
                    if (teachedList.Any(x => x == text.position))
                        continue;
                
                    if (text.position < outputs)
                        teachedList.Add(text.position);
                    
//                    bitmap = bmp.DrawString(text.symble, fontSize, rotateImage, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y);
                    symbleMap = bmp.DrawString2(text.symble, fontSize, rotateImage, random: _random);
//                    var averBright = bitmap.GetAverBright();
//                    bitmap = bitmap.ToBlackWite(averBright);

//                    symbleMap = GetSymbolMap(symbols, text.symble);
//                    
//                    if (symbleMap == null)
//                        continue;
//                    
                    var result = _networkNew.Compute(new Matrix(symbleMap));
//                    var result = _networkNew.Compute(new Matrix(bitmap.GetDoubleMatrix(optimize: false)));
                    
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

                    output[text.position] = 1d;// Math.Max(1f, maxRes);
                    succeses = text.position != maxIter ? 0 : succeses + 1;
                    trueAnswerCount++;
                }
                else
                {
                    text = _random.RandomSymble(string.Join("", badChars));
//                    bitmap = bmp.DrawString(text.symble, fontSize, rotateImage, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y);
                    symbleMap = bmp.DrawString2(text.symble, fontSize, rotateImage, random: _random);
//                    symbleMap = GetSymbolMap(symbols, text.symble);
                    
//                    if (symbleMap == null)
//                        continue;
                    
//                    var result = _networkNew.Compute(new Matrix(bitmap.GetDoubleMatrix(optimize: false)));
                    var result = _networkNew.Compute(new Matrix(symbleMap));

                    output[outputs - 1] = 1d;
                    succeses = result[outputs - 1] >= 0.5d ? succeses + 1 : 0;
                    trueAnswerCount = 0;
                }

//                totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                totalError += teacher.Run(symbleMap, output);
                
                st.Stop();
                totalTime += st.ElapsedMilliseconds;

                if (i % 10 == 0)
                {
                    BeginInvoke(new EventHandler<LogEventArgs>(ShowLogs), this, new LogEventArgs(i, succeses, totalTime / (i + 1), totalError / (i + 1)));
                    //succeses = 0;
                }

                st.Reset();
                i++;
            }

            startLearnButton.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var open = new SaveFileDialog();
            open.Filter = "Network Files(*.nw)|*.nw";

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
            open.Filter = "Network Files(*.nw)|*.nw";

            if (open.ShowDialog() == DialogResult.OK)
            {
                var a = File.ReadAllBytes(open.FileName);
                _networkNew.Network.Load(a);
                InitNetworkSettings();
            }
        }

        private void netSettingsApplyButton_Click(object sender, EventArgs e)
        {
            var data = bindingSource1.List.Cast<NetworkSettings>();
            var initData = new List<ILayer>();
            _networkNew.Network = new Neuro.Networks.Network();

            foreach(var item in data)
            {
                switch (item.Type) {
                    case LayerType.Convolution:
                        initData.Add(new ConvolutionLayer(item.Activation.Value, item.NeuronsCount.Value, item.KernelSize.Value, true));
                        break;

                    case LayerType.MaxPoolingLayer:
                        initData.Add(new MaxPoolingLayer(item.KernelSize.Value));
                        break;

                    case LayerType.FullyConnected:
                        initData.Add(new FullyConnectedLayer(item.NeuronsCount.Value, item.Activation.Value));
                        break;
                    
                    case LayerType.Softmax:
                        initData.Add(new SoftmaxLayer(item.NeuronsCount.Value));
                        break;
                }
            }

            _networkNew.Network.InitLayers(pictureSize, initData.ToArray());
            _networkNew.Network.Randomize();
        }

        private void LoadTestImgButton_Click(object sender, EventArgs e)
        {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bit = new Bitmap(open.FileName);

                    var rotateImage = (double)textRotateNumeric.Value;
                    var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);
                    var scale = ((int)scaleFromNum.Value, (int)scaleToNum.Value);
                    var averege = bit.GetAverBright();
                    
                    var bitmap = bit
                        .ToBlackWite(averege)
                        .CutSymbol(padding, scale)
                        .ScaleImage(pictureSize.x, pictureSize.y);
                    
                    textViewPicture.Image = bitmap;
                    
                    var result = _networkNew.Compute(new Matrix(bitmap.GetDoubleMatrix()));

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
        }
    }
}
