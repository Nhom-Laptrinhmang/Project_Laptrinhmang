using System;
using System.Windows.Forms;
using ChatApp.Server.Forms;

namespace ChatApp.Server
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi chạy màn hình giao diện Server chính
            Application.Run(new ServerForm());
        }
    }
}
