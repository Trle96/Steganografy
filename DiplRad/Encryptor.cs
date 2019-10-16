using System;
using System.Collections.Generic;
using System.IO;

namespace DiplRad
{
    abstract class Encryptor
    {
        internal const byte BitMask = 0x01;
        internal string outputPath;
        internal abstract void EncryptPicture(string picturePath, string messagePath);

        internal abstract void DecryptPicture(string picturePath);

        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal static List<Boolean> ConvertTxtToBitArray(string messagePath, out ulong charCount)
        {
            List<Boolean> convertedList = new List<Boolean>();
            charCount = 0;
            int nextChar;
            using (StreamReader streamReader = new StreamReader(messagePath))
            {
                while ((nextChar = streamReader.Read()) != -1)
                {
                    charCount++;

                    // Trasverse each bit in char and store it in array. 
                    // We want to store bits from highest to lowset, hence using descending FOR loop (7 being highest bit, 0 lowest).
                    for (int i = 7; i >= 0; i--)
                    {
                        convertedList.Add((nextChar & (BitMask << i)) > 0);
                    }
                }
            }
            

            return convertedList;
        }

        internal static List<Boolean> ConvertTxtLenghthToBitArray(ulong charCount)
        {
            if (charCount >= Math.Pow(2, 16))
            {
                throw new Exception("Message is over maximum size of 65,536 characters");
            }

            List<Boolean> convertedList = new List<Boolean>();

            // Trasverse each bit in char and store it in array. 
            // We want to store bits from highest to lowset, hence using descending FOR loop (15 being highest bit, 0 lowest).
            for (int i = 15; i >= 0; i--)
            {
                convertedList.Add((charCount & (uint)(BitMask << i)) > 0);
            }

            return convertedList;
        }

        public Encryptor(string outputPath)
        {
            this.outputPath = outputPath;
        }
    }
}
