using System;
using System.Collections.Generic;
using System.Diagnostics; 
using System.Drawing;
using System.IO; 
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;

namespace RurinaAudio_Receiver
{
    public partial class Form1 : Form
    {
        private Label lblStatus = null!;
        private FlowLayoutPanel pnlDevices = null!; 
        private Button btnOpen = null!;
        private Button btnClose = null!;
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

        private void InitializeUI()
        {
            this.Text = "Rurina Bluetooth Audio Receiver";
            this.Size = new Size(350, 400); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black; 
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ShowIcon = false; 

            lblStatus = new Label 
            { 
                Text = "", 
                Location = new Point(0, 20), 
                Size = new Size(330, 20), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            pnlDevices = new FlowLayoutPanel
            {
                Location = new Point(30, 50),
                Size = new Size(280, 100), 
                AutoScroll = true,
                BackColor = Color.Black
            };

            System.Windows.Forms.Panel pnlControls = new System.Windows.Forms.Panel
            {
                Location = new Point(100, 170), 
                Size = new Size(130, 80),
                BackColor = Color.FromArgb(45, 45, 45) 
            };

            btnOpen = new Button
            {
                Text = "Open Connection",
                Location = new Point(0, 0),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray, 
                Enabled = false 
            };
            btnOpen.FlatAppearance.BorderSize = 0;
            btnOpen.Click += BtnOpen_Click;

            btnClose = new Button
            {
                Text = "Close Connection",
                Location = new Point(0, 40),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray, 
                Enabled = false 
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;

            pnlControls.Controls.Add(btnOpen);
            pnlControls.Controls.Add(btnClose);

            lblTip = new Label
            {
                Text = "Tip: Close (X) to exit, Minimize (-) to hide in tray.",
                Location = new Point(0, 290), 
                Size = new Size(330, 40),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lnkGithub = new LinkLabel
            {
                Text = "Open GitHub", 
                Location = new Point(15, 330), 
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray, 
                LinkColor = Color.Gray, 
                ActiveLinkColor = Color.White, 
                VisitedLinkColor = Color.Gray, 
            };
            lnkGithub.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkGithub.MouseEnter += (s, e) => lnkGithub.LinkColor = Color.White;
            lnkGithub.MouseLeave += (s, e) => lnkGithub.LinkColor = Color.Gray;
            lnkGithub.Click += LnkGithub_Click;

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => { ShowForm(); });
            trayMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });

            trayIcon = new NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            trayIcon.Text = "Rurina Bluetooth Audio Receiver";
            trayIcon.Visible = true;
            trayIcon.ContextMenuStrip = trayMenu; 
            trayIcon.DoubleClick += (s, e) => ShowForm(); 

            this.Resize += Form1_Resize;

            this.Controls.Add(lblStatus);
            this.Controls.Add(pnlDevices);
            this.Controls.Add(pnlControls);
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

        private void SelectDeviceUI(DeviceInformation device, Button btn)
        {
            foreach (Control ctrl in pnlDevices.Controls)
            {
                if (ctrl is Button b) 
                {
                    b.BackColor = Color.FromArgb(45, 45, 45);
                    b.Invalidate(); 
                }
            }

            btn.BackColor = Color.FromArgb(0, 80, 150);
            btn.Invalidate(); 
            
            selectedDevice = device;
            lblStatus.Text = $"Selected: {device.Name}";
            lblStatus.ForeColor = Color.White;
            
            btnOpen.Enabled = true;
            btnOpen.ForeColor = Color.White;
            
            btnClose.Enabled = false;
            btnClose.ForeColor = Color.Gray;
        }

        private async Task FetchDevicesAndAutoConnectAsync()
        {
            discoveredDevices.Clear();
            pnlDevices.Controls.Clear();
            btnOpen.Enabled = false;
            btnClose.Enabled = false;
            pnlDevices.Enabled = true; 
            
            lblStatus.Text = "Scanning devices...";
            lblStatus.ForeColor = Color.LightGray;

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
                Button? autoConnectButton = null;

                foreach (var device in devices)
                {
                    if (!string.IsNullOrEmpty(device.Name))
                    {
                        discoveredDevices.Add(device);
                        
                        Button deviceBtn = new Button
                        {
                            Text = "", 
                            Size = new Size(250, 45), 
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(45, 45, 45), 
                            ForeColor = Color.White,
                            Margin = new Padding(5)
                        };
                        deviceBtn.FlatAppearance.BorderSize = 0;
 
                        deviceBtn.Paint += (s, ev) =>
                        {
                            Button btn = (Button)s!;
                            string iconText = "🎵";
                            string deviceName = device.Name;

                            using (Font iconFont = new Font("Segoe UI Emoji", 16, FontStyle.Regular))
                            using (Font textFont = new Font("Segoe UI", 10, FontStyle.Bold))
                            {
                                Size iconSize = TextRenderer.MeasureText(ev.Graphics, iconText, iconFont);
                                TextRenderer.DrawText(ev.Graphics, iconText, iconFont, 
                                    new Point(10, (btn.Height - iconSize.Height) / 2), 
                                    btn.ForeColor);

                                Rectangle textBounds = new Rectangle(
                                    15 + iconSize.Width, 
                                    0, 
                                    btn.Width - (25 + iconSize.Width), 
                                    btn.Height);
                                    
                                TextRenderer.DrawText(ev.Graphics, deviceName, textFont, textBounds, 
                                    btn.ForeColor, 
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.WordEllipsis);
                            }
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
                    lblStatus.ForeColor = Color.LightCoral;
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
                lblStatus.ForeColor = Color.LightCoral;
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
            lblStatus.ForeColor = Color.LightSkyBlue;
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
                        lblStatus.ForeColor = Color.LimeGreen; 
                        
                        btnClose.Enabled = true;
                        btnClose.ForeColor = Color.White;

                        File.WriteAllText(configPath, device.Id);
                    }
                    else
                    {
                        lblStatus.Text = $"Failed: {result.Status}";
                        lblStatus.ForeColor = Color.LightCoral; 
                        
                        btnOpen.Enabled = true;
                        pnlDevices.Enabled = true; 
                    }
                }
            }
            catch (Exception)
            {
                lblStatus.Text = "Connection Error!";
                lblStatus.ForeColor = Color.LightCoral;
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
                btnClose.ForeColor = Color.Gray;
                
                btnOpen.Enabled = true;
                btnOpen.ForeColor = Color.White;

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
}