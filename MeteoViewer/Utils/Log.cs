using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Utils
{
    internal static class Log
    {
        private static string pathLog = "log";
        private static string format = "yyMMdd_HHmmss";
        private static string ext = ".log";
        private static void CreatePathLog()
        {
            if (!Directory.Exists(pathLog))
                Directory.CreateDirectory(pathLog);
        }

        internal static void Error(Exception ex, string message="")
        {
            try
            {
                CreatePathLog();
                string path = Path.Combine(pathLog,$"err_{DateTime.Now.ToString(format)}{ext}");
                Console.WriteLine($"Error -> {path}");
                using (StreamWriter sw = File.CreateText(path))
                {
                    if (message != "")
                        sw.WriteLine(message);
                    sw.Write(ex.ToString() + Environment.NewLine+Environment.NewLine);
                    sw.WriteLine(ex.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Chyba chyby {e.Message}");
            }
        }
    }
}
