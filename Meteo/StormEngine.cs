using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class StormEngine
    {
        List<CloudORPS> ORPS = Model.Cloud.ORPSGetORPNames();
        List<ForecastAreaParameters> fapList = new List<ForecastAreaParameters>();
        public string sampleName { get; set; }
        
        public StormEngine()
        {
            //Util.l("Algoritmus pro výpočet bouřek");
            //
        }

        public StormEngine(string sampleName) {
            this.sampleName = sampleName;
            foreach (var ORP in ORPS) {
                ForecastAreaParameters fap = new ForecastAreaParameters(ORP, this.sampleName);
                fapList.Add(fap);
            }
            Algorithm();


        }
        //Zde se bude počítat předpověď na základě parametrů
        public void Algorithm() {
            //Util.l($"{fapList.First().Parameters["MLCAPE"]}");
        }


    }
}
