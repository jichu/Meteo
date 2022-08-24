using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WRFparser
{
    public class ApplyWRF
    {
        public event EventHandler OnCompleted;
        public JArray Outputs = new JArray();

        public ApplyWRF(string configFilename = null)
        {
            var cfg = configFilename == null ? "WRFparser.config.json" : configFilename;
            if (!File.Exists(cfg)) return;
            WRFparser.Init(cfg);

            foreach (var orp in WRFparser.Config.ORP)
            {
                _ = RunWebAsync(orp["url"].ToString(), orp["name"].ToString());
            }
            _ = Task.Run(Completed);
        }

        private async Task RunWebAsync(string url, string name)
        {
            Microsoft.Web.WebView2.WinForms.WebView2 wv = new Microsoft.Web.WebView2.WinForms.WebView2();
            wv.Tag = name;
            await wv.EnsureCoreWebView2Async();
            wv.CoreWebView2.Navigate(url);
            //wv.Source = new Uri(url);
            wv.NavigationCompleted += webView_NavigationCompleted;
        }

        private async void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Thread.Sleep(WRFparser.Config.Delay);
            await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
        }


        private async Task LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
        {
            string clickCookies = "document.getElementById(\"accept-choices\").click();";
            await wv.CoreWebView2.ExecuteScriptAsync(clickCookies);

            string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
            string html = "";
            var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
            o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
            html = o.ToString();
            JArray ja = WRFparser.Parse(html);
            Outputs.Add(new JObject(
                new JProperty("name", wv.Tag),
                new JProperty("wind", ja)
                ));
        }

        public async Task Completed()
        {
            while (true)
            {
                if (Outputs.Count == WRFparser.Config.ORP.Count)
                {
                    Close();
                    return;
                }
                Thread.Sleep(50);
            }
        }

        private void Close()
        {
            OnCompleted?.Invoke(this, new EventArgs());
        }
    }
}
