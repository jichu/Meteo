using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class PrecipitationFilter
    {
        public List<CloudORPS> ORPList { get; set; }
        public List<CloudSamples> samplesList { get; set; }
        public List<int> id_regions { get; set; } = new List<int>();
        public List<CloudSamples> finalSampleList { get; set; } = new List<CloudSamples>();

        public Dictionary<string, int> typeValueDictionary = new Dictionary<string, int>
        {
            { "DEFAULT", 0 },
            { "CONVECTIVE", 1 },
            { "RASTER", 2 },
            { "REAL", 3 },
            { "WIND", 4}
        };

        public PrecipitationFilter(List<CloudORPS> ORPList, List<CloudSamples> samplesList) {
            Util.l("Filtrování podle srážkových modelů");
            this.ORPList = ORPList;
            this.samplesList = samplesList;

            foreach (var s in samplesList) {
                foreach (var orp in ORPList) {
                    //For debug only
                    /*Util.l($"Sample: {s.sample_name} ORP:{orp.name} value: {GetParameter("Model_ALADIN_CZ", "Srážky_MAIN", s.sample_name, orp.id)} ");
                    Util.l($"Sample: {s.sample_name} ORP:{orp.name} value: {GetParameter("Model_WRF_ARW", "Srážky_MAIN", s.sample_name, orp.id)} ");
                    Util.l($"Sample: {s.sample_name} ORP:{orp.name} value: {GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN", s.sample_name, orp.id)} ");*/

                    if (GetParameter("Model_ALADIN_CZ", "Srážky_MAIN", s.sample_name, orp.id) +
                        GetParameter("Model_WRF_ARW", "Srážky_MAIN", s.sample_name, orp.id) +
                        GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN", s.sample_name, orp.id) >= 2f) {
                        finalSampleList.Add(s);
                        break;
                    }
                }        
            }
            foreach (var orp in finalSampleList.First().ORPS) {
                if (!(id_regions.Contains(orp.id_region)))
                {
                    id_regions.Add(orp.id_region);
                }
            }


            foreach (var s in finalSampleList){
                foreach (var orp in s.ORPS){
                    orp.precipitationResult = (GetParameter("Model_ALADIN_CZ", "Srážky_MAIN", s.sample_name, orp.id) + GetParameter("Model_WRF_ARW", "Srážky_MAIN", s.sample_name, orp.id) + GetParameter("Model_WRF_NMM_FLYMET_Srážky", "Srážky_MAIN", s.sample_name, orp.id));
                }
                foreach (var region in id_regions){
                   int tempResult = 0; 
                   foreach (var orp in s.ORPS){
                        if (region == orp.id_region) {
                            if (orp.precipitationResult > tempResult) tempResult = (int) orp.precipitationResult;
                        }
                   }
                   foreach (var orp in s.ORPS){
                        if (region == orp.id_region)
                        {
                            orp.precipitationResultRegion = tempResult;
                        }
                    }
                }

            }

        }

        private float GetParameter(string model, string submodel, string sampleName, int id_orp, string type = "DEFAULT")
        {
            return Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sampleName, id_orp, typeValueDictionary[type]);
        }
    }
}
