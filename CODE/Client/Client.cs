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
        // Khai báo các biến thông thường (Không dùng dấu ? để đỡ rối mắt)
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread receiveThread;
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

        // 1. Hàm xử lý khi bấm nút KẾT NỐI
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = txtIP.Text.Trim();
                int port;

                // Kiểm tra xem người dùng nhập Port có đúng là số không
                if (int.TryParse(txtPort.Text.Trim(), out port) == false || string.IsNullOrEmpty(ip))
                {
                    MessageBox.Show("IP và Port không được để trống!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Khởi tạo kết nối Socket
                client = new TcpClient(ip, port);

                // Lấy luồng đọc/ghi dữ liệu từ Socket ra
                NetworkStream stream = client.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8);
                writer.AutoFlush = true; // Gửi tin nhắn đi ngay lập tức không cần đợi bộ đệm đầy

                isConnected = true;
                AddMessageLeftRight("Đã kết nối thành công tới Server " + ip + ":" + port, false);

                // Khóa các ô nhập liệu lại không cho người dùng sửa khi đang chat
                btnConnect.Enabled = false;
                txtIP.Enabled = false;
                txtPort.Enabled = false;
                txtName.Enabled = false;
                btnDisconnect.Enabled = true; // Mở khóa nút Rời phòng

                // Tạo một luồng (Thread) chạy ngầm để liên tục ngồi đợi nghe tin nhắn từ Server về
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Hàm CHẠY NGẦM ĐỂ NHẬN TIN NHẮN liên tục từ Server
        private void ReceiveMessages()
        {
            try
            {
                string message;
                // Vòng lặp vô tận: Cứ có dòng tin nhắn nào từ Server gửi tới là đọc ra ngay
                while (isConnected == true && reader != null)
                {
                    message = reader.ReadLine();
                    if (message != null)
                    {
                        AddMessageLeftRight(message, false); // Hiện tin nhắn của người khác bên lề trái
                    }
                }
            }
            catch
            {
                if (isConnected == true)
                {
                    AddMessageLeftRight("Mất kết nối với Server.", false);
                }
            }
            finally
            {
                Disconnect(); // Nếu có lỗi xảy ra hoặc ngắt kết nối thì dọn dẹp tài nguyên
            }
        }

        // 3. Hàm xử lý khi bấm nút GỬI TIN NHẮN
        private void btnSend_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem biến client và writer đã được khởi tạo và kết nối chưa
            if (client != null && client.Connected == true && writer != null)
            {
                string msg = txtMessage.Text.Trim();
                if (string.IsNullOrEmpty(msg) == false)
                {
                    // Lấy tên hiển thị, nếu không nhập thì để là "Ẩn danh"
                    string displayName = txtName.Text.Trim();
                    if (string.IsNullOrEmpty(displayName) == true)
                    {
                        displayName = "Ẩn danh";
                    }

                    // Gửi tin nhắn lên Server theo định dạng "Tên: Nội dung"
                    writer.WriteLine(displayName + ": " + msg);

                    // Tự hiển thị tin nhắn của chính mình bên lề phải
                    AddMessageLeftRight("Bạn: " + msg, true);
                    txtMessage.Clear(); // Xóa trống ô nhập liệu để chuẩn bị gõ tin tiếp theo
                }
            }
            else
            {
                MessageBox.Show("Chưa kết nối tới Server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 4. Hàm xử lý khi bấm nút RỜI PHÒNG (Disconnect)
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (isConnected == true)
            {
                AddMessageLeftRight("Bạn đã chủ động rời phòng chat.", false);
                Disconnect(); // Thực hiện ngắt kết nối
            }
        }

        // 5. Hàm HIỂN THỊ TIN NHẮN lên giao diện (Chia lề Trái/Phải)
        private void AddMessageLeftRight(string message, bool isMe)
        {
            // Vì luồng chạy ngầm (ReceiveMessages) không thể trực tiếp can thiệp vào giao diện,
            // nên ta cần dùng Invoke để ủy quyền cho luồng chính (UI Thread) vẽ tin nhắn.
            if (pnlChat.InvokeRequired == true)
            {
                pnlChat.Invoke(new Action(() => AddMessageLeftRight(message, isMe)));
            }
            else
            {
                // Tạo một cái nhãn (Label) chứa tin nhắn một cách thủ công
                Label lblMsg = new Label();
                lblMsg.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "]\n" + message;
                lblMsg.AutoSize = true;
                lblMsg.MaximumSize = new Size(pnlChat.Width - 50, 0);
                lblMsg.Padding = new Padding(8);

                // Đổi màu nền để phân biệt Ta và Người khác
                if (isMe == true)
                {
                    lblMsg.BackColor = Color.LightGreen; // Mình nói -> Màu xanh lá
                    lblMsg.ForeColor = Color.Black;
                }
                else
                {
                    lblMsg.BackColor = Color.LightGray; // Người khác nói -> Màu xám
                    lblMsg.ForeColor = Color.Black;
                }

                // Tính toán vị trí Y (chiều dọc) để tin nhắn sau nằm dưới tin nhắn trước
                int nextY = 10;
                if (pnlChat.Controls.Count > 0)
                {
                    Control lastControl = pnlChat.Controls[pnlChat.Controls.Count - 1];
                    nextY = lastControl.Bottom + 10;
                }

                lblMsg.Location = new Point(0, nextY);
                pnlChat.Controls.Add(lblMsg);

                // Căn lề X (chiều ngang): Mình nói nằm bên Phải, người khác nằm bên Trái
                if (isMe == true)
                {
                    lblMsg.Location = new Point(pnlChat.Width - lblMsg.Width - 25, nextY);
                }
                else
                {
                    lblMsg.Location = new Point(10, nextY);
                }

                // Tự động cuộn khung chat xuống dưới cùng để thấy tin nhắn mới nhất
                pnlChat.ScrollControlIntoView(lblMsg);
            }
        }

        // 6. Hàm DỌN DẸP, đóng kết nối mạng và mở khóa lại giao diện
        private void Disconnect()
        {
            isConnected = false;

            // Dùng câu lệnh if truyền thống để đóng các kết nối một cách an toàn
            if (reader != null)
            {
                reader.Close();
            }
            if (writer != null)
            {
                writer.Close();
            }
            if (client != null)
            {
                client.Close();
            }

            // Trả lại trạng thái ban đầu cho các nút bấm và ô nhập liệu trên giao diện
            if (btnConnect.InvokeRequired == true)
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

        // 7. Sự kiện phòng hờ: Nếu người dùng bấm dấu X tắt hẳn App thì tự ngắt kết nối luôn
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            base.OnFormClosing(e);
        }
    }
}