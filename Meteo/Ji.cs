using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    class Ji
    {
        public Ji() {
            /*Util.l("Tohle je Jickovo hriste|kkk", new Dictionary<string, object>
            {
                { "messageBoxIcon", MessageBoxIcon.Stop }
            });*/
            //MaskSpectrumGetCoodsByColor();
            //MaskSpectrumGetAllColorsForModel();
            //MaskSpectrumInsertOrUpdate();
            //ModelSpectrumScaleForModel();
            //ORPS_GetORPNames();
            //ORPS_GetORPColors();
            //Pomocné nahrávání dat do DB z CSV souborů
            //ReadCSVFileORPS(@"ObceSRozsirenouPusobnosti_CR.csv");
            //ReadCSVFileORPColor(@"PaletaBarev.csv");

            //NEW

            //MODELS_InsertOrUpdate();
            //SETTING_GetSettings();
            SETTING_InsertOrUpdateSettings();
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

        public void ORPS_GetORPColors()
        {
            List<CloudORPColor> l = Model.Cloud.ORPColorGetORPColors();
            foreach (var ll in l)
            {
                Util.l($"ID_ORP: {ll.id_orp} Barva: {ll.color}");
            }
        }

        public void MODELS_InsertOrUpdate() {
            //Použití metody Model.Cloud.MODELSInsertOrUpdate() - přidání záznamu do tabulky MODELS
            //Model.Cloud.MODELSInsertOrUpdate(Nazev nového modelu (musí být zadán vždy)
                                             //,nazev rodičovského modelu (zadává se jen u podmodelů)
                                             //, nastavení (libovolně použitelné pole))
            
            //Vložení nového hlavního modelu (např. Model_ALADIN_CZ). 
            //Pokud již existuje v databázi hlavní model s tímto jménem, pak původní zůstává nezměnen.
            //CloudModels recordParent = new CloudModels("Nový root model");
            //Model.Cloud.MODELSInsertOrUpdate(recordParent);

            //Vložení podmodelu (např. Srážky_MAIN)
            //Druhý parametr je název rodičovského modelu. 
            //Pokud nelze v DB název dohledat, udělá se z podmodelu hlavní model.
            //CloudModels recordChild = new CloudModels("Nový child model", "Nový root model");
            //Model.Cloud.MODELSInsertOrUpdate(recordChild);

            //Přidání nastavení (používá se při určení metody výpočtu). Struktura není pevn dána.
            //Pole options si lze využít libovolně. V ukázce je pouze schematický návrh.
            //Pole options je také to jediné, co funkce  Model.Cloud.MODELSInsertOrUpdate umožňuje u jednotlivých položek aktualizovat.
            //Nelze tedy aktualizovat název modelu a rodiče.
            //CloudModels recordWithOptions = new CloudModels("Nový child model", "Nový root model", "{countMethod = average}");
            //Model.Cloud.MODELSInsertOrUpdate(recordWithOptions);

            //Příklady použití s reálnými daty

            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Model_ALADIN_CZ"));
            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Srážky_MAIN", "Model_ALADIN_CZ"));
            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Srážky_MAIN", "Model_ALADIN_CZ", "{countMethod = sum}"));
        }

        public void SETTING_GetSettings() {
            //načtení celého nastavení aplikace s výpisem jednotlivých parametrů nastavení
            List<CloudSettings> settings = Model.Cloud.SETTINGSGetSettings();
            foreach (var s in settings)
            {
                Util.l($"OPTION_NAME: {s.option_name} OPTION_VALUE: {s.option_value}");
            }
        }

        public void SETTING_InsertOrUpdateSettings()
        {
            //Přidání nebo změna existujícího parametru do nastavení aplikace
            //Pokud existuje položka s názvem, pak je pouze změněna hodnota parametru
            //new CloudSettings(Jméno parametru,Hodnota parametru);
            Model.Cloud.SETTINGSInsertOrUpdate(new CloudSettings("debug", "true|false"));

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
                var header = reader.ReadLine(); //načte hlavičku
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    //CloudORPColor record = new CloudORPColor(values[0], $"#{values[1].Substring(2,6)}");
                    CloudORPColor record = new CloudORPColor(values[0],values[1]);
                    listOfRecords.Add(record);
                }
                Util.l(header);
                foreach (var r in listOfRecords)
                {
                    Util.l($"{r.id_orp}:{r.color}");
                    Model.Cloud.ORPColorInsertOrUpdate(r);
                }
            }
        }
    }
}
