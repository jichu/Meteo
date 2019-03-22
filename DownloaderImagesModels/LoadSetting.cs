using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderImagesModels
{
    static class LoadSetting
    {
        public static List<Record> listOfRecords { get; set; }

        public static bool ReadCSVFile(string filename)
        {
            if (!File.Exists(filename))
                return false;
            using (var reader = new StreamReader(filename))
            {
                listOfRecords = new List<Record>();
                var header = reader.ReadLine(); //načte hlavičku
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length >= 3)
                    {
                        Record record = new Record();
                        record.Model = values[0];
                        record.Submodel = values[1];
                        record.URL = values[2];
                        listOfRecords.Add(record);
                    }
                }
                return true;
            }
        }

    }
}
