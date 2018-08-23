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
                List<CloudMaskSpectrum> records = new List<CloudMaskSpectrum>();
                records.Add(item);
                conn.Execute("dbo.MASK_SPECTRUM_InsertOrUpdateData @ID, @ID_ORP, @COODS", records);
                
                return true;
            }
        }

        public List<CloudModelSpectrum> ModelSpectrumGetScaleForModels(string mod, string submod, string typeName = "DEFAULT")
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = MODELSGetSubmodelIDFromName(mod, submod);
                int typeId = Model.Cloud.ModelSpectrumTypeGetIDForName(typeName);
                var output = conn.Query<CloudModelSpectrum>("dbo.MODEL_SPECTRUM_GetScaleForModel @Model, @type", new { model = id, type=typeId }).ToList();

                return output;
            }
        }

        public bool ModelSpectrumInsertOrUpdate(CloudModelSpectrum item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                List<CloudModelSpectrum> records = new List<CloudModelSpectrum>();
                records.Add(item);
                conn.Execute("dbo.MODEL_SPECTRUM_InsertOrUpdateData @ID_MODEL, @RANK, @COLOR, @TYPE", records);

                return true;
            }
        }

        public int ModelSpectrumTypeGetIDForName(string typeName)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = conn.Query<CloudModels>("dbo.MODEL_SPECTRUM_TYPE_GetIdForName @name", new { name = typeName}).ToList().First().id;

                return id;
            }

        }

        public bool MODELSInsertOrUpdate(CloudModels item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                List<CloudModels> records = new List<CloudModels>();
                records.Add(item);
                conn.Execute("dbo.MODELS_InsertOrUpdateData @name, @id_parent, @options", records);

                return true;
            }
        }

        public bool MODELSInsertOrUpdateParent(string name)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                CloudModels item = new CloudModels(name);
                List<CloudModels> records = new List<CloudModels>();
                records.Add(item);
                conn.Execute("dbo.MODELS_InsertOrUpdateData @name, @id_parent, @options", records);

                return true;
            }
        }

        public int MODELSGetIDFromName(string nam)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = conn.Query<CloudModels>("dbo.MODELS_GetIDFromName @NAME", new { name = nam }).ToList().First().id;
                return id;
            }

        }

        public int MODELSGetSubmodelIDFromName(string namModel, string namSubmodel)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = conn.Query<CloudModels>("dbo.MODELS_GetSubmodelIDFromName @NMODEL, @NSUBMODEL", new { nmodel = namModel, nsubmodel = namSubmodel }).ToList().First().id;

                return id;
            }

        }

        public string MODELSGetModelOptions(string namModel, string namSubmodel)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                string options = conn.Query<CloudModels>("dbo.MODELS_GetModel @NMODEL, @NSUBMODEL", new { nmodel = namModel, nsubmodel = namSubmodel }).ToList().First().options;

                return options;
            }

        }

        public List<CloudModelEntity> MODELSGetModelStructure()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudModelEntity>("dbo.MODELS_GetModelStructure").ToList();
                return output;
            }
        }

        public bool ORPColorInsertOrUpdate(CloudORPColor item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                List<CloudORPColor> records = new List<CloudORPColor>();
                records.Add(item);
                conn.Execute("dbo.ORP_COLOR_InsertOrUpdateData @ID_ORP, @COLOR", records);

                return true;
            }
        }

        public List<CloudORPColor> ORPColorGetORPColors()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORPColor>("dbo.ORP_COLOR_GetORPColors").ToList();

                return output;
            }
        }

        public int ORPSGetIDFromName(string nam)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                int id = conn.Query<CloudORPS>("dbo.ORPS_GetIDFromName @NAME",new {name =  nam}).ToList().First().id;
                return id;
            }
            
        }

        public bool ORPSInsertOrUpdate(CloudORPS item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                List<CloudORPS> records = new List<CloudORPS>();
                records.Add(item);
                conn.Execute("dbo.ORPS_InsertOrUpdateData @ID, @NAME", records);

                return true;
            }
        }

        public List<CloudORPS> ORPSGetORPNames()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORPS>("dbo.ORPS_GetORPNames").ToList();

                return output;
            }
        }

        public List<CloudORPS> REGIONSGetRegionCities()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudORPS>("dbo.REGIONS_GetRegionCities").ToList();

                return output;
            }
        }

        public List<CloudSettings> SETTINGSGetSettings()
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                var output = conn.Query<CloudSettings>("dbo.SETTINGS_GetSettings").ToList();

                return output;
            }
        }

        public bool SETTINGSInsertOrUpdate(CloudSettings item)
        {
            using (IDbConnection conn = new SqlConnection(Model.ConnStr("Cloud")))
            {
                List<CloudSettings> records = new List<CloudSettings>();
                records.Add(item);
                conn.Execute("dbo.SETTINGS_InsertOrUpdateSettings @option_name, @option_value", records);

                return true;
            }
        }

    }
}
