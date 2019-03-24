using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
                { "[yy]", DateTime.Now.ToString("yy") },
                { "[MM]", DateTime.Now.ToString("MM") },
                { "[dd]", DateTime.Now.ToString("dd") }
            };

        private string ruleCounter = "[cc]";
        private string ruleCounterShort = "[c]";
        private string ruleCounterNow = "[scc:";
        private string ruleCounterNowShort = "[sc:";

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
                int skip = 14;
                foreach (var item in LoadSetting.listOfRecords)
                {
                    i++;
                    /*
                    if (i < skip)
                        continue;

    */

                    AutoCounter($"./models/{item.Model}/{item.Submodel}/", ReplaceDate(item.URL));
                    /*
                    if(i>skip-1)
                        break;
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
            if (url.IndexOf(ruleCounter) != -1 | url.IndexOf(ruleCounterShort) != -1)
                Counter24(path, url);

            if (url.IndexOf(ruleCounterNow) != -1)
                CounterNow(path, url, ruleCounterNow);

            if (url.IndexOf(ruleCounterNowShort) != -1)
                CounterNow(path, url, ruleCounterNowShort);
        }
        
        private void Counter24(string path, string url)
        {
            for (int i = 0; i <= 24; i += stepHour)
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
                    cc = i < 10 ? "0" + i.ToString() : i.ToString();
                    if (SaveImage(path, link, cc + ".png"))
                    {
                        counterSuccess++;
                    }
                }
            }
        }

        private void CounterNow(string path, string url, string rc="[scc")
        {
            var regex = new Regex(@"\"+rc+@"\d+:\d+:\d\]");
            char sep = ':';
            int hour = 0;
            int.TryParse(DateTime.Now.ToString("HH"),out hour);
            foreach (Match match in regex.Matches(url))
            {
                string rule = match.Value.Replace("[","").Replace("]", "");
                if (rule.IndexOf(sep)!=-1)
                {
                    string[] counter = rule.Split(sep);
                    if(counter.Length==4)
                    {
                        int from = 0;
                        int to = 0;
                        int update = 6;
                        int.TryParse(counter[1], out from);
                        int.TryParse(counter[2], out to);
                        int.TryParse(counter[3], out update);

                        int startHour = FindHourByUpdate(hour, update);
                        int counterHour = 0;

                        if (to>=from)
                            for (int i= from; i <= to; i++)
                            {
                                string index = rc.IndexOf("cc") != -1 ? (i < 10 ? "0" + i.ToString() : i.ToString()):i.ToString();
                                string link = url.Replace(match.Value, index);
                                string c = (startHour + counterHour).ToString();
                                if (startHour + counterHour > 24)
                                    break;
                                counterHour += stepHour;
                                string cc = c.Length < 2 ? "0" + c : c;
                                if (SaveImage(path, link, cc+".png"))
                                {
                                    counterSuccess++;
                                }
                            }
                    }
                }
            }
        }

        private int FindHourByUpdate(int hour, int update)
        {
            for (int i = 0; i <= 24; i += update)
            {
                if (Enumerable.Range(i, update).Contains(hour))
                    return i;
            }
            return 0;
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
                            Util.l($"Načítám/ukládám do: {path+filename} {url}");
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
