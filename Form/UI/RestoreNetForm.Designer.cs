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
            this.errorLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.stopTeachButton = new System.Windows.Forms.Button();
            this.restorePictureButton = new System.Windows.Forms.Button();
            this.teachButton = new System.Windows.Forms.Button();
            this.loadExpectedPictureButton = new System.Windows.Forms.Button();
            this.loadActualPictureButton = new System.Windows.Forms.Button();
            this.toBppButtom = new System.Windows.Forms.Button();
            this.saveResultPictureButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expectedPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualPicture)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(2668, 1574);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 103);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(2262, 1468);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // expectedPicture
            // 
            this.expectedPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.expectedPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expectedPicture.Location = new System.Drawing.Point(1134, 3);
            this.expectedPicture.Name = "expectedPicture";
            this.expectedPicture.Size = new System.Drawing.Size(1125, 1462);
            this.expectedPicture.TabIndex = 1;
            this.expectedPicture.TabStop = false;
            // 
            // actualPicture
            // 
            this.actualPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actualPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actualPicture.Location = new System.Drawing.Point(3, 3);
            this.actualPicture.Name = "actualPicture";
            this.actualPicture.Size = new System.Drawing.Size(1125, 1462);
            this.actualPicture.TabIndex = 0;
            this.actualPicture.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.errorLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2262, 94);
            this.panel1.TabIndex = 2;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.errorLabel.Location = new System.Drawing.Point(156, 18);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(51, 55);
            this.errorLabel.TabIndex = 2;
            this.errorLabel.Text = "_";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "Error:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.saveResultPictureButton);
            this.panel2.Controls.Add(this.toBppButtom);
            this.panel2.Controls.Add(this.stopTeachButton);
            this.panel2.Controls.Add(this.restorePictureButton);
            this.panel2.Controls.Add(this.teachButton);
            this.panel2.Controls.Add(this.loadExpectedPictureButton);
            this.panel2.Controls.Add(this.loadActualPictureButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(2271, 103);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(394, 1468);
            this.panel2.TabIndex = 4;
            // 
            // stopTeachButton
            // 
            this.stopTeachButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.stopTeachButton.Location = new System.Drawing.Point(21, 323);
            this.stopTeachButton.Name = "stopTeachButton";
            this.stopTeachButton.Size = new System.Drawing.Size(352, 74);
            this.stopTeachButton.TabIndex = 7;
            this.stopTeachButton.Text = "Stop teach";
            this.stopTeachButton.UseVisualStyleBackColor = true;
            this.stopTeachButton.Click += new System.EventHandler(this.stopTeachButton_Click);
            // 
            // restorePictureButton
            // 
            this.restorePictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.restorePictureButton.Location = new System.Drawing.Point(21, 432);
            this.restorePictureButton.Name = "restorePictureButton";
            this.restorePictureButton.Size = new System.Drawing.Size(352, 74);
            this.restorePictureButton.TabIndex = 6;
            this.restorePictureButton.Text = "Restore image";
            this.restorePictureButton.UseVisualStyleBackColor = true;
            this.restorePictureButton.Click += new System.EventHandler(this.restorePictureButton_Click);
            // 
            // teachButton
            // 
            this.teachButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.teachButton.Location = new System.Drawing.Point(21, 225);
            this.teachButton.Name = "teachButton";
            this.teachButton.Size = new System.Drawing.Size(352, 74);
            this.teachButton.TabIndex = 5;
            this.teachButton.Text = "Run teach";
            this.teachButton.UseVisualStyleBackColor = true;
            this.teachButton.Click += new System.EventHandler(this.teachButton_Click);
            // 
            // loadExpectedPictureButton
            // 
            this.loadExpectedPictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadExpectedPictureButton.Location = new System.Drawing.Point(21, 124);
            this.loadExpectedPictureButton.Name = "loadExpectedPictureButton";
            this.loadExpectedPictureButton.Size = new System.Drawing.Size(352, 74);
            this.loadExpectedPictureButton.TabIndex = 4;
            this.loadExpectedPictureButton.Text = "Load expected picture";
            this.loadExpectedPictureButton.UseVisualStyleBackColor = true;
            this.loadExpectedPictureButton.Click += new System.EventHandler(this.loadExpectedPictureButton_Click);
            // 
            // loadActualPictureButton
            // 
            this.loadActualPictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadActualPictureButton.Location = new System.Drawing.Point(21, 23);
            this.loadActualPictureButton.Name = "loadActualPictureButton";
            this.loadActualPictureButton.Size = new System.Drawing.Size(352, 74);
            this.loadActualPictureButton.TabIndex = 3;
            this.loadActualPictureButton.Text = "Load actual picture";
            this.loadActualPictureButton.UseVisualStyleBackColor = true;
            this.loadActualPictureButton.Click += new System.EventHandler(this.loadActualPictureButton_Click);
            // 
            // toBppButtom
            // 
            this.toBppButtom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toBppButtom.Location = new System.Drawing.Point(21, 697);
            this.toBppButtom.Name = "toBppButtom";
            this.toBppButtom.Size = new System.Drawing.Size(352, 74);
            this.toBppButtom.TabIndex = 8;
            this.toBppButtom.Text = "ToBpp";
            this.toBppButtom.UseVisualStyleBackColor = true;
            this.toBppButtom.Click += new System.EventHandler(this.toBppButtom_Click);
            // 
            // saveResultPictureButton
            // 
            this.saveResultPictureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveResultPictureButton.Location = new System.Drawing.Point(21, 797);
            this.saveResultPictureButton.Name = "saveResultPictureButton";
            this.saveResultPictureButton.Size = new System.Drawing.Size(352, 74);
            this.saveResultPictureButton.TabIndex = 9;
            this.saveResultPictureButton.Text = "Save result picture";
            this.saveResultPictureButton.UseVisualStyleBackColor = true;
            this.saveResultPictureButton.Click += new System.EventHandler(this.saveResultPictureButton_Click);
            // 
            // RestoreNetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2668, 1574);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RestoreNetForm";
            this.Text = "RestoreNetForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expectedPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualPicture)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
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
        private System.Windows.Forms.Button toBppButtom;
        private System.Windows.Forms.Button saveResultPictureButton;
    }
}