using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meteo
{
    class StormEngine
    {
        List<CloudORPS> ORPS = Model.Cloud.ORPSGetORPNames();
        List<ForecastAreaParameters> fapList = new List<ForecastAreaParameters>();
        public string sampleName { get; set; }
        public List<string> sampleNames = new List<string>{
           /* "00",
            "03",
            "06",
            "09",
            "12",
            "15",*/
            "18",
            "21",
        };

        public StormEngine()
        {
            Thread t = new Thread(() => Run());
            t.Start();
        }

        public StormEngine(string sampleName) {
            this.sampleName = sampleName;
           
                foreach (var ORP in ORPS)
                {
                    ForecastAreaParameters fap = new ForecastAreaParameters(ORP, this.sampleName);
                    fapList.Add(fap);
                }
            
            Output();
        }

        public void Run() {
            foreach (var s in sampleNames)
            {
                Util.l($"Počítám předpověď pro {s}. hodinu.");
                foreach (var ORP in ORPS)
                {
                    ForecastAreaParameters fap = new ForecastAreaParameters(ORP, s);
                    fapList.Add(fap);
                }
            }
            Output();

        }

        //Zde se bude počítat předpověď na základě parametrů
        public void Output() {
            foreach (var f in fapList)
            {
                CloudOutputData mainOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["1. RIZIKO PŘÍVALOVÉ POVODNĚ"], Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutput);
                CloudOutputData stormIntensityOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Util.algorithmOutput["PŘEDPOVĚĎ INTENZITY BOUŘÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(stormIntensityOutput);
                CloudOutputData precipitationPlaceOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["MÍSTO VÝSKYTU BOUŘEK"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceOutput);
                CloudOutputData precipitationPlaceKoefOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK - KOEFICIENT"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceKoefOutput);
                CloudOutputData torrentialRainOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["MÍSTO VÝSKYTU - PŘÍVALOVÉ SRÁŽKY"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(torrentialRainOutput);
                CloudOutputData strongWindscreensOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutput);
                CloudOutputData hailOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["MÍSTO VÝSKYTU - KRUPOBITÍ"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA KRUPOBITÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(hailOutput);
                CloudOutputData supercelarTornadosOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["MÍSTO VÝSKYTU - SUPERCELÁRNÍ TORNÁDA"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(supercelarTornadosOutput);
                if (f.Output.ContainsKey("SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST"))
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(f.id_orp, f.sampleName, f.Output["SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }
                else
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(f.id_orp, f.sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }
            }

        }


    }
}
