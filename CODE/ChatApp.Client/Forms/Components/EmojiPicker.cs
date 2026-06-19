using System;
using System.Windows.Forms;

namespace ChatApp.Client.Components
{
    public partial class EmojiPicker : UserControl
    {
        public event Action<string> EmojiSelected;

        private readonly string[] emojis = { 
            "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊", 
            "😋", "😎", "😍", "😘", "🥰", "😗", "😙", "😚", "🙂", "🤗", 
            "🤩", "🤔", "🤨", "😐", "😑", "😶", "🙄", "😏", "😣", "😥", 
            "😮", "🤐", "😯", "😪", "😫", "🥱", "😴", "😌", "😛", "😜" 
        };

        public EmojiPicker()
        {
            InitializeComponent();
            InitializeEmojiButtons();
        }

        private void InitializeEmojiButtons()
        {
            if (flpEmojis == null) return;

            flpEmojis.Controls.Clear();
            foreach (var emoji in emojis)
            {
                Button btn = new Button
                {
                    Text = emoji,
                    Width = 35,
                    Height = 35,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;

                btn.Click += (s, e) =>
                {
                    EmojiSelected?.Invoke(btn.Text);
                };

                flpEmojis.Controls.Add(btn);
            }
        }
    }
}
