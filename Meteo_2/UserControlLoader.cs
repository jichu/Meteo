using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Meteo
{
    public partial class UserControlLoader : UserControl
    {
        public static UserControlLoader uc;

        private StringBuilder log = new StringBuilder();

        public event PropertyChangedEventHandler PropertyLogChanged;

        public static UserControlLoader Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlLoader();
                return uc;
            }
        }

        //private List<string> log = new List<string>();
        private int logCount = 0;

        public UserControlLoader()
        {
            InitializeComponent();

        }

        private void UserControlLoader_Load(object sender, EventArgs e)
        {
            Task.Run(() => UpdateLog());

        }

        private void UpdateLog()
        {
            while (true)
            {
                /*
                infoText.BeginInvoke((Action)(() =>
                {
                    infoText.Text = "l";
                }));*/
                Thread.Sleep(100);
            }
        }

        public void UpdateMessage(string message)
        {
            Util.l(message);
            labelMessage.BeginInvoke((Action)(() =>
            {
                labelMessage.Text = message;
            }));
            ClearLog();
            Application.DoEvents();
        }


        public void UpdateInfo(string message)
        {
            log.AppendLine(message);
            //Util.l(log.Length);
            /*
             infoText.BeginInvoke((Action)(() =>
            {
                infoText.Text = message + Environment.NewLine + infoText.Text;
            }));
            */
            Application.DoEvents();

        }

        internal void ClearLog()
        {
            log.Clear();
            infoText.BeginInvoke((Action)(() =>
            {
                infoText.Text = "";
            }));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyLogChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
