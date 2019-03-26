using ScannerNet.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sobel.UI
{
    public partial class RestoreNetForm : Form
    {
        private RestoredNet net = new RestoredNet();
        
        public RestoreNetForm()
        {
            InitializeComponent();
            net.InitNet();
            net.EventHandler += ShowLog;
        }

        private void teachButton_Click(object sender, EventArgs e)
        {
            var actualPicture = (Bitmap)this.actualPicture.Image;
            var expectedPicture = (Bitmap)this.expectedPicture.Image;
            
            Task.Factory.StartNew(() => { net.Learn(actualPicture, expectedPicture, 0.02f); });
        }

        private void ShowLog(object sender, double error)
        {
            errorLabel.Text = error.ToString(CultureInfo.InvariantCulture);
        }

        private void loadActualPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bit = new Bitmap(open.FileName);
                    actualPicture.Image = bit;
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

        private Bitmap result = null;
        
        private void restorePictureButton_Click(object sender, EventArgs e)
        {
            var image = (Bitmap)this.actualPicture.Image;

//            this.actualPicture.Image = null;
            result = net.Restore(image);
            this.actualPicture.Image = result;
        }

        private void stopTeachButton_Click(object sender, EventArgs e)
        {
            net.Running = false;
        }

        private void toBppButtom_Click(object sender, EventArgs e)
        {
            this.actualPicture.Image = ((Bitmap)this.actualPicture.Image.Clone()).To1bpp2(3, 0);
        }

        private void saveResultPictureButton_Click(object sender, EventArgs e)
        {
            var open = new SaveFileDialog();
            open.Filter = "Image File(*.png)|*.png";

            if (result != null && open.ShowDialog() == DialogResult.OK)
            {
                result.Save(open.FileName);
            }
        }
    }
}
