using System;
using System.Collections.Generic;

namespace ChatApp.Server
{
    public class ChatHistory
    {
        private readonly List<string> _logs;
        private readonly object _lock = new object();

        public ChatHistory()
        {
            _logs = new List<string>();
        }

        public void Save(string user, string message)
        {
            string safeUser = user;
            string safeMessage = message;
            if (safeUser == null)
            {
                safeUser = "Ẩn danh";
            }

            if (safeMessage == null)
            {
                safeMessage = string.Empty;
            }

            lock (_lock)
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {safeUser}: {safeMessage}";
                _logs.Add(logEntry);
            }
        }

        public List<string> GetHistory()
        {
            lock (_lock)
            {
                // Trả về một bản sao danh sách để bên ngoài không can thiệp trực tiếp làm lỗi bộ nhớ
                return new List<string>(_logs);
            }
        }
    }
}
