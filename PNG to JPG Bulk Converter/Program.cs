using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string sourcePath = @"C:\jakaś_ścieżka_1"; // ścieżka do folderu z plikami źródłowymi .png
        string targetPath = @"C:\jakaś_ścieżka_2"; // ścieżka do folderu z przekonwertowanymi plikami .jpg

        Console.WriteLine("Wciśnij ENTER, aby zacząć");
        Console.ReadKey();

        // pobierz listę plików z rozszerzeniem .png
        string[] pngFiles = Directory.GetFiles(sourcePath, "*.png");

        // przejdź przez każdy plik i skonwertuj go na .jpg
        foreach (string pngFilePath in pngFiles)
        {
            // pobierz nazwę pliku bez rozszerzenia
            string fileName = Path.GetFileNameWithoutExtension(pngFilePath);

            // zmień rozszerzenie pliku na .jpg
            string newFilePath = Path.Combine(targetPath, fileName + ".jpg");

            // przejdź do następnego pliku, jeśli już istnieje plik o takiej nazwie z rozszerzeniem .jpg
            if (File.Exists(newFilePath))
            {
                continue;
            }

            // otwórz plik .png
            using (Image pngImage = Image.FromFile(pngFilePath))
            {
                // stwórz obiekt bitmapy o wymiarach obrazka .png
                using (Bitmap bitmap = new Bitmap(pngImage.Width, pngImage.Height))
                {
                    // skopiuj zawartość .png do bitmapy
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImage(pngImage, 0, 0, pngImage.Width, pngImage.Height);
                    }

                    // zapisz bitmapę w formacie .jpg z użyciem ustawień jakości
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 75L); // jakość kompresji 75%
                    encoderParams.Param[0] = qualityParam;
                    bitmap.Save(newFilePath, jpgEncoder, encoderParams);
                }
            }

            // skopiuj atrybuty pliku .png na plik .jpg
            FileInfo pngFileInfo = new FileInfo(pngFilePath);
            FileInfo jpgFileInfo = new FileInfo(newFilePath);
            jpgFileInfo.CreationTime = pngFileInfo.CreationTime;
            jpgFileInfo.LastWriteTime = pngFileInfo.LastWriteTime;
            jpgFileInfo.Attributes = pngFileInfo.Attributes;
        }

        Console.WriteLine("Przekonwertowano wszystkie pliki z .png na .jpg");
        Console.ReadKey();
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }
}