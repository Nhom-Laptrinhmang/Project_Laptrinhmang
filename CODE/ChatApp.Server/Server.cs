using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Shared.Network;
using ChatApp.Server.Forms;

namespace ChatApp.Server
{
    public class ChatServer
    {
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<Guid, ClientHandler> _clients;
        private readonly MessageRouter _router;
        private bool _isRunning;
        private readonly ServerForm _form;

        // ==========================================================
        // GIAO VIỆC LƯU LỊCH SỬ CHO CHATHISTORY QUẢN LÝ ĐỘC LẬP
        // ==========================================================
        public ChatHistory History { get; } = new ChatHistory();

        public event Action<string>? OnLog;

        public ChatServer(int port, ServerForm form)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _clients = new ConcurrentDictionary<Guid, ClientHandler>();
            _router = new MessageRouter(this);
            _form = form;
        }

        public void Start()
        {
            _isRunning = true;
            _listener.Start();
            Log($"Server đã khởi động trên cổng {_listener.LocalEndpoint}");

            Task.Run(ListenForClientsAsync);
        }

        private async Task ListenForClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    Guid clientId = Guid.NewGuid();

                    Log($"Client mới kết nối... ID: {clientId}");

                    var handler = new ClientHandler(clientId, tcpClient, _router);

                    if (_clients.TryAdd(clientId, handler))
                    {
                        handler.OnDisconnected += RemoveClient;

                        // FIX LỖI CS8209: Gọi hàm void bình thường, không dùng "_ ="
                        handler.StartListening();

                        _form.UpdateClientList(_clients);
                    }
                    else
                    {
                        tcpClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (_isRunning) Log($"Lỗi kết nối: {ex.Message}");
                }
            }
        }

        // Phát loa công khai cho tất cả mọi người (Public)
        public void Broadcast(Protocol protocol, string jsonPayload, Guid? senderId = null)
        {
            string safePayload = jsonPayload ?? string.Empty;

            if (!string.IsNullOrEmpty(safePayload) && safePayload.StartsWith("{"))
            {
                try
                {
                    // CẤU HÌNH QUAN TRỌNG: Ép hệ thống không phân biệt chữ hoa / chữ thường khi đọc JSON
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };

                    // Giải mã chuỗi JSON về Object MessagePacket
                    var packet = JsonSerializer.Deserialize<MessagePacket>(safePayload, options);
                    if (packet != null)
                    {
                        if (string.IsNullOrEmpty(packet.Sender) || packet.Sender == "Ẩn danh")
                        {
                            if (senderId.HasValue && _clients.TryGetValue(senderId.Value, out var senderHandler))
                            {
                                packet.Sender = senderHandler.ClientName ?? "Ẩn danh";
                                packet.AvatarBase64 = senderHandler.AvatarBase64;
                            }
                        }

                        // Luôn cập nhật mốc thời gian chuẩn từ Server 
                        packet.TimeSent = DateTime.Now.ToString("HH:mm");

                        // ==========================================================
                        // LƯU TIN NHẮN VÀO CUỐN SỔ LỊCH SỬ THÔNG QUA CHATHISTORY
                        // ==========================================================
                        if (protocol == Protocol.Message)
                        {
                            History.AddMessage(packet);
                        }

                        // Mã hóa ngược lại thành chuỗi JSON sạch sẽ để gửi đi
                        safePayload = JsonSerializer.Serialize(packet, options);
                    }
                }
                catch
                {
                    // Bỏ qua nếu chuỗi truyền vào không phải cấu trúc JSON mẫu MessagePacket
                }
            }

            foreach (var client in _clients.Values)
            {
                // Chống dội ngược tại Server: Không gửi ngược lại tin nhắn cho chính người vừa bấm nút gửi
                if (senderId.HasValue && client.Id == senderId.Value)
                    continue;

                // Thêm "_ =" để tắt cảnh báo Vàng CS4014
                _ = client?.SendAsync(protocol, safePayload);
            }
        }

        // Gửi tin nhắn riêng tư đích danh cho 1 người (Private)
        public bool SendToTarget(Guid targetClientId, Protocol protocol, string jsonPayload)
        {
            if (_clients.TryGetValue(targetClientId, out var client))
            {
                _ = client?.SendAsync(protocol, jsonPayload ?? string.Empty);
                return true;
            }
            return false;
        }

        private void RemoveClient(Guid clientId)
        {
            if (_clients.TryRemove(clientId, out var handler))
            {
                Log($"Client {clientId} ({handler.ClientName}) đã ngắt kết nối.");

                // 1. Tạo gói tin Disconnect CHUẨN chứa Tên người dùng (Sender)
                var disconnectPacket = new MessagePacket
                {
                    Type = Protocol.Disconnect,
                    Sender = handler.ClientName ?? "Ai đó",
                    Content = $"Người dùng {handler.ClientName} đã rời phòng chat!"
                };

                // Ép kiểu Object thành chuỗi JSON để gửi đi
                string jsonPayload = JsonSerializer.Serialize(disconnectPacket);
                Broadcast(Protocol.Disconnect, jsonPayload);

                _form.UpdateClientList(_clients);

                // 2. Tự động đồng bộ lại danh sách Online cho TẤT CẢ những người còn lại
                BroadcastUserList();
            }
        }

        // ==========================================================
        // THÊM HÀM MỚI NÀY ĐỂ GỌI MỖI KHI CÓ NGƯỜI VÀO/RA
        // ==========================================================
        public void BroadcastUserList()
        {
            var userListPacket = new MessagePacket
            {
                Type = Protocol.UserList,
                Content = GetAllOnlineUsernames() // Lấy danh sách tên ngăn cách bởi dấu phẩy
            };

            string jsonPayload = JsonSerializer.Serialize(userListPacket);
            Broadcast(Protocol.UserList, jsonPayload);
        }

        public void Log(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        // Hàm trục xuất 1 Client
        public void KickClient(Guid clientId)
        {
            if (_clients.TryGetValue(clientId, out var client))
            {
                Log($"[Hệ Thống] Đang thực hiện trục xuất Client có ID: {clientId}");
                _ = client.SendAsync(Protocol.Disconnect, "Bạn đã bị quản trị viên trục xuất khỏi máy chủ!");
                client.Disconnect();
            }
        }

        // Hàm giải tán phòng
        public void KickAll()
        {
            Log("[Hệ Thống] Đang thực hiện giải tán phòng, trục xuất tất cả mọi người.");
            foreach (var clientId in _clients.Keys)
            {
                KickClient(clientId);
            }
        }

        // Hàm lấy chuỗi danh sách User Online
        public string GetAllOnlineUsernames()
        {
            var onlineUsers = new System.Collections.Generic.List<string>();

            foreach (var client in _clients.Values)
            {
                if (!string.IsNullOrEmpty(client.ClientName))
                {
                    onlineUsers.Add(client.ClientName);
                }
            }
            return string.Join(",", onlineUsers);
        }

        // Hàm gửi tin nhắn mật qua Tên
        public bool SendToTargetByUsername(string targetUsername, Protocol protocol, string jsonPayload)
        {
            if (string.IsNullOrEmpty(targetUsername)) return false;

            foreach (var client in _clients.Values)
            {
                if (string.Equals(client.ClientName, targetUsername, StringComparison.OrdinalIgnoreCase))
                {
                    _ = client.SendAsync(protocol, jsonPayload ?? string.Empty);
                    return true;
                }
            }

            Log($"[Cảnh báo] Không tìm thấy user '{targetUsername}' để gửi tin nhắn mật.");
            return false;
        }

        // ==============================================================================
        // BỘ LỌC KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP
        // ==============================================================================
        public bool IsUsernameTaken(string username, Guid currentClientId)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            foreach (var client in _clients.Values)
            {
                if (client.Id == currentClientId) continue;

                if (string.Equals(client.ClientName, username.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        // Hàm kích hoạt lệnh vẽ lại bảng danh sách Client bên giao diện
        public void RefreshClientList()
        {
            _form.UpdateClientList(_clients);
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();

            foreach (var client in _clients.Values)
            {
                client?.Disconnect();
            }
            _clients.Clear();

            // Yêu cầu anh chàng ChatHistory tự xóa sổ khi tắt Server
            History.Clear();

            Log("Server đã dừng.");
        }
    }
}