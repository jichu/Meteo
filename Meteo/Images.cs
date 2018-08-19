using Newtonsoft.Json;
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
        private List<CloudModelSpectrum> cloudModelSpectrum;
        private List<CloudORPS> ORPSGetORPNames;
        private List<CloudORPColor> ORPColorGetORPColors;

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

        private string GetRegionNameByColor(string regioncolor)
        {
            if (ORPColorGetORPColors.Any(s => s.color.Trim() == regioncolor))
                return ORPSGetORPNames.First(i => i.id == ORPColorGetORPColors.First(s => s.color.Trim() == regioncolor).id_orp).name;
            else
                return regioncolor;
        }


        private void LoadPointsOfColorsInMap()
        {
            ORPSGetORPNames = Model.Cloud.ORPSGetORPNames();
            ORPColorGetORPColors = Model.Cloud.ORPColorGetORPColors();

            bool checkBoxShowORPchecked = (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"]as CheckBox).Checked;

            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Clear();
            }));
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).Enabled = false;
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).Checked = false;
            }));
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).Enabled = false;
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).Checked = false;
            }));

            try
            {
                Util.rainRegion.Clear();
                foreach (var map in ORPColorGetORPColors)
                {
                    List<CloudMaskSpectrum> cms = Model.Cloud.MaskSpectrumGetCoodsByColor(map.color.Trim());
                    string coods = cms.Count > 0 ? cms.First().coods : "";
                    if (coods == "")
                    {
                        //Util.l("Nemáte načtenou ORP masku.|Debug mode");
                       // return;
                    }
                    else
                    {
                        string regionName = GetRegionNameByColor(map.color);
                        //Util.l("Region: " + GetRegionNameByColor(map.color)+map.color);
                        (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
                        {
                            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Text += regionName+":\n";
                        }));
                        //Util.l(coods);
                        List<Color> colors = new List<Color>();
                        int sizeRegion = 0;
                        foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                        {
                            Color c = bmp.GetPixel((int)point[0], (int)point[1]);
                            /*
                            if (checkBoxShowORPchecked)
                                bmp.SetPixel((int)point[0], (int)point[1], ColorTranslator.FromHtml(map.color));
                                */
                            colors.Add(c);
                            sizeRegion++;
                        }
                        int value = GetValueFromSpectrumBar(colors, sizeRegion);
                        if (value == 1)
                        {
                            int x = 0, y =0, count=0;
                            foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                            {
                                x += (int)point[0];
                                y += (int)point[1];
                                count++;
                            }
                            Util.rainRegion.Add(regionName,(new Point((int)Math.Round((float)x/count), (int)Math.Round((float)y / count))));
                        }

                    }
                }
                //(View.FormMain.panelLayout.Controls["UserControlModel"].Controls["pictureBoxMap"] as PictureBox).Image = bmp;

                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).BeginInvoke((Action)(() =>
                {
                    (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).Enabled=true;
                }));
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).BeginInvoke((Action)(() =>
                {
                    (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).Enabled = true;
                }));
            } catch(Exception e) { Util.l(e); }

            /*
            foreach (var map in Chu.data)
            {
                Util.l("Region: " + GetRegionNameByColor("#" + map.Key.Substring(2, 6)));
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).BeginInvoke((Action)(() =>
                {
                    (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["richTextBoxOutput"] as RichTextBox).Text += GetRegionNameByColor("#" + map.Key.Substring(2, 6)).Trim() + ":\n";
                }));
                List<Color> colors = new List<Color>();
                int sizeRegion = 0;
                foreach (var point in map.Value)
                {
                    try
                    {
                        Color c = bmp.GetPixel((int)point[0], (int)point[1]);
                        if (checkBoxShowORPchecked)
                            bmp.SetPixel((int)point[0], (int)point[1], System.Drawing.ColorTranslator.FromHtml("#" + map.Key.Substring(2, 6)));
                        colors.Add(c);
                        sizeRegion++;

                    }
                    catch (Exception e)
                    {
                        Util.l(e);
                    }
                    //Util.l("  -- " + point.X + "x" + point.Y+ " "+c.Name);
                }
                GetColorFromSpectrumBar(colors, sizeRegion);
            }
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["pictureBoxMap"] as PictureBox).Image = bmp;
            */
        }

        private int GetValueFromSpectrumBar(List<Color> list, int sizeRegion)
        {
            Dictionary<string,int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();

            cloudModelSpectrum = Model.Cloud.ModelSpectrumGetScaleForModels(Util.curModelName, Util.curSubmodelName);
            float sumValues = 0;


            foreach (var c in list)
                foreach (var r in cloudModelSpectrum) {
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

            return n;
        }

        private void GetValueFromSpectrumBarAvarage(List<Color> list, int sizeRegion)
        {
            Dictionary<string,int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();
            float sumValues = 0;


            foreach (var c in list)
                foreach (var r in cloudModelSpectrum) {
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
