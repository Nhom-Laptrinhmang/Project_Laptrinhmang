using System;
using System.Windows.Forms;

namespace ChatApp.Client.Forms.Components
{
    public class EmojiPicker : UserControl
    {
        public event Action<string> EmojiSelected;
        private FlowLayoutPanel flowLayout;

        // Tập hợp danh sách Emoji Real-time phổ biến
        private readonly string[] emojis = { "😊", "❤", "💚", "👽", "🐸", "👍", "🔥", "🎉", "😂", "😮" };

        public EmojiPicker()
        {
            flowLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            this.Controls.Add(flowLayout);
            InitializePicker();
        }

        private void InitializePicker()
        {
            flowLayout.Controls.Clear();
            foreach (var emoji in emojis)
            {
                Button btn = new Button
                {
                    Text = emoji,
                    Size = new System.Drawing.Size(32, 32),
                    Font = new System.Drawing.Font("Segoe UI Emoji", 12F),
                    FlatStyle = FlatStyle.Flat
                };
                btn.Click += (s, e) => EmojiSelected?.Invoke(btn.Text);
                flowLayout.Controls.Add(btn);
            }
        }
    }
}
