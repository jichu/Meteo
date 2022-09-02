using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRFparser
{
    internal class Timeout
    {
        public int tryCount { get; set; } = 0;
        public int tryCountMax { get; set; } = 100;
    }
}
