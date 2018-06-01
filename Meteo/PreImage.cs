using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meteo
{
    class PreImage
    {
        Bitmap orp;

        public PreImage()
        {
            View.FormMain.button1.Click += new EventHandler(button1_Click);

            orp = Properties.Resources.ORP;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Test));
            t.Start();
        }

        private void Test()
        {
            try {
                Util.ShowLoading("Zpracování...");

                View.FormMain.button1.BeginInvoke((Action)(() =>
                    View.FormMain.button1.Text = "počítám"
                ));

                for (var i = 1; i < 11000; i++)
                    Util.l(i);

                View.FormMain.button1.BeginInvoke((Action)(() =>
                    View.FormMain.button1.Text = "hotovo"
                ));
            }
            catch (Exception ex)
            {
                Util.l(ex.ToString());
            }
        }
    }
}
