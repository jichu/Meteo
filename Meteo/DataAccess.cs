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
        public List<CloudORP> GetORPs()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORP>("SELECT * FROM ORP").ToList();
                return output;
            }
        }
        public List<CloudORP> GetORPsPointByColor(string color)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORP>("dbo.ORP_GetPointByColor @Color",new { Color=color }).ToList();
                return output;
            }
        }
    }
}
