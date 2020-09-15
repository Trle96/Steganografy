using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplRad
{
    class PITEncryptor : Encryptor
    {
        #region Fields
        internal ulong messageSize;
        internal ulong mask = (long)0x1 << 47;

        internal ColorType IndicatorChannel;
        internal ColorType FirstChannel;
        internal ColorType SecondChannel;

        internal Bitmap picture;

        internal int currentXCoord = 1;
        internal int currentYCoord = 0;

        #endregion Fields

        #region Methods

        // Constructor
        public PITEncryptor() : base()
        {
        }

        internal void Reset()
        {
            currentXCoord = 1;
            currentYCoord = 0;
        }

        internal override void SetStegoMessage(string messagePath, StegoOptions stegoOptions)
        {
            stegoOptions.InsertMessageSizeAtBeginning = false;
            stegoMessage = new StegoMessage(messagePath, stegoOptions);
        }

        internal override string GetAlgorithmName()
        {
            return "PIT";
        }

        //
        // Sets new color for current pixel based on firstChannelColor and secondChannelColor.
        //
        internal void SetCurrentPixel(int firstChannelColor, int secondChannelColor)
        {
            Color pixel = picture.GetPixel(currentXCoord, currentYCoord);

            var newRedColor = (IndicatorChannel == ColorType.RED) ? pixel.R : ((FirstChannel == ColorType.RED) ? firstChannelColor : secondChannelColor);
            var newBlueColor = (IndicatorChannel == ColorType.BLUE) ? pixel.B : ((FirstChannel == ColorType.BLUE) ? firstChannelColor : secondChannelColor);
            var newGreenColor = (IndicatorChannel == ColorType.GREEN) ? pixel.G : ((FirstChannel == ColorType.GREEN) ? firstChannelColor : secondChannelColor);

            picture.SetPixel(currentXCoord, currentYCoord, Color.FromArgb(newRedColor, newGreenColor, newBlueColor));
        }

        //
        // Gets next pixel for alteration.
        //
        internal Color GetNextPixel()
        {
            currentYCoord++;
            if (currentYCoord == picture.Height)
            {
                currentXCoord += 1;
                currentYCoord = 0;
            }

            if(currentXCoord >= picture.Width)
            {
                throw new Exception("Picture is not big enough for this message");
            }

            
            return picture.GetPixel(currentXCoord, currentYCoord);
        }

        //
        // Calculates new color for header, after alteration with messageSize.
        //
        internal int CalculateNewHeaderPixel(int currentColor)
        {
            currentColor |= 0x03;
            if ((mask & messageSize) == 0)
            {
                currentColor ^= 0x02;
            }

            mask >>= 1;
            if ((mask & messageSize) == 0)
            {
                currentColor ^= 0x01;
            }
            mask >>= 1;

            return currentColor;
        }

        //
        // Fetchs part of message lenght from current pixel.
        //
        internal List<Boolean> CalculateMessageLengthFromPixel(int currentColor)
        {
            List<Boolean> ret = new List<Boolean>();
            ret.Add((currentColor & 0x02) != 0);
            ret.Add((currentColor & 0x01) != 0);
            return ret;
        }

        //
        // Get pixel color value for given colorType.
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
        // Delegates ColorType to each channel
        //
        internal void DelegateColorTypesToChannels ()
        {
            if (messageSize % 2 == 0)
            {
                IndicatorChannel = ColorType.RED;
            }
            else if (Helper.IsPrime(messageSize))
            {
                IndicatorChannel = ColorType.BLUE;
            }
            else
            {
                IndicatorChannel = ColorType.GREEN;
            }

            if (Helper.IsParityBitOne(messageSize))
            {
                switch (IndicatorChannel)
                {
                    case ColorType.RED:
                        FirstChannel = ColorType.BLUE;
                        SecondChannel = ColorType.GREEN;
                        break;
                    case ColorType.BLUE:
                        FirstChannel = ColorType.GREEN;
                        SecondChannel = ColorType.RED;
                        break;
                    case ColorType.GREEN:
                        FirstChannel = ColorType.BLUE;
                        SecondChannel = ColorType.RED;
                        break;
                }
            }
            else
            {
                switch (IndicatorChannel)
                {
                    case ColorType.RED:
                        FirstChannel = ColorType.GREEN;
                        SecondChannel = ColorType.BLUE;
                        break;
                    case ColorType.BLUE:
                        FirstChannel = ColorType.RED;
                        SecondChannel = ColorType.GREEN;
                        break;
                    case ColorType.GREEN:
                        FirstChannel = ColorType.RED;
                        SecondChannel = ColorType.BLUE;
                        break;
                }
            }
        }

        private int ModifyChannel(int channelValue)
        {
            channelValue &= 0xFC;

            var channelModification = stegoMessage.GetNextStegoMessageBit() << 1;
            if (!stegoMessage.IsWholeMessageEncoded())
            {
                channelModification += stegoMessage.GetNextStegoMessageBit();
            }

            return channelValue | channelModification;
        }

        //
        // Main function used for encryption
        //
        internal override string EncryptPicture()
        {
            Reset();
            picture = new Bitmap(ioPaths.ImagePath);
            double cachedLimiter = stegoMessage.GetTotalBitCount() / 20.0;
            messageSize = (ulong)stegoMessage.GetTotalBitCount();
            // Use first 8 pixels in the first row to write message size.
            // We are using 2 LSBs per pixel color, that means that we are storing 48bits.
            for (int i = 0; i < 8; i++)
            {
                Color pixelColor = picture.GetPixel(0, i);
                int redColor = CalculateNewHeaderPixel(pixelColor.R);
                int blueColor = CalculateNewHeaderPixel(pixelColor.B);
                int greenColor = CalculateNewHeaderPixel(pixelColor.G);

                picture.SetPixel(0, i, Color.FromArgb(redColor, greenColor, blueColor));
            }

            // Determine which channel will be indicator.
            DelegateColorTypesToChannels();
            while (!stegoMessage.IsWholeMessageEncoded())
            {
                Color currentPixel = GetNextPixel();

                bool populateFirstChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x02) != 0;
                bool populateSecondChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x01) != 0;

                int firstChannelColor = GetPixelColor(currentPixel, FirstChannel);
                int secondChannelColor = GetPixelColor(currentPixel, SecondChannel);

                if (populateFirstChannel)
                {
                    firstChannelColor = ModifyChannel(firstChannelColor);
                }

                if (populateSecondChannel && !stegoMessage.IsWholeMessageEncoded())
                {
                    secondChannelColor = ModifyChannel(secondChannelColor);
                }

                SetCurrentPixel(firstChannelColor, secondChannelColor);
                if (stegoMessage.GetCurrentBit() >= cachedLimiter)
                {
                    cachedLimiter += stegoMessage.GetTotalBitCount() / 20.0;
                    parentForm.SetCurrentProgress((int)Math.Round(stegoMessage.GetCurrentBit() / (stegoMessage.GetTotalBitCount() / 100.0)));
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
            List<Boolean> messageInBits =  new List<Boolean>();

            picture = new Bitmap(ioPaths.ImagePath);

            // First 8 pixels are used to code message length
            for (int i = 0; i < 8; i++)
            {
                Color pixelColor = picture.GetPixel(0, i);
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.R));
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.B));
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.G));
            }

            messageSize = Helper.CreateLongFromBoolArray(messageInBits);

            // Determine which channel will be indicator.
            DelegateColorTypesToChannels();
            messageInBits.Clear();

            ulong currentBit = 0;
            double cachedLimiter = messageSize / 20.0;
            while (currentBit < messageSize)
            {
                Color currentPixel = GetNextPixel();

                bool firstChannelPopulated = (GetPixelColor(currentPixel, IndicatorChannel) & 0x02) != 0;
                bool secondChannelPopulated = (GetPixelColor(currentPixel, IndicatorChannel) & 0x01) != 0;

                if (firstChannelPopulated)
                {
                    int firstChannelColor = GetPixelColor(currentPixel, FirstChannel);
                    messageInBits.Add((firstChannelColor & 0x02) != 0);
                    currentBit++;

                    if (currentBit >= messageSize)
                    {
                        continue;
                    }

                    messageInBits.Add((firstChannelColor & 0x01) != 0);
                    currentBit++;
                }

                if (secondChannelPopulated && currentBit < messageSize)
                {

                    int secondChannelColor = GetPixelColor(currentPixel, SecondChannel);
                    messageInBits.Add((secondChannelColor & 0x02) != 0);
                    currentBit++;

                    if (currentBit >= messageSize)
                    {
                        continue;
                    }

                    messageInBits.Add((secondChannelColor & 0x01) != 0);
                    currentBit++;
                }

                if (currentBit >= cachedLimiter)
                {
                    cachedLimiter += messageSize / 20.0;
                    parentForm.SetCurrentProgress((int)Math.Round(currentBit / (cachedLimiter / 100.0)));
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
                Color currentPixel = GetNextPixel();

                histogram_r[currentPixel.R]++;
                histogram_b[currentPixel.B]++;
                histogram_g[currentPixel.G]++;

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
