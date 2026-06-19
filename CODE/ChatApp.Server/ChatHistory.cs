using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class ChatHistory
    {
        // ConcurrentDictionary giúp nhiều luồng kết nối Client ghi dữ liệu đồng thời mà không gây crash bộ nhớ
        private readonly ConcurrentDictionary<string, MessagePacket> _messages = new ConcurrentDictionary<string, MessagePacket>();

        // LƯU TIN NHẮN MỚI VÀO KHO
        public void SaveMessage(MessagePacket packet)
        {
            if (packet != null && !string.IsNullOrEmpty(packet.MessageId))
            {
                // Thêm vào từ điển lưu trữ với Khóa chính là ID duy nhất của tin nhắn
                _messages.TryAdd(packet.MessageId, packet);
            }
        }

        // TÌM LẠI CHỮ CỦA TIN NHẮN GỐC DỰA VÀO ID (PHỤC VỤ REPLY)
        public string GetMessageContent(string messageId)
        {
            if (string.IsNullOrEmpty(messageId)) return string.Empty;

            // Kiểm tra xem ID có nằm trong kho không, nếu có lấy ra đối tượng packet tương ứng
            if (_messages.TryGetValue(messageId, out var packet))
            {
                return packet.Content;
            }
            return "Tin nhắn cũ không tồn tại hoặc đã bị xóa.";
        }

        // TRÍCH XUẤT TOÀN BỘ LỊCH SỬ PHÒNG CHUNG (TÙY CHỌN DÙNG KHI NÂNG CẤP LOAD TIN CŨ)
        public List<MessagePacket> GetBroadcastHistory()
        {
            return _messages.Values
                .Where(m => m.Recipient == "All" && m.Command == CommandType.BroadcastMessage)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        // DỌN SẠCH KHO LƯU TRỮ KHI SẬP HOẶC TẮT SERVER
        public void ClearAll()
        {
            _messages.Clear();
        }
    }
}