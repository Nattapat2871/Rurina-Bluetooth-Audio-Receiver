using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RurinaAudio_Receiver
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        private const int SW_RESTORE = 9;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "RurinaAudio_Receiver_SingleInstance_Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    IntPtr hWnd = FindWindow(null, "Bluetooth Audio Receiver");

                    if (hWnd != IntPtr.Zero)
                    {
                        ShowWindow(hWnd, SW_RESTORE);
                        SetForegroundWindow(hWnd);
                    }
                    
                    return; 
                }

                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }
    }
}