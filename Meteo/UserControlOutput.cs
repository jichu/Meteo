using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Meteo
{
    public partial class UserControlOutput : UserControl
    {
        private static UserControlOutput uc;
        private PictureBox canvas;

        public static UserControlOutput Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlOutput();
                return uc;
            }
        }

        public UserControlOutput()
        {
            InitializeComponent();
            CreateCanvas();

            Render();
        }

        private void CreateCanvas()
        {
            PictureBox pb = new PictureBox();
            pb.Width = 600;
            pb.Height = 370;
            pb.BackColor = Color.Khaki;
            pb.Name = "canvas";
            this.Controls.Add(pb);
            canvas = (this.Controls["canvas"] as PictureBox);
            canvas.Image = new Bitmap(canvas.Width, canvas.Height);
            canvas.Paint += Canvas_Paint;
        }

        private void Render()
        {
            canvas.Invalidate();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {

            List<CloudORPColor> l = Model.Cloud.ORPColorGetORPColors();
            foreach (var ll in l)
            {
                DrawRegion(ll.color, Color.Red);
            }
            //DrawRegion("Beroun", Color.Red);

        }

        private void DrawRegion(string hex, Color value)
        {
            Util.ShowLoading("Vykreslování");
            List<CloudMaskSpectrum> cms = Model.Cloud.MaskSpectrumGetCoodsByColor(hex, "Model_ALADIN_CZ");
            string coods = cms.Count > 0 ? cms.First().coods : "";
            if (coods != "")
            {
                foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                {
                    ((Bitmap)canvas.Image).SetPixel((int)point[0], (int)point[1], value);

                }

            }
        }
    }
}
