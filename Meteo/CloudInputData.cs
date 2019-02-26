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

        }

        public CloudInputData(string namModel, string namSubmodel, string namORP, string sample_name, float value) {
            id_model = Model.Cloud.MODELSGetSubmodelIDFromName(namModel,namSubmodel);
            this.sample_name = sample_name;
            this.value = value;

            try { id_orp = Model.Cloud.ORPSGetIDFromName(namORP);
                    region = false;
            }
            catch (InvalidOperationException e) {
                region = true;
            }

            if (region) {
                try {
                    id_orp = Model.Cloud.REGIONSGetIDFromName(namORP);
                }
                catch (InvalidOperationException e) {
                    Util.l("Neexistující obec nebo region"+e);
                    id_orp = -1;
                }
            }

        }
    }
}
