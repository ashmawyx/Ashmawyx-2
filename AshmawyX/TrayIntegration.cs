using System;
using System.Drawing;
using System.Windows.Forms;

namespace AshmawyX
{
    public class TrayIntegration
    {
        private NotifyIcon trayIcon;
        private MainForm mainForm;

        public TrayIntegration(MainForm form)
        {
            mainForm = form;

            trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Application;
            trayIcon.Visible = true;

            var menu = new ContextMenuStrip();

            menu.Items.Add("Show", null, (s, e) => mainForm.Show());
            menu.Items.Add("About", null, (s, e) => new AboutForm().ShowDialog());
            menu.Items.Add("Exit", null, (s, e) => Application.Exit());

            trayIcon.ContextMenuStrip = menu;
        }
    }
}
