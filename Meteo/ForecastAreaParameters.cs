using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class ForecastAreaParameters
    {
        public int id_orp { get; set; }
        public string Name_orp { get; set; }
        public string sampleName { get; set; }

        public Dictionary<string, float> Parameters { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, List<CloudInputData>> PrecipitationModels { get; set; } = new Dictionary<string, List<CloudInputData>>();
        public Dictionary<string, float> Output { get; set; } = new Dictionary<string, float>();
        private List<float> PrecipitationPlaceModels = new List<float>();
        private List<float> LevelScale = new List<float>() { 0.25f, 0.5f, 0.75f, 1.0f };
        private List<float> FinalScale = new List<float>() { 0.08f, 0.33f, 0.67f, 1.0f };
        private List<float> TorrentialFloodRiscScale = new List<float>() { 0.22f, 0.39f, 0.67f, 1.0f };
        private List<float> TorrentialFloodRiscScale2 = new List<float>() { 0.39f, 1.0f };
        private List<float> StormIntensityScale = new List<float>() { 0.33f, 0.5f, 0.83f, 1.0f };
        private List<float> StormIntensityDangerousPhenScale = new List<float>() { 0.33f, 0.5f, 0.67f, 1.0f };
        private List<float> HumidityInfluencesScale = new List<float>() { 0.22f, 0.33f, 0.67f, 1.0f };
        private List<float> OroKonvScale = new List<float>() { 0.17f, 0.33f, 0.67f, 1.0f };
        private List<float> DangerousPhenomenaScale = new List<float>() { 0.33f, 0.5f, 0.75f, 1.0f };
        private List<float> RHLevelsAlternative = new List<float>() { 0.1f, 0.3f, 0.4f, 1.0f }; //??? Není jisté, zda je tato stupnice dobře
        private const int RATIO = 3;
        

        public ForecastAreaParameters() {

        }

        public ForecastAreaParameters(CloudORPS ORP, string sampleName) {
            Name_orp = ORP.name;
            id_orp = ORP.id;
            this.sampleName = sampleName;
            DoCountOperations();

        }


        private void LoadParameters() {

            //Zatím vynechané parametry
            //Parameters.Add("MOCON", 1); // prozatím vynechat
            //Parameters.Add("RV 850 hPa", 1); prozatím vynechat
            //Parameters.Add("RV 500 hPa", 1); prozatím vynechat
            //Parameters.Add("RV 300 hPa", 1); prozatím vynechat
            //Parameters.Add("SWEAT", 1); // prozatím vynechat
            //Parameters.Add("1000 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_10m")); // prozatím vynechat
            //Parameters.Add("925 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Vítr_925")); // prozatím vynechat
            //Parameters.Add("850 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_850")); // prozatím vynechat
            //Parameters.Add("700 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700")); // prozatím vynechat
            //Parameters.Add("600 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_600")); // prozatím vynechat
            //Parameters.Add("500 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_500")); // prozatím vynechat
            //Parameters.Add("400 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_400")); // prozatím vynechat
            //Parameters.Add("300 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_300")); // prozatím vynechat
            //Parameters.Add("Směr větru v hladině 700 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700")); // prozatím vynechat
            //Parameters.Add("Teplota (MAX)", 2); // prozatím vynechat
            //Parameters.Add("Intenzita bouřek (SIVS) Staniční srážkoměry", GetParameter("Model_Výstrahy_chmu", "Výstrahy_chmu")); //nakonec se nebude používat

            /*Orografické vlastnosti oblasti
            Parameters.Add("Proudění větru J", 1);
            Parameters.Add("Proudění větru S", 1);
            Parameters.Add("Proudění větru V", 1);
            Parameters.Add("Proudění větru Z", 1);*/


            /*Nezařazené parametry z adresářové struktury
            Model_Radarové_snímky	Radarové_snímky
            Model_Synoptická_předpověď Synoptická_předpověď
            Model_Výstrahy_chmu Výstrahy_chmu
            Model_Výstrahy_estofex Výstrahy_estofex
            Model_WRF_ARW   Relativni_vorticita_850 - 300_hPa_WRF
            */

            Parameters.Add("MLCAPE", GetParameter("Model_GFS_Meteomodel_PL_25km", "MLCAPE_GFS")); // Alternace: Model_GFS_Wetter3_DE_25km	MLCAPE+LI_Wetter_3_de
            Parameters.Add("LI", GetParameter("Model_GFS_Austria_50km", "LI_index_GFS_MAIN"));
            Parameters.Add("MUCAPE", GetParameter("Model_GFS_Meteomodel_PL_25km", "MUCAPE_GFS")); 
            Parameters.Add("SI", GetParameter("Model_GFS_Austria_50km", "SI_index_GFS_MAIN"));
            Parameters.Add("MLCIN", GetParameter("Model_GFS_Wetter3_DE_25km", "MLCIN_Wetter_3_de")); // Alternace: MLCIN_Wetter_3_de_MAIN
            Parameters.Add("TT index", GetParameter("Model_WRF_ARW_Balearsmeteo", "TT_Totals_Totals_index_Aladin_HR"));
            Parameters.Add("KI", GetParameter("Model_GFS_Meteomodel_PL_25km", "KI_Whiting_index"));
            Parameters.Add("GRAD 850-500 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Instabilita_GRAD_850-500"));
            Parameters.Add("WETBULB", GetParameter("Model_GFS_Wetter3_DE_25km", "Wet_bulb_temp"));
            Parameters.Add("FRONTOGENEZE 850 hPa", GetParameter("Model_GFS_Wetter3_DE_50km", "Frontogeneze_parametr_850_hPa"));
            Parameters.Add("GRAD 925-700 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "GRAD_925-700hPa"));
            Parameters.Add("MXR", GetParameter("Model_GFS_Lightning_Wizard_50km", "Mixing_Ratio_0-1km"));          
            Parameters.Add("MFDIV 0-1 km", GetParameter("Model_WRF_ARW", "MFDIV_0-1km"));
            Parameters.Add("MTV VECTOR", GetParameter("Model_GFS_Lightning_Wizard_50km", "MTV_vector_RH_1000-600 hPa"));
            Parameters.Add("POTENTIAL OROGRAPHIC LIFITING", GetParameter("Model_WRF_ARW", "Potential_Orographic_Lift"));
            Parameters.Add("RH 1000 hPa", GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000"));
            Parameters.Add("RH 925 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_925"));
            Parameters.Add("RH 850 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_850"));
            Parameters.Add("RH 700 hPa", GetParameter("Model_WRF_ARW", "Relativní_vlhkost_700")); 
            Parameters.Add("RH 500 hPa", GetParameter("Model_WRF_ARW", "Relativní_vlhkost_500")); 
            Parameters.Add("RH 300 hPa", GetParameter("Model_WRF_ARW", "Relativní_vlhkost_300"));
            Parameters.Add("Pwater", GetParameter("Model_WRF_ARW", "Pwater")); 
            Parameters.Add("T 850 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Teplota_850")); 
            Parameters.Add("DLS", GetParameter("Model_GFS_Meteomodel_PL_25km", "SHEAR_DLS_Střih_větru_0-6_km")); 
            Parameters.Add("LLS", GetParameter("Model_GFS_Meteomodel_PL_25km", "SHEAR_LLS_Střih_větru_0-1_km"));
            Parameters.Add("SREH 3 km", GetParameter("Model_WRF_ARW", "SRH_3km_WRF_ARW")); 
            Parameters.Add("SREH 1 km", GetParameter("Model_GFS_Meteomodel_PL_25km", "SRH_1km"));
            Parameters.Add("Rychlost větru v 300 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_300"));
            Parameters.Add("Rychlost větru v 850 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_850")); 
            Parameters.Add("MCS VEKTOR", GetParameter("Model_GFS_Lightning_Wizard_50km", "MCS_vektor_pohybu_bouří"));
            Parameters.Add("850 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_850")); 
            Parameters.Add("700 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700")); 
            Parameters.Add("600 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_600")); 
            Parameters.Add("500 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_500")); 
            Parameters.Add("400 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_400")); 
            Parameters.Add("300 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_300")); 
            Parameters.Add("LCL", GetParameter("Model_WRF_ARW", "LCL_Výška_základny_oblaku"));
            Parameters.Add("Nulová izoterma (km)", GetParameter("Model_GFS_Meteomodel_PL_25km", "0_izoterma_výška"));

            //Který je správně? odkomentovaný vrací kladný výsledek
            Parameters.Add("Hloubka teplé fáze oblaku (km)", Parameters["Nulová izoterma (km)"] - Parameters["LCL"]); 
            //Parameters.Add("Hloubka teplé fáze oblaku (km)", Parameters["LCL"] - Parameters["Nulová izoterma (km)"]); 

            Parameters.Add("SHIP", GetParameter("Model_GFS_Meteomodel_PL_25km", "SHIP")); 
            Parameters.Add("DTHE", GetParameter("Model_GFS_Lightning_Wizard_50km", "DTHE_MAIN")); 
            Parameters.Add("SBCAPE 0-2 km (J/kg) - den", GetParameter("Model_GFS_Lightning_Wizard_50km", "SBCAPE_2km"));
            Parameters.Add("STP", GetParameter("Model_GFS_Meteomodel_PL_25km", "EHI,STP_MAIN")); 
            Parameters.Add("Tlak MSLP", GetParameter("Model_WRF_ARW", "Tlaková_tendence_MSLP"));
            Parameters.Add("Oblačnost", GetParameter("Model_ALADIN_CZ", "Oblačnost"));
            Parameters.Add("Rychlost větru v 10 m nad terénem v m/s", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_10m"));
            Parameters.Add("RH 2 m (%)", GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000"));
            Parameters.Add("KONV+/DIV- (0-1 km)", GetParameter("Model_WRF_ARW", "MFDIV_0-1km")); 
            Parameters.Add("OROGRAPHIC LIFT", GetParameter("Model_GFS_Lightning_Wizard_50km", "MTV_vector_RH_1000-600 hPa"));
            Parameters.Add("Staniční srážkoměry CHMU+interpolace stanic", GetParameter("Model_Sumarizace_srazek", "Sumarizace_srazek", "srážkoměry"));
            Parameters.Add("Interpolace (radary+srážkoměry)", GetParameter("Model_Sumarizace_srazek", "Sumarizace_srazek", "radary_srážkoměry"));
            Parameters.Add("Stupeň nasycení", GetParameter("Model_Nasycenost_pud", "Nasycenost_pud_2_typ")); 
            Parameters.Add("Suma srážek (1.hod.)", GetParameter("Model_Nasycenost_pud", "Nasycenost_pud_1_typ")); 
            Parameters.Add("Srážky ALADIN", GetParameter("Model_ALADIN_CZ", "Srážky_MAIN"));
            Parameters.Add("Srážky GDPS", GetParameter("Model_GDPS", "Srážky_MAIN")); 
            Parameters.Add("Srážky EURO4", GetParameter("Model_EURO4", "Srážky_MAIN")); 
            Parameters.Add("Srážky HIRLAM", GetParameter("Model_HIRLAM_Nový", "Srážky_MAIN_Nový"));
            Parameters.Add("Srážky HIRLAM Starý", GetParameter("Model_HIRLAM_Starý", "Srážky_MAIN_Starý"));
            Parameters.Add("Srážky WRF-NMM", GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN")); 
            Parameters.Add("Srážky WRF-ARW", GetParameter("Model_WRF_ARW", "Srážky_MAIN"));
            Parameters.Add("Srážky GFS", GetParameter("Model_GFS_Wetterzentrale_DE_25km", "Srážky_MAIN_Nový"));
            Parameters.Add("Srážky GFS Starý", GetParameter("Model_GFS_Wetterzentrale_DE_25km_STARY", "Srážky_MAIN_Starý"));
            


            //Parametry pro suchý downburst
            Parameters.Add("RH 1000 hPa Real", 75); //75  //GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000")
            Parameters.Add("RH 925 hPa Real", 60); //60 //GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_925")
            Parameters.Add("RH 850 hPa Real", 75); //75 //GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_850")
            Parameters.Add("LCL Real", 1200); //1200 //GetParameter("Model_WRF_ARW", "LCL_Výška_základny_oblaku")

            //Příprava dat pro předpověď času výskytu srážek
            PrecipitationModels.Add("Srážky ALADIN", GetPrecipitationData(Parameters["Srážky ALADIN"]));
            PrecipitationModels.Add("Srážky GDPS", GetPrecipitationData(Parameters["Srážky GDPS"]));
            PrecipitationModels.Add("Srážky EURO4", GetPrecipitationData(Parameters["Srážky EURO4"]));
            PrecipitationModels.Add("Srážky HIRLAM", GetPrecipitationData(Parameters["Srážky HIRLAM"]));
            PrecipitationModels.Add("Srážky HIRLAM Starý", GetPrecipitationData(Parameters["Srážky HIRLAM Starý"])); //pro historické události - přepínatko
            PrecipitationModels.Add("Srážky WRF-NMM", GetPrecipitationData(Parameters["Srážky WRF-NMM"]));
            PrecipitationModels.Add("Srážky WRF-ARW", GetPrecipitationData(Parameters["Srážky WRF-ARW"]));
            PrecipitationModels.Add("Srážky GFS", GetPrecipitationData(Parameters["Srážky GFS"]));
            PrecipitationModels.Add("Srážky GFS Starý", GetPrecipitationData(Parameters["Srážky GFS Starý"])); //pro historické události - přepínatko

            //Přídání dostupných srážkových modelů
            AddPrecipitationModel(new List<float>() { Parameters["Srážky ALADIN"], Parameters["Srážky GDPS"], Parameters["Srážky EURO4"], Parameters["Srážky HIRLAM"], Parameters["Srážky WRF-NMM"], Parameters["Srážky WRF-ARW"], Parameters["Srážky HIRLAM Starý"], Parameters["Srážky GFS Starý"] });
        }

        private void DoCountOperations()
        {
            //Util.l("\n--------------------------------------\n" + "Počítám jednotlivé kroky algoritmu pro: " + Name_orp);
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
            HumidityInfluences();
            WindEffect();
            MergeB();
            WriteToDatabase();

            //WriteOutputLog();

        }

        private void WriteToDatabase() {
            CloudOutputData mainOutput = new CloudOutputData(id_orp, sampleName, Output["1. RIZIKO PŘÍVALOVÉ POVODNĚ"],Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutput);
            CloudOutputData stormIntensityOutput = new CloudOutputData(id_orp, sampleName, Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Util.algorithmOutput["PŘEDPOVĚĎ INTENZITY BOUŘÍ"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(stormIntensityOutput);
            CloudOutputData precipitationPlaceOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU BOUŘEK"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceOutput);
            CloudOutputData precipitationPlaceKoefOutput = new CloudOutputData(id_orp, sampleName, Output["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK - KOEFICIENT"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceKoefOutput);
            CloudOutputData torrentialRainOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - PŘÍVALOVÉ SRÁŽKY"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(torrentialRainOutput);
            CloudOutputData strongWindscreensOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutput);
            CloudOutputData hailOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - KRUPOBITÍ"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA KRUPOBITÍ"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(hailOutput);
            CloudOutputData supercelarTornadosOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - SUPERCELÁRNÍ TORNÁDA"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"]);
            Model.Cloud.OUTPUTDATAInsertOrUpdate(supercelarTornadosOutput);
        }

        //8. Sloučení B (DEN) - Intenzita bouřek a Lokální předpověď
        private void MergeB() {
            List<float> values;
            if (IsDay())
            {
                values = new List<float>() { Output["TEPLOTNÍ VLIVY ZEMSKÉHO POVRCHU"], Output["VĚTRNÉ VLIVY"], Output["VLHKOSTNÍ VLIVY"], Output["NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; JZ,J,JV proudění"] };

            }
            else {
                values = new List<float>() { Output["VĚTRNÉ VLIVY"], Output["VLHKOSTNÍ VLIVY"], Output["NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; JZ,J,JV proudění"] };
            }

            int level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("LOKÁLNÍ PŘEDPOVĚĎ", level);
            values = new List<float>() { Output["LOKÁLNÍ PŘEDPOVĚĎ"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"]};
            level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU BOUŘEK", level);


            //8)Nebezpečné doprovodné jevy (6.krok+Sloučení A)
            values = new List<float>() { Output["PODPORA VZNIKU NEBEZPEČNÝCH JEVŮ"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Output["MÍSTO VÝSKYTU BOUŘEK"] };
            level = ValueToLevel(StormIntensityDangerousPhenScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY", level);

            //Přívalové srážky
            values = new List<float>() { Output["PŘÍVALOVÉ SRÁŽKY"], Output["POHYB BOUŘE"], Output["MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(StormIntensityScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU - PŘÍVALOVÉ SRÁŽKY", level);

            //Silné nárazy větru
            values = new List<float>() { Output["SILNÉ NÁRAZY VĚTRU"], Output["MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(StormIntensityScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU", level);

            //Krupobití
            values = new List<float>() { Output["KRUPOBITÍ"], Output["MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(StormIntensityScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU - KRUPOBITÍ", level);

            //Supercelární tornáda
            values = new List<float>() { Output["DEN - SUPERCELÁRNÍ TORNÁDA"], Output["MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(StormIntensityScale, Probability(values));
            Output.Add("MÍSTO VÝSKYTU - SUPERCELÁRNÍ TORNÁDA", level);

            //Sumarizace výstupů
            values = new List<float>() { Output["MÍSTO VÝSKYTU - PŘÍVALOVÉ SRÁŽKY"], Output["MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU"], Output["MÍSTO VÝSKYTU - KRUPOBITÍ"], Output["MÍSTO VÝSKYTU - SUPERCELÁRNÍ TORNÁDA"] };
            level = ValueToLevel(FinalScale, Probability(values));
            Output.Add("NEBEZPEČNÉ JEVY", level);

            //Hlavní výstup algoritmu
            values = new List<float>() { Parameters["Stupeň nasycení"], Parameters["Suma srážek (1.hod.)"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Output["POHYB BOUŘE"], Output["NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(TorrentialFloodRiscScale, Probability(values));
            Output.Add("1. RIZIKO PŘÍVALOVÉ POVODNĚ", level);//
            level = ValueToLevel(TorrentialFloodRiscScale2, Probability(values));
            Output.Add("2. RIZIKO PŘÍVALOVÉ POVODNĚ", level);

            values = new List<float>() { Parameters["Stupeň nasycení"], Parameters["Suma srážek (1.hod.)"], Output["MÍSTO VÝSKYTU BOUŘEK"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Output["POHYB BOUŘE"], Output["NEBEZPEČNÉ JEVY"] };
            if (Parameters["Stupeň nasycení"] <= 1.5){
                List<float> weights = new List<float>() { 1, 1, 3, 3, 3, 2 };
                level = ValueToLevel(TorrentialFloodRiscScale, ProbabilityWeights(values, weights));
                Output.Add("1. RIZIKO PŘÍVALOVÉ POVODNĚ - SUCHÝ", level);
                level = ValueToLevel(TorrentialFloodRiscScale2, ProbabilityWeights(values, weights));
                Output.Add("2. RIZIKO PŘÍVALOVÉ POVODNĚ - SUCHÝ", level);
            }
            else {
                List<float> weights = new List<float>() { 3, 3, 3, 3, 2, 1 };
                level = ValueToLevel(TorrentialFloodRiscScale, ProbabilityWeights(values, weights));
                Output.Add("1. RIZIKO PŘÍVALOVÉ POVODNĚ - VLHKÝ", level);
                level = ValueToLevel(TorrentialFloodRiscScale2, ProbabilityWeights(values, weights));
                Output.Add("2. RIZIKO PŘÍVALOVÉ POVODNĚ - VLHKÝ", level);
            }
            
        }



        //7. Lokální předpověď
        //Teplotní vlivy zemského povrchu
        private void TemperatureInfluencesOfEarthSurface() {
            List<float> values = new List<float>() { Parameters["Oblačnost"] }; ///+ Charakteristiky reliéfu
            int level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("TEPLOTNÍ VLIVY ZEMSKÉHO POVRCHU", level);
        }

        //Větrné vlivy
        private void WindInfluences()
        {
            List<float> values = new List<float>() { Parameters["Rychlost větru v 10 m nad terénem v m/s"] }; //+ Charakteristiky reliéfu
            int level = ValueToLevel(LevelScale, Probability(values));
            Output.Add("VĚTRNÉ VLIVY", level);
        }

        //Vlhkostní vlivy
        private void HumidityInfluences()
        {
            List<float> values = new List<float>() { Parameters["RH 2 m (%)"], Parameters["KONV+/DIV- (0-1 km)"] };
            //List<float> previousDayValues = new List<float>() { Parameters["Intenzita bouřek (SIVS) Staniční srážkoměry"], Parameters["Staniční srážkoměry CHMU+interpolace stanic"], Parameters["Interpolace (radary+srážkoměry)"] };
            List<float> previousDayValues = new List<float>() { Parameters["Staniční srážkoměry CHMU+interpolace stanic"], Parameters["Interpolace (radary+srážkoměry)"] };
            int previousDayLevel = ValueToLevel(HumidityInfluencesScale, Probability(previousDayValues));
            values.Add(previousDayLevel);
            int level = ValueToLevel(HumidityInfluencesScale, Probability(values));
            Output.Add("VLHKOSTNÍ VLIVY", level);
        }

        //Návětrný + závětrný efekt
        //Zjistit jak konkrétně budou vypočtené parametry využívána a počítány! 
        private void WindEffect()
        {
            List<float> windwardValues = new List<float>() { Parameters["GRAD 925-700 hPa"], Parameters["MXR"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["OROGRAPHIC LIFT"], Parameters["Rychlost větru v 850 hPa"] }; // + Charakteristiky reliéfu
            int windwardLevel = ValueToLevel(LevelScale, Probability(windwardValues));

            List<float> leeValues = new List<float>() { Parameters["GRAD 850-500 hPa"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["Rychlost větru v 10 m nad terénem v m/s"] };
            int leeLevel = ValueToLevel(LevelScale, Probability(leeValues));

            List<float> windEffectValues = new List<float>();
            windEffectValues.Add(windwardLevel);
            windEffectValues.Add(leeLevel);
            int windEffectLevel = ValueToLevel(OroKonvScale, Probability(windEffectValues));

            Output.Add("NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; JZ,J,JV proudění", windEffectLevel);
            Output.Add("NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; SZ,S,SV proudění", windEffectLevel);
            Output.Add("NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; Z proudění", windEffectLevel);
            Output.Add("NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT; V proudění", windEffectLevel);

        }

        //Návětrný efekt



        //6. Riziko výskytu nebezpečných jevů
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
            List<float> values = new List<float>() { Parameters["Pwater"],
                Output["RH 1000-850 hPa"],
                Parameters["MXR"],
                Parameters["LCL"],
                Parameters["Rychlost větru v 850 hPa"],
                Parameters["Hloubka teplé fáze oblaku (km)"],
                Parameters["MCS VEKTOR"],
                Parameters["DLS"],
                Output["ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa"] };
            Output.Add("PŘÍVALOVÉ SRÁŽKY", DangerousPhenomenaCount(weights, values));
        }

        //Den/Noc - Krupobití
        private void DayHailStorm()
        {
            List<float> weights = new List<float>() { 3, 1, 2, 3, 3, 3, 1, 2, 3, 2, 3};
            List<float> values;
            if (IsDay())
            {
                /*List<float> values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                    Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };*/
                values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };
                
            }
            else
            {
                values = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };
                
            }
            Output.Add("KRUPOBITÍ", DangerousPhenomenaCount(weights, values));
        }

        //DEN - Silné nárazy větru
        private void StrongWindImpact()
        {
            List<float> weights = new List<float>() { 3, 2, 2, 3, 3, 2, 2, 1, 1, 2, 3 };
            List<float> RHReal = new List<float>() { Parameters["RH 1000 hPa Real"] , Parameters["RH 925 hPa Real"] , Parameters["RH 850 hPa Real"] };
            
            //Podmínka pro suchý downburst
            if (Output[this.sampleName] < 0.57 && Average(RHReal) < 40 && Parameters["LCL Real"] >1500) {
                //Nastal suchý downburst - do analýzy se dostanou nové KOEFICIENTY.
                Output.Add("NASTAL SUCHÝ DOWNBURST", 1);
                float[] boundariesLCL = new float[3] { 1500, 2000, 2500};
                float[] boundariesRH = new float[3] { 20, 30, 40};
                /*Parameters["LCL"] = ChangeValueOfParameterLCL(boundariesLCL, Parameters["LCL Real"]);
                Parameters["RH 1000 hPa"] = ChangeValueOfParameterRH(boundariesRH, Parameters["RH 1000 hPa Real"]);
                Parameters["RH 925 hPa"] = ChangeValueOfParameterRH(boundariesRH, Parameters["RH 925 hPa Real"]);
                Parameters["RH 850 hPa"] = ChangeValueOfParameterRH(boundariesRH, Parameters["RH 850 hPa Real"]);
                
                List<float> valuesDryDownburst = new List<float>(){Parameters["RH 1000 hPa"], Parameters["RH 925 hPa"], Parameters["RH 850 hPa"] };
                Output["RH 1000-850 hPa"] = ValueToLevel(LevelScale, Probability(valuesDryDownburst));*/

                Parameters.Add("LCL SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["LCL Real"]));
                Parameters.Add("RH 1000 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 1000 hPa Real"]));
                Parameters.Add("RH 925 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 925 hPa Real"]));
                Parameters.Add("RH 850 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 850 hPa Real"]));

                List<float> valuesDryDownburst = new List<float>() { Parameters["RH 1000 hPa SD"], Parameters["RH 925 hPa SD"], Parameters["RH 850 hPa SD"] };
                Output.Add("RH 1000-850 hPa SD", ValueToLevel(LevelScale, Probability(valuesDryDownburst)));

                
                List<float> valuesSD;
                if (IsDay())
                {
                    /*List<float> valuesSD = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa SD"], Parameters["LCL SD"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};*/
                    valuesSD = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa SD"], Parameters["LCL SD"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                    Output.Add("SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST", DangerousPhenomenaCount(weights, valuesSD));
                }
                else {
                    valuesSD = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa SD"], Parameters["LCL SD"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                    Output.Add("SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST", DangerousPhenomenaCount(weights, valuesSD));
                }
            }

            List<float> values;
            if (IsDay())
            {
                /*List<float> values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                    Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};*/
                values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                Output.Add("SILNÉ NÁRAZY VĚTRU", DangerousPhenomenaCount(weights, values));
            }
            else {
                values = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                Output.Add("SILNÉ NÁRAZY VĚTRU", DangerousPhenomenaCount(weights, values));
            }
        }

        //Supercelární tornáda
        private void SupercelarTornados()
        {
            List<float> weights = new List<float>() { 3, 3, 2, 2, 2, 3, 1, 3};
            /*List<float> values = new List<float>() { Parameters["SBCAPE 0-2 km (J/kg) - den"], Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["LCL"],
                Parameters["STP"]};*/
            List<float> values = new List<float>() { Parameters["SBCAPE 0-2 km (J/kg) - den"], Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"], Parameters["Rychlost větru v 850 hPa"], Parameters["LCL"],
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
            //List<float> values = new List<float>() { Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"], Parameters["SWEAT"] };
            List<float> values = new List<float>() { Parameters["LLS"], Parameters["SREH 3 km"], Parameters["SREH 1 km"] };
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

            /*List<float> relativeVorticityValues = new List<float>() { Parameters["RV 850 hPa"], Parameters["RV 500 hPa"], Parameters["RV 300 hPa"] };
            int relVor = ValueToLevel(LevelScale, Probability(relativeVorticityValues));
            values.Add(relVor);*/

            int supCon = ValueToLevel(LevelScale, Probability(values));
            Output.Add("PODPORA KONVEKCE", supCon);
        }


        //Spouštění konvekce
        private void TriggeringConvection() {
            List<float> values = new List<float>() { Parameters["Tlak MSLP"], Parameters["FRONTOGENEZE 850 hPa"] };
            List<float> orographicSupportValues = new List<float>() { Parameters["GRAD 925-700 hPa"], Parameters["MXR"] };
            //List<float> konvDivValues = new List<float>() { Parameters["MOCON"], Parameters["MFDIV 0-1 km"] };
            List<float> konvDivValues = new List<float>() { Parameters["MFDIV 0-1 km"] };
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
            List<float> values;
            if (IsDay())
            {
                values = new List<float>() {Parameters["MLCAPE"], Parameters["LI"], Parameters["MLCIN"]
                , Parameters["TT index"], Parameters["KI"], Parameters["GRAD 850-500 hPa"], Parameters["WETBULB"]  };
            }
            else {
                values = new List<float>() {Parameters["MUCAPE"], Parameters["SI"], Parameters["MLCIN"]
                , Parameters["TT index"], Parameters["KI"], Parameters["GRAD 850-500 hPa"], Parameters["WETBULB"]  };
            }
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
            Output.Add("RH 1000-850 hPa Value", probability);
            Output.Add("RH 1000-850 hPa", level);
            //Na základě nějaké podmínky, se bude lišit RHLevels

        }

        //Pravděpodobnost srážek
        private void PrecipitationPlace() {
            float probability = 0;
            probability = SumArray(PrecipitationPlaceModels) / PrecipitationPlaceModels.Count;
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK", probability);
            Output.Add("PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK - KOEFICIENT", level);
            
        }

        //Čas výskytu srážek
        private void PrecipitationTime()
        {
            List<float> hi = new List<float>() {0, 0, 0, 0, 0, 0, 0, 0};
            List<int> modelsUnavailable = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0};
            foreach (KeyValuePair<string, List<CloudInputData>> kvp in PrecipitationModels) {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value.ElementAt(i).value == -1)
                    {
                        modelsUnavailable[i]++;

                    }
                    else {
                        hi[i] += kvp.Value.ElementAt(i).value;
                    }
                }              
            }

            for (int i = 0; i < hi.Count; i++) {
                hi[i] /= (PrecipitationModels.Count-modelsUnavailable[i]);
                Output.Add((i * 3).ToString(), hi[i]);
            }
            //Output.Add(this.sampleName, hi[0]);
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

        //Aritmetický průměr hodnot
        private float Average(List<float> arr) {
            float avg = 0;
            avg = SumArray(arr) / arr.Count;
            return avg;
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

        private float ChangeValueOfParameterLCL(float [] boundaries, float param) {
            float temp = 0;
            switch (param)
            {
                case float val when val < boundaries[0]: temp = 0; break;
                case float val when val < boundaries[1]: temp = 1; break;
                case float val when val < boundaries[2]: temp = 2; break;
                case float val when val >= boundaries[2]: temp = 3; break;
            }
            return temp;
        }


        private float ChangeValueOfParameterRH(float[] boundaries, float param)
        {
            float temp = 0;
            switch (param)
            {
                case float val when val > boundaries[2]: temp = 0; break;
                case float val when val > boundaries[1]: temp = 1; break;
                case float val when val > boundaries[0]: temp = 2; break;
                case float val when val < boundaries[0]: temp = 3; break;
            }
            return temp;
        }
        //Výpis výstupu
        private void WriteOutputLog() {
            
            Util.l("\n\nDOSTUPNÉ VSTUPNÍ PARAMETRY\n--------------------\n");
            Util.l(this.Name_orp);
            Util.l("Počet dostupných srážkových modelů: " + PrecipitationPlaceModels.Count);
            foreach (var item in Parameters)
            {
                if(item.Value!=-1) Util.l(item.Key + ":" + item.Value);
            }

            Util.l("\n\nNEDOSTUPNÉ VSTUPNÍ PARAMETRY\n--------------------\n");
            foreach (var item in Parameters) {
                if (item.Value == -1) Util.l(item.Key);
            }

            Util.l("\n\nPRŮBĚŽNÉ VÝSLEDKY\n--------------------\n");
            
            foreach(var item in Output)
            {
                Util.l(item.Key + ":" + item.Value);
            }
        }

        //Vytažení parametru z databáze
        private float GetParameter(string model, string submodel, string sample = "") {
            float value;

            if (sample == "")
            {
                value = Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sampleName, id_orp);
            }
            else {
                value = Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sample, id_orp);
            }
            return value;
        }

        //Načtení dat ze srážkových modelů
        private List<CloudInputData> GetPrecipitationData(float value)
        {
            List<CloudInputData> data = new List<CloudInputData>();
            CloudInputData firstInterval = new CloudInputData(value);
            data.Add(firstInterval);

            return data;
        }

        //Přidání srážkového modelu do precipitationPlaceModels
        private void AddPrecipitationModel(List<float> value) {
            foreach (var v in value)
            {
                if (v != -1)
                    PrecipitationPlaceModels.Add(v);
            }
        }

        private bool IsDay() {
            if (sampleName == "21" || sampleName == "00" || sampleName == "03" || sampleName == "06")
                return false;
            else
                return true;
        }
    }
}
