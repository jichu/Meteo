using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    static class Chu
    {
        public static string color { get; set; }
        public static string coords { get; set; }
        public static Dictionary<string, JArray> data = new Dictionary<string, JArray>();
    }
}
