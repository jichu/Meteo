using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public partial class FormLoader : Form
    {
        public FormLoader(string message,string info="")
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            labelMessage.Text = message;
            labelInfo.Text = info;
        }

        public void UpdateInfo(string message)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                labelInfo.Text = message;
            }));
        }

        public void ShowLoader()
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                this.Show();
                this.Refresh();
            }));
        }

    }
}
