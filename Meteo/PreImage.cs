﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Meteo
{
    class PreImage
    {
        private Dictionary<string, List<Point>> mapORP=new Dictionary<string, List<Point>>();

        public PreImage()
        {
		}

        public Dictionary<string, List<Point>> getMapORP() {
            return mapORP;
        }


        public void LoadORPfromModels()
        {
            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory+@"\models\";
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    string orpMask = dir+@"\"+ model + ".bmp";
                    Util.ShowLoading("Načítání a ukládání ORP masek do databáze...");
                    if (File.Exists(orpMask))
                    {
                        Util.l("Load mask: " + orpMask);
                        Thread t = new Thread(() => LoadORP((Bitmap)Image.FromFile(orpMask)));
                        t.Start();
                    }
                    Console.WriteLine("{0}", dir.Substring(dir.LastIndexOf("\\") + 1));
                }
                //View.FormMain.Close();
            }
            catch(Exception e)
            {
                Util.l(e);
            }
        }


        private void LoadORP(Bitmap orp)
        {
            try
            {

                var mapCR =
                     from x in Enumerable.Range(0, orp.Width - 1)
                     from y in Enumerable.Range(0, orp.Height - 1)
                     select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

                mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));

                Dictionary<string, JArray> data = new Dictionary<string, JArray>();
                foreach (var map in mapCR)
                {
                    if (data.ContainsKey(map.color.Name))
                    {
                        JArray array = data[map.color.Name];
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
                        data.Add(map.color.Name, array);
                    }
                }

                Chu.data = data;
                foreach (var map in data)
                {
                    Util.l(JsonConvert.SerializeObject(map.Key));
                    Util.l(JsonConvert.SerializeObject(map.Value));
                    Chu.color = JsonConvert.SerializeObject(map.Key);
                    Chu.coords = JsonConvert.SerializeObject(map.Value);
                }
            }
            catch (Exception ex)
            {
                Util.l(ex.ToString());
            }
        }





        private void button1_Click(object sender, EventArgs e)
		{
            /*
			Thread t = new Thread(new ThreadStart(LoadORP));
			t.Start();
            */
        }

    }
}
