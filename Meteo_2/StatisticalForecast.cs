﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    class StatisticalForecast
    {
        private float valueNoComputing = -1f;
        private CloudOutput data;
        private int precision = 10;
        private const int RATIO = 3;
        private List<float> LevelScale = new List<float>() { 0.25f, 0.5f, 0.75f, 1.0f };

        public CloudSamples sample { get; set; }

        public int windDirection { get; set; }

        public StatisticalForecast(CloudSamples sample, bool doComputing = true)
        {
            this.sample = sample;
            //Parametry z CloudSample
            windDirection = sample.windDirection;

            foreach (var orp in this.sample.ORPS)
            {
                if (doComputing)
                {
                    //Parametry z CloudORP
                    float wetBulb = orp.wetBulb;
                    //Util.l($"Konvektivní srážky / dešťové srážky pro {this.sample.sample_name}: {wetBulb}");

                    if ((windDirection == 4 || windDirection == 5) && (wetBulb >= 42f) || (windDirection < 4 || windDirection > 5) && (wetBulb >= 36f))
                    {
                        //Util.l("Výpočet bude pokračovat");
                        //Vektor pohybu bouře
                        List<float> windSpeedValues = new List<float>() { orp.wind_850, orp.wind_700, orp.wind_600, orp.wind_500, orp.wind_400, orp.wind_300 };
                        orp.corfidiVector = 2 * Average(TestParameters(windSpeedValues)) - windSpeedValues.First();
                        if (orp.corfidiVector <= 3) orp.corfidiVectorLevel = 3;
                        else if (orp.corfidiVector <= 9) orp.corfidiVectorLevel = 2;
                        else if (orp.corfidiVector <= 15) orp.corfidiVectorLevel = 1;
                        else orp.corfidiVectorLevel = 0;

                        ConvectionType(orp);
                        CombinePrecipitation(orp);

                        //Výstupní parametry / vlastnosti
                        /*orp.output.Add("M_KOMBINOVANÁ PŘEDPOVĚĎ INTENZITY KONVEKTIVNÍCH SRÁŽEK", orp.combineIntensity != 0 ? orp.combineIntensity.ToString() : "0");
                        orp.output.Add("SMĚR PROUDĚNÍ", Util.windDirectionToString[sample.windDirection]);
                        orp.output.Add("ČAS VÝSKYTU", sample.sample_name);
                        orp.output.Add("RYCHLOST POHYBU BOUŘE", (orp.corfidiVectorLevel <= 2) ? "rychly pohyb" : "pomaly pohyb");
                        orp.output.Add("PODTYP KONVEKCE", orp.convectionTypesStringForm);
                        orp.output.Add("TYP KONVEKCE", orp.convectionSuperTypesStringForm);
                        orp.output.Add("SRÁŽKY KRAJ", orp.precipitationResultRegion.ToString());*/

                        orp.output.Add("A_TYP KONVEKCE", orp.convectionSuperTypesStringForm);
                        orp.output.Add("A_PODTYP KONVEKCE", orp.convectionTypesStringForm);

                        //Příprava na zjištění majoritních typů/supertypů konvekce
                        this.sample.convTypesAll.Add(orp.convectionTypesStringForm);
                        this.sample.convSuperTypesAll.Add(orp.convectionSuperTypesStringForm);

                        if (!this.sample.convTypesKeys.Contains(orp.convectionTypesStringForm))
                        {
                            this.sample.convTypesKeys.Add(orp.convectionTypesStringForm);
                        }

                        if (!this.sample.convSuperTypesKeys.Contains(orp.convectionSuperTypesStringForm))
                        {
                            this.sample.convSuperTypesKeys.Add(orp.convectionSuperTypesStringForm);
                        }


                        orp.output.Add("A_RYCHLOST POHYBU BOUŘE", (orp.corfidiVectorLevel <= 2) ? "rychly pohyb" : "pomaly pohyb");

                        //Vedlejší výstupy
                        orp.output.Add("S_SOUHRNNÁ NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.precipitationResult.ToString());
                        orp.output.Add("S_ALADIN ČHMÚ - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.aladin.ToString());
                        orp.output.Add("S_WRF ARW - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_arw.ToString());
                        orp.output.Add("S_WRF NMM - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_nmm.ToString());
                        orp.output.Add("S_KOMBINOVANÁ PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", orp.combineInfluence.ToString());


                        //TODO VÝPOČET STATISTICKÉ PŘEDPOVĚDI

                        //Util.windDirectionToString[sample.windDirection] //směr proudění
                        //Dráha bouří?// orp.output.Add("SOUHRNNÁ NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.precipitationResult.ToString());

                        //sample.sample_name //čas výskytu
                        //orp.convectionSuperTypesStringForm //typ konvekce
                        //(orp.corfidiVectorLevel <= 2) ? "rychly pohyb" : "pomaly pohyb")//rychlost pohybu bouře

                        orp.statisticalPrecipitation = -1;

                        if (orp.statisticalPrecipitation == -1)
                        {
                            orp.finalPlace = orp.combineInfluence;
                            orp.finalStorm = ValueToLevel(LevelScale, Probability(new List<float>() { orp.sum_merge, orp.significantPredictors })); //chybí summmerge?  
                        }
                        else
                        {
                            orp.finalPlace = ValueToLevel(LevelScale, Probability(new List<float>() { orp.statisticalPrecipitation, orp.combineInfluence }));
                            orp.finalStorm = ValueToLevel(LevelScale, Probability(new List<float>() { orp.statisticalPrecipitation, orp.sum_merge, orp.significantPredictors })); //chybí summmerge?  
                        }

                        //Hlavní výstupy
                        orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", orp.finalPlace.ToString());
                        orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU PŘÍVALOVÝCH SRÁŽEK", orp.finalStorm.ToString());

                        //Vedlejší výstupy
                        orp.output.Add("S_STATISTICKÝ ODHAD VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", orp.statisticalPrecipitation.ToString());

                        //orp.output.Add("M_VÝSLEDNÁ PŘEDPOVĚĎ INTENZITY KONVEKTIVNÍCH SRÁŽEK", orp.finalStorm.ToString());
                        this.data = new CloudOutput(orp.name, sample.sample_name, orp.output);
                        Util.outputDataCache.Add(this.data);
                    }
                    else
                    {
                        orp.output.Add("A_TYP KONVEKCE", valueNoComputing.ToString());
                        orp.output.Add("A_PODTYP KONVEKCE", valueNoComputing.ToString());
                        orp.output.Add("A_RYCHLOST POHYBU BOUŘE", valueNoComputing.ToString());
                        orp.output.Add("S_SOUHRNNÁ NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.precipitationResult.ToString());
                        orp.output.Add("S_ALADIN ČHMÚ - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.aladin.ToString());
                        orp.output.Add("S_WRF ARW - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_arw.ToString());
                        orp.output.Add("S_WRF NMM - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_nmm.ToString());
                        orp.output.Add("S_KOMBINOVANÁ PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                        orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                        orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU PŘÍVALOVÝCH SRÁŽEK", valueNoComputing.ToString());
                        orp.output.Add("S_STATISTICKÝ ODHAD VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                        this.data = new CloudOutput(orp.name, sample.sample_name, orp.output);
                        Util.outputDataCache.Add(this.data);
                        //Util.l("Výpočet ukončen");
                    }

                    //Výpis výsledků
                    //Util.l($"{orp.name}; Sloučená intenzita: {orp.combineIntensity}; Směr větru: {Util.windDirectionToString[sample.windDirection]}; Typů konvekce: {orp.convectionTypes.Count}; Čas: {sample.sample_name}; Srážky: {orp.precipitationResult}");
                    /*foreach (var type in orp.convectionTypes)
                    {
                        Util.l($"{type.Key}");
                    }*/

                    //Výstupní parametry / vlastnosti
                    /*orp.output.Add("M_KOMBINOVANÁ PŘEDPOVĚĎ INTENZITY KONVEKTIVNÍCH SRÁŽEK", orp.combineIntensity!=0?orp.combineIntensity.ToString():"0");
                    orp.output.Add("M_SMĚR PROUDĚNÍ", Util.windDirectionToString[sample.windDirection]);
                    orp.output.Add("M_ČAS VÝSKYTU", sample.sample_name);
                    orp.output.Add("M_RYCHLOST POHYBU BOUŘE", (orp.corfidiVectorLevel <= 2) ? "rychly pohyb" : "pomaly pohyb");
                    orp.output.Add("M_PODTYP KONVEKCE", orp.convectionTypesStringForm);
                    orp.output.Add("M_TYP KONVEKCE", orp.convectionSuperTypesStringForm);
                    orp.output.Add("M_SRÁŽKY ORP", orp.precipitationResult.ToString());                
                    orp.output.Add("M_SRÁŽKY KRAJ", orp.precipitationResultRegion.ToString());*/


                    //Util.l("Generování výstupních dat..");

                }
                else
                {
                    orp.output.Add("A_TYP KONVEKCE", valueNoComputing.ToString());
                    orp.output.Add("A_PODTYP KONVEKCE", valueNoComputing.ToString());
                    orp.output.Add("A_RYCHLOST POHYBU BOUŘE", valueNoComputing.ToString());
                    orp.output.Add("S_SOUHRNNÁ NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.precipitationResult.ToString());
                    orp.output.Add("S_ALADIN ČHMÚ - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.aladin.ToString());
                    orp.output.Add("S_WRF ARW - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_arw.ToString());
                    orp.output.Add("S_WRF NMM - NUMERICKÁ PŘEDPOVĚĎ KONVEKTIVNÍCH SRÁŽEK", orp.wrf_nmm.ToString());
                    orp.output.Add("S_KOMBINOVANÁ PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                    orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                    orp.output.Add("M_PŘEDPOVĚĎ VÝSKYTU PŘÍVALOVÝCH SRÁŽEK", valueNoComputing.ToString());
                    orp.output.Add("S_STATISTICKÝ ODHAD VÝSKYTU KONVEKTIVNÍCH SRÁŽEK", valueNoComputing.ToString());
                    this.data = new CloudOutput(orp.name, sample.sample_name, orp.output);
                    Util.outputDataCache.Add(this.data);
                    //Util.l("Generování výstupních dat..");
                }
            }

            this.sample.CountMajorConvectionType(doComputing);

        }
        //Kombinovaná předpověď
        private void CombinePrecipitation(CloudORPS orp)
        {
            //Util.l($"Výpočet kombinované předpovědi...");
            //Util.l($"Zvlněná studená fronta: {orp.convectionTypes.Keys.Contains("Zvlněná studentá fronta")}");
            if (orp.convectionTypes.Keys.Contains("Zvlněná studentá fronta"))
            {
                precipitationProbability(orp, 14, 20);
                precipitationProbability(orp, -300, 13, true); //Pouští se jen pro tento případ
            }
            if (orp.convectionTypes.Keys.Contains("Zvlněná studentá fronta - supercelární bouře"))
            {
                precipitationProbability(orp, 16, 20);
            }
            if (orp.convectionTypes.Keys.Contains("Studentá fronta"))
            {
                precipitationProbability(orp, 10, 13);
            }
            if (orp.convectionTypes.Keys.Contains("Studentá okluze"))
            {
                precipitationProbability(orp, 5, 13);
            }
            if (orp.convectionTypes.Keys.Contains("Teplá okluze"))
            {
                precipitationProbability(orp, 14, 18);
            }
            if (orp.convectionTypes.Keys.Contains("Teplá okluze - supercelární bouře"))
            {
                precipitationProbability(orp, 14, 17);
            }
            if (orp.convectionTypes.Keys.Contains("Kvazifrontální konvekce"))
            {
                precipitationProbability(orp, 7, 17);
            }
            if (orp.convectionTypes.Keys.Contains("Orografická konvekce"))
            {
                precipitationProbability(orp, 10, 19);
            }
            if (orp.convectionTypes.Keys.Contains("Orografická konvekce - linie konvergence"))
            {
                precipitationProbability(orp, 16, 19);
            }

        }

        private void precipitationProbability(CloudORPS orp, float minT, float maxT, bool cold = false)
        {
            if (orp.temperature_850 >= minT && orp.temperature_850 <= maxT)
            {
                orp.warmWetSectorPlace = ValueToLevel(LevelScale, Probability(new List<float>() { orp.temperature_850, orp.frontogenesis_850, orp.pressureMLSP, orp.mfdiv, orp.relativeVorticity, orp.rh_700 }));
                orp.coldSectorPlace = (cold) ? ValueToLevel(LevelScale, Probability(new List<float>() { orp.pressureMLSP, orp.mfdiv, orp.relativeVorticity, orp.rh_700 })) : -1;
                orp.combineSectorPlace = ValueToLevel(LevelScale, Probability(new List<float>() { orp.warmWetSectorPlace, orp.coldSectorPlace }));

                //Předpověď lokálních podmínek
                //Intervaly 6,9,30,33
                if (sample.sample_name == "06" || sample.sample_name == "09" || sample.sample_name == "30" || sample.sample_name == "33")
                {
                    orp.temperatureInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.sklonitost_reliefu, orp.orientace_reliefu_tepelny_prohrev_dopoledne, orp.vegetace_pokryti, orp.ir_kontrast, orp.cloudy }));
                }
                //Intervaly 12,15,18,36,39,42
                else if (sample.sample_name == "12" || sample.sample_name == "15" || sample.sample_name == "18" || sample.sample_name == "36" || sample.sample_name == "39" || sample.sample_name == "42")
                {
                    orp.temperatureInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.sklonitost_reliefu, orp.orientace_reliefu_tepelny_prohrev_odpoledne, orp.vegetace_pokryti, orp.ir_kontrast, orp.cloudy }));
                }
                //NOC
                else
                {
                    orp.temperatureInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.sklonitost_reliefu, orp.vegetace_pokryti, orp.ir_kontrast }));

                }
                orp.windInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.sidelni_utvar, orp.sirka_udoli, orp.obtekani_prekazky, orp.wind_1000 }));
                orp.humidityInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.rh_1000, orp.mfdiv }));
                orp.orographicInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.polohy_nadmorskych_vysek, orp.sirka_hrebene, orp.mfdiv, orp.potentional_orographic_lift, orp.wind_850 }));
                orp.combineInfluence = ValueToLevel(LevelScale, Probability(new List<float>() { orp.combineSectorPlace, orp.temperatureInfluence, orp.windInfluence, orp.humidityInfluence, orp.orographicInfluence }));

                //Kombinovaná předpověď intenzity konvektivních srážek
                orp.significantPredictors = ValueToLevel(LevelScale, Probability(new List<float>() { orp.combineInfluence, orp.temperature_850, orp.corfidiVector, orp.wetBulb, orp.dls }));
                orp.otherPredictors = ValueToLevel(LevelScale, Probability(new List<float>() { orp.combineInfluence, orp.frontogenesis_850, orp.mlcape, orp.mlcin, orp.mixr, orp.sreh_3km, orp.wind_850, orp.wind_300, orp.pwater })); //mlcape vs mucape
                orp.combineIntensity = ValueToLevel(LevelScale, Probability(new List<float>() { orp.combineInfluence, orp.significantPredictors, orp.otherPredictors }));
            }
        }

        //Předpověď typu konvekce
        private void ConvectionType(CloudORPS orp)
        {
            //Util.l($"Zjišťování typu konvekce pro jednotlivé ORP...");
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
            QuasifontalConvection(orp); //KVAZIFRONÁTLNÍ
            //Orografická konvekce
            OrographicConvection(orp); //OROGRAFICKA
            //Orografická konvekce - linie konvergence
            OrographicConvectionConvergenceLine(orp); //OROGRAFICKA

            var types = orp.convectionTypeResults.Where(p => p.Value.Equals(orp.convectionTypeResults.Values.Max()));

            foreach (var type in types)
            {
                //Util.l($" Nejvyšší hodnotu: {type.Value} má {type.Key}");
                orp.convectionTypes[type.Key] = type.Value;
            }

            orp.ProcessConvectionTypes();

            /*
            foreach (KeyValuePair<string, float> kvp in orp.convectionTypes)
            {
                Console.WriteLine("ORP= {0}, Key = {1}, Value = {2}",
                    orp.name, kvp.Key, kvp.Value);
            }*/

        }

        private void WavyColdFront(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["JZ"])
            {
                List<float> param = new List<float>() {
                (orp.temperature_850!=-1f)?((orp.temperature_850>=14 && orp.temperature_850<=20)?1:0):-1f,
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=2f)?1:0):-1f,
                (orp.corfidiVector!=-1f)?((orp.corfidiVector>=6f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=51f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=13?1:0):-1f
            };
                List<float> parameters = TestParameters(param);
                orp.wavyColdFront = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Zvlněná studená fronta"] = orp.wavyColdFront;
            }
            else
            {
                orp.wavyColdFront = 0;
            }
        }
        private void WavyColdFrontS(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["J"] && windDirection <= Util.windDirectionToInt["JZ"])
            {
                List<float> param = new List<float>() {
                (orp.temperature_850!=-1f)?((orp.temperature_850>=16 && orp.temperature_850<=20)?1:0):-1f,
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=2f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=63f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=20?1:0):-1f,
                (orp.sreh_3km!=-1f)?((orp.sreh_3km>=300f)?1:0):-1f,
                (orp.wind_300!=-1f)?((orp.wind_300>=21f)?1:0):-1f
            };
                if (IsDay()) { param.Add((orp.mlcape != -1f) ? ((orp.mlcape >= 1000 && orp.mlcape <= 3600) ? 1 : 0) : -1f); }
                else { param.Add((orp.mucape != -1f) ? ((orp.mucape >= 1750 && orp.mucape <= 5500) ? 1 : 0) : -1f); }
                List<float> parameters = TestParameters(param);
                orp.wavyColdFrontS = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Zvlněná studená fronta - supercelární bouře"] = orp.wavyColdFrontS;
            }
            else
            {
                orp.wavyColdFrontS = 0;
            }
        }
        private void ColdFront(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["Z"] || windDirection <= Util.windDirectionToInt["SV"])
            {
                List<float> param = new List<float>() {
                (orp.temperature_850!=-1f)?((orp.temperature_850>=10 && orp.temperature_850<=13)?1:0):-1f,
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=1f)?1:0):-1f,
                (orp.corfidiVector!=-1f)?((orp.corfidiVector>=4f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=48f && orp.wetBulb<=60f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=8?1:0):-1f
            };
                List<float> parameters = TestParameters(param);
                orp.coldFront = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Studená fronta"] = orp.coldFront;
            }
            else
            {
                orp.coldFront = 0;
            }
        }
        private void ColdOcclusion(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["Z"] || (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["JV"]))
            {
                List<float> param = new List<float>() {
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=-0.2f && orp.frontogenesis_850<=0.8f)?1:0):-1f,
                (orp.corfidiVector!=-1f)?((orp.corfidiVector>=4f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=33f && orp.wetBulb<=57f)?1:0):-1f,
                (orp.mixr!=-1f)?((orp.mixr>=7f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=6?1:0):-1f
            };
                List<float> parameters = TestParameters(param);
                orp.coldOcclusion = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Studená okluze"] = orp.coldOcclusion;
            }
            else
            {
                orp.coldOcclusion = 0;
            }
        }
        private void WarmOcclusion(CloudORPS orp)
        {
            if (windDirection != Util.windDirectionToInt["S"] && windDirection != Util.windDirectionToInt["J"])
            {
                List<float> param = new List<float>() {
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=-0.2f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=10?1:0):-1f,
                (orp.sreh_3km!=-1f)?((orp.sreh_3km>=50f)?1:0):-1f,
                (orp.wind_300!=-1f)?((orp.wind_300>=9f)?1:0):-1f,
            };
                List<float> parameters = TestParameters(param);
                orp.warmOcclusion = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Teplá okluze"] = orp.warmOcclusion;
            }
            else
            {
                orp.warmOcclusion = 0;
            }
        }
        private void WarmOcclusionS(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["JZ"])
            {
                List<float> param = new List<float>() {
                (orp.temperature_850!=-1f)?((orp.temperature_850>=14 && orp.temperature_850<=17)?1:0):-1f,
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=0.4f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=51)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=15?1:0):-1f,
                (orp.sreh_3km!=-1f)?((orp.sreh_3km >=100f)?1:0):-1f,
                (orp.wind_300!=-1f)?((orp.wind_300>=24f)?1:0):-1f
            };
                if (IsDay()) { param.Add((orp.mlcape != -1f) ? ((orp.mlcape >= 100 && orp.mlcape <= 3600) ? 1 : 0) : -1f); }
                else { param.Add((orp.mucape != -1f) ? ((orp.mucape >= 75 && orp.mucape <= 5500) ? 1 : 0) : -1f); }
                List<float> parameters = TestParameters(param);
                orp.warmOcclusionS = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Teplá okluze - supercelární bouře"] = orp.warmOcclusionS;
            }
            else
            {
                orp.warmOcclusionS = 0;
            }
        }
        private void QuasifontalConvection(CloudORPS orp)
        {
            if (windDirection >= Util.windDirectionToInt["Z"] || windDirection == Util.windDirectionToInt["S"])
            {
                float corfidiVectorCondition = 0;
                float mixrCondition = 0;
                float wind_850Condition = 0;
                float pwaterCondition = 0;

                if (windDirection == Util.windDirectionToInt["Z"])
                {
                    corfidiVectorCondition = (orp.corfidiVector != -1f) ? ((orp.corfidiVector >= 6f && orp.corfidiVector <= 12f) ? 1 : 0) : -1f;
                    mixrCondition = (orp.mixr != -1f) ? ((orp.mixr >= 8f && orp.mixr <= 14f) ? 1 : 0) : -1f;
                    wind_850Condition = (orp.wind_850 != -1f) ? ((orp.wind_850 >= 1.5f && orp.wind_850 <= 17f) ? 1 : 0) : -1f;
                    pwaterCondition = (orp.pwater != -1f) ? ((orp.pwater >= 22f && orp.pwater <= 40f) ? 1 : 0) : -1f;
                };
                if (windDirection == Util.windDirectionToInt["SZ"])
                {
                    corfidiVectorCondition = (orp.corfidiVector != -1f) ? ((orp.corfidiVector >= 4f && orp.corfidiVector <= 15f) ? 1 : 0) : -1f;
                    mixrCondition = (orp.mixr != -1f) ? ((orp.mixr >= 7f && orp.mixr <= 12f) ? 1 : 0) : -1f;
                    wind_850Condition = (orp.wind_850 != -1f) ? ((orp.wind_850 >= 1.5f && orp.wind_850 <= 12f) ? 1 : 0) : -1f;
                    pwaterCondition = (orp.pwater != -1f) ? ((orp.pwater >= 20f && orp.pwater <= 30f) ? 1 : 0) : -1f;
                };
                if (windDirection == Util.windDirectionToInt["S"])
                {
                    corfidiVectorCondition = (orp.corfidiVector != -1f) ? ((orp.corfidiVector >= 2f && orp.corfidiVector <= 10f) ? 1 : 0) : -1f;
                    mixrCondition = (orp.mixr != -1f) ? ((orp.mixr >= 6f && orp.mixr <= 12f) ? 1 : 0) : -1f;
                    wind_850Condition = (orp.wind_850 != -1f) ? ((orp.wind_850 >= 1.5f && orp.wind_850 <= 12f) ? 1 : 0) : -1f;
                    pwaterCondition = (orp.pwater != -1f) ? ((orp.pwater >= 15f && orp.pwater <= 30f) ? 1 : 0) : -1f;
                };

                List<float> param = new List<float>() {
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=-1.5f)?1:0):-1f,
                (orp.potentional_orographic_lift!=-1f)?((orp.potentional_orographic_lift>=0.05f)?1:0):-1f,
                corfidiVectorCondition,
                mixrCondition,
                wind_850Condition,
                pwaterCondition
                 };
                List<float> parameters = TestParameters(param);
                orp.quasifontalConvection = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Kvazifrontální konvekce"] = orp.quasifontalConvection;
            }
            else
            {
                orp.quasifontalConvection = 0;
            }
        }
        private void OrographicConvection(CloudORPS orp)
        {
            if (windDirection == Util.windDirectionToInt["SZ"] || (windDirection >= Util.windDirectionToInt["SV"] && windDirection <= Util.windDirectionToInt["J"]))
            {
                List<float> param = new List<float>() {
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=0.8f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=10?1:0):-1f,
                (orp.wind_850!=-1f)?((orp.wind_850<=6f)?1:0):-1f,
                (orp.wind_300!=-1f)?((orp.wind_300<=15)?1:0):-1f,
            };
                List<float> parameters = TestParameters(param);
                orp.orographicConvection = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Orografická konvekce"] = orp.orographicConvection;
            }
            else
            {
                orp.orographicConvection = 0;
            }
        }
        private void OrographicConvectionConvergenceLine(CloudORPS orp)
        {
            if (windDirection != Util.windDirectionToInt["S"] && windDirection != Util.windDirectionToInt["V"] && windDirection != Util.windDirectionToInt["Z"])
            {
                List<float> param = new List<float>() {
                (orp.temperature_850!=-1f)?((orp.temperature_850>=16 && orp.temperature_850<=19)?1:0):-1f,
                (orp.frontogenesis_850!=-1f)?((orp.frontogenesis_850>=-25f && orp.frontogenesis_850<=0.8f)?1:0):-1f,
                (orp.wetBulb!=-1f)?((orp.wetBulb>=63f)?1:0):-1f,
                (orp.dls!=-1f)?(orp.dls>=5f?1:0):-1f,
                (orp.wind_850!=-1f)?((orp.wind_850<=3f)?1:0):-1f,
                (orp.pwater!=-1f)?((orp.pwater>=38f)?1:0):-1f
            };
                List<float> parameters = TestParameters(param);
                orp.orographicConvectionConvergenceLine = SumArray(parameters) / parameters.Count;
                orp.convectionTypeResults["Orografická konvekce - linie konvergence"] = orp.orographicConvectionConvergenceLine;
            }
            else
            {
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
        //Zjištění denní doby podle názvu snímku
        private bool IsDay()
        {
            if (this.sample.sample_name == "21" || this.sample.sample_name == "00" || this.sample.sample_name == "03" || this.sample.sample_name == "06" || this.sample.sample_name == "24" ||
                this.sample.sample_name == "27" || this.sample.sample_name == "30" || this.sample.sample_name == "45" || this.sample.sample_name == "48")
                return false;
            else
                return true;
        }

        //Převod hodnoty pravděpodobnosti na úroveň dle tabulky
        private int ValueToLevel(List<float> levels, float value)
        {
            int level = -1;
            foreach (var l in levels)
            {

                level = (value <= l) ? levels.IndexOf(l) : -1;
                if (level != -1) break;
            }
            return level;
        }
        //Výpočet pravděpodobnosti
        private float Probability(List<float> list)
        {
            List<float> values = TestParameters(list);
            if (values.Count != 0) return (float)Math.Round((double)(new decimal(SumArray(values) / (values.Count * RATIO))), precision);
            else { return -1; }
        }

        private List<float> TestParameters(List<float> list)
        {
            List<float> values = new List<float>();
            foreach (var l in list)
            {
                if (l != -1) values.Add(l);
            }
            return values;
        }
    }
}
