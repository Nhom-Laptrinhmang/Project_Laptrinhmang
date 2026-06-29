using System;
using System.Drawing;
using System.Windows.Forms;
using ChatApp.Client.Services;
using ChatApp.Shared.Network;

namespace ChatApp.Client.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            // Nạp cấu hình giao diện từ file Designer của bạn
            InitializeComponent();

            // Cấu hình vị trí xuất hiện ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        // Đảm bảo tên hàm trùng khớp hoàn toàn với liên kết sự kiện 'btnConnect.Click += btnConnect_Click;' trong Designer
        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Sử dụng chính xác tên biến 'txtIP' từ file Designer của bạn
            string ip = txtIP.Text.Trim();
            string username = txtUsername.Text.Trim();

            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Cổng kết nối (Port) phải là định dạng số nguyên hợp lệ!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Vui lòng điền Tên tài khoản trước khi kết nối!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vô hiệu hóa nút bấm tạm thời để tránh người dùng nhấn liên tục khi đang kết nối
            btnConnect.Enabled = false;
            btnConnect.Text = "Connecting...";

            // Gọi hàm kết nối nhị phân real-time
            bool isConnected = TcpClientService.Instance.Connect(ip, port);

            if (isConnected)
            {
                // Đóng gói gói tin đăng nhập gửi lên Server
                var loginPacket = new MessagePacket
                {
                    Type = Protocol.Message,
                    Sender = username,
                    Content = $"[Hệ thống]: Người dùng [{username}] đã tham gia phòng chat."
                };
                TcpClientService.Instance.SendPacket(loginPacket);

                // Ẩn màn hình đăng nhập hiện tại và mở màn hình ChatForm chính lên
                this.Hide();
                ChatForm chatForm = new ChatForm(username);
                chatForm.ShowDialog();

                // Giải phóng hoàn toàn bộ nhớ Form đăng nhập sau khi đóng ChatForm
                this.Close();
            }
            else
            {
                MessageBox.Show("Kết nối tới máy chủ thất bại!\nVui lòng kiểm tra lại địa chỉ IP, Cổng (Port) hoặc trạng thái của Server.", "Lỗi mạng Real-time", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Khôi phục lại trạng thái nút bấm cho người dùng thử lại
                btnConnect.Enabled = true;
                btnConnect.Text = "Connect";
            }
        }
    }
}
