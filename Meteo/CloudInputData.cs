using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class CloudInputData
    {
        public int id_model { get; set; }
        public int id_orp { get; set; }
        public string sample_name { get; set; }
        public float value { get; set; }

        public CloudInputData() { }

        public CloudInputData(string namModel, string namSubmodel, string namORP, string sample_name, float value) {
            id_model = Model.Cloud.MODELSGetSubmodelIDFromName(namModel,namSubmodel);
            id_orp = Model.Cloud.ORPSGetIDFromName(namORP);
            this.sample_name = sample_name;
            this.value = value;
        }
    }
}
