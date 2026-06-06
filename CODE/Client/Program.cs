using System;
using System.Windows.Forms;

namespace ChatCilent
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Gọi đúng Form1 nằm trong namespace ChatCilent
            Application.Run(new Form1());
        }
    }
}