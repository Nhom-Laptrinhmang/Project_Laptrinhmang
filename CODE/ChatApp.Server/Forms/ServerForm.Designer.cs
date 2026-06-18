// ChatApp.Server/Forms/ServerForm.Designer.cs
namespace ChatApp.Server.Forms
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;
        #region Controls
        // Server Config
        private System.Windows.Forms.GroupBox gbServerConfig;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        // Chat Area (Left)
        private System.Windows.Forms.TabControl tabChat;
        private System.Windows.Forms.TabPage tabPublicChat;
        private System.Windows.Forms.RichTextBox rtbPublicChat;
        private System.Windows.Forms.TabPage tabPrivateChat;
        private System.Windows.Forms.RichTextBox rtbPrivateChat;
        // Active Clients (Right)
        private System.Windows.Forms.GroupBox gbActiveClients;
        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colUsername;
        private System.Windows.Forms.ColumnHeader colIPAddress;
        private System.Windows.Forms.ColumnHeader colJoinTime;
        private System.Windows.Forms.Button btnKickSelected;
        private System.Windows.Forms.Button btnKickAll;
        // Bottom
        private System.Windows.Forms.PictureBox pbServerAvatar;
        private System.Windows.Forms.Label lblReplyingTo;
        private System.Windows.Forms.TextBox txtMessageInput;
        private System.Windows.Forms.Button btnEmoji;
        private System.Windows.Forms.Button btnSendToAll;
        private System.Windows.Forms.Button btnSendToSelected;
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // Server Configuration
            this.gbServerConfig = new System.Windows.Forms.GroupBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabChat = new System.Windows.Forms.TabControl();
            this.tabPublicChat = new System.Windows.Forms.TabPage();
            this.rtbPublicChat = new System.Windows.Forms.RichTextBox();
            this.tabPrivateChat = new System.Windows.Forms.TabPage();
            this.rtbPrivateChat = new System.Windows.Forms.RichTextBox();
            this.gbActiveClients = new System.Windows.Forms.GroupBox();
            this.lvClients = new System.Windows.Forms.ListView();
            this.colNo = new System.Windows.Forms.ColumnHeader();
            this.colUsername = new System.Windows.Forms.ColumnHeader();
            this.colIPAddress = new System.Windows.Forms.ColumnHeader();
            this.colJoinTime = new System.Windows.Forms.ColumnHeader();
            this.btnKickSelected = new System.Windows.Forms.Button();
            this.btnKickAll = new System.Windows.Forms.Button();
            this.pbServerAvatar = new System.Windows.Forms.PictureBox();
            this.lblReplyingTo = new System.Windows.Forms.Label();
            this.txtMessageInput = new System.Windows.Forms.TextBox();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.btnSendToAll = new System.Windows.Forms.Button();
            this.btnSendToSelected = new System.Windows.Forms.Button();
            this.gbServerConfig.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tabChat.SuspendLayout();
            this.tabPublicChat.SuspendLayout();
            this.tabPrivateChat.SuspendLayout();
            this.gbActiveClients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbServerAvatar)).BeginInit();
            this.SuspendLayout();
            // ================== SERVER CONFIG ==================
            this.gbServerConfig.Location = new System.Drawing.Point(12, 8);
            this.gbServerConfig.Size = new System.Drawing.Size(1160, 75);
            this.gbServerConfig.Text = "Server Configuration";
            this.lblIP.Location = new System.Drawing.Point(20, 28); this.lblIP.Text = "IP Address:";
            this.txtIP.Location = new System.Drawing.Point(110, 25); this.txtIP.Size = new System.Drawing.Size(150, 23); this.txtIP.ReadOnly = true;
            this.lblPort.Location = new System.Drawing.Point(280, 28); this.lblPort.Text = "Port:";
            this.txtPort.Location = new System.Drawing.Point(330, 25); this.txtPort.Size = new System.Drawing.Size(80, 23);
            this.btnStartServer.Location = new System.Drawing.Point(430, 20); this.btnStartServer.Size = new System.Drawing.Size(145, 40);
            this.btnStartServer.Text = "▶ START SERVER"; this.btnStartServer.BackColor = System.Drawing.Color.LimeGreen; this.btnStartServer.ForeColor = System.Drawing.Color.White;
            this.btnStopServer.Location = new System.Drawing.Point(585, 20); this.btnStopServer.Size = new System.Drawing.Size(145, 40);
            this.btnStopServer.Text = "■ STOP SERVER"; this.btnStopServer.BackColor = System.Drawing.Color.Red; this.btnStopServer.ForeColor = System.Drawing.Color.White; this.btnStopServer.Enabled = false;
            this.lblStatus.Location = new System.Drawing.Point(750, 28); this.lblStatus.Text = "OFFLINE"; this.lblStatus.ForeColor = System.Drawing.Color.Red; this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // ================== SPLIT CONTAINER ==================
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainerMain.Location = new System.Drawing.Point(12, 90);
            this.splitContainerMain.Size = new System.Drawing.Size(1160, 380);
            this.splitContainerMain.SplitterDistance = 720;
            // Tab Chat
            this.tabChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPublicChat.Text = "💬 Public Chat";
            this.rtbPublicChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPublicChat.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.rtbPublicChat.ForeColor = System.Drawing.Color.Lime;
            this.tabPublicChat.Controls.Add(this.rtbPublicChat);
            this.tabPrivateChat.Text = "🔒 Private Chat";
            this.rtbPrivateChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPrivateChat.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.rtbPrivateChat.ForeColor = System.Drawing.Color.Cyan;
            this.tabPrivateChat.Controls.Add(this.rtbPrivateChat);
            this.tabChat.Controls.Add(this.tabPublicChat);
            this.tabChat.Controls.Add(this.tabPrivateChat);
            this.splitContainerMain.Panel1.Controls.Add(this.tabChat);
            // Active Clients
            this.gbActiveClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbActiveClients.Text = "Active Clients (0)";
            this.lvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvClients.View = System.Windows.Forms.View.Details;
            this.lvClients.FullRowSelect = true;
            this.lvClients.MultiSelect = false;
            this.lvClients.Columns.AddRange(new[] { this.colNo, this.colUsername, this.colIPAddress, this.colJoinTime });
            this.colNo.Text = "No"; this.colNo.Width = 45;
            this.colUsername.Text = "Username"; this.colUsername.Width = 140;
            this.colIPAddress.Text = "IP Address"; this.colIPAddress.Width = 170;
            this.colJoinTime.Text = "Join Time"; this.colJoinTime.Width = 110;
            this.btnKickSelected.Location = new System.Drawing.Point(20, 335);
            this.btnKickSelected.Size = new System.Drawing.Size(200, 38);
            this.btnKickSelected.Text = "❌ KICK SELECTED CLIENT";
            this.btnKickAll.Location = new System.Drawing.Point(235, 335);
            this.btnKickAll.Size = new System.Drawing.Size(180, 38);
            this.btnKickAll.Text = "🚫 Kick All Clients";
            this.gbActiveClients.Controls.Add(this.lvClients);
            this.gbActiveClients.Controls.Add(this.btnKickSelected);
            this.gbActiveClients.Controls.Add(this.btnKickAll);
            this.splitContainerMain.Panel2.Controls.Add(this.gbActiveClients);
            // Bottom controls
            this.pbServerAvatar.Location = new System.Drawing.Point(20, 485); this.pbServerAvatar.Size = new System.Drawing.Size(80, 80);
            this.pbServerAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; this.pbServerAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.lblReplyingTo.Location = new System.Drawing.Point(120, 480); this.lblReplyingTo.Size = new System.Drawing.Size(750, 25); this.lblReplyingTo.Text = "Not replying to any message";
            this.txtMessageInput.Location = new System.Drawing.Point(120, 510); this.txtMessageInput.Size = new System.Drawing.Size(810, 65); this.txtMessageInput.Multiline = true;
            this.btnEmoji.Location = new System.Drawing.Point(950, 510); this.btnEmoji.Size = new System.Drawing.Size(60, 65); this.btnEmoji.Text = "😊";
            this.btnSendToAll.Location = new System.Drawing.Point(1020, 510); this.btnSendToAll.Size = new System.Drawing.Size(145, 35);
            this.btnSendToAll.Text = "🚀 SEND TO ALL"; this.btnSendToAll.BackColor = System.Drawing.Color.DodgerBlue; this.btnSendToAll.ForeColor = System.Drawing.Color.White;
            this.btnSendToSelected.Location = new System.Drawing.Point(1020, 550); this.btnSendToSelected.Size = new System.Drawing.Size(145, 35); this.btnSendToSelected.Text = "📤 Send to Selected";
            // Form settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(1190, 630);
            this.Text = "ChatApp Server - Control Panel";
            this.MinimumSize = new System.Drawing.Size(1150, 680);
            this.Controls.Add(this.pbServerAvatar);
            this.Controls.Add(this.lblReplyingTo);
            this.Controls.Add(this.txtMessageInput);
            this.Controls.Add(this.btnEmoji);
            this.Controls.Add(this.btnSendToAll);
            this.Controls.Add(this.btnSendToSelected);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.gbServerConfig);
            this.gbServerConfig.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.tabChat.ResumeLayout(false);
            this.tabPublicChat.ResumeLayout(false);
            this.tabPrivateChat.ResumeLayout(false);
            this.gbActiveClients.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbServerAvatar)).EndInit();
            this.ResumeLayout(false);
        }
    }
}