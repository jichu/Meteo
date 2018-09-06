using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudModelSpectrum
    {
        public int id_model { get; set; }
        public float rank { get; set; }
        public string color { get; set; }
        public int type { get; set; }

        public CloudModelSpectrum() {

        }

        public CloudModelSpectrum(int id_model, string rank, string color, string type)
        {
            this.id_model = id_model;
            this.rank = float.Parse(rank);
            this.color = color;
            this.type = Int32.Parse(type);
        }

        //deprecated
        public CloudModelSpectrum(string id_model, string rank, string color)
        {
            this.id_model = Int32.Parse(id_model);
            this.rank = float.Parse(rank);
            this.color = color;
        }
    }
}
