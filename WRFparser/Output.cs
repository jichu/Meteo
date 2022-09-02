using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRFparser
{
    public class Output
    {
        public Dictionary<string, List<string>> DicData { get; set; } = new Dictionary<string, List<string>>();
        public JArray JsonData = new JArray();
        public bool ErrorTimeout { get; set; } = false;
        public long ExecutionTime { get; set; } = 0;
        public List<string> ErrorDataNull = new List<string>();
    }
}
