using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudOutputData
    {
        public int id_orp { get; set; }
        public string sample_name { get; set; }
        public float value { get; set; }
        public int id_out { get; set; }       

        public CloudOutputData()
        {
            value = 0;
        }

        public CloudOutputData(int id_orp, string sample_name, float value, int id_out=0)
        {
            this.id_orp=id_orp;
            this.sample_name = sample_name;
            this.value = value;
            this.id_out = id_out;
        }
    }
}
