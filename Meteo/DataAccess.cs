using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Meteo
{
    public class DataAccess
    {
        public List<CloudMaskSpectrum> GetORPs()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudMaskSpectrum>("SELECT * FROM ORP").ToList();
                return output;
            }
        }
        public List<CloudMaskSpectrum> GetORPsPointByColor(string color)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudMaskSpectrum>("dbo.ORP_GetPointByColor @Color",new { Color=color }).ToList();
                return output;
            }
        }

        public List<CloudMaskSpectrum> GetMSCoodsByColor(string color)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudMaskSpectrum>("dbo.MASK_SPECTRUM_GetCoodsByColor @Color", new { Color = color }).ToList();
                               
                return output;
            }
        }
    }
}
