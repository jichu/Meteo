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
            MaskSpectrumGetAllColorsForModel();
            MaskSpectrumInsertOrUpdate();
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

        public void MaskSpectrumGetAllColorsForModel() {
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetAllColorsForModel(Model.Cloud.MODELSGetIDFromName("Model_ALADIN_CZ"));
            foreach (var ll in l)
            {
                Util.l("Barvy pro zvolený model: "+ ll.color);
                //ll.ShowRecord();
            }
        }

        public void MaskSpectrumInsertOrUpdate()
        {
            //Nastavení atributů se provádí přes klasický konstruktor - KULATÉ závorky namísto složených - v pořadí: Hlavní model, podmodel, okres, barva, souřadnice. 
            //Všechno mohou být stringy - tam, kde je v tabulce MaskSpectrum uvedeno číselné ID se provádí automatický překlad ze stringu na int.
            //Je potřeba dodržet ty názvy, které jsou již uložené v DB, jinak je nutno updatovat "překladové" tabulky.
            //Je možno použít u IDček - první 3 položky - i konstruktor, který přijímá int hodnoty. 
            CloudMaskSpectrum record = new CloudMaskSpectrum("Model_ALADIN_CZ", "Oblačnost","ZL","#FUNGUJ","{}");
            Model.Cloud.MaskSpectrumInsertOrUpdate(record);

        }
        
    }
}
