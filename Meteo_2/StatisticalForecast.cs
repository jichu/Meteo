using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class StatisticalForecast
    {
        public CloudSamples sample { get; set; }

        public int windDirection { get; set; }


        public StatisticalForecast(CloudSamples sample)
        {
            this.sample = sample;
            //Parametry z CloudSample
            windDirection = sample.windDirection;

            foreach (var orp in sample.ORPS) {
                //Parametry z CloudORP
                float wetBulb = orp.wetBulb;
                //Util.l($"Konvektivní srážky / dešťové srážky pro {this.sample.sample_name}: {wetBulb}");

                if ((windDirection == 4 || windDirection == 5) && (wetBulb >= 42f) || (windDirection < 4 || windDirection > 5) && (wetBulb >= 36f))
                {
                    //Util.l("Výpočet bude pokračovat");
                    //Vektor pohybu bouře
                    List<float> windSpeedValues = new List<float>() { orp.wind_850, orp.wind_700, orp.wind_600, orp.wind_500, orp.wind_400, orp.wind_300 };
                    orp.corfidiVector = 2 * Average(windSpeedValues) - windSpeedValues.First();
                    float corfidiVectorLevel = 0;
                    if (orp.corfidiVector <= 3) corfidiVectorLevel = 3;
                    else if (orp.corfidiVector <= 9) corfidiVectorLevel = 2;
                    else if (orp.corfidiVector <= 15) corfidiVectorLevel = 1;
                    else corfidiVectorLevel = 0;

                    ConvectionType(orp);
                    CombinePrecipitation(orp);


                }
                else
                {
                    //Util.l("Výpočet ukončen");
                }
            }

        }
        //Kombinovaná předpověď
        private void CombinePrecipitation(CloudORPS orp) {
            Util.l($"Zvlněná studená fronta: {orp.convectionTypes.Keys.Contains("Zvlněná studentá fronta")}");
        }

        //Předpověď typu konvekce
        private void ConvectionType(CloudORPS orp)
        {
            //Zvlněná studená fronta
            WavyColdFront(orp);
            //Zvlněná studená fronta - supercelární bouře
            WavyColdFrontS(orp);
            //Studená fronta
            ColdFront(orp);
            //Studená okluze
            ColdOcclusion(orp);
            //Teplá okluze
            WarmOcclusion(orp);
            //Teplá okluze - supercelární bouře
            WarmOcclusionS(orp);
            //Kvazifrontální konvekce
            QuasifontalConvection(orp);
            //Orografická konvekce
            OrographicConvection(orp);
            //Orografická konvekce - linie konvergence
            OrographicConvectionConvergenceLine(orp);

            var types = orp.convectionTypeResults.Where(p => p.Value.Equals(orp.convectionTypeResults.Values.Max()));

            foreach (var type in types)
            {
                //Util.l($" Nejvyšší hodnotu: {type.Value} má {type.Key}");
                orp.convectionTypes[type.Key] = type.Value;
            }

            /*foreach (KeyValuePair<string, float> kvp in orp.convectionTypes)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                    kvp.Key, kvp.Value);
            }*/

        }

        private void WavyColdFront(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["JZ"])
            {
                List<float> parameters = new List<float>() { 
                //orp.temperature_850, //<14.20>
                (orp.frontogenesis_850>=2f)?1:0,
                (orp.corfidiVector>=6f)?1:0,
                (orp.wetBulb>=51f)?1:0, 
                //orp.dls>=13?1:0
            };
                orp.wavyColdFront = SumArray(parameters)/parameters.Count;
                orp.convectionTypeResults["Zvlněná studentá fronta"] = orp.wavyColdFront;
            }
            else {
                orp.wavyColdFront = 0;
            }
        }
        private void WavyColdFrontS(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["J"] && windDirection <= Util.windDirectionToInt["JZ"])
            {
                List<float> parameters = new List<float>() { 
                //orp.temperature_850, //<16,20>
                (orp.frontogenesis_850>=2f)?1:0,
                //orp.cape, //MLCAPE<1000,3600>, MUCAPE <1750,5500>
                (orp.wetBulb>=63f)?1:0, 
                //orp.dls>=20?1:0,
                (orp.sreh_3km>=300f)?1:0,
                (orp.wind_300>=21f)?1:0
            };
                orp.wavyColdFrontS = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Zvlněná studentá fronta - supercelární bouře"] = orp.wavyColdFrontS;
            }
            else {
                orp.wavyColdFrontS = 0;
            }
        }
        private void ColdFront(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["Z"] || windDirection <= Util.windDirectionToInt["SV"])
            {
                List<float> parameters = new List<float>() { 
                //orp.temperature_850, //<10,13>
                (orp.frontogenesis_850>=1f)?1:0,
                (orp.corfidiVector>=4f)?1:0,
                (orp.wetBulb>=48f && orp.wetBulb<=60f)?1:0, 
                //orp.dls>=8?1:0
            };
                orp.coldFront = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Studentá fronta"] = orp.coldFront;
            }
            else {
                orp.coldFront = 0;
            }
        }
        private void ColdOcclusion(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["Z"] || (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["JV"]))
            {
                List<float> parameters = new List<float>() {
                (orp.frontogenesis_850>=-0.2f && orp.frontogenesis_850<=0.8f)?1:0,
                (orp.corfidiVector>=4f)?1:0,
                (orp.wetBulb>=33f && orp.wetBulb<=57f)?1:0,
                (orp.mixr>=7f)?1:0,
                //orp.dls>=6?1:0
            };
                orp.coldOcclusion = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Studentá okluze"] = orp.coldOcclusion;
            }
            else {
                orp.coldOcclusion = 0;
            }
        }
        private void WarmOcclusion(CloudORPS orp) {
            if (windDirection != Util.windDirectionToInt["S"] && windDirection != Util.windDirectionToInt["J"])
            {
                List<float> parameters = new List<float>() {
                (orp.frontogenesis_850>=-0.2f)?1:0,
                //orp.dls>=10?1:0,
                (orp.sreh_3km>=50f)?1:0,
                (orp.wind_300>=9f)?1:0,
            };
                orp.warmOcclusion = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Teplá okluze"] = orp.warmOcclusion;
            }
            else {
                orp.warmOcclusion = 0;
            }
        }
        private void WarmOcclusionS(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["JZ"])
            {
                List<float> parameters = new List<float>() { 
                //orp.temperature_850,// <14,17>
                (orp.frontogenesis_850>=0.4f)?1:0,
                //orp.cape // MLCAPE<100,3600> MUCAPE <75,5500>
                (orp.wetBulb>=51)?1:0, 
                //orp.dls>=15?1:0,
                (orp.sreh_3km >=100f)?1:0,
                (orp.wind_300>=24f)?1:0
            };
                orp.warmOcclusionS = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Teplá okluze - supercelární bouře"] = orp.warmOcclusionS;
            }
            else {
                orp.warmOcclusionS = 0;
            }
        }
        private void QuasifontalConvection(CloudORPS orp) {
            if (windDirection >= Util.windDirectionToInt["Z"] || windDirection == Util.windDirectionToInt["S"])
            {
                float corfidiVectorCondition = 0;
                float mixrCondition = 0;
                float wind_850Condition = 0;
                float pwaterCondition = 0;

                if (windDirection == Util.windDirectionToInt["Z"])
                {
                    corfidiVectorCondition = (orp.corfidiVector >= 6f && orp.corfidiVector <= 12f) ? 1 : 0;
                    mixrCondition = (orp.mixr >= 8f && orp.mixr <= 14f) ? 1 : 0;
                    wind_850Condition = (orp.wind_850 >= 1.5f && orp.wind_850 <= 17f) ? 1 : 0;
                    pwaterCondition = (orp.pwater >= 22f && orp.pwater <= 40f) ? 1 : 0;
                };
                if (windDirection == Util.windDirectionToInt["SZ"])
                {
                    corfidiVectorCondition = (orp.corfidiVector >= 4f && orp.corfidiVector <= 15f) ? 1 : 0;
                    mixrCondition = (orp.mixr >= 7f && orp.mixr <= 12f) ? 1 : 0;
                    wind_850Condition = (orp.wind_850 >= 1.5f && orp.wind_850 <= 12f) ? 1 : 0;
                    pwaterCondition = (orp.pwater >= 20f && orp.pwater <= 30f) ? 1 : 0;
                };
                if (windDirection == Util.windDirectionToInt["S"])
                {
                    corfidiVectorCondition = (orp.corfidiVector >= 2f && orp.corfidiVector <= 10f) ? 1 : 0;
                    mixrCondition = (orp.mixr >= 6f && orp.mixr <= 12f) ? 1 : 0;
                    wind_850Condition = (orp.wind_850 >= 1.5f && orp.wind_850 <= 12f) ? 1 : 0;
                    pwaterCondition = (orp.pwater >= 15f && orp.pwater <= 30f) ? 1 : 0;
                };

                List<float> parameters = new List<float>() {
                (orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=-1.5f)?1:0,
                (orp.potentional_orographic_lift>=0.05f)?1:0,
                corfidiVectorCondition,
                mixrCondition,
                wind_850Condition,
                pwaterCondition
                 };
                orp.quasifontalConvection = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Kvazifrontální konvekce"] = orp.quasifontalConvection;
            }
            else {
                orp.quasifontalConvection = 0;
            }
        }
        private void OrographicConvection(CloudORPS orp) {
            if (windDirection == Util.windDirectionToInt["SZ"] || (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["J"]))
            {
                List<float> parameters = new List<float>() {
                (orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=0.8f)?1:0,
                //orp.dls>=10?1:0,
                (orp.wind_850<=6f)?1:0,
                (orp.wind_300<=15)?1:0,
            };
                orp.orographicConvection = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Orografická konvekce"] = orp.orographicConvection;
            }
            else {
                orp.orographicConvection = 0;
            }
        }
        private void OrographicConvectionConvergenceLine(CloudORPS orp) {
            if (windDirection != Util.windDirectionToInt["S"] && windDirection != Util.windDirectionToInt["V"] && windDirection != Util.windDirectionToInt["Z"])
            {
                List<float> parameters = new List<float>() { 
                //orp.temperature_850, // <16,19>
                (orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=0.8f)?1:0,
                (orp.wetBulb>=63f)?1:0, 
                //orp.dls>=5f?1:0,
                (orp.wind_850<=3f)?1:0,
                (orp.pwater>=38f)?1:0
            };
                orp.orographicConvectionConvergenceLine = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Orografická konvekce - linie konvergence"] = orp.orographicConvectionConvergenceLine;
            }
            else {
                orp.orographicConvectionConvergenceLine = 0;
            }
        }

        private float Average(List<float> arr)
        {
            float avg = 0;
            avg = SumArray(arr) / arr.Count;
            return avg;
        }

        private float SumArray(List<float> arr, int start = 0, int end = -1)
        {
            float sum = 0;
            end = (end > arr.Count) ? arr.Count : end;
            start = (start > arr.Count) ? 0 : start;
            List<float> array = arr.GetRange(start, (end == -1) ? (arr.Count) - start : end - start);
            foreach (var a in array)
            {
                sum += a;
            }
            return sum;
        }
    }
}
