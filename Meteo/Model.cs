using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Meteo
{
    public static class Model
    {
        public static DataAccess Cloud = new DataAccess();

        public static string ConnStr(string name)
        {
            if (Util.Develop)
            return ConfigurationManager.ConnectionStrings[name].ConnectionString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug",""));
        else
            return ConfigurationManager.ConnectionStrings[name].ConnectionString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory);
        }

    }
}
