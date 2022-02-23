using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;

namespace Meteo
{
    public partial class UserControlForecast : UserControl
    {
        private static UserControlForecast uc;
        public static UserControlForecast Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlForecast();
                return uc;
            }
        }

        public UserControlForecast()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Thread t = new Thread(() => ShowMap("preba", "http://portal.chmi.cz/files/portal/docs/meteo/om/evropa/preba/preba.gif",0,0));
            t.Start();
        }

        private void ShowMap(string name, string url, int x, int y)
        {
            ClearControl(name);
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                PictureBox pb = new PictureBox();
                pb.Name = name;
                Image img = Bitmap.FromStream(stream);
                pb.Image = img;
                pb.Width = img.Width;
                pb.Height = img.Height;
                pb.Location = new Point(x, y);
                this.BeginInvoke((Action)(() =>
                {
                    this.Controls.Add(pb);
                }));

                if (name == "analyza")
                {
                    return;
                }

                ShowMap("analyza", "http://portal.chmi.cz/files/portal/docs/meteo/om/evropa/analyza.gif", 0, img.Height + 10);
            }
        }

        private void ClearControl(string v)
        {
            foreach (Control item in this.Controls.OfType<Control>())
            {
                if (item.Name == v)
                    this.Controls.Remove(item);
            }
        }
    }
}
