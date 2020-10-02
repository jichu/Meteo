using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Config
{
    internal class MeteoViewer
    {
        public JArray DataType { get; set; } = new JArray();
        public int DataTypeSelected { get; set; } = 1;

        public MeteoViewer()
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
