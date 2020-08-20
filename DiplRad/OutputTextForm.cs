using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplRad
{
    public partial class OutputTextForm : Form
    {
        private Form prevForm;
        string inputPath;

        public OutputTextForm(Form parentForm, string inputPath)
        {
            InitializeComponent();
            this.inputPath = inputPath;
            this.prevForm = parentForm;
            richTextBox1.Text = File.ReadAllText(inputPath);
        }

        private void OutputTextForm_Dispose(object sender, EventArgs e)
        {
            prevForm.Location = this.Location;
            prevForm.Show();
            this.Dispose();
        }
    }
}
