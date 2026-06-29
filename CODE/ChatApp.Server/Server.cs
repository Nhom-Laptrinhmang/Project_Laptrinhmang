using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        // Phát loa công khai cho tất cả mọi người (Public) - ĐÃ ĐỒNG BỘ THỜI GIAN VÀ KHÔI PHỤC REPLY
        // Phát loa công khai cho tất cả mọi người (Public)
        // Phát loa công khai cho tất cả mọi người (Public)
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
                        PropertyNamingPolicy = null // Giữ nguyên định dạng gốc của Client gửi lên
                    };

                    // Giải mã an toàn chuỗi JSON về Object MessagePacket để chỉnh sửa trực tiếp thuộc tính
                    var packet = JsonSerializer.Deserialize<MessagePacket>(safePayload, options);
                    if (packet != null)
                    {
                        // Kiểm tra xem trường người gửi có bị trống hay không (Chấp nhận cả trường hợp gán chữ "Ẩn danh")
                        if (string.IsNullOrEmpty(packet.Sender) || packet.Sender == "Ẩn danh")
                        {
                            // Nếu trống, tra cứu ID mạng từ Server để lấy tên thật đã lưu trong ClientHandler
                            if (senderId.HasValue && _clients.TryGetValue(senderId.Value, out var senderHandler))
                            {
                                packet.Sender = senderHandler.ClientName ?? "Ẩn danh";
                                packet.AvatarBase64 = senderHandler.AvatarBase64;
                            }
                        }

                        // Luôn cập nhật mốc thời gian chuẩn từ Server (Định dạng Giờ:Phút)
                        packet.TimeSent = DateTime.Now.ToString("HH:mm");

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

                client?.SendAsync(protocol, safePayload);
            }
        }



        // Gửi tin nhắn riêng tư đích danh cho 1 người (Private)
        public bool SendToTarget(Guid targetClientId, Protocol protocol, string jsonPayload)
        {
            if (_clients.TryGetValue(targetClientId, out var client))
            {
                client?.SendAsync(protocol, jsonPayload ?? string.Empty);
                return true;
            }
            return false; // Trả về false nếu không tìm thấy người nhận (họ đã offline)
        }

        private void RemoveClient(Guid clientId)
        {
            if (_clients.TryRemove(clientId, out var handler))
            {
                Log($"Client {clientId} đã ngắt kết nối.");
                Broadcast(Protocol.Disconnect, clientId.ToString());

                _form.UpdateClientList(_clients);
            }
        }

        public void Log(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        // Hàm trục xuất 1 Client đích danh khỏi phòng chat
        public void KickClient(Guid clientId)
        {
            // Tìm kiếm xem Client này có đang online trong cuốn sổ danh sách không
            if (_clients.TryGetValue(clientId, out var client))
            {
                Log($"[Hệ Thống] Đang thực hiện trục xuất Client có ID: {clientId}");

                // 1. Gửi tín hiệu thông báo ép buộc ngắt kết nối về phía app của khách
                client.SendAsync(Protocol.Disconnect, "Bạn đã bị quản trị viên trục xuất khỏi máy chủ!");

                // 2. Chủ động đóng đường truyền mạng của Client đó (Hàm này tự động gạch tên họ khỏi ListView luôn)
                client.Disconnect();
            }
        }

        // Hàm trục xuất SẠCH SẼ toàn bộ Client đang kết nối
        public void KickAll()
        {
            Log("[Hệ Thống] Đang thực hiện giải tán phòng, trục xuất tất cả mọi người.");

            // Quét qua toàn bộ danh sách ID mạng đang hoạt động để đuổi từng người một
            foreach (var clientId in _clients.Keys)
            {
                KickClient(clientId);
            }
        }
        
        // CÁC HÀM BỔ TRỢ MỚI CHO GIAO TIẾP VÀ ĐỒNG BỘ TRẠNG THÁI
       
        // 1. Hàm lấy danh sách toàn bộ User đang Online để cập nhật lên giao diện Client
        public string GetAllOnlineUsernames()
        {
            // Lọc ra những Client đã có tên (đã qua bước Connect thành công)
            var onlineUsers = new System.Collections.Generic.List<string>();

            foreach (var client in _clients.Values)
            {
                if (!string.IsNullOrEmpty(client.ClientName))
                {
                    onlineUsers.Add(client.ClientName);
                }
            }

            // Nối danh sách bằng dấu phẩy để Client dễ dàng Split(",")
            return string.Join(",", onlineUsers);
        }

        // 2. Hàm gửi tin nhắn mật đích danh dựa theo Tên người dùng (Username) thay vì Guid
        public bool SendToTargetByUsername(string targetUsername, Protocol protocol, string jsonPayload)
        {
            if (string.IsNullOrEmpty(targetUsername)) return false;

            // Duyệt qua sổ xưng danh để tìm Client có ClientName khớp với yêu cầu
            foreach (var client in _clients.Values)
            {
                if (string.Equals(client.ClientName, targetUsername, StringComparison.OrdinalIgnoreCase))
                {
                    client.SendAsync(protocol, jsonPayload ?? string.Empty);
                    return true; // Gửi thành công
                }
            }

            Log($"[Cảnh báo] Không tìm thấy user '{targetUsername}' để gửi tin nhắn mật.");
            return false; // Người nhận không tồn tại hoặc đã offline
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
            Log("Server đã dừng.");
        }
    }
}
