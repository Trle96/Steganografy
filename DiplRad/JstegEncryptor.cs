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
        #region StandardTables
        private static readonly int[,] LuminanceQuantizationTable = {
                                                                        { 16, 12, 14, 14, 18, 24, 49, 72 },
                                                                        { 11, 12, 13, 17, 22, 35, 64, 92 },
                                                                        { 10, 14, 16, 22, 37, 55, 78, 95 },
                                                                        { 16, 19, 24, 29, 56, 64, 87, 98 },
                                                                        { 24, 26, 40, 51, 68, 81, 103, 112 },
                                                                        { 40, 58, 57, 87, 109, 104, 121, 100 },
                                                                        { 51, 60, 69, 80, 103, 113, 120, 103 },
                                                                        { 61, 55, 56, 62, 77, 92, 101, 99 } };

        private static readonly int[,] ChrominanceQuantizationTable = {
                                                                        { 17, 18, 24, 47, 99, 99, 99, 99 },
                                                                        { 18, 21, 26, 66, 99, 99, 99, 99 },
                                                                        { 24, 26, 56, 99, 99, 99, 99, 99 },
                                                                        { 47, 66, 99, 99, 99, 99, 99, 99 },
                                                                        { 99, 99, 99, 99, 99, 99, 99, 99 },
                                                                        { 99, 99, 99, 99, 99, 99, 99, 99 },
                                                                        { 99, 99, 99, 99, 99, 99, 99, 99 },
                                                                        { 99, 99, 99, 99, 99, 99, 99, 99 } };

        private static readonly string[] LuminanceDCHuffmanTable = { "00", "010", "011", "100", "101", "110", "1110", "11110", "111110", "1111110", "11111110", "111111110" };
        private static readonly string[] ChrominanceDCHuffmanTable = { "00", "01", "10", "110", "1110", "11110", "111110", "1111110", "11111110", "111111110", "1111111110", "11111111110" };

        private static readonly string[,] LuminanceACHuffmanTable = {
                                                                        { "1010", "00", "01", "100", "1011", "11010", "1111000", "11111000", "1111110110", "1111111110000010", "1111111110000011" },
                                                                        { "", "1100", "11011", "1111001", "111110110", "11111110110", "1111111110000100", "1111111110000101", "1111111110000110", "1111111110000111", "1111111110001000" },
                                                                        { "", "11100", "11111001", "1111110111", "111111110100", "1111111110001001", "1111111110001010", "1111111110001011", "1111111110001100", "1111111110001101", "1111111110001110" },
                                                                        { "", "111010", "111110111", "111111110101", "1111111110001111", "1111111110010000", "1111111110010001", "1111111110010010", "1111111110010011", "1111111110010100", "1111111110010101" },
                                                                        { "", "111011", "1111111000", "1111111110010110", "1111111110010111", "1111111110011000", "1111111110011001", "1111111110011010", "1111111110011011", "1111111110011100", "1111111110011101" },
                                                                        { "", "1111010", "11111110111", "1111111110011110", "1111111110011111", "1111111110100000", "1111111110100001", "1111111110100010", "1111111110100011", "1111111110100100", "1111111110100101" },
                                                                        { "", "1111011", "111111110110", "1111111110100110", "1111111110100111", "1111111110101000", "1111111110101001", "1111111110101010", "1111111110101011", "1111111110101100", "1111111110101101" },
                                                                        { "", "11111010", "111111110111", "1111111110101110", "1111111110101111", "1111111110110000", "1111111110110001", "1111111110110010", "1111111110110011", "1111111110110100", "1111111110110101" },
                                                                        { "", "111111000", "111111111000000", "1111111110110110", "1111111110110111", "1111111110111000", "1111111110111001", "1111111110111010", "1111111110111011", "1111111110111100", "1111111110111101" },
                                                                        { "", "111111001", "1111111110111110", "1111111110111111", "1111111111000000", "1111111111000001", "1111111111000010", "1111111111000011", "1111111111000100", "1111111111000101", "1111111111000110" },
                                                                        { "", "111111010", "1111111111000111", "1111111111001000", "1111111111001001", "1111111111001010", "1111111111001011", "1111111111001100", "1111111111001101", "1111111111001110", "1111111111001111" },
                                                                        { "", "1111111001", "1111111111010000", "1111111111010001", "1111111111010010", "1111111111010011", "1111111111010100", "1111111111010101", "1111111111010110", "1111111111010111", "1111111111011000" },
                                                                        { "", "1111111010", "1111111111011001", "1111111111011010", "1111111111011011", "1111111111011100", "1111111111011101", "1111111111011110", "1111111111011111", "1111111111100000", "1111111111100001" },
                                                                        { "", "11111111000", "1111111111100010", "1111111111100011", "1111111111100100", "1111111111100101", "1111111111100110", "1111111111100111", "1111111111101000", "1111111111101001", "1111111111101010" },
                                                                        { "", "1111111111101011", "1111111111101100", "1111111111101101", "1111111111101110", "1111111111101111", "1111111111110000", "1111111111110001", "1111111111110010", "1111111111110011", "1111111111110100" },
                                                                        { "11111111001", "1111111111110101", "1111111111110110", "1111111111110111", "1111111111111000", "1111111111111001", "1111111111111010", "1111111111111011", "1111111111111100", "1111111111111101", "1111111111111110" } };

        private static readonly string[,] ChrominanceACHuffmanTable = {
                                                                        { "00", "01", "100", "1010", "11000", "11001", "111000", "1111000", "111110100", "1111110110", "111111110100" },
                                                                        { "", "1011", "111001", "11110110", "111110101", "11111110110", "111111110101", "1111111110001000", "1111111110001001", "1111111110001010", "1111111110001011" },
                                                                        { "", "11010", "11110111", "1111110111", "111111110110", "111111111000010", "1111111110001100", "1111111110001101", "1111111110001110", "1111111110001111", "1111111110010000" },
                                                                        { "", "11011", "11111000", "1111111000", "111111110111", "1111111110010001", "1111111110010010", "1111111110010011", "1111111110010100", "1111111110010101", "1111111110010110" },
                                                                        { "", "111010", "111110110", "1111111110010111", "1111111110011000", "1111111110011001", "1111111110011010", "1111111110011011", "1111111110011100", "1111111110011101", "1111111110011110" },
                                                                        { "", "111011", "1111111001", "1111111110011111", "1111111110100000", "1111111110100001", "1111111110100010", "1111111110100011", "1111111110100100", "1111111110100101", "1111111110100110" },
                                                                        { "", "1111001", "11111110111", "1111111110100111", "1111111110101000", "1111111110101001", "1111111110101010", "1111111110101011", "1111111110101100", "1111111110101101", "1111111110101110" },
                                                                        { "", "1111010", "11111111000", "1111111110101111", "1111111110110000", "1111111110110001", "1111111110110010", "1111111110110011", "1111111110110100", "1111111110110101", "1111111110110110" },
                                                                        { "", "11111001", "1111111110110111", "1111111110111000", "111111110111001", "1111111110111010", "1111111110111011", "1111111110111100", "1111111110111101", "1111111110111110", "1111111110111111" },
                                                                        { "", "111110111", "1111111111000000", "1111111111000001", "1111111111000010", "1111111111000011", "1111111111000100", "1111111111000101", "1111111111000110", "1111111111000111", "1111111111001000" },
                                                                        { "", "111111000", "1111111111001001", "1111111111001010", "1111111111001011", "1111111111001100", "1111111111001101", "1111111111001110", "1111111111001111", "1111111111010000", "1111111111010001" },
                                                                        { "", "111111001", "1111111111010010", "1111111111010011", "1111111111010100", "1111111111010101", "1111111111010110", "1111111111010111", "1111111111011000", "1111111111011001", "1111111111011010" },
                                                                        { "", "111111010", "1111111111011011", "1111111111011100", "1111111111011101", "1111111111011110", "1111111111011111", "1111111111100000", "1111111111100001", "1111111111100010", "1111111111100011" },
                                                                        { "", "11111111001", "1111111111100100", "1111111111100101", "1111111111100110", "1111111111100111", "1111111111101000", "1111111111101001", "1111111111101010", "1111111111101011", "1111111111101100" },
                                                                        { "", "11111111100000", "1111111111101101", "1111111111101110", "1111111111101111", "1111111111110000", "1111111111110001", "1111111111110010", "1111111111110011", "1111111111110100", "1111111111110101" },
                                                                        { "1111111010", "111111111000011", "1111111111110110", "1111111111110111", "1111111111111000", "1111111111111001", "1111111111111010", "1111111111111011", "1111111111111100", "1111111111111101", "1111111111111110" } };
        #endregion StandardTables
        #region Flags
        private static readonly byte HuffmanMarker = 0xC4;

        private static readonly byte HuffmanYACMarker = 0x10;
        private static readonly byte HuffmanYDCMarker = 0x00;
        private static readonly byte HuffmanCrACMarker = 0x11;
        private static readonly byte HuffmanCrDCMarker = 0x01;

        private static readonly byte[] LuminanceDCHuffmanMetadataLength = { 0x00, 0x1F };
        private static readonly byte[] LuminanceDCHuffmanMetadata = { 0x00, 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B };
        private static readonly byte[] LuminanceACHuffmanMetadataLength = { 0x00, 0xB5 };
        private static readonly byte[] LuminanceACHuffmanMetadata = { 0x00, 0x02, 0x01, 0x03, 0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D, 0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08, 0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA };

        private static readonly byte[] ChrominanceDCHuffmanMetadataLength = { 0x00, 0x1F };
        private static readonly byte[] ChrominanceDCHuffmanMetadata = { 0x00, 0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B };
        private static readonly byte[] ChrominanceACHuffmanMetadataLength = { 0x00, 0xB5 };
        private static readonly byte[] ChrominanceACHuffmanMetadata = { 0x00, 0x02, 0x01, 0x02, 0x04, 0x04, 0x03, 0x04, 0x07, 0x05, 0x04, 0x04, 0x00, 0x01, 0x02, 0x77, 0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21, 0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71, 0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91, 0xA1, 0xB1, 0xC1, 0x09, 0x23, 0x33, 0x52, 0xF0, 0x15, 0x62, 0x72, 0xD1, 0x0A, 0x16, 0x24, 0x34, 0xE1, 0x25, 0xF1, 0x17, 0x18, 0x19, 0x1A, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA };

        private static readonly byte ControlFlag = 0xFF;
        private static readonly byte SOI = 0xD8;
        private static readonly byte EOI = 0xD9;
        private static readonly byte APP0 = 0xE0;
        private static readonly byte[] APP0Length = { 0x00, 0x10 };
        private static readonly byte[] JFIF = { 0x4A, 0x46, 0x49, 0x46, 0x00 };
        private static readonly byte[] VersionJFIF1 = { 0x01, 0x01 };
        private static readonly byte[] APP0Density = { 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] NoThumbnail = { 0x00, 0x00 };

        private static readonly byte QuantizationMarker = 0xDB;
        private static readonly byte[] QuantizationMarkerLength = { 0x00, 0x43 };
        private static readonly byte QuantizationYMarkerMeta = 0x00;
        private static readonly byte QuantizationCrMarkerMeta = 0x01;

        private static readonly byte StartOfTheFrameMarker = 0xC0;
        private static readonly byte[] StartOfTheFrameLength = { 0x00, 0x11 };
        private static readonly byte DataPrecisionStartOfTheFrame = 0x08;
        private static readonly byte[] StartOfTheFrameComponents = { 0x03, 0x01, 0x11, 0x00, 0x02, 0x11, 0x01, 0x03, 0x11, 0x01 };

        private static readonly byte StartOfScanMarker = 0xDA;
        private static readonly byte[] StartOfScanLength = { 0x00, 0x0C };
        private static readonly byte[] StartOfScanComponents = { 0x03, 0x01, 0x00, 0x02, 0x11, 0x03, 0x11 };
        private static readonly byte[] StartOfScanEmpty = { 0x00, 0x3F, 0x00 };

        private static readonly byte NonControlFlagMarker = 0x00;
        #endregion Flags

        // Use standard matrix size.
        private const int MatrixSize = 8;

        // Since we are encoding DC diff instead of absolute value, we need to keep track of last's block DC value.
        private int lastYDC = 0;
        private int lastCbDC = 0;
        private int lastCrDC = 0;

        // Keep track of what byte/bit are we encoding/decoding. We are doing this to optimse memory usage.
        private int currentByte = 0;
        private int currentBit = 0;

        private Dictionary<string, int> inverseLuminanceDCHuffmanTable = new Dictionary<string, int>();
        private Dictionary<string, int> inverseChrominanceDCHuffmanTable = new Dictionary<string, int>();
        private Dictionary<string, Tuple<int, int>> inverseLuminanceACHuffmanTable = new Dictionary<string, Tuple<int, int>>();
        private Dictionary<string, Tuple<int, int>> inverseChrominanceACHuffmanTable = new Dictionary<string, Tuple<int, int>>();


        #region Headers

        // Encode start header.
        //
        private void EncodeStartHeader(FileStream fileStream)
        {
            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(SOI);

            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(APP0);

            fileStream.Write(APP0Length, 0, APP0Length.Length);

            fileStream.Write(JFIF, 0, JFIF.Length);
            fileStream.Write(VersionJFIF1, 0, VersionJFIF1.Length);

            fileStream.Write(APP0Density, 0, APP0Density.Length);
            fileStream.Write(NoThumbnail, 0, NoThumbnail.Length);
        }

        // Encode quantization table header.
        //
        private void EncodeQuantizationTable(FileStream fileStream, bool isLuminance)
        {
            int[] serializedList;
            StringBuilder encodedTable = new StringBuilder();

            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(QuantizationMarker);
            fileStream.Write(QuantizationMarkerLength, 0, QuantizationMarkerLength.Length);

            if (isLuminance)
            {
                fileStream.WriteByte(QuantizationYMarkerMeta);
                serializedList = ZigZagSerialization(LuminanceQuantizationTable);
            }
            else
            {
                fileStream.WriteByte(QuantizationCrMarkerMeta);
                serializedList = ZigZagSerialization(ChrominanceQuantizationTable);
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
            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(StartOfTheFrameMarker);
            fileStream.Write(StartOfTheFrameLength, 0, StartOfTheFrameLength.Length);
            fileStream.WriteByte(DataPrecisionStartOfTheFrame);

            var pictureHeight = Helper.ConvertIntToByteArray(picture.GetDisplayHeight());
            fileStream.Write(pictureHeight, 0, pictureHeight.Length);

            var pictureWidth = Helper.ConvertIntToByteArray(picture.GetDisplayWidth());
            fileStream.Write(pictureWidth, 0, pictureWidth.Length);

            fileStream.Write(StartOfTheFrameComponents, 0, StartOfTheFrameComponents.Length);
        }

        // Encode huffman table header.
        //
        private void EncodeHuffmanTable(FileStream fileStream, bool isLuminance, bool isDCComponent)
        {
            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(HuffmanMarker);

            if (isLuminance)
            {
                var current = isDCComponent ? LuminanceDCHuffmanMetadataLength : LuminanceACHuffmanMetadataLength;
                fileStream.Write(current, 0, current.Length);

                fileStream.WriteByte(isDCComponent ? HuffmanYDCMarker : HuffmanYACMarker);

                current = isDCComponent ? LuminanceDCHuffmanMetadata : LuminanceACHuffmanMetadata;
                fileStream.Write(current, 0, current.Length);
            }
            else
            {
                var current = isDCComponent ? ChrominanceDCHuffmanMetadataLength : ChrominanceACHuffmanMetadataLength;
                fileStream.Write(current, 0, current.Length);

                fileStream.WriteByte(isDCComponent ? HuffmanCrDCMarker : HuffmanCrACMarker);

                current = isDCComponent ? ChrominanceDCHuffmanMetadata : ChrominanceACHuffmanMetadata;
                fileStream.Write(current, 0, current.Length);
            }
        }

        // Encode start of the scan header.
        //
        private void EncodeStartOfScan(FileStream fileStream)
        {
            fileStream.WriteByte(ControlFlag);
            fileStream.WriteByte(StartOfScanMarker);
            fileStream.Write(StartOfScanLength, 0, StartOfScanLength.Length);
            fileStream.Write(StartOfScanComponents, 0, StartOfScanComponents.Length);
            fileStream.Write(StartOfScanEmpty, 0, StartOfScanEmpty.Length);
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

        public JstegEncryptor()
        {
            Initialize();
        }

        // Initiazes JStegEncryptor. It creates inverse tables.
        private void Initialize()
        {
            // Populate inverse luminance DC huffman table
            for (int category = 0; category < LuminanceDCHuffmanTable.Length; category++)
            {
                inverseLuminanceDCHuffmanTable.Add(LuminanceDCHuffmanTable[category], category);
            }

            // Populate inverse chrominance DC huffman table
            for (int category = 0; category < ChrominanceDCHuffmanTable.Length; category++)
            {
                inverseChrominanceDCHuffmanTable.Add(ChrominanceDCHuffmanTable[category], category);
            }

            // Populate inverse luminance AC huffman table
            for (int category = 0; category < LuminanceACHuffmanTable.GetLength(1); category++)
            {
                for (int zeroesBefore = 0; zeroesBefore < LuminanceACHuffmanTable.GetLength(0); zeroesBefore++)
                {
                    if (LuminanceACHuffmanTable[zeroesBefore, category] != "")
                    {
                        inverseLuminanceACHuffmanTable.Add(LuminanceACHuffmanTable[zeroesBefore, category], new Tuple<int, int>(category, zeroesBefore));
                    }
                }
            }

            // Populate inverse chrominance AC huffman table
            for (int category = 0; category < ChrominanceACHuffmanTable.GetLength(1); category++)
            {
                for (int zeroesBefore = 0; zeroesBefore < ChrominanceACHuffmanTable.GetLength(0); zeroesBefore++)
                {
                    if (ChrominanceACHuffmanTable[zeroesBefore, category] != "")
                    {
                        inverseChrominanceACHuffmanTable.Add(ChrominanceACHuffmanTable[zeroesBefore, category], new Tuple<int, int>(category, zeroesBefore));
                    }
                }
            }
        }

        // Get next bit for encoding.
        //
        private int CreateMask()
        {
            int mask = 0x00;
            for (int i = 0; i < lsbUsed; i++)
            {
                mask <<= 1;
                mask = mask | encryptionMessage.GetNextBit();
            }

            return mask;
        }

        // Get next bit for encoding.
        //
        private int CreateRemoverMask()
        {
            return (int)(0xFFFFFFFE << (lsbUsed - 1));
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
        public int GetPreviousDCValue(ComponentType type)
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
        public void SetPreviousDCValue(ComponentType type, int newValue)
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
        internal string GetHuffmanDCTableValue(ComponentType type, int category)
        {
            if (type == ComponentType.Y)
            {
                return LuminanceDCHuffmanTable[category];
            }
            else
            {
                return ChrominanceDCHuffmanTable[category];
            }
        }

        // Gets value from Huffman standard AC table based on type, category and number of zeroes before nonZero AC compenent.
        //
        internal string GetHuffmanACTableValue(ComponentType type, int category, int zeroesBeforeValue = 0)
        {
            if (type == ComponentType.Y)
            {
                return LuminanceACHuffmanTable[zeroesBeforeValue, category];
            }
            else
            {
                return ChrominanceACHuffmanTable[zeroesBeforeValue, category];
            }
        }

        // Gets a value from standard quantization table.
        //
        internal double GetQuantizationTableElement(ComponentType type, int u, int v)
        {
            if (type == ComponentType.Y)
            {
                return LuminanceQuantizationTable[u, v];
            }
            else
            {
                return ChrominanceQuantizationTable[u, v];
            }
        }

        // Takes a quantized Block and encodes it using standard huffman tables.
        //
        internal String EncodeBlock(int[,] quantizedBlock, ComponentType type, bool encodeMessage)
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
                        potentialAdded = potentialAdded + GetHuffmanACTableValue(type, 0, zeroesBeforeCurrent - 1);
                        zeroesBeforeCurrent = 0;
                    }
                }
                else
                {
                    completeString = completeString + potentialAdded;
                    potentialAdded = "";

                    int currentElem = serializedList[elemNum];
                    if (encodeMessage)
                    {
                        if ((currentElem < 0 || currentElem > (Math.Pow(2, lsbUsed) - 1)) && !encryptionMessage.IsWholeMessageEncoded())
                        {
                            currentElem &= CreateRemoverMask();
                            currentElem |= CreateMask();
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
        internal String EncodeDCComponent(ComponentType type, int newDC)
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
        internal int DetermineCategory(int val)
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
        internal StringBuilder DetermineMagnitude(int val)
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

            return magnitude;
        }

        // Serializes block using zig-zag order.
        //
        internal int[] ZigZagSerialization(int[,] quantizedBlock, int matixSize = MatrixSize)
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
        internal int[,] Quantization(double[,] block, ComponentType type)
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
        internal string EncodeWholeColorBlock(ColorYCbCrBlock colorYCbCrBlock, bool encodeMessage)
        {
            string[] ret = new string[3];

            Parallel.For((int)ComponentType.Y, (int)ComponentType.Cr + 1, ComponentType =>
            {
                double[,] dctBlock = colorYCbCrBlock.DCTTransformation((ComponentType)ComponentType);
                int[,] quantizedBlock = Quantization(dctBlock, (ComponentType)ComponentType);
                ret[ComponentType] += EncodeBlock(quantizedBlock, (ComponentType)ComponentType, encodeMessage);
            });

            return ret[0] + ret[1] + ret[2];
        }

        // Returns inverse lumiance/chromiance DC huffman table.
        //
        internal Dictionary<string, int> GetInverseDCHuffmanTable(bool isLuminance)
        {
            if (isLuminance)
            {
                return inverseLuminanceDCHuffmanTable;
            }
            else
            {
                return inverseChrominanceDCHuffmanTable;
            }
        }

        // Returns inverse lumiance/chromiance AC huffman table.
        //
        internal Dictionary<string, Tuple<int, int>> GetInverseACHuffmanTable(bool isLuminance)
        {
            if (isLuminance)
            {
                return inverseLuminanceACHuffmanTable;
            }
            else
            {
                return inverseChrominanceACHuffmanTable;
            }
        }

        // Find start of the scan frame.
        //
        internal long FindStartOfTheScanFrame(FileStream fs)
        {
            
            byte prev = 0;
            byte current = (byte)fs.ReadByte();

            // Search for start of scan.
            while (current != StartOfScanMarker || prev != ControlFlag)
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
        internal string CompressPicture(string picturePath, bool encodeMessage)
        {
            lastYDC = 0;
            lastCbDC = 0;
            lastCrDC = 0;
            PictureYCbCr pictureYCbCr = new PictureYCbCr(new Bitmap(picturePath));

            var outputFileName = outputPath + "\\JpegCompressionTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".jpg";
            using (FileStream fs = File.Create(outputFileName))
            {
                EncodeHeaders(fs, pictureYCbCr);

                StringBuilder encodedPicture = new StringBuilder();

                // Pass through all of the blocks and encode each of them.
                double textPercent = pictureYCbCr.height / 100.0;
                int percent = 1;
                for (int blockY = 0; blockY < pictureYCbCr.height; blockY++)
                {
                    for (int blockX = 0; blockX < pictureYCbCr.width; blockX++)
                    {
                        ColorYCbCrBlock currentBlock = pictureYCbCr.blocks[blockX, blockY];
                        encodedPicture.Append(EncodeWholeColorBlock(currentBlock, encodeMessage: encodeMessage));
                    }

                    while (blockY + 1 >= textPercent)
                    {
                        textPercent += pictureYCbCr.height / 100.0;
                        parentForm.SetCurrentProgress(percent++);
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
                        fs.WriteByte(NonControlFlagMarker);
                    }
                }

                // Encode end of the file.
                fs.WriteByte(ControlFlag);
                fs.WriteByte(EOI);
                parentForm.SetCurrentProgress(100);
            }

            return outputFileName;
        }

        // Main function used to encrypt message.
        //
        internal override string EncryptPicture(string picturePath, string messagePath)
        {
            SetEncryptionMessage(messagePath);
            string outputFileName = CompressPicture(picturePath, encodeMessage: true);

            // Throw error if message is longer than capacity. We are doing this now,
            // because we cannot determine picture capacity before jpeg compression, therefore we don't know if message is too long to encode before this point.
            if (!encryptionMessage.IsWholeMessageEncoded())
            {
                throw new Exception(String.Format("Message is too long. Capacity is {0}, message length is {1}", encryptionMessage.GetCurrentBit() / 8, encryptionMessage.GetMessageSize() / 8));
            }

            return outputFileName;
        }

        // Main function used to decrypt message. Note: We are assuming that all of the standard tables are being used, and same options as in encrypting algorithm.
        //
        public override string DecryptPicture(string picturePath, string outputPath)
        {
            List<Boolean> messageInBits = new List<Boolean>();

            using (FileStream fs = File.OpenRead(picturePath))
            {
                long startPosition = FindStartOfTheScanFrame(fs);

                byte[] scanFrameBuffer = new byte[fs.Length - startPosition - 2];
                fs.Read(scanFrameBuffer, 0, (int)(fs.Length - startPosition - 2));

                string currentKey = "";
                bool messageSizeFound = false;
                uint messageSize = 0;

                double textPercent = 0;
                int percent = 1;
                while (!messageSizeFound || messageSize * 8 > messageInBits.Count)
                {
                    if (!messageSizeFound && messageInBits.Count > 32)
                    {
                        messageSize = Helper.GetMessageSizeFromBoolArray(messageInBits);
                        messageSizeFound = true;
                        textPercent = (messageSize * 8) / 100.0;

                        Console.WriteLine(String.Format("Found message legth: {0}", messageSize));

                        // Remove first 16bits as they represent message size.
                        messageInBits.RemoveRange(0, 16);
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
                                    List<bool> tempList = new List<bool>();
                                    for (int i = 0; i < GetInverseACHuffmanTable(type == 0)[currentKey].Item1; i++)
                                    {
                                        tempList.Add(GetNextBitFromEncryptedPicture(scanFrameBuffer));
                                    }

                                    // Add only if temp != 1 and we haven't filled messageInBits to the messageSize * 8.
                                    if (!(tempList.Count == 1 && tempList[0]) && (!messageSizeFound || messageInBits.Count < messageSize * 8))
                                    {
                                        messageInBits.Add(tempList[0] ? tempList[tempList.Count - 1] : !tempList[tempList.Count - 1]);
                                    }
                                }

                                currentKey = "";
                            }
                        }
                    }

                    while (messageSizeFound && messageInBits.Count >= textPercent)
                    {
                        textPercent += (messageSize * 8) / 100.0;
                        parentForm.SetCurrentProgress(percent++);
                    }
                }

                parentForm.SetCurrentProgress(100);
                var outputFileName = outputPath + "\\JpegDecryptionTest_" + DateTime.UtcNow.ToFileTimeUtc() + ".txt";
                File.WriteAllText(outputFileName, Helper.CreateStringFromBoolArray(messageInBits));
                return outputFileName;
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
                double textPercent = scanFrameBuffer.Length / 100.0;
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
                                    List<bool> tempList = new List<bool>();
                                    int coef = 0;

                                    for (int i = 0; i < GetInverseACHuffmanTable(type == 0)[currentKey].Item1; i++)
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

                    if (currentByte >= textPercent)
                    {
                        double probability = CalculateProbabilityAtPoint(histogram_cr);
                        PercentChart.Series["Cr"].Points.AddXY(percent, probability * 100);

                        probability = CalculateProbabilityAtPoint(histogram_cb);
                        PercentChart.Series["Cb"].Points.AddXY(percent, probability * 100);

                        probability = CalculateProbabilityAtPoint(histogram_y);
                        PercentChart.Series["Y"].Points.AddXY(percent, probability * 100);
                        textPercent += scanFrameBuffer.Length / 100.0;
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
    }
}
