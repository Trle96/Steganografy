using MathNet.Numerics;
using MathNet.Numerics.Integration;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplRad
{
    public abstract class Encryptor
    {
        public enum ColorType
        {
            RED = 0,
            BLUE = 1,
            GREEN = 2,
        }

        internal int lsbUsed;
        internal SteganographyForm parentForm;
        internal StegoMessage stegoMessage;
        internal IOPaths ioPaths;

        internal virtual void SetStegoMessage(string messagePath, StegoOptions stegoOptions)
        {
            stegoMessage = new StegoMessage(messagePath, stegoOptions);
        }

        internal string GenerateEncryptionOutputFileName()
        {
            var outputFileName = String.Format("{0}\\{1}EncryptionTest_{2}.{3}", ioPaths.OutputPath, GetAlgorithmName(), DateTime.UtcNow.ToFileTimeUtc(), GetEncryptionOutputFileExtension()); ;
            return outputFileName;
        }

        internal string GenerateDecryptionOutput(List<Boolean> outputMessage, StegoOptions stegoOptions)
        {
            StegoMessage encryptionMessage = new StegoMessage(outputMessage, stegoOptions);
            var outputFileName = String.Format("{0}\\{1}DecryptionTest_{2}.txt", ioPaths.OutputPath, GetAlgorithmName(), DateTime.UtcNow.ToFileTimeUtc()); ;
            File.WriteAllText(outputFileName, encryptionMessage.GenerateOutput());
            return outputFileName;
        }

        internal abstract string GetAlgorithmName();

        internal virtual string GetEncryptionOutputFileExtension()
        {
            return "png";
        }

        internal virtual void SetParentForm(SteganographyForm parentForm)
        {
            this.parentForm = parentForm;
        }

        internal double CalculateHiSqure(int[] histogram, out int numOfCategories)
        {
            double x2 = 0;
            numOfCategories = 128;
            for (int i = 0; i < histogram.Length / 2; i++)
            {
                if (histogram[i * 2] + histogram[i * 2 + 1] > 4)
                {
                    double avgg = (histogram[i * 2] + histogram[i * 2 + 1]) / 2.0;
                    x2 += (Math.Pow((avgg - histogram[i * 2]), 2)) / avgg;
                }
                else
                {
                    numOfCategories -= 1;
                }
            }

            return x2;
        }

        internal double CalculateProbabilityAtPoint(int[] histogram)
        {
            int numOfCategories;
            double chiSquare = CalculateHiSqure(histogram, out numOfCategories);
            double temp = (1 - DoubleExponentialTransformation.Integrate(x => ChiSquared.PDF(numOfCategories, x), 0, chiSquare, 1e-5));
            temp = temp < 0.01 ? 0 : temp;
            temp = temp > 0.99 ? 1 : temp;
            return temp;
        }

        internal abstract string EncryptPicture();

        internal abstract string DecryptPicture(StegoOptions stegoOptions);

        #region PublicSurface
        public string EncryptPicture(IOPaths ioPaths, StegoOptions stegoOptions)
        {
            this.ioPaths = ioPaths;
            SetStegoMessage(ioPaths.MessagePath, stegoOptions);
            lsbUsed = stegoOptions.LsbUsed;
            return EncryptPicture();
        }

        public string DecryptPicture(IOPaths ioPaths, StegoOptions stegoOptions)
        {
            this.ioPaths = ioPaths;
            return DecryptPicture(stegoOptions);
        }

        public abstract void PopulateChart(Chart chart, Chart PercentChart, string bmPath, out double max);
        #endregion

    }
}
