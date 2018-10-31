using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class ForecastAreaParameters
    {
        public int Id_orp { get; set; }
        public string Name_orp { get; set; }
        public string SampleName { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new Dictionary<string, float>();

        public ForecastAreaParameters() {
                
        }

        public ForecastAreaParameters(CloudORPS ORP, string sampleName) {
            Name_orp = ORP.name;
            Id_orp = ORP.id;
            this.SampleName = sampleName;

            //Hodnoty parametrů - zatím sebrané ze vzorové předpovědi (hodnoty pro uherské hradiště), posléze se bude tahat z DB (Input_DATA)
            Parameters.Add("MLCAPE", 1);
            Parameters.Add("LI", 1);
            Parameters.Add("MLCIN", 0);
            Parameters.Add("TT index", 1);
            Parameters.Add("KI", 1);
            Parameters.Add("GRAD 850-500 hPa", 1);
            Parameters.Add("WETBULB", 1);
            Parameters.Add("FRONTOGENEZE 850 hPa", 1);
            Parameters.Add("GRAD 925-700 hPa", 1);
            Parameters.Add("MXR", 1);
            Parameters.Add("MOCON", 1);
            Parameters.Add("MFDIV 0-1 km", 1);
            Parameters.Add("MTV VECTOR", 1);
            Parameters.Add("POTENTIAL OROGRAPHIC LIFITING", 1);
            Parameters.Add("RH 1000 hPa", 2);
            Parameters.Add("RH 925 hPa", 1);
            Parameters.Add("RH 850 hPa", 2);
            Parameters.Add("RH 700 hPa", 1.5f);
            Parameters.Add("RH 500 hPa", 0);
            Parameters.Add("RH 300 hPa", 0);
            Parameters.Add("RV 850 hPa", 1);
            Parameters.Add("RV 500 hPa", 1);
            Parameters.Add("RV 300 hPa", 1);
            Parameters.Add("Pwater", 1);
            Parameters.Add("T 850 hPa", 1);
            Parameters.Add("DLS", 1);
            Parameters.Add("LLS", 1);
            Parameters.Add("SREH 3 km", 0);
            Parameters.Add("SREH 1 km", 0);
            Parameters.Add("SWEAT", 1);
            Parameters.Add("Rychlost větru v 300 hPa", 0);
            Parameters.Add("Rychlost větru v 850 hPa", 1);
            Parameters.Add("MCS VEKTOR", 2);
            //Parameters.Add("1000 hPa", JV);
            //Parameters.Add("925 hPa", JZ);
            //Parameters.Add("850 hPa", JZ);
            //Parameters.Add("700 hPa", JZ);
            //Parameters.Add("600 hPa", JZ);
            //Parameters.Add("500 hPa", JZ);
            //Parameters.Add("400 hPa", JZ);
            //Parameters.Add("300 hPa", JZ);
            Parameters.Add("850 hPa", 8);
            Parameters.Add("700 hPa", 7);
            Parameters.Add("600 hPa", 10);
            Parameters.Add("500 hPa", 8);
            Parameters.Add("400 hPa", 8);
            Parameters.Add("300 hPa", 18);
            Parameters.Add("LCL", 1);
            Parameters.Add("Nulová izoterma (km)", 1);
            Parameters.Add("Hloubka teplé fáze oblaku (km)", 1);
            Parameters.Add("SHIP", 1);
            Parameters.Add("DTHE", 0);
            Parameters.Add("SBCAPE 0-2 km (J/kg) - den", 1);
            Parameters.Add("STP", 0);
            Parameters.Add("Tlak MSLP", 1);
            Parameters.Add("Teplota (MAX)", 2);
            Parameters.Add("Oblačnost", 1);
            Parameters.Add("Rychlost větru v 10 m nad terénem v m/s", 1);
            Parameters.Add("RH 2 m (%)", 2);
            //Parameters.Add("Směr větru v hladině 700 hPa", JZ);
            Parameters.Add("MXR (Směšovací poměr)", 1);
            Parameters.Add("KONV+/DIV- (0-1 km)", 2);
            Parameters.Add("OROGRAPHIC LIFT", 2);
            Parameters.Add("Intenzita bouřek (SIVS) Staniční srážkoměry", 0);
            Parameters.Add("Staniční srážkoměry CHMU+interpolace stanic", 0);
            Parameters.Add("Interpolace (radary+srážkoměry)", 0);
            Parameters.Add("Stupeň nasycení", 0);
            Parameters.Add("Suma srážek (1.hod.)", 0);
            Parameters.Add("Srážky ALADIN", 0);
            Parameters.Add("Srážky GEM", 0);
            Parameters.Add("Srážky EURO4", 0);
            Parameters.Add("Srážky HIRLAM", 0);
            Parameters.Add("Srážky WRF-NMM", 0);
            Parameters.Add("Srážky WRF-ARW", 0);
            
        }
    }
}
