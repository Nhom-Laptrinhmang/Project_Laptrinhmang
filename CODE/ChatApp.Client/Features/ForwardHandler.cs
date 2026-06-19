using System;
using System.Windows.Forms;

namespace ChatApp.Client.Features
{
    public static class ForwardHandler
    {
        private static string _messageToForward = string.Empty;
        private static string _originalSender = string.Empty;

        public static void PrepareForward(string originalSender, string content)
        {
            _originalSender = originalSender;
            _messageToForward = content;

            MessageBox.Show($"Đã chọn tin nhắn của @{originalSender}. Hãy chọn một người dùng trong danh sách để chuyển tiếp!", 
                            "Forward Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static string GetForwardedBody()
        {
            if (string.IsNullOrEmpty(_messageToForward)) return string.Empty;
            return $"[Chuyển tiếp từ @{_originalSender}]: {_messageToForward}";
        }

        public static void ClearForwardContext()
        {
            _messageToForward = string.Empty;
            _originalSender = string.Empty;
        }
    }
}
