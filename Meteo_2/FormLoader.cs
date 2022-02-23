using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Meteo
{
    public partial class FormLoader : Form
    {
        private List<string> log = new List<string>();
        private int logCount=0;

        public FormLoader(string message,string info="")
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            labelMessage.Text = message;
            infoText.Text = info;
            log.Clear();
            Resize2();
        }

        private void Resize2()
        {
            this.Size = new Size(View.FormMain.Size.Width, View.FormMain.Size.Height);
            this.Location = new Point(View.FormMain.Location.X, View.FormMain.DesktopLocation.Y); 
        }

        public void UpdateInfo(string message)
        {
            if (message != "")
            {
                Util.l(message);
            }
                infoText.BeginInvoke((Action)(() =>
                {
                    List<string> tmp = log;
                        infoText.Text = message + Environment.NewLine + infoText.Text;
                }));
                Application.DoEvents();
        }

        private void FormLoader_Load(object sender, EventArgs e)
        {
        }


        private void FormLoader_Shown(object sender, EventArgs e)
        {
            /*
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 10;
            aTimer.Enabled = true;
            */
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (logCount != log.Count)
                {
                    infoText.BeginInvoke((Action)(() =>
                    {
                        List<string> tmp = log.AsEnumerable().Reverse().ToList();
                        foreach (var item in tmp)
                        {
                            infoText.Text += item + Environment.NewLine;
                        }
                    }));
                    logCount = log.Count;
                }
                Application.DoEvents();
            }
            catch (Exception ex) { Util.l(ex); }
        }

        private void labelMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
