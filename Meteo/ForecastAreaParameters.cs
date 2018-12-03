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
        public Dictionary<string, List<CloudInputData>> PrecipitationModels { get; set; } = new Dictionary<string, List<CloudInputData>>();
        public Dictionary<string, float> Output { get; set; } = new Dictionary<string, float>();
        private List<float> LevelScale = new List<float>() { 0.25f, 0.5f, 0.75f, 1.0f };
        private List<float> HumidityInfluencesScale = new List<float>() { 0.22f, 0.33f, 0.67f, 1.0f };
        private List<float> OroKonvScale = new List<float>() { 0.17f, 0.33f, 0.67f, 1.0f };
        private List<float> DangerousPhenomenaScale = new List<float>() { 0.33f, 0.5f, 0.75f, 1.0f };
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
            Parameters.Add("Srážky GFS", 0);


            //Pokusná data pro výpočet času srážek
            List<CloudInputData> precipitationData = new List<CloudInputData>();
            CloudInputData firstInterval = new CloudInputData();
            CloudInputData secondInterval = new CloudInputData(1);
            CloudInputData thirdInterval = new CloudInputData();
            CloudInputData fourthInterval = new CloudInputData(1);
            CloudInputData fifthInterval = new CloudInputData();
            CloudInputData sixthInterval = new CloudInputData();
            CloudInputData seventhInterval = new CloudInputData(1);
            CloudInputData eighthInterval = new CloudInputData();

            precipitationData.Add(firstInterval);
            precipitationData.Add(secondInterval);
            precipitationData.Add(thirdInterval);
            precipitationData.Add(fourthInterval);
            precipitationData.Add(fifthInterval);
            precipitationData.Add(sixthInterval);
            precipitationData.Add(seventhInterval);
            precipitationData.Add(eighthInterval);

            List<CloudInputData> precipitationData2 = new List<CloudInputData>();
            CloudInputData firstInterval2 = new CloudInputData(1);
            CloudInputData secondInterval2 = new CloudInputData(1);
            CloudInputData thirdInterval2 = new CloudInputData(1);
            CloudInputData fourthInterval2 = new CloudInputData(1);
            CloudInputData fifthInterval2 = new CloudInputData(1);
            CloudInputData sixthInterval2 = new CloudInputData(1);
            CloudInputData seventhInterval2 = new CloudInputData(1);
            CloudInputData eighthInterval2 = new CloudInputData(1);

            precipitationData2.Add(firstInterval2);
            precipitationData2.Add(secondInterval2);
            precipitationData2.Add(thirdInterval2);
            precipitationData2.Add(fourthInterval2);
            precipitationData2.Add(fifthInterval2);
            precipitationData2.Add(sixthInterval2);
            precipitationData2.Add(seventhInterval2);
            precipitationData2.Add(eighthInterval2);

            PrecipitationModels.Add("Srážky ALADIN", precipitationData);
            PrecipitationModels.Add("Srážky GDPS", precipitationData2);
            PrecipitationModels.Add("Srážky EURO4", precipitationData);
            PrecipitationModels.Add("Srážky HIRLAM", precipitationData2);
            PrecipitationModels.Add("Srážky HIRLAM Starý", precipitationData);
            PrecipitationModels.Add("Srážky WRF-NMM", precipitationData2);
            PrecipitationModels.Add("Srážky WRF-ARW", precipitationData);
            PrecipitationModels.Add("Srážky GFS", precipitationData2);
            PrecipitationModels.Add("Srážky GFS Starý", precipitationData);
        }

        private void DoCountOperations()
        {
            Util.l("\n--------------------------------------\n" + "Počítám jednotlivé kroky algoritmu pro: " + Name_orp);
            LoadParameters();
            PrecipitationTime();
            PrecipitationPlace();
            RelativeHumidity();
            DayInstability();
            TriggeringConvection();
            SupportOfConvection();
            WindCut();
            SupportOfDangerousPhenomenaCreation();
            ConvectiveStormOrganization();
            StormMoving();
            IntensityOfStrongStormsDay();
            TorrentialRain();
            DayHailStorm();
            StrongWindImpact();
            SupercelarTornados();
            TemperatureInfluencesOfEarthSurface();
            WindInfluences();
            WindEffect();
            WriteOutputLog();

        }

        //7. Lokální předpověď
        //Teplotní vlivy zemského povrchu
        private void TemperatureInfluencesOfEarthSurface() {
            List<float> values = new List<float>() { Parameters["Oblačnost"] };
            int level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("TEPLOTNÍ VLIVY ZEMSKÉHO POVRCHU", level);
        }

        //Větrné vlivy
        private void WindInfluences()
        {
            List<float> values = new List<float>() { Parameters["Rychlost větru v 10 m nad terénem v m/s"] };
            int level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("VĚTRNÉ VLIVY", level);
        }

        //Vlhkostní vlivy
        private void HumidityInfluences()
        {
            List<float> values = new List<float>() { Parameters["RH 2 m (%)"], Parameters["KONV+/DIV- (0-1 km)"] };
            List<float> previousDayValues = new List<float>() { Parameters["Intenzita bouřek (SIVS) Staniční srážkoměry"], Parameters["Staniční srážkoměry CHMU+interpolace stanic"], Parameters["Interpolace (radary+srážkoměry)"] };
            int previousDayLevel = ValueToLevel(HumidityInfluencesScale, Probability(previousDayValues));
            values.Add(previousDayLevel);
            int level = ValueToLevel(HumidityInfluencesScale, Probability(values));
            Output.Add("VLHKOSTNÍ VLIVY", level);
        }

        //Návětrný + závětrný efekt
        private void WindEffect()
        {
            List<float> windwardValues = new List<float>() { Parameters["GRAD 925-700 hPa"], Parameters["MXR (Směšovací poměr)"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["OROGRAPHIC LIFT"], Parameters["Rychlost větru v 850 hPa"] };
            int windwardLevel = ValueToLevel(LevelScale, Probability(windwardValues));

            List<float> leeValues = new List<float>() { Parameters["GRAD 850-500 hPa"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["Rychlost větru v 10 m nad terénem v m/s"] };
            int leeLevel = ValueToLevel(LevelScale, Probability(leeValues));

            List<float> windEffectValues = new List<float>();
            windEffectValues.Add(windwardLevel);
            windEffectValues.Add(leeLevel);
            int windEffectLevel = ValueToLevel(OroKonvScale, Probability(windEffectValues));

            Output.Add("JZ,J,JV proudění", windEffectLevel);

        }

        //Návětrný efekt



        //Riziko výskytu nebezpečných jevů
        private int DangerousPhenomenaCount(List<float> weights, List<float> values) {
            List<float> parameterValues = new List<float>();
            //Multikriteriální hodnocení
            int multiCrit = ValueToLevel(LevelScale, ProbabilityWeights(values, weights));
            //Intenzita předpovědních faktorů
            int factorIntensity = ValueToLevel(LevelScale, Probability(values));

            //Výpočet parametru
            parameterValues.Add(multiCrit);
            parameterValues.Add(factorIntensity);
            int parameter = ValueToLevel(DangerousPhenomenaScale, Probability(parameterValues));

            return parameter;
        }

        //Přívalové srážky
        private void TorrentialRain()
        {
            List<float> weights = new List<float>() { 3, 2, 1, 2, 1, 2, 3, 3, 3 };
            List<float> values = new List<float>() { Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["MXR"], Parameters["LCL"], Parameters["Rychlost větru v 850 hPa"], Parameters["Hloubka teplé fáze oblaku (km)"], Parameters["MCS VEKTOR"], Parameters["DLS"], Output["ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa"] };
            Output.Add("PŘÍVALOVÉ SRÁŽKY", DangerousPhenomenaCount(weights, values));
        }

        //Den - Krupobití
        private void DayHailStorm()
        {
            List<float> weights = new List<float>() { 3, 1, 2, 3, 3, 3, 1, 2, 3, 2, 3};
            List<float> values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };
            Output.Add("DEN - KRUPOBITÍ", DangerousPhenomenaCount(weights, values));
        }

        //DEN - Silné nárazy větru
        private void StrongWindImpact()
        {
            List<float> weights = new List<float>() { 3, 2, 2, 3, 3, 2, 2, 1, 1, 2, 3 };
            List<float> values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
            Output.Add("DEN - SILNÉ NÁRAZY VĚTRU", DangerousPhenomenaCount(weights, values));
        }

        //Supercelární tornáda
        private void SupercelarTornados()
        {
            List<float> weights = new List<float>() { 3, 3, 2, 2, 2, 3, 1, 3};
            List<float> values = new List<float>() { Parameters["SBCAPE 0-2 km (J/kg) - den"], Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["LCL"],
                Parameters["STP"]};
            Output.Add("DEN - SUPERCELÁRNÍ TORNÁDA", DangerousPhenomenaCount(weights, values));
        }



        //Intenzita silných bouřek přes den
        private void IntensityOfStrongStormsDay()
        {
            List<float> values = new List<float>() { Output["DENNÍ INSTABILITA ATMOSFÉRY"], Output["STŘIH VĚTRU"], Output["POHYB BOUŘE"] };
            List<float> complexSupportTriggeringConvectionValues = new List<float>() { Output["SPOUŠTĚNÍ KONVEKCE"], Output["PODPORA KONVEKCE"], Output["PODPORA VZNIKU NEBEZPEČNÝCH JEVŮ"], Output["ORGANIZACE KONV. BOUŘE"], Output["POHYB BOUŘE"] };
            int complexSupportTriggeringConvection = ValueToLevel(LevelScale, Probability(complexSupportTriggeringConvectionValues));
            values.Add(complexSupportTriggeringConvection);
            int stormIntensityDay = ValueToLevel(LevelScale, Probability(values));
            int stormIntensityDay2 = ValueTest(values);
            Output.Add("INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN)", stormIntensityDay);
            Output.Add("INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2", stormIntensityDay2);
        }


        //Pohyb bouře
        //Zatím se nepočítá ze všeho, protože z obrazového vstupu tuto zatím nelze zjistit požadovaná data.
        private void StormMoving()
        {
            

            Output.Add("ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa", 1); //

            //Vektor pohybu bouře
            List<float> windSpeedValues = new List<float>() { Parameters["850 hPa"], Parameters["700 hPa"], Parameters["600 hPa"], Parameters["500 hPa"], Parameters["400 hPa"], Parameters["300 hPa"] };
            float avg=0;
            foreach (var item in windSpeedValues) {
                avg += item;
            }
            avg /= windSpeedValues.Count;
            float stormVector = 2 * avg - windSpeedValues.First();

            float stormVectorLevel = 0;
            if (stormVector <= 3) stormVectorLevel = 3;
            else if (stormVector <= 9) stormVectorLevel = 2;
            else if (stormVector <= 15) stormVectorLevel = 1;
            else stormVectorLevel = 0;

            List<float> values = new List<float>() { Parameters["MCS VEKTOR"]};
            values.Add(stormVectorLevel);

            int stormMovement = ValueToLevel(LevelScale, Probability(values));
            stormMovement = (stormMovement == 0 || stormMovement == 1) ? 0 : 1;

            Output.Add("POHYB BOUŘE", stormMovement);
        }

        //Organizace konv. bouře
        private void ConvectiveStormOrganization()
        {
            List<float> values = new List<float>() { Parameters["Rychlost větru v 300 hPa"], Parameters["Rychlost větru v 850 hPa"] };
            int convStormOrg = ValueToLevel(OroKonvScale, Probability(values));
            //Output.Add("DEBUG", Probability(values));
            Output.Add("ORGANIZACE KONV. BOUŘE", convStormOrg);
        }



        //Podpora vzniku nebezpečných jevů
        private void SupportOfDangerousPhenomenaCreation()
        {
            List<float> values = new List<float>() { Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"], Parameters["SWEAT"] };
            int supDanPhenCreat = ValueToLevel(LevelScale, Probability(values));
            Output.Add("PODPORA VZNIKU NEBEZPEČNÝCH JEVŮ", supDanPhenCreat);
        }


        //Střih větru
        private void WindCut()
        {
            List<float> values = new List<float>() { Parameters["DLS"] };
            int windCut = ValueToLevel(LevelScale, Probability(values));
            Output.Add("STŘIH VĚTRU", windCut);
        }

        //Podpora konvekce
        private void SupportOfConvection() {
            List<float> values = new List<float>() { Parameters["Pwater"], Parameters["T 850 hPa"] };
            List<float> relativeVorticityValues = new List<float>() { Parameters["RV 850 hPa"], Parameters["RV 500 hPa"], Parameters["RV 300 hPa"] };
            int relVor = ValueToLevel(LevelScale, Probability(relativeVorticityValues));
            values.Add(relVor);
            int supCon = ValueToLevel(LevelScale, Probability(values));
            Output.Add("PODPORA KONVEKCE", supCon);
        }


        //Spouštění konvekce
        private void TriggeringConvection() {
            List<float> values = new List<float>() { Parameters["Tlak MSLP"], Parameters["FRONTOGENEZE 850 hPa"] };
            List<float> orographicSupportValues = new List<float>() { Parameters["GRAD 925-700 hPa"], Parameters["MXR"] };
            List<float> konvDivValues = new List<float>() { Parameters["MOCON"], Parameters["MFDIV 0-1 km"] };
            List<float> oroOutputValues = new List<float>() { Parameters["MTV VECTOR"], Parameters["POTENTIAL OROGRAPHIC LIFITING"] };
            int KonvDiv = ValueToLevel(OroKonvScale, SumArray(konvDivValues) / (konvDivValues.Count * RATIO));
            orographicSupportValues.Add(KonvDiv);
            int OrographicLift = ValueToLevel(OroKonvScale, SumArray(oroOutputValues) / (oroOutputValues.Count * RATIO));
            orographicSupportValues.Add(OrographicLift);
            int OrographicSupport = ValueToLevel(LevelScale, SumArray(orographicSupportValues) / (orographicSupportValues.Count * RATIO));
            values.Add(OrographicSupport);
            values.Add(Output["RH 1000-300 hPa"]);
            int triggeringConv = ValueToLevel(LevelScale, SumArray(values) / (values.Count * RATIO));
            Output.Add("SPOUŠTĚNÍ KONVEKCE", triggeringConv);
        }


        //Denní instabilita
        private void DayInstability() {
            List<float> values = new List<float>() {Parameters["MLCAPE"], Parameters["LI"], Parameters["MLCIN"]
                , Parameters["TT index"], Parameters["KI"], Parameters["GRAD 850-500 hPa"], Parameters["WETBULB"]  };
            float probabilityMLCAPELI = SumArray(values, 0, 2) / (2 * RATIO);
            int MLCAPELI = ValueToLevel(LevelScale, probabilityMLCAPELI);
            float probability = (SumArray(values, 2) + MLCAPELI) / ((values.Count - 1) * RATIO);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("DENNÍ INSTABILITA ATMOSFÉRY", level);

        }

        //Relativní vlhkost - potřebuje přidat podmínku pro suchý downburst
        private void RelativeHumidity() {
            //Suchý downburst
            

            List<float> values = new List<float>(){Parameters["RH 1000 hPa"], Parameters["RH 925 hPa"], Parameters["RH 850 hPa"]
                , Parameters["RH 700 hPa"], Parameters["RH 500 hPa"], Parameters["RH 300 hPa"] };
            float probability = SumArray(values) / (values.Count * RATIO);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 1000-300 hPa", level);

            probability = SumArray(values, 2) / ((values.Count - 2) * RATIO);
            level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 850-300 hPa", level);

            probability = SumArray(values, 0, 3) / ((values.Count - 3) * RATIO);
            level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 1000-850 hPa", level);
            //Na základě nějaké podmínky, se bude lišit RHLevels

        }

        //Pravděpodobnost srážek
        private void PrecipitationPlace() {
            float probability = 0;
            List<float> values = new List<float>(){Parameters["Srážky ALADIN"], Parameters["Srážky GDPS"], Parameters["Srážky EURO4"]
                , Parameters["Srážky HIRLAM"], Parameters["Srážky WRF-NMM"], Parameters["Srážky WRF-ARW"] };
            probability = SumArray(values) / values.Count;
            Output.Add("Pravděpodobnost Srážek", probability);
        }

        //Čas výskytu srážek
        private void PrecipitationTime()
        {
            List<float> hi = new List<float>() {0, 0, 0, 0, 0, 0, 0, 0};
            foreach (KeyValuePair<string, List<CloudInputData>> kvp in PrecipitationModels) {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    hi[i] += kvp.Value.ElementAt(i).value;
                }              
            }

            for (int i = 0; i < hi.Count; i++) {
                hi[i] /= PrecipitationModels.Count;
                Output.Add((i * 3).ToString(), hi[i]);
            }
        } 



        //Pomocné výpočetní funkce

        //Převod hodnoty pravděpodobnosti na úroveň dle tabulky
        private int ValueToLevel(List<float> levels, float value) {
            int level = -1;
            foreach (var l in levels) {

                level = (value < l) ? levels.IndexOf(l) : -1;
                if (level != -1) break;
            }
            return level;
        }
        //Výpočet pravděpodobnosti
        private float Probability(List<float> list) {
            return (float) Math.Round((double)(new decimal(SumArray(list) / (list.Count * RATIO))), 2);
        }

        private float ProbabilityWeights(List<float> arr, List<float> weights)
        {
            float sum = 0;
            float sumWeights = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                sum += arr.ElementAt(i) * weights.ElementAt(i);
                //Util.l($"value: {arr.ElementAt(i)}, weight: {weights.ElementAt(i)}");
            }

            foreach (var w in weights)
            {
                sumWeights += w * RATIO;
            }

            return (float) Math.Round((double)(new decimal(sum / sumWeights)), 2);
        }

        //Suma prvků v poli
        private float SumArray(List<float> arr, int start = 0, int end = -1) {
            float sum = 0;
            end = (end > arr.Count) ? arr.Count : end;
            start = (start > arr.Count) ? 0 : start;
            List<float> array = arr.GetRange(start, (end == -1) ? (arr.Count) - start : end - start);
            foreach (var a in array) {
                sum += a;
                //Util.l(a);
            }
            return sum;
        }
                

        private int ValueTest(List<float> list)
        {
            int result=1;
            foreach (var item in list)
            {
                result = (item < 1) ? 0 : 1;
                if (result==0) break;
            }
            return result;
        }

        //Výpis výstupu
        private void WriteOutputLog() {
            Util.l(this.Name_orp);
            foreach(var item in Output)
            {
                Util.l(item.Key + ":" + item.Value);
            }
        }
    }
}
