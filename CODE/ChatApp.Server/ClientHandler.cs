using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
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

            BinaryReader reader = new BinaryReader(_stream);

            try
            {
                while (_isConnected && _tcpClient.Connected)
                {
                    int dataLength = reader.ReadInt32();

                    int protocolInt = reader.ReadInt32();

                    Protocol protocol = (Protocol)protocolInt;

                    byte[] messageBytes =
                        reader.ReadBytes(dataLength - 4);

                    string message =
                        Encoding.UTF8.GetString(messageBytes);

                    _router.Route(
                        Id,
                        protocol,
                        message,
                        this);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public async Task SendAsync(Protocol protocol, string message)
        {
            string safeMessage = message ?? string.Empty;

            if (_stream == null || !_isConnected || !_tcpClient.Connected)
            {
                return;
            }

            try
            {
                // 1. Chuyển chuỗi tin nhắn thành mảng byte UTF-8
                byte[] messageBytes = Encoding.UTF8.GetBytes(safeMessage);

                // 2. Tính toán tổng độ dài dữ liệu thực tế gửi đi (4 byte Protocol + số byte của chuỗi)
                int dataLength = 4 + messageBytes.Length;

                // 3. Đóng gói dữ liệu vào MemoryStream
                using MemoryStream memoryStream = new MemoryStream();
                using BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.UTF8);

                // Ghi độ dài dữ liệu (4 byte)
                writer.Write(dataLength);
                // Ghi mã Protocol (4 byte)
                writer.Write((int)protocol);
                // Ghi mảng byte nội dung tin nhắn
                writer.Write(messageBytes);

                writer.Flush();

                // 4. Lấy mảng byte hoàn chỉnh và gửi qua NetworkStream mạng
                byte[] packet = memoryStream.ToArray();

                // Sử dụng Semaphore hoặc khóa lock nếu có nhiều luồng cùng gọi SendAsync (Khuyến nghị nếu chat nhóm đông)
                await _stream.WriteAsync(packet, 0, packet.Length);
                await _stream.FlushAsync();
            }
            catch
            {
                Disconnect();
            }
        }


        public void Disconnect()
        {
            if (!_isConnected)
            {
                return;
            }

            _isConnected = false;

            if (_stream != null)
            {
                _stream.Close();
            }

            _tcpClient.Close();

            if (OnDisconnected != null)
            {
                OnDisconnected(Id);
            }
        }
    }
}