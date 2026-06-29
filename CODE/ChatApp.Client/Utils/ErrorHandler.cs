using System;
using System.Windows.Forms;

namespace ChatApp.Client.Utils
{
    public static class ErrorHandler
    {
        public static void HandleException(Exception ex, string contextMessage)
        {
            string fullMessage = $"{contextMessage}\nChi tiết: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[LỖI HỆ THỐNG]: {fullMessage}");

            // Đẩy thông báo ra màn hình dạng hộp thoại
            MessageBox.Show(fullMessage, "Cảnh báo lỗi Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void LogDebug(string info)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG LOG - {DateTime.Now:HH:mm:ss}]: {info}");
        }
    }
}
