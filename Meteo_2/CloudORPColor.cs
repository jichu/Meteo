using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudORPColor
    {
        public int id_orp { get; set; }
        public string color { get; set; }

        public CloudORPColor()
        {
        }
        public CloudORPColor(string id_orp, string color)
        {
            this.id_orp = Int32.Parse(id_orp);
            this.color = color;

        }
    }
}
