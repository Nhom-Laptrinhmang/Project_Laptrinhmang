using System;
using System.Collections.Generic;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class ChatHistory
    {
        // Chuyển sang lưu danh sách các Object gói tin thay vì string thô
        private readonly List<MessagePacket> _historyPackets;
        private readonly object _lock = new object();

        // CHỐNG TRÀN RAM: Chỉ giữ lại tối đa 100 tin nhắn gần nhất
        private const int MaxHistorySize = 100;

        public ChatHistory()
        {
            _historyPackets = new List<MessagePacket>();
        }

        // Cập nhật hàm Save để nhận đủ tham số từ MessageRouter
        public void Save(string user, string message, string avatarBase64 = "", Protocol protocol = Protocol.Message)
        {
            // Đóng gói lại thành chuẩn MessagePacket hệt như lúc Client gửi lên
            var packet = new MessagePacket
            {
                Type = protocol,
                Sender = string.IsNullOrEmpty(user) ? "Ẩn danh" : user,
                Content = string.IsNullOrEmpty(message) ? string.Empty : message,
                AvatarBase64 = avatarBase64,
                TimeSent = DateTime.Now.ToString("HH:mm") // Lưu luôn thời gian Server
            };

            lock (_lock)
            {
                _historyPackets.Add(packet);

                // Nếu danh sách vượt quá 100, xóa cái cũ nhất (ở vị trí index 0)
                if (_historyPackets.Count > MaxHistorySize)
                {
                    _historyPackets.RemoveAt(0);
                }
            }
        }

        // Đổi tên và kiểu trả về để khớp hoàn toàn với lệnh gọi ở MessageRouter
        public List<MessagePacket> GetAllMessages()
        {
            lock (_lock)
            {
                // Trả về một bản sao độc lập (Shallow Copy) để luồng khác không can thiệp làm crash
                return new List<MessagePacket>(_historyPackets);
            }
        }
    }
}