namespace ChatApp.Server.Forms
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;

        // Khai báo các thành phần giao diện
        private System.Windows.Forms.GroupBox gbServerConfig;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TabControl tabChat;
        private System.Windows.Forms.TabPage tabPublicChat;
        private System.Windows.Forms.RichTextBox rtbPublicChat;
        private System.Windows.Forms.TabPage tabPrivateChat;
        private System.Windows.Forms.RichTextBox rtbPrivateChat;
        private System.Windows.Forms.GroupBox gbActiveClients;
        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colUsername;
        private System.Windows.Forms.ColumnHeader colIPAddress;
        private System.Windows.Forms.ColumnHeader colJoinTime;
        private System.Windows.Forms.Button btnKickSelected;
        private System.Windows.Forms.Button btnKickAll;
        private System.Windows.Forms.PictureBox pbServerAvatar;
        private System.Windows.Forms.Label lblReplyingTo;
        private System.Windows.Forms.TextBox txtMessageInput;
        private System.Windows.Forms.Button btnEmoji;
        private System.Windows.Forms.Button btnSendToAll;
        private System.Windows.Forms.Button btnSendToSelected;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Khởi tạo đối tượng
            this.gbServerConfig = new System.Windows.Forms.GroupBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabChat = new System.Windows.Forms.TabControl();
            this.tabPublicChat = new System.Windows.Forms.TabPage();
            this.rtbPublicChat = new System.Windows.Forms.RichTextBox();
            this.tabPrivateChat = new System.Windows.Forms.TabPage();
            this.rtbPrivateChat = new System.Windows.Forms.RichTextBox();
            this.gbActiveClients = new System.Windows.Forms.GroupBox();
            this.lvClients = new System.Windows.Forms.ListView();
            this.colNo = new System.Windows.Forms.ColumnHeader();
            this.colUsername = new System.Windows.Forms.ColumnHeader();
            this.colIPAddress = new System.Windows.Forms.ColumnHeader();
            this.colJoinTime = new System.Windows.Forms.ColumnHeader();
            this.btnKickSelected = new System.Windows.Forms.Button();
            this.btnKickAll = new System.Windows.Forms.Button();
            this.pbServerAvatar = new System.Windows.Forms.PictureBox();
            this.lblReplyingTo = new System.Windows.Forms.Label();
            this.txtMessageInput = new System.Windows.Forms.TextBox();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.btnSendToAll = new System.Windows.Forms.Button();
            this.btnSendToSelected = new System.Windows.Forms.Button();

            this.gbServerConfig.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tabChat.SuspendLayout();
            this.tabPublicChat.SuspendLayout();
            this.tabPrivateChat.SuspendLayout();
            this.gbActiveClients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbServerAvatar)).BeginInit();
            this.SuspendLayout();

            // ================== CẤU HÌNH CỤ THỂ CHO GB_SERVER_CONFIG ==================
            this.gbServerConfig.Location = new System.Drawing.Point(12, 10);
            this.gbServerConfig.Size = new System.Drawing.Size(1160, 75);
            this.gbServerConfig.Text = "Cấu Hình Máy Chủ (Server Configuration)";

            this.lblIP.Location = new System.Drawing.Point(20, 30);
            this.lblIP.Size = new System.Drawing.Size(70, 20);
            this.lblIP.Text = "Địa chỉ IP:";

            this.txtIP.Location = new System.Drawing.Point(100, 26);
            this.txtIP.Size = new System.Drawing.Size(140, 23);
            this.txtIP.ReadOnly = true;

            this.lblPort.Location = new System.Drawing.Point(270, 30);
            this.lblPort.Size = new System.Drawing.Size(70, 20);
            this.lblPort.Text = "Cổng (Port):";

            this.txtPort.Location = new System.Drawing.Point(350, 26);
            this.txtPort.Size = new System.Drawing.Size(80, 23);

            this.btnStartServer.Location = new System.Drawing.Point(460, 18);
            this.btnStartServer.Size = new System.Drawing.Size(140, 40);
            this.btnStartServer.Text = "▶ START SERVER";
            this.btnStartServer.BackColor = System.Drawing.Color.LimeGreen;
            this.btnStartServer.ForeColor = System.Drawing.Color.White;
            this.btnStartServer.Click += new System.EventHandler(this.BtnStartServer_Click);

            this.btnStopServer.Location = new System.Drawing.Point(615, 18);
            this.btnStopServer.Size = new System.Drawing.Size(140, 40);
            this.btnStopServer.Text = "■ STOP SERVER";
            this.btnStopServer.BackColor = System.Drawing.Color.Red;
            this.btnStopServer.ForeColor = System.Drawing.Color.White;
            this.btnStopServer.Enabled = false;
            this.btnStopServer.Click += new System.EventHandler(this.BtnStopServer_Click);

            this.lblStatus.Location = new System.Drawing.Point(780, 30);
            this.lblStatus.Size = new System.Drawing.Size(100, 20);
            this.lblStatus.Text = "OFFLINE";
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);

            // Thêm linh kiện con vào khung config máy chủ
            this.gbServerConfig.Controls.Add(this.lblIP);
            this.gbServerConfig.Controls.Add(this.txtIP);
            this.gbServerConfig.Controls.Add(this.lblPort);
            this.gbServerConfig.Controls.Add(this.txtPort);
            this.gbServerConfig.Controls.Add(this.btnStartServer);
            this.gbServerConfig.Controls.Add(this.btnStopServer);
            this.gbServerConfig.Controls.Add(this.lblStatus);

            // ================== CẤU HÌNH PHẦN GIỮA (SPLIT CONTAINER) ==================
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.None; // Tắt Dock để không đè lên thanh config
            this.splitContainerMain.Location = new System.Drawing.Point(12, 95); // Đặt vị trí ở dưới thanh config
            this.splitContainerMain.Size = new System.Drawing.Size(1160, 380);
            this.splitContainerMain.SplitterDistance = 720;

            // Tab Chat bên trái
            this.tabChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPublicChat.Text = "💬 Public Chat (Chung)";
            this.rtbPublicChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPublicChat.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.rtbPublicChat.ForeColor = System.Drawing.Color.Lime;
            this.rtbPublicChat.ReadOnly = true;
            this.tabPublicChat.Controls.Add(this.rtbPublicChat);

            this.tabPrivateChat.Text = "🔒 Private Chat (Riêng tư)";
            this.rtbPrivateChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPrivateChat.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.rtbPrivateChat.ForeColor = System.Drawing.Color.Cyan;
            this.rtbPrivateChat.ReadOnly = true;
            this.tabPrivateChat.Controls.Add(this.rtbPrivateChat);

            this.tabChat.Controls.Add(this.tabPublicChat);
            this.tabChat.Controls.Add(this.tabPrivateChat);
            this.splitContainerMain.Panel1.Controls.Add(this.tabChat);

            // Cấu hình vùng danh sách bên phải
            this.gbActiveClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbActiveClients.Text = "Active Clients (0)";
            this.lvClients.Location = new System.Drawing.Point(10, 25);
            this.lvClients.Size = new System.Drawing.Size(415, 290);
            this.lvClients.View = System.Windows.Forms.View.Details;
            this.lvClients.FullRowSelect = true;
            this.lvClients.MultiSelect = false;
            this.lvClients.Columns.AddRange(new[] { this.colNo, this.colUsername, this.colIPAddress, this.colJoinTime });

            this.colNo.Text = "STT"; this.colNo.Width = 45;
            this.colUsername.Text = "Tên Người Dùng"; this.colUsername.Width = 130;
            this.colIPAddress.Text = "Mã ID Mạng"; this.colIPAddress.Width = 130;
            this.colJoinTime.Text = "Thời Gian"; this.colJoinTime.Width = 100;

            this.btnKickSelected.Location = new System.Drawing.Point(10, 330);
            this.btnKickSelected.Size = new System.Drawing.Size(200, 38);
            this.btnKickSelected.Text = "❌ TRỤC XUẤT NGƯỜI CHỌN";
            this.btnKickSelected.Click += new System.EventHandler(this.BtnKickSelected_Click);

            this.btnKickAll.Location = new System.Drawing.Point(225, 330);
            this.btnKickAll.Size = new System.Drawing.Size(180, 38);
            this.btnKickAll.Text = "🚫 Trục Xuất Tất Cả";
            this.btnKickAll.Click += new System.EventHandler(this.BtnKickAll_Click);

            this.gbActiveClients.Controls.Add(this.lvClients);
            this.gbActiveClients.Controls.Add(this.btnKickSelected);
            this.gbActiveClients.Controls.Add(this.btnKickAll);
            this.splitContainerMain.Panel2.Controls.Add(this.gbActiveClients);

            // 🛠️ TỐI ƯU CẤU HÌNH AVATAR - ĐƯA LÊN GÓC TRÊN CÙNG BÊN PHẢI THANH CONFIG
            this.pbServerAvatar.Location = new System.Drawing.Point(900, 15); // Đặt vị trí cạnh chữ OFFLINE
            this.pbServerAvatar.Size = new System.Drawing.Size(45, 45); // Thu nhỏ vuông vắn vừa vặn với thanh trên
            this.pbServerAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbServerAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbServerAvatar.BackColor = System.Drawing.Color.DarkGray; // Tạo nền xám để hiện rõ khung ảnh lúc trống

            // Đăng ký sự kiện click chọn người dùng trong danh sách để xem ảnh đại diện
            this.lvClients.SelectedIndexChanged += new System.EventHandler(this.LvClients_SelectedIndexChanged);

            // Thêm linh kiện vào thanh cấu hình máy chủ (Bao gồm cả pbServerAvatar)
            this.gbServerConfig.Controls.Add(this.lblIP);
            this.gbServerConfig.Controls.Add(this.txtIP);
            this.gbServerConfig.Controls.Add(this.lblPort);
            this.gbServerConfig.Controls.Add(this.txtPort);
            this.gbServerConfig.Controls.Add(this.btnStartServer);
            this.gbServerConfig.Controls.Add(this.btnStopServer);
            this.gbServerConfig.Controls.Add(this.lblStatus);
            this.gbServerConfig.Controls.Add(this.pbServerAvatar); // Đã đưa Avatar vào đây an toàn

            // ================== CẤU HÌNH CÁC NÚT ĐIỀU KHIỂN DƯỚI CÙNG ==================
            this.lblReplyingTo.Location = new System.Drawing.Point(120, 485);
            this.lblReplyingTo.Size = new System.Drawing.Size(750, 20);
            this.lblReplyingTo.Text = "Trạng thái phản hồi: Trống (Chưa chọn client)";

            this.txtMessageInput.Location = new System.Drawing.Point(120, 510);
            this.txtMessageInput.Size = new System.Drawing.Size(810, 65);
            this.txtMessageInput.Multiline = true;

            this.btnEmoji.Location = new System.Drawing.Point(950, 510);
            this.btnEmoji.Size = new System.Drawing.Size(60, 65);
            this.btnEmoji.Text = "😊";
            this.btnEmoji.Click += new System.EventHandler(this.BtnEmoji_Click);

            this.btnSendToAll.Location = new System.Drawing.Point(1020, 510);
            this.btnSendToAll.Size = new System.Drawing.Size(150, 32);
            this.btnSendToAll.Text = "🚀 GỬI CẢ PHÒNG";
            this.btnSendToAll.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSendToAll.ForeColor = System.Drawing.Color.White;
            this.btnSendToAll.Click += new System.EventHandler(this.BtnSendToAll_Click);

            this.btnSendToSelected.Location = new System.Drawing.Point(1020, 545);
            this.btnSendToSelected.Size = new System.Drawing.Size(150, 32);
            this.btnSendToSelected.Text = "📤 Gửi Riêng (Private)";
            this.btnSendToSelected.Click += new System.EventHandler(this.BtnSendToSelected_Click);

            // ================== ĐƯA TOÀN BỘ LINH KIỆN VÀO KHUNG FORM CHÍNH ==================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1194, 631);
            this.Text = "Bảng Điều Khiển Chat Mạng Máy Chủ (Server Control Panel)";
            this.MinimumSize = new System.Drawing.Size(1150, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Thêm các GroupBox lớn lên Form theo đúng thứ tự lớp layout
            this.Controls.Add(this.gbServerConfig);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.lblReplyingTo);
            this.Controls.Add(this.txtMessageInput);
            this.Controls.Add(this.btnEmoji);
            this.Controls.Add(this.btnSendToAll);
            this.Controls.Add(this.btnSendToSelected);

            this.gbServerConfig.ResumeLayout(false);
            this.gbServerConfig.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.tabChat.ResumeLayout(false);
            this.tabPublicChat.ResumeLayout(false);
            this.tabPrivateChat.ResumeLayout(false);
            this.gbActiveClients.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbServerAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}