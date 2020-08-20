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
    public partial class HistogramComparison : Form
    {
        private string stegoImage;
        private string coverImage;
        private Form prevForm;

        public HistogramComparison(Form prevForm, string stegoImage, string coverImage, Encryptor encryptor)
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
    }
}
