using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    class Helper
    {
        public static bool IsPrime(ulong number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (uint i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        public static bool IsParityBitOne(ulong value)
        {
            ulong mask = 0x1;
            if ((value & 0xFFFF000000000000) != 0)
            {
                throw new Exception("Message is too long");
            }

            int bitCount = 0;

            for (int i = 0; i < 48; i++)
            {
                if ((value & mask) != 0)
                {
                    bitCount++;
                }

                mask = mask << 1;
            }

            return (bitCount % 2) == 0;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] ConvertIntToByteArray(int integer)
        {
            byte[] bytes = new byte[2];

            bytes[0] = (byte)(integer >> 8);
            bytes[1] = (byte)(integer);

            return bytes;
        }

        public static byte[] BinaryStringToByteArray(StringBuilder binaryString)
        {

            while (binaryString.Length % 8 != 0)
            {
                binaryString.Append('1');
            }

            string newString = binaryString.ToString();

            return Enumerable.Range(0, newString.Length)
                             .Where(x => x % 8 == 0)
                             .Select(x => Convert.ToByte(newString.Substring(x, 8), 2))
                             .ToArray();
        }

        public static uint GetMessageSizeFromBoolArray(List<bool> boolList)
        {
            uint ret = 0;
            for (int i = 0; i <= 31; i++)
            {
                ret <<= 1;
                if (boolList[i])
                {
                    ret += 1;
                }
            }

            return ret;
        }

        public static ulong CreateLongFromBoolArray(List<bool> boolList)
        {
            ulong ret = 0;
            for (int i = 0; i < boolList.Count; i++)
            {
                ret <<= 1;
                if (boolList[i])
                {
                    ret += 1;
                }
            }

            return ret;
        }
    }
}
