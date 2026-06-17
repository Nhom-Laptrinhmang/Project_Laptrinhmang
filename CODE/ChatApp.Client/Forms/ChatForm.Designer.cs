using static System.Net.Mime.MediaTypeNames;

namespace Client.Forms
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        private ListBox lstMessages;
        private TextBox txtMessage;
        private Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lstMessages = new ListBox();
            txtMessage = new TextBox();
            btnSend = new Button();

            SuspendLayout();

            lstMessages.Location =
                new Point(20, 20);
            lstMessages.Size =
                new Size(450, 250);

            txtMessage.Location =
                new Point(20, 290);
            txtMessage.Size =
                new Size(350, 27);

            btnSend.Location =
                new Point(390, 290);
            btnSend.Text = "Send";
            btnSend.Click += btnSend_Click;

            Controls.Add(lstMessages);
            Controls.Add(txtMessage);
            Controls.Add(btnSend);

            ClientSize =
                new Size(500, 350);

            Text = "Chat";

            ResumeLayout(false);
        }
    }
}