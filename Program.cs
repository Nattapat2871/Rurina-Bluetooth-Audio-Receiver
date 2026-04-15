using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RurinaAudio_Receiver
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        private const int WM_SHOWME = 0x0401; 

        [STAThread]
        static void Main()
        {
            try { SetCurrentProcessExplicitAppUserModelID("Nattapat2871.RurinaAudioReceiver"); } catch { }

            Application.ThreadException += (sender, args) => LogCrashAndExit(args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => LogCrashAndExit(args.ExceptionObject as Exception);

            bool createdNew;
            using (Mutex mutex = new Mutex(true, "RurinaAudio_Receiver_SingleInstance_Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    IntPtr hWnd = FindWindow(null, "Rurina Bluetooth Audio Receiver");
                    if (hWnd != IntPtr.Zero)
                    {
                        SendMessage(hWnd, WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                        SetForegroundWindow(hWnd);
                    }
                    return; 
                }

                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }

        private static void LogCrashAndExit(Exception? ex)
        {
            if (ex == null) return;
            try
            {
                Form crashForm = new Form
                {
                    Text = "Rurina Audio Receiver - Crash Report",
                    Size = new Size(550, 450),
                    StartPosition = FormStartPosition.CenterScreen,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    TopMost = true, 
                    BackColor = Color.FromArgb(12, 12, 12),
                    ForeColor = Color.White,
                    ShowIcon = false
                };

                TextBox txtLog = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Top,
                    Height = 350,
                    BackColor = Color.FromArgb(24, 24, 27),
                    ForeColor = Color.FromArgb(248, 113, 113),
                    Font = new Font("Consolas", 10F),
                    Text = $"[{DateTime.Now}] CRASH LOG:\r\n\r\n{ex.ToString()}",
                    BorderStyle = BorderStyle.None,
                    Margin = new Padding(10)
                };

                Button btnClose = new Button
                {
                    Text = "Close Application",
                    Dock = DockStyle.Bottom,
                    Height = 45,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(39, 39, 42),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) => crashForm.Close();
                crashForm.Controls.Add(txtLog);
                crashForm.Controls.Add(btnClose);
                crashForm.ShowDialog();
            }
            catch
            {
                
            }
            finally
            {
                Environment.Exit(1);
            }
        }
    }
}