using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class Ji
    {
        public Ji() {
            Util.l("Tohle je Jickovo hriste");
            MaskSpectrumGetCoodsByColor();
            MaskSpectrumGetAllColors();
        }
        public void MaskSpectrumGetCoodsByColor() {
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetCoodsByColor("#fff");

            foreach (var ll in l)
            {
                //string json = @"{'coods':[[11,21],[12,22]]}";
                ll.ShowRecord();
                /*string jsonString = ll.coods;
                JObject jsonObj = JObject.Parse(jsonString);
                JArray posArray = (JArray)jsonObj["coods"];
                string posX = (string)jsonObj["coods"][0][0];
                string posY = (string)jsonObj["coods"][0][1];
                Util.l($"Barva: {ll.color}");
                Util.l("X:Y");
                foreach (JArray arr in posArray)
                {
                    Util.l(String.Join(":", arr.Select(i => i.ToString())));
                }*/
            }
        }

        public void MaskSpectrumGetAllColors() {
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetAllColors();
            foreach (var ll in l)
            {
                Util.l(ll.color);
                ll.ShowRecord();
            }
        }

        /*public void MaskSpectrumInsertOrUpdate()
        {
            CloudMaskSpectrum objekt = new CloudMaskSpectrum();
            objekt.id = "Model_ALADIN_CZ";
            objekt.id_model = "Oblačnost";
            objekt.id_orp = "ZL";
            objekt.coods = "{}";
            objekt.color = "#GGG";
            bool vystup = Model.Cloud.MaskSpectrumInsertOrUpdate(objekt);

        }*/
    }
}
