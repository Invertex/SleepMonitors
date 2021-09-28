using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace Invertex.SleepMonitors
{
    class Program
    {
        private static readonly int SC_MONITORPOWER = 0xF170;
        private static readonly uint WM_SYSCOMMAND = 0x0112;

        [STAThread]
        static void Main(string[] args)
        {
            var opts = new Options(args);
            Application.Run(new UITimer(opts));
        }

        private class UITimer : Form
        {
            private readonly BackgroundWorker bgWorker;
            private Options options;
            public int TimeLeft
            {
                get => options.sleepDelayInSeconds; 
                set 
                {
                    options.sleepDelayInSeconds = value;
                    Text = $"Putting Monitors to sleep in {value}s...   (ESC) to cancel.";
                }
            }

            private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                TimeLeft = (int)e.UserState;

                if(TimeLeft <= 0)
                {
                    bgWorker.Dispose();
                    ExecuteSleep(Handle);

                    Application.Exit();
                }
            }

            private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
            {
                int timer = TimeLeft;

                bgWorker.ReportProgress(0, timer);

                while (timer > 0)
                {
                    Thread.Sleep(1000);
                    timer--;
                    bgWorker.ReportProgress(0, timer);
                }
            }

            public UITimer(Options options)
            {
                this.options = options;
                TimeLeft = options.sleepDelayInSeconds;

                ControlBox = false;
                Icon = Properties.Resources.screen_sleep_glossy;
                Height = 36;
                Width = 300;

                //Prevent form window from showing is isSilent. When Form is launched using Application.Run() it's forced visible so you need this.
                Visible = !options.isSilent;
                WindowState = options.isSilent ? FormWindowState.Minimized : FormWindowState.Normal;
                ShowInTaskbar = !options.isSilent;
                if(!options.isSilent) { CenterToScreen(); }

                KeyDown += (object _, KeyEventArgs e) =>
                {
                    if(e.KeyCode == Keys.Escape) { Text = "Cancelling..."; }
                    Application.Exit();
                };

                //Do timing on a background worker to avoid blocking Form UI thread.
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += BgWorker_DoWork;
                bgWorker.ProgressChanged += BgWorker_ProgressChanged;
                bgWorker.WorkerReportsProgress = true;
                bgWorker.RunWorkerAsync();
            }
        }

        public static void ExecuteSleep(IntPtr appHandle)
        {
            //Use our created Form handle to SendMessage to so that our program doesn't hang and we have full control over closing the thread.
            SendMessage(appHandle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)2);
        }

        public struct Options
        {
            public bool isSilent;
            public int sleepDelayInSeconds;

            public Options(string[] args)
            {
                isSilent = false;
                sleepDelayInSeconds = -1;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (arg == "-s" || arg == "-silent")
                    {
                        isSilent = true;
                        sleepDelayInSeconds = 0;
                    }
                    if (i < args.Length - 1 && arg == "-sleepDelaySeconds")
                    {
                        if (int.TryParse(args[i + 1], out int sleepDelay) && sleepDelay >= 0)
                        {
                            sleepDelayInSeconds = sleepDelay;
                        }
                    }
                }

                if (sleepDelayInSeconds < 0)
                {//No sleep delay was set, so set a default sleep delay
                    sleepDelayInSeconds = isSilent ? 2 : 5;
                }
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}