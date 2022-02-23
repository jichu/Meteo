using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudSettings
    {
        public string option_name { get; set; }
        public string option_value { get; set; }

        public CloudSettings() {
        }

        public CloudSettings(string option_name, string option_value)
        {
            this.option_name = option_name;
            this.option_value = option_value;
        }

    }
}
