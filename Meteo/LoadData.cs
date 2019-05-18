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
        public LoadData()
        {

            Util.ShowLoading("Načítání modelů, ORP masek a ukládání do databáze...");
            Thread t = new Thread(() => SaveToDB());
            t.Start();

        }

        public void SaveToDB()
        {
            FormSetModelsDir dlg = new FormSetModelsDir("Chcete přepat data vybranou adresářovou strukturou, maskami a stupnicemi?");
            dlg.ShowDialog();

            if (Util.curModelDir == null)
                return;

            try
            {
                string dirPath = Util.pathSource["models"] + Util.curModelDir;
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    string orpMask = Util.pathSource["masks"] + model + ".bmp";


                    List<string> subdirs = new List<string>(Directory.EnumerateDirectories(dirPath + "\\" + model));
                    //Model.Cloud.MODELSInsertOrUpdate(new CloudModels(model));
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        if (File.Exists(orpMask))
                        {
                            Util.l("Load mask: " + orpMask);
                            Thread t = new Thread(() => LoadModel(model, submodel));
                            t.Start();
                        }
                        //Model.Cloud.MODELSInsertOrUpdate(new CloudModels(submodel, model, options));
                    }

                    /*
                    List<string> subdirs = new List<string>(Directory.EnumerateDirectories(dirPath + "\\" + model));
                    //Model.Cloud.MODELSInsertOrUpdate(new CloudModels(model));
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        string options = Model.Cloud.MODELSGetModelOptions(model, submodel);
                        JObject jo = JObject.Parse(options);
                        var p = jo.Property("countMethod");
                        if (p == null)
                        {
                            FormSetOptions f = new FormSetOptions(model, submodel, options);
                            f.ShowDialog(View.FormMain);
                            options = f.options;
                            Util.l(model + " " + options);
                        }
                        //Model.Cloud.MODELSInsertOrUpdate(new CloudModels(submodel, model, options));
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Util.l(e);
            }
        }

        private void LoadModel(string model, string submodel)
        {
            string orpMask = Util.pathSource["masks"] + model + ".bmp";
            if (File.Exists(orpMask))
            {
                Util.l("Load mask: " + orpMask);
                //LoadMask((Bitmap)Image.FromFile(orpMask), model);
            }
            else
                Util.l($"Maska {orpMask} nenalezena pro model {model}");
        }

        private void LoadMask(Bitmap orp, string modelName)
        {
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
                    string regionName = Util.GetRegionNameByColorForLoading("#" + map.Key.Substring(2, 6));
                    if (regionName != "")
                        Model.Cloud.MaskSpectrumInsertOrUpdate(new CloudMaskSpectrum(modelName, regionName, JsonConvert.SerializeObject(map.Value)));

                }
            }
            catch (Exception ex)
            {
                Util.l(ex.ToString());
            }
        }




    }
}
