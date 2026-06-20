using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatApp.Shared.Services 
    public class TcpClientService
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        // Tạo sự kiện (event) để báo cho Form (UI) biết khi có tin nhắn mới
        public delegate void MessageReceivedHandler(string messageData);
        public event MessageReceivedHandler OnMessageReceived;
        public void Connect(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ip, port);
                _stream = _client.GetStream();
                // Tạo một luồng (Thread) riêng chạy ngầm để liên tục đọc tin nhắn từ Server
                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true; 
                _receiveThread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể kết nối đến Server: " + ex.Message);
            }
        }
        public void SendData(string data)
        {
            if (_client != null && _client.Connected && _stream != null)
            {
                // Chuyển chuỗi thành mảng byte để gửi đi
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                _stream.Write(buffer, 0, buffer.Length);
            }
        }
        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[2048]; // Bộ đệm 2KB
                while (true)
                {
                    if (_stream != null && _stream.DataAvailable)
                    {
                        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            // Giải mã byte nhận được thành chuỗi
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);                          
                            // Bắn sự kiện ra ngoài cho Form xử lý
                            OnMessageReceived?.Invoke(receivedData);
                        }
                    }
                    Thread.Sleep(100); // Tạm nghỉ để tránh ngốn CPU
                }
            }
            catch
            {
                // Xử lý khi mất kết nối (có thể log lỗi hoặc bắn event ngắt kết nối)
            }
        }

        public void Disconnect()
        {
            if (_stream != null) _stream.Close();
            if (_client != null) _client.Close();
        }
    }
}
