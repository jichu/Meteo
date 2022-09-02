using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewerSmery
{
    class Loader
    {
        public Loader()
        {
            Config.Manage.LoadData();
        }
    }
}
