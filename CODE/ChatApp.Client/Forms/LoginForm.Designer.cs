namespace ChatApp.Client.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

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
            txtUsername = new TextBox();
            txtIP = new TextBox();
            txtPort = new TextBox();
            btnConnect = new Button();

            SuspendLayout();

            txtUsername.Location = new Point(30, 30);
            txtUsername.PlaceholderText = "Username";

            txtIP.Location = new Point(30, 70);
            txtIP.Text = "";//127.0.0.1

            txtPort.Location = new Point(30, 110);
            txtPort.Text = "";

            btnConnect.Location = new Point(30, 150);
            btnConnect.Text = "Connect";
            btnConnect.Click += btnConnect_Click;

            Controls.Add(txtUsername);
            Controls.Add(txtIP);
            Controls.Add(txtPort);
            Controls.Add(btnConnect);

            ClientSize = new Size(300, 220);
            Text = "Login";

            ResumeLayout(false);
        }
    }
}