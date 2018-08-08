﻿using System;
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
        public List<CloudMaskSpectrum> MaskSpectrumGetAllColorsForModel(string mod)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = Model.Cloud.MODELSGetIDFromName(mod);
                var output = conn.Query<CloudMaskSpectrum>("dbo.MASK_SPECTRUM_GetAllColorsForModel @Model", new {model = id }).ToList();

                return output;
            }
        }
        public bool MaskSpectrumInsertOrUpdate(CloudMaskSpectrum item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                item.ShowRecord();
                List<CloudMaskSpectrum> records = new List<CloudMaskSpectrum>();
                records.Add(item);
                conn.Execute("dbo.MASK_SPECTRUM_InsertOrUpdateData @ID, @ID_ORP, @COLOR, @COODS", records);
                
                return true;
            }
        }
        public int ORPSGetIDFromName(string nam)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORPS>("dbo.ORPS_GetIDFromName @NAME",new {name =  nam}).ToList();

                int id = 0;
                foreach (var o in output)
                {
                    id = o.id;
                }
                return id;
            }
            
        }
        public int MODELSGetIDFromName(string nam)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudModels>("dbo.MODELS_GetIDFromName @NAME", new { name = nam }).ToList();

                int id = 0;
                foreach (var o in output)
                {
                    id = o.id;
                }
                return id;
            }

        }

        public int MODELSGetSubmodelIDFromName(string namModel, string namSubmodel)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudModels>("dbo.MODELS_GetSubmodelIDFromName @NMODEL, @NSUBMODEL", new { nmodel = namModel, nsubmodel=namSubmodel }).ToList();

                int id = 0;
                foreach (var o in output)
                {
                    id = o.id;
                }
                return id;
            }

        }

        public List<CloudModelSpectrum> ModelSpectrumGetScaleForModels(string mod, string submod)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = MODELSGetSubmodelIDFromName(mod, submod);
                var output = conn.Query<CloudModelSpectrum>("dbo.MODEL_SPECTRUM_GetScaleForModel @Model", new { model = id }).ToList();

                return output;
            }
        }
    }
}
