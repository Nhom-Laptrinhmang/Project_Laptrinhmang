using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class ClientHandler
    {
        public Guid Id { get; }

        public string ClientName { get; set; } = string.Empty;

        public string AvatarBase64 { get; set; } = string.Empty;

        private readonly TcpClient _tcpClient;

        private readonly NetworkStream? _stream;

        private readonly MessageRouter _router;

        private bool _isConnected;

        // BỔ SUNG: Khóa Semaphore để ép các luồng gửi tin nhắn mật/công khai phải xếp hàng, chống dính dòng byte mạng
        private readonly SemaphoreSlim _sendSemaphore = new SemaphoreSlim(1, 1);

        public event Action<Guid>? OnDisconnected;

        public ClientHandler(Guid id, TcpClient tcpClient, MessageRouter router)
        {
            Id = id;

            if (tcpClient == null)
            {
                throw new ArgumentNullException(nameof(tcpClient));
            }

            if (router == null)
            {
                throw new ArgumentNullException(nameof(router));
            }

            _tcpClient = tcpClient;
            _router = router;

            try
            {
                _stream = _tcpClient.GetStream();
                _isConnected = true;
            }
            catch
            {
                _isConnected = false;
            }
        }

            public void StartListening()
        {
            if (_stream != null && _isConnected)
            {
                Task.Run(ReceiveLoopAsync);
            }
        }

        private async Task ReceiveLoopAsync()
        {
            if (_stream == null)
            {
                return;
            }

            // Bọc BinaryReader vào khối using để tự động giải phóng bộ nhớ RAM khi ngắt kết nối mạng
            using (BinaryReader reader = new BinaryReader(_stream, Encoding.UTF8, true))
            {
                try
                {
                    while (_isConnected && _tcpClient.Connected)
                    {
                        // 1. Đọc kích thước gói tin (4 byte đầu tiên)
                        int dataLength = reader.ReadInt32();

                        // 2. Đọc mã định danh giao thức (4 byte tiếp theo)
                        int protocolInt = reader.ReadInt32();
                        Protocol protocol = (Protocol)protocolInt;

                        // 3. Đọc chính xác số byte nội dung văn bản còn lại của gói tin
                        byte[] messageBytes = reader.ReadBytes(dataLength - 4);
                        string message = Encoding.UTF8.GetString(messageBytes);

                        // ==========================================================
                        // FIX LỖI CS1501: Giải mã chuỗi JSON thành MessagePacket
                        // ==========================================================
                        MessagePacket packet = null;
                        try
                        {
                            // Ép kiểu chuỗi JSON nhận được thành dạng Object để Router dễ xử lý
                            packet = System.Text.Json.JsonSerializer.Deserialize<MessagePacket>(message, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch
                        {
                            // Đề phòng trường hợp Client gửi chuỗi rỗng hoặc không phải JSON hợp lệ
                            packet = new MessagePacket();
                        }

                        if (packet != null)
                        {
                            // Đồng bộ loại Protocol từ byte mạng vào thuộc tính Type của Packet
                            packet.Type = protocol;

                            // 4. Định tuyến chuyển tiếp xử lý dữ liệu tới MessageRouter (GỌI ĐÚNG 2 THAM SỐ)
                            _router.Route(this, packet);
                        }
                    }
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public async Task SendAsync(Protocol protocol, string message)
        {
            string safeMessage = message ?? string.Empty;

            if (_stream == null || !_isConnected || !_tcpClient.Connected)
            {
                return;
            }

            // Kích hoạt khóa bảo mật luồng: Bắt các luồng khác phải chờ luồng hiện tại ghi xong byte
            await _sendSemaphore.WaitAsync();

            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(safeMessage);

                // Tổng độ dài dữ liệu thực tế = 4 byte Protocol + độ dài mảng byte tin nhắn văn bản
                int dataLength = 4 + messageBytes.Length;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.UTF8))
                    {
                        writer.Write(dataLength);
                        writer.Write((int)protocol);
                        writer.Write(messageBytes);
                        writer.Flush();

                        byte[] packet = memoryStream.ToArray();

                        // Ghi dữ liệu đồng bộ xuống đường truyền mạng socket an toàn
                        await _stream.WriteAsync(packet, 0, packet.Length);
                        await _stream.FlushAsync();
                    }
                }
            }
            catch
            {
                Disconnect();
            }
            finally
            {
                // Giải phóng khóa để nhường quyền cho tin nhắn của người tiếp theo được gửi đi
                _sendSemaphore.Release();
            }
        }

        public void Disconnect()
        {
            if (!_isConnected)
            {
                return;
            }

            _isConnected = false;

            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                }
                _tcpClient.Close();
            }
            catch
            {
                // Bỏ qua lỗi phát sinh trong quá trình đóng socket thô
            }

            // Giải phóng luôn tài nguyên của khóa Semaphore để tránh rò rỉ RAM hệ thống
            _sendSemaphore.Dispose();

            if (OnDisconnected != null)
            {
                OnDisconnected(Id);
            }
        }
    }
}