using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ImageEncryptor
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Select an option:");

            Console.WriteLine("[+] 1. Encrypt Image\n[+] 2. Decrypt Image");

            ConsoleKeyInfo key = Console.ReadKey(true);

            while (true)
            {
                if (key.Key == ConsoleKey.D1)
                {
                    EncryptImageAsync();
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    DecryptImageAsync();
                }
            }
        }

        private static void EncryptImageAsync()
        {
            try
            {
                Console.WriteLine("Please enter the image path to encrypt:");
                string imagePath = Console.ReadLine();

                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    Console.WriteLine("Invalid or non-existing image path");
                    return;
                }

                Image image = Image.FromFile(imagePath);
                byte[] imageBytes = ImageToByteArray(image);

                string base64Image = Convert.ToBase64String(imageBytes);
                string encryptedImage = HashHelper.Encrypt(base64Image);

                Console.WriteLine("Encrypted Image Data:");
                Console.WriteLine(encryptedImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        public static void DecryptImageAsync()
        {
            try
            {
                Console.WriteLine("Please enter the encrypted image to decrypt:");
                string encryptedImage = Console.ReadLine();

                string decryptedBase64 = HashHelper.Decrypt(encryptedImage);
                byte[] imgBytes = Convert.FromBase64String(decryptedBase64);

                using (var ms = new MemoryStream(imgBytes))
                {
                    using (Bitmap bmp = new Bitmap(ms))
                    {
                        string outputFileName = "decrypted.png";
                        bmp.Save(outputFileName);
                        Console.WriteLine($"Image decrypted and saved as {outputFileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to decrypt image: {ex.Message}");
            }
        }

        private static byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}