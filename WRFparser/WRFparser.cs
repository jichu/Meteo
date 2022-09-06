using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WRFparser
{
    internal class WRFparser
    {
        private static List<int> numbers = new List<int>();
        public static JArray Time { get; set; } = new JArray() { 4, 11 };
        private static char sep = ';';
        public static string Name{ get; set; } = "temp";
        public static bool DebugTest { get; set; } = true;
        public static Config Config { get; set; } = new Config();

        private static JArray outputs = new JArray();

        public WRFparser(string configFile)
        {
            
            Console.WriteLine(Config.Delay);
            
        }

        public static void Init(string configFile)
        {
            LoadConfig(configFile);            
            
        }

        public static void LoadConfig(string filenameConfig = "")
        {
            try
            {
                if (!File.Exists(filenameConfig)) return;
                using (var r = new StreamReader(filenameConfig))
                {
                    string json = r.ReadToEnd();
                    Config = JsonConvert.DeserializeObject<Config>(@json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static JArray Parse(string html)
        {
            
            numbers = new List<int>();
            JArray ja = new JArray();
            if (html.IndexOf(sep) == -1) return ja;

            foreach (var item in html.Split(sep))
            {
                if (item.IndexOf(' ') == -1) continue;
                string rotate = item.Split(' ')[1];
                rotate = rotate.Replace("rotate(", "").Replace(")", "").Replace(",0,0","");
                int i = 0;
                int.TryParse(rotate, out i);
                numbers.Add(i);
            }

            if(DebugTest)
                CreateBitmap();

            //Debug.WriteLine(numbers.Count);

            int count = 1;
            int timeCounter = 1;
            foreach (var i in numbers)
            {
                if (count % 23 == 0)
                {
                    if (timeCounter >= (int)Time[0] && timeCounter <= (int)Time[1])
                        //ja.Add(i.ToString());
                        ja.Add(prevod((float)i));
                    timeCounter++;
                }
                count++;
            }
            return ja;
        }

        private static void CreateBitmap()
        {
            int size = 500;
            Bitmap bmp = new Bitmap(size*4, size);
            int offset = 20;
            Graphics g = Graphics.FromImage(bmp);
            int x = 0;
            int y = size-offset-20;
            int yStart = y;
            int count = 0;
            int timeCounter = 1;
            foreach (var i in numbers)
            {
                count++;
                if (count % 23 == 0)
                {
                    if(timeCounter>=(int)Time[0]&& timeCounter <= (int)Time[1])
                        g.DrawImage(CreateBitmapArrow(i - 90, Color.DarkGreen), new Point(x, y));
                    else 
                        g.DrawImage(CreateBitmapArrow(i - 90, Color.Black), new Point(x, y));
                    timeCounter++;
                }
                else
                    g.DrawImage(CreateBitmapArrow(i - 90, Color.DarkRed), new Point(x, y));
                y -= offset;
                if (count%23==0)
                {
                    x += offset;
                    y = yStart;
                }
                if (y <= 0)
                    break;
            }
            Show(bmp);
        }

        private static Bitmap CreateBitmapArrow(int angle, Color color)
        {
            int size = 20;
            Bitmap bmp = new Bitmap(size,size);
            int s = size/10;
            using (var g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform(size/2, size/2);  
                g.RotateTransform(angle);    
                g.TranslateTransform(-size/2, -size/2); 
                g.DrawLine(new Pen(color, (float)1), new Point(0, size/2), new Point(size, size/2));
                g.DrawLine(new Pen(color, (float)1), new Point(size/4, size / 2+s), new Point(size/4, size / 2-s));
            }
            return bmp;
        }

        private static void Show(Bitmap bmp, string title = "")
        {
            //new FormTemplate(title, bmpNew).Show();                
            string dir = "temp";
            if(!Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            bmp.Save(dir+"/"+Name+".png", ImageFormat.Png);
        }

        private static string prevod(float deg)
        {
            if (deg <= 22.5 || deg > 337.5) return "S";
            else if (deg <= 67.5 && deg > 22.5) return "SV";
            else if (deg <= 112.5 && deg > 67.5) return "V";
            else if (deg <= 157.5 && deg > 112.5) return "JV";
            else if (deg <= 202.5 && deg > 157.5) return "J";
            else if (deg <= 247.5 && deg > 202.5) return "JZ";
            else if (deg <= 292.5 && deg > 247.5) return "Z";
            else if (deg <= 337.5 && deg > 292.5) return "SZ";
            else return "Error";
        }

    }

}
