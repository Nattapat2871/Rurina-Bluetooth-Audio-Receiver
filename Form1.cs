using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Diagnostics; 
using System.Drawing;
using System.Drawing.Drawing2D; 
using System.IO; 
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32; 
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using System.Net.Http;       
using System.Text.Json;      

namespace RurinaAudio_Receiver
{
    public partial class Form1 : Form
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        private Label lblStatus = null!;
        private FlowLayoutPanel pnlDevices = null!; 
        private ModernButton btnOpen = null!; 
        private ModernButton btnClose = null!; 
        private Label lblTip = null!;
        private LinkLabel lnkGithub = null!; 
        private CheckBox chkRunOnStartup = null!; 
        private NotifyIcon trayIcon = null!;
        private ContextMenuStrip trayMenu = null!;

        private List<DeviceInformation> discoveredDevices = new List<DeviceInformation>();
        private DeviceInformation? selectedDevice = null;
        private AudioPlaybackConnection? audioConnection;

        // === ส่วนจัดการไฟล์ตั้งค่า (Configuration Group) ===
        private static readonly string AppDataFolder = Path.Combine(Path.GetTempPath(), "RurinaAudio");
        private readonly string configPath = Path.Combine(AppDataFolder, "last_device.txt");
        private readonly string startupPrefPath = Path.Combine(AppDataFolder, "startup_pref.txt");
        private readonly string skipUpdatePath = Path.Combine(AppDataFolder, "skip_version.txt");
        // ============================================
        
        private readonly string startupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly string appName = "RurinaAudioReceiver";
        private readonly string githubUrl = "https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver";

        private const int WM_SHOWME = 0x0401;

        public Form1()
        {
            EnsureConfigDirectory(); // ตรวจสอบและสร้างโฟลเดอร์เก็บไฟล์ก่อนเริ่ม UI
            InitializeUI();
        }

        private void EnsureConfigDirectory()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                {
                    Directory.CreateDirectory(AppDataFolder);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create config directory: {ex.Message}");
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int[] useImmersiveDarkMode = { 1 };
            DwmSetWindowAttribute(this.Handle, 20, useImmersiveDarkMode, 4);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SHOWME)
            {
                if (!this.Visible || this.WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate(); 
                }
            }
            base.WndProc(ref m);
        }

        private void InitializeUI()
        {
            this.Text = "Rurina Bluetooth Audio Receiver";
            this.ClientSize = new Size(350, 380); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(12, 12, 12); 
            this.ForeColor = Color.FromArgb(244, 244, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ShowIcon = true; 
            
            string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
            try { this.Icon = Icon.ExtractAssociatedIcon(exePath); } catch { }

            lblStatus = new Label 
            { 
                Text = "", 
                Location = new Point(0, 15), 
                Size = new Size(350, 25), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            pnlDevices = new FlowLayoutPanel
            {
                Location = new Point(25, 45),
                Size = new Size(300, 130), 
                AutoScroll = true,
                BackColor = Color.Transparent 
            };

            btnOpen = new ModernButton
            {
                Text = "Open Connection",
                Location = new Point(105, 190),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(59, 130, 246), 
                NormalColor = Color.FromArgb(59, 130, 246),
                HoverColor = Color.FromArgb(96, 165, 250), 
                DisabledColor = Color.FromArgb(39, 39, 42),
                Enabled = false 
            };
            btnOpen.Click += BtnOpen_Click;

            btnClose = new ModernButton
            {
                Text = "Close Connection",
                Location = new Point(105, 245),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(24, 24, 27), 
                NormalColor = Color.FromArgb(24, 24, 27),
                HoverColor = Color.FromArgb(239, 68, 68), 
                DisabledColor = Color.FromArgb(39, 39, 42),
                Enabled = false 
            };
            btnClose.Click += BtnClose_Click;

            lblTip = new Label
            {
                Text = "Tip: Close (X) to exit, Minimize (-) to hide in tray.",
                Location = new Point(0, 310), 
                Size = new Size(350, 30),
                ForeColor = Color.FromArgb(113, 113, 122), 
                TextAlign = ContentAlignment.MiddleCenter
            };

            chkRunOnStartup = new CheckBox
            {
                Text = "Run on startup",
                Location = new Point(225, 345), 
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(161, 161, 170),
                Cursor = Cursors.Hand
            };

            bool isStartup = true; 
            if (File.Exists(startupPrefPath))
            {
                isStartup = File.ReadAllText(startupPrefPath) == "1";
            }
            else
            {
                SetStartupRegistryState(true);
                File.WriteAllText(startupPrefPath, "1");
            }

            chkRunOnStartup.Checked = isStartup;
            chkRunOnStartup.CheckedChanged += ChkRunOnStartup_CheckedChanged;

            lnkGithub = new LinkLabel
            {
                Text = "Open GitHub", 
                Location = new Point(15, 348), 
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(161, 161, 170), 
                LinkColor = Color.FromArgb(161, 161, 170), 
                ActiveLinkColor = Color.White, 
                VisitedLinkColor = Color.FromArgb(161, 161, 170), 
            };
            lnkGithub.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkGithub.MouseEnter += (s, e) => lnkGithub.LinkColor = Color.White;
            lnkGithub.MouseLeave += (s, e) => lnkGithub.LinkColor = Color.FromArgb(161, 161, 170);
            lnkGithub.Click += LnkGithub_Click;

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => { ShowForm(); });
            trayMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });

            trayIcon = new NotifyIcon();
            trayIcon.Icon = this.Icon ?? SystemIcons.Application; 
            trayIcon.Text = "Rurina Bluetooth Audio Receiver";
            trayIcon.Visible = true;
            trayIcon.ContextMenuStrip = trayMenu; 
            trayIcon.DoubleClick += (s, e) => ShowForm(); 

            this.Resize += Form1_Resize;

            this.Controls.Add(lblStatus);
            this.Controls.Add(pnlDevices);
            this.Controls.Add(btnOpen);
            this.Controls.Add(btnClose);
            this.Controls.Add(lblTip);
            this.Controls.Add(chkRunOnStartup);
            this.Controls.Add(lnkGithub); 
        }

        private void SetStartupRegistryState(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
                            key.SetValue(appName, $"\"{exePath}\"");
                        }
                        else
                        {
                            key.DeleteValue(appName, false);
                        }
                    }
                }
            }
            catch { } 
        }

        private void ChkRunOnStartup_CheckedChanged(object? sender, EventArgs e)
        {
            bool isChecked = chkRunOnStartup.Checked;
            SetStartupRegistryState(isChecked);
            File.WriteAllText(startupPrefPath, isChecked ? "1" : "0");
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            bool isUpdating = await CheckForUpdatesAsync();
            if (!isUpdating)
            {
                await FetchDevicesAndAutoConnectAsync();
            }
        }

        private async Task<bool> CheckForUpdatesAsync()
        {
            string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
            string oldExePath = exePath + ".old";

            if (File.Exists(oldExePath))
            {
                try { File.Delete(oldExePath); } catch { }
            }

            try
            {
                lblStatus.Text = "Checking for updates...";
                lblStatus.ForeColor = Color.FromArgb(161, 161, 170);

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "RurinaAudioReceiver-AutoUpdate");
                string apiUrl = "https://api.github.com/repos/Nattapat2871/Rurina-Bluetooth-Audio-Receiver/releases/latest";
                
                string jsonResponse = await client.GetStringAsync(apiUrl);
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                
                string latestVersionStr = doc.RootElement.GetProperty("tag_name").GetString() ?? "";
                latestVersionStr = latestVersionStr.Replace("v", "").Trim();
                
                if (!Version.TryParse(latestVersionStr, out Version? latestVersion)) return false;

                string currentVerStr = FileVersionInfo.GetVersionInfo(exePath).FileVersion ?? "1.0.0.0";
                if (!Version.TryParse(currentVerStr, out Version? currentVersion)) return false;

                if (latestVersion > currentVersion)
                {
                    if (File.Exists(skipUpdatePath))
                    {
                        string skippedVersion = File.ReadAllText(skipUpdatePath).Trim();
                        if (skippedVersion == latestVersionStr) return false;
                    }

                    string downloadUrl = "";
                    foreach (var asset in doc.RootElement.GetProperty("assets").EnumerateArray())
                    {
                        string assetName = asset.GetProperty("name").GetString() ?? "";
                        if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? "";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        bool doUpdate = false;
                        bool doSkip = false;

                        using (var updateForm = new UpdateForm(currentVerStr, latestVersionStr))
                        {
                            var result = updateForm.ShowDialog(this);
                            if (result == DialogResult.Yes) doUpdate = true;
                            else if (result == DialogResult.Ignore) doSkip = true;
                        }

                        if (doSkip)
                        {
                            File.WriteAllText(skipUpdatePath, latestVersionStr);
                        }
                        else if (doUpdate)
                        {
                            await PerformUpdateAsync(downloadUrl, exePath, oldExePath);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private async Task PerformUpdateAsync(string downloadUrl, string currentExe, string oldExe)
        {
            try
            {
                lblStatus.Text = "Downloading update...";
                lblStatus.ForeColor = Color.FromArgb(96, 165, 250);
                pnlDevices.Enabled = false;
                btnOpen.Enabled = false;
                btnClose.Enabled = false;

                using HttpClient client = new HttpClient();
                byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl);

                if (File.Exists(oldExe)) File.Delete(oldExe);
                File.Move(currentExe, oldExe);
                File.WriteAllBytes(currentExe, fileBytes);

                Process.Start(new ProcessStartInfo { FileName = currentExe, UseShellExecute = true });
                Application.Exit();
            }
            catch (Exception)
            {
                lblStatus.Text = "Update failed!";
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
                try { if (!File.Exists(currentExe) && File.Exists(oldExe)) File.Move(oldExe, currentExe); } catch { }
                pnlDevices.Enabled = true;
                btnOpen.Enabled = selectedDevice != null;
            }
        }

        private void SelectDeviceUI(DeviceInformation device, ModernButton btn)
        {
            foreach (Control ctrl in pnlDevices.Controls)
            {
                if (ctrl is ModernButton b) 
                {
                    b.BackColor = Color.FromArgb(24, 24, 27); 
                    b.NormalColor = Color.FromArgb(24, 24, 27);
                    b.Invalidate(); 
                }
            }

            btn.BackColor = Color.FromArgb(59, 130, 246);
            btn.NormalColor = Color.FromArgb(59, 130, 246);
            btn.Invalidate(); 
            
            selectedDevice = device;
            lblStatus.Text = $"Selected: {device.Name}";
            lblStatus.ForeColor = Color.White;
            btnOpen.Enabled = true;
            btnClose.Enabled = false;
        }

        private async Task FetchDevicesAndAutoConnectAsync()
        {
            discoveredDevices.Clear();
            pnlDevices.Controls.Clear();
            btnOpen.Enabled = false;
            btnClose.Enabled = false;
            pnlDevices.Enabled = true; 
            
            lblStatus.Text = "Scanning devices...";
            lblStatus.ForeColor = Color.FromArgb(161, 161, 170);

            try
            {
                string selector = AudioPlaybackConnection.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(selector);

                string lastDeviceId = "";
                if (File.Exists(configPath))
                {
                    lastDeviceId = File.ReadAllText(configPath);
                }

                DeviceInformation? autoConnectDevice = null;
                ModernButton? autoConnectButton = null;

                foreach (var device in devices)
                {
                    if (!string.IsNullOrEmpty(device.Name))
                    {
                        discoveredDevices.Add(device);
                        
                        ModernButton deviceBtn = new ModernButton
                        {
                            Text = device.Name, 
                            IconText = "🎵", 
                            Size = new Size(270, 50), 
                            BackColor = Color.FromArgb(24, 24, 27), 
                            NormalColor = Color.FromArgb(24, 24, 27),
                            HoverColor = Color.FromArgb(39, 39, 42), 
                            Margin = new Padding(5)
                        };
                        
                        deviceBtn.Click += (s, ev) => SelectDeviceUI(device, deviceBtn);
                        pnlDevices.Controls.Add(deviceBtn);

                        if (device.Id == lastDeviceId)
                        {
                            autoConnectDevice = device;
                            autoConnectButton = deviceBtn;
                        }
                    }
                }

                if (discoveredDevices.Count == 0)
                {
                    lblStatus.Text = "No devices found";
                    lblStatus.ForeColor = Color.FromArgb(248, 113, 113); 
                }
                else
                {
                    lblStatus.Text = "Select a device to connect";
                    lblStatus.ForeColor = Color.White;
                    if (autoConnectDevice != null && autoConnectButton != null)
                    {
                        SelectDeviceUI(autoConnectDevice, autoConnectButton);
                        await ConnectDeviceAsync(autoConnectDevice);
                    }
                }
            }
            catch { lblStatus.Text = "Error scanning devices"; lblStatus.ForeColor = Color.FromArgb(248, 113, 113); }
        }

        private async void BtnOpen_Click(object? sender, EventArgs e)
        {
            if (selectedDevice != null) await ConnectDeviceAsync(selectedDevice);
        }

        private async Task ConnectDeviceAsync(DeviceInformation device)
        {
            lblStatus.Text = $"Connecting to {device.Name}...";
            lblStatus.ForeColor = Color.FromArgb(96, 165, 250); 
            btnOpen.Enabled = false; 
            pnlDevices.Enabled = false; 

            try
            {
                audioConnection = AudioPlaybackConnection.TryCreateFromId(device.Id);
                if (audioConnection != null)
                {
                    await audioConnection.StartAsync();
                    var result = await audioConnection.OpenAsync();

                    if (result.Status == AudioPlaybackConnectionOpenResultStatus.Success)
                    {
                        lblStatus.Text = $"Connected to {device.Name}";
                        lblStatus.ForeColor = Color.FromArgb(74, 222, 128); 
                        btnClose.Enabled = true;
                        File.WriteAllText(configPath, device.Id);
                    }
                    else
                    {
                        lblStatus.Text = $"Failed: {result.Status}";
                        lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
                        btnOpen.Enabled = true;
                        pnlDevices.Enabled = true; 
                    }
                }
            }
            catch { lblStatus.Text = "Connection Error!"; lblStatus.ForeColor = Color.FromArgb(248, 113, 113); btnOpen.Enabled = true; pnlDevices.Enabled = true; }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            if (audioConnection != null)
            {
                audioConnection.Dispose();
                audioConnection = null;
                if (selectedDevice != null) { lblStatus.Text = $"Disconnected from {selectedDevice.Name}"; lblStatus.ForeColor = Color.White; }
                btnClose.Enabled = false;
                btnOpen.Enabled = true;
                pnlDevices.Enabled = true; 
            }
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                trayIcon.ShowBalloonTip(2000, "Audio Receiver", "App is running in the background.", ToolTipIcon.Info);
            }
        }

        private void LnkGithub_Click(object? sender, EventArgs e)
        {
            try { Process.Start(new ProcessStartInfo(githubUrl) { UseShellExecute = true }); } catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (audioConnection != null) audioConnection.Dispose();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            base.OnFormClosed(e);
        }
    }

    public class UpdateForm : Form
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public UpdateForm(string currentVer, string newVer)
        {
            this.Text = "New Update Available!";
            this.Size = new Size(360, 210);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(12, 12, 12);
            this.ForeColor = Color.White;
            this.ShowIcon = false;

            Label lblInfo = new Label
            {
                Text = $"A new version of Rurina is available!\n\nCurrent Version: {currentVer}\nLatest Version:   {newVer}",
                Location = new Point(20, 20),
                Size = new Size(300, 60),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            ModernButton btnUpdate = new ModernButton
            {
                Text = "Update Now",
                Location = new Point(25, 100),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(59, 130, 246),
                NormalColor = Color.FromArgb(59, 130, 246),
                HoverColor = Color.FromArgb(96, 165, 250),
                DialogResult = DialogResult.Yes
            };

            ModernButton btnSkip = new ModernButton
            {
                Text = "Skip Version",
                Location = new Point(175, 100),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(39, 39, 42),
                NormalColor = Color.FromArgb(39, 39, 42),
                HoverColor = Color.FromArgb(24, 24, 27),
                DialogResult = DialogResult.Ignore
            };

            this.Controls.Add(lblInfo);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnSkip);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int[] useImmersiveDarkMode = { 1 };
            try { DwmSetWindowAttribute(this.Handle, 20, useImmersiveDarkMode, 4); } catch { }
        }
    }

    public class ModernButton : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius { get; set; } = 8; 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color NormalColor { get; set; } = Color.FromArgb(45, 45, 45);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor { get; set; } = Color.FromArgb(60, 60, 60);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color DisabledColor { get; set; } = Color.FromArgb(63, 63, 70);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string IconText { get; set; } = ""; 
        private bool isHovering = false;
        public ModernButton() { this.FlatStyle = FlatStyle.Flat; this.FlatAppearance.BorderSize = 0; this.BackColor = Color.Transparent; this.ForeColor = Color.White; this.Cursor = Cursors.Hand; }
        protected override void OnMouseEnter(EventArgs e) { isHovering = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { isHovering = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.Clear(this.Parent!.BackColor);
            Color bgColor = !this.Enabled ? DisabledColor : (isHovering ? HoverColor : NormalColor);
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = BorderRadius; path.AddArc(0, 0, r, r, 180, 90); path.AddArc(Width - r, 0, r, r, 270, 90); path.AddArc(Width - r, Height - r, r, r, 0, 90); path.AddArc(0, Height - r, r, r, 90, 90); path.CloseFigure();
                using (SolidBrush brush = new SolidBrush(bgColor)) g.FillPath(brush, path);
            }
            Color textColor = !this.Enabled ? Color.FromArgb(113, 113, 122) : this.ForeColor;
            if (!string.IsNullOrEmpty(IconText))
            {
                using (Font iconFont = new Font("Segoe UI Emoji", 16, FontStyle.Regular))
                using (Font textFont = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    Size iconSize = TextRenderer.MeasureText(g, IconText, iconFont);
                    TextRenderer.DrawText(g, IconText, iconFont, new Point(12, (Height - iconSize.Height) / 2), textColor);
                    Rectangle textBounds = new Rectangle(20 + iconSize.Width, 0, Width - (30 + iconSize.Width), Height);
                    TextRenderer.DrawText(g, Text, textFont, textBounds, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.WordEllipsis);
                }
            }
            else { using (Font textFont = new Font("Segoe UI", 10, FontStyle.Bold)) TextRenderer.DrawText(g, Text, textFont, ClientRectangle, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter); }
        }
    }
}