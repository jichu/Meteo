using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloaderImagesModels
{
    public partial class FormMain : Form
    {
        private NotifyIcon notifyIcon = new NotifyIcon();
        private Download download = null;

        public FormMain()
        {
            InitializeComponent();
            this.Icon= (Icon)Properties.Resources.icon;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            Util.form = this;

            Notify();
        }

        private void Notify()
        {
            notifyIcon.Icon = (Icon)Properties.Resources.icon;
            notifyIcon.Visible = true;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem exitApplication = new ToolStripMenuItem();
            ToolStripMenuItem download = new ToolStripMenuItem();
            ToolStripMenuItem logClear = new ToolStripMenuItem();

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += Monitoring_Click;

            download.Text = "Spustit stahování";
            download.Click += new EventHandler(Download_Click);
            contextMenu.Items.Add(download);

            logClear.Text = "Vyčistit log";
            logClear.Click += new EventHandler(LogClear_Click);
            contextMenu.Items.Add(logClear);

            exitApplication.Text = "Zavřít";
            exitApplication.Click += new EventHandler(ExitApplication_Click);
            contextMenu.Items.Add(exitApplication);
        }

        private void Monitoring_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void LogClear_Click(object sender, EventArgs e)
        {
            if(File.Exists(Util.logFile))
                File.WriteAllText(Util.logFile, String.Empty);
        }


        private void ExitApplication_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            download = new Download();
            download.EventProcess += OnProcess;


        }

        private void OnProcess(object sender, DownloadEventArgs e)
        {
            notifyIcon.Icon = !e.Process?(Icon)Properties.Resources.icon: (Icon)Properties.Resources.icon_process;
            this.BeginInvoke((Action)(() =>
            {
                this.Icon = !e.Process ? (Icon)Properties.Resources.icon : (Icon)Properties.Resources.icon_process;
                this.Text = e.Process ? "Stahování "+e.Hour : "Downloader Images Models";
                this.button1.Enabled = e.Process ? false : true;
            }));
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon.Icon = null;
            try
            {
                foreach (var process in Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName))
                {
                    process.Kill();
                }
                foreach (var process in Process.GetProcessesByName("DownloaderImagesModels.vshost"))
                {
                    process.Kill();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            download.downloadHour="";
            download?.Process();
        }

        private void Download_Click(object sender, EventArgs e)
        {
            download.downloadHour = "";
            download?.Process();
        }
    }
}
