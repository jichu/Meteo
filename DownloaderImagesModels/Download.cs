using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloaderImagesModels
{
    class Download 
    {
        public List<string> Errors = new List<string>();

        private Dictionary<string, string> rulesReplace = new Dictionary<string, string>
            {
                { "[yyyy]", DateTime.Now.Year.ToString() },
                { "[MM]", DateTime.Now.ToString("MM") },
                { "[dd]", DateTime.Now.ToString("dd") }
            };

        private string ruleCounter = "[cc]";
        private string ruleCounterShort = "[c]";

        private int stepHour=3;

        public Download()
        {
            Thread t = new Thread(() => Do());
            t.Start();
        }

        int counterSuccess = 0;
        public void Do()
        {
            Errors.Clear();
            try
            {
                Util.ProcessReady("Prosím čekejte...");
                int i = 0;
                foreach (var item in LoadSetting.listOfRecords)
                {

                    AutoCounter($"./models/{item.Model}/{item.Submodel}/", ReplaceDate(item.URL));
                    /*
                    if(i>2)
                        break;
                    i++;
                    */

                }

                Util.ProcessReady($"Uloženo celkem {counterSuccess} obrázků do adresáře models. Chyb: {Errors.Count}");

                if(Errors.Count>0)
                {
                    ErrorsSaveLog();
                }
            }
            catch (Exception e)
            {
                Util.l(e);
            }
        }

        private void ErrorsSaveLog()
        {
            string filename = "log.txt";

            //File.WriteAllText(filename, String.Empty);
            using (StreamWriter sw = File.CreateText(filename))
            {
                foreach (string err in Errors)
                {
                    sw.WriteLine(err);
                }
            }
        }

        private void AutoCounter(string path, string url)
        {
            for (int i = 0; i <= 24; i+= stepHour)
            {
                string cc = "";
                string link = "";
                if (url.IndexOf(ruleCounter) != -1)
                {
                    cc = i < 10 ? "0" + i.ToString() : i.ToString();
                    link = url.Replace(ruleCounter, cc);
                }
                if (url.IndexOf(ruleCounterShort) != -1)
                {
                    cc = i.ToString();
                    link = url.Replace(ruleCounterShort, cc);
                }
                if (cc != "")
                {
                    Util.l($"Načítám/ukládám do: {path} {link}");
                    cc = i < 10 ? "0" + i.ToString() : i.ToString();
                    if (SaveImage(path, link, cc + ".png"))
                    {
                        counterSuccess++;
                    }
                }
            }
        }

        private string ReplaceDate(string url)
        {
            foreach (var item in rulesReplace)
            {
                if (url.IndexOf(item.Key) != -1)
                {
                    url = url.Replace(item.Key, item.Value);
                }
            }
            return url;
        }

        public bool SaveImage(string path, string url, string filename)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(url);
                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var img = Image.FromStream(mem))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(path);
                            img.Save(path + filename, ImageFormat.Png);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Util.l("Chyba (nelze stáhnout) z "+url);
                Errors.Add(url);
                return false;
            }
        }

    }
}
