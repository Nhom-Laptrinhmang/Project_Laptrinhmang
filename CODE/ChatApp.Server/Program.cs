// ChatApp.Server/Program.cs
using System;
using System.Windows.Forms;
using ChatApp.Server.Forms;   // ← Quan trọng: using này

namespace ChatApp.Server
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());   // ← Phải match tên class + namespace
        }
    }
}