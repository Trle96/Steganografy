using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace DiplRad
{
    class Program
    {
        static private JstegEncryptor jstegEncryptor;
        static private PITEncryptor pitEncryptor;

        static void Main(string[] args)
        {
            string picturePath;
            string messagePath;

            System.Console.WriteLine("Choose algorithm. Type:\n 1. For PIT encryption algorithm\n 2. For PIT decryption algorithm\n 3. For Jsteg encryption algorithm\n 4. For Jsteg decryption algorithm");
            string outputPath = System.Console.ReadLine();

            jstegEncryptor = new JstegEncryptor(outputPath);
            pitEncryptor = new PITEncryptor(outputPath);

            System.Console.WriteLine("Choose algorithm. Type:\n 1. For PIT encryption algorithm\n 2. For PIT decryption algorithm\n 3. For Jsteg encryption algorithm\n 4. For Jsteg decryption algorithm");
            string algorithm = System.Console.ReadLine();

            switch (algorithm)
            {
                case "1":
                    System.Console.WriteLine("Please enter path to your picture");
                    picturePath = System.Console.ReadLine();
                    System.Console.WriteLine("Please enter path to txt file containg message");
                    messagePath = System.Console.ReadLine();
                    pitEncryptor.EncryptPicture(picturePath, messagePath);
                    break;
                case "2":
                    System.Console.WriteLine("Please enter path to your picture");
                    picturePath = System.Console.ReadLine();
                    pitEncryptor.DecryptPicture(picturePath);
                    break;
                case "3":
                    System.Console.WriteLine("Please enter path to your picture");
                    picturePath = System.Console.ReadLine();
                    System.Console.WriteLine("Please enter path to txt file containg message");
                    messagePath = System.Console.ReadLine();
                    jstegEncryptor.EncryptPicture(picturePath, messagePath);
                    break;
                case "4":
                    System.Console.WriteLine("Please enter path to your picture");
                    picturePath = System.Console.ReadLine();
                    jstegEncryptor.DecryptPicture(picturePath);
                    break;
                default:
                    System.Console.WriteLine("Wrong option");
                    break;
            }
        }
    }
}
