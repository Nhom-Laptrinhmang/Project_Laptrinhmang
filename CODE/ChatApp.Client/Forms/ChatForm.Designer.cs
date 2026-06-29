using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatApp.Client.Forms
{
    partial class ChatForm
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
            this.txtChatLog = new System.Windows.Forms.RichTextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.btnChooseAvatar = new System.Windows.Forms.Button();
            this.btnCancelReply = new System.Windows.Forms.Button();
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.lblReplyStatus = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();

            // 
            // txtChatLog
            // 
            this.txtChatLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChatLog.Location = new System.Drawing.Point(12, 12);
            this.txtChatLog.Name = "txtChatLog";
            this.txtChatLog.ReadOnly = true;
            this.txtChatLog.Size = new System.Drawing.Size(600, 350);
            this.txtChatLog.TabIndex = 0;
            this.txtChatLog.Text = "";
           

            // 
            // lstUsers
            // 
            this.lstUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.ItemHeight = 20;
            this.lstUsers.Location = new System.Drawing.Point(620, 12);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(180, 344);
            this.lstUsers.TabIndex = 1;

            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(12, 400);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(500, 27);
            this.txtInput.TabIndex = 2;

            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(520, 398);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(90, 30);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // 
            // btnEmoji
            // 
            this.btnEmoji.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEmoji.Location = new System.Drawing.Point(12, 435);
            this.btnEmoji.Name = "btnEmoji";
            this.btnEmoji.Size = new System.Drawing.Size(70, 30);
            this.btnEmoji.TabIndex = 4;
            this.btnEmoji.Text = "😊";
            this.btnEmoji.UseVisualStyleBackColor = true;
            this.btnEmoji.Click += new System.EventHandler(this.btnEmoji_Click);

            // 
            // btnChooseAvatar
            // 
            this.btnChooseAvatar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseAvatar.Location = new System.Drawing.Point(620, 370);
            this.btnChooseAvatar.Name = "btnChooseAvatar";
            this.btnChooseAvatar.Size = new System.Drawing.Size(180, 30);
            this.btnChooseAvatar.TabIndex = 5;
            this.btnChooseAvatar.Text = "Chọn Avatar";
            this.btnChooseAvatar.UseVisualStyleBackColor = true;
            this.btnChooseAvatar.Click += new System.EventHandler(this.btnChooseAvatar_Click);

            // 
            // picAvatar
            // 
            this.picAvatar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAvatar.Location = new System.Drawing.Point(620, 410);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(120, 120);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 6;
            this.picAvatar.TabStop = false;

            // 
            // lblReplyStatus
            // 
            this.lblReplyStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblReplyStatus.Location = new System.Drawing.Point(90, 440);
            this.lblReplyStatus.Name = "lblReplyStatus";
            this.lblReplyStatus.Size = new System.Drawing.Size(400, 25);
            this.lblReplyStatus.TabIndex = 7;
            this.lblReplyStatus.Visible = false;

            // 
            // btnCancelReply
            // 
            this.btnCancelReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelReply.Location = new System.Drawing.Point(500, 435);
            this.btnCancelReply.Name = "btnCancelReply";
            this.btnCancelReply.Size = new System.Drawing.Size(110, 30);
            this.btnCancelReply.TabIndex = 8;
            this.btnCancelReply.Text = "Hủy Reply";
            this.btnCancelReply.UseVisualStyleBackColor = true;
            this.btnCancelReply.Visible = false;
            this.btnCancelReply.Click += new System.EventHandler(this.btnCancelReply_Click);

            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 550);
            this.Controls.Add(this.btnCancelReply);
            this.Controls.Add(this.lblReplyStatus);
            this.Controls.Add(this.picAvatar);
            this.Controls.Add(this.btnChooseAvatar);
            this.Controls.Add(this.btnEmoji);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lstUsers);
            this.Controls.Add(this.txtChatLog);
            this.Name = "ChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat Application";

            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox txtChatLog;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnEmoji;
        private System.Windows.Forms.Button btnChooseAvatar;
        private System.Windows.Forms.Button btnCancelReply;
        private System.Windows.Forms.PictureBox picAvatar;
        private System.Windows.Forms.Label lblReplyStatus;
    }
}