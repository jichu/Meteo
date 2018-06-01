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
        Bitmap orp = Properties.Resources.ORP;
		
		public PreImage()
        {
            View.FormMain.button1.Click += new EventHandler(button1_Click);

		}

		private void LoadORP()
		{
			try
			{
				Util.ShowLoading("ORP...");
				var mapCR =
					 from x in Enumerable.Range(0, orp.Width - 1)
					 from y in Enumerable.Range(0, orp.Height - 1)
					 select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

				mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));

				foreach (var map in mapCR.Where((key, val) => key.color.Name == "ff191919"))
				{
					Util.l(map.color.Name+" "+map.point.X+"x"+map.point.Y);
				}
			}
			catch (Exception ex)
			{
				Util.l(ex.ToString());
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(LoadORP));
			t.Start();
		}



        private void Test()
        {
            try {
                Util.ShowLoading("Zpracování ...");

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
