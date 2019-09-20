namespace Sobel.UI
{
    partial class RestoreNetForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.expectedPicture = new System.Windows.Forms.PictureBox();
            this.actualPicture = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timeLabel = new System.Windows.Forms.Label();
            this.errorLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.testConvButton = new System.Windows.Forms.Button();
            this.getTrueResultsButton = new System.Windows.Forms.Button();
            this.loadPhotoNetButton = new System.Windows.Forms.Button();
            this.savePhotoNetButton = new System.Windows.Forms.Button();
            this.photoResultLabel = new System.Windows.Forms.Label();
            this.teachPhotoStopButton = new System.Windows.Forms.Button();
            this.testPhotoButton = new System.Windows.Forms.Button();
            this.TeachPhotoButton = new System.Windows.Forms.Button();
            this.SavePictureBtn = new System.Windows.Forms.Button();
            this.stopTeachButton = new System.Windows.Forms.Button();
            this.restorePictureButton = new System.Windows.Forms.Button();
            this.teachButton = new System.Windows.Forms.Button();
            this.loadExpectedPictureButton = new System.Windows.Forms.Button();
            this.loadActualPictureButton = new System.Windows.Forms.Button();
            this.learningRateNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.iterationLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expectedPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualPicture)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.learningRateNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.learningRateNumericUpDown, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1334, 826);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.expectedPicture, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.actualPicture, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 54);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1130, 770);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // expectedPicture
            // 
            this.expectedPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.expectedPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.expectedPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expectedPicture.Location = new System.Drawing.Point(567, 2);
            this.expectedPicture.Margin = new System.Windows.Forms.Padding(2);
            this.expectedPicture.Name = "expectedPicture";
            this.expectedPicture.Size = new System.Drawing.Size(561, 766);
            this.expectedPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.expectedPicture.TabIndex = 1;
            this.expectedPicture.TabStop = false;
            // 
            // actualPicture
            // 
            this.actualPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.actualPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actualPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actualPicture.Location = new System.Drawing.Point(2, 2);
            this.actualPicture.Margin = new System.Windows.Forms.Padding(2);
            this.actualPicture.Name = "actualPicture";
            this.actualPicture.Size = new System.Drawing.Size(561, 766);
            this.actualPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.actualPicture.TabIndex = 0;
            this.actualPicture.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.iterationLabel);
            this.panel1.Controls.Add(this.timeLabel);
            this.panel1.Controls.Add(this.errorLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1130, 48);
            this.panel1.TabIndex = 2;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timeLabel.Location = new System.Drawing.Point(552, 10);
            this.timeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(27, 29);
            this.timeLabel.TabIndex = 3;
            this.timeLabel.Text = "_";
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.errorLabel.Location = new System.Drawing.Point(78, 9);
            this.errorLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(27, 29);
            this.errorLabel.TabIndex = 2;
            this.errorLabel.Text = "_";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Error:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.testConvButton);
            this.panel2.Controls.Add(this.getTrueResultsButton);
            this.panel2.Controls.Add(this.loadPhotoNetButton);
            this.panel2.Controls.Add(this.savePhotoNetButton);
            this.panel2.Controls.Add(this.photoResultLabel);
            this.panel2.Controls.Add(this.teachPhotoStopButton);
            this.panel2.Controls.Add(this.testPhotoButton);
            this.panel2.Controls.Add(this.TeachPhotoButton);
            this.panel2.Controls.Add(this.SavePictureBtn);
            this.panel2.Controls.Add(this.stopTeachButton);
            this.panel2.Controls.Add(this.restorePictureButton);
            this.panel2.Controls.Add(this.teachButton);
            this.panel2.Controls.Add(this.loadExpectedPictureButton);
            this.panel2.Controls.Add(this.loadActualPictureButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1136, 54);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(196, 770);
            this.panel2.TabIndex = 4;
            // 
            // testConvButton
            // 
            this.testConvButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.testConvButton.Location = new System.Drawing.Point(10, 509);
            this.testConvButton.Margin = new System.Windows.Forms.Padding(2);
            this.testConvButton.Name = "testConvButton";
            this.testConvButton.Size = new System.Drawing.Size(176, 38);
            this.testConvButton.TabIndex = 16;
            this.testConvButton.Text = "test conv";
            this.testConvButton.UseVisualStyleBackColor = true;
            this.testConvButton.Click += new System.EventHandler(this.TestConvButton_Click);
            // 
            // getTrueResultsButton
            // 
            this.getTrueResultsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.getTrueResultsButton.Location = new System.Drawing.Point(10, 320);
            this.getTrueResultsButton.Margin = new System.Windows.Forms.Padding(2);
            this.getTrueResultsButton.Name = "getTrueResultsButton";
            this.getTrueResultsButton.Size = new System.Drawing.Size(176, 38);
            this.getTrueResultsButton.TabIndex = 15;
            this.getTrueResultsButton.Text = "get true results";
            this.getTrueResultsButton.UseVisualStyleBackColor = true;
            this.getTrueResultsButton.Click += new System.EventHandler(this.GetTrueResultsButton_Click);
            // 
            // loadPhotoNetButton
            // 
            this.loadPhotoNetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadPhotoNetButton.Location = new System.Drawing.Point(10, 644);
            this.loadPhotoNetButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadPhotoNetButton.Name = "loadPhotoNetButton";
            this.loadPhotoNetButton.Size = new System.Drawing.Size(176, 38);
            this.loadPhotoNetButton.TabIndex = 14;
            this.loadPhotoNetButton.Text = "load net";
            this.loadPhotoNetButton.UseVisualStyleBackColor = true;
            this.loadPhotoNetButton.Click += new System.EventHandler(this.LoadPhotoNetButton_Click);
            // 
            // savePhotoNetButton
            // 
            this.savePhotoNetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.savePhotoNetButton.Location = new System.Drawing.Point(10, 602);
            this.savePhotoNetButton.Margin = new System.Windows.Forms.Padding(2);
            this.savePhotoNetButton.Name = "savePhotoNetButton";
            this.savePhotoNetButton.Size = new System.Drawing.Size(176, 38);
            this.savePhotoNetButton.TabIndex = 13;
            this.savePhotoNetButton.Text = "save photo";
            this.savePhotoNetButton.UseVisualStyleBackColor = true;
            this.savePhotoNetButton.Click += new System.EventHandler(this.SavePhotoNetButton_Click);
            // 
            // photoResultLabel
            // 
            this.photoResultLabel.AutoSize = true;
            this.photoResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.photoResultLabel.Location = new System.Drawing.Point(15, 558);
            this.photoResultLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.photoResultLabel.Name = "photoResultLabel";
            this.photoResultLabel.Size = new System.Drawing.Size(27, 29);
            this.photoResultLabel.TabIndex = 12;
            this.photoResultLabel.Text = "_";
            // 
            // teachPhotoStopButton
            // 
            this.teachPhotoStopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.teachPhotoStopButton.Location = new System.Drawing.Point(10, 414);
            this.teachPhotoStopButton.Margin = new System.Windows.Forms.Padding(2);
            this.teachPhotoStopButton.Name = "teachPhotoStopButton";
            this.teachPhotoStopButton.Size = new System.Drawing.Size(176, 38);
            this.teachPhotoStopButton.TabIndex = 11;
            this.teachPhotoStopButton.Text = "teach photo stop";
            this.teachPhotoStopButton.UseVisualStyleBackColor = true;
            this.teachPhotoStopButton.Click += new System.EventHandler(this.TeachPhotoStopButton_Click);
            // 
            // testPhotoButton
            // 
            this.testPhotoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.testPhotoButton.Location = new System.Drawing.Point(10, 467);
            this.testPhotoButton.Margin = new System.Windows.Forms.Padding(2);
            this.testPhotoButton.Name = "testPhotoButton";
            this.testPhotoButton.Size = new System.Drawing.Size(176, 38);
            this.testPhotoButton.TabIndex = 10;
            this.testPhotoButton.Text = "test photo";
            this.testPhotoButton.UseVisualStyleBackColor = true;
            this.testPhotoButton.Click += new System.EventHandler(this.TestPhotoButton_Click);
            // 
            // TeachPhotoButton
            // 
            this.TeachPhotoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TeachPhotoButton.Location = new System.Drawing.Point(10, 362);
            this.TeachPhotoButton.Margin = new System.Windows.Forms.Padding(2);
            this.TeachPhotoButton.Name = "TeachPhotoButton";
            this.TeachPhotoButton.Size = new System.Drawing.Size(176, 38);
            this.TeachPhotoButton.TabIndex = 9;
            this.TeachPhotoButton.Text = "teach photo";
            this.TeachPhotoButton.UseVisualStyleBackColor = true;
            this.TeachPhotoButton.Click += new System.EventHandler(this.TeachPhotoButton_Click);
            // 
            // SavePictureBtn
            // 
            this.SavePictureBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SavePictureBtn.Location = new System.Drawing.Point(10, 709);
            this.SavePictureBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SavePictureBtn.Name = "SavePictureBtn";
            this.SavePictureBtn.Size = new System.Drawing.Size(176, 38);
            this.SavePictureBtn.TabIndex = 8;
            this.SavePictureBtn.Text = "Save picture";
            this.SavePictureBtn.UseVisualStyleBackColor = true;
            this.SavePictureBtn.Click += new System.EventHandler(this.SavePictureBtn_Click);
            // 
            // stopTeachButton
            // 
            this.stopTeachButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.stopTeachButton.Location = new System.Drawing.Point(10, 168);
            this.stopTeachButton.Margin = new System.Windows.Forms.Padding(2);
            this.stopTeachButton.Name = "stopTeachButton";
            this.stopTeachButton.Size = new System.Drawing.Size(176, 38);
            this.stopTeachButton.TabIndex = 7;
            this.stopTeachButton.Text = "Stop teach";
            this.stopTeachButton.UseVisualStyleBackColor = true;
            this.stopTeachButton.Click += new System.EventHandler(this.stopTeachButton_Click);
            // 
            // restorePictureButton
            // 
            this.restorePictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.restorePictureButton.Location = new System.Drawing.Point(10, 225);
            this.restorePictureButton.Margin = new System.Windows.Forms.Padding(2);
            this.restorePictureButton.Name = "restorePictureButton";
            this.restorePictureButton.Size = new System.Drawing.Size(176, 38);
            this.restorePictureButton.TabIndex = 6;
            this.restorePictureButton.Text = "Restore image";
            this.restorePictureButton.UseVisualStyleBackColor = true;
            this.restorePictureButton.Click += new System.EventHandler(this.restorePictureButton_Click);
            // 
            // teachButton
            // 
            this.teachButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.teachButton.Location = new System.Drawing.Point(10, 117);
            this.teachButton.Margin = new System.Windows.Forms.Padding(2);
            this.teachButton.Name = "teachButton";
            this.teachButton.Size = new System.Drawing.Size(176, 38);
            this.teachButton.TabIndex = 5;
            this.teachButton.Text = "Run teach";
            this.teachButton.UseVisualStyleBackColor = true;
            this.teachButton.Click += new System.EventHandler(this.teachButton_Click);
            // 
            // loadExpectedPictureButton
            // 
            this.loadExpectedPictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadExpectedPictureButton.Location = new System.Drawing.Point(10, 64);
            this.loadExpectedPictureButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadExpectedPictureButton.Name = "loadExpectedPictureButton";
            this.loadExpectedPictureButton.Size = new System.Drawing.Size(176, 38);
            this.loadExpectedPictureButton.TabIndex = 4;
            this.loadExpectedPictureButton.Text = "Load expected picture";
            this.loadExpectedPictureButton.UseVisualStyleBackColor = true;
            this.loadExpectedPictureButton.Click += new System.EventHandler(this.loadExpectedPictureButton_Click);
            // 
            // loadActualPictureButton
            // 
            this.loadActualPictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadActualPictureButton.Location = new System.Drawing.Point(10, 12);
            this.loadActualPictureButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadActualPictureButton.Name = "loadActualPictureButton";
            this.loadActualPictureButton.Size = new System.Drawing.Size(176, 38);
            this.loadActualPictureButton.TabIndex = 3;
            this.loadActualPictureButton.Text = "Load actual picture";
            this.loadActualPictureButton.UseVisualStyleBackColor = true;
            this.loadActualPictureButton.Click += new System.EventHandler(this.loadActualPictureButton_Click);
            // 
            // learningRateNumericUpDown
            // 
            this.learningRateNumericUpDown.DecimalPlaces = 6;
            this.learningRateNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.learningRateNumericUpDown.Location = new System.Drawing.Point(1137, 3);
            this.learningRateNumericUpDown.Name = "learningRateNumericUpDown";
            this.learningRateNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.learningRateNumericUpDown.TabIndex = 5;
            this.learningRateNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // iterationLabel
            // 
            this.iterationLabel.AutoSize = true;
            this.iterationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.iterationLabel.Location = new System.Drawing.Point(862, 10);
            this.iterationLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.iterationLabel.Name = "iterationLabel";
            this.iterationLabel.Size = new System.Drawing.Size(27, 29);
            this.iterationLabel.TabIndex = 4;
            this.iterationLabel.Text = "0";
            // 
            // RestoreNetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 826);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RestoreNetForm";
            this.Text = "RestoreNetForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expectedPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualPicture)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.learningRateNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox expectedPicture;
        private System.Windows.Forms.PictureBox actualPicture;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button teachButton;
        private System.Windows.Forms.Button loadExpectedPictureButton;
        private System.Windows.Forms.Button loadActualPictureButton;
        private System.Windows.Forms.Button restorePictureButton;
        private System.Windows.Forms.Button stopTeachButton;
        private System.Windows.Forms.Button SavePictureBtn;
        private System.Windows.Forms.Button testPhotoButton;
        private System.Windows.Forms.Button TeachPhotoButton;
        private System.Windows.Forms.Button teachPhotoStopButton;
        private System.Windows.Forms.Label photoResultLabel;
        private System.Windows.Forms.Button savePhotoNetButton;
        private System.Windows.Forms.Button loadPhotoNetButton;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Button getTrueResultsButton;
        private System.Windows.Forms.Button testConvButton;
        private System.Windows.Forms.NumericUpDown learningRateNumericUpDown;
        private System.Windows.Forms.Label iterationLabel;
    }
}