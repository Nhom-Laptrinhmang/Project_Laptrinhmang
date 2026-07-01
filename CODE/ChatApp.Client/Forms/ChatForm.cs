using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ChatApp.Shared.Network;
using ChatApp.Client.Services;
using ChatApp.Client.Features;
using ChatApp.Client.Forms.Components;

namespace ChatApp.Client.Forms
{
    public partial class ChatForm : Form
    {
        private TcpClientService _clientService;
        private string _myUsername;
        private string _targetRecipient = "All";
        private string _myAvatarBase64 = "";

        // Lưu lịch sử tin nhắn: Key = MessageId, Value = Nội dung gốc
        private Dictionary<string, string> _messageHistory = new Dictionary<string, string>();
        private string _selectedReplyId = null;
        private string _selectedReplyContent = null;
        private string _lastSystemMessage = "";

        private EmojiPicker emojiPicker;
        private FlowLayoutPanel pnlChatBackground;

        // ====== DESIGN TOKENS ======
        private static readonly Color ColorBubbleMe = Color.FromArgb(0, 132, 255);
        private static readonly Color ColorBubbleOther = Color.FromArgb(241, 242, 245);
        private static readonly Color ColorBubbleSystem = Color.FromArgb(229, 231, 235);
        private static readonly Color ColorTextSystem = Color.FromArgb(110, 118, 128);
        private static readonly Color ColorChatBg = Color.FromArgb(250, 250, 251);
        private static readonly Color ColorSenderLabel = Color.FromArgb(0, 132, 255);

        private const int BubbleCornerRadius = 16;
        private const int AvatarSize = 36;
        private const int BubbleMaxTextWidth = 280;

        public ChatForm(string username)
        {
            InitializeComponent();
            _myUsername = username;
            _clientService = TcpClientService.Instance;

            this.Text = $"Messenger - Tài khoản: [{_myUsername}]";
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            _clientService.OnPacketReceived += ClientService_OnPacketReceived;
            _clientService.OnDisconnected += ClientService_OnDisconnected;

            this.Load += ChatForm_Load;
            txtInput.KeyDown += TxtInput_KeyDown;
            lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;

            SetupMessengerLayout();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            lstUsers.Items.Clear();
            lstUsers.Items.Add("All");
            lstUsers.SelectedIndex = 0;

            InitializeEmojiPickerComponent();
            StyleSidebarList();
            StyleInputArea();

            if (btnChooseAvatar != null)
            {
                btnChooseAvatar.FlatStyle = FlatStyle.Flat;
                btnChooseAvatar.FlatAppearance.BorderSize = 0;
                btnChooseAvatar.BackColor = Color.FromArgb(240, 242, 245);
                btnChooseAvatar.ForeColor = Color.FromArgb(50, 55, 60);
                btnChooseAvatar.Cursor = Cursors.Hand;
                ApplyRoundedCorners(btnChooseAvatar, 10);
            }

            // 1. Gửi gói tin Connect để báo danh
            var connectPacket = new MessagePacket
            {
                Type = Protocol.Connect,
                Sender = _myUsername,
                AvatarBase64 = _myAvatarBase64
            };
            _clientService.SendPacket(connectPacket);

            // ==========================================================
            // BỔ SUNG: YÊU CẦU LỊCH SỬ KHI VỪA VÀO PHÒNG
            // ==========================================================
            // Ghi chú: Chúng ta giả định Server sẽ tự động nhồi lịch sử lại cho bạn ngay sau khi xử lý gói Connect.
            // Đoạn này phụ thuộc vào việc bạn ĐÃ THÊM logic gửi lịch sử vào MessageRouter bên Server hay chưa. 
            // Nếu bạn làm đúng phần trước, Server sẽ TỰ GỬI hàng loạt gói Protocol.Message cũ về cho Client.
        }

        private void SetupMessengerLayout()
        {
            txtChatLog.Visible = false;

            pnlChatBackground = new FlowLayoutPanel
            {
                AutoScroll = true,
                BackColor = ColorChatBg,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15, 20, 15, 20)
            };

            typeof(Panel).InvokeMember("DoubleBuffered",
              System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
              null, pnlChatBackground, new object[] { true });

            this.Controls.Add(pnlChatBackground);

            if (picAvatar != null)
            {
                picAvatar.Size = new Size(140, 140);
                picAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
                picAvatar.BorderStyle = BorderStyle.None;
                picAvatar.BringToFront();

                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, 140, 140);
                picAvatar.Region = new Region(path);
            }

            this.Resize += (s, e) => RearrangeControls();
            RearrangeControls();

            pnlChatBackground.SizeChanged += (s, e) =>
            {
                pnlChatBackground.VerticalScroll.Value = pnlChatBackground.VerticalScroll.Maximum;
            };
        }

        private void RearrangeControls()
        {
            int margin = 15;
            int rightSidebarWidth = 220;
            int inputHeight = 40;
            int bottomY = this.ClientSize.Height - margin - inputHeight;

            this.SuspendLayout();
            if (pnlChatBackground != null) pnlChatBackground.SuspendLayout();

            // 1. NÚT EMOJI
            if (btnEmoji != null)
            {
                btnEmoji.Size = new Size(45, inputHeight);
                btnEmoji.Location = new Point(margin, bottomY);

                if (emojiPicker != null)
                {
                    emojiPicker.Location = new Point(margin, btnEmoji.Top - emojiPicker.Height - 5);
                }
            }

            // 2. NÚT GỬI
            if (btnSend != null)
            {
                btnSend.Size = new Size(90, inputHeight);
                btnSend.Location = new Point(this.ClientSize.Width - rightSidebarWidth - margin * 2 - btnSend.Width, bottomY);
            }

            // 3. Ô NHẬP LIỆU
            if (txtInput != null && btnEmoji != null && btnSend != null)
            {
                int txtX = btnEmoji.Right + 10;
                int txtWidth = btnSend.Left - 10 - txtX;

                txtInput.Size = new Size(txtWidth, inputHeight);
                txtInput.Location = new Point(txtX, bottomY);
            }

            // 4. LABLE TRẠNG THÁI REPLY
            int chatPanelBottomY = bottomY;
            if (lblReplyStatus != null && txtInput != null)
            {
                lblReplyStatus.Location = new Point(txtInput.Left, txtInput.Top - lblReplyStatus.Height - 8);

                if (btnCancelReply != null)
                {
                    btnCancelReply.Location = new Point(lblReplyStatus.Right + 5, lblReplyStatus.Top);
                }

                if (lblReplyStatus.Visible)
                {
                    chatPanelBottomY = lblReplyStatus.Top;
                }
                else
                {
                    chatPanelBottomY = txtInput.Top;
                }
            }
            else if (txtInput != null)
            {
                chatPanelBottomY = txtInput.Top;
            }

            // 5. CỘT BÊN PHẢI
            int rightPanelX = this.ClientSize.Width - rightSidebarWidth - margin;

            if (lstUsers != null)
            {
                lstUsers.Location = new Point(rightPanelX, margin);
                lstUsers.Size = new Size(rightSidebarWidth, 250);
            }

            if (btnChooseAvatar != null && lstUsers != null)
            {
                btnChooseAvatar.Location = new Point(rightPanelX, lstUsers.Bottom + margin);
                btnChooseAvatar.Size = new Size(rightSidebarWidth, 35);
            }

            if (picAvatar != null && btnChooseAvatar != null)
            {
                int centerAvatarX = rightPanelX + (rightSidebarWidth - picAvatar.Width) / 2;
                picAvatar.Location = new Point(centerAvatarX, btnChooseAvatar.Bottom + margin);
            }

            // 6. KHUNG NHẮN TIN
            if (pnlChatBackground != null)
            {
                pnlChatBackground.Location = new Point(margin, margin);
                pnlChatBackground.Size = new Size(this.ClientSize.Width - rightSidebarWidth - margin * 3, chatPanelBottomY - margin - 10);

                int bubbleWidth = pnlChatBackground.Width - pnlChatBackground.Padding.Horizontal - 20;
                foreach (Control bubble in pnlChatBackground.Controls)
                {
                    bubble.Width = bubbleWidth;
                }
            }

            if (pnlChatBackground != null) pnlChatBackground.ResumeLayout();
            this.ResumeLayout();
        }

        private void StyleSidebarList()
        {
            if (lstUsers == null) return;
            lstUsers.BorderStyle = BorderStyle.FixedSingle;
            lstUsers.BackColor = Color.White;
            lstUsers.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lstUsers.ItemHeight = 26;
        }

        private void StyleInputArea()
        {
            if (txtInput != null)
            {
                txtInput.BorderStyle = BorderStyle.FixedSingle;
                txtInput.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }

            if (btnSend != null)
            {
                btnSend.FlatStyle = FlatStyle.Flat;
                btnSend.FlatAppearance.BorderSize = 0;
                btnSend.BackColor = ColorBubbleMe;
                btnSend.ForeColor = Color.White;
                btnSend.Cursor = Cursors.Hand;
                ApplyRoundedCorners(btnSend, 10);
            }

            if (btnEmoji != null)
            {
                btnEmoji.FlatStyle = FlatStyle.Flat;
                btnEmoji.FlatAppearance.BorderSize = 0;
                btnEmoji.BackColor = Color.FromArgb(240, 242, 245);
                btnEmoji.Cursor = Cursors.Hand;
                ApplyRoundedCorners(btnEmoji, 10);
            }

            if (lblReplyStatus != null)
            {
                lblReplyStatus.ForeColor = Color.FromArgb(90, 95, 100);
                lblReplyStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            }
        }

        private void InitializeEmojiPickerComponent()
        {
            emojiPicker = new EmojiPicker { Location = new Point(12, 310), Size = new Size(220, 85), Visible = false };
            this.Controls.Add(emojiPicker);
            emojiPicker.BringToFront();

            emojiPicker.EmojiSelected += (emoji) =>
            {
                txtInput.AppendText(emoji);
                emojiPicker.Visible = false;
                txtInput.Focus();
            };
        }

        private void btnEmoji_Click(object sender, EventArgs e) => emojiPicker.Visible = !emojiPicker.Visible;

        private void btnChooseAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Ảnh thẻ|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picAvatar.Image = Image.FromFile(ofd.FileName);
                    _myAvatarBase64 = AvatarManager.ImageToBase64(ofd.FileName);
                    AddMessengerBubble("Hệ thống", "Bạn đã cập nhật ảnh đại diện thành công.", isMe: false, isSystem: true);
                }
            }
        }

        private void LstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null)
            {
                string selectedText = lstUsers.SelectedItem.ToString();
                _targetRecipient = selectedText.Replace(" (● Đang hoạt động)", "").Replace(" ( Đang hoạt động)", "").Trim();
            }
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(this, EventArgs.Empty);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtInput != null && !txtInput.Enabled) return;
            string messageText = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(messageText)) return;

            Protocol cmd = (_targetRecipient == "All") ? Protocol.Message : Protocol.PrivateMessage;
            string msgId = Guid.NewGuid().ToString();

            var packet = new MessagePacket
            {
                Type = cmd,
                Sender = _myUsername,
                Content = messageText,
                Receiver = _targetRecipient,
                AvatarBase64 = _myAvatarBase64,
                MessageId = msgId,
                ReplyToId = _selectedReplyId,
                ReplyToContent = _selectedReplyContent
            };

            _clientService.SendPacket(packet);
            _messageHistory[msgId] = messageText;

            string labelPrefix = (cmd == Protocol.PrivateMessage) ? $"[Tin nhắn được gửi cho {_targetRecipient}] " : "";
            string finalMsg = labelPrefix + messageText;

            if (!string.IsNullOrEmpty(_selectedReplyContent))
            {
                finalMsg = $"(Đang trả lời: \"{_selectedReplyContent}\")\n👉 {finalMsg}";
            }

            AddMessengerBubble("Tôi", finalMsg, isMe: true, messageId: msgId);

            btnCancelReply_Click(null, null);
            txtInput.Clear();
            txtInput.Focus();
        }

        // ==========================================================
        // HÀM XỬ LÝ CHUYỂN TIẾP (FORWARD)
        // ==========================================================
        private void ForwardMessage(string originalSender, string message, string target)
        {
            string fwMsgId = Guid.NewGuid().ToString();
            var forwardPacket = new MessagePacket
            {
                Type = Protocol.Forward,
                Sender = _myUsername,
                Content = $"[Bản gốc từ {originalSender}]: {message}",
                Receiver = target,
                AvatarBase64 = _myAvatarBase64,
                MessageId = fwMsgId
            };

            _clientService.SendPacket(forwardPacket);
            string displayTarget = (target == "All") ? "Cả phòng" : target;
            AddMessengerBubble("Hệ thống", $"Đã chuyển tiếp tới {displayTarget}.", isMe: false, isSystem: true);
            AddMessengerBubble("Tôi", $"[Chuyển tiếp] {message}", isMe: true, messageId: fwMsgId);
        }


        private void ClientService_OnPacketReceived(MessagePacket packet)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ClientService_OnPacketReceived(packet)));
                return;
            }

            if (packet == null) return;
            string protocolName = packet.Type.ToString().ToLower();

            // 1. Cập nhật TOÀN BỘ danh sách
            if (protocolName.Contains("user") || protocolName.Contains("list"))
            {
                UpdateUserListUI(packet.Content);
                return;
            }

            string senderName = !string.IsNullOrEmpty(packet.Sender) ? packet.Sender.Trim() : "";
            string displayContent = packet.Content != null ? packet.Content.Trim() : "";

            // 2. Thêm hoặc Xóa người dùng dựa vào gói tin chuẩn (Nếu server có hỗ trợ)
            if (packet.Type == Protocol.Connect && !string.IsNullOrEmpty(senderName))
            {
                AddUserToList(senderName);
            }
            else if (packet.Type == Protocol.Disconnect && !string.IsNullOrEmpty(senderName))
            {
                RemoveUserFromList(senderName);
            }

            // [QUAN TRỌNG] Bóc tách tên từ nội dung chat nếu Server chỉ gửi thông báo dạng chữ
            if (displayContent.StartsWith("Người dùng", StringComparison.OrdinalIgnoreCase))
            {
                if (displayContent.Contains("tham gia"))
                {
                    string newUser = displayContent.Replace("Người dùng", "")
                                                   .Replace("đã tham gia phòng chat!", "")
                                                   .Replace("đã tham gia", "")
                                                   .Trim();
                    AddUserToList(newUser);
                }
                else if (displayContent.Contains("thoát") || displayContent.Contains("rời"))
                {
                    string dropUser = displayContent.Replace("Người dùng", "")
                                                    .Replace("đã thoát khỏi phòng chat!", "")
                                                    .Replace("đã thoát", "")
                                                    .Replace("đã rời", "")
                                                    .Trim();
                    RemoveUserFromList(dropUser);
                }
            }

            // 3. Xử lý hiển thị nội dung chat lên màn hình
            if (!string.IsNullOrEmpty(displayContent))
            {
                bool isChatProtocol = packet.Type == Protocol.Message ||
                                      packet.Type == Protocol.PrivateMessage ||
                                      packet.Type == Protocol.Reply ||
                                      packet.Type == Protocol.Forward;

                if (string.IsNullOrEmpty(senderName))
                {
                    if (displayContent.Contains("tham gia") || displayContent.Contains("thoát") || packet.Type == Protocol.Connect || packet.Type == Protocol.Disconnect)
                        senderName = "Hệ thống";
                    else if (displayContent.Contains(":"))
                    {
                        int colonIndex = displayContent.IndexOf(":");
                        if (colonIndex > 0 && colonIndex < 30)
                        {
                            senderName = displayContent.Substring(0, colonIndex).Trim();
                            displayContent = displayContent.Substring(colonIndex + 1).Trim();
                        }
                    }
                    if (string.IsNullOrEmpty(senderName)) senderName = "Ai đó";
                }

                if (!string.IsNullOrEmpty(senderName) && senderName.Equals(_myUsername, StringComparison.OrdinalIgnoreCase)) return;
                if (string.IsNullOrEmpty(displayContent) && isChatProtocol) return;

                bool isSystemMsg = (senderName == "Hệ thống" || senderName == "Server" || packet.Type == Protocol.Connect || packet.Type == Protocol.Disconnect);

                // ==========================================
                // FIX LỖI NHÂN ĐÔI THÔNG BÁO HỆ THỐNG
                // ==========================================
                if (isSystemMsg)
                {
                    // Nếu tin nhắn hệ thống giống hệt tin nhắn vừa in ra ngay trước đó -> Bỏ qua
                    if (displayContent == _lastSystemMessage) return;
                    _lastSystemMessage = displayContent;
                }
                else
                {
                    _lastSystemMessage = ""; // Reset bộ nhớ nếu là tin nhắn người dùng bình thường
                }

                if (packet.Type == Protocol.PrivateMessage && !isSystemMsg) displayContent = $"[Tin mật] {displayContent}";
                else if (packet.Type == Protocol.Forward && !isSystemMsg) displayContent = $"[Chuyển tiếp] {displayContent}";

                if (!string.IsNullOrEmpty(packet.ReplyToContent)) displayContent = $"(Trả lời: \"{packet.ReplyToContent}\")\n👉 {displayContent}";

                AddMessengerBubble(senderName, displayContent, isMe: false, isSystem: isSystemMsg, avatar: AvatarManager.Base64ToImage(packet.AvatarBase64), messageId: packet.MessageId);
            }
        }

        private void AddUserToList(string username)
        {
            username = username.Trim();
            // Bỏ qua nếu tên rỗng, hoặc là chính mình, hoặc là hệ thống
            if (string.IsNullOrEmpty(username) || username == _myUsername || username.Contains("Hệ thống") || username.Contains("Server")) return;

            string displayItem = $"{username} (● Đang hoạt động)";

            // Đảm bảo không thêm trùng lặp
            if (!lstUsers.Items.Contains(displayItem))
            {
                lstUsers.Items.Add(displayItem);
            }
        }

        private void RemoveUserFromList(string username)
        {
            username = username.Trim();
            string displayItem = $"{username} (● Đang hoạt động)";

            if (lstUsers.Items.Contains(displayItem))
            {
                lstUsers.Items.Remove(displayItem);
            }

            // Nếu người bị xóa đang là mục tiêu nhắn tin riêng, trả về "All"
            if (_targetRecipient == username && lstUsers.Items.Count > 0)
            {
                lstUsers.SelectedIndex = 0;
            }
        }

        private void UpdateUserListUI(string userListContent)
        {
            if (string.IsNullOrEmpty(userListContent)) return;

            // Bắt đầu cập nhật giao diện để tránh giật
            lstUsers.BeginUpdate();

            string[] onlineUsers = userListContent.Split(new char[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string selectedUser = "All";
            if (lstUsers.SelectedItem != null)
            {
                selectedUser = lstUsers.SelectedItem.ToString().Replace(" (● Đang hoạt động)", "").Replace(" ( Đang hoạt động)", "").Trim();
            }

            lstUsers.Items.Clear();
            lstUsers.Items.Add("All");

            foreach (string user in onlineUsers)
            {
                string trimmedUser = user.Trim();
                // Không thêm chính bản thân mình vào danh sách bên phải
                if (trimmedUser != _myUsername)
                {
                    lstUsers.Items.Add($"{trimmedUser} (● Đang hoạt động)");
                }
            }

            // Phục hồi lại vị trí đang chọn
            int indexToSelect = 0;
            for (int i = 0; i < lstUsers.Items.Count; i++)
            {
                if (lstUsers.Items[i].ToString().StartsWith(selectedUser))
                {
                    indexToSelect = i;
                    break;
                }
            }
            if (lstUsers.Items.Count > indexToSelect) lstUsers.SelectedIndex = indexToSelect;

            lstUsers.EndUpdate();
        }

        private void AddMessengerBubble(string sender, string message, bool isMe, bool isSystem = false, Image avatar = null, string messageId = null)
        {
            string timeStr = DateTime.Now.ToString("HH:mm");

            Panel bubbleContainer = new Panel
            {
                Width = pnlChatBackground.Width - pnlChatBackground.Padding.Horizontal - 20,
                Margin = new Padding(0, 5, 0, 15),
                BackColor = Color.Transparent
            };

            Label lblTime = new Label
            {
                Text = timeStr,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                ForeColor = Color.DarkGray,
                AutoSize = true,
                Margin = new Padding(0)
            };

            lblTime.Location = new Point((bubbleContainer.Width - lblTime.PreferredWidth) / 2, 0);
            bubbleContainer.Controls.Add(lblTime);

            int startY = lblTime.Bottom + 5;

            PictureBox picUserAvatar = new PictureBox
            {
                Size = new Size(AvatarSize, AvatarSize),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            if (avatar != null) picUserAvatar.Image = avatar;
            else
            {
                string defaultAvatarBase64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAASklEQVRYR2NkYGD4z0AEYGTIY2QgATAqaBgFo2AUjIJRMApGwSgYBaNgFIyCgYvA////Gf7//88wXID9DAwMhIUhb6BsFAw7AAA68gYBAvN4twAAAABJRU5ErkJggg==";
                picUserAvatar.Image = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(defaultAvatarBase64)));
            }

            picUserAvatar.Paint += (s, e) =>
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, picUserAvatar.Width - 1, picUserAvatar.Height - 1);
                picUserAvatar.Region = new Region(path);
            };

            string senderLine = (!isSystem && !isMe) ? sender : null;
            Label lblSender = null;
            if (senderLine != null)
            {
                lblSender = new Label
                {
                    Text = senderLine,
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = ColorSenderLabel,
                    AutoSize = true,
                    Margin = new Padding(0)
                };
            }

            Label lblMessage = new Label
            {
                Text = message,
                Font = new Font("Segoe UI Emoji", 9.5F, FontStyle.Regular),
                MaximumSize = new Size(BubbleMaxTextWidth, 0),
                AutoSize = true,
                Padding = new Padding(13, 9, 13, 9),
                Tag = messageId,
                UseCompatibleTextRendering = true
            };

            ApplyRoundedCorners(lblMessage, BubbleCornerRadius);

            lblMessage.DoubleClick += (s, ev) =>
            {
                string clickedMsgId = lblMessage.Tag as string;
                if (!string.IsNullOrEmpty(clickedMsgId) && _messageHistory != null && _messageHistory.ContainsKey(clickedMsgId))
                {
                    _selectedReplyId = clickedMsgId;
                    _selectedReplyContent = _messageHistory[clickedMsgId];
                }
                else
                {
                    _selectedReplyId = clickedMsgId ?? Guid.NewGuid().ToString();
                    _selectedReplyContent = message.Contains("👉") ? message.Substring(message.IndexOf("👉") + 1).Trim() : message;
                }
                lblReplyStatus.Text = $"Đang phản hồi [{sender}]: \"{_selectedReplyContent}\"";
                lblReplyStatus.Visible = true;
                btnCancelReply.Visible = true;
                RearrangeControls();
            };

            // --- XỬ LÝ MENU CHUYỂN TIẾP (FORWARD) ---
            ContextMenuStrip msgMenu = new ContextMenuStrip();
            ToolStripMenuItem mnuForwardParent = new ToolStripMenuItem("Chuyển tiếp (Forward)");

            // 1. Chuyển tiếp công khai
            ToolStripMenuItem mnuForwardPublic = new ToolStripMenuItem("Công khai (All)");
            mnuForwardPublic.Click += (s, ev) => ForwardMessage(sender, message, "All");
            mnuForwardParent.DropDownItems.Add(mnuForwardPublic);

            // 2. Chuyển tiếp riêng tư (Menu con động)
            ToolStripMenuItem mnuForwardPrivate = new ToolStripMenuItem("Gửi riêng tư cho...");
            bool hasSomeoneElse = false;

            foreach (var item in lstUsers.Items)
            {
                string userEntry = item.ToString();
                if (userEntry == "All") continue;

                string userName = userEntry.Replace(" (● Đang hoạt động)", "").Trim();
                if (userName == _myUsername) continue;

                ToolStripMenuItem userItem = new ToolStripMenuItem(userName);
                userItem.Click += (s, ev) => ForwardMessage(sender, message, userName);
                mnuForwardPrivate.DropDownItems.Add(userItem);
                hasSomeoneElse = true;
            }

            if (hasSomeoneElse)
                mnuForwardParent.DropDownItems.Add(mnuForwardPrivate);
            else
                mnuForwardPrivate.Enabled = false;

            msgMenu.Items.Add(mnuForwardParent);
            lblMessage.ContextMenuStrip = msgMenu;

            // --- VẼ BONG BÓNG ---
            if (isSystem)
            {
                lblMessage.BackColor = ColorBubbleSystem;
                lblMessage.ForeColor = ColorTextSystem;
                lblMessage.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
                lblMessage.Location = new Point((bubbleContainer.Width - lblMessage.PreferredWidth) / 2, startY);
                bubbleContainer.Controls.Add(lblMessage);
                bubbleContainer.Height = lblMessage.Bottom + 5;
                lblTime.Visible = false;
            }
            else if (isMe)
            {
                lblMessage.BackColor = ColorBubbleMe;
                lblMessage.ForeColor = Color.White;
                lblMessage.Location = new Point(bubbleContainer.Width - lblMessage.PreferredWidth - 4, startY);
                bubbleContainer.Controls.Add(lblMessage);
                bubbleContainer.Height = lblMessage.Bottom + 5;
            }
            else
            {
                lblMessage.BackColor = ColorBubbleOther;
                lblMessage.ForeColor = Color.FromArgb(30, 30, 30);
                picUserAvatar.Location = new Point(0, startY);
                int textLeft = AvatarSize + 12;

                if (lblSender != null)
                {
                    lblSender.Location = new Point(textLeft + lblMessage.Padding.Left - 2, startY);
                    lblMessage.Location = new Point(textLeft, lblSender.Bottom + 2);
                    bubbleContainer.Controls.Add(lblSender);
                }
                else
                {
                    lblMessage.Location = new Point(textLeft, startY);
                }

                bubbleContainer.Controls.Add(picUserAvatar);
                bubbleContainer.Controls.Add(lblMessage);
                bubbleContainer.Height = Math.Max(lblMessage.Bottom, picUserAvatar.Bottom) + 5;
            }

            pnlChatBackground.Controls.Add(bubbleContainer);
            pnlChatBackground.PerformLayout();
            pnlChatBackground.ScrollControlIntoView(bubbleContainer);
            pnlChatBackground.AutoScrollPosition = new Point(0, pnlChatBackground.DisplayRectangle.Height);
        }

        private void ApplyRoundedCorners(Control ctrl, int radius)
        {
            void Recalculate(object s, EventArgs e)
            {
                if (ctrl.Width <= 0 || ctrl.Height <= 0) return;

                GraphicsPath path = new GraphicsPath();
                Rectangle bounds = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
                int d = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));

                path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
                path.AddArc(bounds.X + bounds.Width - d, bounds.Y, d, d, 270, 90);
                path.AddArc(bounds.X + bounds.Width - d, bounds.Y + bounds.Height - d, d, d, 0, 90);
                path.AddArc(bounds.X, bounds.Y + bounds.Height - d, d, d, 90, 90);
                path.CloseAllFigures();

                Region oldRegion = ctrl.Region;
                ctrl.Region = new Region(path);
                if (oldRegion != null) oldRegion.Dispose();
            }

            ctrl.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Recalculate(s, e);
            };
            ctrl.SizeChanged += Recalculate;
        }

        private void btnCancelReply_Click(object sender, EventArgs e)
        {
            _selectedReplyId = null;
            _selectedReplyContent = null;
            lblReplyStatus.Visible = false;
            btnCancelReply.Visible = false;
            RearrangeControls();
        }

        private void txtChatLog_DoubleClick(object sender, EventArgs e) { }

        private void ClientService_OnDisconnected()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ClientService_OnDisconnected));
                return;
            }

            AddMessengerBubble("Hệ thống", "Bạn đã bị ngắt kết nối hoặc trục xuất khỏi Server. Không thể gửi thêm tin.", isMe: false, isSystem: true);

            if (txtInput != null) txtInput.Enabled = false;
            if (btnSend != null) btnSend.Enabled = false;
            if (btnEmoji != null) btnEmoji.Enabled = false;
            if (lstUsers != null) lstUsers.Enabled = false;
            if (btnChooseAvatar != null) btnChooseAvatar.Enabled = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _clientService.Disconnect();
            Environment.Exit(0);
        }
    }
}