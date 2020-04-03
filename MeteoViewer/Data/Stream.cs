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
                    if (JData.ContainsKey("data"))
                    return (JArray)JData["data"][Cache.indexHour][Cache.indexOutputlist];
                return new JArray();
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return new JArray();
            }
        }
        internal static int GetJDataValueByRegionName(string name)
        {
            int ret = -1;
            try
            {
                if (JData != null)
                    if (JData.ContainsKey("data"))
                    {
                        JArray orplist = GetJRoot("orplist");
                        if (orplist.Count > 0)
                        {
                            int index = orplist.TakeWhile(x => x.ToString() != name).Count();
                            JArray values = GetJDataValues();
                            if (values.Count!=orplist.Count) return ret;
                            if (index==-1) return ret;
                            if (index== orplist.Count) return ret;
                            return (int)values[index];
                        }
                        else
                            return ret;
                    }
                return ret;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
                return ret;
            }
        }
    }
}
