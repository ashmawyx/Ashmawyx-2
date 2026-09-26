using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace AshmawyX
{
    public class MainForm : Form
    {
        public MirroringEngine engine;

        public ComboBox comboMainWindow;
        public ComboBox comboMirrorWindow;
        private Button btnRefreshWindows;

        public ListBox listMirrorKeys;
        private Button btnAddKey;
        private Button btnRemoveKey;

        public NumericUpDown numDelayFrom;
        public NumericUpDown numDelayTo;

        private Button btnStart;
        private Button btnStop;
        private Button btnPanic;

        private Button btnAshmawyX;

        private Panel panelStatus;
        private Label lblStatusText;
        private Panel panelLed;

        private Timer ledGlowTimer;
        private int ledGlowStep = 0;

        private Color ledIdleColor = Color.FromArgb(0, 180, 255);
        private Color ledActiveColor = Color.FromArgb(255, 140, 0);

        private string mirroredKeysPath = Application.StartupPath + "\\mirrored_keys.txt";
        private string delayPath = Application.StartupPath + "\\delay_settings.txt";

        private KeyCaptureHook keyHook;

        public MainForm()
        {
            InitializeForm();
            InitializeControls();
            InitializeEngine();
            InitializeKeyHook();
            InitializeLedGlow();

            LoadMirroredKeys();
            LoadDelay();
            ScanWindows();
        }

        private void InitializeForm()
        {
            this.Text = "AshmawyX";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ClientSize = new Size(280, 280);
            this.BackColor = Color.FromArgb(18, 18, 22);
        }

        private void InitializeControls()
        {
            // AO windows
            comboMainWindow = new ComboBox
            {
                Location = new Point(10, 10),
                Size = new Size(160, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboMirrorWindow = new ComboBox
            {
                Location = new Point(10, 35),
                Size = new Size(160, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnRefreshWindows = new Button
            {
                Text = "Refresh",
                Location = new Point(180, 10),
                Size = new Size(90, 45),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Mirrored keys
            listMirrorKeys = new ListBox
            {
                Location = new Point(10, 65),
                Size = new Size(120, 70),
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White
            };

            btnAddKey = new Button
            {
                Text = "Add Key",
                Location = new Point(140, 65),
                Size = new Size(130, 22),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnRemoveKey = new Button
            {
                Text = "Remove Key",
                Location = new Point(140, 93),
                Size = new Size(130, 22),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Delay
            Label lblDelayFrom = new Label
            {
                Text = "Delay From:",
                Location = new Point(10, 140),
                Size = new Size(100, 18),
                ForeColor = Color.White
            };

            numDelayFrom = new NumericUpDown
            {
                Location = new Point(110, 140),
                Size = new Size(50, 18),
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.1M
            };

            Label lblDelayTo = new Label
            {
                Text = "Delay To:",
                Location = new Point(10, 165),
                Size = new Size(100, 18),
                ForeColor = Color.White
            };

            numDelayTo = new NumericUpDown
            {
                Location = new Point(110, 165),
                Size = new Size(50, 18),
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.1M
            };

            // Start / Stop / Panic
            btnStart = new Button
            {
                Text = "Start (F7)",
                Location = new Point(170, 140),
                Size = new Size(100, 20),
                BackColor = Color.FromArgb(0, 160, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnStop = new Button
            {
                Text = "Stop (F8)",
                Location = new Point(170, 165),
                Size = new Size(100, 20),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnPanic = new Button
            {
                Text = "Panic (F9)",
                Location = new Point(170, 190),
                Size = new Size(100, 20),
                BackColor = Color.Yellow,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };

            // Status panel
            panelStatus = new Panel
            {
                Location = new Point(10, 215),
                Size = new Size(260, 20),
                BackColor = Color.FromArgb(25, 25, 30)
            };

            lblStatusText = new Label
            {
                Text = "Idle",
                Location = new Point(10, 3),
                Size = new Size(120, 15),
                ForeColor = Color.White
            };

            panelLed = new Panel
            {
                Location = new Point(230, 3),
                Size = new Size(20, 15),
                BackColor = ledIdleColor
            };

            panelStatus.Controls.Add(lblStatusText);
            panelStatus.Controls.Add(panelLed);

            // AshmawyX button
            btnAshmawyX = new Button
            {
                Text = "AshmawyX",
                Location = new Point(10, 240),
                Size = new Size(260, 20),
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Add controls
            Controls.Add(comboMainWindow);
            Controls.Add(comboMirrorWindow);
            Controls.Add(btnRefreshWindows);

            Controls.Add(listMirrorKeys);
            Controls.Add(btnAddKey);
            Controls.Add(btnRemoveKey);

            Controls.Add(lblDelayFrom);
            Controls.Add(numDelayFrom);
            Controls.Add(lblDelayTo);
            Controls.Add(numDelayTo);

            Controls.Add(btnStart);
            Controls.Add(btnStop);
            Controls.Add(btnPanic);

            Controls.Add(panelStatus);
            Controls.Add(btnAshmawyX);

            // Events
            btnRefreshWindows.Click += (s, e) => ScanWindows();
            btnAddKey.Click += BtnAddKey_Click;
            btnRemoveKey.Click += BtnRemoveKey_Click;

            numDelayFrom.ValueChanged += (s, e) => { UpdateDelay(); SaveDelay(); };
            numDelayTo.ValueChanged += (s, e) => { UpdateDelay(); SaveDelay(); };

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnPanic.Click += BtnPanic_Click;

            btnAshmawyX.Click += (s, e) => new AboutForm().ShowDialog();
        }

        private void InitializeEngine()
        {
            engine = new MirroringEngine();
            engine.OnMirroredKey += Engine_OnMirroredKey;
        }

        private void InitializeKeyHook()
        {
            keyHook = new KeyCaptureHook();
            keyHook.OnKeyCaptured += (vk) => engine.ProcessKey(vk);
        }

        private void InitializeLedGlow()
        {
            ledGlowTimer = new Timer();
            ledGlowTimer.Interval = 30;
            ledGlowTimer.Tick += LedGlowTimer_Tick;
        }

        private void LedGlowTimer_Tick(object sender, EventArgs e)
        {
            ledGlowStep++;

            int size = 15 + ledGlowStep;
            panelLed.Size = new Size(size, size);

            if (ledGlowStep >= 6)
            {
                panelLed.Size = new Size(20, 15);
                panelLed.BackColor = ledIdleColor;
                ledGlowStep = 0;
                ledGlowTimer.Stop();
            }
        }

        private void Engine_OnMirroredKey(int vkCode)
        {
            panelLed.BackColor = ledActiveColor;
            ledGlowTimer.Stop();
            ledGlowTimer.Start();
        }

        private void ScanWindows()
        {
            comboMainWindow.Items.Clear();
            comboMirrorWindow.Items.Clear();

            foreach (var info in WindowScanner.GetAOWindows())
            {
                comboMainWindow.Items.Add(info);
                comboMirrorWindow.Items.Add(info);
            }

            if (comboMainWindow.Items.Count > 0)
                comboMainWindow.SelectedIndex = 0;

            if (comboMirrorWindow.Items.Count > 1)
                comboMirrorWindow.SelectedIndex = 1;
        }

        private void BtnAddKey_Click(object sender, EventArgs e)
        {
            string input = PromptForKey();
            if (string.IsNullOrWhiteSpace(input))
                return;

            input = input.Trim().ToUpper();

            foreach (var item in listMirrorKeys.Items)
                if (item.ToString() == input)
                    return;

            listMirrorKeys.Items.Add(input);
            engine.UpdateMirroredKeys(listMirrorKeys.Items);
            SaveMirroredKeys();
        }

        private void BtnRemoveKey_Click(object sender, EventArgs e)
        {
            if (listMirrorKeys.SelectedItem == null)
                return;

            listMirrorKeys.Items.Remove(listMirrorKeys.SelectedItem);
            engine.UpdateMirroredKeys(listMirrorKeys.Items);
            SaveMirroredKeys();
        }

        private string PromptForKey()
        {
            using (var form = new Form())
            {
                form.Text = "Add Key";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(220, 100);
                form.BackColor = Color.FromArgb(18, 18, 22);

                var txt = new TextBox
                {
                    Location = new Point(10, 10),
                    Size = new Size(200, 20),
                    BackColor = Color.FromArgb(25, 25, 30),
                    ForeColor = Color.White
                };

                var btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(40, 40),
                    Size = new Size(60, 25),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(120, 40),
                    Size = new Size(60, 25),
                    BackColor = Color.FromArgb(180, 50, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnOk.Click += (s, e) => form.DialogResult = DialogResult.OK;
                btnCancel.Click += (s, e) => form.DialogResult = DialogResult.Cancel;

                form.Controls.Add(txt);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog(this) == DialogResult.OK)
                    return txt.Text;

                return null;
            }
        }

        private void UpdateDelay()
        {
            engine.SetRandomDelay((double)numDelayFrom.Value, (double)numDelayTo.Value);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            var mainInfo = comboMainWindow.SelectedItem as WindowInfo;
            var mirrorInfo = comboMirrorWindow.SelectedItem as WindowInfo;

            if (mainInfo == null || mirrorInfo == null)
            {
                MessageBox.Show("Select both AO windows first.");
                return;
            }

            engine.SetWindows(mainInfo.Handle, mirrorInfo.Handle);
            engine.Start();

            lblStatusText.Text = "Running";
            panelStatus.BackColor = Color.FromArgb(0, 80, 40);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            engine.Stop();
            lblStatusText.Text = "Stopped";
            panelStatus.BackColor = Color.FromArgb(25, 25, 30);
        }

        private void BtnPanic_Click(object sender, EventArgs e)
        {
            engine.Panic();
            lblStatusText.Text = "Panic";
            panelStatus.BackColor = Color.FromArgb(120, 20, 20);
        }

        private void SaveMirroredKeys()
        {
            using (StreamWriter sw = new StreamWriter(mirroredKeysPath, false))
            {
                foreach (var item in listMirrorKeys.Items)
                    sw.WriteLine(item.ToString());
            }
        }

        private void LoadMirroredKeys()
        {
            if (!File.Exists(mirroredKeysPath))
                return;

            listMirrorKeys.Items.Clear();

            foreach (string key in File.ReadAllLines(mirroredKeysPath))
                if (!string.IsNullOrWhiteSpace(key))
                    listMirrorKeys.Items.Add(key.Trim().ToUpper());

            engine.UpdateMirroredKeys(listMirrorKeys.Items);
        }

        private void SaveDelay()
        {
            using (StreamWriter sw = new StreamWriter(delayPath, false))
            {
                sw.WriteLine(numDelayFrom.Value.ToString());
                sw.WriteLine(numDelayTo.Value.ToString());
            }
        }

        private void LoadDelay()
        {
            if (!File.Exists(delayPath))
                return;

            string[] lines = File.ReadAllLines(delayPath);
            if (lines.Length >= 2)
            {
                numDelayFrom.Value = decimal.Parse(lines[0]);
                numDelayTo.Value = decimal.Parse(lines[1]);
                engine.SetRandomDelay((double)numDelayFrom.Value, (double)numDelayTo.Value);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            keyHook?.Dispose();
            engine?.Stop();
            base.OnFormClosing(e);
        }
    }
}
