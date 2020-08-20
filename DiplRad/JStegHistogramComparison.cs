using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.Integration;
using MathNet.Numerics.Distributions;
using System.Numerics;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplRad
{
    public partial class JStegHistogramComparison : Form
    {
        private string stegoImage;
        private string coverImage;
        private Form prevForm;

        public JStegHistogramComparison(Form prevForm, string stegoImage, string coverImage, Encryptor encryptor)
        {
            this.stegoImage = stegoImage;
            this.coverImage = coverImage;
            this.prevForm = prevForm;
            InitializeComponent();
            double max;
            encryptor.PopulateChart(this.chart1, this.chart3, stegoImage, out max);
            encryptor.PopulateChart(this.chart2, this.chart4, coverImage, out max);

            this.chart1.ChartAreas[0].AxisY.Maximum = max;
            this.chart2.ChartAreas[0].AxisY.Maximum = max;
        }

        private void HistogramComparison_Dispose(object sender, EventArgs e)
        {
            prevForm.Location = this.Location;
            prevForm.Show();
            this.Dispose();
        }

        private void Y_CheckedChanged(object sender, EventArgs e)
        {
            this.chart1.Series["Y"].Enabled = Y.Checked;
            this.chart2.Series["Y"].Enabled = Y.Checked;
            this.chart3.Series["Y"].Enabled = Y.Checked;
            this.chart4.Series["Y"].Enabled = Y.Checked;
        }

        private void Cr_CheckedChanged(object sender, EventArgs e)
        {
            this.chart1.Series["Cr"].Enabled = Cr.Checked;
            this.chart2.Series["Cr"].Enabled = Cr.Checked;
            this.chart3.Series["Cr"].Enabled = Cr.Checked;
            this.chart4.Series["Cr"].Enabled = Cr.Checked;
        }

        private void Cb_CheckedChanged(object sender, EventArgs e)
        {
            this.chart1.Series["Cb"].Enabled = Cb.Checked;
            this.chart2.Series["Cb"].Enabled = Cb.Checked;
            this.chart3.Series["Cb"].Enabled = Cb.Checked;
            this.chart4.Series["Cb"].Enabled = Cb.Checked;
        }
    }
}
