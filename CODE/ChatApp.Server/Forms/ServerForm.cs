using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Concurrent;
using ChatApp.Shared.Network;

namespace ChatApp.Server.Forms
{
    public partial class ServerForm : Form
    {
        private bool _isServerRunning = false;
        private ChatServer? _chatServer;

        // Cuốn sổ lưu tạm Avatar của các Client để giao diện Server tự truy xuất hiển thị
        private readonly ConcurrentDictionary<Guid, string> _clientAvatars = new ConcurrentDictionary<Guid, string>();

        public ServerForm()
        {
            InitializeComponent();
            LoadDefaultValues();

            // Đăng ký sự kiện click chọn dòng trong danh sách Client
            lvClients.SelectedIndexChanged += LvClients_SelectedIndexChanged;

            // Cấu hình Font hỗ trợ hiển thị Emoji cho các ô Log chat trên giao diện Server
            Font emojiFont = new Font("Segoe UI Emoji", 9.5F, FontStyle.Regular);
            if (rtbPublicChat != null) rtbPublicChat.Font = emojiFont;
            if (rtbPrivateChat != null) rtbPrivateChat.Font = emojiFont;
            if (txtMessageInput != null) txtMessageInput.Font = emojiFont;
        }

        private void LoadDefaultValues()
        {
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8080";
            gbActiveClients.Text = "Active Clients (0)";
            lblStatus.Text = "OFFLINE";
            lblStatus.ForeColor = Color.Red;
            btnStopServer.Enabled = false;
            btnStartServer.Enabled = true;
            if (lblReplyingTo != null) lblReplyingTo.Text = "Trạng thái phản hồi: Trống";
        }

        // Hàm này tự động quét dữ liệu từ Server gửi lên để vẽ lại bảng ListView
        public void UpdateClientList(ConcurrentDictionary<Guid, ClientHandler> clients)
        {
            if (lvClients.InvokeRequired)
            {
                lvClients.Invoke(new Action(() => UpdateClientList(clients)));
                return;
            }

            lvClients.Items.Clear();
            _clientAvatars.Clear(); // Làm sạch kho lưu ảnh cũ
            int counter = 1;

            foreach (var client in clients.Values)
            {
                var item = new ListViewItem(counter.ToString());
                item.SubItems.Add(client.ClientName ?? "Ẩn danh");
                item.SubItems.Add(client.Id.ToString().Substring(0, 8));
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
                item.Tag = client.Id; // Lưu mã gốc ẩn vào đây

                lvClients.Items.Add(item);
                counter++;

                // Lưu chuỗi ảnh Base64 của khách vào kho để Form sử dụng
                if (!string.IsNullOrEmpty(client.AvatarBase64))
                {
                    _clientAvatars.TryAdd(client.Id, client.AvatarBase64);
                }
            }

            gbActiveClients.Text = $"Active Clients ({clients.Count})";
        }

        // ====================== SỰ KIỆN CLICK CHỌN ĐỂ XEM AVATAR ======================
        private void LvClients_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                var oldImage = pbServerAvatar.Image;
                pbServerAvatar.Image = null;
                oldImage?.Dispose(); // Giải phóng RAM ảnh cũ

                if (lblReplyingTo != null) lblReplyingTo.Text = "Trạng thái phản hồi: Trống";
                return;
            }

            var selectedItem = lvClients.SelectedItems[0];
            if (selectedItem.Tag is Guid clientId)
            {
                if (lblReplyingTo != null) lblReplyingTo.Text = $"Đang chọn người dùng: {selectedItem.SubItems[1].Text}";

                // Dọn dẹp bộ nhớ ảnh cũ trước khi nạp ảnh mới
                var oldImg = pbServerAvatar.Image;

                if (_clientAvatars.TryGetValue(clientId, out string? base64Str) && !string.IsNullOrEmpty(base64Str))
                {
                    try
                    {
                        if (base64Str == "MOCK_AVATAR_BASE64_STRING")
                        {
                            pbServerAvatar.Image = SystemIcons.Application.ToBitmap();
                            oldImg?.Dispose();
                            return;
                        }

                        byte[] imageBytes = Convert.FromBase64String(base64Str);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            pbServerAvatar.Image = Image.FromStream(ms);
                        }
                    }
                    catch
                    {
                        pbServerAvatar.Image = SystemIcons.Error.ToBitmap();
                    }
                }
                else
                {
                    pbServerAvatar.Image = SystemIcons.Application.ToBitmap();
                }

                // Tiến hành giải phóng vùng nhớ GDI+ của ảnh trước đó
                oldImg?.Dispose();
            }
        }

        // ====================== CÁC SỰ KIỆN NÚT BẤM KHÁC ======================

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            if (_isServerRunning) return;

            if (int.TryParse(txtPort.Text, out int port))
            {
                try
                {
                    _chatServer = new ChatServer(port, this);
                    _chatServer.OnLog += (msg) => AppendPublicChat(msg);
                    _chatServer.Start();

                    _isServerRunning = true;
                    btnStartServer.Enabled = false;
                    btnStopServer.Enabled = true;
                    txtPort.Enabled = false;
                    lblStatus.Text = "RUNNING";
                    lblStatus.ForeColor = Color.LimeGreen;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi không mở được cổng mạng: {ex.Message}", "Lỗi Hệ Thống");
                }
            }
            else
            {
                MessageBox.Show("Cổng mạng (Port) không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStopServer_Click(object sender, EventArgs e)
        {
            if (!_isServerRunning) return;

            _isServerRunning = false;
            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
            txtPort.Enabled = true;
            lblStatus.Text = "OFFLINE";
            lblStatus.ForeColor = Color.Red;

            lvClients.Items.Clear();

            var oldImg = pbServerAvatar.Image;
            pbServerAvatar.Image = null;
            oldImg?.Dispose();

            gbActiveClients.Text = "Active Clients (0)";

            if (_chatServer != null)
            {
                _chatServer.Stop();
                _chatServer = null;
            }
        }

        private void BtnKickSelected_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng click chọn một người dùng từ danh sách bên trên để trục xuất!", "Thông báo");
                return;
            }

            var selectedItem = lvClients.SelectedItems[0];
            if (selectedItem.Tag is Guid clientId && _chatServer != null)
            {
                _chatServer.KickClient(clientId);
                AppendPublicChat($"[Hệ Thống] Người dùng [{selectedItem.SubItems[1].Text}] đã bị Admin trục xuất.");
            }
        }

        private void BtnKickAll_Click(object sender, EventArgs e)
        {
            if (lvClients.Items.Count == 0) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn giải tán phòng và trục xuất tất cả thành viên?", "Xác nhận hành động",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _chatServer?.KickAll();
                lvClients.Items.Clear();

                var oldImg = pbServerAvatar.Image;
                pbServerAvatar.Image = null;
                oldImg?.Dispose();

                gbActiveClients.Text = "Active Clients (0)";
            }
        }

        private void BtnSendToAll_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessageInput.Text) || _chatServer == null) return;

            string msg = txtMessageInput.Text.Trim();

            var packet = new MessagePacket
            {
                Type = Protocol.Message,
                MessageId = Guid.NewGuid().ToString(),
                Sender = "Hệ thống",
                Content = $"[Quản Trị Viên]: {msg}",
                Receiver = "All",
                AvatarBase64 = "",
                ReplyToId = "",
                ReplyToContent = ""
            };

            // Sử dụng thư viện System.Text.Json chuẩn của hệ thống để đồng bộ hóa tốc độ cao
            string jsonPacket = System.Text.Json.JsonSerializer.Serialize(packet);

            _chatServer.Broadcast(Protocol.Message, jsonPacket);

            AppendPublicChat($"[Server Broadcast] {msg}");
            txtMessageInput.Clear();
        }

        private void BtnSendToSelected_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng click chọn một tài khoản từ danh sách Active Clients để nhắn tin mật!", "Thông báo");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMessageInput.Text) || _chatServer == null) return;

            var selectedItem = lvClients.SelectedItems[0];

            if (selectedItem.Tag is Guid clientId)
            {
                string msg = txtMessageInput.Text.Trim();
                string receiverName = selectedItem.SubItems[1].Text;

                var packet = new MessagePacket
                {
                    Type = Protocol.PrivateMessage,
                    MessageId = Guid.NewGuid().ToString(),
                    Sender = "Hệ thống (Mật)",
                    Content = msg,
                    Receiver = receiverName,
                    AvatarBase64 = "",
                    ReplyToId = "",
                    ReplyToContent = ""
                };

                string jsonPacket = System.Text.Json.JsonSerializer.Serialize(packet);

                bool isSent = _chatServer.SendToTarget(clientId, Protocol.PrivateMessage, jsonPacket);

                if (isSent)
                {
                    AppendPublicChat($"[Server gửi mật tới {receiverName}]: {msg}");
                }
                else
                {
                    MessageBox.Show("Gửi thất bại! Người dùng này có thể đã offline.", "Lỗi");
                }

                txtMessageInput.Clear();
            }
        }

        // FIX DỨT ĐIỂM LỖI EMOJI: Chèn danh sách biểu tượng cảm xúc dạng văn bản Unicode chuẩn vào ô nhập liệu
        // FIX NÂNG CẤP: Bấm vào hiện ra 1 danh sách (Menu) Emoji để chọn từng cái
        private void BtnEmoji_Click(object sender, EventArgs e)
        {
            if (txtMessageInput == null) return;

            // 1. Khởi tạo một Menu thả xuống
            ContextMenuStrip emojiMenu = new ContextMenuStrip();

            // Đặt font Segoe UI Emoji để các biểu tượng cảm xúc không bị lỗi ô vuông
            emojiMenu.Font = new Font("Segoe UI Emoji", 10F, FontStyle.Regular);

            // 2. Danh sách các Emoji bạn muốn cho Admin chọn
            string[] emojis = { "😊", "❤️", "😂", "👍", "🔥", "😎", "😮", "😢", "🎉", "👏" };

            // 3. Vòng lặp tự động tạo từng dòng Item cho Menu
            foreach (string emoji in emojis)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(emoji);

                // Sự kiện khi Admin click chọn 1 Emoji cụ thể trong List
                item.Click += (s, ev) =>
                {
                    txtMessageInput.AppendText(emoji); // Chèn emoji vào ô nhập liệu
                    txtMessageInput.Focus();           // Giữ con trỏ chuột ở ô nhập
                };

                emojiMenu.Items.Add(item);
            }

            // 4. Hiển thị Menu ngay tại vị trí của nút Emoji trên giao diện
            if (sender is Control btn)
            {
                emojiMenu.Show(btn, new Point(0, btn.Height));
            }
        }

        public void AppendPublicChat(string message)
        {
            if (rtbPublicChat.InvokeRequired)
            {
                rtbPublicChat.Invoke(new Action<string>(AppendPublicChat), message);
                return;
            }
            rtbPublicChat.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            rtbPublicChat.ScrollToCaret();
        }

        public void AppendPrivateChat(string message)
        {
            if (rtbPrivateChat.InvokeRequired)
            {
                rtbPrivateChat.Invoke(new Action<string>(AppendPrivateChat), message);
                return;
            }
            rtbPrivateChat.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            rtbPrivateChat.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _chatServer?.Stop();
            base.OnFormClosing(e);
        }
    }
}