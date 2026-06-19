using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChatApp.Client.Components
{
    public partial class UserListPanel : UserControl
    {
        public event Action<string> UserSelected;

        public UserListPanel()
        {
            InitializeComponent();
        }

        public void UpdateUserList(List<string> onlineUsers, string currentUsername)
        {
            if (lstUsers == null) return;

            if (lstUsers.InvokeRequired)
            {
                lstUsers.Invoke(new Action(() => UpdateUserList(onlineUsers, currentUsername)));
                return;
            }

            lstUsers.Items.Clear();
            foreach (var user in onlineUsers)
            {
                if (user != currentUsername)
                {
                    lstUsers.Items.Add(user);
                }
            }
        }

        public string GetSelectedUser()
        {
            return lstUsers?.SelectedItem?.ToString() ?? string.Empty;
        }

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null)
            {
                UserSelected?.Invoke(lstUsers.SelectedItem.ToString());
            }
        }
    }
}
