using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo.JSONparser
{
    internal static class JDataRoot
    {
        public static JArray orpname { get; set; }
        public static JArray paramname { get; set; }
        public static JArray hourname { get; set; }
    }
}
