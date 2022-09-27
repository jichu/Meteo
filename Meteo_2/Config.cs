using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        [JsonProperty]
        public bool Debug { get; set; } = true;
        [JsonProperty]
        public string PathExportOutputData { get; set; } = "webview/data/";
        [JsonProperty]
        public string PathExportOutputDataArchives { get; set; } = "archives/data/";

        private static string file = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".config.json";

        public Config()
        {
        }

        public static void Load(string filenameConfig = "")
        {
            file = filenameConfig == "" ? file : filenameConfig;
            if (!File.Exists(file)) return;
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                FormMain.Config = JsonConvert.DeserializeObject<Config>(@json);
            }
        }
        public static void Save()
        {
            string json = JsonConvert.SerializeObject(FormMain.Config, Formatting.Indented);
            System.IO.File.WriteAllText(@file, json);
        }
    }
}
