using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudORPS
    {
        public int id { get; set; }
        public string name { get; set; }

        public CloudORPS() {
        }
        public CloudORPS(string id, string name)
        {
            this.id = Int32.Parse(id);
            this.name = name;
            
        }
    }

}
