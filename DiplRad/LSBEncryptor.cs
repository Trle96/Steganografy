using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplRad
{
    class LSBEncryptor : Encryptor
    {
        #region Fields
        internal Bitmap picture;

        internal int currentXCoord = 0;
        internal int currentYCoord = 0;

        internal int newRedColor = 0;
        internal int newBlueColor = 0;
        internal int newGreenColor = 0;

        #endregion Fields
        #region Methods

        public LSBEncryptor()
        {
        }

        internal void Reset()
        {
            currentXCoord = 0;
            currentYCoord = 0;
        }

        internal override string GetAlgorithmName()
        {
            return "BasicLSB";
        }

        //
        // Calculates image capacity based on image size and 
        //
        internal uint CalculateCapacity()
        {
            return (uint)(picture.Height * picture.Width * lsbUsed * 3);
        }

        //
        // Sets new color for current pixel based on newRedColor, newBlueColor, newGreenColor
        //
        internal void SetCurrentPixel()
        {
            picture.SetPixel(currentXCoord, currentYCoord, Color.FromArgb(newRedColor, newGreenColor, newBlueColor));
        }

        //
        // Gets current pixel for alteration.
        //
        internal Color GetCurrentPixel()
        {
            return picture.GetPixel(currentXCoord, currentYCoord);
        }

        //
        // Iterate to next pixel.
        //
        internal void IterateToNextPixel()
        {
            currentYCoord++;
            if (currentYCoord == picture.Height)
            {
                currentXCoord += 1;
                currentYCoord = 0;
            }
        }

        //
        // Gets pixel color value for given colorType.
        //
        internal byte GetPixelColor(Color pixel, ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.RED:
                    return pixel.R;
                case ColorType.BLUE:
                    return pixel.B;
                case ColorType.GREEN:
                default:
                    return pixel.G;
            }
        }

        //
        // Main function used for encryption
        //
        internal override string EncryptPicture()
        {
            Reset();

            picture = new Bitmap(ioPaths.ImagePath);
            uint imageCapacity = CalculateCapacity();
            uint pixelCount = (uint)(picture.Width * picture.Height);
            bool loopedMessage = stegoMessage.IsLooped();

            double cachedLimiter;
            if (!loopedMessage)
            {
                cachedLimiter = stegoMessage.GetTotalBitCount() / 20.0;
            }
            else
            {
                cachedLimiter = pixelCount / 20.0;
            }

            if (imageCapacity < stegoMessage.GetTotalBitCount())
            {
                throw new Exception("Image capacity is lower then message size");
            }

            for (uint pixelsProcessed = 1; pixelsProcessed <= pixelCount && !stegoMessage.IsWholeMessageEncoded(); pixelsProcessed++)
            {
                Color currentPixel = GetCurrentPixel();

                foreach (ColorType colorType in Enum.GetValues(typeof(ColorType)))
                {
                    int currentColor = GetPixelColor(currentPixel, colorType);
                    byte mask = (byte)(0xFF << lsbUsed);
                    currentColor = (byte)(currentColor & mask);

                    var colorModification = 0;
                    for (int i = 0; i < lsbUsed && !stegoMessage.IsWholeMessageEncoded(); i++)
                    {
                        colorModification <<= 1;
                        colorModification += stegoMessage.GetNextStegoMessageBit();
                    }

                    switch (colorType)
                    {
                        case ColorType.RED:
                            newRedColor = currentColor | colorModification;
                            break;
                        case ColorType.BLUE:
                            newBlueColor = currentColor | colorModification;
                            break;
                        case ColorType.GREEN:
                            newGreenColor = currentColor | colorModification;
                            break;
                    }
                }

                SetCurrentPixel();
                IterateToNextPixel();
                if (!loopedMessage && stegoMessage.GetCurrentBit() > cachedLimiter)
                {
                    cachedLimiter += stegoMessage.GetTotalBitCount() / 20.0;
                    parentForm.SetCurrentProgress((int)Math.Round(stegoMessage.GetCurrentBit() / (stegoMessage.GetTotalBitCount() / 100.0)));
                }
                else if (loopedMessage && pixelsProcessed > cachedLimiter)
                {

                    cachedLimiter += pixelCount / 20.0;
                    parentForm.SetCurrentProgress((int)Math.Round(pixelsProcessed / (pixelCount / 100.0)));
                }
            }

            parentForm.SetCurrentProgress(100);
            var outputFileName = GenerateEncryptionOutputFileName();
            picture.Save(outputFileName);

            return outputFileName;
        }

        //
        // Main function used for decryption.
        //
        internal override string DecryptPicture(StegoOptions stegoOptions)
        {
            Reset();
            lsbUsed = stegoOptions.LsbUsed;
            bool messageSizeFound = false;
            uint messageSize = 0;
            List<Boolean> messageInBits = new List<Boolean>();
            picture = new Bitmap(ioPaths.ImagePath);
            uint pixelCount = (uint)(picture.Width * picture.Height);
            double cachedLimiter = 0;

            for (int pixelsProcessed = 0; pixelsProcessed < pixelCount && (!messageSizeFound || messageSize > messageInBits.Count / 8); pixelsProcessed++)
            {
                Color currentPixel = GetCurrentPixel();

                if (!messageSizeFound && messageInBits.Count >= 32)
                {
                    messageSize = Helper.GetMessageSizeFromBoolArray(messageInBits);
                    messageInBits.RemoveRange(0, 32);
                    messageSizeFound = true;
                    cachedLimiter = messageSize * 8 / 20.0;
                }

                foreach (ColorType colorType in Enum.GetValues(typeof(ColorType)))
                {
                    int currentColor = GetPixelColor(currentPixel, colorType);
                    var mask = 0x01 << (lsbUsed - 1);
                    for (int i = 0; i < lsbUsed && (!messageSizeFound || messageSize > messageInBits.Count / 8); i++)
                    {
                        messageInBits.Add((currentColor & mask) != 0);
                        mask >>= 1;
                    }
                }

                IterateToNextPixel();
                if (messageSizeFound && messageInBits.Count >= cachedLimiter)
                {
                    cachedLimiter += messageSize * 8 / 20.0;
                    parentForm.SetCurrentProgress((int)Math.Round(messageInBits.Count / (messageSize * 8 / 100.0)));
                }
            }

            parentForm.SetCurrentProgress(100);
            return GenerateDecryptionOutput(messageInBits, stegoOptions);
        }

        public override void PopulateChart(Chart chart, Chart PercentChart, string bmPath, out double max)
        {
            Reset();
            max = 0;
            picture = new Bitmap(bmPath);
            int[] histogram_r = new int[256];
            int[] histogram_b = new int[256];
            int[] histogram_g = new int[256];

            int percent = 0;
            uint pixelCount = (uint)(picture.Width * picture.Height);
            double textPercent = pixelCount / 100.0;
            for (int pixelsProcessed = 0; pixelsProcessed < pixelCount; pixelsProcessed++)
            {
                Color currentPixel = GetCurrentPixel();

                histogram_r[currentPixel.R]++;
                histogram_b[currentPixel.B]++;
                histogram_g[currentPixel.G]++;

                IterateToNextPixel();

                if (pixelsProcessed >= textPercent)
                {
                    double probability = CalculateProbabilityAtPoint(histogram_r);
                    PercentChart.Series["RedProbability"].Points.AddXY(percent, probability * 100);

                    probability = CalculateProbabilityAtPoint(histogram_b);
                    PercentChart.Series["BlueProbability"].Points.AddXY(percent, probability * 100);

                    probability = CalculateProbabilityAtPoint(histogram_g);
                    PercentChart.Series["GreenProbability"].Points.AddXY(percent, probability * 100);

                    percent++;
                    textPercent += pixelCount / 100.0;
                    parentForm.SetCurrentProgress(percent);
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
        #endregion Methods
    }
}
