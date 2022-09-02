using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewerSmery.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Data
    {
        [JsonProperty]
        public JArray Map { get; set; } = new JArray();

        public Data()
        {
        }

    }
}
