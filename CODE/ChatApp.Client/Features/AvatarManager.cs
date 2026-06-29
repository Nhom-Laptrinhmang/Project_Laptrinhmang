using System;
using System.IO;
using System.Drawing;

namespace ChatApp.Client.Features
{
    public static class AvatarManager
    {
        // Mã hóa ảnh từ file thành chuỗi chuỗi mã hóa Base64 để gửi qua Socket real-time
        public static string ImageToBase64(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath)) return string.Empty;
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        // Giải mã chuỗi Base64 nhận từ mạng về thành đối tượng Image hiển thị lên giao diện
        public static Image Base64ToImage(string base64String)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String)) return null;

                // ĐÃ SỬA: Sử dụng hàm chuẩn duy nhất của C# để chuyển chuỗi Base64 thành mảng byte
                byte[] bytes = Convert.FromBase64String(base64String);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch   
            {
                return null;
            }
        }
    }
}
