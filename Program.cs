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

            Console.WriteLine("[+] 1. Encrypt Image\n[+] 2. Decrypt Image\n[+] 3. Decrypt image by name (saved in data.json)");

            ConsoleKeyInfo key = Console.ReadKey(true);

            while (true)
            {
                if (key.Key == ConsoleKey.D1)
                {
                    EncryptImageAsync();
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    DecryptImageAsync(false);
                }
                else if (key.Key == ConsoleKey.D3)
                {
                    DecryptImageAsync(true);
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

                Config.SaveEncryptedImage(new Config.ConfigData(Path.GetFileName(imagePath), encryptedImage, new DateTime().ToLocalTime().ToString()));

                Console.WriteLine("Image encrypted and saved in data.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        public static void DecryptImageAsync(bool storage)
        {
            try
            {
                if (storage)
                {
                    DecryptLocalImage();
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Please enter the encrypted image to decrypt:");
                string encryptedImage = Console.ReadLine();

                DecryptImage(encryptedImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to decrypt image: {ex.Message}");
            }
        }

        public static void DecryptImage(string encryptedImage)
        {
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

        public static void DecryptLocalImage()
        {
            Console.WriteLine("Please enter image name:");
            string imageName = Console.ReadLine();
            Config.ConfigData configData = Config.FindDataByName(imageName);

            if (configData != null)
            {
                DecryptImage(configData.encryptedImage);
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