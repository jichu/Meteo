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
            mapORP = new PreImage();
            LoadModel();
            //LoadORP();
            LoadPointsOfColorsInMap();

        }

        private void LoadPointsOfColorsInMap()
        {

            foreach (var map in mapORP.getMapORP())
            {
                Util.l(map.Key + ":");
                List<Color> colors = new List<Color>();
                foreach (var point in map.Value)
                {
                    Color c = bmp.GetPixel(point.X, point.Y);
                    bmp.SetPixel(point.X, point.Y, Color.FromArgb(0,Color.ForestGreen));
                    colors.Add(c);
                    //Util.l("  -- " + point.X + "x" + point.Y+ " "+c.Name);
                }
                GetColorFromSpectrumBar(colors);
            }
            View.FormMain.pictureBoxModel.Image = bmp;
        }

        private void GetColorFromSpectrumBar(List<Color> list)
        {
            foreach (var c in list)
                if (Util.spektrumRadar.Contains(c.Name)) {
                    Util.l(c.Name);
                }
            ;

        }

        private void LoadORP()
        {
            View.FormMain.pictureBoxORP.Image = Properties.Resources.ORP;
            View.FormMain.pictureBoxORP.Width = Properties.Resources.ORP.Width;
            View.FormMain.pictureBoxORP.Height = Properties.Resources.ORP.Height;
            View.FormMain.pictureBoxORP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseDown);
            View.FormMain.pictureBoxORP.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseMove);
            View.FormMain.pictureBoxORP.MouseUp+= new System.Windows.Forms.MouseEventHandler(this.pictureBoxORP_MouseUp);
        }

        private void pictureBoxORP_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            currentX = e.X;
            currentY = e.Y;
        }

        private void pictureBoxORP_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                View.FormMain.pictureBoxORP.Top = View.FormMain.pictureBoxORP.Top + (e.Y - currentY);
                View.FormMain.pictureBoxORP.Left = View.FormMain.pictureBoxORP.Left + (e.X - currentX);
            }
        }
        private void pictureBoxORP_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void LoadModel()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"models\1_Radarove_snimky\");

            Util.l(path);
            string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var filename in files)
            
                {
                    
                    try
                    {
                        bmp = new Bitmap(filename);
                        View.FormMain.pictureBoxModel.Image = bmp;
                    View.FormMain.pictureBoxModel.Width = bmp.Width;
                    View.FormMain.pictureBoxModel.Height = bmp.Height;
                    }
                    catch (Exception e)
                    {
                    }
                }

        }
    }
}
