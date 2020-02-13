using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo.JSONparser
{
    class JSONwriter
    {
        internal static string PathJson { get; set; } = "export";
        private static string format = "yyMMdd_HHmmss";
        private static string ext = ".json";

        internal async Task Do()
        {
            string filename = "nnn" + ext;
            List<object> data = new List<object>();
            /*
            data.Add(new object() {
                key=1
            });*/
            await Task.Run(() => SaveToFile(filename, data));
        }
        private static void CreatePath()
        {
            if (!Directory.Exists(PathJson))
                Directory.CreateDirectory(PathJson);
        }

        private bool SaveToFile(string filename, List<object> data)
        {
            try
            {
                using (StreamWriter file = File.CreateText(Path.Combine(PathJson, filename)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, data);
                }
                return true;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e, this.GetType().Name);
                return false;
            }
        }
    }
}
