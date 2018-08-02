using Newtonsoft.Json;
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
        Bitmap orp = Properties.Resources.ORP;
        private Dictionary<string, List<Point>> mapORP=new Dictionary<string, List<Point>>();
        public PreImage()
        {
            //View.FormMain.button1.Click += new EventHandler(button1_Click);
            //LoadORP();
            LoadORPfromModels();

		}

        public Dictionary<string, List<Point>> getMapORP() {
            return mapORP;
        }

		private void oldLoadORP()
		{
			try
			{
				Util.ShowLoading("ORP...");
                

                /*List<CloudORP> l = Model.Cloud.GetORPs();
                foreach(var ll in l)
                {
                    Util.l($"{ll.Color}  point: {ll.Point}");
                }*/

                /*l= Model.Cloud.GetORPsPointByColor("fff");
                foreach (var ll in l)
                {
                    Util.l(ll.Point);
                }*/
                List<CloudMaskSpectrum> l = Model.Cloud.MaskSpectrumGetCoodsByColor("#fff");
                Util.l("Tu to bude vypisovat");
                foreach (var ll in l)
                {
                    //string json = @"{'coods':[[11,21],[12,22]]}";
                    string jsonString = ll.coods; 
                    JObject jsonObj = JObject.Parse(jsonString);
                    JArray posArray = (JArray)jsonObj["coods"];
                    string posX = (string)jsonObj["coods"][0][0];
                    string posY = (string)jsonObj["coods"][0][1];
                    Util.l($"Barva: {ll.color}");
                    Util.l("X:Y");
                    foreach (JArray arr in posArray) {
                          Util.l(String.Join(":", arr.Select(i => i.ToString())));  
                    }
                }


                

                var mapCR =
                     from x in Enumerable.Range(0, orp.Width - 1)
                     from y in Enumerable.Range(0, orp.Height - 1)
                     select new { color = orp.GetPixel(x, y), point = new Point(x, y) };

                mapCR = mapCR.Where((key, val) => !(key.color.Name == "ffffffff" || key.color.Name == "ff000000"));
                /*
                List<Color> greyColors = new List<Color>();
                foreach (var map in mapCR)
                {
                    if (!greyColors.Exists(x => x == map.color))
                        greyColors.Add(map.color);
                }
                foreach (var color in greyColors)
                {
                    foreach (var map in mapCR.Where((key, val) => key.color.Name == color.Name))
                    {
                        Util.l(map.color.Name + " " + map.point.X + "x" + map.point.Y);
                    }
                }*/

                
                foreach (var map in mapCR)
                {
                    if (mapORP.ContainsKey(map.color.Name))
                    {
                        List<Point> pp = mapORP[map.color.Name];
                        pp.Add(map.point);
                        mapORP[map.color.Name]=pp;
                    }
                    else
                    {
                        List<Point> p = new List<Point>();
                        p.Add(map.point);
                        mapORP.Add(map.color.Name, p);
                    }
                }

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

                foreach (var map in data)
                 {
                         Util.l(JsonConvert.SerializeObject(map.Value));
                 }
            }
			catch (Exception ex)
			{
				Util.l(ex.ToString());
			}
		}

        private void LoadORPfromModels()
        {
            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory+@"\models\";
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    string orpMask = dir+@"\"+ model + ".bmp";
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
                Util.ShowLoading("ORP...");

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

                foreach (var map in data)
                {
                    Util.l(JsonConvert.SerializeObject(map.Value));
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
