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
    public partial class FormLog : Form
    {
        private List<string> Log = new List<string>();

        public FormLog()
        {
            InitializeComponent();
        }

        public FormLog(List<string> log, string title="Výpis logu", int width=600, int height=400)
        {
            InitializeComponent();
            this.Text = title;
            this.Width = width;
            this.Height = height;
            this.Log = log;
        }

        private void FormLog_Load(object sender, EventArgs e)
        {
            foreach (var item in Log)
            {
                log.Text += item + Environment.NewLine + Environment.NewLine;
            }
        }
    }
}
