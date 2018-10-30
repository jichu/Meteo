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
            //ModelSpectrumGetScaleForModel();
            //ModelSpectrumTypeGetIdFroName();
            //ORP_COLOR_GetORPColors()
            //ORPS_GetORPNames();
            //ORPS_GetIDFromName();

            //Pomocné nahrávání dat do DB z CSV souborů
            //ReadCSVFileORPS(@"ObceSRozsirenouPusobnosti_CR.csv"); //Zde se používá ORPSInsertOrUpdate().
            //ReadCSVFileORPColor(@"PaletaBarev.csv"); //Zde se používá  ORPColorInsertOrUpdate().
            //ReadCSVFileModelSpectrum(@"Barvy_stupnice.csv");//Využívá ModelSpectrumInsertOrUpdate()
            //NEW

            //MODELS_InsertOrUpdate();
            //MODELS_GetIDFromName();
            //MODELS_GetSubmodelIDFromName();
            //MODELS_GetModelOptions();
            //REGIONS_GetRegionCities();
            //SETTING_GetSettings();
            //SETTING_InsertOrUpdateSettings();
            MODELS_GetModelStructure();
            //REGIONS_GetNameFromColor();
            //MODELS_GetNumberOfAreasForModel();

        }


        public void MaskSpectrumGetCoodsByColor() {
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetCoodsByColor("#fff", "Model_ALADIN_CZ");

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
            List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetAllColorsForModel("Model_ALADIN_CZ");
            foreach (var ll in l)
            {
                Util.l("Barvy pro zvolený model: " + ll.color);
                //ll.ShowRecord();
            }
        }

        public void MaskSpectrumInsertOrUpdate()
        {
            //Nastavení atributů se provádí přes klasický konstruktor - KULATÉ závorky namísto složených - v pořadí: Hlavní model, podmodel, okres, barva, souřadnice. 
            //Všechno mohou být stringy - tam, kde je v tabulce MaskSpectrum uvedeno číselné ID se provádí automatický překlad ze stringu na int.
            //Je potřeba dodržet ty názvy, které jsou již uložené v DB, jinak je nutno updatovat "překladové" tabulky.
            //Je možno použít u IDček - první 2 položky - i konstruktor, který přijímá int hodnoty. 
            CloudMaskSpectrum record = new CloudMaskSpectrum("Model_EURO4", "Zlín", "{}");
            Model.Cloud.MaskSpectrumInsertOrUpdate(record);

        }

        public void ModelSpectrumGetScaleForModel() {
            List<CloudModelSpectrum> records = Model.Cloud.ModelSpectrumGetScaleForModels("Model_GFS_Wetterzentrale_DE_25km", "Srážky_MAIN_Nový", "CONVECTIVE");//Jako třetí parametr lze využít: DEFAULT (nemusí se zadávat, je nastaven jako defaultní), CONVECTIVE, RASTER 
            foreach (var r in records)
            {
                Util.l($"Barva: {r.color} Rank: {r.rank}");
            }
        }

        public void ModelSpectrumTypeGetIdFroName() {

            Util.l(Model.Cloud.ModelSpectrumTypeGetIDForName("DEFAULT"));
            Util.l(Model.Cloud.ModelSpectrumTypeGetIDForName("CONVECTIVE"));
            Util.l(Model.Cloud.ModelSpectrumTypeGetIDForName("RASTER"));

        }

        public void MODELS_InsertOrUpdate()
        {
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

            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Model_ALADIN_SK", "KJHKDJ"));
            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Srážky_MAIN", "Model_ALADIN_CZ"));
            Model.Cloud.MODELSInsertOrUpdate(new CloudModels("Srážky_MAIN", "Model_ALADIN_CZ", "{countMethod = sum}"));
        }
        public void MODELS_GetNumberOfAreasForModel(){
            Util.l(Model.Cloud.MODELSGetNumberOfAreasForModel("Model_ALADIN_CZ"));
        }

        public void MODELS_GetIDFromName() {
            Util.l(Model.Cloud.MODELSGetIDFromName("Model_ALADIN_CZ"));
        }

        public void MODELS_GetSubmodelIDFromName()
        {
            Util.l(Model.Cloud.MODELSGetSubmodelIDFromName("Model_ALADIN_CZ", "Srážky_MAIN"));
        }

        public void MODELS_GetModelOptions() {
            string options = Model.Cloud.MODELSGetModelOptions("Model_ALADIN_CZ", "Srážky_MAIN");
            Util.l(options);

        }

        public void MODELS_GetModelStructure()
        {
            List<CloudModelEntity> modelStructure = Model.Cloud.MODELSGetModelStructure();
            foreach (var ms in modelStructure) {
                string dirPath = Util.pathSource["models"];
                string modelPath = ms.modelName + @"\" + ms.submodelName + @"\";
                //string modelPath = @"Model_ALADIN_CZ\Srážky_MAIN\";
                string filename = @"Barvy_stupnice.csv";
                string filePath = dirPath + modelPath + filename;
                Util.l($"{filePath} : {Model.Cloud.MODELSGetSubmodelIDFromName(ms.modelName, ms.submodelName)} ");

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        List<CloudModelSpectrum> listOfRecords = new List<CloudModelSpectrum>();
                        var header = reader.ReadLine(); //načte hlavičku
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');
                            CloudModelSpectrum record = new CloudModelSpectrum(Model.Cloud.MODELSGetSubmodelIDFromName(ms.modelName, ms.submodelName), values[0], values[1], values[2]);
                            listOfRecords.Add(record);
                        }

                        foreach (var r in listOfRecords)
                        {
                            Util.l($"{r.id_model}:{r.rank}:{r.color}:{r.type} ");
                            Model.Cloud.ModelSpectrumInsertOrUpdate(r);
                        }

                    }
                }
                catch (Exception e) {
                    Util.l(e);
                }

            }
        }

        public void ORP_COLOR_GetORPColors()
        {
            List<CloudORPColor> l = Model.Cloud.ORPColorGetORPColors();
            foreach (var ll in l)
            {
                Util.l($"ID_ORP: {ll.id_orp} Barva: {ll.color}");
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

        public void ORPS_GetIDFromName()
        {
            Util.l(Model.Cloud.ORPSGetIDFromName("Hlavní město Praha"));
        }

        public void REGIONS_GetNameFromColor() {
            Util.l(Model.Cloud.REGIONSGetNameFromColor("#ff0000"));
        }

        public void REGIONS_GetRegionCities()
        {
            List<CloudORPS> l = Model.Cloud.REGIONSGetRegionCities();
            foreach (var ll in l)
            {
                Util.l($"ID: {ll.id} Zástupce Okresu: {ll.name}");
            }
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
                    Util.l(values[0]);
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

        public void ReadCSVFileModelSpectrum(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                List<CloudModelSpectrum> listOfRecords = new List<CloudModelSpectrum>();
                var header = reader.ReadLine(); //načte hlavičku
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    CloudModelSpectrum record = new CloudModelSpectrum(values[0], values[1], values[2]);
                    listOfRecords.Add(record);
                }
                Util.l(header);
                foreach (var r in listOfRecords)
                {
                    Util.l($"{r.id_model} : {r.rank} : {r.color}: {r.type}");
                    Model.Cloud.ModelSpectrumInsertOrUpdate(r);
                }
            }
        }
    }
}
