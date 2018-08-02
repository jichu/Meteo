using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    class Images
    {
        private bool isDragging;
        private int currentX;
        private int currentY;
        private PreImage mapORP;
        private Bitmap bmp;

        public Images() {
            /*
            mapORP = new PreImage();
            mapORP.LoadORPfromModels();
            LoadModel();
            LoadORP();
            LoadPointsOfColorsInMap();*/

        }

        private void LoadPointsOfColorsInMap()
        {

            foreach (var map in mapORP.getMapORP())
            {
                Util.l("Region: "+map.Key + ":");
                List<Color> colors = new List<Color>();
                int sizeRegion = 0;
                foreach (var point in map.Value)
                {
                    try
                    {
                        Color c = bmp.GetPixel(point.X, point.Y);
                        bmp.SetPixel(point.X, point.Y, Color.ForestGreen);
                        colors.Add(c);
                        sizeRegion++;
                        
                    }
                    catch (Exception e) {
                    }
                    //Util.l("  -- " + point.X + "x" + point.Y+ " "+c.Name);
                }
                GetColorFromSpectrumBar(colors, sizeRegion);
            }
            //View.FormMain.pictureBoxModel.Image = bmp;
        }

        private void GetColorFromSpectrumBar(List<Color> list, int sizeRegion)
        {
            Dictionary<string,int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();
            float sumValues = 0;
            foreach (var c in list)
                if (Util.spektrumSrazky.ContainsKey(c.Name))
                {
                    if (counts.ContainsKey(c.Name))
                        counts[c.Name]++;
                    else
                        counts[c.Name]=1;
                    sumValues += Util.spektrumSrazky[c.Name];
                }

            foreach (var c in counts)
                Util.l($" + nalezeno {c.Key}: {c.Value}x");
            Util.l($" - region size: {sizeRegion}");
            Util.l($" - průměrná hodnota regionu: {sumValues/sizeRegion}");
        }

        private void LoadORP()
        {
            /*
            View.FormMain.pictureBoxModel.Image = Properties.Resources.ORP;
            View.FormMain.pictureBoxModel.Width = Properties.Resources.ORP.Width;
            View.FormMain.pictureBoxModel.Height = Properties.Resources.ORP.Height;
            View.FormMain.pictureBoxModel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseDown);
            View.FormMain.pictureBoxModel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseMove);
            View.FormMain.pictureBoxModel.MouseUp+= new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseUp);
            */
            
        }
        
        private void pictureBoxORP_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            currentX = e.X;
            currentY = e.Y;
        }

        private void pictureBoxORP_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (isDragging)
            {
                View.FormMain.pictureBoxModel.Top = View.FormMain.pictureBoxModel.Top + (e.Y - currentY);
                View.FormMain.pictureBoxModel.Left = View.FormMain.pictureBoxModel.Left + (e.X - currentX);
            }*/
        }
        private void pictureBoxORP_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
        
        private void LoadModel()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"models\Model_ALADIN_CZ\");

            Util.l(path);
            string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var filename in files)
            
                {
                    
                    try
                    {
                        bmp = new Bitmap(filename);
                    /*
                        View.FormMain.pictureBoxModel.Image = bmp;
                    View.FormMain.pictureBoxModel.Width = bmp.Width;
                    View.FormMain.pictureBoxModel.Height = bmp.Height;
                    */
                    }
                    catch (Exception e)
                    {
                    }
                }

        }
    }
}
