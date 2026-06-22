using System;
using System.Text.Json;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class MessageRouter
    {
        private readonly ChatServer _server;
        private readonly ChatHistory _history;

        public MessageRouter(ChatServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _history = new ChatHistory();
        }

        public void Route(Guid senderId, Protocol protocol, string rawData, ClientHandler handler)
        {
            string safeData = rawData ?? string.Empty;

            try
            {
                switch (protocol)
                {
                    case Protocol.Connect:
                        handler.ClientName = safeData;
                        _server.Log($"User [{handler.ClientName}] (ID: {senderId}) đã đăng nhập.");
                        _server.Broadcast(Protocol.Connect, $"Hệ thống: {handler.ClientName} đã tham gia phòng.");
                        break;

                    case Protocol.UpdateAvatar:
                        handler.AvatarBase64 = safeData;
                        _server.Log($"User [{handler.ClientName}] vừa cập nhật hình ảnh đại diện.");
                        break;

                    // XỬ LÝ CHAT PRIVATE (RIÊNG TƯ)
                    case Protocol.PrivateMessage:
                        var privatePacket = JsonSerializer.Deserialize<ChatPacket>(safeData);
                        if (privatePacket != null)
                        {
                            privatePacket.SenderId = senderId.ToString();
                            privatePacket.SenderName = handler.ClientName ?? "Ẩn danh";
                            privatePacket.AvatarBase64 = handler.AvatarBase64;

                            string processedJson = JsonSerializer.Serialize(privatePacket);

                            // Kiểm tra xem Client có truyền lên ID người nhận hợp lệ không
                            if (!string.IsNullOrEmpty(privatePacket.ReceiverId) && Guid.TryParse(privatePacket.ReceiverId, out Guid targetGuid))
                            {
                                _server.Log($"[{privatePacket.SenderName}] CHAT RIÊNG tới [ID: {privatePacket.ReceiverId?.Substring(0, 5)}]: {privatePacket.Content}");

                                // 1. Gửi cho người nhận đích danh
                                bool isSent = _server.SendToTarget(targetGuid, Protocol.PrivateMessage, processedJson);

                                if (isSent)
                                {
                                    // 2. Gửi ngược lại cho chính người gửi để giao diện hiển thị lịch sử đối thoại của họ
                                    _server.SendToTarget(senderId, Protocol.PrivateMessage, processedJson);
                                }
                                else
                                {
                                    // Báo lỗi cho người gửi nếu người nhận đã rời phòng chat
                                    _server.Log($"Lỗi: Không tìm thấy người nhận {privatePacket.ReceiverId}. Có thể họ đã offline.");
                                }
                            }
                        }
                        break;

                    // XỬ LÝ CHAT PUBLIC, REPLY, FORWARD (CÔNG KHAI)
                    case Protocol.Message:
                    case Protocol.Reply:
                    case Protocol.Forward:
                        var chatPacket = JsonSerializer.Deserialize<ChatPacket>(safeData);
                        if (chatPacket != null)
                        {
                            chatPacket.SenderId = senderId.ToString();
                            chatPacket.SenderName = handler.ClientName ?? "Ẩn danh";
                            chatPacket.AvatarBase64 = handler.AvatarBase64;

                            if (protocol == Protocol.Reply)
                                _server.Log($"[{chatPacket.SenderName}] REPLY tin nhắn [{chatPacket.ReplyToMessageId?.Substring(0, 5)}]: {chatPacket.Content}");
                            else if (protocol == Protocol.Forward)
                                _server.Log($"[{chatPacket.SenderName}] FORWARD tin nhắn: {chatPacket.Content}");
                            else
                                _server.Log($"[{chatPacket.SenderName}] CHAT PUBLIC: {chatPacket.Content}");

                            string processedJson = JsonSerializer.Serialize(chatPacket);
                            _history.Save(chatPacket.SenderName, chatPacket.Content);

                            // Phát loa cho toàn bộ mọi người trong phòng cùng nhận
                            _server.Broadcast(protocol, processedJson);
                        }
                        break;

                    case Protocol.Disconnect:
                        _server.Log($"User {senderId} gửi yêu cầu rời ứng dụng.");
                        break;

                    default:
                        _server.Log($"Nhận được mã Protocol không hợp lệ từ {senderId}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _server.Log($"Lỗi xử lý định tuyến dữ liệu: {ex.Message}");
            }
        }
    }
}
