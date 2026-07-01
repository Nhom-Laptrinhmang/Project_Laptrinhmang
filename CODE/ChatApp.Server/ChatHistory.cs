using System.Collections.Generic;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class ChatHistory
    {
        // Cuốn sổ lưu trữ thực sự
        private readonly List<MessagePacket> _messages = new();
        private readonly object _lock = new object();
        private readonly int _maxLimit;

        // Khởi tạo giới hạn lưu trữ mặc định là 50 tin nhắn
        public ChatHistory(int maxLimit = 50)
        {
            _maxLimit = maxLimit;
        }

        // Hàm ghi thêm tin nhắn mới vào sổ
        public void AddMessage(MessagePacket packet)
        {
            lock (_lock)
            {
                _messages.Add(packet);
                // Nếu vượt quá giới hạn thì xóa tin nhắn cũ nhất (tin ở đầu mảng)
                if (_messages.Count > _maxLimit)
                {
                    _messages.RemoveAt(0);
                }
            }
        }

        // Hàm trích xuất bản sao của cuốn sổ để gửi cho Client
        public List<MessagePacket> GetHistory()
        {
            lock (_lock)
            {
                return new List<MessagePacket>(_messages);
            }
        }

        // Hàm xóa sổ khi Server tắt
        public void Clear()
        {
            lock (_lock)
            {
                _messages.Clear();
            }
        }
    }
}