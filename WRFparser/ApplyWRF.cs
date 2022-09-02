using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WRFparser
{
    public class ApplyWRF
    {
        public event EventHandler OnCompleted;
        public Output Output { get; set; } = new Output();
        private int CountOfORP { get; set; } = 0;

        public ApplyWRF(List<string> filterOnlyThisORP = null, string configFilename = null)
        {
            var cfg = configFilename == null ? "WRFparser.config.json" : configFilename;
            if (!File.Exists(cfg)) return;
            WRFparser.Init(cfg);

            foreach (var orp in WRFparser.Config.ORP)
            {
                if(filterOnlyThisORP!=null)
                    if (!filterOnlyThisORP.Contains(orp["name"].ToString()))
                        continue;

                CountOfORP++;
               _ = RunWebAsync(orp["url"].ToString(), orp["name"].ToString());
            }
            _ = Task.Run(Completed);
        }

        private async Task RunWebAsync(string url, string name)
        {
            var wv = new Microsoft.Web.WebView2.WinForms.WebView2();
            wv.Tag = name;
            await wv.EnsureCoreWebView2Async();
            wv.CoreWebView2.Navigate(url);
            wv.CoreWebView2.DOMContentLoaded += (sende1r, args1) =>
            {
                wv.CoreWebView2.FrameNavigationCompleted += (sende, args) =>
                {
                    Thread.Sleep(10);
                    if (Output.DicData.ContainsKey(wv.Tag.ToString())) return;
                    _ = LoadHtmlAsync2(wv);
                };
            };
        }
        private bool cookiesOnce = true;
        private async Task LoadHtmlAsync2(WebView2 wv)
        {
            if (cookiesOnce)
            {
                string clickCookies = "document.getElementById(\"accept-choices\").click();";
                await wv.CoreWebView2.ExecuteScriptAsync(clickCookies);
                cookiesOnce = false;
            }

            int wait = WRFparser.Config.Delay;
            string click = "document.querySelector(\"[data-name='2d_w']\").click();let fce = function() { let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };document.body.outerHTML = result;};setTimeout(fce,"+wait+");";
            await wv.CoreWebView2.ExecuteScriptAsync(click);
            Thread.Sleep(wait*2);
            string html = await wv.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");

            JArray ja = WRFparser.Parse(html);

            if (Output.DicData.ContainsKey(wv.Tag.ToString())) return;

            Output.JsonData.Add(new JObject(
                    new JProperty("name", wv.Tag),
                    new JProperty("wind", ja)
                    ));
            Output.DicData.Add(wv.Tag.ToString(), ja.ToObject<List<string>>());
        }

        public async Task Completed()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (true)
            {
                if (Output.DicData.Count == CountOfORP)
                {
                    watch.Stop();
                    Output.ExecutionTime = watch.ElapsedMilliseconds;
                    Close();
                    return;
                }
                if(watch.ElapsedMilliseconds>WRFparser.Config.Timeout)
                {
                    watch.Stop();
                    Output.ExecutionTime = watch.ElapsedMilliseconds;
                    Output.ErrorTimeout = true;
                    Close();
                    return;
                }
                Thread.Sleep(40);
            }
        }

        private void Close()
        {
            CheckData();
            OnCompleted?.Invoke(this, new EventArgs());
        }

        private void CheckData()
        {
            foreach(var orp in Output.DicData)
            {
                if (orp.Value.Count == 0)
                    Output.ErrorDataNull.Add(orp.Key);
            }
        }
    }
}
