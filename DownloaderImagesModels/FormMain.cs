using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloaderImagesModels
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            Util.form = this;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            bool csv = LoadSetting.ReadCSVFile(@"data.csv");

            if (csv)
            {
                status.Text = "Ready...";
                new Download();
            }
            else {
                Util.l("CSV nenačteno",
                     new Dictionary<string, object>
                     {
                        { "messageBoxIcon", MessageBoxIcon.Warning}
                     }
                );
             }

        }


        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
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

    }
}
