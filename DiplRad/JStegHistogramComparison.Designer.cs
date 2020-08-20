namespace DiplRad
{
    partial class JStegHistogramComparison
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea13 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend13 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series37 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series38 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series39 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title13 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea14 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend14 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series40 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series41 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series42 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title14 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea15 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend15 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series43 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series44 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series45 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title15 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea16 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend16 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series46 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series47 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series48 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title16 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Y = new System.Windows.Forms.CheckBox();
            this.Cr = new System.Windows.Forms.CheckBox();
            this.Cb = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea13.AxisX.Interval = 16D;
            chartArea13.AxisX.Maximum = 128D;
            chartArea13.AxisX.Minimum = -128D;
            chartArea13.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea13);
            this.chart1.Cursor = System.Windows.Forms.Cursors.Default;
            legend13.Name = "Legend1";
            this.chart1.Legends.Add(legend13);
            this.chart1.Location = new System.Drawing.Point(0, -2);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series37.ChartArea = "ChartArea1";
            series37.Color = System.Drawing.Color.Blue;
            series37.CustomProperties = "PointWidth=1.5";
            series37.Legend = "Legend1";
            series37.Name = "Cb";
            series38.ChartArea = "ChartArea1";
            series38.Color = System.Drawing.Color.Red;
            series38.CustomProperties = "PointWidth=1.5";
            series38.Legend = "Legend1";
            series38.Name = "Cr";
            series39.ChartArea = "ChartArea1";
            series39.Color = System.Drawing.Color.Gray;
            series39.CustomProperties = "PointWidth=1.5";
            series39.Legend = "Legend1";
            series39.Name = "Y";
            this.chart1.Series.Add(series37);
            this.chart1.Series.Add(series38);
            this.chart1.Series.Add(series39);
            this.chart1.Size = new System.Drawing.Size(767, 387);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "Original Image Histogram";
            title13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            title13.Name = "Original image histogram";
            title13.Text = "Original image histogram";
            this.chart1.Titles.Add(title13);
            // 
            // chart2
            // 
            chartArea14.AxisX.Interval = 16D;
            chartArea14.AxisX.Maximum = 128D;
            chartArea14.AxisX.Minimum = -128D;
            chartArea14.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea14);
            this.chart2.Cursor = System.Windows.Forms.Cursors.Default;
            legend14.Name = "Legend1";
            this.chart2.Legends.Add(legend14);
            this.chart2.Location = new System.Drawing.Point(0, 391);
            this.chart2.Name = "chart2";
            series40.ChartArea = "ChartArea1";
            series40.Color = System.Drawing.Color.Blue;
            series40.CustomProperties = "PointWidth=1.5";
            series40.Legend = "Legend1";
            series40.Name = "Cb";
            series41.ChartArea = "ChartArea1";
            series41.Color = System.Drawing.Color.Red;
            series41.CustomProperties = "PointWidth=1.5";
            series41.Legend = "Legend1";
            series41.Name = "Cr";
            series42.ChartArea = "ChartArea1";
            series42.Color = System.Drawing.Color.Gray;
            series42.CustomProperties = "PointWidth=1.5";
            series42.Legend = "Legend1";
            series42.Name = "Y";
            this.chart2.Series.Add(series40);
            this.chart2.Series.Add(series41);
            this.chart2.Series.Add(series42);
            this.chart2.Size = new System.Drawing.Size(767, 387);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            title14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            title14.Name = "Stego image histogram";
            title14.Text = "Stego image histogram";
            this.chart2.Titles.Add(title14);
            // 
            // chart3
            // 
            chartArea15.AxisX.Interval = 10D;
            chartArea15.AxisX.Maximum = 100D;
            chartArea15.AxisX.Minimum = 0D;
            chartArea15.AxisY.LabelStyle.Format = "{0}%";
            chartArea15.AxisY.Maximum = 100D;
            chartArea15.AxisY.Minimum = 0D;
            chartArea15.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea15);
            this.chart3.Cursor = System.Windows.Forms.Cursors.Default;
            legend15.Name = "Legend1";
            this.chart3.Legends.Add(legend15);
            this.chart3.Location = new System.Drawing.Point(773, -2);
            this.chart3.Name = "chart3";
            series43.BorderWidth = 2;
            series43.ChartArea = "ChartArea1";
            series43.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series43.Color = System.Drawing.Color.Red;
            series43.Legend = "Legend1";
            series43.Name = "Cr";
            series44.BorderWidth = 2;
            series44.ChartArea = "ChartArea1";
            series44.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series44.Color = System.Drawing.Color.Blue;
            series44.Legend = "Legend1";
            series44.Name = "Cb";
            series45.BorderWidth = 2;
            series45.ChartArea = "ChartArea1";
            series45.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series45.Color = System.Drawing.Color.Green;
            series45.Legend = "Legend1";
            series45.Name = "Y";
            this.chart3.Series.Add(series43);
            this.chart3.Series.Add(series44);
            this.chart3.Series.Add(series45);
            this.chart3.Size = new System.Drawing.Size(767, 387);
            this.chart3.TabIndex = 2;
            this.chart3.Text = "chart3";
            title15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            title15.Name = "Probability of hidden message in original image";
            title15.Text = "Probability of hidden message in original image";
            this.chart3.Titles.Add(title15);
            // 
            // chart4
            // 
            chartArea16.AxisX.Interval = 10D;
            chartArea16.AxisX.Maximum = 100D;
            chartArea16.AxisX.Minimum = 0D;
            chartArea16.AxisY.LabelStyle.Format = "{0}%";
            chartArea16.AxisY.Maximum = 100D;
            chartArea16.AxisY.Minimum = 0D;
            chartArea16.Name = "ChartArea1";
            this.chart4.ChartAreas.Add(chartArea16);
            this.chart4.Cursor = System.Windows.Forms.Cursors.Default;
            legend16.Name = "Legend1";
            this.chart4.Legends.Add(legend16);
            this.chart4.Location = new System.Drawing.Point(773, 391);
            this.chart4.Name = "chart4";
            series46.BorderWidth = 2;
            series46.ChartArea = "ChartArea1";
            series46.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series46.Color = System.Drawing.Color.Red;
            series46.Legend = "Legend1";
            series46.Name = "Cr";
            series47.BorderWidth = 2;
            series47.ChartArea = "ChartArea1";
            series47.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series47.Color = System.Drawing.Color.Blue;
            series47.Legend = "Legend1";
            series47.Name = "Cb";
            series48.BorderWidth = 2;
            series48.ChartArea = "ChartArea1";
            series48.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series48.Color = System.Drawing.Color.Green;
            series48.Legend = "Legend1";
            series48.Name = "Y";
            this.chart4.Series.Add(series46);
            this.chart4.Series.Add(series47);
            this.chart4.Series.Add(series48);
            this.chart4.Size = new System.Drawing.Size(767, 387);
            this.chart4.TabIndex = 3;
            this.chart4.Text = "chart4";
            title16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            title16.Name = "Probability of hidden message in stego image";
            title16.Text = "Probability of hidden message in stego image";
            this.chart4.Titles.Add(title16);
            // 
            // Y
            // 
            this.Y.AutoSize = true;
            this.Y.Checked = true;
            this.Y.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Y.Location = new System.Drawing.Point(12, 784);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(63, 17);
            this.Y.TabIndex = 4;
            this.Y.Text = "Show Y";
            this.Y.UseVisualStyleBackColor = true;
            this.Y.CheckedChanged += new System.EventHandler(this.Y_CheckedChanged);
            // 
            // Cr
            // 
            this.Cr.AutoSize = true;
            this.Cr.Checked = true;
            this.Cr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Cr.Location = new System.Drawing.Point(81, 784);
            this.Cr.Name = "Cr";
            this.Cr.Size = new System.Drawing.Size(66, 17);
            this.Cr.TabIndex = 5;
            this.Cr.Text = "Show Cr";
            this.Cr.UseVisualStyleBackColor = true;
            this.Cr.CheckedChanged += new System.EventHandler(this.Cr_CheckedChanged);
            // 
            // Cb
            // 
            this.Cb.AutoSize = true;
            this.Cb.Checked = true;
            this.Cb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Cb.Location = new System.Drawing.Point(150, 784);
            this.Cb.Name = "Cb";
            this.Cb.Size = new System.Drawing.Size(69, 17);
            this.Cb.TabIndex = 6;
            this.Cb.Text = "Show Cb";
            this.Cb.UseVisualStyleBackColor = true;
            this.Cb.CheckedChanged += new System.EventHandler(this.Cb_CheckedChanged);
            // 
            // JStegHistogramComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1522, 823);
            this.Controls.Add(this.Cb);
            this.Controls.Add(this.Cr);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.chart4);
            this.Controls.Add(this.chart3);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Name = "JStegHistogramComparison";
            this.Text = "HistogramComparison";
            this.Disposed += new System.EventHandler(this.HistogramComparison_Dispose);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart4;
        private System.Windows.Forms.CheckBox Y;
        private System.Windows.Forms.CheckBox Cr;
        private System.Windows.Forms.CheckBox Cb;
    }
}

