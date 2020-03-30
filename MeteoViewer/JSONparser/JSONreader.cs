using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.JSONparser
{
    internal static class JSONreader
    {
        internal static string PathJson { get; set; } = "export";
        private static string ext = ".json";
        private static JObject JData;

        internal static async Task<JObject> LoadJson(string filename)
        {
            if (filename == string.Empty)
                return null;
            return await Task.Run(() => ReadFromFile(filename));
        }

        internal static async Task<JObject> LoadJsonRoot(string filename = "root")
        {
            return await Task.Run(() => ReadFromFile(filename));
        }

        internal async static Task Do(string name = "")
        {
            await Task.Run(() => ReadFromFile(name));
        }

        private static JObject ReadFromFile(string name)
        {
            try
            {
                using (StreamReader file = File.OpenText(@Path.Combine(PathJson, name+ext)))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JData = (JObject)JToken.ReadFrom(reader);
                }
                return JData;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e, "ReadFromFile");
                return null;
            }
        }
    }
}
