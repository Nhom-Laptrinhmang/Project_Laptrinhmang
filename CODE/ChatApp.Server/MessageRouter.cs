using System;
using System.Text.Json;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class MessageRouter
    {
        private readonly ChatServer _server;

        public MessageRouter(ChatServer server)
        {
            _server = server;
        }

        // Hàm này được ClientHandler gọi khi nhận được tin nhắn từ Client
        public void Route(ClientHandler handler, MessagePacket packet)
        {
            // Đóng gói lại thành chuỗi JSON để chuẩn bị gửi đi
            string jsonMessage = System.Text.Json.JsonSerializer.Serialize(packet);

            switch (packet.Type)
            {
                case Protocol.Connect:
                    HandleClientConnect(handler, packet);
                    break;

                case Protocol.Message:
                    // 1. TIN NHẮN CHUNG (Gửi cho tất cả)
                    _server.Log($"[Tin Chung] {handler.ClientName}: {packet.Content}");
                    _server.Broadcast(Protocol.Message, jsonMessage, handler.Id);
                    break;

                case Protocol.PrivateMessage:
                    // 2. TIN NHẮN MẬT (Gửi cho 1 người)
                    _server.Log($"[Tin Mật] {handler.ClientName} -> {packet.Receiver}: {packet.Content}");
                    bool isSentPrivately = _server.SendToTargetByUsername(packet.Receiver, Protocol.PrivateMessage, jsonMessage);

                    // Nếu người nhận đã out phòng, báo lỗi ngược lại cho người gửi
                    if (!isSentPrivately)
                    {
                        var err = new MessagePacket { Type = Protocol.Message, Sender = "Hệ thống", Content = $"Người dùng {packet.Receiver} hiện không online." };
                        _ = handler.SendAsync(Protocol.Message, System.Text.Json.JsonSerializer.Serialize(err));
                    }
                    break;

                case Protocol.Forward:
                    // 3. TIN NHẮN CHUYỂN TIẾP (Phân loại đích đến)
                    _server.Log($"[Chuyển Tiếp] {handler.ClientName} -> {packet.Receiver}: {packet.Content}");

                    if (packet.Receiver == "All" || string.IsNullOrEmpty(packet.Receiver))
                    {
                        // Nếu đích là "All" -> Phát loa cho cả phòng
                        _server.Broadcast(Protocol.Forward, jsonMessage, handler.Id);
                    }
                    else
                    {
                        // Nếu đích là 1 người cụ thể -> Gửi thẳng cho người đó
                        bool isSentFw = _server.SendToTargetByUsername(packet.Receiver, Protocol.Forward, jsonMessage);
                        if (!isSentFw)
                        {
                            var err = new MessagePacket { Type = Protocol.Message, Sender = "Hệ thống", Content = $"Người dùng {packet.Receiver} hiện không online." };
                            _ = handler.SendAsync(Protocol.Message, System.Text.Json.JsonSerializer.Serialize(err));
                        }
                    }
                    break;
            }
        }

        private void HandleClientConnect(ClientHandler handler, MessagePacket packet)
        {
            string requestedUsername = packet.Sender?.Trim() ?? "Ẩn danh";

            // 1. Nếu bị trùng tên
            if (_server.IsUsernameTaken(requestedUsername, handler.Id))
            {
                // Sử dụng Type = Protocol.Disconnect và Sender = "Hệ thống" để Client nhận diện được
                var rejectPacket = new MessagePacket
                {
                    Type = Protocol.Disconnect,
                    Sender = "Hệ thống",
                    Content = $"Tên đăng nhập '{requestedUsername}' đã tồn tại trong phòng. Vui lòng chọn tên khác!"
                };

                string jsonReject = JsonSerializer.Serialize(rejectPacket);
                _ = handler.SendAsync(Protocol.Disconnect, jsonReject);
                handler.Disconnect();
                return;
            }

            // 2. Nếu đăng nhập thành công
            handler.ClientName = requestedUsername;
            handler.AvatarBase64 = packet.AvatarBase64;

            // Kích hoạt cập nhật danh sách UI bên Server
            _server.RefreshClientList();

            // ================================================================
            // BỔ SUNG QUAN TRỌNG: GỬI GÓI "USERLIST" ĐỂ ĐỒNG BỘ DANH SÁCH ONLINE MỚI NHẤT
            // ================================================================
            _server.BroadcastUserList();

            var successPacket = new MessagePacket
            {
                Type = Protocol.Connect,
                Sender = requestedUsername, // Gửi đúng tên người vào để Client dễ dàng bắt được
                Content = $"Người dùng {requestedUsername} đã tham gia phòng chat!"
            };

            // Phát loa thông báo cho cả phòng biết
            _server.Broadcast(Protocol.Connect, JsonSerializer.Serialize(successPacket), null);

            // ================================================================
            // ĐỌC LỊCH SỬ VÀ GỬI CHO NGƯỜI MỚI VÀO
            // ================================================================
            var historyMessages = _server.History.GetHistory();
            foreach (var oldPacket in historyMessages)
            {
                string jsonOldMsg = JsonSerializer.Serialize(oldPacket);
                // Gửi lại cho riêng người này dưới dạng tin nhắn bình thường
                _ = handler.SendAsync(Protocol.Message, jsonOldMsg);
            }
        }
    }
}