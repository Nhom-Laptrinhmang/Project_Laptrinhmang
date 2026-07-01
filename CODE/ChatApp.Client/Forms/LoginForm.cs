using System;
using System.Drawing;
using System.Windows.Forms;
using ChatApp.Client.Services;
using ChatApp.Shared.Network;

namespace ChatApp.Client.Forms
{
    public partial class LoginForm : Form
    {
        private string _username = "";

        // ==========================================================
        // CỜ CHỐNG MỞ TAB KÉP (Khóa luồng khi đã đăng nhập thành công)
        // ==========================================================
        private bool _isChatFormOpened = false;

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text.Trim();
            _username = txtUsername.Text.Trim();

            // Kiểm tra tính hợp lệ của Port
            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Cổng kết nối (Port) phải là định dạng số nguyên hợp lệ!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra tính hợp lệ của Tên tài khoản
            if (string.IsNullOrEmpty(_username))
            {
                MessageBox.Show("Vui lòng điền Tên tài khoản trước khi kết nối!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Khóa nút UI để tránh click nhiều lần
            btnConnect.Enabled = false;
            btnConnect.Text = "Connecting...";

            // Đặt lại cờ đề phòng trường hợp trước đó kết nối lỗi rồi thử lại
            _isChatFormOpened = false;

            // 1. Kết nối Socket tới Server
            bool isConnected = TcpClientService.Instance.Connect(ip, port);

            if (isConnected)
            {
                // Đăng ký sự kiện lắng nghe phản hồi từ Server
                TcpClientService.Instance.OnPacketReceived += Instance_OnPacketReceived;

                // Đóng gói dữ liệu kết nối
                var connectPacket = new MessagePacket
                {
                    Type = Protocol.Connect,
                    Sender = _username,
                    Content = ""
                };

                // Bắn gói tin lên Server
                TcpClientService.Instance.SendPacket(connectPacket);
            }
            else
            {
                MessageBox.Show("Kết nối tới máy chủ thất bại!\nVui lòng kiểm tra lại địa chỉ IP, Cổng (Port) hoặc trạng thái của Server.", "Lỗi mạng Real-time", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Mở lại nút UI nếu lỗi mạng
                btnConnect.Enabled = true;
                btnConnect.Text = "Connect";
            }
        }

        private void Instance_OnPacketReceived(MessagePacket packet)
        {
            if (packet == null) return;

            // Đảm bảo thao tác với giao diện UI được thực hiện trên luồng chính (Thread-Safe)
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Instance_OnPacketReceived(packet)));
                return;
            }

            // [QUAN TRỌNG NHẤT]: Nếu đã lỡ mở 1 ChatForm rồi thì chặn đứng các lệnh tiếp theo lại!
            if (_isChatFormOpened) return;

            // Trường hợp 1: Bị Server từ chối (Do trùng tên đăng nhập hoặc bị kick)
            if (packet.Type == Protocol.Disconnect && packet.Sender == "Hệ thống")
            {
                // Ngắt sự kiện lắng nghe để tránh lỗi chồng chéo logic
                TcpClientService.Instance.OnPacketReceived -= Instance_OnPacketReceived;

                // Ép Client đóng Socket: Giải phóng cổng mạng ngay lập tức
                TcpClientService.Instance.Disconnect();

                // Hiển thị thông báo lỗi từ Server (Ví dụ: "Tên đăng nhập đã tồn tại")
                MessageBox.Show(packet.Content, "Đăng Nhập Thất Bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Khôi phục lại trạng thái giao diện để người dùng có thể nhập tên khác
                btnConnect.Enabled = true;
                btnConnect.Text = "Connect";
                txtUsername.Focus();
                return;
            }

            // Trường hợp 2: Đăng nhập thành công 
            if (packet.Type == Protocol.UserList || (packet.Type == Protocol.Connect && packet.Content.Contains("tham gia")))
            {
                // Bật cờ khóa ngay lập tức để chặn gói tin thứ 2 đang bay tới
                _isChatFormOpened = true;

                // Ngắt sự kiện lắng nghe ở Form này để nhường toàn quyền xử lý mạng cho ChatForm
                TcpClientService.Instance.OnPacketReceived -= Instance_OnPacketReceived;

                this.Hide();

                // Mở ChatForm và truyền tên đăng nhập vào
                ChatForm chatForm = new ChatForm(_username);
                chatForm.ShowDialog();

                // Đóng hoàn toàn LoginForm khi ChatForm bị tắt
                this.Close();
            }
        }
    }
}