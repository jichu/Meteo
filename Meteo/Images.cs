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

            JObject jo = JObject.Parse(Model.Cloud.MODELSGetModelOptions(Util.curModelName, Util.curSubmodelName));
            var p = jo.Property("countMethod");

            DefaultUserControlModel();

            try
            {
                Util.rainRegion.Clear();
                Util.curModelOutput = "";
                foreach (var map in Util.ORPColorGetORPColors)
                {
                    string regionName = Util.GetRegionNameByColor(map.color);
                    Util.curModelOutput += regionName == "" ? map.color : regionName + Environment.NewLine;
                    List<CloudMaskSpectrum> cms = Model.Cloud.MaskSpectrumGetCoodsByColor(map.color.Trim(), Util.curModelName);
                    string coods = cms.Count > 0 ? cms.First().coods : "";
                    if (coods != "")
                    {
                        List<Color> colors = new List<Color>();
                        int sizeRegion = 0;
                        foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                        {
                            Color c = bmp.GetPixel((int)point[0], (int)point[1]);
                            colors.Add(c);
                            sizeRegion++;
                        }

                        float value = 0;
                        if (p != null)
                        {
                            switch (p.Value.ToString()) {
                                default:
                                case "sum":
                                    value = (float)GetValueFromSpectrumBar(colors, sizeRegion);
                                    break;
                                case "average":
                                    value = GetValueFromSpectrumBarAverage(colors, sizeRegion);
                                    break;
                            }
                        }
                        else
                        {
                            Util.l($"Chybí specifikace metody, nastavte v {Util.curModelName}/{Util.pathSource["model_cfg"]}|Chyba modelu");
                            return; 
                        }

                        if (value >= 1.0)
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

                DefaultUserControlModelReady();

            } catch(Exception e) { Util.l(e); }

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

            foreach (var c in counts)
            {
                //Util.l($" + nalezeno {c.Key}: {c.Value}x");
                Util.curModelOutput += $" + nalezeno {c.Key}: {c.Value}x"+Environment.NewLine;
            }

            int n = sumValues > 1 ? 1 : 0;
            Util.curModelOutput += $" - hodnota regionu: {n}" + Environment.NewLine+Environment.NewLine;
            return n;
        }

        private float GetValueFromSpectrumBarAverage(List<Color> list, int sizeRegion)
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
            Util.l(sumValues);
            foreach (var c in counts)
            {
                //Util.l($" + nalezeno {c.Key}: {c.Value}x");
                Util.curModelOutput += $" + nalezeno {c.Key}: {c.Value}x"+Environment.NewLine;
            }

            Util.curModelOutput += $" - průměrná hodnota regionu: {sumValues / sizeRegion}" + Environment.NewLine+Environment.NewLine;
            return sumValues;
        }
                
        private void pictureBoxORP_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            currentX = e.X;
            currentY = e.Y;
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

        private void DefaultUserControlModel()
        {
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
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowOutput"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowOutput"] as CheckBox).Enabled = false;
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowOutput"] as CheckBox).Checked = false;
            }));
        }

        private void DefaultUserControlModelReady()
        {
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShoweRain"] as CheckBox).Enabled = true;
            }));
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowORP"] as CheckBox).Enabled = true;
            }));
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowOutput"] as CheckBox).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["checkBoxShowOutput"] as CheckBox).Enabled = true;
            }));
        }

    }
}
