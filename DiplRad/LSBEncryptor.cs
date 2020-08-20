using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    class LSBEncryptor : Encryptor
    {
        #region Fields
        internal ulong messageSize;
        internal Bitmap picture;

        internal int currentXCoord = 0;
        internal int currentYCoord = 0;

        internal int newRedColor = 0;
        internal int newBlueColor = 0;
        internal int newGreenColor = 0;

        #endregion Fields
        #region Methods

        // Constructor
        public LSBEncryptor(string outputPath) : base(outputPath)
        {
        }

        public LSBEncryptor()
        {
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
        internal override string EncryptPicture(string picturePath, string messagePath)
        {
            currentXCoord = 0;
            currentYCoord = 0;
            int percent = 0;
            encryptionMessage = new EncryptionMessage(messagePath, insertMessageSizeAtBeggining: false);
            double textPercent = encryptionMessage.GetMessageSize() / 100.0;
            //var messageInBits = ConvertTxtToEncryptedBitArray(messagePath, out messageSize);
            /*List<bool> messageInBits;
            using (StreamReader streamReader = new StreamReader(messagePath))
            {
                byte[] message = Zip(streamReader.ReadToEnd());
                messageInBits = ConvertByteArrayToBitArray(message, out messageSize);
            }*/

            picture = new Bitmap(picturePath);
            uint imageCapacity = CalculateCapacity();
            uint pixelCount = (uint)(picture.Width * picture.Height);

            if (imageCapacity < encryptionMessage.GetMessageSize())
            {
                throw new Exception("Image capacity is lower then message size");
            }

            // int testMask = 1;
            for (int pixelsProcessed = 0; pixelsProcessed < (pixelCount/2) /*&& currentBit < messageInBits.Count*/; pixelsProcessed++)
            {
                Color currentPixel = GetCurrentPixel();

                foreach (ColorType colorType in Enum.GetValues(typeof(ColorType)))
                {
                    int currentColor = GetPixelColor(currentPixel, colorType);
                    byte mask = (byte)(0xFF << lsbUsed);
                    currentColor = (byte)(currentColor & mask);

                    var colorModification = 0;
                    for (int i = 0; i < lsbUsed/* && currentBit < messageInBits.Count*/; i++)
                    {
                        colorModification <<= 1;
                        colorModification += encryptionMessage.GetNextBit();
                        // colorModification += testMask;
                        // testMask = testMask == 1 ? 0 : 1;
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
                /*if (currentBit >= textPercent)
                {
                    percent++;
                    textPercent += messageInBits.Count / 100.0;
                    parentForm.SetCurrentProgress(percent);
                }*/
            }

            parentForm.SetCurrentProgress(100);
            var outputFileName = outputPath + "\\LSBTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".png";
            picture.Save(outputFileName);
            Console.WriteLine(String.Format("New file created. File: {0}", outputFileName));

            return outputFileName;
        }

        //
        // Main function used for decryption.
        //
        public override string DecryptPicture(string picturePath, string outputPath)
        {
            currentXCoord = 0;
            currentYCoord = 0;
            int percent = 0;
            List<Boolean> messageInBits = new List<Boolean>();
            picture = new Bitmap(picturePath);
            uint pixelCount = (uint)(picture.Width * picture.Height);
            double textPercent = pixelCount / 100.0;

            for (int pixelsProcessed = 0; pixelsProcessed < pixelCount; pixelsProcessed++)
            {
                Color currentPixel = GetCurrentPixel();

                foreach (ColorType colorType in Enum.GetValues(typeof(ColorType)))
                {
                    int currentColor = GetPixelColor(currentPixel, colorType);
                    var mask = 0x01 << (lsbUsed - 1);
                    for (int i = 0; i < lsbUsed; i++)
                    {
                        messageInBits.Add((currentColor & mask) != 0);
                        mask >>= 1;
                    }
                }

                IterateToNextPixel();
                if (pixelsProcessed >= textPercent)
                {
                    percent++;
                    textPercent += pixelCount / 100.0;
                    parentForm.SetCurrentProgress(percent);
                }
            }

            parentForm.SetCurrentProgress(100);
            var outputFileName = outputPath + "\\LSBDecryptionTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".txt";
            File.WriteAllText(outputFileName, Helper.CreateStringFromBoolArray(messageInBits));
            return outputFileName;
        }
        #endregion Methods
    }
}
