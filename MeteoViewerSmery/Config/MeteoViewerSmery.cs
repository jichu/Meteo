using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewerSmery.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class MeteoViewerSmery
    {
        [JsonProperty]
        public JArray DataType { get; set; } = new JArray();
        [JsonProperty]
        public int DataTypeSelected { get; set; } = 0;

        public MeteoViewerSmery()
        {
            SetDefault();
        }

        private void SetDefault()
        {
            DataType.Add(new JObject(
                new JProperty("list", "mainoutputlist"),
                new JProperty("data", "maindata"),
                new JProperty("tag", "Hlavní")
                ));

            DataType.Add(new JObject(
                new JProperty("list", "secondaryoutputlist"),
                new JProperty("data", "secondarydata"),
                new JProperty("tag", "Vedlejší")
                ));
        }

        internal string GetDataType(string type)
        {
            try
            {
                return DataType[DataTypeSelected][type].ToString();
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return string.Empty;
            }
        }
    }
}
