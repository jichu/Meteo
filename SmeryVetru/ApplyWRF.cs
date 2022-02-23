using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmeryVetru
{
    class ApplyWRF
    {
        private JArray Outputs = new JArray();

        public ApplyWRF()
        {
            WRFparser.WRFparser.Init("SmeryVetru.json");

            foreach (var orp in WRFparser.WRFparser.Config.ORP)
            {
                RunWeb(orp["url"].ToString(), orp["name"].ToString());
            }
        }

        private void RunWeb(string url, string name)
        {
            Microsoft.Web.WebView2.WinForms.WebView2 wv = new Microsoft.Web.WebView2.WinForms.WebView2();
            wv.Source = new Uri(url);
            wv.Tag = name;
            wv.NavigationCompleted += webView_NavigationCompleted;
        }

        private async void webView_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
        }

        private async void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
        }


        private async Task LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
        {
            string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
            string html = "";
            var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
            o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
            html = o.ToString();
            JArray ja = WRFparser.WRFparser.Parse(html);
            Outputs.Add(new JObject(
                new JProperty("name", wv.Tag),
                new JProperty("wind", ja)
                ));
            /*
            WRFparser.WRFparser.DebugTest = Config.Debug;
            WRFparser.WRFparser.Time = Config.Time;
            WRFparser.WRFparser.Name = wv.Tag.ToString();
            JArray ja = WRFparser.WRFparser.Parse(html);
            outputs.Add(new JObject(
                new JProperty("name", wv.Tag),
                new JProperty("wind", ja)
                ));
            */
        }
        public async Task Completed()
        {
            while (true)
            {
                if (Outputs.Count == WRFparser.WRFparser.Config.ORP.Count)
                {
                    Console.WriteLine(Outputs);
                    break;
                }
                Thread.Sleep(50);
            }
            /*
            labelProgressInfo.Text = $"ORP {outputs.Count} z {Config.ORP.Count}";
            progressBar.Value = (int)((float)outputs.Count /(float) Config.ORP.Count * 100);
            if (outputs.Count==Config.ORP.Count)
            {
                SaveOutput();
                timer.Stop();
                labelProgressInfo.Text += $" ✔";
            }*/
        }
    }
}
