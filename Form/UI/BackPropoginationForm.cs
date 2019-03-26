using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Neuro.Domain.Layers;
using Neuro.Layers;
using Neuro.Models;
using Neuro.Networks;
using Neuro.Neurons;
using Neuro.ThirdPath;
using ScannerNet;
using ScannerNet.Extensions;
using ScannerNet.Models;
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
        private (int x, int y) pictureSize = (26, 26);
        private BindingSource bindingSource1 = new BindingSource();
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        
        public BackPropoginationForm()
        {
            InitializeComponent();
            InitLerningChart();
//            InitNetworkSettingsPanel();
            InitNetworkSettings();
            Test();

            //var a = new Class1();
            //a.Test();

            //networkThirdPath.Init();
        }

        private void Test()
        {
            var net = new Network();
            var activation = ActivationType.None;

            net.InitLayers(3, 3,
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

            //Learn();
            await Task.Run(() => Learn());
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
                .ScaleImage(pictureSize.x, pictureSize.y);
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
                
//                st.Start();

                text = _random.RandomSymble();

                padding.H = _random.Next((-((int)paddingHNumeric.Value)), ((int)paddingHNumeric.Value));
                padding.V = _random.Next((-((int)paddingVNumeric.Value)), ((int)paddingVNumeric.Value));
                
                if (!text.symble.Equals(trueAnswerText.Text) && falseAnswerCount < 2)
                {
                    //teacher.LearningRate = (double)learningRateNumeric.Value / 2;
                    bitmap = bmp.DrawString(text.symble, fontSize, rotateImage, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y);
                    output = new double[] { -1f };
                    st.Start();

                    var computed = _networkNew.Compute(bitmap)[0];


                    if (computed >= 0f)
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                        
                        succeses = 0;
                    }
                    else
                    {
                        //if (succeses == 0)
                        //totalError += teacher.Run(bitmap.GetDoubleMatrix(), new double[] { computed - teacher.LearningRate });
                        succeses++;
                    }

                    st.Stop();
                    totalTime += st.ElapsedMilliseconds;
                    falseAnswerCount++;
                    trueAnswerCount = 0;
                }
                else
                {
                    //teacher.LearningRate = (double)learningRateNumeric.Value;
                    bitmap = bmp.DrawString(trueAnswerText.Text, fontSize, 0, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y);
                    //output = (Math.Abs(padding.H) > 2 || Math.Abs(padding.V) > 2) ? new[] { -1f } : new double[] { 1 };
                    output = new double[] { 1f };
                    st.Start();

                    var computed = _networkNew.Compute(bitmap)[0];


                    if (/*!(Math.Abs(padding.H) > 0 || Math.Abs(padding.V) > 0) && */computed < 0.3f)
                    {
                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), output);
                        succeses = 0;
                    }
                    else
                    {
                        //if (succeses == 0)
                            //totalError += teacher.Run(bitmap.GetDoubleMatrix(), new double[] { computed + teacher.LearningRate });
//                        totalError += teacher.Run(bitmap.GetDoubleMatrix(), new double[] { computed + teacher.LearningRate });
                        succeses++;
                    }

                    st.Stop();
                    totalTime += st.ElapsedMilliseconds;
                    //falseAnswerCount = (Math.Abs(padding.H) > 2 || Math.Abs(padding.V) > 2) ? falseAnswerCount + 1 : 0;
                    //trueAnswerCount = (Math.Abs(padding.H) > 2 || Math.Abs(padding.V) > 2) ? 0 : trueAnswerCount + 1;
                    falseAnswerCount=0;
                    trueAnswerCount++;
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
            double[] output;
            int succeses = 0;
            double totalTime = 0;
            var rotateImage = (double)textRotateNumeric.Value;
            var padding = ((int)paddingVNumeric.Value, (int)paddingHNumeric.Value);
            var scale = ((int)scaleFromNum.Value, (int)scaleToNum.Value);

            long i = 0;

            var teacher = new Neuro.Learning.BackPropagationLearning(_networkNew.Network)
            {
                LearningRate = (double)learningRateNumeric.Value
            };

            var st = new Stopwatch();
            
            while ((iterations == 0 ||i < iterations))
            {
                if (_neadToStopLearning) break;
                
                st.Start();

                text = _random.RandomSymble();
                
                int maxIter = 0;
                double maxRes = 0;
                var k = 0;

                bitmap = bmp.DrawString(text.symble, 50, rotateImage, random: _random).CutSymbol(padding, scale).ScaleImage(pictureSize.x, pictureSize.y);
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
                    output = new double[79];

                    for (var j = 0; j < 79; j++)
                    {
                        output[j] = j == text.position ? 1f : 0f;
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
                        initData.Add(new ConvolutionLayer(item.Activation.Value, item.NeuronsCount.Value, item.KernelSize.Value));
                        break;

                    case LayerType.MaxPoolingLayer:
                        initData.Add(new MaxPoolingLayer(item.KernelSize.Value));
                        break;

                    case LayerType.FullyConnected:
                        initData.Add(new FullyConnectedLayer(item.NeuronsCount.Value, item.Activation.Value));
                        break;
                }
            }

            _networkNew.Network.InitLayers(pictureSize.x, pictureSize.y, initData.ToArray());
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
                    
                    var result = _networkNew.Compute(bitmap);

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
