using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using ChatApp.Shared.Network;

namespace ChatApp.Server
{
    public class ClientHandler
    {
        // --- CÁC BIẾN QUẢN LÝ KẾT NỐI VẬT LÝ ---
        private TcpClient _client;
        private Server _server;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Thread _handlerThread;

        // Tên tài khoản của người dùng tương ứng với luồng kết nối này
        public string Username { get; private set; }

        // --- HÀM KHỞI TẠO (KHI CLIENT CHẠM ĐƯỢC VÀO SERVER) ---
        public ClientHandler(TcpClient client, Server server)
        {
            _client = client;
            _server = server;

            // Lấy dòng chảy dữ liệu (Stream) giữa 2 máy
            _stream = client.GetStream();

            // Thiết lập công cụ đọc / ghi dữ liệu dạng chuỗi với định dạng UTF-8 chống lỗi font
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

            // Tạo một luồng chạy ngầm độc lập dành riêng cho người dùng này để nghe ngóng tin nhắn công việc
            _handlerThread = new Thread(ListenAndProcess);
            _handlerThread.IsBackground = true;
            _handlerThread.Start();

            // Đưa bản thân kết nối này vào danh sách quản lý chung của Server
            _server.AddClient(this);
        }

        // --- LUỒNG LẮNG NGHE CHUỖI JSON TỪ MẠNG ---
        private void ListenAndProcess()
        {
            try
            {
                while (true)
                {
                    // Chờ và đọc một dòng văn bản (kết thúc bằng dấu xuống dòng) gửi lên từ Client
                    string jsonString = _reader.ReadLine();

                    // Nếu Client ngắt kết nối đột ngột, chuỗi thu được sẽ rỗng (null) -> Thoát khỏi vòng lặp
                    if (jsonString == null) break;

                    // Dịch mã chuỗi văn bản JSON thành cấu trúc gói tin MessagePacket dễ hiểu
                    var packet = JsonSerializer.Deserialize<MessagePacket>(jsonString);
                    if (packet != null)
                    {
                        // Đẩy gói tin sang khâu phân loại lệnh hành động
                        ProcessPacket(packet);
                    }
                }
            }
            catch
            {
                // Bắt các lỗi mất mạng vật lý đột ngột
            }
            finally
            {
                // Khi kết nối bị đứt, xóa Client này khỏi danh sách chung và đóng Socket
                _server.RemoveClient(this);
                Disconnect();
            }
        }

        // --- PHÂN LOẠI HÀNH ĐỘNG CỦA GÓI TIN ---
        private void ProcessPacket(MessagePacket packet)
        {
            switch (packet.Command)
            {
                // Hành động 1: Client xin đăng nhập đặt tên tài khoản
                case CommandType.LoginRequest:
                    string requestedName = packet.Sender.Trim();

                    // Kiểm tra bảo mật tên trống, tên trùng, hoặc trùng từ khóa hệ thống "All"
                    if (string.IsNullOrEmpty(requestedName) || _server.IsUsernameTaken(requestedName) || requestedName.ToLower() == "all")
                    {
                        SendPacket(new MessagePacket(CommandType.LoginResponse, "Server", "Failed"));
                    }
                    else
                    {
                        // Lưu tên định danh cho luồng này
                        Username = requestedName;

                        // Trả lời phản hồi đăng nhập thành công về cho Client biết
                        SendPacket(new MessagePacket(CommandType.LoginResponse, "Server", "Success"));

                        _server.Log($"Người dùng '{Username}' đăng nhập thành công.");

                        // Đồng bộ lại danh sách online mới nhất gửi cho tất cả các máy Client
                        _server.BroadcastUserList();
                        _server.Broadcast(new MessagePacket(CommandType.BroadcastMessage, "Hệ thống", $"{Username} đã tham gia phòng chat."));
                    }
                    break;

                // Hành động 2: Tin nhắn chat nhóm hoặc chat cá nhân riêng tư
                case CommandType.BroadcastMessage:
                case CommandType.PrivateMessage:
                    // 🔥 ĐÃ SỬA: Ép đi qua bộ định tuyến MessageRouter để lọc Reply và lưu lịch sử
                    _server.MessageRouterInstance.Route(packet);
                    break;
            }
        }

        // --- GỬI GÓI TIN XUỐNG MÁY CLIENT ---
        public void SendPacket(MessagePacket packet)
        {
            try
            {
                // Chuyển đối tượng C# thành chuỗi JSON và viết đẩy qua Socket mạng
                string jsonString = JsonSerializer.Serialize(packet);
                _writer.WriteLine(jsonString);
            }
            catch
            {
                // Bỏ qua lỗi nếu đường truyền phía Client bị ngắt giữa chừng
            }
        }

        // --- ĐÓNG KẾT NỐI AN TOÀN ---
        public void Disconnect()
        {
            try
            {
                _reader?.Close();
                _writer?.Close();
                _stream?.Close();
                _client?.Close();
            }
            catch { }
        }
    }
}