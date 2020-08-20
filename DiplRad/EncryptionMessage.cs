using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    class EncryptionMessage
    {
        internal const byte BitMask = 0x01;
        internal List<Boolean> _messageInBits;
        private int currentBit = 0; 
        private int messageSize;

        public int GetCurrentBit()
        {
            return currentBit;
        }

        public int GetMessageSize()
        {
            return messageSize;
        }

        public EncryptionMessage(string messagePath, bool insertMessageSizeAtBeggining)
        {
            _messageInBits = ConvertTxtToBitArray(messagePath, out messageSize);

            // Add message length at begging of message.
            if (insertMessageSizeAtBeggining)
            {
                _messageInBits.InsertRange(0, ConvertTxtLenghthToBitArray(messageSize));
            }

            messageSize = _messageInBits.Count();
        }

        // Get next bit for encoding.
        //
        public int GetNextBit()
        {
            if (currentBit >= _messageInBits.Count)
            {
                return 0x1;
            }

            if (_messageInBits[currentBit++])
            {
                return 0x01;
            }
            else
            {
                return 0x00;
            }
        }

        // Get next bit for encoding.
        //
        public bool IsWholeMessageEncoded()
        {
            if (currentBit >= _messageInBits.Count)
            {
                return true;
            }

            return false;
        }

        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal List<Boolean> ConvertTxtToBitArray(string messagePath, out int charCount)
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

        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal List<Boolean> ConvertTxtToEncryptedBitArray(string messagePath, out ulong charCount)
        {
            using (StreamReader streamReader = new StreamReader(messagePath))
            {
                string test = EncryptString("Steva123Steva123Steva123Steva123", streamReader.ReadToEnd());

                return ConvertStringToBitArray(test, out charCount);
            }
        }


        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal static List<Boolean> ConvertStringToBitArray(string message, out ulong charCount)
        {
            List<Boolean> convertedList = new List<Boolean>();
            charCount = (ulong)message.Length;
            for (int stringElem = 0; stringElem < message.Length; stringElem++)
            {
                // Trasverse each bit in char and store it in array. 
                // We want to store bits from highest to lowset, hence using descending FOR loop (7 being highest bit, 0 lowest).
                for (int i = 7; i >= 0; i--)
                {
                    convertedList.Add((message[stringElem] & (BitMask << i)) > 0);
                }
            }

            return convertedList;
        }

        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal static List<Boolean> ConvertByteArrayToBitArray(byte[] message, out int charCount)
        {
            List<Boolean> convertedList = new List<Boolean>();
            charCount = message.Length;
            for (int stringElem = 0; stringElem < message.Length; stringElem++)
            {
                // Trasverse each bit in char and store it in array. 
                // We want to store bits from highest to lowset, hence using descending FOR loop (7 being highest bit, 0 lowest).
                for (int i = 7; i >= 0; i--)
                {
                    convertedList.Add((message[stringElem] & (BitMask << i)) > 0);
                }
            }

            return convertedList;
        }

        internal static List<Boolean> ConvertTxtLenghthToBitArray(int charCount)
        {
            List<Boolean> convertedList = new List<Boolean>();

            // Trasverse each bit in char and store it in array. 
            // We want to store bits from highest to lowset, hence using descending FOR loop (15 being highest bit, 0 lowest).
            for (int i = 31; i >= 0; i--)
            {
                convertedList.Add((charCount & (uint)(BitMask << i)) > 0);
            }

            return convertedList;
        }

        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}
