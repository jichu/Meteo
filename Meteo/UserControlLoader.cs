using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public partial class UserControlLoader : UserControl
    {
        public static UserControlLoader uc;

        public static UserControlLoader Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlLoader();
                return uc;
            }
        }

        private List<string> log = new List<string>();
        private int logCount = 0;

        public UserControlLoader()
        {
            InitializeComponent();
        }

        public void UpdateMessage(string message)
        {
            labelMessage.BeginInvoke((Action)(() =>
            {
                labelMessage.Text = message;
            }));
            ClearLog();
            Application.DoEvents();
        }


        public void UpdateInfo(string message)
        {
             infoText.BeginInvoke((Action)(() =>
            {
                infoText.Text = message + Environment.NewLine + infoText.Text;
            }));
            Application.DoEvents();
        }

        internal void ClearLog()
        {
            infoText.BeginInvoke((Action)(() =>
            {
                infoText.Text = "";
            }));
        }
    }
}
