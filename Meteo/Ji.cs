using System;
using System.Collections.Generic;
using System.IO;
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
            ModelSpectrumScaleForModel();
            ORPS_GetORPNames();
            //Pomocné nahrávání dat do DB z CSV souborů
            //ReadCSVFileORPS(@"ObceSRozsirenouPusobnosti_CR.csv");
            //ReadCSVFileORPColor(@"PaletaBarev.csv");
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
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetAllColorsForModel("Srážky_MAIN");
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
            //Je možno použít u IDček - první 2 položky - i konstruktor, který přijímá int hodnoty. 
            CloudMaskSpectrum record = new CloudMaskSpectrum("Model_ALADIN_CZ","Zlín","#FUNGUJ","{}");
            Model.Cloud.MaskSpectrumInsertOrUpdate(record);

        }

        public void ModelSpectrumScaleForModel() {
            List<CloudModelSpectrum> records = Model.Cloud.ModelSpectrumGetScaleForModels("Model_ALADIN_CZ", "Srážky_MAIN");
            foreach (var r in records)
            {
                Util.l($"Barva: {r.color} Rank: {r.rank}");
            }
        }

        public void ORPS_GetORPNames()
        {
            List<CloudORPS> l = Model.Cloud.ORPSGetORPNames();
            foreach (var ll in l)
            {
                Util.l($"ID: {ll.id} Název Okresu: {ll.name}");
            }
        }

        public void ReadCSVFileORPS(string filename) {
            using (var reader = new StreamReader(filename)) {
                List<CloudORPS> listOfRecords = new List<CloudORPS>();
                var header = reader.ReadLine(); //načte hlavičku
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    
                    CloudORPS record = new CloudORPS(values[0], values[1]);
                    listOfRecords.Add(record);
                }

                Util.l(header);
                foreach (var r in listOfRecords) {
                    Util.l($"{r.id}:{r.name}");
                    Model.Cloud.ORPSInsertOrUpdate(r);
                }
            }
        }

        public void ReadCSVFileORPColor(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                List<CloudORPColor> listOfRecords = new List<CloudORPColor>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    CloudORPColor record = new CloudORPColor(values[0], values[1]);
                    listOfRecords.Add(record);
                }

                foreach (var r in listOfRecords)
                {
                    Util.l($"{r.id_orp}:{r.color}");
                    Model.Cloud.ORPColorInsertOrUpdate(r);
                }
            }
        }
    }
}
