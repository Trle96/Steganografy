using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplRad
{
    public partial class ImageComparison : Form
    {
        private string coverImagePath;
        private string stegoImagePath;
        private Form prevForm;

        public ImageComparison(Form prevForm, string coverPath, string stegoPath)
        {
            this.prevForm = prevForm;
            coverImagePath = coverPath;
            stegoImagePath = stegoPath;
            InitializeComponent();
        }

        private void ImageComparison_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = coverImagePath;
            pictureBox2.ImageLocation = stegoImagePath;
            pictureBox1.Load();
            pictureBox2.Load();
        }

        private void ImageComparison_Dispose(object sender, EventArgs e)
        {
            prevForm.Location = this.Location;
            prevForm.Show();
            this.Dispose();
        }
    }
}
