using Newtonsoft.Json.Linq;
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
    class Images : Form
    {
        private bool isDragging;
        private int currentX;
        private int currentY;
        private PreImage mapORP;
        public Bitmap bmp;

        public Images() {
            /*
            mapORP = new PreImage();
            mapORP.LoadORPfromModels();
            LoadModel();
            LoadORP();
            LoadPointsOfColorsInMap();*/

        }

        public Images(string path)
        {
            LoadImage(path);
            LoadPointsOfColorsInMap();
        }

        private void LoadPointsOfColorsInMap()
        {
            bool checkBoxShowORPchecked = (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"]as CheckBox).Checked;

            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Clear();
            }));

            if(Chu.coords==null)
                MessageBox.Show("Nemáte načtenou ORP masku.", "Debug mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            foreach (var map in Chu.data)
            {
                Util.l("Region: "+map.Key + ":");
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
                {
                    (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Text += "Region: " + map.Key + ":\n";
                }));
                List<Color> colors = new List<Color>();
                int sizeRegion = 0;
                foreach (var point in map.Value)
                {
                    try
                    {
                        Color c = bmp.GetPixel((int)point[0], (int)point[1]);
                        if(checkBoxShowORPchecked)
                            bmp.SetPixel((int)point[0], (int)point[1], System.Drawing.ColorTranslator.FromHtml("#"+map.Key.Substring(2,6)));
                        colors.Add(c);
                        sizeRegion++;
                        
                    }
                    catch (Exception e) {
                        Util.l(e);
                    }
                    //Util.l("  -- " + point.X + "x" + point.Y+ " "+c.Name);
                }
                GetColorFromSpectrumBar(colors, sizeRegion);
            }
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["pictureBoxMap"] as PictureBox).Image = bmp;
        }

        private void GetColorFromSpectrumBar(List<Color> list, int sizeRegion)
        {
            Dictionary<string,int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();
            List<CloudModelSpectrum> records = Model.Cloud.ModelSpectrumGetScaleForModels("Model_ALADIN_CZ", "Srážky_MAIN");
            float sumValues = 0;


            foreach (var c in list)
                foreach (var r in records) {
                    if (r.color.Replace("#", "ff") == c.Name)
                        {
                            if (counts.ContainsKey(c.Name))
                                counts[c.Name]++;
                            else
                                counts[c.Name] = 1;
                            sumValues += r.rank;
                        }
                }

            string output = "";
            foreach (var c in counts)
            {
                Util.l($" + nalezeno {c.Key}: {c.Value}x");
                output += $" + nalezeno {c.Key}: {c.Value}x"+Environment.NewLine;
            }
            Util.l($" - region size: {sizeRegion}");
            Util.l($" - sum value: {sumValues}");

            int n = sumValues > 1 ? 1 : 0;
            output += $" - hodnota regionu: {n}" + Environment.NewLine+Environment.NewLine;

            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Text += output;
            }));
        }

        private void GetColorFromSpectrumBarAverage(List<Color> list, int sizeRegion)
        {
            Dictionary<string,int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();
            List<CloudModelSpectrum> records = Model.Cloud.ModelSpectrumGetScaleForModel("Model_ALADIN_CZ");
            float sumValues = 0;


            foreach (var c in list)
                foreach (var r in records) {
                    if (r.color.Replace("#", "ff") == c.Name)
                        {
                            if (counts.ContainsKey(c.Name))
                                counts[c.Name]++;
                            else
                                counts[c.Name] = 1;
                            sumValues += r.rank;
                        }
                }

            string output = "";
            foreach (var c in counts)
            {
                Util.l($" + nalezeno {c.Key}: {c.Value}x");
                output += $" + nalezeno {c.Key}: {c.Value}x"+Environment.NewLine;
            }
            Util.l($" - region size: {sizeRegion}");
            Util.l($" - sum value: {sumValues}");
            Util.l($" - průměrná hodnota regionu: {sumValues/sizeRegion}");
            output += $" - průměrná hodnota regionu: {sumValues / sizeRegion}" + Environment.NewLine+Environment.NewLine;


            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Text += output;
            }));
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
        
        private void LoadImage(string path)
        {
            try
            {
                Bitmap bmp1 =  (Bitmap)Image.FromFile(path);
                bmp = new Bitmap(bmp1, new Size(bmp1.Width, bmp1.Height));
            }
            catch (Exception e)
            {
                Util.l(e);
            }
        }
    }
}
