using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudOutput
    {
        public string nameOrp { get; set; }
        public string sampleName { get; set; }
        public Dictionary<string, string> output { get; set; } = new Dictionary<string, string>();

        public CloudOutput(){

        }

        public CloudOutput(string nameOrp, string sampleName, Dictionary<string, string> output) {
            this.nameOrp = nameOrp;
            this.sampleName = sampleName;
            this.output = output;
        } 
        


        
    }
}
