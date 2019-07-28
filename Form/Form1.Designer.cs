namespace Sobel
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.vertPos = new System.Windows.Forms.NumericUpDown();
            this.prevVertPos = new System.Windows.Forms.Button();
            this.nextVertPos = new System.Windows.Forms.Button();
            this.nextHorPos = new System.Windows.Forms.Button();
            this.prevHorPos = new System.Windows.Forms.Button();
            this.horPosition = new System.Windows.Forms.NumericUpDown();
            this.contrastValue = new System.Windows.Forms.NumericUpDown();
            this.applyContrast = new System.Windows.Forms.Button();
            this.NeuroNet = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.openRestoreNetFormButton = new System.Windows.Forms.Button();
            this.loadNamedNetBtn = new System.Windows.Forms.Button();
            this.netsPathTextBox = new System.Windows.Forms.TextBox();
            this.averageNum = new System.Windows.Forms.NumericUpDown();
            this.BackPropoginationOpenButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.reloadNetsButton = new System.Windows.Forms.Button();
            this.recognizeButton = new System.Windows.Forms.Button();
            this.grayFilterButton = new System.Windows.Forms.Button();
            this.autoRotateButton = new System.Windows.Forms.Button();
            this.gaussianFilterButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.sigmaNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.kernelNumeric = new System.Windows.Forms.NumericUpDown();
            this.cannyApplyButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cannyTlNumeric = new System.Windows.Forms.NumericUpDown();
            this.cannyThNumeric = new System.Windows.Forms.NumericUpDown();
            this.sobelFilter2 = new System.Windows.Forms.Button();
            this.findMinNumeric = new System.Windows.Forms.NumericUpDown();
            this.reloadImgButton = new System.Windows.Forms.Button();
            this.RotateButton = new System.Windows.Forms.Button();
            this.PictureAngleNumeric = new System.Windows.Forms.NumericUpDown();
            this.GetAvrBrightButton = new System.Windows.Forms.Button();
            this.mainPicturePanel = new System.Windows.Forms.Panel();
            this.scaleUpButton = new System.Windows.Forms.Button();
            this.scaleDownButton = new System.Windows.Forms.Button();
            this.NeuroNetSettings = new System.Windows.Forms.TabPage();
            this.SearchSolutionWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastValue)).BeginInit();
            this.NeuroNet.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.averageNum)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sigmaNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kernelNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyTlNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyThNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.findMinNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureAngleNumeric)).BeginInit();
            this.mainPicturePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 708);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "load img";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.loadImgButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(2, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "sobel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.sobelFilterButton_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 61);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "find text";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.findTextButton_Click);
            // 
            // vertPos
            // 
            this.vertPos.Location = new System.Drawing.Point(9, 174);
            this.vertPos.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.vertPos.Name = "vertPos";
            this.vertPos.Size = new System.Drawing.Size(120, 20);
            this.vertPos.TabIndex = 4;
            this.vertPos.ValueChanged += new System.EventHandler(this.vertPos_ValueChanged);
            // 
            // prevVertPos
            // 
            this.prevVertPos.Location = new System.Drawing.Point(6, 200);
            this.prevVertPos.Name = "prevVertPos";
            this.prevVertPos.Size = new System.Drawing.Size(75, 23);
            this.prevVertPos.TabIndex = 5;
            this.prevVertPos.Text = "prev";
            this.prevVertPos.UseVisualStyleBackColor = true;
            this.prevVertPos.Click += new System.EventHandler(this.prevVertPos_Click);
            // 
            // nextVertPos
            // 
            this.nextVertPos.Location = new System.Drawing.Point(89, 200);
            this.nextVertPos.Name = "nextVertPos";
            this.nextVertPos.Size = new System.Drawing.Size(75, 23);
            this.nextVertPos.TabIndex = 6;
            this.nextVertPos.Text = "next";
            this.nextVertPos.UseVisualStyleBackColor = true;
            this.nextVertPos.Click += new System.EventHandler(this.nextVertPos_Click);
            // 
            // nextHorPos
            // 
            this.nextHorPos.Location = new System.Drawing.Point(88, 255);
            this.nextHorPos.Name = "nextHorPos";
            this.nextHorPos.Size = new System.Drawing.Size(75, 23);
            this.nextHorPos.TabIndex = 9;
            this.nextHorPos.Text = "next";
            this.nextHorPos.UseVisualStyleBackColor = true;
            this.nextHorPos.Click += new System.EventHandler(this.nextHorPos_Click);
            // 
            // prevHorPos
            // 
            this.prevHorPos.Location = new System.Drawing.Point(7, 255);
            this.prevHorPos.Name = "prevHorPos";
            this.prevHorPos.Size = new System.Drawing.Size(75, 23);
            this.prevHorPos.TabIndex = 8;
            this.prevHorPos.Text = "prev";
            this.prevHorPos.UseVisualStyleBackColor = true;
            this.prevHorPos.Click += new System.EventHandler(this.prevHorPos_Click);
            // 
            // horPosition
            // 
            this.horPosition.Location = new System.Drawing.Point(7, 229);
            this.horPosition.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.horPosition.Name = "horPosition";
            this.horPosition.Size = new System.Drawing.Size(120, 20);
            this.horPosition.TabIndex = 7;
            this.horPosition.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // contrastValue
            // 
            this.contrastValue.Location = new System.Drawing.Point(6, 318);
            this.contrastValue.Name = "contrastValue";
            this.contrastValue.Size = new System.Drawing.Size(120, 20);
            this.contrastValue.TabIndex = 10;
            this.contrastValue.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // applyContrast
            // 
            this.applyContrast.Location = new System.Drawing.Point(132, 315);
            this.applyContrast.Name = "applyContrast";
            this.applyContrast.Size = new System.Drawing.Size(75, 23);
            this.applyContrast.TabIndex = 11;
            this.applyContrast.Text = "apply";
            this.applyContrast.UseVisualStyleBackColor = true;
            this.applyContrast.Click += new System.EventHandler(this.applyContrast_Click);
            // 
            // NeuroNet
            // 
            this.NeuroNet.Controls.Add(this.mainTab);
            this.NeuroNet.Controls.Add(this.NeuroNetSettings);
            this.NeuroNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NeuroNet.Location = new System.Drawing.Point(0, 0);
            this.NeuroNet.Name = "NeuroNet";
            this.NeuroNet.SelectedIndex = 0;
            this.NeuroNet.Size = new System.Drawing.Size(1070, 746);
            this.NeuroNet.TabIndex = 14;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.tableLayoutPanel1);
            this.mainTab.Location = new System.Drawing.Point(4, 22);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainTab.Size = new System.Drawing.Size(1062, 720);
            this.mainTab.TabIndex = 0;
            this.mainTab.Text = "Main";
            this.mainTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.mainPicturePanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1056, 714);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.openRestoreNetFormButton);
            this.panel1.Controls.Add(this.loadNamedNetBtn);
            this.panel1.Controls.Add(this.netsPathTextBox);
            this.panel1.Controls.Add(this.averageNum);
            this.panel1.Controls.Add(this.BackPropoginationOpenButton);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.reloadNetsButton);
            this.panel1.Controls.Add(this.recognizeButton);
            this.panel1.Controls.Add(this.grayFilterButton);
            this.panel1.Controls.Add(this.autoRotateButton);
            this.panel1.Controls.Add(this.gaussianFilterButton);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.sobelFilter2);
            this.panel1.Controls.Add(this.findMinNumeric);
            this.panel1.Controls.Add(this.reloadImgButton);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.RotateButton);
            this.panel1.Controls.Add(this.applyContrast);
            this.panel1.Controls.Add(this.vertPos);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.contrastValue);
            this.panel1.Controls.Add(this.PictureAngleNumeric);
            this.panel1.Controls.Add(this.nextVertPos);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.nextHorPos);
            this.panel1.Controls.Add(this.prevVertPos);
            this.panel1.Controls.Add(this.GetAvrBrightButton);
            this.panel1.Controls.Add(this.prevHorPos);
            this.panel1.Controls.Add(this.horPosition);
            this.panel1.Location = new System.Drawing.Point(809, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 706);
            this.panel1.TabIndex = 0;
            // 
            // openRestoreNetFormButton
            // 
            this.openRestoreNetFormButton.Location = new System.Drawing.Point(7, 284);
            this.openRestoreNetFormButton.Name = "openRestoreNetFormButton";
            this.openRestoreNetFormButton.Size = new System.Drawing.Size(75, 23);
            this.openRestoreNetFormButton.TabIndex = 37;
            this.openRestoreNetFormButton.Text = "restore net";
            this.openRestoreNetFormButton.UseVisualStyleBackColor = true;
            this.openRestoreNetFormButton.Click += new System.EventHandler(this.openRestoreNetFormButton_Click);
            // 
            // loadNamedNetBtn
            // 
            this.loadNamedNetBtn.Location = new System.Drawing.Point(12, 589);
            this.loadNamedNetBtn.Name = "loadNamedNetBtn";
            this.loadNamedNetBtn.Size = new System.Drawing.Size(102, 23);
            this.loadNamedNetBtn.TabIndex = 36;
            this.loadNamedNetBtn.Text = "load named net";
            this.loadNamedNetBtn.UseVisualStyleBackColor = true;
            this.loadNamedNetBtn.Click += new System.EventHandler(this.loadNamedNetBtn_Click);
            // 
            // netsPathTextBox
            // 
            this.netsPathTextBox.Location = new System.Drawing.Point(12, 564);
            this.netsPathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.netsPathTextBox.Name = "netsPathTextBox";
            this.netsPathTextBox.Size = new System.Drawing.Size(217, 20);
            this.netsPathTextBox.TabIndex = 35;
            this.netsPathTextBox.Text = "C:\\Users\\zemtsov\\Pictures\\test2";
            // 
            // averageNum
            // 
            this.averageNum.DecimalPlaces = 3;
            this.averageNum.Location = new System.Drawing.Point(88, 93);
            this.averageNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.averageNum.Name = "averageNum";
            this.averageNum.Size = new System.Drawing.Size(51, 20);
            this.averageNum.TabIndex = 34;
            // 
            // BackPropoginationOpenButton
            // 
            this.BackPropoginationOpenButton.Location = new System.Drawing.Point(162, 95);
            this.BackPropoginationOpenButton.Name = "BackPropoginationOpenButton";
            this.BackPropoginationOpenButton.Size = new System.Drawing.Size(75, 23);
            this.BackPropoginationOpenButton.TabIndex = 33;
            this.BackPropoginationOpenButton.Text = "Network";
            this.BackPropoginationOpenButton.UseVisualStyleBackColor = true;
            this.BackPropoginationOpenButton.Click += new System.EventHandler(this.BackPropoginationOpenButton_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(9, 633);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(220, 144);
            this.panel2.TabIndex = 27;
            // 
            // reloadNetsButton
            // 
            this.reloadNetsButton.Location = new System.Drawing.Point(93, 523);
            this.reloadNetsButton.Name = "reloadNetsButton";
            this.reloadNetsButton.Size = new System.Drawing.Size(75, 23);
            this.reloadNetsButton.TabIndex = 25;
            this.reloadNetsButton.Text = "reload nets";
            this.reloadNetsButton.UseVisualStyleBackColor = true;
            this.reloadNetsButton.Click += new System.EventHandler(this.loadNetButton_Click);
            // 
            // recognizeButton
            // 
            this.recognizeButton.Location = new System.Drawing.Point(12, 523);
            this.recognizeButton.Name = "recognizeButton";
            this.recognizeButton.Size = new System.Drawing.Size(75, 23);
            this.recognizeButton.TabIndex = 24;
            this.recognizeButton.Text = "recognize";
            this.recognizeButton.UseVisualStyleBackColor = true;
            this.recognizeButton.Click += new System.EventHandler(this.recognizeButton_Click);
            // 
            // grayFilterButton
            // 
            this.grayFilterButton.Location = new System.Drawing.Point(166, 61);
            this.grayFilterButton.Name = "grayFilterButton";
            this.grayFilterButton.Size = new System.Drawing.Size(75, 23);
            this.grayFilterButton.TabIndex = 23;
            this.grayFilterButton.Text = "gray";
            this.grayFilterButton.UseVisualStyleBackColor = true;
            this.grayFilterButton.Click += new System.EventHandler(this.grayFilterButton_Click);
            // 
            // autoRotateButton
            // 
            this.autoRotateButton.Location = new System.Drawing.Point(144, 134);
            this.autoRotateButton.Name = "autoRotateButton";
            this.autoRotateButton.Size = new System.Drawing.Size(75, 23);
            this.autoRotateButton.TabIndex = 22;
            this.autoRotateButton.Text = "auto rotate";
            this.autoRotateButton.UseVisualStyleBackColor = true;
            this.autoRotateButton.Click += new System.EventHandler(this.autoRotateButton_Click);
            // 
            // gaussianFilterButton
            // 
            this.gaussianFilterButton.Location = new System.Drawing.Point(166, 32);
            this.gaussianFilterButton.Name = "gaussianFilterButton";
            this.gaussianFilterButton.Size = new System.Drawing.Size(75, 23);
            this.gaussianFilterButton.TabIndex = 21;
            this.gaussianFilterButton.Text = "gaussian";
            this.gaussianFilterButton.UseVisualStyleBackColor = true;
            this.gaussianFilterButton.Click += new System.EventHandler(this.gaussianFilterButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.sigmaNumeric);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.kernelNumeric);
            this.groupBox4.Controls.Add(this.cannyApplyButton);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.cannyTlNumeric);
            this.groupBox4.Controls.Add(this.cannyThNumeric);
            this.groupBox4.Location = new System.Drawing.Point(3, 344);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(238, 173);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Canny filter";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 109);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Sigma";
            // 
            // sigmaNumeric
            // 
            this.sigmaNumeric.Location = new System.Drawing.Point(48, 107);
            this.sigmaNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sigmaNumeric.Name = "sigmaNumeric";
            this.sigmaNumeric.Size = new System.Drawing.Size(51, 20);
            this.sigmaNumeric.TabIndex = 25;
            this.sigmaNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Kernel";
            // 
            // kernelNumeric
            // 
            this.kernelNumeric.Location = new System.Drawing.Point(48, 81);
            this.kernelNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.kernelNumeric.Name = "kernelNumeric";
            this.kernelNumeric.Size = new System.Drawing.Size(51, 20);
            this.kernelNumeric.TabIndex = 23;
            this.kernelNumeric.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cannyApplyButton
            // 
            this.cannyApplyButton.Location = new System.Drawing.Point(9, 144);
            this.cannyApplyButton.Name = "cannyApplyButton";
            this.cannyApplyButton.Size = new System.Drawing.Size(75, 23);
            this.cannyApplyButton.TabIndex = 21;
            this.cannyApplyButton.Text = "apply";
            this.cannyApplyButton.UseVisualStyleBackColor = true;
            this.cannyApplyButton.Click += new System.EventHandler(this.cannyApplyButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Tl";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Th";
            // 
            // cannyTlNumeric
            // 
            this.cannyTlNumeric.Location = new System.Drawing.Point(48, 55);
            this.cannyTlNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.cannyTlNumeric.Name = "cannyTlNumeric";
            this.cannyTlNumeric.Size = new System.Drawing.Size(51, 20);
            this.cannyTlNumeric.TabIndex = 20;
            this.cannyTlNumeric.Value = new decimal(new int[] {
            130,
            0,
            0,
            0});
            // 
            // cannyThNumeric
            // 
            this.cannyThNumeric.Location = new System.Drawing.Point(48, 29);
            this.cannyThNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.cannyThNumeric.Name = "cannyThNumeric";
            this.cannyThNumeric.Size = new System.Drawing.Size(51, 20);
            this.cannyThNumeric.TabIndex = 19;
            this.cannyThNumeric.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // sobelFilter2
            // 
            this.sobelFilter2.Location = new System.Drawing.Point(88, 32);
            this.sobelFilter2.Name = "sobelFilter2";
            this.sobelFilter2.Size = new System.Drawing.Size(75, 23);
            this.sobelFilter2.TabIndex = 19;
            this.sobelFilter2.Text = "sobel 2";
            this.sobelFilter2.UseVisualStyleBackColor = true;
            this.sobelFilter2.Click += new System.EventHandler(this.sobelFilter2_Click);
            // 
            // findMinNumeric
            // 
            this.findMinNumeric.Location = new System.Drawing.Point(88, 64);
            this.findMinNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.findMinNumeric.Name = "findMinNumeric";
            this.findMinNumeric.Size = new System.Drawing.Size(51, 20);
            this.findMinNumeric.TabIndex = 18;
            this.findMinNumeric.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // reloadImgButton
            // 
            this.reloadImgButton.Location = new System.Drawing.Point(88, 3);
            this.reloadImgButton.Name = "reloadImgButton";
            this.reloadImgButton.Size = new System.Drawing.Size(75, 23);
            this.reloadImgButton.TabIndex = 17;
            this.reloadImgButton.Text = "reload";
            this.reloadImgButton.UseVisualStyleBackColor = true;
            this.reloadImgButton.Click += new System.EventHandler(this.reloadImgButton_Click);
            // 
            // RotateButton
            // 
            this.RotateButton.Location = new System.Drawing.Point(63, 134);
            this.RotateButton.Name = "RotateButton";
            this.RotateButton.Size = new System.Drawing.Size(75, 23);
            this.RotateButton.TabIndex = 16;
            this.RotateButton.Text = "rotate";
            this.RotateButton.UseVisualStyleBackColor = true;
            this.RotateButton.Click += new System.EventHandler(this.RotateButton_Click);
            // 
            // PictureAngleNumeric
            // 
            this.PictureAngleNumeric.DecimalPlaces = 2;
            this.PictureAngleNumeric.Location = new System.Drawing.Point(6, 137);
            this.PictureAngleNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.PictureAngleNumeric.Name = "PictureAngleNumeric";
            this.PictureAngleNumeric.Size = new System.Drawing.Size(51, 20);
            this.PictureAngleNumeric.TabIndex = 15;
            // 
            // GetAvrBrightButton
            // 
            this.GetAvrBrightButton.Location = new System.Drawing.Point(3, 90);
            this.GetAvrBrightButton.Name = "GetAvrBrightButton";
            this.GetAvrBrightButton.Size = new System.Drawing.Size(75, 23);
            this.GetAvrBrightButton.TabIndex = 12;
            this.GetAvrBrightButton.Text = "avr bright";
            this.GetAvrBrightButton.UseVisualStyleBackColor = true;
            this.GetAvrBrightButton.Click += new System.EventHandler(this.GetAvrBrightButton_Click);
            // 
            // mainPicturePanel
            // 
            this.mainPicturePanel.AutoScroll = true;
            this.mainPicturePanel.Controls.Add(this.scaleUpButton);
            this.mainPicturePanel.Controls.Add(this.scaleDownButton);
            this.mainPicturePanel.Controls.Add(this.pictureBox1);
            this.mainPicturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPicturePanel.Location = new System.Drawing.Point(3, 3);
            this.mainPicturePanel.Name = "mainPicturePanel";
            this.mainPicturePanel.Size = new System.Drawing.Size(800, 708);
            this.mainPicturePanel.TabIndex = 1;
            // 
            // scaleUpButton
            // 
            this.scaleUpButton.Location = new System.Drawing.Point(3, 32);
            this.scaleUpButton.Name = "scaleUpButton";
            this.scaleUpButton.Size = new System.Drawing.Size(30, 23);
            this.scaleUpButton.TabIndex = 2;
            this.scaleUpButton.Text = "+";
            this.scaleUpButton.UseVisualStyleBackColor = true;
            this.scaleUpButton.Click += new System.EventHandler(this.scaleUpButton_Click);
            // 
            // scaleDownButton
            // 
            this.scaleDownButton.Location = new System.Drawing.Point(3, 3);
            this.scaleDownButton.Name = "scaleDownButton";
            this.scaleDownButton.Size = new System.Drawing.Size(30, 23);
            this.scaleDownButton.TabIndex = 1;
            this.scaleDownButton.Text = "-";
            this.scaleDownButton.UseVisualStyleBackColor = true;
            this.scaleDownButton.Click += new System.EventHandler(this.scaleDownButton_Click);
            // 
            // NeuroNetSettings
            // 
            this.NeuroNetSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.NeuroNetSettings.Location = new System.Drawing.Point(4, 22);
            this.NeuroNetSettings.Name = "NeuroNetSettings";
            this.NeuroNetSettings.Padding = new System.Windows.Forms.Padding(3);
            this.NeuroNetSettings.Size = new System.Drawing.Size(992, 526);
            this.NeuroNetSettings.TabIndex = 1;
            this.NeuroNetSettings.Text = "NeuroNet";
            // 
            // SearchSolutionWorker
            // 
            this.SearchSolutionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SearchSolutionWorker_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 746);
            this.Controls.Add(this.NeuroNet);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastValue)).EndInit();
            this.NeuroNet.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.averageNum)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sigmaNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kernelNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyTlNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyThNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.findMinNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureAngleNumeric)).EndInit();
            this.mainPicturePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.NumericUpDown vertPos;
        private System.Windows.Forms.Button prevVertPos;
        private System.Windows.Forms.Button nextVertPos;
        private System.Windows.Forms.Button nextHorPos;
        private System.Windows.Forms.Button prevHorPos;
        private System.Windows.Forms.NumericUpDown horPosition;
        private System.Windows.Forms.NumericUpDown contrastValue;
        private System.Windows.Forms.Button applyContrast;
        private System.Windows.Forms.TabControl NeuroNet;
        private System.Windows.Forms.TabPage NeuroNetSettings;
        private System.Windows.Forms.TabPage mainTab;
        private System.ComponentModel.BackgroundWorker SearchSolutionWorker;
        private System.Windows.Forms.Button GetAvrBrightButton;
        private System.Windows.Forms.NumericUpDown PictureAngleNumeric;
        private System.Windows.Forms.Button RotateButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button reloadImgButton;
        private System.Windows.Forms.NumericUpDown findMinNumeric;
        private System.Windows.Forms.Button sobelFilter2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cannyApplyButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown cannyTlNumeric;
        private System.Windows.Forms.NumericUpDown cannyThNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown kernelNumeric;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown sigmaNumeric;
        private System.Windows.Forms.Panel mainPicturePanel;
        private System.Windows.Forms.Button gaussianFilterButton;
        private System.Windows.Forms.Button autoRotateButton;
        private System.Windows.Forms.Button grayFilterButton;
        private System.Windows.Forms.Button BackPropoginationOpenButton;
        private System.Windows.Forms.Button recognizeButton;
        private System.Windows.Forms.Button reloadNetsButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown averageNum;
        private System.Windows.Forms.Button loadNamedNetBtn;
        private System.Windows.Forms.TextBox netsPathTextBox;
        private System.Windows.Forms.Button openRestoreNetFormButton;
        private System.Windows.Forms.Button scaleUpButton;
        private System.Windows.Forms.Button scaleDownButton;
    }
}

