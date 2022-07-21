using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudORPS
    {
        public int id { get; set; }
        public string name { get; set; }
        public int id_region { get; set; }

        public List<string> sampleNames = new List<string>{
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

        //Předpovědní parametry ORP
        //SRážkové modely
        public float aladin { get; set; }
        public float wrf_arw { get; set; }
        public float wrf_nmm { get; set; }

        //Ostatní
        public float wetBulb { get; set; } 
        public float corfidiVector { get; set; } 
        public float corfidiVectorLevel { get; set; } 
        public float wind_1000 { get; set; }
        public float wind_850 { get; set; }
        public float wind_700 { get; set; }
        public float wind_600 { get; set; }
        public float wind_500 { get; set; }
        public float wind_400 { get; set; }
        public float wind_300 { get; set; }
        public float temperature_850 { get; set; }
        public float frontogenesis_850 { get; set; }
        public float dls { get; set; }
        public float mlcape { get; set; }
        public float mucape { get; set; }
        public float sreh_3km { get; set; }
        public float mixr { get; set; }
        public float potentional_orographic_lift { get; set; }
        public float pwater { get; set; }
        public float pressureMLSP { get; set; }
        public float mfdiv { get; set; }
        public float relativeVorticity { get; set; }
        public float rh_700 { get; set; }
        public float rh_1000 { get; set; }
        public float cloudy { get; set; }
        public float mlcin { get; set; }
        public float precipitationResult { get; set; }
        public float precipitationResultRegion { get; set; }

        //Charakteristiky reliéfu
        public float sklonitost_reliefu { get; set; }
        public float orientace_reliefu_tepelny_prohrev_dopoledne { get; set; }
        public float orientace_reliefu_tepelny_prohrev_odpoledne { get; set; }
        public float vegetace_pokryti { get; set; }
        public float ir_kontrast { get; set; }
        public float sidelni_utvar { get; set; }
        public float sirka_udoli { get; set; }
        public float obtekani_prekazky { get; set; }
        public float polohy_nadmorskych_vysek { get; set; }
        public float sirka_hrebene { get; set; }
        public float sum_merge { get; set; }

        //Typy konvekce
        public float wavyColdFront { get; set; }
        public float wavyColdFrontS { get; set; }
        public float coldFront { get; set; }
        public float coldOcclusion { get; set; }
        public float warmOcclusion { get; set; }
        public float warmOcclusionS { get; set; }
        public float quasifontalConvection { get; set; }
        public float orographicConvection { get; set; }
        public float orographicConvectionConvergenceLine { get; set; }

        public Dictionary<string, float> convectionTypeResults = new Dictionary<string, float>
        {
            { "Zvlněná studená fronta", 0f },
            { "Zvlněná studená fronta - supercelární bouře", 0f },
            { "Studená fronta", 0f },
            { "Studená okluze", 0f },
            { "Teplá okluze", 0f},
            { "Teplá okluze - supercelární bouře", 0f},
            { "Kvazifrontální konvekce", 0f},
            { "Orografická konvekce", 0f},
            { "Orografická konvekce - linie konvergence", 0f}
        };

        public Dictionary<string, string> convectionTypeConversion = new Dictionary<string, string>
        {
            { "Zvlněná studená fronta", "FRONTÁLNÍ (Teplá instabilní hmota)" },
            { "Zvlněná studená fronta - supercelární bouře", "FRONTÁLNÍ (Teplá instabilní hmota)"  },
            { "Studená fronta", "FRONTÁLNÍ (Studená instabilní hmota)"  },
            { "Studená okluze", "FRONTÁLNÍ (Studená instabilní hmota)"  },
            { "Teplá okluze", "FRONTÁLNÍ (Teplá instabilní hmota)"},
            { "Teplá okluze - supercelární bouře", "FRONTÁLNÍ (Teplá instabilní hmota)"},
            { "Kvazifrontální konvekce", "KVAZIFRONTÁLNÍ (Studená instabilní hmota)"},
            { "Orografická konvekce", "OROGRAFICKÁ (Teplá instabilní hmota)"},
            { "Orografická konvekce - linie konvergence", "OROGRAFICKÁ (Teplá instabilní hmota)"}
        };

        public Dictionary<string, float> convectionTypes { get; set; } = new Dictionary<string, float>();
        public string convectionTypesStringForm { get; set; }
        public string convectionSuperTypesStringForm { get; set; }

        //Místa výskytu srážek
        public int warmWetSectorPlace { get; set; }
        public int coldSectorPlace { get; set; }
        public int combineSectorPlace { get; set; }

        //Předpověď lokálních podmínek
        public int temperatureInfluence { get; set; }
        public int windInfluence { get; set; }
        public int humidityInfluence { get; set; }
        public int orographicInfluence { get; set; }
        public int combineInfluence { get; set; }
        public int significantPredictors { get; set; }
        public int otherPredictors { get; set; }
        public int combineIntensity { get; set; }

        //Výsledné předpovědi
        public int statisticalPrecipitation { get; set; }
        public int finalPlace { get; set; }
        public int finalStorm { get; set; }

        public Dictionary<string, string> output { get; set; } = new Dictionary<string, string>();

        public CloudORPS() {
        }
        public CloudORPS(string id, string name)
        {
            this.id = Int32.Parse(id);
            this.name = name;
            
        }
        public void LoadData(string sample_name) {
            //Srážkové parametry
            aladin = GetParameter("Model_ALADIN_CZ", "Srážky_MAIN", sample_name, id);
            wrf_arw = GetParameter("Model_WRF_ARW", "Srážky_MAIN", sample_name, id);
            wrf_nmm = GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN", sample_name, id);


            //přepovědní parametry
            wetBulb = GetParameter("Model_GFS_Wetter3_DE_25km", "Wet_bulb_temp", sample_name, id, "REAL");
            wind_1000 = GetParameter("Model_WRF_NMM_FLYMET", "Vítr_10m", sample_name, id);
            wind_850 = GetParameter("Model_WRF_NMM_FLYMET", "Vítr_850", sample_name, id);
            wind_700 = GetParameter("Model_WRF_NMM_FLYMET", "Vítr_700", sample_name, id);
            wind_600 = GetParameter("Model_WRF_NMM_FLYMET", "Vítr_600", sample_name, id);
            wind_500 = GetParameter("Model_WRF_NMM_FLYMET", "Vítr_500", sample_name, id);
            wind_400 = GetParameter("Model_GFS_FLYMET_50km", "Vítr_400", sample_name, id);
            wind_300 = GetParameter("Model_GFS_FLYMET_50km", "Vítr_300", sample_name, id);
            frontogenesis_850 = GetParameter("Model_GFS_Wetter3_DE_50km", "Frontogeneze_parametr_850_hPa", sample_name, id, "REAL");
            sreh_3km = GetParameter("Model_WRF_ARW", "SRH_3km_WRF_ARW", sample_name, id, "REAL");
            mixr = GetParameter("Model_GFS_Lightning_Wizard_50km", "Mixing_Ratio_0-1km", sample_name, id, "REAL");
            potentional_orographic_lift = GetParameter("Model_WRF_ARW", "Potential_Orographic_Lift", sample_name, id, "REAL");
            pwater = GetParameter("Model_WRF_ARW", "Pwater", sample_name, id, "REAL");
            mfdiv = GetParameter("Model_WRF_ARW", "MFDIV_0-1km", sample_name, id, "REAL");
            rh_700 = GetParameter("Model_WRF_ARW", "Relativní_vlhkost_700", sample_name, id, "REAL");
            rh_1000 = GetParameter("Model_ALADIN_CZ", "Relativní_vlhkost_1000", sample_name, id, "REAL");
            cloudy = GetParameter("Model_ALADIN_CZ", "Oblačnost", sampleNames[sampleNames.IndexOf(sample_name) - 1], id); //Bere hodnotu z předchozího samplu
            pressureMLSP = GetParameter("Model_WRF_ARW", "Tlaková_tendence_MSLP", sample_name, id, "REAL");
            mlcape = GetParameter("Model_GFS_Wetter3_DE_25km", "MLCAPE+LI_Wetter_3_de", sample_name, id, "REAL");  
            mlcin = GetParameter("Model_GFS_Wetter3_DE_25km", "MLCIN_Wetter_3_de", sample_name, id, "REAL");
            temperature_850 = GetParameter("Model_GFS_Wetter3_DE_25km", "Teplota_850hPa", sample_name, id, "REAL");
            mucape = GetParameter("Model_GFS_Lightning_Wizard_50km", "MUCAPE", sample_name, id, "REAL");
            relativeVorticity = GetParameter("Model_WRF_ARW", "Relativni_vorticita_500_hPa_WRF", sample_name, id);


            dls = -1f;
            //dls = GetParameter("Model_GFS_Meteomodel_PL_25km", "SHEAR_DLS_Střih_větru_0-6_km", sample_name, id);//možná nebude k dispozici
            
            //charakteristiky reliéfu / statické parametry
            sklonitost_reliefu = GetRelief("Sklonitost reliéfu (průměrná)");
            orientace_reliefu_tepelny_prohrev_dopoledne = GetRelief("Orientace reliéfu (tepelný prohřev) dopoledne"); //pro sample: 6,9,30,33
            orientace_reliefu_tepelny_prohrev_odpoledne = GetRelief("Orientace reliéfu (tepelný prohřev) odpoledne"); //pro sample: 12,15,18,36,39,42
            vegetace_pokryti = GetRelief("Vegetace-pokrytí (%)");
            ir_kontrast = GetRelief("IR kontrast");
            sidelni_utvar = GetRelief("Sídelní útvar");
            sirka_udoli = GetRelief("Šířka údolí");
            obtekani_prekazky = GetRelief("Obtékání překážky");
            polohy_nadmorskych_vysek = GetRelief("Polohy nadmořských výšek");
            sirka_hrebene = GetRelief("Šířka hřebene");
            sum_merge = GetRelief("SUM MERGE");

        }

        public void ProcessConvectionTypes() {
            if (convectionTypes.Count >= 2)
            {
                foreach (var item in convectionTypes)
                {
                    convectionTypesStringForm += item.Key + ";";
                    convectionSuperTypesStringForm += convectionTypeConversion[item.Key] + ";";
                }
                //Util.l($"XXX:{convectionTypesStringForm}: {convectionSuperTypesStringForm}");
            }
            else {
                
                convectionTypesStringForm = convectionTypes.First().Key;
                convectionSuperTypesStringForm = convectionTypeConversion[convectionTypes.First().Key];
                //Util.l($"YYY:{convectionTypesStringForm}: {convectionSuperTypesStringForm}");
            }
        }

        private float GetParameter(string model, string submodel, string sampleName, int id_orp, string type = "DEFAULT")
        {
            return Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sampleName, id_orp, Util.typeValueDictionary[type]);
        }

        private float GetRelief(string name)
        {
            float value = 0;

            value = Model.Cloud.RELIEFCHARVALUESGetValueForName(id, name);
            //value = -1; //Vypnutí charakteristik reliéfu (pro debug)
            return value;
        }
    }

}
