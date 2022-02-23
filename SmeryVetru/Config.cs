﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmeryVetru
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        [JsonProperty]
        public JArray ORP { get; set; } = new JArray();
        [JsonProperty]
        public bool Debug { get; set; } = false;
        [JsonProperty]
        public int Delay { get; set; } = 600;
        [JsonProperty]
        public string OutputFile { get; set; } = "__.json";
        [JsonProperty]
        public JArray Time { get; set; } = new JArray() { 4, 11 };

        private static string file = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json";

        public Config()
        {
            SetDefault();
        }

        private void SetDefault()
        {
            ORP.Add(new JObject(
                new JProperty("name", "Zlín"),
                new JProperty("url", "https://www.windguru.cz/343224")
                ));
        }

        /*public static void Load(string filenameConfig = "")
        {
            file = filenameConfig == "" ? file : filenameConfig;
            if (!File.Exists(file)) return;
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                MainForm.Config = JsonConvert.DeserializeObject<Config>(@json);

            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(MainForm.Config, Formatting.Indented);
            System.IO.File.WriteAllText(@file, json);
        }*/
    }
}
