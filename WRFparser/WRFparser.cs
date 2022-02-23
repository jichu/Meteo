﻿using System;
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
    public class WRFparser
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
            foreach (var orp in Config.ORP)
            {
                RunWeb(orp["url"].ToString(), orp["name"].ToString());
                Console.WriteLine($"{orp["name"]}");
            }
            
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

            Debug.WriteLine(numbers.Count);

            int count = 1;
            int timeCounter = 1;
            foreach (var i in numbers)
            {
                if (count % 23 == 0)
                {
                    if (timeCounter >= (int)Time[0] && timeCounter <= (int)Time[1])
                        ja.Add(i);
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

        private static void RunWeb(string url, string name)
        {
            Microsoft.Web.WebView2.WinForms.WebView2 wv = new Microsoft.Web.WebView2.WinForms.WebView2();
            wv.Source = new Uri(url);
            wv.Tag = name;
            wv.NavigationCompleted += webView_NavigationCompleted;
        }


        private static async void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //MessageBox.Show("sdfg");
            //(sender as Microsoft.Web.WebView2.WinForms.WebView2).CoreWebView2.DOMContentLoaded += webView_DOMContentLoaded;
            await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
        }


        private static async Task LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
        {
            string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
            string html = "";
            var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
            o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
            html = o.ToString();
            /*
            while (true)
            {
                Thread.Sleep(100);
                var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
                o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
                html = o.ToString();
            Debug.WriteLine(html);
            }
            */
            DebugTest = Config.Debug;
            Time = Config.Time;
            Name = wv.Tag.ToString();
            JArray ja = Parse(html);
            outputs.Add(new JObject(
                new JProperty("name", wv.Tag),
                new JProperty("wind", ja)
                ));
        }
    }

}
