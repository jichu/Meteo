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
        private string path;
        private PreImage mapORP;
        public Bitmap bmp;
        private string typeStupnice;
        private List<CloudModelSpectrum> cloudModelSpectrum;

        public Images() {
        }

        public Images(string path)
        {
            this.path = path;
            this.typeStupnice = "DEFAULT";
            LoadImage(path);
            LoadPointsOfColorsInMap();
        }

        public Images(SourceImage si, bool onlyEnumeration = false)
        {
            this.path = si.Path;
            Util.curModelName = si.Model;
            Util.curSubmodelName = si.Submodel;
            this.typeStupnice = si.Type;
            LoadImage(path);
            LoadPointsOfColorsInMap(onlyEnumeration);
        }

        private void LoadPointsOfColorsInMap(bool onlyEnumeration=false)
        {
            string options = Model.Cloud.MODELSGetModelOptions(Util.curModelName, Util.curSubmodelName);
            if (options == string.Empty)
            {
                Util.l($"Neexistující model {Util.curModelName}:{Util.curSubmodelName}|Chyba modelu");
                return;
            }

            JObject jo = JObject.Parse(options);
            var p = jo.Property("countMethod");

            if(!onlyEnumeration)
                DefaultUserControlModel();

            try
            {
                Util.rainRegion.Clear();
                Util.rainRegionValue.Clear();
                Util.curDataOutputs.Clear();
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
                            cloudModelSpectrum = Model.Cloud.ModelSpectrumGetScaleForModels(Util.curModelName, Util.curSubmodelName, typeStupnice);

                            switch (p.Value.ToString()) {
                                default:
                                case "sum":
                                    Util.curCountMethod = "suma";
                                    value = (float)GetValueFromSpectrumBar(colors, sizeRegion);
                                    break;
                                case "average":
                                    Util.curCountMethod = "průměr";
                                    value = GetValueFromSpectrumBarAverage(colors, sizeRegion);
                                    break;
                                case "average_raw":
                                    Util.curCountMethod = "průměr (raw)";
                                    value = (float)GetValueFromSpectrumBarAverageRaw(colors, sizeRegion);
                                    break;
                                case "majority":
                                    Util.curCountMethod = "majorita";
                                    value = GetValueFromSpectrumBarMajority(colors, sizeRegion);
                                    break;
                            }
                        }
                        else
                        {
                            Util.l($"Chybí specifikace metody, nastavte v {Util.curModelName}/{Util.pathSource["model_cfg"]}|Chyba modelu");
                            return;
                        }

                        if (onlyEnumeration)
                        {
                            //Util.l($"CloudInputData({Util.curModelName},{Util.curSubmodelName},{regionName},{Path.GetFileNameWithoutExtension(path)},{value})");

                            CloudInputData inputORP = new CloudInputData(Util.curModelName, Util.curSubmodelName, regionName, Path.GetFileNameWithoutExtension(path), value, typeStupnice);
                         
                            Model.Cloud.InputDataInsertOrUpdate(inputORP); //INPUT_DATA - DON'T TOUCH!!!! 
                        }
                        else
                        {
                            if (value >= 1.0)
                            {
                                int x = 0, y = 0, count = 0;
                                foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                                {
                                    x += (int)point[0];
                                    y += (int)point[1];
                                    count++;
                                }
                                Util.rainRegion.Add(regionName, (new Point((int)Math.Round((float)x / count), (int)Math.Round((float)y / count))));
                                Util.rainRegionValue.Add(value);
                            }
                            Util.curDataOutputs.Add(new DataOutput()
                            {
                                RegionName = regionName,
                                Value = value,
                                Color = ColorTranslator.FromHtml(map.color)
                            });
                        }
                    }
                }


                if (!onlyEnumeration)
                    DefaultUserControlModelReady();

            } catch(Exception e) { Util.l(e); }

        }

        private int GetValueFromSpectrumBar(List<Color> list, int sizeRegion)
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
            float value = sumValues / sizeRegion;
            if (value < 0.25)
                value = 0;
            if (value>=0.25 && value < 0.75)
                value = 0.5f;
            if (value >= 0.75 && value <1.25)
                value = 1;
            if (value >= 1.25 && value < 1.75)
                value = 1.5f;
            if (value >= 1.75 && value < 2.25)
                value = 2;
            if (value >= 2.25 && value < 2.75)
                value = 2.5f;
            if (value >= 2.75 && value <=3)
                value = 3;
            if(typeStupnice!="REAL")
                if (value >= 3)
                    value = 3;
            Util.curModelOutput += $" - průměrná hodnota regionu: {sumValues / sizeRegion} ~ {value}" + Environment.NewLine+Environment.NewLine;
            return value;
        }

        private float GetValueFromSpectrumBarAverageRaw(List<Color> list, int sizeRegion)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            Dictionary<string, int> values = new Dictionary<string, int>();

            float sumValues = 0;
            foreach (var c in list)
                foreach (var r in cloudModelSpectrum)
                {
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
                Util.curModelOutput += $" + nalezeno {c.Key}: {c.Value}x" + Environment.NewLine;
            }
            float value = sumValues / sizeRegion;
            Util.curModelOutput += $" - průměrná hodnota regionu: {sumValues / sizeRegion} ~ {value}" + Environment.NewLine + Environment.NewLine;
            return value;
        }

        private float GetValueFromSpectrumBarMajority(List<Color> list, int sizeRegion)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();

            float max = 0;
            string maxColor = "";
            foreach (var c in list)
                foreach (var r in cloudModelSpectrum)
                {
                    if (r.color.Replace("#", "ff") == c.Name)
                    {
                        if (counts.ContainsKey(c.Name))
                            counts[c.Name]++;
                        else
                            counts[c.Name] = 1;
                    }
                }
            foreach (var c in counts)
            {
                //Util.l($" + nalezeno {c.Key}: {c.Value}x");
                if (c.Value > max)
                {
                    max = c.Value;
                    maxColor = c.Key;
                }
                Util.curModelOutput += $" + nalezeno {c.Key}: {c.Value}x" + Environment.NewLine;
            }
            float rank = 0;
            foreach (var r in cloudModelSpectrum)
            {
                if (r.color.Replace("#", "ff") == maxColor)
                {
                    rank = r.rank;
                    break;
                }
            }

            Util.curModelOutput += $" - majoritní hodnota regionu [{maxColor}]: {rank}" + Environment.NewLine + Environment.NewLine;
            return rank;
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
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["labelCountMethod"] as Label).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["labelCountMethod"] as Label).Text = "CountMethod: -";
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
            (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["labelCountMethod"] as Label).BeginInvoke((Action)(() =>
            {
                (View.FormMain.panelLayout.Controls["UserControlModel"].Controls["labelCountMethod"] as Label).Text = "CountMethod: "+ Util.curCountMethod;
            }));
        }

    }
}
