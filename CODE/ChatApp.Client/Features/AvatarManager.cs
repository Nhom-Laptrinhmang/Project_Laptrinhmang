using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ChatApp.Client.Features
{
    public static class AvatarManager
    {
        public static async Task<string> ConvertImageToBase64Async(Image image)
        {
            if (image == null) return string.Empty;

            return await Task.Run(() =>
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Bitmap bmp = new Bitmap(image))
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageBytes = ms.ToArray();
                            return Convert.ToBase64String(imageBytes);
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }
            });
        }

        public static async Task<Image> ConvertBase64ToImageAsync(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;

            return await Task.Run(() =>
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(base64String);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        return new Bitmap(Image.FromStream(ms));
                    }
                }
                catch
                {
                    return null;
                }
            });
        }
    }
}
