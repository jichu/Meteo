using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public static class Util
    {
        //public static Dictionary<Color, List<Point>> mapCR = new Dictionary<Color, List<Point>>();
        public static List<string> spektrumRadar = new List<string>() { "fffcfcfc", "ffa00000", "fffc0000", "fffc5800", "fffc8400", "fffcb000", "ffe0dc00", "ff9cdc00", "ff34d800", "ff00bc00", "ff00a000", "ff006cc0", "ff0000fc", "ff3000a8", "ff380070" };
        public static Dictionary<string, string> pathSource = new Dictionary<string, string>
        {
            { "config", @".\config\" },
            { "models", @".\models\" },
            { "model_cfg", @"model.cfg" },
            { "symbol_rain", @".\images\symbol_rain.png" },
            { "symbol_storm", @".\images\symbol_storm.png" },
            { "symbol_cloud", @".\images\symbol_cloud.png" }
        };

        public static List<CloudORPS> ORPSGetORPNames { get; set; }
        public static List<CloudORPColor> ORPColorGetORPColors { get; set; }

        public static Dictionary<int, string> palette = new Dictionary<int, string>();
        public static Dictionary<string, Point> rainRegion = new Dictionary<string, Point>();
        public static List<float> rainRegionValue = new List<float>();

        public static bool Develop = true;

        public static string curModelName { get; set; }
        public static string curSubmodelName { get; set; }
        public static string curCountMethod { get; set; }
        public static string curModelOutput { get; set; }
        public static List<DataOutput> curDataOutputs = new List<DataOutput>();

        public static string ExceptionText = "Exception";
        public static char logMessageDelimiter = '|';

        public static void ShowLoading(string message)
        {
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
                return;
            View.FormLoader = new FormLoader(message);
            View.FormLoader.TopMost = true;
            View.FormLoader.StartPosition = FormStartPosition.CenterScreen;
            View.FormLoader.Show();
            View.FormLoader.Refresh();
            Thread.Sleep(500);
            Application.Idle += OnLoaded;
        }

        private static void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            View.FormLoader.Close();
        }
        
        public static string GetRegionNameByColor(string regioncolor)
        {
            if (Model.Cloud.MODELSGetNumberOfAreasForModel(curModelName) > 14)
            {
                if (ORPColorGetORPColors.Any(s => s.color.Trim() == regioncolor))
                    return ORPSGetORPNames.First(i => i.id == ORPColorGetORPColors.First(s => s.color.Trim() == regioncolor).id_orp).name;
                else
                    return "";
            }
            else
                return Model.Cloud.REGIONSGetNameFromColor(regioncolor);
        }

        public static void l(object obj, Dictionary<string, object> logOptions = null )
        {
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "toFile", false },
                { "filename", "log.txt" },
                { "clearFile", false},
                { "messageBoxIcon", MessageBoxIcon.Warning}
            };

            if (logOptions != null)
                foreach (var item in logOptions) {
                    if (options.ContainsKey(item.Key)) options[item.Key] = item.Value;
                }
            
            Console.WriteLine(obj);
            if (obj.GetType().ToString().Contains(ExceptionText) || (bool) options["toFile"])
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter((string) options["filename"], !((bool) options["clearFile"])))
                    {
                        file.WriteLine(obj);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            if (obj.ToString().Contains(logMessageDelimiter))
                MessageBox.Show(obj.ToString().Split(logMessageDelimiter)[0], obj.ToString().Split(logMessageDelimiter)[1], MessageBoxButtons.OK, (MessageBoxIcon) options["messageBoxIcon"]);
        }

        private static void LoadSetting(string fileName)
        {
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
                    }
                    reader.Close();
                }
            }
            catch (Exception e) { Util.l(e); }
        }
    }

}
