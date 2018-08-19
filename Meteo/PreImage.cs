using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Meteo
{
    class PreImage
    {
        private Dictionary<string, List<Point>> mapORP=new Dictionary<string, List<Point>>();
        private List<CloudORPS> ORPSGetORPNames;
        private List<CloudORPColor> ORPColorGetORPColors;

        public PreImage()
        {
		}

        public Dictionary<string, List<Point>> getMapORP() {
            return mapORP;
        }


        public void LoadORPfromModels()
        {
            try
            {
                ORPSGetORPNames = Model.Cloud.ORPSGetORPNames();
                ORPColorGetORPColors = Model.Cloud.ORPColorGetORPColors();
                string dirPath = Util.pathSource["models"];
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    string orpMask = dir+@"\"+ model + ".bmp";
                    Util.ShowLoading("Načítání a ukládání ORP masek do databáze...");
                    if (File.Exists(orpMask))
                    {
                        Util.l("Load mask: " + orpMask);
                        Thread t = new Thread(() => LoadORP((Bitmap)Image.FromFile(orpMask),model));
                        t.Start();
                    }

                    List<string> subdirs = new List<string>(Directory.EnumerateDirectories(dirPath + "\\" + model));
                    Model.Cloud.MODELSInsertOrUpdate(new CloudModels(model));
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        //Util.l(model + " " + submodel);
                        Model.Cloud.MODELSInsertOrUpdate(new CloudModels(submodel, model, JsonConvert.SerializeObject(LoadConfig(dir+@"\"+ Util.pathSource["model_cfg"]))));
                    }
                }
                //View.FormMain.Close();
            }
            catch(Exception e)
            {
                Util.l(e);
            }
        }

        private string GetRegionNameByColor(string regioncolor)
        {
            if (ORPColorGetORPColors.Any(s => s.color.Trim() == regioncolor))
                return ORPSGetORPNames.First(i => i.id == ORPColorGetORPColors.First(s => s.color.Trim() == regioncolor).id_orp).name;
            else
                return "";
        }

        private void LoadORP(Bitmap orp, string modelName)
        {
            try
            {

                var mapCR =
                     from x in Enumerable.Range(0, orp.Width - 1)
                     from y in Enumerable.Range(0, orp.Height - 1)
                     select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

                mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));

                Dictionary<string, JArray> data = new Dictionary<string, JArray>();
                foreach (var map in mapCR)
                {
                    if (data.ContainsKey(map.color.Name))
                    {
                        JArray array = data[map.color.Name];
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data[map.color.Name] = array;
                    }
                    else
                    {
                        JArray array = new JArray();
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data.Add(map.color.Name, array);
                    }
                }

                //Chu.data = data;
                foreach (var map in data)
                {
                    string regionName = GetRegionNameByColor("#" + map.Key.Substring(2, 6));
                    //Util.l(regionName+JsonConvert.SerializeObject("#"+map.Key.Substring(2,6))+": "+JsonConvert.SerializeObject(map.Value));
                    //Chu.color = JsonConvert.SerializeObject(map.Key);
                    //Chu.coords = JsonConvert.SerializeObject(map.Value);
                    if (regionName != "")
                        Model.Cloud.MaskSpectrumInsertOrUpdate(new CloudMaskSpectrum(modelName, regionName, "#" + map.Key.Substring(2, 6), JsonConvert.SerializeObject(map.Value)));
                    
                }
            }
            catch (Exception ex)
            {
                Util.l(ex.ToString());
            }
        }

        private JObject LoadConfig(string fileName)
        {
            JObject jo = new JObject();
            try
            {
                if (File.Exists(fileName))
                {
                    StreamReader reader = new StreamReader(fileName);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == null) continue;
                        if (line[0].ToString() == "#") continue;
                        if (line.IndexOf('=') == -1) continue;
                        string[] item = line.Split('=');
                        if (item.Length > 2) continue;
                        string value = item[1];
                        string key = item[0];
                        jo.Add(new JProperty(key, value));
                    }
                    reader.Close();
                }
            } catch(Exception e) { Util.l(e); }
            return jo;
        }




        private void button1_Click(object sender, EventArgs e)
		{
            /*
			Thread t = new Thread(new ThreadStart(LoadORP));
			t.Start();
            */
        }

    }
}
