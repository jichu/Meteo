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
            /*
            //string connstr = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename={AppDir}Cloud3.mdf;Integrated Security=True;Connect Timeout=30";
            string connstr = @"Data Source=Meteo;Initial Catalog=C:\SOURCE\REPOS\METEO_2\CLOUD3.MDF;Integrated Security=True;Connect Timeout=30";
            //var connstr = ConfigurationManager.ConnectionStrings["Cloud"].ConnectionString;
            if (Util.Develop)
                return connstr.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", ""));
                //return ConfigurationManager.ConnectionStrings[name].ConnectionString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug",""));
            else
                return ConfigurationManager.ConnectionStrings[name].ConnectionString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory);
            */
            return ConfigurationManager.ConnectionStrings["Meteo.Properties.Settings.Cloud"].ConnectionString;
            
        }

    }
}
