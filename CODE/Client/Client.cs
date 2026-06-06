using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatCilent
{
    public partial class Form1 : Form
    {
        private TcpClient? client;
        private StreamReader? reader;
        private StreamWriter? writer;
        private Thread? receiveThread;
        private bool isConnected = false;

        public Form1()
        {
            InitializeComponent();
            btnConnect.Click += btnConnect_Click;
            btnDisconnect.Click += btnDisconnect_Click;
            btnSend.Click += btnSend_Click;

            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8888";
            txtName.PlaceholderText = "Nhập tên của bạn (Bỏ trống = Ẩn danh)";

            btnDisconnect.Enabled = false;
        }

        private void btnConnect_Click(object? sender, EventArgs e)
        {
            try
            {
                string ip = txtIP.Text.Trim();
                if (!int.TryParse(txtPort.Text.Trim(), out int port) || string.IsNullOrEmpty(ip))
                {
                    MessageBox.Show("IP và Port không được để trống và phải hợp lệ!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                isConnected = true;
                AddMessageLeftRight($"Đã kết nối thành công tới Server {ip}:{port}", false);

                btnConnect.Enabled = false;
                txtIP.Enabled = false;
                txtPort.Enabled = false;
                txtName.Enabled = false;
                btnDisconnect.Enabled = true;

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                string? message;
                while (isConnected && reader != null && (message = reader.ReadLine()) != null)
                {
                    AddMessageLeftRight(message, false);
                }
            }
            catch
            {
                if (isConnected)
                {
                    AddMessageLeftRight("Mất kết nối với Server.", false);
                }
            }
            finally
            {
                Disconnect();
            }
        }

        private void btnSend_Click(object? sender, EventArgs e)
        {
            if (client != null && client.Connected && writer != null)
            {
                string msg = txtMessage.Text.Trim();
                if (!string.IsNullOrEmpty(msg))
                {
                    string displayName = string.IsNullOrEmpty(txtName.Text.Trim()) ? "Ẩn danh" : txtName.Text.Trim();
                    writer.WriteLine($"{displayName}: {msg}");
                    AddMessageLeftRight($"Bạn: {msg}", true);
                    txtMessage.Clear();
                }
            }
            else
            {
                MessageBox.Show("Chưa kết nối tới Server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDisconnect_Click(object? sender, EventArgs e)
        {
            if (isConnected)
            {
                AddMessageLeftRight("Bạn đã chủ động rời phòng chat.", false);
                Disconnect();
            }
        }

        private void AddMessageLeftRight(string message, bool isMe)
        {
            if (pnlChat.InvokeRequired)
            {
                pnlChat.Invoke(new Action(() => AddMessageLeftRight(message, isMe)));
            }
            else
            {
                Label lblMsg = new Label();
                lblMsg.Text = $"[{DateTime.Now.ToString("HH:mm:ss")}]\n{message}";
                lblMsg.AutoSize = true;
                lblMsg.MaximumSize = new Size(pnlChat.Width - 50, 0);
                lblMsg.Padding = new Padding(8);

                if (isMe)
                {
                    lblMsg.BackColor = Color.LightGreen;
                    lblMsg.ForeColor = Color.Black;
                }
                else
                {
                    lblMsg.BackColor = Color.LightGray;
                    lblMsg.ForeColor = Color.Black;
                }

                int nextY = 10;
                if (pnlChat.Controls.Count > 0)
                {
                    Control lastControl = pnlChat.Controls[pnlChat.Controls.Count - 1];
                    nextY = lastControl.Bottom + 10;
                }

                lblMsg.Location = new Point(0, nextY);
                pnlChat.Controls.Add(lblMsg);

                if (isMe)
                {
                    lblMsg.Location = new Point(pnlChat.Width - lblMsg.Width - 25, nextY);
                }
                else
                {
                    lblMsg.Location = new Point(10, nextY);
                }

                pnlChat.ScrollControlIntoView(lblMsg);
            }
        }

        private void Disconnect()
        {
            isConnected = false;
            reader?.Close();
            writer?.Close();
            client?.Close();

            if (btnConnect.InvokeRequired)
            {
                btnConnect.Invoke(new Action(() => {
                    btnConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    txtIP.Enabled = true;
                    txtPort.Enabled = true;
                    txtName.Enabled = true;
                }));
            }
            else
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                txtIP.Enabled = true;
                txtPort.Enabled = true;
                txtName.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            base.OnFormClosing(e);
        }
    }
}