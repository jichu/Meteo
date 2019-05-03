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
using System.Timers;
using System.Windows.Forms;

namespace DownloaderImagesModels
{
    class Download
    {
        public event EventHandler<DownloadEventArgs> EventProcess;

        public List<string> Errors = new List<string>();
        public string downloadHour = "";

        private string pathOutput = "models";
        private string pathOutputDate = "";
        private List<int> timeupdate = new List<int>();
        private List<int> alreadyDownloaded = new List<int>();

        private string ruleDownload = "[s:";
        private string ruleCounter = "[cc]";
        private string ruleCounterShort = "[c]";
        private string ruleCounterNow = "[scc:";
        private string ruleCounterNowShort = "[sc:";
        private string ruleCounterMulti = "[mcc:";

        private int stepHour = 3;
        private bool process = false;

        public Download()
        {
            LoadConfig(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".cfg");

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            int hour = -1;
            int.TryParse(DateTime.Now.ToString("HH"), out hour);
            if (timeupdate.Contains(hour))
                if (!alreadyDownloaded.Contains(hour))
                    if (!process)
                    {
                        downloadHour = hour > 10 ? hour.ToString() : "0" + hour.ToString();
                        alreadyDownloaded.Add(hour);
                        Process();
                    }

            DownloadEventArgs ea = new DownloadEventArgs();
            ea.Process = process;
            ea.Hour = downloadHour == "" ? downloadHour : downloadHour + "h";
            OnProcess(ea);
            if (hour == 0)
                alreadyDownloaded.Clear();
        }

        protected virtual void OnProcess(DownloadEventArgs e)
        {
            var handler = EventProcess;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Process()
        {
            Thread t = new Thread(() => Do());
            t.Start();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        int counterSuccess = 0;
        private void Do()
        {
            process = true;
            Errors.Clear();
            bool csv = LoadSetting.ReadCSVFile(@"data" + downloadHour + ".csv");
            if (!csv)
            {
                Util.l(@"data" + downloadHour + ".csv nenačteno",
                     new Dictionary<string, object>
                     {
                        { "messageBoxIcon", MessageBoxIcon.Warning}
                     }
                );
                process = false;
                return;
            }
            try
            {
                Util.ProcessReady();
                counterSuccess = 0;

                string newPathOutput = pathOutput.Replace("[h]", downloadHour);

                if (pathOutputDate != "")
                    newPathOutput += DateTime.Now.ToString(pathOutputDate);

                int i = 0;
                foreach (var item in LoadSetting.listOfRecords)
                {
                    i++;
                    AutoCounter($"{newPathOutput}/{item.Model}/{item.Submodel}/", ReplaceDate(item.URL));
                }

                Util.ProcessReady($"Uloženo celkem {counterSuccess} obrázků do adresáře models. Chyb: {Errors.Count}");

                if (Errors.Count > 0)
                {
                    ErrorsSaveLog();
                }
            }
            catch (Exception e)
            {
                Util.l(e);
            }

            process = false;
        }

        private void LoadConfig(string fileName)
        {
            if (File.Exists(fileName))
            {
                StreamReader reader = new StreamReader(fileName);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == null) continue;
                    if (line.Length < 2) continue;
                    if (line[0].ToString() == "#") continue;
                    if (line.IndexOf('=') == -1) continue;
                    string[] item = line.Split('=');
                    if (item.Length > 2) continue;
                    string value = item[1];
                    string key = item[0];
                    if (key == "pathOutput") pathOutput = value;
                    if (key == "pathOutputDate") pathOutputDate = value;
                    if (key == "timeupdate") ParseTimeUpdate(value);

                }
                reader.Close();
            }
        }

        private void ParseTimeUpdate(string value)
        {
            if (value.IndexOf(',') != -1)
                foreach (var item in value.Split(','))
                    ParseTimeUpdateHour(item);
            else
                ParseTimeUpdateHour(value);

            foreach (var item in timeupdate)
            {
                Util.LabelTimeupdate(item);
            }
        }
        private void ParseTimeUpdateHour(string value)
        {
            int update = -1;
            int.TryParse(value, out update);
            if (update != -1)
                if (!timeupdate.Contains(update))
                    timeupdate.Add(update);
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
            if (url.IndexOf(ruleDownload) != -1) {
                DownloadOne(path, url, ruleDownload);
                return;
            }

            if (url.IndexOf(ruleCounter) != -1 | url.IndexOf(ruleCounterShort) != -1)
                Counter24(path, url);

            if (url.IndexOf(ruleCounterNow) != -1)
                CounterNow(path, url, ruleCounterNow);

            if (url.IndexOf(ruleCounterNowShort) != -1)
                CounterNow(path, url, ruleCounterNowShort);

            if (url.IndexOf(ruleCounterMulti) != -1)
                CounterMulti(path, url, ruleCounterMulti);

        }

        private void DownloadOne(string path, string url, string rc = "[s:")
        {
            var regex = new Regex(@"\" + rc + @"\d+\]");
            char sep = ':';
            foreach (Match match in regex.Matches(url))
            {
                string rule = match.Value.Replace("[", "").Replace("]", "");
                if (rule.IndexOf(sep) != -1)
                {
                    string[] s = rule.Split(sep);
                    if (s.Length == 2)
                    {
                        string link = url.Replace(match.Value, "");
                        if (SaveImage(path, link, s[1] + ".png"))
                        {
                            counterSuccess++;
                        }
                    }
                }
            }
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

        private void CounterNow(string path, string url, string rc = "[scc:")
        {
            var regex = new Regex(@"\" + rc + @"\d+:\d+:\d+\]");
            char sep = ':';
            int hour = 0;
            int.TryParse(DateTime.Now.ToString("HH"), out hour);
            foreach (Match match in regex.Matches(url))
            {
                string rule = match.Value.Replace("[", "").Replace("]", "");
                if (rule.IndexOf(sep) != -1)
                {
                    string[] counter = rule.Split(sep);
                    if (counter.Length == 4)
                    {
                        int from = 0;
                        int to = 0;
                        int update = 6;
                        int.TryParse(counter[1], out from);
                        int.TryParse(counter[2], out to);
                        int.TryParse(counter[3], out update);

                        int startHour = FindHourByUpdate(hour, update);
                        int counterHour = 0;

                        if (to >= from)
                            for (int i = from; i <= to; i++)
                            {
                                string index = rc.IndexOf("cc") != -1 ? (i < 10 ? "0" + i.ToString() : i.ToString()) : i.ToString();
                                string link = url.Replace(match.Value, index);
                                string c = (startHour + counterHour).ToString();
                                counterHour += stepHour;
                                string cc = c.Length < 2 ? "0" + c : c;
                                if (SaveImage(path, link, cc + ".png"))
                                {
                                    counterSuccess++;
                                }
                            }
                    }
                }
            }
        }

        private void CounterMulti(string path, string url, string rc = "[mcc:")
        {
            var regex = new Regex(@"\" + rc + @"\d+:\d+:\d+:\d+\]");
            char sep = ':';
            foreach (Match match in regex.Matches(url))
            {
                string rule = match.Value.Replace("[", "").Replace("]", "");
                if (rule.IndexOf(sep) != -1)
                {
                    string[] counter = rule.Split(sep);
                    if (counter.Length == 5)
                    {
                        int from = 0;
                        int to = 0;
                        int step = 0;
                        int startHour = 0;
                        int.TryParse(counter[1], out from);
                        int.TryParse(counter[2], out to);
                        int.TryParse(counter[3], out step);
                        int.TryParse(counter[4], out startHour);

                        int counterHour = 0;
                        if (to >= from)
                            for (int i = from; i <= to; i += step)
                            {
                                string index = rc.IndexOf("cc") != -1 ? (i < 10 ? "0" + i.ToString() : i.ToString()) : i.ToString();
                                string link = url.Replace(match.Value, index);
                                string c = (startHour + counterHour).ToString();
                                counterHour += stepHour;
                                string cc = c.Length < 2 ? "0" + c : c;
                                if (SaveImage(path, link, cc + ".png"))
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

        private string ReplaceDate(string url) {
            url = ReplaceDateRule(url, "e");
            url = ReplaceDateRule(url, "f");
            return url;
        }

        private int DateShift = 0;
        private string ReplaceDateRule(string url, string rule)
        {
            DateShift = 0;
            url = ReplaceDateShift(url,rule, '-');
            url = ReplaceDateShift(url,rule, '+');

            Dictionary<string, string> rulesReplace = new Dictionary<string, string>
                {
                    { "[yyyy]", DateTime.Now.Year.ToString() },
                    { "[yy]", DateTime.Now.ToString("yy") },
                    { "[MM]", DateTime.Now.ToString("MM") },
                    { "[dd]", DateTime.Now.ToString("dd") },
                    { "[yyyy"+rule+"]", DateTime.Now.AddDays(DateShift).ToString("yy") },
                    { "[yy"+rule+"]", DateTime.Now.AddDays(DateShift).ToString("yy") },
                    { "[MM"+rule+"]", DateTime.Now.AddDays(DateShift).ToString("MM") },
                    { "[dd"+rule+"]", DateTime.Now.AddDays(DateShift).ToString("dd") }
                };
            foreach (var item in rulesReplace)
            {
                if (url.IndexOf(item.Key) != -1)
                {
                    url = url.Replace(item.Key, item.Value);
                }
            }
            return url;
        }

        private string ReplaceDateShift(string url,string r, char sep)
        {
            var regex = new Regex(@"\["+r+@"\"+sep+@"\d+\]");
            foreach (Match match in regex.Matches(url))
            {
                string rule = match.Value.Replace("[", "").Replace("]", "");
                if (rule.IndexOf(sep) != -1)
                {
                    string[] c = rule.Split(sep);
                    if (c.Length == 2)
                    {
                        int num = 0;
                        int.TryParse(c[1], out num);
                        if (sep == '-') num *= -1;
                        int dd = 0;
                        string day = DateTime.Now.ToString("dd");
                        int.TryParse(day, out dd);
                        DateShift = num;
                        return url.Replace(match.Value, "");
                    }
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
                    webClient.Headers.Add("User-Agent: Other");
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
                Console.WriteLine(e);
                Errors.Add($"Nelze stáhnout: {url} do {path}");
                return false;
            }
        }

    }
}
