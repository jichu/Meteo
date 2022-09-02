﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewerSmery.Map
{
    internal class MaskORP
    {
        public MaskORP()
        {
            CreateRegions();
        }

        private void CreateRegions()
        {
            try
            {
                Bitmap orp = Data.Resources.BitmapMapMaskORP;

                var mapCR =
                     from x in Enumerable.Range(0, orp.Width - 1)
                     from y in Enumerable.Range(0, orp.Height - 1)
                     select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

                mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));

                Dictionary<string, JArray> data = new Dictionary<string, JArray>();
                foreach (var map in mapCR)
                {
                    string colorName = "#" + map.color.Name.Substring(2, 6);
                    if (data.ContainsKey(colorName))
                    {
                        JArray array = data[colorName];
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data[map.color.Name] = array;
                    }
                    else
                    {
                        JArray array = new JArray();
                        JArray p = new JArray();
                        p.Add(map.point.X);
                        p.Add(map.point.Y);
                        array.Add(p);
                        data.Add(colorName, array);
                    }
                }

                Data.Region.ORPcoods = data;

                /*
                foreach (var map in data)
                {
                    Debug.WriteLine(JsonConvert.SerializeObject(map.Value));
                }
                */
            }
            catch (Exception ex)
            {
                Utils.Log.Error(ex);
            }
        }
    }
}
