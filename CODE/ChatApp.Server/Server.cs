using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class Server
    {
        // --- THÀNH PHẦN MẠNG LÕI TCP/IP ---
        private TcpListener _listener;
        private List<ClientHandler> _clients = new List<ClientHandler>();
        private bool _isRunning;
        private Action<string> _logCallback;

        // --- CÁC THÀNH PHẦN QUẢN LÝ TÍNH NĂNG NÂNG CAO ---
        private ChatHistory _chatHistory = new ChatHistory();
        private MessageRouter _messageRouter;

        // Thuộc tính mở (Property) để các luồng ClientHandler dễ dàng gọi bộ định tuyến MessageRouter
        public MessageRouter MessageRouterInstance => _messageRouter;

        // --- HÀM KHỞI TẠO SERVER ---
        public Server(int port, Action<string> logCallback)
        {
            // Lắng nghe ở bất kỳ địa chỉ IP nào của máy tính thông qua Cổng (Port) chỉ định
            _listener = new TcpListener(IPAddress.Any, port);
            _logCallback = logCallback;

            // Kết nối các khối logic điều hướng và lưu trữ vào Server
            _messageRouter = new MessageRouter(this, _chatHistory);
        }

        // --- KHỞI CHẠY SERVER ---
        public void Start()
        {
            _isRunning = true;
            _listener.Start();

            // Đọc cổng thực tế mà máy đang cấp phép chạy
            int activePort = ((IPEndPoint)_listener.LocalEndpoint).Port;
            Log($"Server đã khởi động thành công trên cổng {activePort} và đang lắng nghe...");

            // Tạo luồng ngầm chuyên đứng ở cổng mạng để canh và đón Client kết nối vào
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        // --- LUỒNG CHỜ ĐÓN CÁC CLIENT VÀO PHÒNG CHỜ ---
        private void AcceptClients()
        {
            while (_isRunning)
            {
                try
                {
                    // Lệnh này dừng luồng lại để đợi kết nối vật lý, khi có Client chạm vào -> Sinh ra đối tượng TcpClient
                    TcpClient client = _listener.AcceptTcpClient();

                    // Tạo một người quản lý riêng biệt cho kết nối mới này
                    ClientHandler handler = new ClientHandler(client, this);
                }
                catch
                {
                    // Thoát luồng nếu Listener bị dừng đột ngột
                    break;
                }
            }
        }

        // --- QUẢN LÝ THÊM/XÓA CLIENT TRONG DANH SÁCH (DÙNG CƠ CHẾ LOCK AN TOÀN LUỒNG) ---
        public void AddClient(ClientHandler client)
        {
            lock (_clients)
            {
                if (!_clients.Contains(client))
                {
                    _clients.Add(client);
                }
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (_clients)
            {
                _clients.Remove(client);
            }
            if (!string.IsNullOrEmpty(client.Username))
            {
                Log($"Người dùng '{client.Username}' đã ngắt kết nối.");

                // Cập nhật lại danh sách online cho những người còn lại biết
                BroadcastUserList();
                Broadcast(new MessagePacket(CommandType.BroadcastMessage, "Hệ thống", $"{client.Username} đã rời phòng chat."));
            }
        }

        // --- KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP ---
        public bool IsUsernameTaken(string username)
        {
            lock (_clients)
            {
                return _clients.Any(c => c.Username != null && c.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
        }

        // --- GỬI TIN NHẮN PHÒNG CHUNG (BROADCAST) ---
        public void Broadcast(MessagePacket packet)
        {
            List<ClientHandler> targets;
            lock (_clients)
            {
                // Sao chép danh sách ra mảng tạm để tránh lỗi xung đột bộ nhớ khi duyệt vòng lặp foreach đa luồng
                targets = _clients.Where(c => !string.IsNullOrEmpty(c.Username)).ToList();
            }

            foreach (var client in targets)
            {
                client.SendPacket(packet);
            }
        }

        // --- ĐIỀU HƯỚNG TIN CHAT RIÊNG BIỆT (PRIVATE MESSAGE) ---
        public void RouteMessage(MessagePacket packet)
        {
            Log($"[Tin Nhắn] Từ: {packet.Sender} -> Đến: {packet.Recipient}. Nội dung: {packet.Content}");

            if (packet.Recipient == "All")
            {
                Broadcast(packet);
            }
            else
            {
                List<ClientHandler> localClients;
                lock (_clients)
                {
                    localClients = _clients.ToList();
                }

                // 1. Gửi gói tin sang cho người nhận đích thực
                var target = localClients.FirstOrDefault(c => c.Username == packet.Recipient);
                target?.SendPacket(packet);

                // 2. Gửi ngược lại cho chính người vừa gõ để hiển thị lên khung chat đôi của họ
                var sender = localClients.FirstOrDefault(c => c.Username == packet.Sender);
                if (sender != target)
                {
                    sender?.SendPacket(packet);
                }
            }
        }

        // --- PHÁT SÓNG CẬP NHẬT DANH SÁCH USER ONLINE ---
        public void BroadcastUserList()
        {
            lock (_clients)
            {
                // Gom tất cả các tên người dùng đang online hợp lệ vào danh sách string
                var activeUsers = _clients.Where(c => !string.IsNullOrEmpty(c.Username)).Select(c => c.Username).ToList();

                // Đóng gói danh sách mảng chuỗi thành cấu trúc văn bản JSON gọn gàng
                string jsonUserList = JsonSerializer.Serialize(activeUsers);
                var packet = new MessagePacket(CommandType.UserListUpdate, "Server", jsonUserList);

                // Đẩy hạt tin danh sách xuống cho từng máy một
                foreach (var client in _clients.Where(c => !string.IsNullOrEmpty(c.Username)))
                {
                    client.SendPacket(packet);
                }
            }
        }

        // --- IN THÔNG TIN HOẠT ĐỘNG RA GIAO DIỆN CHÍNH SERVER ---
        public void Log(string message)
        {
            // Gọi hàm Callback an toàn để đẩy text lên TextBox giao diện WinForms ở Main Thread
            _logCallback?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        // --- DỪNG HOẠT ĐỘNG MÁY CHỦ AN TOÀN ---
        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();

            // Xóa sạch lịch sử tạm trong RAM
            _chatHistory.ClearAll();

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    client.Disconnect();
                }
                _clients.Clear();
            }
            Log("Server đã dừng hoạt động.");
        }
    }
}