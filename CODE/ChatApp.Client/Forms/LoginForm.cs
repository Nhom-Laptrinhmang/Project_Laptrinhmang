using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient();

                client.Connect(
                    txtIP.Text,
                    int.Parse(txtPort.Text));

                ChatForm chat =
                    new ChatForm(client, txtUsername.Text);

                chat.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}