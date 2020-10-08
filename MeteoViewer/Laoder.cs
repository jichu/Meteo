using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer
{
    class Laoder
    {
        public Laoder()
        {
            Config.Manage.Load();
        }
    }
}
