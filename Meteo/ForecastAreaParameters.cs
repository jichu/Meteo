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
        public string previousSample { get; set; }
        private int precision = 10;
        private bool drydownburst = false;
        private float precipitationTreshold = 0.1f;//Stanovuje jaké množství srážkových modelů musí vracet 1, aby byl zahájen výpočet předpovědi

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

        private Dictionary<float, float> mapWindwardEffect;
        private Dictionary<float, float> mapLeeEffect;

        public Dictionary<string, int> typeValueDictionary = new Dictionary<string, int>
        {
            { "DEFAULT", 0 },
            { "CONVECTIVE", 1 },
            { "RASTER", 2 },
            { "REAL", 3 },
            { "WIND", 4}
        };

        private List<string> sampleNames = new List<string>{
            "N",
            "00",
            "03",
            "06",
            "09",
            "12",
            "15",
            "18",
            "21",
            "24",
            "27",
            "30",
            "33",
            "36",
            "39",
            "42",
            "45",
            "48"
        };

        //Pořadí směrů: S, SV, V, JV, J, JZ, Z, SZ (podle růžice)
        private float[][] windDirectioncoordinates =
            {
                    new float[] { 0f, 2f },
                    new float[] { 1.42f, 1.42f},
                    new float[] { 2f, 0f },
                    new float[] { 1.42f, -1.42f },
                    new float[] {  0f, -2f  },
                    new float[] { -1.42f, -1.42f  },
                    new float[] { -2f, 0f },
                    new float[] { -1.42f, 1.42f },
                };

        //List na uchování všech směrů větru, které byly nalzeny v rámci analýzy směru větru
        private List<float> windDirections = new List<float>();

        public ForecastAreaParameters() {

        }

        public ForecastAreaParameters(CloudORPS ORP, string sampleName) {
            Name_orp = ORP.name;
            id_orp = ORP.id;
            this.sampleName = sampleName;
            int index = sampleNames.IndexOf(sampleName);
            if (index != -1) this.previousSample = sampleNames.GetRange(index - 1, 1).ToArray()[0];
            else this.previousSample = sampleNames.GetRange(0, 1).ToArray()[0];
            DoCountOperations();

        }

        private void LoadParameters() {

            //Zatím vynechané parametry
            Parameters.Add("Teplota (MAX)", -1);
            Parameters.Add("MOCON", -1); // prozatím vynechat
            Parameters.Add("RV 850 hPa", -1);
            Parameters.Add("RV 300 hPa", -1);
            Parameters.Add("SWEAT", -1);

            //Parameters.Add("925 hPa", GetParameter("Model_GFS_Meteomodel_PL_25km", "Vítr_925")); 
            //Parameters.Add("Směr větru v hladině 700 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700")); // prozatím vynechat

            //Parameters.Add("Intenzita bouřek (SIVS) Staniční srážkoměry", GetParameter("Model_Výstrahy_chmu", "Výstrahy_chmu")); //nakonec se nebude používat
            /*Nezařazené parametry z adresářové struktury
            Model_Radarové_snímky	Radarové_snímky
            Model_Synoptická_předpověď Synoptická_předpověď
            Model_Výstrahy_chmu Výstrahy_chmu
            Model_Výstrahy_estofex Výstrahy_estofex
            Model_WRF_ARW   Relativni_vorticita_850 - 300_hPa_WRF
            */

            //Charakteristiky reliéfu
            Parameters.Add("Sklonitost reliéfu (průměrná)", GetRelief("Sklonitost reliéfu (průměrná)"));
            if(sampleName == "06" || sampleName == "09" || sampleName == "30" || sampleName == "33") Parameters.Add("Orientace reliéfu (tepelný prohřev)", GetRelief("Orientace reliéfu (tepelný prohřev) dopoledne"));
            if(sampleName == "12" || sampleName == "15" || sampleName == "18" || sampleName == "36" || sampleName == "39" || sampleName == "42") Parameters.Add("Orientace reliéfu (tepelný prohřev)", GetRelief("Orientace reliéfu (tepelný prohřev) odpoledne"));
            if(sampleName == "06" || sampleName == "09" || sampleName == "30" || sampleName == "33") Parameters.Add("Světelnost od JZ (Z-factor)", GetRelief("Světelnost od JZ (Z-factor) dopoledne"));
            if(sampleName == "12" || sampleName == "15" || sampleName == "18" || sampleName == "36" || sampleName == "39" || sampleName == "42") Parameters.Add("Světelnost od JZ (Z-factor)", GetRelief("Světelnost od JZ (Z-factor) odpoledne"));
            Parameters.Add("Vegetace-pokrytí (%)", GetRelief("Vegetace-pokrytí (%)"));
            Parameters.Add("Teplotní gradient (nadmořská výška)", GetRelief("Teplotní gradient (nadmořská výška)"));
            Parameters.Add("IR kontrast", GetRelief("IR kontrast"));
            Parameters.Add("Sídelní útvar", GetRelief("Sídelní útvar"));
            Parameters.Add("Šířka údolí", GetRelief("Šířka údolí"));
            Parameters.Add("Obtékání překážky", GetRelief("Obtékání překážky"));
            Parameters.Add("Polohy nadmořských výšek", GetRelief("Polohy nadmořských výšek"));
            Parameters.Add("Hřeben", GetRelief("Šířka hřebene"));

            //Orografické vlastnosti oblasti
            Parameters.Add("Návětrný efekt S", GetRelief("Návětrný efekt S"));
            Parameters.Add("Návětrný efekt J", GetRelief("Návětrný efekt J"));
            Parameters.Add("Návětrný efekt V", GetRelief("Návětrný efekt V"));
            Parameters.Add("Návětrný efekt Z", GetRelief("Návětrný efekt Z"));
            Parameters.Add("Závětrný efekt S", GetRelief("Závětrný efekt S"));
            Parameters.Add("Závětrný efekt J", GetRelief("Závětrný efekt J"));
            Parameters.Add("Závětrný efekt V", GetRelief("Závětrný efekt V"));
            Parameters.Add("Závětrný efekt Z", GetRelief("Závětrný efekt Z"));

            //Alternaci parametrů udělat přes settings (tabulka v DB)
            Parameters.Add("MLCAPE", GetParameter("Model_GFS_Meteomodel_PL_25km", "MLCAPE_GFS")); // Alternace: Model_GFS_Wetter3_DE_25km	MLCAPE+LI_Wetter_3_de
            Parameters.Add("LI", GetParameter("Model_GFS_Austria_50km", "LI_index_GFS_MAIN"));
            Parameters.Add("MLCIN", GetParameter("Model_GFS_Wetter3_DE_25km", "MLCIN_Wetter_3_de")); // Alternace: MLCIN_Wetter_3_de_MAIN
            Parameters.Add("MUCAPE", GetParameter("Model_GFS_Meteomodel_PL_25km", "MUCAPE_GFS")); 
            Parameters.Add("SI", GetParameter("Model_GFS_Austria_50km", "SI_index_GFS_MAIN"));
            //Parameters.Add("MUCIN", GetParameter("Model_GFS_Wetter3_DE_25km", "MLCIN_Wetter_3_de"));
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
            Parameters.Add("RV 500 hPa", GetParameter("Model_WRF_ARW", "Relativni_vorticita_500_hPa_WRF"));
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
            Parameters.Add("Hloubka teplé fáze oblaku (km)", Parameters["Nulová izoterma (km)"] - Parameters["LCL"]);
            Parameters.Add("SHIP", GetParameter("Model_GFS_Meteomodel_PL_25km", "SHIP")); 
            Parameters.Add("DTHE", GetParameter("Model_GFS_Lightning_Wizard_50km", "DTHE_MAIN")); 
            Parameters.Add("SBCAPE 0-2 km (J/kg) - den", GetParameter("Model_GFS_Lightning_Wizard_50km", "SBCAPE_2km"));
            Parameters.Add("STP", GetParameter("Model_GFS_Meteomodel_PL_25km", "EHI,STP_MAIN")); 
            Parameters.Add("Tlak MSLP", GetParameter("Model_WRF_ARW", "Tlaková_tendence_MSLP"));
            Parameters.Add("Oblačnost", GetParameter("Model_ALADIN_CZ", "Oblačnost", true, "DEFAULT", true));
            Parameters.Add("Rychlost větru v 10 m nad terénem v m/s", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_10m"));
            Parameters.Add("RH 2 m (%)", GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000"));
            Parameters.Add("KONV+/DIV- (0-1 km)", GetParameter("Model_WRF_ARW", "MFDIV_0-1km")); 
            Parameters.Add("OROGRAPHIC LIFT", GetParameter("Model_GFS_Lightning_Wizard_50km", "MTV_vector_RH_1000-600 hPa"));//MTV + POTENTIAL OROGRAPHIC LIFITING |  oroOutputValues nastavit do tohoto parametru.
            Parameters.Add("Staniční srážkoměry CHMU+interpolace stanic", GetParameter("Model_Sumarizace_srazek", "Srážkoměry", false));
            Parameters.Add("Interpolace (radary+srážkoměry)", GetParameter("Model_Sumarizace_srazek", "Radary_srážkoměry", false));
            Parameters.Add("Stupeň nasycení", GetParameter("Model_Nasycenost_pud", "Nasycenost_pud_1",false)); 
            Parameters.Add("Stupeň nasycení max", GetParameter("Model_Nasycenost_pud", "Nasycenost_pud_1_max",false)); 
            Parameters.Add("Suma srážek (1.hod.)", GetParameter("Model_Nasycenost_pud", "Suma srážek 1 hod",false)); 
            Parameters.Add("Srážky ALADIN", GetParameter("Model_ALADIN_CZ", "Srážky_MAIN"));
            Parameters.Add("Srážky GDPS", GetParameter("Model_GDPS", "Srážky_MAIN")); 
            Parameters.Add("Srážky EURO4", GetParameter("Model_EURO4", "Srážky_MAIN")); 
            Parameters.Add("Srážky HIRLAM", GetParameter("Model_HIRLAM_Nový", "Srážky_MAIN_Nový"));
            Parameters.Add("Srážky HIRLAM Starý", GetParameter("Model_HIRLAM_Starý", "Srážky_MAIN_Starý"));
            Parameters.Add("Srážky WRF-NMM", GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN")); 
            Parameters.Add("Srážky WRF-ARW", GetParameter("Model_WRF_ARW", "Srážky_MAIN"));
            Parameters.Add("Srážky GFS", GetParameter("Model_GFS_Wetterzentrale_DE_25km", "Srážky_MAIN_Nový"));
            Parameters.Add("Srážky GFS Starý", GetParameter("Model_GFS_Wetterzentrale_DE_25km_STARY", "Srážky_MAIN_Starý"));

            //Směry větrů
            Parameters.Add("Směr větru 1000 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_10m", true, "REAL")); 
            Parameters.Add("Směr větru 850 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_850", true, "REAL"));
            Parameters.Add("Směr větru 700 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700", true, "REAL")); 
            Parameters.Add("Směr větru 600 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_600", true, "REAL"));
            Parameters.Add("Směr větru 500 hPa", GetParameter("Model_WRF_NMM_FLYMET", "Vítr_500", true, "REAL")); 
            Parameters.Add("Směr větru 400 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_400", true, "REAL")); 
            Parameters.Add("Směr větru 300 hPa", GetParameter("Model_GFS_FLYMET_50km", "Vítr_300", true, "REAL"));

            //Příprava dat pro výpočet střihu větru
            if (Parameters["Směr větru 1000 hPa"] != -1) windDirections.Add(Parameters["Směr větru 1000 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 850 hPa"])&& Parameters["Směr větru 850 hPa"] !=-1) windDirections.Add(Parameters["Směr větru 850 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 700 hPa"]) && Parameters["Směr větru 700 hPa"] != -1) windDirections.Add(Parameters["Směr větru 700 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 600 hPa"]) && Parameters["Směr větru 600 hPa"] != -1) windDirections.Add(Parameters["Směr větru 600 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 500 hPa"]) && Parameters["Směr větru 500 hPa"] != -1) windDirections.Add(Parameters["Směr větru 500 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 400 hPa"]) && Parameters["Směr větru 400 hPa"] != -1) windDirections.Add(Parameters["Směr větru 400 hPa"]);
            if(!windDirections.Contains(Parameters["Směr větru 300 hPa"]) && Parameters["Směr větru 300 hPa"] != -1) windDirections.Add(Parameters["Směr větru 300 hPa"]);
            

            //Slovník pro Návětrný efekt
            mapWindwardEffect = new Dictionary<float, float> {
                {-1, 0},
                {0f, Parameters["Návětrný efekt S"]},
                {1f, Parameters["Návětrný efekt S"]},
                {2f, Parameters["Návětrný efekt V"]},
                {3f, Parameters["Návětrný efekt J"]},
                {4f, Parameters["Návětrný efekt J"]},
                {5f, Parameters["Návětrný efekt J"]},
                {6f, Parameters["Návětrný efekt Z"]},
                {7f, Parameters["Návětrný efekt S"]}
            };

            //Slovník pro Závětrný efekt
             mapLeeEffect = new Dictionary<float, float> {
                {-1, 0},
                {0f, Parameters["Závětrný efekt S"]},
                {1f, Parameters["Závětrný efekt S"]},
                {2f, Parameters["Závětrný efekt V"]},
                {3f, Parameters["Závětrný efekt J"]},
                {4f, Parameters["Závětrný efekt J"]},
                {5f, Parameters["Závětrný efekt J"]},
                {6f, Parameters["Závětrný efekt Z"]},
                {7f, Parameters["Závětrný efekt S"]}
            };

            //Parametry pro suchý downburst
            Parameters.Add("RH 1000 hPa Real", GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000",true, "REAL")); //75 
            Parameters.Add("RH 925 hPa Real", GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_925",true, "REAL")); //60 
            Parameters.Add("RH 850 hPa Real", GetParameter("Model_GFS_Meteomodel_PL_25km", "Relativní_vlhkost_850",true, "REAL")); //75 
            Parameters.Add("LCL Real", GetParameter("Model_WRF_ARW", "LCL_Výška_základny_oblaku", true, "REAL")); //1200 

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

        private void WriteToDatabase() {
            if (TestCondition())
            {
                CloudOutputData mainOutput = new CloudOutputData(id_orp, sampleName, Output["1. RIZIKO PŘÍVALOVÉ POVODNĚ"], Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutput);
                CloudOutputData stormIntensityOutput = new CloudOutputData(id_orp, sampleName, Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Util.algorithmOutput["PŘEDPOVĚĎ INTENZITY BOUŘÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(stormIntensityOutput);
                CloudOutputData precipitationPlaceOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU BOUŘEK"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceOutput);
                CloudOutputData precipitationPlaceKoefOutput = new CloudOutputData(id_orp, sampleName, Output["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK - KOEFICIENT"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceKoefOutput);
                CloudOutputData torrentialRainOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - PŘÍVALOVÉ SRÁŽKY"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(torrentialRainOutput);
                CloudOutputData strongWindscreensOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutput);
                CloudOutputData hailOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - KRUPOBITÍ"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA KRUPOBITÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(hailOutput);
                CloudOutputData supercelarTornadosOutput = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - SUPERCELÁRNÍ TORNÁDA"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(supercelarTornadosOutput);
                if (drydownburst)
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(id_orp, sampleName, Output["MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }
                else
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }

                if (Output.ContainsKey("1. RIZIKO PŘÍVALOVÉ POVODNĚ - SUCHÝ"))
                {
                    CloudOutputData mainOutputDry = new CloudOutputData(id_orp, sampleName, Output["1. RIZIKO PŘÍVALOVÉ POVODNĚ - SUCHÝ"], Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - SUCHÁ VARIANTA"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputDry);
                }
                else {
                    CloudOutputData mainOutputDry = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - SUCHÁ VARIANTA"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputDry);
                }

                if (Output.ContainsKey("1. RIZIKO PŘÍVALOVÉ POVODNĚ - VLHKÝ"))
                {
                    CloudOutputData mainOutputWet = new CloudOutputData(id_orp, sampleName, Output["1. RIZIKO PŘÍVALOVÉ POVODNĚ - VLHKÝ"], Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - VLHKÁ VARIANTA"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputWet);
                }
                else {
                    CloudOutputData mainOutputWet = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - VLHKÁ VARIANTA"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputWet);
                }
            }
            else {
                //Util.l($"Pro {sampleName}. interval a {Name_orp} nic NEPOČÍTÁM!");
                CloudOutputData mainOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutput);
                CloudOutputData stormIntensityOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ INTENZITY BOUŘÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(stormIntensityOutput);
                CloudOutputData precipitationPlaceOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceOutput);
                CloudOutputData precipitationPlaceKoefOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceKoefOutput);
                CloudOutputData torrentialRainOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(torrentialRainOutput);
                CloudOutputData strongWindscreensOutput = new CloudOutputData(id_orp, sampleName,-1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutput);
                CloudOutputData hailOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA KRUPOBITÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(hailOutput);
                CloudOutputData supercelarTornadosOutput = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(supercelarTornadosOutput);
                CloudOutputData strongWindscreensOutputSD = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                CloudOutputData mainOutputDry = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - SUCHÁ VARIANTA"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputDry);
                CloudOutputData mainOutputWet = new CloudOutputData(id_orp, sampleName, -1, Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÉ POVODNĚ - VLHKÁ VARIANTA"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutputWet);

            }
        }

        private void DoCountOperations()
        {
            LoadParameters();
            PrecipitationTime();
            //Otestuje jestli většina srážkových modelů hlásí srážky
            if (TestCondition())
            {
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
                WriteOutputLog();
            }
            //WriteToDatabase();
        }

        //8. Sloučení B (DEN) - Intenzita bouřek a Lokální předpověď
        private void MergeB() {
            List<float> values;
            if (IsDay())
            {
                values = new List<float>() { Output["TEPLOTNÍ VLIVY ZEMSKÉHO POVRCHU"], Output["VĚTRNÉ VLIVY"], Output["VLHKOSTNÍ VLIVY"], Output["NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT"] };

            }
            else {
                values = new List<float>() { Output["VĚTRNÉ VLIVY"], Output["VLHKOSTNÍ VLIVY"], Output["NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT"] };
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

            //Silné nárazy větru - suchý downburst
            if (drydownburst) {
                values = new List<float>() { Output["SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST"], Output["MÍSTO VÝSKYTU - NEBEZPEČNÉ JEVY"] };
                level = ValueToLevel(StormIntensityScale, Probability(values));
                Output.Add("MÍSTO VÝSKYTU - SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST", level);
            }

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
            values = new List<float>() { Parameters["Stupeň nasycení"], Parameters["Suma srážek (1.hod.)"], Output["MÍSTO VÝSKYTU BOUŘEK"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Output["POHYB BOUŘE"], Output["NEBEZPEČNÉ JEVY"] };
            level = ValueToLevel(TorrentialFloodRiscScale, Probability(values));
            Output.Add("1. RIZIKO PŘÍVALOVÉ POVODNĚ", level);
            level = ValueToLevel(TorrentialFloodRiscScale2, Probability(values));
            Output.Add("2. RIZIKO PŘÍVALOVÉ POVODNĚ", level);
                                                        
            values = new List<float>() { Parameters["Stupeň nasycení"], Parameters["Suma srážek (1.hod.)"], Output["MÍSTO VÝSKYTU BOUŘEK"], Output["INTENZITA SILNÝCH - EXTRÉMNĚ SILNÝCH BOUŘEK (DEN) 2"], Output["POHYB BOUŘE"], Output["NEBEZPEČNÉ JEVY"] };
            if (Parameters["Stupeň nasycení max"] <= 1.5){
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
            if (IsDay())
            {
                List<float> values = new List<float>() { Parameters["Sklonitost reliéfu (průměrná)"], Parameters["Orientace reliéfu (tepelný prohřev)"], Parameters["Světelnost od JZ (Z-factor)"], Parameters["Vegetace-pokrytí (%)"],
                                                    Parameters["Teplotní gradient (nadmořská výška)"], Parameters["IR kontrast"], Parameters["Teplota (MAX)"], Parameters["Oblačnost"]
                                                    };
                int level = ValueToLevel(LevelScale, Probability(values));
                Output.Add("TEPLOTNÍ VLIVY ZEMSKÉHO POVRCHU", level);
            }
        }

        //Větrné vlivy
        private void WindInfluences()
        {
            List<float> values = new List<float>() { Parameters["Sídelní útvar"], Parameters["Šířka údolí"], Parameters["Obtékání překážky"], Parameters["Rychlost větru v 10 m nad terénem v m/s"] };
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
        //Zjistit jak konkrétně budou vypočtené parametry využívány a počítány! 
        private void WindEffect()
        {
            List<float> windwardValues, leeValues;
            List<float> windEffectValues = new List<float>();
            int windwardLevel, leeLevel;

            if (mapWindwardEffect[Parameters["Směr větru 600 hPa"]] == 1){
                windwardValues = new List<float>() { Parameters["Polohy nadmořských výšek"], Parameters["Hřeben"], Parameters["GRAD 925-700 hPa"], Parameters["MXR"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["OROGRAPHIC LIFT"], Parameters["Rychlost větru v 850 hPa"] };
                windwardLevel = ValueToLevel(LevelScale, Probability(windwardValues));
                windEffectValues.Add(windwardLevel);
            }
            else {
                windEffectValues.Add(0);
            }

            if (mapLeeEffect[Parameters["Směr větru 600 hPa"]] == 1){
                leeValues = new List<float>() { Parameters["GRAD 850-500 hPa"], Parameters["KONV+/DIV- (0-1 km)"], Parameters["Rychlost větru v 10 m nad terénem v m/s"] };
                leeLevel = ValueToLevel(LevelScale, Probability(leeValues));
                windEffectValues.Add(leeLevel);
            }
            else{
                windEffectValues.Add(0);
            }

            int windEffectLevel = ValueToLevel(OroKonvScale, Probability(windEffectValues));

            Output.Add("NÁVĚTRNÝ+ZÁVĚTRNÝ EFEKT", windEffectLevel);

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
                values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                Parameters["SREH 3 km"],Parameters["SWEAT"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };
                
            }
            else
            {
                values = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Parameters["Pwater"], Output["RH 1000-850 hPa"], Parameters["DLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 300 hPa"], Parameters["MXR"], Parameters["SHIP"] };
                
            }
            Output.Add("KRUPOBITÍ", DangerousPhenomenaCount(weights, values));
        }

        //DEN - Silné nárazy větru
        private void StrongWindImpact()
        {
            List<float> weights = new List<float>() { 3, 2, 2, 3, 3, 2, 2, 1, 1, 2, 3 };
            List<float> RHReal = new List<float>() { Parameters["RH 1000 hPa Real"] , Parameters["RH 925 hPa Real"] , Parameters["RH 850 hPa Real"] };

            //Podmínka pro suchý downburst
            if (Output.ContainsKey(this.sampleName) && Output[this.sampleName] < 0.57 && Average(RHReal) < 40 && Parameters["LCL Real"] > 1500)
            {
                //Nastal suchý downburst - do analýzy se dostanou nové KOEFICIENTY.
                drydownburst = true;
                float[] boundariesLCL = new float[3] { 1500, 2000, 2500 };
                float[] boundariesRH = new float[3] { 20, 30, 40 };

                Parameters.Add("LCL SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["LCL Real"]));
                Parameters.Add("RH 1000 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 1000 hPa Real"]));
                Parameters.Add("RH 925 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 925 hPa Real"]));
                Parameters.Add("RH 850 hPa SD", ChangeValueOfParameterLCL(boundariesLCL, Parameters["RH 850 hPa Real"]));

                List<float> valuesDryDownburst = new List<float>() { Parameters["RH 1000 hPa SD"], Parameters["RH 925 hPa SD"], Parameters["RH 850 hPa SD"] };
                Output.Add("RH 1000-850 hPa SD", ValueToLevel(LevelScale, Probability(valuesDryDownburst)));


                List<float> valuesSD;
                if (IsDay())
                {
                    valuesSD = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa SD"], Parameters["LCL SD"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                    Output.Add("SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST", DangerousPhenomenaCount(weights, valuesSD));
                }
                else
                {
                    valuesSD = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa SD"], Parameters["LCL SD"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                    Output.Add("SILNÉ NÁRAZY VĚTRU - SUCHÝ DOWNBURST", DangerousPhenomenaCount(weights, valuesSD));
                }
            }
            else {
                drydownburst = false;
            }

            List<float> values;
            if (IsDay())
            {
                values = new List<float>() { Parameters["MLCAPE"], Parameters["LI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                Output.Add("SILNÉ NÁRAZY VĚTRU", DangerousPhenomenaCount(weights, values));
            }
            else {
                values = new List<float>() { Parameters["MUCAPE"], Parameters["SI"], Parameters["GRAD 850-500 hPa"], Output["RH 1000-850 hPa"], Parameters["LCL"], Parameters["DLS"], Parameters["LLS"],
                Parameters["SREH 3 km"], Parameters["SWEAT"], Parameters["Rychlost větru v 850 hPa"], Parameters["DTHE"]};
                Output.Add("SILNÉ NÁRAZY VĚTRU", DangerousPhenomenaCount(weights, values));
            }
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
        private void StormMoving()
        {

            //Output.Add("ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa", -1); //

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

            List<float> values = new List<float>() { Parameters["MCS VEKTOR"], Output["ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa"]};
            values.Add(stormVectorLevel);

            int stormMovement = ValueToLevel(LevelScale, Probability(values));
            stormMovement = (stormMovement == 0 || stormMovement == 1) ? 0 : 1;

            Output.Add("POHYB BOUŘE", stormMovement);
        }

        //Organizace konv. bouře
        private void ConvectiveStormOrganization()
        {
            //Převést vstupní parametry na koeficienty podle stupnice viz adr. struktura!
            List<float> values = new List<float>() { Parameters["Rychlost větru v 300 hPa"], Parameters["Rychlost větru v 850 hPa"] };
            int convStormOrg = ValueToLevel(OroKonvScale, Probability(values));
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
            //windDirections
            float windCutCoef = -1;
            if (windDirections.Count() != 0) {
                if (windDirections.Count() == 1) { windCutCoef = 0; }
                else {
                    for (int i = 0; i < windDirections.Count()-1; i++) {
                        for (int j = i+1; j < windDirections.Count(); j++) {
                            float diff = WindDirectionDifference(windDirectioncoordinates[(int)windDirections[i]], windDirectioncoordinates[(int)windDirections[j]]);
                            if(diff>windCutCoef) windCutCoef = diff;
                            if (windCutCoef == 3) break;
                        }
                        if (windCutCoef == 3) break;
                    }
                }
            }

            Output.Add("STŘIH VĚTRU", windCut);
            Output.Add("ZMĚNA SMĚRU VĚTRU (1000 - 300) hPa", windCutCoef);
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

            int KonvDiv = ValueToLevel(OroKonvScale, Probability(konvDivValues));            
            orographicSupportValues.Add(KonvDiv);
            int OrographicLift = ValueToLevel(OroKonvScale, Probability(oroOutputValues));
            orographicSupportValues.Add(OrographicLift);
            int OrographicSupport = ValueToLevel(LevelScale, Probability(orographicSupportValues));
            values.Add(OrographicSupport);
            values.Add(Output["RH 1000-300 hPa"]);
            int triggeringConv = ValueToLevel(LevelScale, Probability(values));
            Output.Add("SPOUŠTĚNÍ KONVEKCE", triggeringConv);
        }


        //Denní instabilita
        private void DayInstability() {
            List<float> values = new List<float>() { Parameters["MLCIN"], Parameters["TT index"], Parameters["KI"], Parameters["GRAD 850-500 hPa"], Parameters["WETBULB"]};
            List<float> values2;
            if (IsDay())
            {
                values2 = new List<float>() {Parameters["MLCAPE"], Parameters["LI"]  };
            }
            else {
                values2 = new List<float>() {Parameters["MUCAPE"], Parameters["SI"]  };
            }

            float probabilityMLCAPELI = Probability(values2);
            int MLCAPELI = ValueToLevel(LevelScale, probabilityMLCAPELI);
            values.Add(MLCAPELI);
            float probability = Probability(values);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("DENNÍ INSTABILITA ATMOSFÉRY", level);

        }

        //Relativní vlhkost - potřebuje přidat podmínku pro suchý downburst
        private void RelativeHumidity() {
            //Suchý downburst
            List<float> values = new List<float>(){Parameters["RH 1000 hPa"], Parameters["RH 925 hPa"], Parameters["RH 850 hPa"]
                , Parameters["RH 700 hPa"], Parameters["RH 500 hPa"], Parameters["RH 300 hPa"] };
            float probability = Probability(values);
            int level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 1000-300 hPa", level);
            
            /*
            values = new List<float> (){Parameters["RH 850 hPa"], Parameters["RH 700 hPa"], Parameters["RH 500 hPa"], Parameters["RH 300 hPa"] };
            probability = Probability(values);
            level = ValueToLevel(LevelScale, probability);
            Output.Add("RH 850-300 hPa", level);*/

            values = new List<float>(){Parameters["RH 1000 hPa"], Parameters["RH 925 hPa"], Parameters["RH 850 hPa"]};
            probability = Probability(values);
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
        }

        //Pomocné výpočetní funkce

        //Výpočet rozdílu směru větru + převedení na koeficient
        private float WindDirectionDifference(float[] first, float[] second) {
            float distance = 0;
            float coeficient = 0;
            //Výpočet míry střihu větru
            distance = (float) Math.Sqrt(Math.Pow(second[0] - first[0], 2) + Math.Pow(second[1]-first[1],2));
            //Přepočet na koeficient
            coeficient = (distance > 3.8f) ? 3 : (distance > 3) ? 2 : (distance > 0) ? 1 : 0;
            return coeficient;
        }


        private bool TestCondition() {
            return (Output["0"] >= precipitationTreshold) ? true : false;
        } 

        //Převod hodnoty pravděpodobnosti na úroveň dle tabulky
        private int ValueToLevel(List<float> levels, float value) {
            int level = -1;
            foreach (var l in levels) {

                level = (value <= l) ? levels.IndexOf(l) : -1;
                if (level != -1) break;
            }
            return level;
        }
        //Výpočet pravděpodobnosti
        private float Probability(List<float> list) {
            List<float> values = TestParameters(list);
            if (values.Count!=0) return (float)Math.Round((double)(new decimal(SumArray(values) / (values.Count * RATIO))), precision);
            else { return -1; }
        }

        private float ProbabilityWeights(List<float> arr, List<float> weights)
        {
            float sum = 0;
            float sumWeights = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr.ElementAt(i) == -1) { weights[i] = 0; }
                sum += arr.ElementAt(i) * weights.ElementAt(i);
            }

            foreach (var w in weights)
            {
                sumWeights += w * RATIO;
            }
            if (sumWeights != 0) return (float)Math.Round((double)(new decimal(sum / sumWeights)), 2);
            else return 0;
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
            Util.l($"{this.Name_orp}:{this.sampleName}");
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
        private float GetParameter(string model, string submodel, bool sample = true,  string type = "DEFAULT", bool usePreviousSample = false) {
            float value;
            if (usePreviousSample) {
                value = Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), previousSample, id_orp, typeValueDictionary[type]);
            }
            else if (sample)
            {
                value = Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sampleName, id_orp, typeValueDictionary[type]);
            }
            else {
                value = Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), "", id_orp, typeValueDictionary[type]);
            }

           // Util.l($"{model}:{submodel}:{type}:{sampleName}:{value}");
            //Preloader.Log("ABC");
            return value;
        }

        private float GetRelief(string name) {
            float value = 0;

            value = Model.Cloud.RELIEFCHARVALUESGetValueForName(id_orp, name);
            //value = -1;//Vypnutí charakteristik reliéfu (pro debug)
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

        //Test na den/noc
        private bool IsDay() {
            if (sampleName == "21" || sampleName == "00" || sampleName == "03" || sampleName == "06" || sampleName == "24" ||
                sampleName == "27" || sampleName == "30" || sampleName == "45" || sampleName == "48")
                return false;
            else
                return true;
        }

        private List<float> TestParameters(List<float> list) {
            List<float> values = new List<float>();
            foreach (var l in list)
            {
                if(l!=-1) values.Add(l);
            }
            return values;
        }
    }
}
