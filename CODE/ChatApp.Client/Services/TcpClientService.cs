using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using ChatApp.Shared.Network; // Đảm bảo chứa enum Protocol và class MessagePacket

namespace ChatApp.Client.Services
{
    public class TcpClientService
    {
        private static TcpClientService _instance;
        public static TcpClientService Instance => _instance ??= new TcpClientService();

        private TcpClient _client;
        private NetworkStream _stream;
        private BinaryReader _binaryReader;
        private BinaryWriter _binaryWriter;
        private Thread _listenThread;

        public event Action<MessagePacket> OnPacketReceived;
        public event Action OnDisconnected;

        private TcpClientService() { }

        public bool Connect(string ip, int port)
        {
            try
            {
                if (_client != null && _client.Connected) return true;

                _client = new TcpClient();
                _client.Connect(ip, port);
                _stream = _client.GetStream();

                _binaryReader = new BinaryReader(_stream, Encoding.UTF8);
                _binaryWriter = new BinaryWriter(_stream, Encoding.UTF8);

                _listenThread = new Thread(ListenServer) { IsBackground = true };
                _listenThread.Start();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ListenServer()
        {
            try
            {
                while (_client != null && _client.Connected)
                {
                    // 1. Đọc 4 byte đầu tiên lấy tổng độ dài gói
                    int dataLength = _binaryReader.ReadInt32();

                    // 2. Đọc 4 byte tiếp theo lấy mã Protocol enum
                    int protocolInt = _binaryReader.ReadInt32();
                    Protocol protocol = (Protocol)protocolInt;

                    int targetMessageLength = dataLength - 4;

                    // 3. Cơ chế REAL-TIME: Ép vòng lặp gom đủ 100% byte dữ liệu mạng (Tránh lỗi hụt byte khi gửi Avatar nặng)
                    byte[] messageBytes = new byte[targetMessageLength];
                    int totalBytesRead = 0;

                    while (totalBytesRead < targetMessageLength)
                    {
                        int bytesRead = _binaryReader.Read(messageBytes, totalBytesRead, targetMessageLength - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Mất kết nối đột ngột khi đang nhận gói tin.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    // 4. Giải mã chuỗi JSON và bắn sự kiện lên giao diện
                    string jsonString = Encoding.UTF8.GetString(messageBytes);

                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var packet = JsonSerializer.Deserialize<MessagePacket>(jsonString, options);

                        if (packet != null)
                        {
                            // Đồng bộ lại loại giao thức nhị phân vào thuộc tính thích hợp của bạn (ví dụ: packet.Type)
                            packet.Type = protocol;
                            OnPacketReceived?.Invoke(packet);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi giải mã JSON: {ex.Message}");
                    }
                }
            }
            catch
            {
                // Xử lý khi ngắt kết nối hoặc mất mạng đột ngột
            }
            finally
            {
                OnDisconnected?.Invoke();
                Disconnect();
            }
        }

        public void SendPacket(MessagePacket packet)
        {
            try
            {
                if (_client == null || !_client.Connected) return;

                string jsonString = JsonSerializer.Serialize(packet);
                byte[] messageBytes = Encoding.UTF8.GetBytes(jsonString);

                // Tổng độ dài dữ liệu = 4 byte enum Protocol + kích thước mảng byte chuỗi JSON
                int dataLength = 4 + messageBytes.Length;

                // Ghi nhị phân xuống luồng mạng mạng
                _binaryWriter.Write(dataLength);
                _binaryWriter.Write((int)packet.Type);
                _binaryWriter.Write(messageBytes);
                _binaryWriter.Flush();
            }
            catch { }
        }

        public void Disconnect()
        {
            try
            {
                _binaryReader?.Close();
                _binaryWriter?.Close();
                _stream?.Close();
                _client?.Close();
            }
            catch { }
        }
    }
}
