using System;
using System.Text.Json;
using System.Collections.Generic;
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
                        // 1. CHỮA LỖI TÊN: Bóc tách JSON thay vì gán nguyên chuỗi thô
                        var connectData = JsonSerializer.Deserialize<MessagePacket>(safeData);
                        if (connectData != null)
                        {
                            handler.ClientName = connectData.Sender;
                            handler.AvatarBase64 = connectData.AvatarBase64;
                            _server.Log($"User [{handler.ClientName}] (ID: {senderId}) đã đăng nhập.");

                            // Phát loa thông báo tham gia
                            var connectPacket = new MessagePacket
                            {
                                Type = Protocol.Connect,
                                Sender = "Hệ thống",
                                Content = $"{handler.ClientName} đã tham gia phòng chat."
                            };
                            _server.Broadcast(Protocol.Connect, JsonSerializer.Serialize(connectPacket));

                            // 2. CHỮA LỖI ONLINE LIST: Yêu cầu Server lấy danh sách tên và Broadcast
                            BroadcastCurrentUserList();

                            // 3. CHỮA LỖI LỊCH SỬ CHAT: Gửi toàn bộ lịch sử cho riêng ông này
                            SendHistoryToNewClient(senderId);
                        }
                        break;

                    case Protocol.UpdateAvatar:
                        handler.AvatarBase64 = safeData;
                        _server.Log($"User [{handler.ClientName}] vừa cập nhật hình ảnh đại diện.");
                        break;

                    case Protocol.PrivateMessage:
                        var privatePacket = JsonSerializer.Deserialize<MessagePacket>(safeData);
                        if (privatePacket != null && !string.IsNullOrEmpty(privatePacket.Receiver))
                        {
                            privatePacket.Sender = handler.ClientName ?? "Ẩn danh";
                            privatePacket.AvatarBase64 = handler.AvatarBase64;

                            string processedJson = JsonSerializer.Serialize(privatePacket);
                            _server.Log($"[{privatePacket.Sender}] CHAT MẬT tới [{privatePacket.Receiver}]: {privatePacket.Content}");

                            // Gửi cho người nhận (Cần đảm bảo ChatServer của bạn có hàm này)
                            _server.SendToTargetByUsername(privatePacket.Receiver, Protocol.PrivateMessage, processedJson);

                            // Gửi lại cho chính mình để UI bên mình cũng hiện tin nhắn đó
                            _server.SendToTarget(senderId, Protocol.PrivateMessage, processedJson);
                        }
                        break;

                    case Protocol.Message:
                    case Protocol.Reply:
                    case Protocol.Forward:
                        var chatPacket = JsonSerializer.Deserialize<MessagePacket>(safeData);
                        if (chatPacket != null)
                        {
                            chatPacket.Sender = handler.ClientName ?? "Ẩn danh";
                            chatPacket.AvatarBase64 = handler.AvatarBase64;

                            // Đóng gói lại thành JSON chuẩn
                            string processedJson = JsonSerializer.Serialize(chatPacket);

                            // Lưu vào bộ nhớ đệm lịch sử
                            _history.Save(chatPacket.Sender, chatPacket.Content, chatPacket.AvatarBase64, protocol);

                            // Broadcast cho tất cả TRỪ người gửi (chống dội ngược)
                            _server.Broadcast(protocol, processedJson, senderId);
                        }
                        break;

                    case Protocol.Disconnect:
                        _server.Log($"User [{handler.ClientName}] gửi yêu cầu rời ứng dụng.");

                        var dcPacket = new MessagePacket
                        {
                            Type = Protocol.Disconnect,
                            Sender = "Hệ thống",
                            Content = $"{handler.ClientName} đã thoát khỏi phòng."
                        };
                        _server.Broadcast(Protocol.Disconnect, JsonSerializer.Serialize(dcPacket));

                        // Cập nhật lại danh sách Online vì vừa có người thoát
                        BroadcastCurrentUserList();
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

        // --- CÁC HÀM BỔ TRỢ ĐỂ CHẠY LOGIC ĐỒNG BỘ ---

        private void BroadcastCurrentUserList()
        {
            // Yêu cầu class ChatServer trả về danh sách các Username đang online (phân tách bằng dấu phẩy)
            string usersStr = _server.GetAllOnlineUsernames();

            var listPacket = new MessagePacket
            {
                Type = Protocol.UserList,
                Content = usersStr
            };

            _server.Broadcast(Protocol.UserList, JsonSerializer.Serialize(listPacket));
        }

        private void SendHistoryToNewClient(Guid newClientId)
        {
            // Lấy danh sách tin nhắn cũ từ ChatHistory
            List<MessagePacket> pastMessages = _history.GetAllMessages();

            foreach (var packet in pastMessages)
            {
                string json = JsonSerializer.Serialize(packet);
                _server.SendToTarget(newClientId, packet.Type, json);
            }
        }
    }
}