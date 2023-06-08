using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo
{
    public static class Log
    {
        private static string PathLog { get; set; } = "logs";
        private static string FileLog { get; set; } = "app.log";

        internal static void Clear()
        {
            try
            {
                if (File.Exists(Path.Combine(PathLog, FileLog)))
                    File.Delete(Path.Combine(PathLog, FileLog));
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }
        internal static void Add(string msg)
        {
            try
            {
                Console.WriteLine(msg);
                if (!Directory.Exists(PathLog))
                    Directory.CreateDirectory(PathLog);

                using (StreamWriter stream = File.AppendText(Path.Combine(PathLog, FileLog)))
                {
                    stream.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
