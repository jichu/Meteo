using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;

namespace Meteo.CsvHelper
{
    internal static class CSVexport
    {
        private static string Filename { get; set; }
        private static string DirectoryPath { get; set; } = "export";
        private static List<CSVdataOutputTemplate> records = new List<CSVdataOutputTemplate>();

        internal static void AddRow(CSVdataOutputTemplate data)
        {
            records.Add(data);
        }

        internal static void Write(string path = "output")
        {
            Filename = path;
            Task.Run(() => DoWrite());
        }

        private static void DoWrite()
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);

                using (var writer = new StreamWriter(Path.Combine(DirectoryPath,Filename+".csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = ";";
                    csv.WriteRecords(records);
                }
                records.Clear();
            }
            catch (Exception e)
            {
                Util.l(e.Message);
            }
        }
    }
}
