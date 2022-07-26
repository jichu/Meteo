using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meteo
{
    public class LoadData
    {
        private List<string> LogErrors = new List<string>();

        public LoadData()
        {
            /*FormSetModelsDir dlg = new FormSetModelsDir("Chcete přepsat data vybranou adresářovou strukturou, maskami a stupnicemi?");
            dlg.ShowDialog();*/

            /*Util.curModelDir = "models__09h__24.06.2021";
            Util.firstSample = "09";*/
            /*Util.curModelDir = "models__03h__05.06.2019";
            Util.firstSample = "03";
            Util.curModelDir = "models__09h__23.07.2022";
            Util.firstSample = "09";*/

            Util.curModelDir = generateCurrentModelDir();
            Model.Cloud.SETTINGSInsertOrUpdate(new CloudSettings("last_date", Util.curModelDir.Split('_')[4]));
            Util.l($"Složka pro aktuální data: { Util.curModelDir}");

            if (Util.curModelDir == null)
                return;

            Util.ShowLoading("Načítání modelů, ORP masek a spektra...");
            ScanDir();
            Util.HideLoading();
        }

        public string generateCurrentModelDir() {
            string dir = "models__";
            DateTime dateTime = DateTime.Now;
            //dateTime = dateTime.AddHours(-11);

            List<int> fcHours = new List<int> { 3, 9, 15, 21};
            int fittedHour = 0;
            
            foreach (var time in fcHours) {
                if (dateTime.Hour - time >= 0) {
                    fittedHour = time;
                }
                else {
                    break;
                }
            }
            if (fittedHour == 0)
            {
                fittedHour = fcHours.Last();
                dateTime = dateTime.AddDays(-1);
            }

            string day = (dateTime.Day.ToString().Length<2)?("0"+ dateTime.Day.ToString()):dateTime.Day.ToString();
            string month = (dateTime.Month.ToString().Length<2)?("0"+ dateTime.Month.ToString()):dateTime.Month.ToString();
            string hour = (fittedHour.ToString().Length<2)?("0"+ fittedHour.ToString()):fittedHour.ToString();
            string year = dateTime.Year.ToString();

            Util.firstSample = hour;

            dir = dir+hour + "h"+"__"+day+"."+month+"."+year;
            //Util.l($"{dir}");
            return dir;
        }

        public void ScanDir()
        {
            try
            {
                List<Task<bool>> tasks = new List<Task<bool>>();
                string dirPath = Util.pathSource["models"] + Util.curModelDir;
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                Util.validData = true;
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    tasks.Add(Task.Run(() => ScanModel(model)));
                }

                Task.WaitAll(tasks.ToArray());
                                
                //ShowLog();
            }
            catch (Exception e)
            {
                Util.l("Nejsou k dispozici aktuální data!");
                Util.validData = false;
            }
        }

        private bool ScanModel(string model)
        {
            string dirPath = Util.pathSource["models"] + Util.curModelDir;
            string orpMask = Util.pathSource["masks"] + model + ".bmp";
            if (File.Exists(orpMask))
            {
                Preloader.Log("Načítání masky: " + orpMask);
                var masks = LoadMask((Bitmap)Image.FromFile(orpMask), model);
                if (masks.Count > 0)
                {
                    var submodel = LoadSubmodelAndSpectrum(dirPath, model);
                    if (submodel.Count > 0)
                    {
                        CloudSendInputData(new DataInput()
                        {
                            ModelName = model,
                            SubmodelSpectrum = submodel,
                            Mask = masks
                        });
                        return true;
                    }
                }
            }
            else
                LogErrors.Add($"Maska {orpMask} nenalezena pro model {model}");
            return false;
        }

        private void CloudSendInputData(DataInput dataInput)
        {
            Model.Cloud.DBInitialization(dataInput);
            Preloader.Log($"Uloženo do db: {dataInput.ModelName}");
        }

        private void ShowLog()
        {
            if (LogErrors.Count > 0)
            {
                FormLog fl = new FormLog(LogErrors);
                fl.ShowDialog();
            }
        }

        private List<DataMask> LoadMask(Bitmap orp, string modelName)
        {
            List<DataMask> mask = new List<DataMask>();
            try
            {
                var mapCR =
                     from x in Enumerable.Range(0, orp.Width - 1)
                     from y in Enumerable.Range(0, orp.Height - 1)
                     select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

                mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));

                Dictionary<string, JArray> data = new Dictionary<string, JArray>();
                foreach (var map in mapCR)
                {
                    if (data.ContainsKey(map.color.Name))
                    {
                        JArray array = data[map.color.Name];
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data[map.color.Name] = array;
                    }
                    else
                    {
                        JArray array = new JArray();
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data.Add(map.color.Name, array);
                    }
                }

                foreach (var map in data)
                {
                    mask.Add(new DataMask()
                    {
                        Color= "#" + map.Key.Substring(2, 6),
                        Coods = JsonConvert.SerializeObject(map.Value)
                    });
                }
            }
            catch (Exception ex)
            {
                Util.l(ex.ToString());
            }

            return mask;
        }

        private Dictionary<string, List<DataSpectrum>> LoadSubmodelAndSpectrum(string path, string model)
        {
            Dictionary<string, List<DataSpectrum>> ret = new Dictionary<string, List<DataSpectrum>>();

            List<string> subdirs = new List<string>(Directory.EnumerateDirectories(path + "\\" + model));
            foreach (var subdir in subdirs)
            {
                string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                string pathSpectrum = $"{Util.pathSource["scales"]}{model}&{submodel}.csv";
                if (File.Exists(pathSpectrum))
                {
                    Preloader.Log("Načítání spektra: " + pathSpectrum);
                    var spectrum = LoadSpectrumCSV(pathSpectrum);
                    if (spectrum.Count>0)
                    {
                        ret.Add(submodel, spectrum);
                    }
                }
                else
                    LogErrors.Add($"Spektrum {pathSpectrum} nenalezeno pro model {model}/{submodel}");
            }
            return ret;
        }

        private List<DataSpectrum> LoadSpectrumCSV(string filename, char separator=';')
        {
            List<DataSpectrum> listOfRecords = new List<DataSpectrum>();
            using (var reader = new StreamReader(filename))
            {
                var header = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.IndexOf(separator) > 0)
                    {
                        var values = line.Split(separator);
                        if (values.Length <= 3)
                        {
                            listOfRecords.Add(new DataSpectrum()
                            {
                                Rank = values[0],
                                Color = values[1],
                                Type = values[2],
                            });
                        }
                    }
                }
            }
            return listOfRecords;
        }

    }
}
