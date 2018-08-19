using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudModels
    {
        public int id { get; set; }
        public string name { get; set; }
        public int id_parent { get; set; }
        public string options { get; set; }

        public CloudModels() {
        }

        public CloudModels(string name, string nameParent = null, string options = null)
        {
            this.name = name;
            if (nameParent != null) {
                id_parent = Model.Cloud.MODELSGetIDFromName(nameParent);
                if (id_parent == 0) {
                    Model.Cloud.MODELSInsertOrUpdateParent(nameParent);
                    id_parent = Model.Cloud.MODELSGetIDFromName(nameParent);
                };
            }
            if (options != null) this.options = options;
        }
    }
}
