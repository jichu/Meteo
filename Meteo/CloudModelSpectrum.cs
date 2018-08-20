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
        public int rank { get; set; }
        public string color { get; set; }

        public CloudModelSpectrum() {

        }

        public CloudModelSpectrum(string id_model, string rank, string color)
        {
            this.id_model = Int32.Parse(id_model);
            this.rank = Int32.Parse(rank);
            this.color = color;
        }
    }
}
