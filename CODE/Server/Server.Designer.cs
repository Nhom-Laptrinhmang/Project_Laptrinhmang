using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class Sever
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMessage = new TextBox();
            btnStart = new Button();
            btnSend = new Button();
            txtPort = new TextBox();
            pnlChat = new Panel();
            SuspendLayout();
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(159, 304);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(257, 27);
            txtMessage.TabIndex = 1;
            txtMessage.TextChanged += txtMessage_TextChanged;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(159, 363);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(275, 29);
            btnStart.TabIndex = 2;
            btnStart.Text = "Start Server";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(481, 302);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(117, 29);
            btnSend.TabIndex = 3;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(473, 363);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(125, 27);
            txtPort.TabIndex = 4;
            // 
            // pnlChat
            // 
            pnlChat.AutoScroll = true;
            pnlChat.Location = new Point(159, 12);
            pnlChat.Name = "pnlChat";
            pnlChat.Size = new Size(439, 266);
            pnlChat.TabIndex = 5;
            // 
            // Sever
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            this.Controls.Add(pnlChat);
            this.Controls.Add(txtPort);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnStart);
            this.Controls.Add(txtMessage);
            this.Name = "Sever";
            this.Text = "Chat Server";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private TextBox txtMessage;
        private Button btnStart;
        private Button btnSend;
        private TextBox txtPort;
        private Panel pnlChat;
    }
}