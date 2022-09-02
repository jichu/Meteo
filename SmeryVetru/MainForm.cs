using ExcelDataReader;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmeryVetru
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Do();
            //Do(new List<string>() { "Zlín" });
        //    _ = RunWebAsync("https://www.windguru.cz/343224", "Zlín");
        }


        private void Do(List<string> missed=null)
        {
            var wrf = new WRFparser.ApplyWRF(missed);
            wrf.OnCompleted += WRF_completed;
        }

        private int WRFattempt = 2;
        private void WRF_completed(object sender, EventArgs e)
        {
            var output = (sender as WRFparser.ApplyWRF).Output;
            Console.WriteLine($"WRF comleted: In time: {(double)output.ExecutionTime / 1000}");

            if (output.ErrorTimeout)
            {
                Console.WriteLine("WRF: ErrorTimeout");
                return;
            }

            if (WRFattempt > 0)
                if (output.ErrorDataNull.Count > 0)
                {
                    foreach (var i in output.ErrorDataNull)
                        Console.WriteLine($"WRF: Data {i} not found");

                    WRFattempt--;
                    //Do(output.ErrorDataNull);
                    //return;
                }

            foreach (var i in output.DicData)
                Console.WriteLine($"WRF: {i.Key} : {i.Value.Count}");


            SaveOutput((sender as WRFparser.ApplyWRF).Output.JsonData.ToString());
        }

        private async Task RunWebAsync(string url, string name)
        {
            wv.Tag = name;
            await wv.EnsureCoreWebView2Async();
            wv.CoreWebView2.Navigate(url);

            wv.CoreWebView2.DOMContentLoaded += (sende1r, args1) =>
            {
                wv.CoreWebView2.FrameNavigationCompleted += (sende, args) =>
                { 
                    //Thread.Sleep(10);
                    var task = LoadHtmlAsync(wv);
                    Console.WriteLine(task.Result);
                };
            };
            /*
            wv.Source = new Uri(url);
            wv.NavigationCompleted += webView_NavigationCompleted;
            */
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
            _ = wv.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(click);
        }

        private async Task<bool> LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
        {
            int wait = 200;
            string click = "document.querySelector(\"[data-name='2d_w']\").click();let fce = function() { let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };document.body.outerHTML = result;};setTimeout(fce," + wait + ");";
            await wv.CoreWebView2.ExecuteScriptAsync(click);

            Thread.Sleep(wait * 2);
            string html = await wv.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
            Console.WriteLine(html);
            return true;
            /*
            string clickCookies = "document.getElementById(\"accept-choices\").click();";
            var r = await wv.CoreWebView2.ExecuteScriptAsync(clickCookies);
            Console.WriteLine(r);
            string click = "document.documentElement.outerHTML";
            var o = await wv.CoreWebView2.ExecuteScriptAsync(click);

            Console.WriteLine(o.ToString());
            */
            /*
            int max_count = 2000;
            while (!o.IsCompleted && --max_count > 0)
            {
                //Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
            if (max_count > 0)
            {

                Console.WriteLine(o.ToString());
            }
            else
            {
                Console.WriteLine("timeout");
            }
            */

        }


        private void timer_Tick(object sender, EventArgs e)
        {/*
            labelProgressInfo.Text = $"ORP {outputs.Count} z {Config.ORP.Count}";
            progressBar.Value = (int)((float)outputs.Count /(float) Config.ORP.Count * 100);
            if (outputs.Count==Config.ORP.Count)
            {
                SaveOutput();
                timer.Stop();
                labelProgressInfo.Text += $" ✔";
            }*/
        }
        
        private void SaveOutput(string outputs)
        {
            System.IO.File.WriteAllText("_tt.txt", outputs.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            of.FilterIndex = 1;
            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //LoadExcel(of.FileName);
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nelze otevřít",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        /*
        private void LoadExcel(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    JArray ORP = new JArray();
                    do
                    {
                        while (reader.Read()) 
                        {                            
                            JObject jo = new JObject();
                            for (int column = 0; column < reader.FieldCount; column++)
                            {
                                if (column == 0)
                                    jo.Add(new JProperty("name", reader.GetString(column)));
                                if (column == 1)
                                    jo.Add(new JProperty("url", reader.GetString(column)));
                            }
                            ORP.Add(jo);
                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET
                    Config.ORP = ORP;
                    Config.Save();
                }
            }
        }*/
    }
}
