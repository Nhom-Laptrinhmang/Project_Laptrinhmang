// ChatApp.Server/Forms/ServerForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
namespace ChatApp.Server.Forms
{
    public partial class ServerForm : Form
    {
        private bool _isServerRunning = false;
        public ServerForm()
        {
            InitializeComponent();
            LoadDefaultValues();
        }
        private void LoadDefaultValues()
        {
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8080";
            gbActiveClients.Text = "Active Clients (0)";
            lblStatus.Text = "OFFLINE";
            lblStatus.ForeColor = Color.Red;
            btnStopServer.Enabled = false;
        }
        // ====================== EVENT HANDLERS ======================
        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            if (_isServerRunning) return;
            if (int.TryParse(txtPort.Text, out int port))
            {
                _isServerRunning = true;
                btnStartServer.Enabled = false;
                btnStopServer.Enabled = true;
                lblStatus.Text = "RUNNING";
                lblStatus.ForeColor = Color.LimeGreen;
                AppendPublicChat($"[System] Server started on {txtIP.Text}:{port}");
            }
            else
            {
                MessageBox.Show("Port không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnStopServer_Click(object sender, EventArgs e)
        {
            _isServerRunning = false;
            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
            lblStatus.Text = "OFFLINE";
            lblStatus.ForeColor = Color.Red;
            lvClients.Items.Clear();
            gbActiveClients.Text = "Active Clients (0)";
            AppendPublicChat("[System] Server has been stopped.");
        }
        private void BtnKickSelected_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một client để kick!", "Thông báo");
                return;
            }
            MessageBox.Show("Client đã bị kick (sẽ hoàn thiện sau).", "Thông báo");
        }
        private void BtnKickAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn kick tất cả clients?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                lvClients.Items.Clear();
                gbActiveClients.Text = "Active Clients (0)";
                AppendPublicChat("[System] All clients have been kicked.");
            }
        }
        private void BtnSendToAll_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessageInput.Text)) return;
            string msg = txtMessageInput.Text.Trim();
            AppendPublicChat($"[Server Broadcast] {msg}");
            txtMessageInput.Clear();
        }
        private void BtnEmoji_Click(object sender, EventArgs e)
        {
            MessageBox.Show("😊 Emoji Picker (đang phát triển)", "Tính năng");
        }
       private void AppendPublicChat(string message)
{
    if (rtbPublicChat.InvokeRequired)
    {
        rtbPublicChat.Invoke(new Action<string>(AppendPublicChat), message);
        return;
    }
    rtbPublicChat.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    rtbPublicChat.ScrollToCaret();
}
private void AppendPrivateChat(string message)
{
    if (rtbPrivateChat.InvokeRequired)
    {
        rtbPrivateChat.Invoke(new Action<string>(AppendPrivateChat), message);
        return;
    }
    rtbPrivateChat.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    rtbPrivateChat.ScrollToCaret();
}
        // ====================== FORM LOAD ======================
        private void ServerForm_Load(object sender, EventArgs e)
        {
            btnStartServer.Click += BtnStartServer_Click;
            btnStopServer.Click += BtnStopServer_Click;
            btnKickSelected.Click += BtnKickSelected_Click;
            btnKickAll.Click += BtnKickAll_Click;
            btnSendToAll.Click += BtnSendToAll_Click;
            btnEmoji.Click += BtnEmoji_Click;
        }
    }
}