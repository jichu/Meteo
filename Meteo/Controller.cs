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
            new StormEngine("09");//Přes parametr se předává, pro který čas se má výpočet provést //Zatím jen pro testování 06
            new Ji();
        }
    }
}
