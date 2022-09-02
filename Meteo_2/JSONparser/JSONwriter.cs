using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo.JSONparser
{
    internal static class JSONwriter
    {
        internal static string PathJson { get; set; } = "export";
        private static string fienamePostfix = "";
        private static string format = "yyMMdd_HHmmss";
        private static string ext = ".json";
        private static JObject JData;

        internal static string CreateJsonFilename(string name = "")
        {
            CreatePath();
            if (name == string.Empty)
                name = $"{DateTime.Now.ToString(format)}{fienamePostfix}{ext}";
            else
            name += ext;
            return Path.Combine(PathJson, name);
        }
        internal static void Add(string prop, JArray values)
        {
            if (JData == null) JData = new JObject();
            JData.Add(
                new JProperty(prop, values)
            );
        }
        internal static void Clear()
        {
            JData = null;
        }

        internal static void CreateJson(JObject data = null, string fPostfix = "")
        {
            fienamePostfix = fPostfix;
            //_ = Do(data ?? JData, "");
            Do(data ?? JData, "");
        }
        internal static void CreateJsonRoot(JObject data = null, string filename = "root")
        {
            // _ = Do(data ?? JData, filename);
            Do(data ?? JData, filename);
        }

        /*internal async static Task Do(dynamic data, string name = "")
        {
            await Task.Run(() => SaveToFile(data, name));
        }*/
        internal static void Do(dynamic data, string name = "")
        {
            SaveToFile(data, name);
        }

        private static void CreatePath()
        {
            if (!Directory.Exists(PathJson))
                Directory.CreateDirectory(PathJson);
        }

        private static bool SaveToFile(dynamic data, string filename)
        {
            try
            {
                using (StreamWriter file = File.CreateText(JSONwriter.CreateJsonFilename(filename)))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    data.WriteTo(writer);
                }
                /*
                CreatePath();
                using (StreamWriter file = File.CreateText(Path.Combine(PathJson, filename)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, data);
                }
                */
                return true;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e, "SaveToFile");
                return false;
            }
        }
    }
}
