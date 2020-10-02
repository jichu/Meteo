using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.Data
{
    internal static class Stream
    {
        internal static JObject JRoot { get; set; }
        internal static JObject JData { get; set; }
        internal static JArray GetJRoot(string key)
        {
            try
            {
                if (JRoot.ContainsKey(key))
                    return (JArray)JRoot[key];
                return new JArray();
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return new JArray();
            }
        }
        internal static JArray GetJData(string key)
        {
            try
            {
                if(JData!=null)
                if (JData.ContainsKey(key))
                    return (JArray)JData[key];
                return new JArray();
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return new JArray();
            }
        }
        internal static JArray GetJDataValues()
        {
            try
            {
                if (JData != null)
                    if (JData.ContainsKey(Cache.Config.GetDataType("data")))
                    return (JArray)JData[Cache.Config.GetDataType("data")][Cache.indexHour][Cache.indexOutputlist];
                return new JArray();
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return new JArray();
            }
        }
        internal static int GetJDataValue(int hour, int output, int index)
        {
            try
            {
                if (JData != null)
                    if (JData.ContainsKey(Cache.Config.GetDataType("data")))
                        return (int)JData[Cache.Config.GetDataType("data")][hour][output][index];
                return -1;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return -1;
            }
        }
    }
}
