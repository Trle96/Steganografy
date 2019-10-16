using System;
using System.Drawing;
using static DiplRad.YCbCrPixel;

namespace DiplRad
{
    class ColorYCbCrBlock
    {
        public YCbCrPixel[,] colorBlock = new YCbCrPixel[8, 8];

        public void SetPixel(Color c, int x, int y)
        {
            colorBlock[x, y] = new YCbCrPixel(c);
        }

        public ColorYCbCrBlock()
        {

        }

        internal double[,] DCTTransformation(ComponentType type)
        {
            double[,] dctBlock = new double[8, 8];
            double helpingDouble = 1 / Math.Sqrt(2);
            double current;

            for (double u = 0; u <= 7; u++)
            {
                for (double v = 0; v <= 7; v++)
                {
                    current = 0.25 * (u == 0 ? helpingDouble : 1) * (v == 0 ? helpingDouble : 1);
                    current *= this.CalculateSum(u, v, type);
                    dctBlock[(int)u, (int)v] = current;
                }
            }

            return dctBlock;
        }

        internal double CalculateSum(double u, double v, ComponentType type)
        {
            double sumResult = 0;
            for (double x = 0; x <= 7; x++)
            {
                for (double y = 0; y <= 7; y++)
                {
                    sumResult += this.colorBlock[(int)x, (int)y].GetComponent(type) * Math.Cos(((2 * x + 1) * u * Math.PI) / 16) * Math.Cos(((2 * y + 1) * v * Math.PI) / 16);
                }
            }
            return sumResult;
        }
    }
}

