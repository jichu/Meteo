using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Data
{
    internal static class Stream
    {
        internal static JObject JRoot { get; set; }
        internal static JObject JData { get; set; }
        internal static void Update()
        {

        }
    }
}
