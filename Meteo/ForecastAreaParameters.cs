using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class ForecastAreaParameters
    {
        public int id_orp { get; set; }
        public string name_orp { get; set; }
        public string sampleName { get; set; }
        public Dictionary<string, float> parameters { get; set; }

        public ForecastAreaParameters() {
                
        }

        public ForecastAreaParameters(CloudORPS ORP, string sampleName) {
            name_orp = ORP.name;
            id_orp = ORP.id;
            this.sampleName = sampleName;
                
        }
    }
}
