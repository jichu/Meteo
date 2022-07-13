using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public class CloudSamples
    {
        public string sample_name { get; set; }

        public List<CloudORPS> ORPS { get; set; } = Model.Cloud.ORPSGetORPNames();

        public int windDirection { get; set; } = Util.windDirectionToInt["J"];  //J(4), JZ(5) || ostatni 0-3, 6-7

        public string convectionTypeMajor { get; set; }
        public List<string> convTypesAll = new List<string>();
        public List<string> convTypesKeys = new List<string>();
        public string convectionSuperTypeMajor { get; set; }
        public List<string> convSuperTypesAll = new List<string>();
        public List<string> convSuperTypesKeys = new List<string>();
        public CloudSamples()
        {

        }

        public void LoadORPS()
        {
            foreach (var orp in ORPS)
            {
                orp.LoadData(sample_name);
            }
        }

        public void CountMajorConvectionType()
        {
            int count = 0;

            foreach (var item in convTypesKeys)
            {
                if (convTypesAll.Count(i => i == item) > count)
                {
                    count = convTypesAll.Count(i => i == item);
                    convectionTypeMajor = item;
                }
            }

            count = 0;

            foreach (var item in convSuperTypesKeys)
            {
                if (convSuperTypesAll.Count(i => i == item) > count)
                {
                    count = convSuperTypesAll.Count(i => i == item);
                    convectionSuperTypeMajor = item;
                }
            }
        }
    }
}
