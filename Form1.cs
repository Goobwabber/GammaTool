using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using gamma2.Properties;

namespace rebellagamma
{
    public partial class Form1 : Form
    {
        [DllImport("gdi32.dll")]
        static extern bool GetDeviceGammaRamp(IntPtr hdc, out RAMP lpRamp);
        
        [DllImport("gdi32.dll")]
        static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Blue;
        }

        private ARSetting? minARSetting = null;
        private ARSetting? maxARSetting = null;
        private string ConfigFilePath => Path.Combine(Application.StartupPath, "config.txt");
        private FileSystemWatcher _configWatcher;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;

        public Form1()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            InitializeComponent();

            using (MemoryStream ms = new MemoryStream(gamma2.Properties.Resources.ico))
            {
                this.Icon = new Icon(ms);
            }

            LoadMonitors();
            UpdateValueLabels();

            this.Load += Form1_Load;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = "gamma slave extra";
            notifyIcon.Visible = true;

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            trayMenu.Items.Add("Exit", null, (s, e) => { notifyIcon.Visible = false; Application.Exit(); });

            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.ShowBalloonTip(1000, "gamma slave extra", "The application is running in the background.", ToolTipIcon.Info);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EnsureConfigFileExists();
            InitializeConfigWatcher();
            LoadProfiles();
            _ = ConnectWithRetryAsync();
        }

        private async Task ConnectWithRetryAsync()
        {
            var uri = new Uri("ws://127.0.0.1:24050/websocket/v2");
            var writer = new ArrayBufferWriter<byte>();
            double lastAR = 0;
            int lastState = 0;
            
            
            while (true)
            {
                try
                {
                    using var ws = new ClientWebSocket();
                    await ws.ConnectAsync(uri, CancellationToken.None);

                    Invoke((MethodInvoker)delegate
                    {
                        numericUpDownTargetAR.Enabled = false;
                        trackBarAR.Visible = false;
                        buttonSetAR3.Visible = false;
                        labelConnected.Visible = true;
                        labelTosuInfo.Visible = true;
                        labelTosuInfo.Text = "";
                    });
                    
                    while (ws.State == WebSocketState.Open)
                    {
                        ValueWebSocketReceiveResult result;

                        do
                        {
                            Memory<byte> memory = writer.GetMemory(2048);
                            result = await ws.ReceiveAsync(memory, CancellationToken.None);
                            writer.Advance(result.Count);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            writer.Clear();
                            Invoke((MethodInvoker)delegate
                            {
                                numericUpDownTargetAR.Enabled = true;
                                trackBarAR.Visible = true;
                                buttonSetAR3.Visible = true;
                                labelConnected.Visible = false;
                                labelTosuInfo.Visible = false;
                            });
                            break;
                        }

                        var message = JsonSerializer.Deserialize<TosuMessage>(writer.WrittenSpan);
                        writer.Clear();
                        
                        //Console.WriteLine("State Id: " + message?.state?.number + " Name: " + message?.state?.name);
                        //Console.WriteLine("Original: " + message?.beatmap?.stats?.ar?.original + " Converted: " + message?.beatmap?.stats?.ar?.converted);
                        
                        Invoke((MethodInvoker)delegate
                        {
                            // basic checks
                            if (message?.state?.number == null)
                                return;
                            var state = message.state.number;
                            
                            if (message?.beatmap?.stats?.ar?.converted == null)
                                return;
                            var ar = message.beatmap.stats.ar.converted;
                            
                            // update AR on the UI
                            if (Math.Abs(ar - lastAR) > 1e-6)
                            {
                                trackBarAR.Value = Convert.ToInt16(message.beatmap.stats.ar.converted * 100);
                                numericUpDownTargetAR.Value = Convert.ToDecimal(message.beatmap.stats.ar.converted);
                                lastAR = message.beatmap.stats.ar.converted;
                            }
                            
                            // If AR settings bad, return.
                            if (minARSetting == null || maxARSetting == null)
                            {
                                labelTosuInfo.ForeColor = Color.Red;
                                labelTosuInfo.Text = "First, set the min and max AR settings.";
                                return;
                            }
                            
                            // Gamestate is not gameplay notification
                            if (state != 2)
                            {
                                labelTosuInfo.ForeColor = Color.Yellow;
                                labelTosuInfo.Text = "Gamestate: " + message?.state?.number + message?.state?.name;
                            }
                            else
                            {
                                labelTosuInfo.ForeColor = Color.Green;
                                labelTosuInfo.Text = "Adjusting AR for gameplay";
                            }
                            
                            // Everything down will change windows settings, so only change if input state has changed.
                            if (lastState == state && Math.Abs(ar - lastAR) < 1e-6)
                                return;
                            
                            if (state == 2 && lastState != 2)
                                _ = InterpolateAR(); // Entering gameplay
                            if (lastState == 2 && state != 2)
                                Reset(); // Exiting gameplay

                            lastState = state;
                            lastAR = ar;
                        });
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    await Task.Delay(2000);
                }
            }
            
            
        }

        private void EnsureConfigFileExists()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    File.WriteAllText(ConfigFilePath, string.Empty);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No permission to write to the application folder. Move the application to a folder with write permissions (e.g., Desktop).",
                              "Write Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot create the configuration file: {ex.Message}",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void InitializeConfigWatcher()
        {
            try
            {
                _configWatcher = new FileSystemWatcher
                {
                    Path = Path.GetDirectoryName(ConfigFilePath),
                    Filter = Path.GetFileName(ConfigFilePath),
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };

                _configWatcher.Changed += ConfigWatcher_Changed;
                _configWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during FileSystemWatcher initialization: " + ex.Message);
            }
        }

        private void ConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LoadProfiles();
            });
        }

        private void LoadMonitors()
        {
            comboBoxMonitors.Items.Clear();
            foreach (var screen in Screen.AllScreens)
            {
                comboBoxMonitors.Items.Add(screen.DeviceName);
            }
            if (comboBoxMonitors.Items.Count > 0)
                comboBoxMonitors.SelectedIndex = 0;
        }

        private void UpdateValueLabels()
        {
            numericUpDown1.Value = (decimal)(trackBarGamma.Value / 100.0f);
            numericUpDown2.Value = (decimal)(trackBarBrightness.Value / 100.0f);
            numericUpDown3.Value = (decimal)(trackBarContrast.Value / 100.0f);
            numericUpDownTargetAR.Value = (decimal)(trackBarAR.Value / 100.0f);
        }
        
        private void ApplyAllSettings()
        {
            if (comboBoxMonitors.SelectedItem == null) return;

            // 1. Ensure this string is formatted exactly like "\\.\DISPLAY1"
            // You can get these from Screen.AllScreens[i].DeviceName
            string deviceName = comboBoxMonitors.SelectedItem.ToString();
            
            // Pass null for lpszDriver, and the device name for lpszDevice
            IntPtr hdc = CreateDC(null, deviceName, IntPtr.Zero, IntPtr.Zero);
            
            // Fallback to primary monitor if the handle failed to create
            if (hdc == IntPtr.Zero)
            {
                hdc = CreateDC("DISPLAY", null, IntPtr.Zero, IntPtr.Zero);
                if (hdc == IntPtr.Zero) return;
            }

            float gamma = Math.Max(trackBarGamma.Value / 100.0f, 0.1f);
            int brightness = trackBarBrightness.Value;
            float contrast = trackBarContrast.Value / 100.0f;

            RAMP ramp = new RAMP()
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            ushort lastVal = 0;

            for (int i = 0; i < 256; i++)
            {
                double normalized = i / 255.0;

                double gammaCorrected = Math.Pow(normalized, 1.0 / gamma);
                double contrastAdjusted = ((gammaCorrected - 0.5) * contrast) + 0.5;
                double brightnessAdjusted = contrastAdjusted + (brightness / 255.0);

                int rampVal = (int)(Math.Clamp(brightnessAdjusted, 0.0, 1.0) * 65535);

                // 2. Enforce strict driver requirements
                if (rampVal < lastVal) rampVal = lastVal; // Must constantly increase
                if (i == 0) rampVal = 0;                  // Index 0 must be 0
                if (i == 255) rampVal = 65535;            // Index 255 must be 65535

                lastVal = (ushort)rampVal;
                ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = (ushort)rampVal;
            }

            bool success = SetDeviceGammaRamp(hdc, ref ramp);
            
            if (!success)
            {
                int err = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Console.WriteLine($"SetDeviceGammaRamp failed. Error Code: {err}");
            }

            DeleteDC(hdc);
        }

        private async Task ApplyAllSettingsGradually()
        {
            if (comboBoxMonitors.SelectedItem == null) return;

            string deviceName = comboBoxMonitors.SelectedItem.ToString();
            IntPtr hdc = CreateDC(null, deviceName, IntPtr.Zero, IntPtr.Zero);
            if (hdc == IntPtr.Zero) hdc = CreateDC("DISPLAY", null, IntPtr.Zero, IntPtr.Zero);
            if (hdc == IntPtr.Zero) return;

            // 1. Get current hardware state
            if (!GetDeviceGammaRamp(hdc, out RAMP currentRamp))
            {
                DeleteDC(hdc);
                return;
            }

            // 2. Generate the target state (using your hardened math from earlier)
            RAMP targetRamp = GenerateTargetRamp(); 

            // 3. Interpolate and apply over 10 steps (~250ms)
            int steps = 10;
            for (int step = 1; step <= steps; step++)
            {
                RAMP stepRamp = new RAMP() 
                { 
                    Red = new ushort[256], Green = new ushort[256], Blue = new ushort[256] 
                };
        
                double progress = (double)step / steps;

                for (int i = 0; i < 256; i++)
                {
                    // Linear interpolation between current and target
                    stepRamp.Red[i] = (ushort)(currentRamp.Red[i] + (targetRamp.Red[i] - currentRamp.Red[i]) * progress);
                    stepRamp.Green[i] = (ushort)(currentRamp.Green[i] + (targetRamp.Green[i] - currentRamp.Green[i]) * progress);
                    stepRamp.Blue[i] = (ushort)(currentRamp.Blue[i] + (targetRamp.Blue[i] - currentRamp.Blue[i]) * progress);
                }

                SetDeviceGammaRamp(hdc, ref stepRamp);
        
                // Wait 25ms before the next step (10 * 25ms = 250ms total)
                await Task.Delay(25); 
            }

            DeleteDC(hdc);
        }
        
        private RAMP GenerateTargetRamp()
        {
            float gamma = Math.Max(trackBarGamma.Value / 100.0f, 0.1f);
            int brightness = trackBarBrightness.Value;
            float contrast = trackBarContrast.Value / 100.0f;

            RAMP ramp = new RAMP()
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            ushort lastVal = 0;

            for (int i = 0; i < 256; i++)
            {
                double normalized = i / 255.0;

                double gammaCorrected = Math.Pow(normalized, 1.0 / gamma);
                double contrastAdjusted = ((gammaCorrected - 0.5) * contrast) + 0.5;
                double brightnessAdjusted = contrastAdjusted + (brightness / 255.0);

                double clamped = Math.Clamp(brightnessAdjusted, 0.0, 1.0);
                int rampVal = (int)(clamped * 65535);

                if (rampVal < lastVal) 
                {
                    rampVal = lastVal;
                }

                if (i > 0 && rampVal == lastVal && rampVal < 65535)
                {
                    rampVal = lastVal + 1;
                }

                lastVal = (ushort)rampVal;
                ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = (ushort)rampVal;
            }

            ramp.Red[0] = ramp.Green[0] = ramp.Blue[0] = 0;
            ramp.Red[255] = ramp.Green[255] = ramp.Blue[255] = 65535;

            return ramp;
        }

        private void trackBarGamma_Scroll(object sender, EventArgs e)
        {
            ApplyAllSettings();
            UpdateValueLabels();
            panelGraph.Invalidate();
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            ApplyAllSettings();
            UpdateValueLabels();
            panelGraph.Invalidate();
        }

        private void trackBarContrast_Scroll(object sender, EventArgs e)
        {
            ApplyAllSettings();
            UpdateValueLabels();
            panelGraph.Invalidate();
        }

        private void trackBarAR_Scroll(object sender, EventArgs e)
        {
            ApplyAllSettings();
            UpdateValueLabels();
            panelGraph.Invalidate();
        }

        private void comboBoxMonitors_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyAllSettings();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            trackBarGamma.Value = 100;
            trackBarBrightness.Value = 0;
            trackBarContrast.Value = 100;
            ApplyAllSettings();
            UpdateValueLabels();
            panelGraph.Invalidate();
        }

        private void buttonSaveAR1_Click(object sender, EventArgs e)
        {
            minARSetting = GetCurrentSettings();
            SetAR1Text();
            this.ActiveControl = null;
        }

        private void buttonSaveAR2_Click(object sender, EventArgs e)
        {
            maxARSetting = GetCurrentSettings();
            SetAR2Text();
            this.ActiveControl = null;
        }

        private void SetAR1Text()
            => labelAR1.Text = minARSetting != null ? $"Min AR: {minARSetting.AR:F2} | G: {minARSetting.Colors.Gamma:0.00} | B: {minARSetting.Colors.Brightness} | C: {minARSetting.Colors.Contrast}" : "Min AR: -"; 
        
        private void SetAR2Text()
            => labelAR2.Text = maxARSetting != null ? $"Max AR: {maxARSetting.AR:F2} | G: {maxARSetting.Colors.Gamma:0.00} | B: {maxARSetting.Colors.Brightness} | C: {maxARSetting.Colors.Contrast}" : "Max AR: -";

        private async void buttonSetAR3_Click(object sender, EventArgs e)
        {
            if (minARSetting == null || maxARSetting == null)
            {
                MessageBox.Show("First, save the AR1 and AR2 settings.");
                return;
            }

            if (Math.Abs(maxARSetting.AR - minARSetting.AR) < 1e-6)
            {
                MessageBox.Show("AR1 and AR2 have the same value; interpolation is not possible.");
                return;
            }
            
            await InterpolateAR();
        }

        private async Task InterpolateAR()
        {
            var targetAR = trackBarAR.Value / 100f;
            double factor = 0;
            if (Math.Abs(maxARSetting.AR - minARSetting.AR) > 1e-6)
            {
                factor = (targetAR - minARSetting.AR) /
                         (maxARSetting.AR - minARSetting.AR);
                factor = Math.Max(0, Math.Min(1, factor));
            }
            else if (targetAR > minARSetting.AR)
                factor = 1;
            else
                factor = 0;

            double gamma = Lerp(minARSetting.Colors.Gamma, maxARSetting.Colors.Gamma, factor);
            int brightness = (int)Math.Round(Lerp(minARSetting.Colors.Brightness, maxARSetting.Colors.Brightness, factor));
            int contrast = (int)Math.Round(Lerp(minARSetting.Colors.Contrast, maxARSetting.Colors.Contrast, factor));

            gamma = Math.Max(0.08, Math.Min(8.88, gamma));
            brightness = Math.Max(-888, Math.Min(888, brightness));
            contrast = Math.Max(8, Math.Min(888, contrast));

            trackBarGamma.Value = (int)(gamma * 100);
            trackBarBrightness.Value = brightness;
            trackBarContrast.Value = contrast;

            UpdateValueLabels();
            panelGraph.Invalidate();
            this.ActiveControl = null;
            await ApplyAllSettingsGradually();
        }

        private ARSetting GetCurrentSettings()
        {
            return new()
            {
                AR = trackBarAR.Value / 100f,
                Colors = new ColorSetting()
                {
                    Gamma = trackBarGamma.Value / 100.0,
                    Brightness = trackBarBrightness.Value,
                    Contrast = trackBarContrast.Value,
                },
            };
        }

        private bool TryParseAR(string text, out double value)
        {
            text = text.Replace(',', '.');
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private double Lerp(double start, double end, double factor)
        {
            return start + (end - start) * factor;
        }

        private double ApplyDTIfChecked(double arValue, CheckBox checkBox)
        {
            return checkBox.Checked ? ((arValue * 2) + 13) / 3 : arValue;
        }

        private void panelGraph_Paint(object sender, PaintEventArgs e)
        {
            int w = panelGraph.Width;
            int h = panelGraph.Height;
            Graphics g = e.Graphics;

            g.DrawImage(gamma2.Properties.Resources.background, new Rectangle(0, 0, w, h));

            using (Pen borderPen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(borderPen, 1, 1, w - 2, h - 2);
            }

            using (Pen pen = new Pen(Color.White, 2))
            {
                Point[] points = new Point[w];
                for (int x = 0; x < w; x++)
                {
                    double normalized = x / (double)(w - 1);

                    float gamma = trackBarGamma.Value / 100f;
                    if (gamma < 0.1f) gamma = 0.1f;

                    int brightness = trackBarBrightness.Value;
                    float contrast = trackBarContrast.Value / 100f;

                    double gammaCorrected = Math.Pow(normalized, 1.0 / gamma);
                    double contrastAdjusted = ((gammaCorrected - 0.5) * contrast) + 0.5;
                    double brightnessAdjusted = contrastAdjusted + (brightness / 255.0);

                    double val = brightnessAdjusted;
                    if (val < 0) val = 0;
                    if (val > 1) val = 1;

                    int y = (int)((1.0 - val) * (h - 1));
                    points[x] = new Point(x, y);
                }

                g.DrawLines(pen, points);
            }
        }

        private async void buttonSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxProfileName.Text))
            {
                MessageBox.Show("Please enter a profile name.");
                return;
            }

            var profile = GetCurrentSettings();
            string profileName = textBoxProfileName.Text;
            string profileLine = $"{profileName}|{profile.Colors.Gamma}|{profile.Colors.Brightness}|{profile.Colors.Contrast}|{minARSetting?.AR}|{minARSetting?.Colors.Gamma}|{minARSetting?.Colors.Brightness}|{minARSetting?.Colors.Contrast}|{maxARSetting?.AR}|{maxARSetting?.Colors.Gamma}|{maxARSetting?.Colors.Brightness}|{maxARSetting?.Colors.Contrast}";

            try
            {
                var lines = File.Exists(ConfigFilePath) ? File.ReadAllLines(ConfigFilePath).ToList() : new List<string>();
                bool profileExists = lines.Any(line => line.Split('|')[0] == profileName);

                if (profileExists)
                {
                    var result = MessageBox.Show("This profile name already exists. Do you want to overwrite it?",
                                                 "Confirm overwrite",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                        return;

                    lines = lines.Where(line => line.Split('|')[0] != profileName).ToList();
                }

                lines.Add(profileLine);
                File.WriteAllLines(ConfigFilePath, lines);

                LoadProfiles();

                buttonSaveProfile.Text = "Saved!";
                await Task.Delay(1000);
                buttonSaveProfile.Text = "Save";
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No write permissions in the application folder. Move the application to a folder with write permissions (e.g., Desktop).",
                              "Saving Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profile: {ex.Message}");
            }
        }

        private void textBoxProfileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSaveProfile.PerformClick();
                e.SuppressKeyPress = true;
            }
        }


        private async void buttonLoadProfile_Click(object sender, EventArgs e)
        {
            if (comboBoxProfiles.SelectedItem == null) return;

            string selectedProfile = comboBoxProfiles.SelectedItem.ToString();
            var profiles = GetSavedProfiles();
            var profile = profiles.FirstOrDefault(p => p.Name == selectedProfile);

            LoadProfile(profile);

            buttonLoadProfile.Text = "Loaded!";
            await Task.Delay(1000);
            buttonLoadProfile.Text = "Load";
        }

        private void LoadProfile(Profile profile)
        {
            if (profile?.Name != null)
            {
                double gamma = Math.Max(0.08, Math.Min(8.88, profile.Colors.Gamma));
                int brightness = Math.Max(-888, Math.Min(888, profile.Colors.Brightness));
                int contrast = Math.Max(8, Math.Min(888, profile.Colors.Contrast));

                minARSetting = profile.MinARSetting;
                maxARSetting = profile.MaxARSetting;

                trackBarGamma.Value = (int)(gamma * 100);
                trackBarBrightness.Value = brightness;
                trackBarContrast.Value = contrast;

                SetAR1Text();
                SetAR2Text();
                UpdateValueLabels();
                ApplyAllSettings();
                panelGraph.Invalidate();
            }
        }

        private List<Profile> GetSavedProfiles()
        {
            var profiles = new List<Profile>();

            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    foreach (var line in File.ReadAllLines(ConfigFilePath))
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 12)
                        {
                            string gammaStr = parts[1].Replace(',', '.');
                            string brightnessStr = parts[2].Replace(',', '.');
                            string contrastStr = parts[3].Replace(',', '.');
                            
                            string minArStr = parts[4].Replace(',', '.');
                            string minGammaStr = parts[5].Replace(',', '.');
                            string minBrightnessStr = parts[6].Replace(',', '.');
                            string minContrastStr = parts[7].Replace(',', '.');
                            
                            string maxArStr = parts[8].Replace(',', '.');
                            string maxGammaStr = parts[9].Replace(',', '.');
                            string maxBrightnessStr = parts[10].Replace(',', '.');
                            string maxContrastStr = parts[11].Replace(',', '.');

                            if (double.TryParse(gammaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double gamma) &&
                                int.TryParse(brightnessStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int brightness) &&
                                int.TryParse(contrastStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int contrast) &&
                                float.TryParse(minArStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float minAr) &&
                                double.TryParse(minGammaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double minGamma) &&
                                int.TryParse(minBrightnessStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int minBrightness) &&
                                int.TryParse(minContrastStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int minContrast) &&
                                float.TryParse(maxArStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float maxAr) &&
                                double.TryParse(maxGammaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double maxGamma) &&
                                int.TryParse(maxBrightnessStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int maxBrightness) &&
                                int.TryParse(maxContrastStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int maxContrast))
                            {
                                profiles.Add(new Profile()
                                {
                                    Name = parts[0],
                                    Colors = new ColorSetting()
                                    {
                                        Gamma = gamma,
                                        Brightness = brightness,
                                        Contrast = contrast,
                                    },
                                    MinARSetting = new ARSetting()
                                    {
                                        AR = minAr,
                                        Colors = new ColorSetting()
                                        {
                                            Gamma = minGamma,
                                            Brightness = minBrightness,
                                            Contrast = minContrast,
                                        }
                                    },
                                    MaxARSetting =  new ARSetting()
                                    {
                                        AR = maxAr,
                                        Colors = new ColorSetting()
                                        {
                                            Gamma = maxGamma,
                                            Brightness = maxBrightness,
                                            Contrast = maxContrast,
                                        },
                                    },
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}");
            }

            return profiles;
        }

        private void LoadProfiles()
        {
            comboBoxProfiles.Items.Clear();
            var profiles = GetSavedProfiles();
            foreach (var profile in profiles)
            {
                comboBoxProfiles.Items.Add(profile.Name);
            }

            var defaultProfile = profiles.FirstOrDefault();
            if (defaultProfile != null)
                LoadProfile(defaultProfile);
        }

        private void trackBar_Enter_RemoveFocus(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void trackBar_MouseDown_RemoveFocus(object sender, MouseEventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            if (tb == null) return;

            int thumbPos = (int)((tb.Width - 16) * (tb.Value - tb.Minimum) / (double)(tb.Maximum - tb.Minimum)) + 8;

            if (e.X < thumbPos - 8 || e.X > thumbPos + 8)
            {
                this.ActiveControl = null;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBarGamma.Value = (int)((float)numericUpDown1.Value * 100.0f);
            ApplyAllSettings();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBarBrightness.Value = (int)((float)numericUpDown2.Value * 100.0f);
            ApplyAllSettings();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBarContrast.Value = (int)((float)numericUpDown3.Value * 100.0f);
            ApplyAllSettings();
        }

        private void numericUpDownKeyPress(object sender, KeyPressEventArgs e)
        {
            var nud = sender as NumericUpDown;
            if (nud == null) return;

            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                e.KeyChar = '.';
                if (nud.Text.Contains("."))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

        private void numericUpDownKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void numericUpDownAR_KeyDown(object sender, KeyEventArgs e)
        {
            trackBarAR.Value = (int)((float)numericUpDownTargetAR.Value * 100.0f);
            ApplyAllSettings();
        }
    }
}