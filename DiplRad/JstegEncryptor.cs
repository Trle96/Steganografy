using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using static DiplRad.YCbCrPixel;

namespace DiplRad
{
    class JstegEncryptor : Encryptor
    {
        // Use standard matrix size.
        private const int MatrixSize = 8;
        // 0xFFFFFFFE
        private const int RemoverMask = -2;

        // Since we are encoding DC diff instead of absolute value, we need to keep track of last's block DC value.
        private int lastYDC = 0;
        private int lastCbDC = 0;
        private int lastCrDC = 0;

        // Keep track of what byte/bit are we encoding/decoding. We are doing this to optimse memory usage.
        private int currentByte = 0;
        private int currentBit = 0;

        #region Headers

        // Encode start header.
        //
        private void EncodeStartHeader(FileStream fileStream)
        {
            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.SOI);

            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.APP0);

            fileStream.Write(JpegEncodingFlags.APP0Length, 0, JpegEncodingFlags.APP0Length.Length);

            fileStream.Write(JpegEncodingFlags.JFIF, 0, JpegEncodingFlags.JFIF.Length);
            fileStream.Write(JpegEncodingFlags.VersionJFIF1, 0, JpegEncodingFlags.VersionJFIF1.Length);

            fileStream.Write(JpegEncodingFlags.APP0Density, 0, JpegEncodingFlags.APP0Density.Length);
            fileStream.Write(JpegEncodingFlags.NoThumbnail, 0, JpegEncodingFlags.NoThumbnail.Length);
        }

        // Encode quantization table header.
        //
        private void EncodeQuantizationTable(FileStream fileStream, bool isLuminance)
        {
            int[] serializedList;

            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.QuantizationMarker);
            fileStream.Write(JpegEncodingFlags.QuantizationMarkerLength, 0, JpegEncodingFlags.QuantizationMarkerLength.Length);

            if (isLuminance)
            {
                fileStream.WriteByte(JpegEncodingFlags.QuantizationYMarkerMeta);
                serializedList = ZigZagSerialization(JpegStandardTables.LuminanceQuantizationTable);
            }
            else
            {
                fileStream.WriteByte(JpegEncodingFlags.QuantizationCrMarkerMeta);
                serializedList = ZigZagSerialization(JpegStandardTables.ChrominanceQuantizationTable);
            }

            for (int i = 0; i < serializedList.Length; i++)
            {
                fileStream.WriteByte(Convert.ToByte(serializedList[i]));
            }
        }

        // Encode start of the frame header.
        //
        private void EncodeStartOfTheFrame(FileStream fileStream, PictureYCbCr picture)
        {
            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.StartOfTheFrameMarker);
            fileStream.Write(JpegEncodingFlags.StartOfTheFrameLength, 0, JpegEncodingFlags.StartOfTheFrameLength.Length);
            fileStream.WriteByte(JpegEncodingFlags.DataPrecisionStartOfTheFrame);

            var pictureHeight = Helper.ConvertIntToByteArray(picture.displayHeight);
            fileStream.Write(pictureHeight, 0, pictureHeight.Length);

            var pictureWidth = Helper.ConvertIntToByteArray(picture.displayWidth);
            fileStream.Write(pictureWidth, 0, pictureWidth.Length);

            fileStream.Write(JpegEncodingFlags.StartOfTheFrameComponents, 0, JpegEncodingFlags.StartOfTheFrameComponents.Length);
        }

        // Encode huffman table header.
        //
        private void EncodeHuffmanTable(FileStream fileStream, bool isLuminance, bool isDCComponent)
        {
            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.HuffmanMarker);

            if (isLuminance)
            {
                var current = isDCComponent ? JpegEncodingFlags.LuminanceDCHuffmanMetadataLength : JpegEncodingFlags.LuminanceACHuffmanMetadataLength;
                fileStream.Write(current, 0, current.Length);

                fileStream.WriteByte(isDCComponent ? JpegEncodingFlags.HuffmanYDCMarker : JpegEncodingFlags.HuffmanYACMarker);

                current = isDCComponent ? JpegEncodingFlags.LuminanceDCHuffmanMetadata : JpegEncodingFlags.LuminanceACHuffmanMetadata;
                fileStream.Write(current, 0, current.Length);
            }
            else
            {
                var current = isDCComponent ? JpegEncodingFlags.ChrominanceDCHuffmanMetadataLength : JpegEncodingFlags.ChrominanceACHuffmanMetadataLength;
                fileStream.Write(current, 0, current.Length);

                fileStream.WriteByte(isDCComponent ? JpegEncodingFlags.HuffmanCrDCMarker : JpegEncodingFlags.HuffmanCrACMarker);

                current = isDCComponent ? JpegEncodingFlags.ChrominanceDCHuffmanMetadata : JpegEncodingFlags.ChrominanceACHuffmanMetadata;
                fileStream.Write(current, 0, current.Length);
            }
        }

        // Encode start of the scan header.
        //
        private void EncodeStartOfScan(FileStream fileStream)
        {
            fileStream.WriteByte(JpegEncodingFlags.ControlFlag);
            fileStream.WriteByte(JpegEncodingFlags.StartOfScanMarker);
            fileStream.Write(JpegEncodingFlags.StartOfScanLength, 0, JpegEncodingFlags.StartOfScanLength.Length);
            fileStream.Write(JpegEncodingFlags.StartOfScanComponents, 0, JpegEncodingFlags.StartOfScanComponents.Length);
            fileStream.Write(JpegEncodingFlags.StartOfScanEmpty, 0, JpegEncodingFlags.StartOfScanEmpty.Length);
        }

        // Encode all necessary headers directly to the new jpeg file.
        //
        private void EncodeHeaders(FileStream fileStream, PictureYCbCr picture)
        {
            // Add some information to the file.
            EncodeStartHeader(fileStream);

            // Add Quantization table information.
            EncodeQuantizationTable(fileStream, isLuminance: true);
            EncodeQuantizationTable(fileStream, isLuminance: false);

            // Add start of the frame header.
            EncodeStartOfTheFrame(fileStream, picture);

            // Encode all huffman tables used.
            EncodeHuffmanTable(fileStream, isLuminance: true, isDCComponent: true);
            EncodeHuffmanTable(fileStream, isLuminance: true, isDCComponent: false);
            EncodeHuffmanTable(fileStream, isLuminance: false, isDCComponent: true);
            EncodeHuffmanTable(fileStream, isLuminance: false, isDCComponent: false);

            // Encode start of the scan.
            EncodeStartOfScan(fileStream);
        }

        #endregion Headers

        private void Reset()
        {
            currentBit = 0;
            currentByte = 0;
            lastYDC = 0;
            lastCbDC = 0;
            lastCrDC = 0;
        }

        internal override string GetAlgorithmName()
        {
            return "JSteg";
        }

        internal override string GetEncryptionOutputFileExtension()
        {
            return "jpg";
        }

        // Get next bit from buffer from decoding.
        private bool GetNextBitFromEncryptedPicture(byte[] buffer)
        {
            if (currentBit == 8)
            {
                currentBit = 0;

                if (buffer[currentByte] == 0xff)
                {
                    currentByte += 2;
                }
                else
                {
                    currentByte++;
                }

                if (currentByte >= buffer.Length)
                {
                    throw new Exception("Error has occured. Message wasn't decoded properly.");
                }
            }

            return ((buffer[currentByte] & (0x80 >> currentBit++)) != 0);
        }

        // Gets value of DC component from previous block.
        //
        private int GetPreviousDCValue(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.Y:
                    return lastYDC;
                case ComponentType.Cb:
                    return lastCbDC;
                default:
                    return lastCrDC;
            }
        }

        // Sets value of DC component from previous block.
        //
        private void SetPreviousDCValue(ComponentType type, int newValue)
        {
            switch (type)
            {
                case ComponentType.Y:
                    lastYDC = newValue;
                    break;
                case ComponentType.Cb:
                    lastCbDC = newValue;
                    break;
                default:
                    lastCrDC = newValue;
                    break;
            }
        }

        // Gets value from Huffman standard DC table based on type and category.
        //
        private string GetHuffmanDCTableValue(ComponentType type, int category)
        {
            if (type == ComponentType.Y)
            {
                return JpegStandardTables.LuminanceDCHuffmanTable[category];
            }
            else
            {
                return JpegStandardTables.ChrominanceDCHuffmanTable[category];
            }
        }

        // Gets value from Huffman standard AC table based on type, category and number of zeroes before nonZero AC compenent.
        //
        private string GetHuffmanACTableValue(ComponentType type, int category, int zeroesBeforeValue = 0)
        {
            if (type == ComponentType.Y)
            {
                return JpegStandardTables.LuminanceACHuffmanTable[zeroesBeforeValue, category];
            }
            else
            {
                return JpegStandardTables.ChrominanceACHuffmanTable[zeroesBeforeValue, category];
            }
        }

        // Gets a value from standard quantization table.
        //
        private double GetQuantizationTableElement(ComponentType type, int u, int v)
        {
            if (type == ComponentType.Y)
            {
                return JpegStandardTables.LuminanceQuantizationTable[u, v];
            }
            else
            {
                return JpegStandardTables.ChrominanceQuantizationTable[u, v];
            }
        }

        // Takes a quantized Block and encodes it using standard huffman tables.
        //
        private String EncodeBlock(int[,] quantizedBlock, ComponentType type, bool encodeMessage)
        {
            int zeroesBeforeCurrent = 0;

            int[] serializedList = ZigZagSerialization(quantizedBlock);

            // Encode DC component.
            String completeString = EncodeDCComponent(type, serializedList[0]);
            String potentialAdded = "";
            // Encode AC component.
            for (int elemNum = 1; elemNum < serializedList.Length; elemNum++)
            {
                if (serializedList[elemNum] == 0)
                {
                    zeroesBeforeCurrent++;

                    // If there are 16 zeroes in a row, we need to encode a special value.
                    if (zeroesBeforeCurrent == 16)
                    {
                        potentialAdded += GetHuffmanACTableValue(type, 0, zeroesBeforeCurrent - 1);
                        zeroesBeforeCurrent = 0;
                    }
                }
                else
                {
                    completeString += potentialAdded;
                    potentialAdded = "";

                    int currentElem = serializedList[elemNum];
                    if (encodeMessage)
                    {
                        if ((currentElem < 0 || currentElem > 1) && !stegoMessage.IsWholeMessageEncoded())
                        {
                            currentElem &= RemoverMask;
                            currentElem |= stegoMessage.GetNextStegoMessageBit();
                        }
                    }

                    completeString += GetHuffmanACTableValue(type, DetermineCategory(currentElem), zeroesBeforeCurrent);
                    completeString += DetermineMagnitude(currentElem);
                    zeroesBeforeCurrent = 0;
                }
            }

            if (zeroesBeforeCurrent > 0 || potentialAdded.Length > 0)
            {
                completeString += GetHuffmanACTableValue(type, 0, 0);
            }

            return completeString;
        }

        // Encodes DC component.
        //
        private String EncodeDCComponent(ComponentType type, int newDC)
        {
            int diff = newDC - GetPreviousDCValue(type);

            // DC component encoding is optimised by using DIFF between DC component of the new block and the previous one.
            // We need to update previousDC component after every iteration.
            SetPreviousDCValue(type, newDC);
            int category = DetermineCategory(diff);
            if (category == 0)
            {
                return "00";
            }

            String firstPart = GetHuffmanDCTableValue(type, category);
            return firstPart + DetermineMagnitude(diff);
        }

        // Determines category in which value belongs.
        //
        private int DetermineCategory(int val)
        {
            int absVal = Math.Abs(val);
            int categoryMark = 1;
            int category = 0;

            while (absVal >= categoryMark)
            {
                categoryMark *= 2;
                category++;
            }

            return category;
        }

        // Determines magnitude for given value.
        //
        private string DetermineMagnitude(int val)
        {
            StringBuilder magnitude = new StringBuilder();

            if (val > 0)
            {
                magnitude.Append(Convert.ToString(val, 2));
            }
            else
            {
                magnitude.Append(Convert.ToString(Math.Abs(val), 2));
                for (int i = 0; i < magnitude.Length; i++)
                {
                    if (magnitude[i] == '0')
                    {
                        magnitude[i] = '1';
                    }
                    else
                    {
                        magnitude[i] = '0';
                    }
                }
            }

            return magnitude.ToString();
        }

        // Serializes block using zig-zag order.
        //
        private int[] ZigZagSerialization(int[,] quantizedBlock, int matixSize = MatrixSize)
        {
            int[] zigZagList = new int[matixSize * matixSize];
            int currentElemNum = 0;
            bool ascDirection = false;
            int currentIteration = 0;
            int x = 0, y = 0;

            while (currentIteration < matixSize)
            {
                while ((!ascDirection && x != 0) || (ascDirection && y != 0))
                {
                    zigZagList[currentElemNum++] = quantizedBlock[y, x];
                    if (ascDirection)
                    {
                        x++;
                        y--;
                    }
                    else
                    {
                        x--;
                        y++;
                    }
                }

                zigZagList[currentElemNum++] = quantizedBlock[y, x];

                if (ascDirection && currentIteration < matixSize)
                {
                    x++;
                }
                else
                {
                    y++;
                }

                currentIteration++;
                ascDirection = !ascDirection;
            }

            if (ascDirection)
            {
                x++;
                y--;
            }
            else
            {
                x--;
                y++;
            }


            while (currentIteration > 1)
            {
                while ((ascDirection && x != matixSize - 1) || (!ascDirection && y != matixSize - 1))
                {
                    zigZagList[currentElemNum++] = quantizedBlock[y, x];
                    if (ascDirection)
                    {
                        x++;
                        y--;
                    }
                    else
                    {
                        x--;
                        y++;
                    }
                }

                zigZagList[currentElemNum++] = quantizedBlock[y, x];

                if (ascDirection)
                {
                    y++;
                }
                else
                {
                    x++;
                }

                currentIteration--;
                ascDirection = !ascDirection;
            }

            return zigZagList;
        }

        // Quantisiezes given block using standard quantization tables.
        //
        private int[,] Quantization(double[,] block, ComponentType type)
        {
            int[,] quantizedBlock = new int[8, 8];
            for (int u = 0; u < 7; u++)
            {
                for (int v = 0; v < 7; v++)
                {
                    quantizedBlock[u, v] = (int)Math.Round(block[u, v] / GetQuantizationTableElement(type, u, v));
                }
            }

            return quantizedBlock;
        }

        // Returns encoded block in a form of string.
        private string EncodeWholeColorBlock(ColorYCbCrBlock colorYCbCrBlock, bool encodeMessage)
        {
            string[] ret = new string[3];

            for (int componentType = (int)ComponentType.Y; componentType <= (int)ComponentType.Cr; componentType++)
            {
                double[,] dctBlock = colorYCbCrBlock.DCTTransformation((ComponentType)componentType);
                int[,] quantizedBlock = Quantization(dctBlock, (ComponentType)componentType);
                ret[componentType] += EncodeBlock(quantizedBlock, (ComponentType)componentType, encodeMessage);
            };

            return ret[0] + ret[1] + ret[2];
        }

        // Returns inverse lumiance/chromiance DC huffman table.
        //
        private Dictionary<string, int> GetInverseDCHuffmanTable(bool isLuminance)
        {
            if (isLuminance)
            {
                return JpegStandardTables.InverseLuminanceDCHuffmanTable;
            }
            else
            {
                return JpegStandardTables.InverseChrominanceDCHuffmanTable;
            }
        }

        // Returns inverse lumiance/chromiance AC huffman table.
        //
        private Dictionary<string, Tuple<int, int>> GetInverseACHuffmanTable(bool isLuminance)
        {
            if (isLuminance)
            {
                return JpegStandardTables.InverseLuminanceACHuffmanTable;
            }
            else
            {
                return JpegStandardTables.InverseChrominanceACHuffmanTable;
            }
        }

        // Find start of the scan frame.
        //
        private long FindStartOfTheScanFrame(FileStream fs)
        {
            
            byte prev = 0;
            byte current = (byte)fs.ReadByte();

            // Search for start of scan.
            while (current != JpegEncodingFlags.StartOfScanMarker || prev != JpegEncodingFlags.ControlFlag)
            {
                prev = current;
                current = (byte)fs.ReadByte();
            }

            uint StartOfScanLenght = (uint)fs.ReadByte();
            StartOfScanLenght <<= 8;
            StartOfScanLenght += (uint)fs.ReadByte();

            fs.Seek(StartOfScanLenght - 2, SeekOrigin.Current);

            return fs.Position;
        }

        // Main function used to encrypt message.
        //
        internal string CompressPicture(IOPaths iOPaths, bool encodeMessage)
        {
            Reset();
            PictureYCbCr pictureYCbCr = new PictureYCbCr(new Bitmap(iOPaths.ImagePath));

            var outputFileName = GenerateEncryptionOutputFileName();
            using (FileStream fs = File.Create(outputFileName))
            {
                EncodeHeaders(fs, pictureYCbCr);

                StringBuilder encodedPicture = new StringBuilder();

                // Pass through all of the blocks and encode each of them.
                double cachedLimiter = pictureYCbCr.height / 20.0;
                for (int blockY = 0; blockY < pictureYCbCr.height; blockY++)
                {
                    for (int blockX = 0; blockX < pictureYCbCr.width; blockX++)
                    {
                        ColorYCbCrBlock currentBlock = pictureYCbCr.blocks[blockX, blockY];
                        encodedPicture.Append(EncodeWholeColorBlock(currentBlock, encodeMessage: encodeMessage));
                    }

                    // UI progressBar update
                    while (blockY + 1 >= cachedLimiter)
                    {
                        cachedLimiter += pictureYCbCr.height / 20.0;
                        double currentProgress = Math.Round((blockY + 1) / (pictureYCbCr.height / 50.0));
                        if (!encodeMessage)
                        {
                            currentProgress += 50;
                        }

                        parentForm.SetCurrentProgress((int)currentProgress);
                    }
                }

                byte[] encodedPictureInBytes = Helper.BinaryStringToByteArray(encodedPicture);
                for (int i = 0; i < encodedPictureInBytes.Length; i++)
                {
                    fs.WriteByte(encodedPictureInBytes[i]);

                    // 0xFF represents the value of control flag, however there is a chance that this value is encoded naturally in last step.
                    // In that case we need to add extra 0x00 byte to mark is as non-control flag
                    if (encodedPictureInBytes[i] == 255)
                    {
                        fs.WriteByte(JpegEncodingFlags.NonControlFlagMarker);
                    }
                }

                // Encode end of the file.
                fs.WriteByte(JpegEncodingFlags.ControlFlag);
                fs.WriteByte(JpegEncodingFlags.EOI);

                if (!encodeMessage)
                {
                    parentForm.SetCurrentProgress(100);
                }
            }

            return outputFileName;
        }

        // Main function used to encrypt message.
        //
        internal override string EncryptPicture()
        {
            string outputFileName = CompressPicture(ioPaths, encodeMessage: true);

            // Throw error if message is longer than capacity. We are doing this now,
            // because we cannot determine picture capacity before jpeg compression, therefore we don't know if message is too long to encode before this point.
            if (!stegoMessage.IsWholeMessageEncoded() && !stegoMessage.IsLoopedMessageEncodedAtLeastOnce())
            {
                throw new Exception(String.Format("Message is too long. Capacity is {0}, message length is {1}", stegoMessage.GetCurrentBit() / 8, stegoMessage.GetTotalBitCount() / 8));
            }

            return outputFileName;
        }

        // Main function used to decrypt message. Note: We are assuming that all of the standard tables are being used, and same options as in encrypting algorithm.
        //
        internal override string DecryptPicture(StegoOptions stegoOptions)
        {
            Reset();
            List<Boolean> messageInBits = new List<Boolean>();

            using (FileStream fs = File.OpenRead(ioPaths.ImagePath))
            {
                long startPosition = FindStartOfTheScanFrame(fs);

                byte[] scanFrameBuffer = new byte[fs.Length - startPosition - 2];
                fs.Read(scanFrameBuffer, 0, (int)(fs.Length - startPosition - 2));

                string currentKey = "";
                bool messageSizeFound = false;
                uint messageSize = 0;

                double cachedLimiter = 0;
                while (!messageSizeFound || messageSize * 8 > messageInBits.Count)
                {
                    if (!messageSizeFound && messageInBits.Count > 31)
                    {
                        messageSize = Helper.GetMessageSizeFromBoolArray(messageInBits);
                        messageSizeFound = true;
                        cachedLimiter = (messageSize * 8) / 20.0;

                        Console.WriteLine(String.Format("Found message legth: {0}", messageSize));

                        // Remove first 16bits as they represent message size.
                        messageInBits.RemoveRange(0, 32);
                    }

                    for (int type = 0; type < 3; type++)
                    {
                        // Once number of zeroes reaches 64 we know that we processed all of the AC components.
                        int zeroes = 0;

                        while (true)
                        {
                            // Add new char to the currentKey and check if that Key exists in Inverse tables.
                            currentKey += GetNextBitFromEncryptedPicture(scanFrameBuffer) ? "1" : "0";

                            if (GetInverseDCHuffmanTable(type == 0).ContainsKey(currentKey))
                            {
                                for (int i = 0; i < GetInverseDCHuffmanTable(type == 0)[currentKey]; i++)
                                {
                                    GetNextBitFromEncryptedPicture(scanFrameBuffer);
                                }
                                currentKey = "";
                                break;
                            }
                        }

                        while (zeroes < 63)
                        {
                            // Add new char to the currentKey and check if that Key exists in Inverse tables.
                            currentKey += GetNextBitFromEncryptedPicture(scanFrameBuffer) ? "1" : "0";
                            if (GetInverseACHuffmanTable(type == 0).ContainsKey(currentKey))
                            {
                                // Check if current key is end of block marker.
                                if (GetHuffmanACTableValue(type == 0 ? ComponentType.Y : ComponentType.Cr, 0, 0) == currentKey)
                                {
                                    currentKey = "";
                                    break;
                                }

                                // Check if current key is ZEROES marker.
                                if (GetHuffmanACTableValue(type == 0 ? ComponentType.Y : ComponentType.Cr, 0, 15) == currentKey)
                                {
                                    zeroes += 16;
                                }
                                else
                                {
                                    // If current key is different then 1, then it contains 1 bit of encoded message.
                                    zeroes += GetInverseACHuffmanTable(type == 0)[currentKey].Item2 + 1;

                                    bool isValid;
                                    int coef = ReadAcValue(scanFrameBuffer, type, currentKey, out isValid);

                                    if (isValid)
                                    {
                                        messageInBits.Add((coef & 0x01) > 0);
                                    }
                                }

                                currentKey = "";
                            }
                        }
                    }

                    while (messageSizeFound && messageInBits.Count >= cachedLimiter)
                    {
                        cachedLimiter += (messageSize * 8) / 20.0;
                        parentForm.SetCurrentProgress((int)Math.Round(messageInBits.Count / (messageSize * 8 / 100.0)));
                    }
                }

                parentForm.SetCurrentProgress(100);
                return GenerateDecryptionOutput(messageInBits, stegoOptions);
            }
        }

        // Populates histogram as well as Stegoanalysis chart.
        public override void PopulateChart(Chart chart, Chart PercentChart, string picturePath, out double max)
        {
            max = 0;
            currentByte = 0;
            currentBit = 0;

            int[] histogram_cr = new int[256];
            int[] histogram_cb = new int[256];
            int[] histogram_y = new int[256];

            using (FileStream fs = File.OpenRead(picturePath))
            {
                long startPosition = FindStartOfTheScanFrame(fs);

                byte[] scanFrameBuffer = new byte[fs.Length - startPosition];
                fs.Read(scanFrameBuffer, 0, (int)(fs.Length - startPosition));

                string currentKey = "";

                double percent = 1;
                double cachedLimiter = scanFrameBuffer.Length / 100.0;
                bool foundEnd = false;
                while (currentByte < scanFrameBuffer.Length && !foundEnd)
                {
                    for (int type = 0; type < 3; type++)
                    {
                        // Once number of zeroes reaches 64 we know that we processed all of the AC components.
                        int zeroes = 0;
                        if (scanFrameBuffer[currentByte] == 0xff && scanFrameBuffer[currentByte + 1] == 0xd9 ||
                            scanFrameBuffer[currentByte + 1] == 0xff && scanFrameBuffer[currentByte + 2] == 0xd9)
                        {
                            foundEnd = true;
                            break;
                        }

                        while (true)
                        {
                            // Add new char to the currentKey and check if that Key exists in Inverse tables.
                            currentKey += GetNextBitFromEncryptedPicture(scanFrameBuffer) ? "1" : "0";

                            if (GetInverseDCHuffmanTable(type == 0).ContainsKey(currentKey))
                            {
                                for (int i = 0; i < GetInverseDCHuffmanTable(type == 0)[currentKey]; i++)
                                {
                                    GetNextBitFromEncryptedPicture(scanFrameBuffer);
                                }
                                currentKey = "";
                                break;
                            }
                        }

                        while (zeroes < 63)
                        {
                            // Add new char to the currentKey and check if that Key exists in Inverse tables.
                            currentKey += GetNextBitFromEncryptedPicture(scanFrameBuffer) ? "1" : "0";
                            if (GetInverseACHuffmanTable(type == 0).ContainsKey(currentKey))
                            {
                                // Check if current key is end of block marker.
                                if (GetHuffmanACTableValue(type == 0 ? ComponentType.Y : ComponentType.Cr, 0, 0) == currentKey)
                                {
                                    currentKey = "";
                                    break;
                                }

                                // Check if current key is ZEROES marker.
                                if (GetHuffmanACTableValue(type == 0 ? ComponentType.Y : ComponentType.Cr, 0, 15) == currentKey)
                                {
                                    zeroes += 16;
                                }
                                else
                                {
                                    // If current key is different then 1, then it contains 1 bit of encoded message.
                                    zeroes += GetInverseACHuffmanTable(type == 0)[currentKey].Item2 + 1;
                                    bool isValid;
                                    int coef = ReadAcValue(scanFrameBuffer, type, currentKey, out isValid);

                                    if (isValid)
                                    {
                                        if (type == 0)
                                        {
                                            histogram_y[coef + 128]++;
                                        }
                                        else if (type == 1)
                                        {
                                            histogram_cb[coef + 128]++;
                                        }
                                        else if (type == 2)
                                        {
                                            histogram_cr[coef + 128]++;
                                        }
                                    }
                                }

                                currentKey = "";
                            }
                        }
                    }

                    if (currentByte >= cachedLimiter)
                    {
                        double probability = CalculateProbabilityAtPoint(histogram_cr);
                        PercentChart.Series["Cr"].Points.AddXY(percent, probability * 100);

                        probability = CalculateProbabilityAtPoint(histogram_cb);
                        PercentChart.Series["Cb"].Points.AddXY(percent, probability * 100);

                        probability = CalculateProbabilityAtPoint(histogram_y);
                        PercentChart.Series["Y"].Points.AddXY(percent, probability * 100);
                        cachedLimiter += scanFrameBuffer.Length / 100.0;
                        percent += 1;
                    }
                }
            }

            for (int i = 0; i < histogram_cr.Length; i++)
            {
                chart.Series["Cr"].Points.AddXY(i - 128, histogram_cr[i]);
                chart.Series["Cb"].Points.AddXY(i - 128, histogram_cb[i]);
                chart.Series["Y"].Points.AddXY(i - 128, histogram_y[i]);
                max = Math.Max(max, histogram_cr[i]);
                max = Math.Max(max, histogram_cb[i]);
                max = Math.Max(max, histogram_y[i]);
            }
        }

        private int ReadAcValue(byte[] scanFrameBuffer, int type, string category, out bool isValid)
        {
            List<bool> tempList = new List<bool>();
            int coef = 0;
            isValid = true;

            for (int i = 0; i < GetInverseACHuffmanTable(type == 0)[category].Item1; i++)
            {
                tempList.Add(GetNextBitFromEncryptedPicture(scanFrameBuffer));

                if ((tempList[0] && tempList[i]) || (!tempList[0] && !tempList[i]))
                {
                    coef = coef * 2 + 1;
                }
                else
                {
                    coef *= 2;
                }
            }

            // Add only if temp != 1 and we haven't filled messageInBits to the messageSize * 8.
            if (!(tempList.Count == 1 && tempList[0]))
            {
                if (!tempList[0])
                {
                    coef *= -1;
                }
            }
            else
            {
                isValid = false;
            }

            return coef;
        }
    }
}
