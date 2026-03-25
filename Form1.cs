using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Diagnostics; 
using System.Drawing;
using System.Drawing.Drawing2D; 
using System.IO; 
using System.Runtime.InteropServices; // เพิ่มสำหรับเรียกใช้ API บังคับ Title Bar สีดำ
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;

namespace RurinaAudio_Receiver
{
    public partial class Form1 : Form
    {
        // --- ส่วนของการเรียกใช้ Windows API เพื่อทำ Dark Title Bar ---
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        // --------------------------------------------------------

        private Label lblStatus = null!;
        private FlowLayoutPanel pnlDevices = null!; 
        private ModernButton btnOpen = null!; 
        private ModernButton btnClose = null!; 
        private Label lblTip = null!;
        private LinkLabel lnkGithub = null!; 
        private NotifyIcon trayIcon = null!;
        private ContextMenuStrip trayMenu = null!;

        private List<DeviceInformation> discoveredDevices = new List<DeviceInformation>();
        private DeviceInformation? selectedDevice = null;
        private AudioPlaybackConnection? audioConnection;

        private readonly string configPath = Path.Combine(Path.GetTempPath(), "Rurina-Bluetooth-Audio-Receiver_last_device.txt");
        private readonly string githubUrl = "https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver";

        public Form1()
        {
            InitializeUI();
        }

        // แทรกแซงกระบวนการสร้างหน้าต่าง เพื่อบังคับใช้ Dark Mode
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // โค้ด 20 คือ DWMWA_USE_IMMERSIVE_DARK_MODE สำหรับ Windows 11 (และ Win 10 เวอร์ชั่นใหม่ๆ)
            int[] useImmersiveDarkMode = { 1 };
            DwmSetWindowAttribute(this.Handle, 20, useImmersiveDarkMode, 4);
        }

        private void InitializeUI()
        {
            this.Text = "Rurina Bluetooth Audio Receiver";
            
            // ใช้ ClientSize กำหนดพื้นที่ด้านในจริงๆ ให้เล็กลงและพอดี
            this.ClientSize = new Size(350, 380); 
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // ปรับพื้นหลังให้ดำสนิทขึ้น (เกือบดำแท้)
            this.BackColor = Color.FromArgb(12, 12, 12); 
            this.ForeColor = Color.FromArgb(244, 244, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // เปิดให้แสดงไอคอนแอปด้านบน และดึงไอคอนจากไฟล์ exe มาใช้
            this.ShowIcon = true; 
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

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
                DisabledColor = Color.FromArgb(39, 39, 42), // เปลี่ยนสีตอน Disable ให้กลืนกับจอ
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

            lnkGithub = new LinkLabel
            {
                Text = "Open GitHub", 
                Location = new Point(15, 345), 
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
            trayIcon.Icon = this.Icon;
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
            this.Controls.Add(lnkGithub); 
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await FetchDevicesAndAutoConnectAsync();
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
                        
                        deviceBtn.Click += (s, ev) => 
                        {
                            SelectDeviceUI(device, deviceBtn);
                        };

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
            catch (Exception)
            {
                lblStatus.Text = "Error scanning devices";
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
            }
        }

        private async void BtnOpen_Click(object? sender, EventArgs e)
        {
            if (selectedDevice != null)
            {
                await ConnectDeviceAsync(selectedDevice);
            }
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
            catch (Exception)
            {
                lblStatus.Text = "Connection Error!";
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
                btnOpen.Enabled = true; 
                pnlDevices.Enabled = true; 
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            if (audioConnection != null)
            {
                audioConnection.Dispose();
                audioConnection = null;
                
                if (selectedDevice != null)
                {
                    lblStatus.Text = $"Disconnected from {selectedDevice.Name}";
                    lblStatus.ForeColor = Color.White;
                }

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
            try
            {
                Process.Start(new ProcessStartInfo(githubUrl) { UseShellExecute = true });
            }
            catch (Exception)
            {
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (audioConnection != null) audioConnection.Dispose();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            base.OnFormClosed(e);
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

        public ModernButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Transparent; 
            this.ForeColor = Color.White;
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { isHovering = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { isHovering = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; 

            g.Clear(this.Parent!.BackColor);

            Color bgColor = !this.Enabled ? DisabledColor : (isHovering ? HoverColor : NormalColor);

            using (GraphicsPath path = new GraphicsPath())
            {
                int r = BorderRadius;
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(Width - r, 0, r, r, 270, 90);
                path.AddArc(Width - r, Height - r, r, r, 0, 90);
                path.AddArc(0, Height - r, r, r, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
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
            else
            {
                using (Font textFont = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, Text, textFont, ClientRectangle, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }
    }
}