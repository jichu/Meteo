using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    internal class DataInput
    {
        public string ModelName { get; set; }
        public Dictionary <string, List<DataSpectrum> > SubmodelSpectrum { get; set; }
        public List<DataMask> Mask { get; set; }
    }

    internal class DataMask
    {
        public string Color { get; set; }
        public string Coods { get; set; }
    }

    internal class DataSpectrum
    {
        public string Color { get; set; }
        public string Rank { get; set; }
        public string Type { get; set; }
    }
}
