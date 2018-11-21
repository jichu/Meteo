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
        public Dictionary<string, float> Output { get; set; } = new Dictionary<string, float>();
        private List<float> LevelScale = new List<float>() {0.25f, 0.5f, 0.75f, 1.0f };
        private List<float> OroKonvScale = new List<float>() { 0.17f, 0.33f, 0.66f, 1.0f };
        private List<float> RHLevelsAlternative = new List<float>() { 0.1f, 0.3f, 0.4f, 1.0f }; //??? Není jisté, zda je tato stupnice dobře
        private const int RATIO = 3;

        public ForecastAreaParameters() {
                
        }

        public ForecastAreaParameters(CloudORPS ORP, string sampleName) {
            Name_orp = ORP.name;
            Id_orp = ORP.id;
            this.SampleName = sampleName;
            DoCountOperations();  
            
        }
        private void DoCountOperations() {
            Util.l("Počítám jednotlivé kroky algoritmu pro: " + Name_orp);
            LoadParameters();
            PrecipitationPlace();
            RelativeHumidity();
            DayInstability();
            TriggeringConvection();
            WriteOutputLog();

        }

        private void LoadParameters() {
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
            //Parameters.Add("Směr větru v hladině 700 hPa", JZ);
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
            Parameters.Add("MXR (Směšovací poměr)", 1);
            Parameters.Add("KONV+/DIV- (0-1 km)", 2);
            Parameters.Add("OROGRAPHIC LIFT", 2);
            Parameters.Add("Intenzita bouřek (SIVS) Staniční srážkoměry", 0);
            Parameters.Add("Staniční srážkoměry CHMU+interpolace stanic", 0);
            Parameters.Add("Interpolace (radary+srážkoměry)", 0);
            Parameters.Add("Stupeň nasycení", 0);
            Parameters.Add("Suma srážek (1.hod.)", 0);
            Parameters.Add("Srážky ALADIN", 0);
            Parameters.Add("Srážky GDPS", 0);
            Parameters.Add("Srážky EURO4", 0);
            Parameters.Add("Srážky HIRLAM", 0);
            Parameters.Add("Srážky WRF-NMM", 0);
            Parameters.Add("Srážky WRF-ARW", 0);
        }
        //Spouštění konvekce
        private void TriggeringConvection() {
            List<float> values = new List<float>() {Parameters["Tlak MSLP"], Parameters["FRONTOGENEZE 850 hPa"]};
            List<float> orographicSupportValues = new List<float>() { Parameters["GRAD 925-700 hPa"], Parameters["MXR"] };
            List<float> konvDivValues = new List<float>() { Parameters["MOCON"], Parameters["MFDIV 0-1 km"] };
            List<float> oroOutputValues = new List<float>() { Parameters["MTV VECTOR"], Parameters["POTENTIAL OROGRAPHIC LIFITING"] };
            int KonvDiv = ValueToLevel(OroKonvScale, SumArray(konvDivValues) / (konvDivValues.Count * RATIO));
            orographicSupportValues.Add(KonvDiv);
            int OrographicLift = ValueToLevel(OroKonvScale,SumArray(oroOutputValues) / (oroOutputValues.Count * RATIO));
            orographicSupportValues.Add(OrographicLift);
            int OrographicSupport = ValueToLevel(LevelScale, SumArray(orographicSupportValues) / (orographicSupportValues.Count * RATIO));
            values.Add(OrographicSupport);
            values.Add(Output["RH 1000-300 hPa"]);
            int triggeringConv = ValueToLevel(LevelScale, SumArray(values) / (values.Count * RATIO));
            Output.Add("SPOUŠTĚNÍ KONVEKCE",triggeringConv);
        }


        //Denní instabilita
        private void DayInstability() {
            List<float> values = new List<float>() {Parameters["MLCAPE"], Parameters["LI"], Parameters["MLCIN"]
                , Parameters["TT index"], Parameters["KI"], Parameters["GRAD 850-500 hPa"], Parameters["WETBULB"]  };
            float probabilityMLCAPELI = SumArray(values,0,2) / (2 * RATIO);
            int MLCAPELI = ValueToLevel(LevelScale, probabilityMLCAPELI);
            float probability = (SumArray(values, 2) + MLCAPELI) / ((values.Count - 1) * RATIO);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("DENNÍ INSTABILITA ATMOSFÉRY", level);

        }

        //Relativní vlhkost
        private void RelativeHumidity() {
            List<float> values = new List<float>(){Parameters["RH 1000 hPa"], Parameters["RH 925 hPa"], Parameters["RH 850 hPa"]
                , Parameters["RH 700 hPa"], Parameters["RH 500 hPa"], Parameters["RH 300 hPa"] };
            float probability = SumArray(values) / (values.Count* RATIO);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 1000-300 hPa", level);

            probability = SumArray(values,2) / ((values.Count-2) * RATIO);
            level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 850-300 hPa", level);

            probability = SumArray(values,0,3) / ((values.Count-3) * RATIO);
            level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 1000-850 hPa", level);
            //Na základě nějaké podmínky, se bude lišit RHLevels

        }
        //Pravděpodobnost srážek
        private void PrecipitationPlace() {
            float probability=0;
            List<float> values = new List<float>(){Parameters["Srážky ALADIN"], Parameters["Srážky GDPS"], Parameters["Srážky EURO4"]
                , Parameters["Srážky HIRLAM"], Parameters["Srážky WRF-NMM"], Parameters["Srážky WRF-ARW"] };
            probability = SumArray(values) / values.Count;
            Output.Add("Pravděpodobnost Srážek", probability);
        }

        //Převod hodnoty pravděpodobnosti na úroveň dle tabulky
        private int ValueToLevel(List<float> levels, float value) {
            int level=-1;
            foreach (var l in levels) {

                level = (value < l) ? levels.IndexOf(l) : -1;
                if (level != -1) break;
            }
            return level;
        }

        //Suma prvků v poli
        private float SumArray(List<float> arr, int start = 0, int end = -1) {
            float sum = 0;
            end = (end > arr.Count) ? arr.Count : end;
            start = (start > arr.Count) ? 0 : start;
            List<float> array = arr.GetRange(start, (end == -1) ? (arr.Count)-start : end-start);
            foreach (var a in array) {
                sum += a;
                //Util.l(a);
            }
            return sum;
        }

        //Výpis výstupu
        private void WriteOutputLog() {
            foreach(var item in Output)
            {
                Util.l(item.Key + ":" + item.Value);
            }
        }
    }
}
