using System.Drawing;
using System.Windows.Forms;

namespace ChatApp.Client.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Bổ sung thêm các Label để giao diện rõ ràng hơn
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblIP;
        private Label lblPort;

        private TextBox txtUsername;
        private TextBox txtIP;
        private TextBox txtPort;
        private Button btnConnect;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblUsername = new Label();
            lblIP = new Label();
            lblPort = new Label();
            txtUsername = new TextBox();
            txtIP = new TextBox();
            txtPort = new TextBox();
            btnConnect = new Button();

            SuspendLayout();

            // ====== FORM SETTINGS ======
            ClientSize = new Size(360, 480);
            Text = "Đăng nhập hệ thống";
            BackColor = Color.White; // Nền trắng sạch sẽ
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            StartPosition = FormStartPosition.CenterScreen; // Mở form ở giữa màn hình
            FormBorderStyle = FormBorderStyle.FixedDialog; // Khóa thay đổi kích thước
            MaximizeBox = false;

            int marginX = 40;
            int controlWidth = ClientSize.Width - (marginX * 2);

            // ====== TITLE ======
            lblTitle.Text = "Welcome Back";
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(marginX, 30);

            lblSubtitle.Text = "Vui lòng nhập thông tin để kết nối";
            lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(marginX, lblTitle.Bottom + 5);

            // ====== TÊN ĐĂNG NHẬP ======
            lblUsername.Text = "Tên hiển thị (Username)";
            lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(50, 50, 50);
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(marginX, lblSubtitle.Bottom + 40);

            txtUsername.Location = new Point(marginX, lblUsername.Bottom + 8);
            txtUsername.Size = new Size(controlWidth, 30);
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // ====== ĐỊA CHỈ IP ======
            lblIP.Text = "Địa chỉ IP Máy chủ";
            lblIP.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblIP.ForeColor = Color.FromArgb(50, 50, 50);
            lblIP.AutoSize = true;
            lblIP.Location = new Point(marginX, txtUsername.Bottom + 20);

            txtIP.Location = new Point(marginX, lblIP.Bottom + 8);
            txtIP.Size = new Size(controlWidth, 30);
            txtIP.Font = new Font("Segoe UI", 11F);
            txtIP.BorderStyle = BorderStyle.FixedSingle;
            txtIP.Text = "127.0.0.1"; // Default IP

            // ====== CỔNG (PORT) ======
            lblPort.Text = "Cổng kết nối (Port)";
            lblPort.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPort.ForeColor = Color.FromArgb(50, 50, 50);
            lblPort.AutoSize = true;
            lblPort.Location = new Point(marginX, txtIP.Bottom + 20);

            txtPort.Location = new Point(marginX, lblPort.Bottom + 8);
            txtPort.Size = new Size(controlWidth, 30);
            txtPort.Font = new Font("Segoe UI", 11F);
            txtPort.BorderStyle = BorderStyle.FixedSingle;
            txtPort.Text = "8080"; // Default Port

            // ====== NÚT ĐĂNG NHẬP ======
            btnConnect.Location = new Point(marginX, txtPort.Bottom + 40);
            btnConnect.Size = new Size(controlWidth, 45);
            btnConnect.Text = "KẾT NỐI NGAY";
            btnConnect.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            // Xóa viền 3D cổ điển, dùng màu xanh chuẩn Messenger
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.BackColor = Color.FromArgb(0, 132, 255);
            btnConnect.ForeColor = Color.White;
            btnConnect.Cursor = Cursors.Hand;

            // Đăng ký sự kiện click (Giữ nguyên logic cũ của bạn)
            btnConnect.Click += btnConnect_Click;

            // ====== THÊM VÀO FORM ======
            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);
            Controls.Add(lblUsername);
            Controls.Add(lblIP);
            Controls.Add(lblPort);
            Controls.Add(txtUsername);
            Controls.Add(txtIP);
            Controls.Add(txtPort);
            Controls.Add(btnConnect);

            ResumeLayout(false);
            PerformLayout(); // Đảm bảo các AutoSize Label hiển thị chuẩn
        }
    }
}