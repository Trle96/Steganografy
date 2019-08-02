using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace DiplRad
{
    class YCbCrPixel
    {
        internal double _y;
        internal double _Cb;
        internal double _Cr;

        public YCbCrPixel(int yComp, int cbComp, int crComp)
        {
            _y = yComp;
            _Cb = cbComp;
            _Cr = crComp;
        }

        public YCbCrPixel(Color color)
        {
            _y = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B - 128;
            _Cb = - 0.1687 * color.R - 0.3313 * color.G + 0.500 * color.B - 128;
            _Cr = 0.500 * color.R - 0.4187 * color.G - 0.0813 * color.B - 128;
        }

        public double GetComponent(int component)
        {
            switch (component)
            {
                case 1:
                    return _y;
                case 2:
                    return _Cb;
                default:
                    return _Cr;
            };
        }
    }
    
    class Program
    {
        static private JstegEncryptor jstegEncryptor = new JstegEncryptor();

        static void Main(string[] args)
        {
            System.Console.WriteLine("Please enter path to your picture");
            string picturePath = System.Console.ReadLine();
            System.Console.WriteLine("Please enter path to txt file containg message");
            string messagePath = System.Console.ReadLine();
            System.Console.WriteLine("Choose encryption algorithm. Type:\n 1. For Jsteg algorithm\n 2. For PIT algorithm");
            string algorithm = System.Console.ReadLine();

            switch (algorithm)
            {
                case "1":
                    jstegEncryptor.EncryptPicture(picturePath, messagePath);
                    break;
                default:
                    System.Console.WriteLine("Wrong option");
                    break;
            }
        }
    }

    class JstegEncryptor
    {
        internal const byte BitMask = 0x01;
        private static readonly int[,] LuminanceQuantizationTable = { {16, 11, 10, 16, 24, 40, 51, 61 },
                                                                        {12, 12, 14, 19, 26, 58, 60, 55 },
                                                                        {14, 13, 16, 24, 40, 57, 69, 56 },
                                                                        {14, 17, 22, 29, 51, 87, 80, 62 },
                                                                        {18, 22, 37, 56, 68, 109, 103, 77 },
                                                                        {24, 35, 55, 64, 81, 104, 113, 92 },
                                                                        {49, 64, 78, 87, 103, 121, 120, 101 },
                                                                        {72, 92, 95, 98, 112, 100, 103, 99 } };

        private static readonly int[,] ChrominanceQuantizationTable = { { 17, 18, 24, 47, 99, 99, 99, 99 },
                                                                         {18, 21, 26, 66, 99, 99, 99, 99 },
                                                                         {24, 26, 56, 99, 99, 99, 99, 99 },
                                                                         {47, 66, 99, 99, 99, 99, 99, 99 },
                                                                         {99, 99, 99, 99, 99, 99, 99, 99 },
                                                                         {99, 99, 99, 99, 99, 99, 99, 99 },
                                                                         {99, 99, 99, 99, 99, 99, 99, 99 },
                                                                         {99, 99, 99, 99, 99, 99, 99, 99 } };

        private const int MatrixSize = 8;


        internal static int x = 0, y = 0, z = 0;

        internal List<Boolean> _messageInBits;
        internal Bitmap _picturePNG;

        internal int GetQuantizationTableElement(int u, int v, int component)
        {
            switch (component)
            {
                case 1:
                    return LuminanceQuantizationTable[u,v];
                default:
                    return ChrominanceQuantizationTable[u, v];
            };
        }

        /*internal static void SetNextPixel(int mark)
        {
            z++;
            if (z == 3)
            {
                z = 0;
                y += 1;
            }
            if (y == b.Height)
            {
                x += 1;
                y = 0;
            }

            Color pixelColor = b.GetPixel(x, y);
            Color newColor = Color.FromArgb(0, 0, 0);
            switch (z)
            {
                case 0:
                    newColor = Color.FromArgb((pixelColor.R & 0xFE) | mark, pixelColor.G, pixelColor.B);
                    break;
                case 1:
                    newColor = Color.FromArgb(pixelColor.R, (pixelColor.G & 0xFE) | mark, pixelColor.B);
                    break;
                case 2:
                    newColor = Color.FromArgb(pixelColor.R, pixelColor.G, (pixelColor.B & 0xFE) | mark);
                    break;
            }

            b.SetPixel(x, y, newColor);
        }

        internal static int GetNextByte()
        {
            z++;
            if (z == 3)
            {
                z = 0;
                y += 1;
            }
            if (y == b.Height)
            {
                x += 1;
                y = 0;
            }

            Color pixelColor = b.GetPixel(x, y);
            switch (z)
            {
                case 0:
                    return pixelColor.R & 1;
                case 1:
                    return pixelColor.G & 1;
                case 2:
                    return pixelColor.B & 1;
                default:
                    return 0;
            }
        }*/

        internal static void CompressPicture(Bitmap picture)
        {
            //YCbCrPixel

            for (int x  = 0; x <= picture.Width / 8; x++)
            {
                for (int y = 0; y <= picture.Height / 8; y++)
                {

                }
            }
        }

        internal static List<Boolean> convertTxtToBitArray(string picturePath)
        {
            List<Boolean> convertedList = new List<Boolean>();
            using (StreamReader streamReader = new StreamReader(picturePath))
            {
                var nextChar = streamReader.Read();
                // Trasverse each bit in char and store it in array. 
                // We want to store bits from highest to lowset, hence using descending for loop(7 being highest bit, 0 lowest).
                for (int i = 7; i > 0; i--)
                { 
                    convertedList.Add((nextChar & (BitMask << i)) > 0);
                }
            }

            return convertedList;
        }

        internal int[,] Quantization(double[,] block, int component)
        {
            int[,] quantizedBlock = new int[8, 8];
            for (int u = 0; u < 7; u++)
            {
                for (int v = 0; v < 7; v++)
                {
                    quantizedBlock[u, v] = (int)Math.Round(block[u, v] / GetQuantizationTableElement(u, v, component)) + 128;
                }
            }

            return quantizedBlock;
        }

        internal List<Tuple<int, int>> SerializeAndApplyRLE(int[,] quantizedBlock)
        {
            List<Tuple<int, int>> rleList = new List<Tuple<int, int>>();
            List<int> serializedList = ZigZagSerialization(quantizedBlock, 4);

            int zeroesBeforeCurrent = 0;
            for (int elemNum = 0; elemNum < serializedList.Count; elemNum++)
            {
                if (serializedList[elemNum] == 0)
                {
                    zeroesBeforeCurrent++;
                }
                else
                {
                    rleList.Add(new Tuple<int, int>(zeroesBeforeCurrent, serializedList[elemNum]));
                    zeroesBeforeCurrent = 0;
                }
            }


            return rleList;
        }

        internal List<int> ZigZagSerialization(int[,] quantizedBlock, int matixSize = MatrixSize)
        {
            List<int> zigZagList = new List<int>();
            bool ascDirection = false;
            int currentIteration = 0;
            int x = 0, y = 0;

            while (currentIteration < matixSize)
            {
                while ((!ascDirection && x != 0) || (ascDirection && y != 0))
                {
                    zigZagList.Add(quantizedBlock[x, y]);
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

                zigZagList.Add(quantizedBlock[x, y]);

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
                    zigZagList.Add(quantizedBlock[x, y]);
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

                zigZagList.Add(quantizedBlock[x, y]);

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

        internal double[,] DCTTransformation(YCbCrPixel[,] block, int component)
        {
            double[,] dctBlock = new double[8, 8];
            double current;

            for (int u = 0; u < 7; u++)
            {
                for (int v = 0; v < 7; v++)
                {
                    current = 1 / 4 * (u == 0 ? 1 / Math.Sqrt(2) : 1) * (v == 0 ? 1 / Math.Sqrt(2) : 1);
                    current *= CalculateSum(block, u, v, component);
                    dctBlock[u, v] = current;
                }
            }

            return dctBlock;
        }

        internal double CalculateSum(YCbCrPixel[,] block, int u, int v, int component)
        {
            double sumResult = 0;
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    sumResult += block[x, y].GetComponent(component) * Math.Cos(((2 * x + 1) * u * Math.PI) / 16) * Math.Cos(((2 * y + 1) * u * Math.PI) / 16);
                }
            }
            return sumResult;
        }

        internal void EncryptPicture(string picturePath, string messagePath)
        {
            //_messageInBits = convertTxtToBitArray(picturePath);
            //_picturePNG = new Bitmap(picturePath);
            int[,] test = { {20, 17, 0, 11 },
                            {0, 0, 0, 1 },
                            {0, -10, 0, 0 },
                            {-5, 0, 0, 0} };

            var test2 = SerializeAndApplyRLE(test);
            foreach (Tuple<int, int> elem in test2)
            {
                Console.WriteLine(elem.Item1 + ", " + elem.Item2);
            }

            /*b.Save("C:\\Users\\Djordje Nisic\\Desktop\\Photographer_Barnstar22.png");

            x = 0;
            y = 0;
            z = 0;

            List<byte> termsList = new List<byte>();

            for (int i = 0; i < 1000; i++)
            {
                var mask = 1;
                byte noviByte = new byte();
                for (int j = 0; j < 8; j++)
                {
                    noviByte = (byte)((int)noviByte | (GetNextByte() * mask));
                    mask *= 2;
                }
                termsList.Add(noviByte);
            }

            Console.WriteLine(System.Text.Encoding.Default.GetString(termsList.ToArray()));*/
        }
    }
}
