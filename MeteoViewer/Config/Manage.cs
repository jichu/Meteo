using MeteoViewer.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Config
{
    public class Manage
    {
        private static string file = System.Diagnostics.Process.GetCurrentProcess().ProcessName+".json";

        public static void Load(string filenameConfig="")
        {
            file = filenameConfig == "" ? file : filenameConfig;
            if (!File.Exists(file)) return;
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                Cache.Config = JsonConvert.DeserializeObject<Config.MeteoViewer>(@json);

            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Data.Cache.Config, Formatting.Indented);
            System.IO.File.WriteAllText(@file, json);
        }
    }
}
