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
       public List<CloudMaskSpectrum> MaskSpectrumGetCoodsByColor(string col)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudMaskSpectrum>("dbo.MASK_SPECTRUM_GetCoodsByColor @Color", new { color = col }).ToList();
                               
                return output;
            }
        }
        public List<CloudMaskSpectrum> MaskSpectrumGetAllColors()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudMaskSpectrum>("dbo.MASK_SPECTRUM_GetAllColors").ToList();

                return output;
            }
        }
    }
}
