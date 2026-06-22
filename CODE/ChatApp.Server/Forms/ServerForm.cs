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
            lblReplyingTo.Text = "Trạng thái phản hồi: Trống";
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
                pbServerAvatar.Image = null;
                lblReplyingTo.Text = "Trạng thái phản hồi: Trống";
                return;
            }

            var selectedItem = lvClients.SelectedItems[0]; // Sửa lấy phần tử đầu tiên
            if (selectedItem.Tag is Guid clientId)
            {
                lblReplyingTo.Text = $"Đang chọn người dùng: {selectedItem.SubItems[1].Text}";

                if (_clientAvatars.TryGetValue(clientId, out string? base64Str) && !string.IsNullOrEmpty(base64Str))
                {
                    try
                    {
                        if (base64Str == "MOCK_AVATAR_BASE64_STRING")
                        {
                           
                            pbServerAvatar.Image = SystemIcons.Application.ToBitmap();
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
                    // SỬA TẠI ĐÂY: Đổi sang Application nếu không tìm thấy ảnh đại diện
                    pbServerAvatar.Image = SystemIcons.Application.ToBitmap();
                }
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
            pbServerAvatar.Image = null;
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
                pbServerAvatar.Image = null;
                gbActiveClients.Text = "Active Clients (0)";
            }
        }

        private void BtnSendToAll_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessageInput.Text) || _chatServer == null) return;

            string msg = txtMessageInput.Text.Trim();
            _chatServer.Broadcast(Protocol.Message, $"[Quản Trị Viên]: {msg}");
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
            if (selectedItem.Tag is Guid targetClientId)
            {
                string msg = txtMessageInput.Text.Trim();
                _chatServer.SendToTarget(targetClientId, Protocol.PrivateMessage, $"[Tin nhắn mật từ Server]: {msg}");
                AppendPrivateChat($"[Mật tới {selectedItem.SubItems[1].Text}]: {msg}");
                txtMessageInput.Clear();
            }
        }

        private void BtnEmoji_Click(object sender, EventArgs e)
        {
            txtMessageInput.AppendText("😊❤️😂👍🔥");
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
            _chatServer?.Stop(); base.OnFormClosing(e); }
    }
}
