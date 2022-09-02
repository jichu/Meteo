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
        public Dictionary<string, string> convectionTypeDescription { get; set; } = new Dictionary<string, string>(){
            { "-1", "V tomto čase se nevyskytují žádné konvektivní srážky."},
            { "Zvlněná studená fronta", "ZVLNĚNÁ STUDENÁ FRONTA: !!!VYSOKÉ nebezpečí na SILNÉ BOUŘKY a PŘÍVALOVÉ POVODNĚ!!!" },
            { "Zvlněná studená fronta - supercelární bouře", "ZVLNĚNÁ STUDENÁ FRONTA - SUPERCELÁRNÍ BOUŘE: !!!EXTRÉMNÍ nebezpečí na SILNÉ BOUŘKY, PŘÍVALOVÉ POVODNĚ a silnější TORNÁDA!!!"},
            { "Studená fronta", "STUDENÁ FRONTA: nehrozí ŽÁDNÉ nebezpečí"},
            { "Studená okluze", "STUDENÁ OKLUZE: nehrozí ŽÁDNÉ nebezpečí"},
            { "Teplá okluze", "TEPLÁ OKLUZE: !!!NÍZKÉ nebezpečí na SILNÉ BOUŘKY a PŘÍVALOVÉ POVODNĚ!!!"},
            { "Teplá okluze - supercelární bouře", "TEPLÁ OKLUZE - SUPERCELÁRNÍ BOUŘE: !!!VYSOKÉ nebezpečí na SILNÉ BOUŘKY, PŘÍVALOVÉ POVODNĚ a slabší TORNÁDA!!!"},
            { "Kvazifrontální konvekce", "KVAZIFRONTÁLNÍ KONVEKCE: nehrozí ŽÁDNÉ nebezpečí"},
            { "Orografická konvekce", "OROGRAFICKÁ KONVEKCE: !!!NÍZKÉ nebezpečí na SILNÉ BOUŘKY a PŘÍVALOVÉ POVODNĚ!!!"},
            { "Orografická konvekce - linie konvergence", "OROGRAFICKÁ KONVEKCE - LINIE KONVERGENCE (SQL): !!!EXTRÉMNÍ nebezpečí na SILNÉ BOUŘKY, PŘÍVALOVÉ POVODNĚ slabší TORNÁDA!!!"}
        };

        public string convectionSuperTypeMajor { get; set; }
        public List<string> convSuperTypesAll = new List<string>();
        public List<string> convSuperTypesKeys = new List<string>();
        public bool keyData { get; set; } = true;
        public CloudSamples()
        {

        }

        public void LoadORPS()
        {
            Util.l($"Načitání předpovědních parametrů pro jednotlivá ORP...");
            foreach (var orp in ORPS)
            {
                orp.LoadData(sample_name);
            }

            if (ORPS.Count(o => o.keyData == false) > 0) { this.keyData = false;
                //Util.l($"{sample_name}: chybí důležitá data!");
            }



        }

        public void CountMajorConvectionType(bool doComputing = true)
        {
            //Util.l($"Zjišťování nejčastějšího typu konvekce...");
            if (doComputing){
                int count = 0;
                string temporaryType = "";

                foreach (var item in convTypesKeys)
                {
                    if (convTypesAll.Count(i => i == item) > count)
                    {
                        count = convTypesAll.Count(i => i == item);
                        temporaryType = item;
                    }
                }

                convectionTypeMajor = convectionTypeDescription[temporaryType];

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
            else {
                if (Util.validData) {
                    convectionTypeMajor = "V tomto čase se nevyskytují žádné konvektivní srážky.";
                    convectionSuperTypeMajor = "V tomto čase se nevyskytují žádné konvektivní srážky.";
                } else {
                    if (Util.noData) {
                        convectionTypeMajor = "Nejsou k dispozici žádná data!";
                        convectionSuperTypeMajor = "Nejsou k dispozici žádná data!";
                    }
                    else
                    {
                        convectionTypeMajor = "Nejsou k dispozici aktuální data!";
                        convectionSuperTypeMajor = "Nejsou k dispozici aktuální data!";
                    }
                }
            }
        }
    }
}
