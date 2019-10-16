using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

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



        public enum ColorType
        {
            RED = 0,
            BLUE = 1,
            GREEN = 2,
        }
        #endregion Fields

        #region Methods

        // Constructor
        public PITEncryptor(string outputPath) : base(outputPath)
        {
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

            if(currentXCoord > picture.Width)
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
        internal override void EncryptPicture(string picturePath, string messagePath)
        {
            var messageInBits = ConvertTxtToBitArray(messagePath, out messageSize);
            picture = new Bitmap(picturePath);

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

            var currentBit = 0;
            while (currentBit < messageInBits.Count)
            {
                Color currentPixel = GetNextPixel();

                bool populateFirstChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x02) != 0;
                bool populateSecondChannel = (GetPixelColor(currentPixel, IndicatorChannel) & 0x01) != 0;

                int firstChannelColor = 0;
                int secondChannelColor = 0;

                if (populateFirstChannel)
                {
                    firstChannelColor = GetPixelColor(currentPixel, FirstChannel);
                    firstChannelColor = firstChannelColor & 0xFC;

                    var firstChannelModification = (messageInBits[currentBit++] ? 1 : 0) << 1;
                    if (currentBit < messageInBits.Count)
                    {
                        firstChannelModification += messageInBits[currentBit++] ? 1 : 0;
                    }

                    firstChannelColor = firstChannelColor | firstChannelModification;
                }

                if (populateSecondChannel && currentBit < messageInBits.Count)
                {
                    secondChannelColor = GetPixelColor(currentPixel, FirstChannel);
                    secondChannelColor = secondChannelColor & 0xFC;

                    var secondChannelModification = (messageInBits[currentBit++] ? 1 : 0) << 1;
                    if (currentBit < messageInBits.Count)
                    {
                        secondChannelModification += messageInBits[currentBit++] ? 1 : 0;
                    }

                    secondChannelColor = secondChannelColor | secondChannelModification;
                }

                SetCurrentPixel(firstChannelColor, secondChannelColor);
            }

            var outputFileName = outputPath + "PITTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".png";
            picture.Save(outputFileName);
            Console.WriteLine(String.Format("New file created. File: {0}", outputFileName));
        }

        //
        // Main function used for decryption.
        //
        internal override void DecryptPicture(string picturePath)
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

            ulong currentBit = 0;
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
            }

            Console.WriteLine(Helper.CreateStringFromBoolArray(messageInBits));
        }
        #endregion Methods
    }
}
