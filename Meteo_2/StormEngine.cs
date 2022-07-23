using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meteo.JSONparser;
using Newtonsoft.Json.Linq;

namespace Meteo
{
    enum algorithms { dangerous_phenomen, statistic_forecast };
    class StormEngine
    {
        
        List<CloudORPS> ORPS = Model.Cloud.ORPSGetORPNames();
        List<ForecastAreaParameters> fapList = new List<ForecastAreaParameters>();
        List<CloudSamples> listSamples = Model.Cloud.InputDataGetSamples();
        public string sampleName { get; set; }
        public List<string> sampleNames = new List<string>();
        public List<string> finalSampleNames = new List<string>();
        public algorithms algorithm { get; set; }
        public PrecipitationFilter precipitationFilter { get; set; }

        public StormEngine(algorithms method = algorithms.dangerous_phenomen)
        {
            if (method == algorithms.dangerous_phenomen)
            {
                Util.l("Algoritmus výpočtu nebezpečných jevů");
                foreach (var l in listSamples)
                {
                    sampleNames.Add(l.sample_name);
                }
                /*Thread t = new Thread(() => Run());
                t.Start();*/
                Run();
            }
            else if (method == algorithms.statistic_forecast)
            {
                Util.l("Algoritmus statistické předpovědi konvenktivních srážek");
                if (listSamples.Count() == 0)
                {
                    listSamples.Add(new CloudSamples());
                    listSamples[0].sample_name = Util.firstSample;
                }

                foreach (var l in listSamples)
                {
                    sampleNames.Add(l.sample_name);
                }

                if (Util.validData)
                {
                    precipitationFilter = new PrecipitationFilter(ORPS, listSamples);

                    foreach (var s in precipitationFilter.finalSampleList)
                    {
                        finalSampleNames.Add(s.sample_name);
                    }

                }
                Util.l("Dále bude výpočet pokračovat pro tyto intervaly: ");

                foreach (var s in listSamples) {
                    s.LoadORPS();
                    Util.l($"{s.sample_name}");

                    if (finalSampleNames.Contains(s.sample_name))
                    {
                        new StatisticalForecast(s);
                    }
                    else {
                        new StatisticalForecast(s, false); //Neprovádí se žádný výpočet, pouze výpis prázdných výsledků
                    }
                }

                //sampleNames = finalSampleNames;
                Run();

            }
            else {
                Util.l("Neplatný algoritmus");
            }
            
        }

        public void Run() {
            //List<Thread> threadList = new List<Thread>();
            //List<Task> taskList = new List<Task>();
            //Util.StartWatch();
            //Util.l($"Počet záznamů v cache: {Util.outputDataCache.Count()}");
            foreach (var s in sampleNames)
            {
                
                //taskList.Add(Task.Run(() => Algorithm(s))); //zapnutí/vypnutí původního algoritmu
                /*Thread t = new Thread(() => Algorithm(s));
                t.Start();*/
                //threadList.Add(t);
                //Algorithm(s);
            }
            
            //Task.WaitAll(taskList.ToArray());

            List<string> outputList = new List<string>(); //Hlavní výstupy
            List<string> secondaryOutputList = new List<string>(); //Vedlejší výstupy
            List<string> advancedOutputList = new List<string>(); //Pokročilé výstupy

            string[] splittedOutputName = { };
            foreach (var item in Util.outputDataCache) {
                foreach(var output in item.output){
                    if (output.Key.StartsWith("M_"))
                    {
                        //Zpracování pro hlavní výstupy
                        splittedOutputName = output.Key.Split('_');
                        if (!outputList.Contains(splittedOutputName[1])) // Název výstupu bez M_
                        {
                            //if (splittedOutputName.Count() <=1) Util.l($"{output.Key.StartsWith("M_")}:{output.Key}:{splittedOutputName.Count()}");
                            outputList.Add(splittedOutputName[1]);
                        }
                    }
                    //Zpracování pro vedlejší výstupy
                    else if (output.Key.StartsWith("S_"))
                    {
                        splittedOutputName = output.Key.Split('_');
                        if (!secondaryOutputList.Contains(splittedOutputName[1]))
                            secondaryOutputList.Add(splittedOutputName[1]);
                    }

                    //Zpracování pro pokročilé výstupy
                    else if (output.Key.StartsWith("A_"))
                    {
                        splittedOutputName = output.Key.Split('_');
                        if (!advancedOutputList.Contains(splittedOutputName[1]))
                            advancedOutputList.Add(splittedOutputName[1]);
                    }
                    else { 
                    
                    }

                }
            }
            List<string> orpList = new List<string>();
            foreach (var orp in Util.outputDataCache)
            {
                if (!orpList.Contains(orp.nameOrp)) orpList.Add(orp.nameOrp);
            }

            string[,,] mainData = new string [sampleNames.Count,outputList.Count,orpList.Count]; //Hodnoty pro hlavní výstupy
            string[,,] secondaryData = new string [sampleNames.Count,secondaryOutputList.Count,orpList.Count]; //Hodnoty pro vedlejší výstupy
            string[,,] advancedData = new string [sampleNames.Count, advancedOutputList.Count,orpList.Count]; //Hodnoty pro pokročilé výstupy

            List<CloudSettings> settings = Model.Cloud.SETTINGSGetSettings();

            List<CloudOutput> filter = new List<CloudOutput>();

            for(int i = 0; i<sampleNames.Count; i++){
                filter.Clear();
                foreach (var item in Util.outputDataCache)
                {
                    if(item.sampleName == sampleNames[i])
                    {
                        filter.Add(item);
                    }
                }
                //Uložení hlavních výstupů
                for(int j = 0; j<outputList.Count; j++)
                {
                    for (int k = 0; k< orpList.Count; k++)
                    {
                        foreach (var item in filter)
                        {
                            if (item.nameOrp == orpList[k])
                                if (item.output.ContainsKey("M_" + outputList[j])) { mainData[i, j, k] = (item.output["M_" + outputList[j]]); }
                                else mainData[i, j, k] = ("-1");
                        }
                    }
                }


                
                //Uložení vedlejších výstupů
                for (int j = 0; j < secondaryOutputList.Count; j++)
                {
                    for (int k = 0; k < orpList.Count; k++)
                    {
                        foreach (var item in filter)
                        {
                            if (item.nameOrp == orpList[k])
                                if (item.output.ContainsKey("S_" + secondaryOutputList[j])) secondaryData[i, j, k] = (item.output["S_" + secondaryOutputList[j]]);
                                else secondaryData[i, j, k] = ("-1");
                        }
                    }
                }

                //Uložení pokročilých výstupů
                for (int j = 0; j < advancedOutputList.Count; j++)
                {
                    for (int k = 0; k < orpList.Count; k++)
                    {
                        foreach (var item in filter)
                        {
                            if (item.nameOrp == orpList[k])
                                if (item.output.ContainsKey("A_" + advancedOutputList[j])) advancedData[i, j, k] = (item.output["A_" + advancedOutputList[j]]);
                                else advancedData[i, j, k] = ("-1");
                        }
                    }
                }
            }

            string[] majorConvectionTypesData = new string[sampleNames.Count];
            string[] majorConvectionSuperTypesData = new string[sampleNames.Count];

            for (int i = 0; i < listSamples.Count; i++)
            { 
                majorConvectionTypesData[i] = listSamples[i].convectionTypeMajor;
                majorConvectionSuperTypesData[i] = listSamples[i].convectionSuperTypeMajor;
            }

            //Vytváření souboru root
            //Hlavní výstupy
            /*JSONwriter.CreateJsonRoot(
              new JObject
              (
                   new JProperty("orplist", orpList),
                   new JProperty("outputlist", outputList),
                   new JProperty("outputresultcolor", new JArray() {
                       GetValueFromSettingsList(settings, "output_result-1_color"),
                       GetValueFromSettingsList(settings, "output_result0_color"),
                       GetValueFromSettingsList(settings, "output_result1_color"),
                       GetValueFromSettingsList(settings, "output_result2_color"),
                       GetValueFromSettingsList(settings, "output_result3_color") })
               ),
              "root_main"
            );
            //Vedlejší výstupy
            JSONwriter.CreateJsonRoot(
              new JObject
              (
                   new JProperty("orplist", orpList),
                   new JProperty("outputlist", secondaryOutputList),
                   new JProperty("outputresultcolor", new JArray() {
                       GetValueFromSettingsList(settings, "output_result-1_color"),
                       GetValueFromSettingsList(settings, "output_result0_color"),
                       GetValueFromSettingsList(settings, "output_result1_color"),
                       GetValueFromSettingsList(settings, "output_result2_color"),
                       GetValueFromSettingsList(settings, "output_result3_color") })
               ),
              "root_secondary"
            );*/

            //Všechno dohromady
            JSONwriter.CreateJsonRoot(
              new JObject
              (
                   new JProperty("orplist", orpList),
                   new JProperty("mainoutputlist", outputList),
                   new JProperty("secondaryoutputlist", secondaryOutputList),
                   new JProperty("advancedoutputlist", advancedOutputList),
                   new JProperty("outputresultcolor", new JArray() {
                       GetValueFromSettingsList(settings, "output_result-1_color"),
                       GetValueFromSettingsList(settings, "output_result0_color"),
                       GetValueFromSettingsList(settings, "output_result1_color"),
                       GetValueFromSettingsList(settings, "output_result2_color"),
                       GetValueFromSettingsList(settings, "output_result3_color") })
               ),
              "root"
            );
            var arrayMain = JArray.FromObject(mainData);
            var arraySec = JArray.FromObject(secondaryData);
            var arrayAdv = JArray.FromObject(advancedData);

            JSONwriter.CreateJson(
              new JObject
              (
                   new JProperty("validdata", Util.validData),
                   new JProperty("date", GetValueFromSettingsList(settings, "last_date")),
                   new JProperty("samplename", sampleNames),
                   new JProperty("majorconvectiontype", majorConvectionTypesData),
                   new JProperty("majorconvectionsupertype", majorConvectionSuperTypesData),
                   new JProperty("maindata", arrayMain),
                   new JProperty("secondarydata", arraySec),
                   new JProperty("advanceddata", arrayAdv)
               ),
              "_" +sampleNames[0] + "h_"+ GetValueFromSettingsList(settings, "last_date")
            );

            //Util.l($"Počet záznamů v cache: {Util.outputDataCache.Count()}");

            //Util.StopWatch("Vypočet dokončen!");
            //Output();

        }

        private string GetValueFromSettingsList(List<CloudSettings> settings, string option_name) {
            string option_value = "";
            foreach (var item in settings) {
                if (item.option_name == option_name) option_value = item.option_value;
            }
            return option_value;

        }

        public void Algorithm(string s) {
            DateTime start = DateTime.Now;
            Util.l($"Počítám předpověď pro {s}. hodinu.");
            foreach (var ORP in ORPS)
            {
                /*ForecastAreaParameters fap = new ForecastAreaParameters(ORP, s);
                fapList.Add(fap);*/
                new ForecastAreaParameters(ORP, s);
            }
            Util.l($"Výpočet pro {s}. hodinu dokončen za {DateTime.Now - start}");
        }


        //Odsud se to nepouští!
        public StormEngine(string sampleName)
        {
            this.sampleName = sampleName;

            foreach (var ORP in ORPS)
            {
                ForecastAreaParameters fap = new ForecastAreaParameters(ORP, this.sampleName);
                fapList.Add(fap);
            }

            //Output();
        }
        //Zde se bude počítat předpověď na základě parametrů
        public void Output() {
            foreach (var f in fapList)
            {
                CloudOutputData mainOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"], Util.algorithmOutput["PŘEDPOVĚĎ RIZIKA PŘÍVALOVÝCH POVODNÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(mainOutput);
                CloudOutputData stormIntensityOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚĎ INTENZITY BOUŘÍ"], Util.algorithmOutput["PŘEDPOVĚĎ INTENZITY BOUŘÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(stormIntensityOutput);
                CloudOutputData precipitationPlaceOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (NWP MODELY)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceOutput);
                CloudOutputData precipitationPlaceKoefOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"], Util.algorithmOutput["PRAVDĚPODOBNOST MÍSTA VÝSKYTU SRÁŽEK (ALGORITMUS)"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(precipitationPlaceKoefOutput);
                CloudOutputData torrentialRainOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA PŘÍVALOVÉHO DEŠTĚ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(torrentialRainOutput);
                CloudOutputData strongWindscreensOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - VLHKÝ DOWNBURST"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutput);
                CloudOutputData hailOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚD RIZIKA KRUPOBITÍ"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA KRUPOBITÍ"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(hailOutput);
                CloudOutputData supercelarTornadosOutput = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA VÝSKYTU TORNÁD"]);
                Model.Cloud.OUTPUTDATAInsertOrUpdate(supercelarTornadosOutput);
                if (f.Output.ContainsKey("M_PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"))
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(f.id_orp, f.sampleName, f.Output["M_PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"], Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }
                else
                {
                    CloudOutputData strongWindscreensOutputSD = new CloudOutputData(f.id_orp, f.sampleName, -1, Util.algorithmOutput["PŘEDPOVĚD RIZIKA SILNÝCH NÁRAZŮ VĚTRU - SUCHÝ DOWNBURST"]);
                    Model.Cloud.OUTPUTDATAInsertOrUpdate(strongWindscreensOutputSD);
                }
            }

        }


    }
}
