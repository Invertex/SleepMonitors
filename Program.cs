using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Invertex.SleepMonitors
{
    class Program
    {
        private static int SC_MONITORPOWER = 0xF170;
        private static uint WM_SYSCOMMAND = 0x0112;

        [STAThread]
        static void Main(string[] args)
        {
            if (!IsSilent(args)) { DisplaySleepMessage(); }

            SendMessage((IntPtr)0xFFFF, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)2);
        }

        private static bool IsSilent(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "-s" || arg == "-silent") { return true; }
            }
            return false;
        }

        private static void DisplaySleepMessage()
        {
            var formy = new Form();
            formy.Text = "Putting Monitor to sleep...";
            formy.ControlBox = false;
            formy.Height = 36;
            formy.Width = 300;
            formy.Show();

            Thread.Sleep(1200);
            formy.Close();
            Thread.Sleep(200);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}