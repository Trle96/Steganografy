using MathNet.Numerics;
using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplRad
{
    public abstract class Encryptor
    {
        internal string outputPath;
        internal int lsbUsed;
        internal SteganographyForm parentForm;

        internal EncryptionMessage encryptionMessage;

        public void SetEncryptionMessage(string encryptionMessagePath)
        {
            encryptionMessage = new EncryptionMessage(encryptionMessagePath, insertMessageSizeAtBeggining: true);
        }

        public enum ColorType
        {
            RED = 0,
            BLUE = 1,
            GREEN = 2,
        }

        public string EncryptPicture(string picturePath, string messagePath, string outputPath)
        {
            this.outputPath = outputPath;
            return EncryptPicture(picturePath, messagePath);
        }

        internal abstract string EncryptPicture(string picturePath, string messagePath);

        public abstract string DecryptPicture(string picturePath, string outputPath);


        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        internal virtual void SetLSBCount(int value)
        {
            lsbUsed = value;
        }

        internal virtual void SetParentForm(SteganographyForm parentForm)
        {
            this.parentForm = parentForm;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[409600];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public Encryptor(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public Encryptor() { 
        }

        internal double CalculateHiSqure(int[] histogram)
        {
            double x2 = 0;
            for (int i = 0; i < histogram.Length / 2; i++)
            {
                double avgg = (histogram[i * 2] + histogram[i * 2 + 1]) / 2.0;
                x2 += avgg < 1 ? 0 : (Math.Pow((avgg - histogram[i * 2]), 2)) / avgg;
            }

            return x2;
        }

        internal double CalculateProbabilityAtPoint(int[] histogram)
        {
            double chiSquare = CalculateHiSqure(histogram);
            double temp = (1 - SimpsonRule.IntegrateComposite(x => (Math.Pow(x, 62.5) * Math.Pow(Math.E, (-x / 2.0))) / (Math.Pow(2, 63.5) * SpecialFunctions.Gamma(63.5)), 0, chiSquare, 10000));
            temp = temp < 0.01 ? 0 : temp;
            temp = temp > 0.99 ? 1 : temp;
            return temp;
        }

        public virtual void PopulateChart(Chart chart, Chart PercentChart, string bmPath, out double max)
        {
            max = 0;
            var test = new Bitmap(bmPath);
            int[] histogram_r = new int[256];
            int[] histogram_b = new int[256];
            int[] histogram_g = new int[256];

            double percent = 1;
            double textPercent = test.Width / 100.0;
            for (int i = 0; i < test.Width; i++)
            {
                for (int j = 0; j < test.Height; j++)
                {
                    histogram_r[test.GetPixel(i, j).R]++;
                    histogram_b[test.GetPixel(i, j).B]++;
                    histogram_g[test.GetPixel(i, j).G]++;
                }

                if (i >= textPercent)
                {
                    double probability = CalculateProbabilityAtPoint(histogram_r);
                    PercentChart.Series["RedProbability"].Points.AddXY(percent, probability * 100);

                    probability = CalculateProbabilityAtPoint(histogram_b);
                    PercentChart.Series["BlueProbability"].Points.AddXY(percent, probability * 100);

                    probability = CalculateProbabilityAtPoint(histogram_g);
                    PercentChart.Series["GreenProbability"].Points.AddXY(percent, probability * 100);
                    textPercent += test.Width / 100.0;
                    percent += 1;
                }
            }

            for (int i = 0; i < histogram_r.Length; i++)
            {
                chart.Series["RED"].Points.AddXY(i, histogram_r[i]);
                chart.Series["BLUE"].Points.AddXY(i, histogram_b[i]);
                chart.Series["GREEN"].Points.AddXY(i, histogram_g[i]);
                max = Math.Max(max, histogram_r[i]);
                max = Math.Max(max, histogram_b[i]);
                max = Math.Max(max, histogram_g[i]);
            }
        }
    }
}
