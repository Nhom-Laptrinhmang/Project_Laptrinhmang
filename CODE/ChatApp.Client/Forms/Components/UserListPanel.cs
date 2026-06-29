using System;
using System.Windows.Forms;

namespace ChatApp.Client.Forms.Components
{
    public class UserListPanel : UserControl
    {
        private ListBox lstUsers;
        public event Action<string> OnUserSelected;

        public UserListPanel()
        {
            lstUsers = new ListBox { Dock = DockStyle.Fill };
            lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;
            this.Controls.Add(lstUsers);

            // Khởi tạo giá trị mặc định ban đầu
            ResetList();
        }

        public void ResetList()
        {
            lstUsers.Items.Clear();
            lstUsers.Items.Add("All");
            lstUsers.SelectedIndex = 0;
        }

        public void AddUser(string username)
        {
            if (!lstUsers.Items.Contains(username))
            {
                lstUsers.Items.Add(username);
            }
        }

        public void RemoveUser(string username)
        {
            if (lstUsers.Items.Contains(username))
            {
                lstUsers.Items.Remove(username);
                if (lstUsers.SelectedIndex == -1) lstUsers.SelectedIndex = 0;
            }
        }

        private void LstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null)
            {
                OnUserSelected?.Invoke(lstUsers.SelectedItem.ToString());
            }
        }
    }
}
