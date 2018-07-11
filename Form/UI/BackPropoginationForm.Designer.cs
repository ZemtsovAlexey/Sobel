namespace Sobel.UI
{
    partial class BackPropoginationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.realAnswerText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.regnizeButton = new System.Windows.Forms.Button();
            this.textRotateNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trueAnswerText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.recognizedText = new System.Windows.Forms.TextBox();
            this.textViewPicture = new System.Windows.Forms.PictureBox();
            this.startLearnButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.learningStopNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.resultErrorText = new System.Windows.Forms.TextBox();
            this.totalTimeText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.learningRateNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.learnIterationsNumeric = new System.Windows.Forms.NumericUpDown();
            this.stopLearnButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textRotateNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textViewPicture)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.learningStopNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.learningRateNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.learnIterationsNumeric)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loadButton);
            this.groupBox1.Controls.Add(this.saveButton);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.realAnswerText);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.regnizeButton);
            this.groupBox1.Controls.Add(this.textRotateNumeric);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trueAnswerText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.recognizedText);
            this.groupBox1.Controls.Add(this.textViewPicture);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 188);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Testing";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(282, 156);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 42;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(202, 156);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 41;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(199, 123);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 40;
            this.label9.Text = "Верный ответ";
            // 
            // realAnswerText
            // 
            this.realAnswerText.Location = new System.Drawing.Point(282, 120);
            this.realAnswerText.MaxLength = 1;
            this.realAnswerText.Name = "realAnswerText";
            this.realAnswerText.Size = new System.Drawing.Size(151, 20);
            this.realAnswerText.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(200, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(233, 2);
            this.label8.TabIndex = 38;
            this.label8.Text = "                                         ";
            // 
            // regnizeButton
            // 
            this.regnizeButton.Location = new System.Drawing.Point(359, 156);
            this.regnizeButton.Name = "regnizeButton";
            this.regnizeButton.Size = new System.Drawing.Size(75, 23);
            this.regnizeButton.TabIndex = 38;
            this.regnizeButton.Text = "Recognize";
            this.regnizeButton.UseVisualStyleBackColor = true;
            this.regnizeButton.Click += new System.EventHandler(this.regnizeButton_Click);
            // 
            // textRotateNumeric
            // 
            this.textRotateNumeric.DecimalPlaces = 2;
            this.textRotateNumeric.Location = new System.Drawing.Point(359, 72);
            this.textRotateNumeric.Maximum = new decimal(new int[] {
            359,
            0,
            0,
            0});
            this.textRotateNumeric.Name = "textRotateNumeric";
            this.textRotateNumeric.Size = new System.Drawing.Size(74, 20);
            this.textRotateNumeric.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Угол наклона текста";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Верный ответ";
            // 
            // trueAnswerText
            // 
            this.trueAnswerText.Location = new System.Drawing.Point(359, 45);
            this.trueAnswerText.MaxLength = 1;
            this.trueAnswerText.Name = "trueAnswerText";
            this.trueAnswerText.Size = new System.Drawing.Size(74, 20);
            this.trueAnswerText.TabIndex = 4;
            this.trueAnswerText.Text = "А";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(199, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Распозноваемый символ";
            // 
            // recognizedText
            // 
            this.recognizedText.Location = new System.Drawing.Point(359, 19);
            this.recognizedText.MaxLength = 1;
            this.recognizedText.Name = "recognizedText";
            this.recognizedText.Size = new System.Drawing.Size(74, 20);
            this.recognizedText.TabIndex = 1;
            this.recognizedText.Text = "А";
            // 
            // textViewPicture
            // 
            this.textViewPicture.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textViewPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textViewPicture.Location = new System.Drawing.Point(6, 19);
            this.textViewPicture.Name = "textViewPicture";
            this.textViewPicture.Size = new System.Drawing.Size(160, 160);
            this.textViewPicture.TabIndex = 0;
            this.textViewPicture.TabStop = false;
            // 
            // startLearnButton
            // 
            this.startLearnButton.Location = new System.Drawing.Point(106, 179);
            this.startLearnButton.Name = "startLearnButton";
            this.startLearnButton.Size = new System.Drawing.Size(75, 23);
            this.startLearnButton.TabIndex = 3;
            this.startLearnButton.Text = "Start";
            this.startLearnButton.UseVisualStyleBackColor = true;
            this.startLearnButton.Click += new System.EventHandler(this.startLearnButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.learningStopNumeric);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.resultErrorText);
            this.groupBox2.Controls.Add(this.totalTimeText);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.learningRateNumeric);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.learnIterationsNumeric);
            this.groupBox2.Controls.Add(this.stopLearnButton);
            this.groupBox2.Controls.Add(this.startLearnButton);
            this.groupBox2.Location = new System.Drawing.Point(6, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 242);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Net settings";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 44;
            this.label11.Text = "Learning stop";
            // 
            // learningStopNumeric
            // 
            this.learningStopNumeric.Location = new System.Drawing.Point(120, 74);
            this.learningStopNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.learningStopNumeric.Name = "learningStopNumeric";
            this.learningStopNumeric.Size = new System.Drawing.Size(74, 20);
            this.learningStopNumeric.TabIndex = 43;
            this.learningStopNumeric.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 140);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Total time";
            // 
            // resultErrorText
            // 
            this.resultErrorText.Enabled = false;
            this.resultErrorText.Location = new System.Drawing.Point(122, 111);
            this.resultErrorText.MaxLength = 1;
            this.resultErrorText.Name = "resultErrorText";
            this.resultErrorText.Size = new System.Drawing.Size(74, 20);
            this.resultErrorText.TabIndex = 37;
            this.resultErrorText.Text = "0";
            // 
            // totalTimeText
            // 
            this.totalTimeText.Enabled = false;
            this.totalTimeText.Location = new System.Drawing.Point(122, 137);
            this.totalTimeText.MaxLength = 1;
            this.totalTimeText.Name = "totalTimeText";
            this.totalTimeText.Size = new System.Drawing.Size(74, 20);
            this.totalTimeText.TabIndex = 41;
            this.totalTimeText.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Total error";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(8, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(188, 2);
            this.label6.TabIndex = 34;
            this.label6.Text = "                                         ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Learning Rate";
            // 
            // learningRateNumeric
            // 
            this.learningRateNumeric.DecimalPlaces = 3;
            this.learningRateNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.learningRateNumeric.Location = new System.Drawing.Point(120, 48);
            this.learningRateNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.learningRateNumeric.Name = "learningRateNumeric";
            this.learningRateNumeric.Size = new System.Drawing.Size(74, 20);
            this.learningRateNumeric.TabIndex = 11;
            this.learningRateNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Iterations";
            // 
            // learnIterationsNumeric
            // 
            this.learnIterationsNumeric.Location = new System.Drawing.Point(120, 22);
            this.learnIterationsNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.learnIterationsNumeric.Name = "learnIterationsNumeric";
            this.learnIterationsNumeric.Size = new System.Drawing.Size(74, 20);
            this.learnIterationsNumeric.TabIndex = 9;
            // 
            // stopLearnButton
            // 
            this.stopLearnButton.Location = new System.Drawing.Point(24, 179);
            this.stopLearnButton.Name = "stopLearnButton";
            this.stopLearnButton.Size = new System.Drawing.Size(75, 23);
            this.stopLearnButton.TabIndex = 4;
            this.stopLearnButton.Text = "Stop";
            this.stopLearnButton.UseVisualStyleBackColor = true;
            this.stopLearnButton.Click += new System.EventHandler(this.stopLearnButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chart1);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Location = new System.Drawing.Point(12, 206);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(708, 267);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Learning";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(219, 19);
            this.chart1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(478, 188);
            this.chart1.TabIndex = 5;
            this.chart1.Text = "chart1";
            // 
            // BackPropoginationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 548);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "BackPropoginationForm";
            this.Text = "BackPropogination Net";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textRotateNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textViewPicture)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.learningStopNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.learningRateNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.learnIterationsNumeric)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox textViewPicture;
        private System.Windows.Forms.TextBox recognizedText;
        private System.Windows.Forms.NumericUpDown textRotateNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox trueAnswerText;
        private System.Windows.Forms.Button startLearnButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown learnIterationsNumeric;
        private System.Windows.Forms.Button stopLearnButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown learningRateNumeric;
        private System.Windows.Forms.TextBox resultErrorText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox realAnswerText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button regnizeButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox totalTimeText;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown learningStopNumeric;
    }
}