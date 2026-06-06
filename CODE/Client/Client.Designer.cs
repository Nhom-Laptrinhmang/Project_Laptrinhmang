using System.Drawing;
using System.Windows.Forms;

namespace ChatCilent
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtMessage = new TextBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            btnSend = new Button();
            txtIP = new TextBox();
            txtPort = new TextBox();
            txtName = new TextBox();
            pnlChat = new Panel();
            SuspendLayout();
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(86, 255);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(611, 27);
            txtMessage.TabIndex = 1;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(575, 308);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(122, 29);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(575, 361);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(122, 29);
            btnDisconnect.TabIndex = 4;
            btnDisconnect.Text = "Thoát";
            btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(444, 308);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(94, 29);
            btnSend.TabIndex = 2;
            btnSend.Text = "Gửi";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(138, 361);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(125, 27);
            txtIP.TabIndex = 5;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(300, 361);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(100, 27);
            txtPort.TabIndex = 6;
            // 
            // txtName
            // 
            txtName.Location = new Point(86, 308);
            txtName.Name = "txtName";
            txtName.Size = new Size(257, 27);
            txtName.TabIndex = 0;
            // 
            // pnlChat
            // 
            pnlChat.AutoScroll = true;
            pnlChat.BorderStyle = BorderStyle.FixedSingle;
            pnlChat.Location = new Point(86, 12);
            pnlChat.Name = "pnlChat";
            pnlChat.Size = new Size(611, 237);
            pnlChat.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            this.Controls.Add(pnlChat);
            this.Controls.Add(txtName);
            this.Controls.Add(txtPort);
            this.Controls.Add(txtIP);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnDisconnect);
            this.Controls.Add(btnConnect);
            this.Controls.Add(txtMessage);
            this.Name = "Form1";
            this.Text = "Chat Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtMessage;
        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnSend;
        private TextBox txtPort;
        private TextBox txtIP;
        private TextBox txtName;
        private Panel pnlChat;
    }
}