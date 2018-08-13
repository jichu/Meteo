using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudMaskSpectrum
    {
        public int id { get; set; }
        public int id_orp { get; set; }
        public string color { get; set; }
        public string coods { get; set; }

        public void ShowRecord() {
            Util.l("id: "+id + " id_orp: "+ id_orp + " Color: " + color + " Coods: " + coods);
        }
        public CloudMaskSpectrum() {

        }
        public CloudMaskSpectrum(int id, int id_orp, string color, string coods)
        {

            this.color = color;
            this.coods = coods;
            this.id_orp = id_orp;
            this.id = id;
            

        }

        public CloudMaskSpectrum(string id, string id_orp, string color, string coods) {

            this.color = color;
            this.coods = coods;
            this.id_orp = Model.Cloud.ORPSGetIDFromName(id_orp);
            this.id = Model.Cloud.MODELSGetIDFromName(id);
            

        }
    }

    
}
