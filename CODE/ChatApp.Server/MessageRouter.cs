using System;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class MessageRouter
    {
        private readonly Server _server;
        private readonly ChatHistory _chatHistory;

        public MessageRouter(Server server, ChatHistory chatHistory)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _chatHistory = chatHistory ?? throw new ArgumentNullException(nameof(chatHistory));
        }

        /// <summary>
        /// Phân tích gói tin TCP/IP nhận được và đưa ra quyết định xử lý phù hợp
        /// </summary>
        public void Route(MessagePacket packet)
        {
            if (packet == null) return;

            switch (packet.Command)
            {
                case CommandType.BroadcastMessage:
                    HandleBroadcast(packet);
                    break;

                case CommandType.PrivateMessage:
                    HandlePrivateMessage(packet);
                    break;

                default:
                    _server.Log($"[Cảnh báo] Nhận được gói tin có lệnh không xác định: {packet.Command} từ {packet.Sender}");
                    break;
            }
        }

        /// <summary>
        /// Xử lý định tuyến tin nhắn phòng chung (Group Chat)
        /// </summary>
        private void HandleBroadcast(MessagePacket packet)
        {
            // 1. Kiểm tra nếu đây là một tin nhắn Reply (Phản hồi)
            if (!string.IsNullOrEmpty(packet.ReplyToId) && string.IsNullOrEmpty(packet.ReplyToContent))
            {
                // Truy vết tìm lại nội dung tin nhắn gốc từ kho dữ liệu lịch sử
                packet.ReplyToContent = _chatHistory.GetMessageContent(packet.ReplyToId);
            }

            // 2. Lưu tin nhắn này vào lịch sử hệ thống
            _chatHistory.SaveMessage(packet);

            // 3. Ra lệnh cho Server phát sóng (Broadcast) gói tin này đến tất cả mọi Client online
            _server.Broadcast(packet);
        }

        /// <summary>
        /// Xử lý định tuyến tin nhắn riêng mật giữa 2 cá nhân (Private Chat)
        /// </summary>
        private void HandlePrivateMessage(MessagePacket packet)
        {
            // 1. Xử lý ngữ cảnh Reply cho tin nhắn riêng
            if (!string.IsNullOrEmpty(packet.ReplyToId) && string.IsNullOrEmpty(packet.ReplyToContent))
            {
                packet.ReplyToContent = _chatHistory.GetMessageContent(packet.ReplyToId);
            }

            // 2. Lưu vết tin nhắn (phục vụ tính năng Reply chéo)
            _chatHistory.SaveMessage(packet);

            // 3. Yêu cầu Server chuyển tiếp chính xác đến đích danh người nhận và người gửi
            _server.RouteMessage(packet);
        }
    }
}