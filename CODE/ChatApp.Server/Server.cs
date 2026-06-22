using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
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

        // Phát loa công khai cho tất cả mọi người (Public)
        public void Broadcast(Protocol protocol, string jsonPayload, Guid? senderId = null)
        {
            string safePayload = jsonPayload ?? string.Empty;

            foreach (var client in _clients.Values)
            {
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
        // ==================== THÊM ĐOẠN NÀY VÀO FILE SERVER.CS ====================

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
