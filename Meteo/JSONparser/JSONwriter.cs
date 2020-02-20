using Newtonsoft.Json;
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
        private static string format = "yyMMdd_HHmmss";
        private static string ext = ".json";

        internal async static Task Do(List<object> data, string name="")
        {
            if (name == string.Empty)
                name = $"{DateTime.Now.ToString(format)}{ext}";
            else
                name += ext;
            await Task.Run(() => SaveToFile(data, name));
        }
        private static void CreatePath()
        {
            if (!Directory.Exists(PathJson))
                Directory.CreateDirectory(PathJson);
        }

        private static bool SaveToFile(List<object> data, string filename)
        {
            try
            {
                CreatePath();
                using (StreamWriter file = File.CreateText(Path.Combine(PathJson, filename)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, data);
                }
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
