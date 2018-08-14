using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public static class Util
    {
        //public static Dictionary<Color, List<Point>> mapCR = new Dictionary<Color, List<Point>>();
        public static List<string> spektrumRadar = new List<string>() { "fffcfcfc", "ffa00000", "fffc0000", "fffc5800", "fffc8400", "fffcb000", "ffe0dc00", "ff9cdc00", "ff34d800", "ff00bc00", "ff00a000", "ff006cc0", "ff0000fc", "ff3000a8", "ff380070" };
        public static Dictionary<string, float> spektrumSrazky = new Dictionary<string, float>
        {
            { "ffffb200", 8 },
            { "ffe3df00", 10 }
        };
        public static Dictionary<int, string> palette = new Dictionary<int, string>();

        public static bool Develop = false;

        public static string curModelName { get; set; }
        public static string curSubmodelName { get; set; }
        public static string ExceptionText = "Exception";
        public static char logMessageDelimiter = '|';

        public static void ShowLoading(string message)
        {
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
                return;
            View.FormLoader = new FormLoader(message);
            View.FormLoader.TopMost = true;
            View.FormLoader.StartPosition = FormStartPosition.CenterScreen;
            View.FormLoader.Show();
            View.FormLoader.Refresh();
            Thread.Sleep(500);
            Application.Idle += OnLoaded;
        }

        private static void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            View.FormLoader.Close();
        }

        public static void l(object obj, Dictionary<string, object> logOptions = null )
        {
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "toFile", false },
                { "filename", "log.txt" },
                { "clearFile", false},
                { "messageBoxIcon", MessageBoxIcon.Warning}
            };

            if (logOptions != null)
                foreach (var item in logOptions) {
                    if (options.ContainsKey(item.Key)) options[item.Key] = item.Value;
                }
            
            Console.WriteLine(obj);
            if (obj.GetType().ToString().Contains(ExceptionText) || (bool) options["toFile"])
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter((string) options["filename"], !((bool) options["clearFile"])))
                    {
                        file.WriteLine(obj);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            if (obj.ToString().Contains(logMessageDelimiter))
                MessageBox.Show(obj.ToString().Split(logMessageDelimiter)[0], obj.ToString().Split(logMessageDelimiter)[1], MessageBoxButtons.OK, (MessageBoxIcon) options["messageBoxIcon"]);
        }

    }
}
