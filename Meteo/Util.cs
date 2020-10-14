using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            { "masks", @".\config\masks\" },
            { "models", @".\models\" },
            { "model_cfg", @"model.cfg" },
            { "scales",   @".\config\scales\"},
            { "symbol_rain", @".\images\symbol_rain.png" },
            { "symbol_storm", @".\images\symbol_storm.png" },
            { "symbol_cloud", @".\images\symbol_cloud.png" },
            { "map_output_background", @".\images\map_output_background.png" },
            { "wrf_mask", @".\images\wrf_mask.png" },
            { "output_legend", @".\images\output_legend_[ID].png" }
        };

        public static Dictionary<string, int> algorithmOutput = new Dictionary<string, int>
        {
            { "PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ", 0 },
            { "PŘEDPOVĚĎ INTENZITY BOUŘÍ", 1 },
            { "PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)", 2 },
            { "PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)", 3 },
            { "PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ", 4 },
            { "PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST", 5 },
            { "PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST", 6 },
            { "PŘEDPOVĚD RIZIKA KRUPOBITÍ",  7},
            { "PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD", 8 },
            { "PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - SUCHÁ VARIANTA", 9}, 
            { "PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - VLHKÁ VARIANTA", 10}
        };

        public static List<string> pathSymbolsAlgorithmOutput = new List<string>()
        {
            @".\images\symbols\riziko přívalové povodně [ID].png",
            @".\images\symbols\intenzita bouří [ID].png",
            @".\images\symbols\pravděpodobnost konvektivních srážek [ID].png",
            @".\images\symbols\pravděpodobnost konvektivních srážek [ID].png",
            @".\images\symbols\riziko přívalového deště [ID].png",
            @".\images\symbols\riziko silných nárazů větru [ID].png",
            @".\images\symbols\riziko silných nárazů větru [ID].png",
            @".\images\symbols\riziko krupobití [ID].png",
            @".\images\symbols\riziko tornád [ID].png",
            @".\images\symbols\riziko přívalové povodně [ID].png"
        };

        public static Dictionary<string, int> windDirectionToInt = new Dictionary<string, int>
        {
            { "S", 0 },
            { "SV", 1 },
            { "V", 2 },
            { "JV", 3 },
            { "J", 4 },
            { "JZ", 5 },
            { "Z", 6 },
            { "SZ",  7}
        };

        public static Dictionary<int, string> windDirectionToString = new Dictionary<int, string>
        {
            { 0, "S" },
            { 1, "SV" },
            { 2, "V" },
            { 3, "JV" },
            { 4, "J" },
            { 5, "JZ" },
            { 6, "Z"},
            { 7, "SZ"}
        };
        public static List<CloudOutput> outputDataCache { get; set; } = new List<CloudOutput>();
        public static List<CloudORPS> ORPSGetORPNames { get; set; }
        public static List<CloudORPColor> ORPColorGetORPColors { get; set; }

        public static List<string> modelsDir = new List<string>();
        public static Dictionary<int, string> palette = new Dictionary<int, string>();
        public static Dictionary<string, Point> rainRegion = new Dictionary<string, Point>();
        public static List<float> rainRegionValue = new List<float>();

        public static bool Develop = true;

        public static string curModelDir { get; set; }
        public static string curModelName { get; set; }
        public static string curSubmodelName { get; set; }
        public static string curCountMethod { get; set; }
        public static string curModelOutput { get; set; }
        public static List<DataOutput> curDataOutputs = new List<DataOutput>();

        public static string ExceptionText = "Exception";
        public static char logMessageDelimiter = '|';
        private static Stopwatch watch;

        public static void ShowLoading(string message, string info="", bool selfClose=true)
        {
            Preloader.ShowAuto(message, info, selfClose);
        }
        public static void HideLoading()
        {
            Preloader.Hide();
        }
        
        public static string GetRegionNameByColorForLoading(string regioncolor) {
            if (ORPColorGetORPColors.Any(s => s.color.Trim() == regioncolor))
                return ORPSGetORPNames.First(i => i.id == ORPColorGetORPColors.First(s => s.color.Trim() == regioncolor).id_orp).name;
            else
            {
                //Util.l($"{curModelName}: {regioncolor}");
                return "";
            }
        }


        public static string GetRegionNameByColor(string regioncolor)
        {
            if (ORPColorGetORPColors.Any(s => s.color.Trim() == regioncolor))
                if (Model.Cloud.MODELSGetNumberOfAreasForModel(curModelName) > 14)
                {
                    return ORPSGetORPNames.First(i => i.id == ORPColorGetORPColors.First(s => s.color.Trim() == regioncolor).id_orp).name;
                }
                else
                {
                    return Model.Cloud.REGIONSGetNameFromColor(regioncolor);
                }
            else
            {
                return "";
            }

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
            if (obj == null) return;
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

        internal static string GetModelDate()
        {
            Console.WriteLine(curModelDir);
            return curModelDir;
        }

        public static string GetSettings(string item)
        {
            foreach (var s in Model.Cloud.SETTINGSGetSettings())
                if (s.option_name == item)
                    return s.option_value;
            return null;
        }

        public static void SetSettings(string key, string value)
        {
            Model.Cloud.SETTINGSInsertOrUpdate(new CloudSettings(key, value));
        }

        public static void StartWatch()
        {
            watch = Stopwatch.StartNew();
        }
        public static void StopWatch(string msg = "")
        {
            watch.Stop();
            Console.WriteLine($"{msg} v čase {watch.ElapsedMilliseconds}ms");
        }

    }

}
