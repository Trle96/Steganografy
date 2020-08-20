using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

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
        public PITEncryptor(string outputPath) : base(outputPath)
        {
            lsbUsed = 2;
        }

        // Constructor
        public PITEncryptor()
        {
        }

        //
        // LSB count cannot be changed for PIT.
        //
        internal override void SetLSBCount(int value)
        {
            return;
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
            currentColor = currentColor | 0x03;
            if ((mask & messageSize) == 0)
            {
                currentColor = currentColor ^ 0x02;
            }

            mask = mask >> 1;
            if ((mask & messageSize) == 0)
            {
                currentColor = currentColor ^ 0x01;
            }
            mask = mask >> 1;

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
                IndicatorChannel = ColorType.RED;
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

        //
        // Main function used for encryption
        //
        internal override string EncryptPicture(string picturePath, string messagePath)
        {
            encryptionMessage = new EncryptionMessage(messagePath, insertMessageSizeAtBeggining: false);

            picture = new Bitmap(picturePath);
            double textPercent = encryptionMessage.GetMessageSize() / 100.0;

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

            int percent = 0;
            var currentBit = 0;
            while (currentBit < encryptionMessage.GetMessageSize())
            {
                Color currentPixel = GetNextPixel();

                bool populateFirstChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x02) != 0;
                bool populateSecondChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x01) != 0;

                int firstChannelColor = GetPixelColor(currentPixel, FirstChannel);
                int secondChannelColor = GetPixelColor(currentPixel, SecondChannel);

                if (populateFirstChannel)
                {
                    firstChannelColor = firstChannelColor & 0xFC;

                    var firstChannelModification = encryptionMessage.GetNextBit() << 1;
                    if (currentBit < encryptionMessage.GetMessageSize())
                    {
                        firstChannelModification += encryptionMessage.GetNextBit();
                    }

                    firstChannelColor = firstChannelColor | firstChannelModification;
                }

                if (populateSecondChannel && currentBit < encryptionMessage.GetMessageSize())
                {
                    secondChannelColor = secondChannelColor & 0xFC;

                    var secondChannelModification = encryptionMessage.GetNextBit() << 1;
                    if (currentBit < encryptionMessage.GetMessageSize())
                    {
                        secondChannelModification += encryptionMessage.GetNextBit();
                    }

                    secondChannelColor = secondChannelColor | secondChannelModification;
                }

                SetCurrentPixel(firstChannelColor, secondChannelColor);
                if (currentBit >= textPercent)
                {
                    percent++;
                    textPercent += encryptionMessage.GetMessageSize() / 100.0;
                    parentForm.SetCurrentProgress(percent);
                }
            }

            parentForm.SetCurrentProgress(100);
            var outputFileName = outputPath + "\\PITTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".png";
            picture.Save(outputFileName);
            Console.WriteLine(String.Format("New file created. File: {0}", outputFileName));

            return outputFileName;
        }

        //
        // Main function used for decryption.
        //
        public override string DecryptPicture(string picturePath, string outputPath)
        {
            List<Boolean> messageInBits =  new List<Boolean>();

            picture = new Bitmap(picturePath);

            // First 8 pixels are used to code message length
            for (int i = 0; i < 8; i++)
            {
                Color pixelColor = picture.GetPixel(0, i);
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.R));
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.B));
                messageInBits.AddRange(CalculateMessageLengthFromPixel(pixelColor.G));
            }

            messageSize = Helper.CreateLongFromBoolArray(messageInBits) * 8;

            // Determine which channel will be indicator.
            DelegateColorTypesToChannels();
            messageInBits.Clear();

            int percent = 0;
            ulong currentBit = 0;
            double textPercent = messageSize / 100.0;
            while (currentBit < messageSize)
            {
                Color currentPixel = GetNextPixel();

                bool firstChannelPopulated = (GetPixelColor(currentPixel, IndicatorChannel) & 0x02) != 0;
                bool secondChannelPopulated = (GetPixelColor(currentPixel, IndicatorChannel) & 0x01) != 0;

                int firstChannelColor = 0;
                int secondChannelColor = 0;

                if (firstChannelPopulated)
                {
                    firstChannelColor = GetPixelColor(currentPixel, FirstChannel);
                    messageInBits.Add((firstChannelColor & 0x02) != 0);
                    currentBit++;

                    if (currentBit >= messageSize)
                    {
                        continue;
                    }

                    messageInBits.Add((firstChannelColor & 0x01) != 0);
                    currentBit ++;
                }

                if (secondChannelPopulated && currentBit < messageSize)
                {

                    secondChannelColor = GetPixelColor(currentPixel, SecondChannel);
                    messageInBits.Add((secondChannelColor & 0x02) != 0);
                    currentBit++;

                    if (currentBit >= messageSize)
                    {
                        continue;
                    }

                    messageInBits.Add((secondChannelColor & 0x01) != 0);
                    currentBit++;
                }

                if (currentBit >= textPercent)
                {
                    percent++;
                    textPercent += messageSize / 100.0;
                    parentForm.SetCurrentProgress(percent);
                }
            }

            parentForm.SetCurrentProgress(100);
            var outputFileName = outputPath + "\\PITDecryptionTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".txt";
            File.WriteAllText(outputFileName, Helper.CreateStringFromBoolArray(messageInBits));
            return outputFileName;
        }
        #endregion Methods
    }
}
