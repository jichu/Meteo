using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo.JSONparser
{
    internal static class JSONcreateRoot
    {
        internal static void Save(List<string> orpname, List<string> paramname, List<string> hourname)
        {

        }
        internal static void SaveAll()
        {

            List<object> data = new List<object>();
            data.Add(new { key = 5, key2 = 1 });

            var task = JSONwriter.Do(data, "name");

            task.Wait();

        }
    }
}
