using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    class StegoMessage
    {
        internal const byte BitMask = 0x01;
        internal List<Boolean> _messageInBits;
        private int currentBit = 0;
        private int encodedBits = 0;
        private readonly int totalBitCount;
        private readonly StegoOptions stegoOptions;

        public int GetCurrentBit()
        {
            return currentBit;
        }

        public int GetTotalBitCount()
        {
            return totalBitCount;
        }

        public StegoMessage(List<Boolean> messageInBits, StegoOptions stegoOptions)
        {
            this._messageInBits = messageInBits;
            this.stegoOptions = stegoOptions;

            totalBitCount = _messageInBits.Count();
        }

        public StegoMessage(string messagePath, StegoOptions so)
        {
            this.stegoOptions = so;
            int messageSize;

            if (stegoOptions.LoopMessage && stegoOptions.CompressMessage)
            {
                throw new Exception("Incompatable options");
            }

            string message;
            using (StreamReader streamReader = new StreamReader(messagePath))
            {
                message = streamReader.ReadToEnd();
            }

            if (stegoOptions.EncryptMessage)
            {
                message = EncryptString("Steva123Steva123Steva123Steva123", message);
            }

            if (stegoOptions.CompressMessage)
            {
                _messageInBits = ConvertByteArrayToBitArray(Zip(message), out messageSize);
            }
            else
            {
                _messageInBits = ConvertStringToBitArray(message, out messageSize);
            }

            // Add message length at begging of message.
            if (stegoOptions.InsertMessageSizeAtBeginning)
            {
                _messageInBits.InsertRange(0, ConvertTxtLenghthToBitArray(messageSize));
            }

            totalBitCount = _messageInBits.Count();
        }

        // Get next bit for encoding.
        //
        public int GetNextStegoMessageBit()
        {
            int ret;
            if (_messageInBits[currentBit++])
            {
                ret = 0x01;
            }
            else
            {
                ret = 0x00;
            }

            if (currentBit >= totalBitCount && stegoOptions.LoopMessage)
            {
                currentBit = stegoOptions.InsertMessageSizeAtBeginning ? 32 : 0;
            }

            encodedBits++;
            return ret;
        }

        // Get next bit for encoding.
        //
        public bool IsWholeMessageEncoded()
        {
            if (currentBit >= _messageInBits.Count && !stegoOptions.LoopMessage)
            {
                return true;
            }

            return false;
        }

        // Get next bit for encoding.
        //
        public bool IsLooped()
        {
            return stegoOptions.LoopMessage;
        }

        // Get next bit for encoding.
        //
        public bool IsLoopedMessageEncodedAtLeastOnce()
        {
            if (stegoOptions.LoopMessage && encodedBits > totalBitCount)
            {
                return true;
            }

            return false;
        }

        public string GenerateOutput()
        {
            string message;

            if (stegoOptions.CompressMessage)
            {
                message = Unzip(CreateByteArrayFromBoolArray(_messageInBits));
            }
            else
            {
                message = CreateStringFromBoolArray(_messageInBits);
            }

            if (stegoOptions.EncryptMessage)
            {
                message = DecryptString("Steva123Steva123Steva123Steva123", message);
            }

            return message;
        }

        // <summary>
        // Transform input message into boolean array, where each element represents one bit.
        // </summary>
        internal static List<Boolean> ConvertStringToBitArray(string message, out int charCount)
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

        internal string EncryptString(string key, string plainText)
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

        internal string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        internal void CopyTo(Stream src, Stream dest)
        {
            byte[] chars = new byte[409600];

            int cnt;

            while ((cnt = src.Read(chars, 0, chars.Length)) != 0)
            {
                dest.Write(chars, 0, cnt);
            }
        }

        internal byte[] Zip(string message)
        {
            using (var msi = new StringReader(message))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    foreach (var c in message)
                    {
                        gs.WriteByte((byte)c);
                    }
                }

                return mso.ToArray();
            }
        }

        internal string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        internal string CreateStringFromBoolArray(List<bool> boolList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < boolList.Count / 8; i++)
            {
                char newChar = (char)0;
                for (int j = 0; j < 8; j++)
                {
                    newChar <<= 1;
                    newChar |= boolList[i * 8 + j] ? (char)0x1 : (char)0x0;
                }

                stringBuilder.Append(newChar);
            }

            return stringBuilder.ToString();
        }

        internal byte[] CreateByteArrayFromBoolArray(List<bool> boolList)
        {
            byte[] byteArray = new byte[boolList.Count / 8];
            for (int i = 0; i < boolList.Count / 8; i++)
            {
                byte newChar = 0;
                for (int j = 0; j < 8; j++)
                {
                    newChar <<= 1;
                    newChar |= boolList[i * 8 + j] ? (byte)0x1 : (byte)0x0;
                }

                byteArray[i] = newChar;
            }

            return byteArray;
        }
    }
}
