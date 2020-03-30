using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Data
{
    internal static class Cache
    {
        internal static string PathJson { get; set; } = "export";
        internal static string JsonDataFormat = "yyMMdd_HHmmss";
        internal static int indexOutputlist { get; set; } = 0;
        internal static int indexHour { get; set; } = 0;
        internal static bool Redraw { get; set; } = true;
    }
}
