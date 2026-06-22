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
        public string? ClientName { get; set; }
        public string? AvatarBase64 { get; set; }

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream? _stream;
        private readonly MessageRouter _router;
        private bool _isConnected;

        public event Action<Guid>? OnDisconnected;

        public ClientHandler(Guid id, TcpClient tcpClient, MessageRouter router)
        {
            Id = id;
            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _router = router ?? throw new ArgumentNullException(nameof(router));

            try
            {
                _stream = tcpClient.GetStream();
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
            if (_stream == null) return;

            var reader = new BinaryReader(_stream);
            try
            {
                while (_isConnected && _tcpClient.Connected)
                {
                    int dataLength = reader.ReadInt32();
                    int protocolInt = reader.ReadInt32();
                    Protocol protocol = (Protocol)protocolInt;

                    byte[] messageBytes = reader.ReadBytes(dataLength - 4);
                    // Giải mã UTF-8 đảm bảo giữ nguyên được các ký tự Emoji biểu cảm
                    string message = Encoding.UTF8.GetString(messageBytes) ?? string.Empty;

                    _router.Route(Id, protocol, message, this);
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public async void SendAsync(Protocol protocol, string message)
        {
            string safeMessage = message ?? string.Empty;

            if (_stream == null || !_isConnected || !_tcpClient.Connected) return;

            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(safeMessage);
                int totalLength = messageBytes.Length + 4;

                using var memoryStream = new MemoryStream();
                using var writer = new BinaryWriter(memoryStream);

                writer.Write(totalLength);
                writer.Write((int)protocol);
                writer.Write(messageBytes);

                byte[] packet = memoryStream.ToArray();

                await _stream.WriteAsync(packet, 0, packet.Length);
                await _stream.FlushAsync();
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (!_isConnected) return;

            _isConnected = false;
            _stream?.Close();
            _tcpClient?.Close();

            OnDisconnected?.Invoke(Id);
        }
    }
}
