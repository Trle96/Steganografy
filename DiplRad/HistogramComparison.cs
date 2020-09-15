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

        private void Red_CheckedChanged(object sender, EventArgs e)
        {
            None_CheckedChanged(sender, e);
            this.chart1.Series["RED"].Enabled = true;
            this.chart2.Series["RED"].Enabled = true;
            this.chart3.Series["RedProbability"].Enabled = true;
            this.chart4.Series["RedProbability"].Enabled = true;
        }

        private void Blue_CheckedChanged(object sender, EventArgs e)
        {
            None_CheckedChanged(sender, e);
            this.chart1.Series["BLUE"].Enabled = true;
            this.chart2.Series["BLUE"].Enabled = true;
            this.chart3.Series["BlueProbability"].Enabled = true;
            this.chart4.Series["BlueProbability"].Enabled = true;
        }

        private void Green_CheckedChanged(object sender, EventArgs e)
        {
            None_CheckedChanged(sender, e);
            this.chart1.Series["GREEN"].Enabled = true;
            this.chart2.Series["GREEN"].Enabled = true;
            this.chart3.Series["GreenProbability"].Enabled = true;
            this.chart4.Series["GreenProbability"].Enabled = true;
        }
        private void None_CheckedChanged(object sender, EventArgs e)
        {
            this.chart1.Series["RED"].Enabled = false;
            this.chart2.Series["RED"].Enabled = false;
            this.chart3.Series["RedProbability"].Enabled = false;
            this.chart4.Series["RedProbability"].Enabled = false;
            this.chart1.Series["BLUE"].Enabled = false;
            this.chart2.Series["BLUE"].Enabled = false;
            this.chart3.Series["BlueProbability"].Enabled = false;
            this.chart4.Series["BlueProbability"].Enabled = false;
            this.chart1.Series["GREEN"].Enabled = false;
            this.chart2.Series["GREEN"].Enabled = false;
            this.chart3.Series["GreenProbability"].Enabled = false;
            this.chart4.Series["GreenProbability"].Enabled = false;
        }

        private void All_CheckedChanged(object sender, EventArgs e)
        {
            this.chart1.Series["RED"].Enabled = true;
            this.chart2.Series["RED"].Enabled = true;
            this.chart3.Series["RedProbability"].Enabled = true;
            this.chart4.Series["RedProbability"].Enabled = true;
            this.chart1.Series["BLUE"].Enabled = true;
            this.chart2.Series["BLUE"].Enabled = true;
            this.chart3.Series["BlueProbability"].Enabled = true;
            this.chart4.Series["BlueProbability"].Enabled = true;
            this.chart1.Series["GREEN"].Enabled = true;
            this.chart2.Series["GREEN"].Enabled = true;
            this.chart3.Series["GreenProbability"].Enabled = true;
            this.chart4.Series["GreenProbability"].Enabled = true;
        }
    }
}
