using System;
using System.Drawing;
using System.Windows.Forms;

namespace AshmawyX
{
    public class ProfileForm : Form
    {
        private ListBox listProfiles;
        private TextBox txtName;
        private Button btnSave;
        private Button btnLoad;
        private Button btnDelete;
        private Button btnClose;

        private MainForm main;

        public ProfileForm(MainForm mainForm)
        {
            main = mainForm;

            InitializeForm();
            InitializeControls();
            LoadProfiles();
        }

        private void InitializeForm()
        {
            this.Text = "Profiles";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(280, 240);
            this.BackColor = Color.FromArgb(18, 18, 22);
        }

        private void InitializeControls()
        {
            listProfiles = new ListBox
            {
                Location = new Point(10, 10),
                Size = new Size(120, 150),
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White
            };

            txtName = new TextBox
            {
                Location = new Point(140, 10),
                Size = new Size(130, 20),
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(140, 40),
                Size = new Size(130, 25),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLoad = new Button
            {
                Text = "Load",
                Location = new Point(140, 70),
                Size = new Size(130, 25),
                BackColor = Color.FromArgb(0, 160, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(140, 100),
                Size = new Size(130, 25),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(80, 180),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (s, e) => this.Close();

            Controls.Add(listProfiles);
            Controls.Add(txtName);
            Controls.Add(btnSave);
            Controls.Add(btnLoad);
            Controls.Add(btnDelete);
            Controls.Add(btnClose);
        }

        private void LoadProfiles()
        {
            listProfiles.Items.Clear();
            foreach (string p in ProfileManager.GetProfiles())
                listProfiles.Items.Add(p);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("Enter profile name.");
                return;
            }

            Profile profile = new Profile
            {
                Name = name,
                DelayFrom = (double)main.numDelayFrom.Value,
                DelayTo = (double)main.numDelayTo.Value
            };

            foreach (var item in main.listMirrorKeys.Items)
                profile.MirroredKeys.Add(item.ToString());

            ProfileManager.SaveProfile(profile);
            LoadProfiles();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (listProfiles.SelectedItem == null)
                return;

            string name = listProfiles.SelectedItem.ToString();
            Profile profile = ProfileManager.LoadProfile(name);

            if (profile == null)
                return;

            main.listMirrorKeys.Items.Clear();
            foreach (string key in profile.MirroredKeys)
                main.listMirrorKeys.Items.Add(key);

            main.engine.UpdateMirroredKeys(main.listMirrorKeys.Items);

            main.numDelayFrom.Value = (decimal)profile.DelayFrom;
            main.numDelayTo.Value = (decimal)profile.DelayTo;

            main.engine.SetRandomDelay(profile.DelayFrom, profile.DelayTo);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (listProfiles.SelectedItem == null)
                return;

            string name = listProfiles.SelectedItem.ToString();
            ProfileManager.DeleteProfile(name);
            LoadProfiles();
        }
    }
}
