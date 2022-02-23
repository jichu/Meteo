using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudSamples
    {
        public string sample_name { get; set; }
        
        public List<CloudORPS> ORPS { get; set; } = Model.Cloud.ORPSGetORPNames();

        public int windDirection { get; set; } = Util.windDirectionToInt["J"];  //J(4), JZ(5) || ostatni 0-3, 6-7
        public CloudSamples() {
        
        }

        public void LoadORPS() {
            foreach (var orp in ORPS) {
                orp.LoadData(sample_name);
            }
        }
    }
}
