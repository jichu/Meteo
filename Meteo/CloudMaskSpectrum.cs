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
        public int id_model { get; set; }
        public int id_orp { get; set; }
        public string Color { get; set; }
        public string Coods { get; set; }

        public void ShowRecord() {
            Util.l("id: "+id + " id_model: " + id_model + " id_orp: "+ id_orp + " Color: " + Color + " Coods: " + Coods);
        }
    }

    
}
