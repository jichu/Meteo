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
        }

        private float GetParameter(string model, string submodel, string sampleName, int id_orp, string type = "DEFAULT")
        {
            return Model.Cloud.InputDataGetData(Model.Cloud.MODELSGetSubmodelIDFromName(model, submodel), sampleName, id_orp, typeValueDictionary[type]);
        }
    }
}
