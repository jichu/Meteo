using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudInputData
    {
        public int id_model { get; set; }
        public int id_orp { get; set; }
        public string sample_name { get; set; }
        public float value { get; set; }
        public bool region { get; set; }
        public int type { get; set; }

        public CloudInputData() {
            value = 0;
        }

        public CloudInputData(float value)
        {
            this.value = value;
        }

        public CloudInputData(CloudInputData item) {
            id_model = item.id_model;
            id_orp = item.id_orp;
            sample_name = item.sample_name;
            value = item.value;
            region = item.region;
            type = item.type;
        }

        public CloudInputData(string namModel, string namSubmodel, string namORP, string sample_name, float value, string type = "DEFAULT") {
            int numberOfRegions = 14; //Počet krajů v zemi
            id_model = Model.Cloud.MODELSGetSubmodelIDFromName(namModel,namSubmodel);
            this.type = Model.Cloud.ModelSpectrumTypeGetIDForName(type);
            this.sample_name = sample_name;
            this.value = value;
            region = (Model.Cloud.MODELSGetNumberOfAreasForModel(namModel)<=numberOfRegions) ? true : false;           
            if (region) id_orp = Model.Cloud.ORPSGetRegionForORP(namORP);
            else id_orp = Model.Cloud.ORPSGetIDFromName(namORP);
        }
    }
}
