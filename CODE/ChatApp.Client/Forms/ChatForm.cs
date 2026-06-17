using Client.Services;
using System;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class ChatForm : Form
    {
        private readonly TcpClient _client;
        private readonly StreamWriter _writer;
        private readonly SocketListener _listener;

        private readonly string _username;

        public ChatForm(
            TcpClient client,
            string username)
        {
            InitializeComponent();

            _client = client;
            _username = username;

            _writer =
                new StreamWriter(_client.GetStream())
                {
                    AutoFlush = true
                };

            _listener =
                new SocketListener(_client);

            _listener.MessageReceived += OnMessageReceived;
            _listener.Start();
        }

        private void OnMessageReceived(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    lstMessages.Items.Add(msg);
                }));
            }
            else
            {
                lstMessages.Items.Add(msg);
            }
        }

        private void btnSend_Click(
            object sender,
            EventArgs e)
        {
            string msg =
                $"{_username}: {txtMessage.Text}";

            _writer.WriteLine(msg);

            txtMessage.Clear();
        }
    }
}