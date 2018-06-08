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

        public Images() {
            LoadModel();
            LoadORP();

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
                    Bitmap bmp = null;
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
