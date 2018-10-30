using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class Controller
    {
        public Controller()
        {

            Util.ORPSGetORPNames = Model.Cloud.ORPSGetORPNames();
            Util.ORPColorGetORPColors = Model.Cloud.ORPColorGetORPColors();

            new Images();
            new Ji();
            new StormEngine();
        }
    }
}
