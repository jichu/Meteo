using ExcelDataReader;
using Microsoft.Web.WebView2.Core;
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
        //private List<Microsoft.Web.WebView2.WinForms.WebView2> wv = new List<Microsoft.Web.WebView2.WinForms.WebView2>();
        //public static Config Config { get; set; } = new Config();

        private JArray outputs = new JArray();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Do();
            //WRFparser.WRFparser wRFparser = new WRFparser.WRFparser("SmeryVetru.json");
            /*
            //Config.Save();
            Config.Load();

            timer.Start();


            WRFparser.WRFparser.Init("SmeryVetru.json");

            foreach (var orp in WRFparser.WRFparser.Config.ORP)
            {
                RunWeb(orp["url"].ToString(), orp["name"].ToString());
            }
            */

        }

        private async void Do()
        {
            var wrf = new ApplyWRF();
            //await wrf.Completed();
        }

        /*
private void RunWeb(string url, string name)
{
   Microsoft.Web.WebView2.WinForms.WebView2 wv = new Microsoft.Web.WebView2.WinForms.WebView2();
   wv.Source = new Uri(url);
   wv.Tag = name;
   wv.NavigationCompleted += webView_NavigationCompleted;
}

private async void webView_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
{
   Debug.WriteLine("ready");
   await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
}

private async void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
{
   //MessageBox.Show("sdfg");
   //(sender as Microsoft.Web.WebView2.WinForms.WebView2).CoreWebView2.DOMContentLoaded += webView_DOMContentLoaded;
   await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
}


private async Task LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
{         
   string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
   Thread.Sleep(Config.Delay);
   string html = "";
   var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
   o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
   html = o.ToString();

   WRFparser.WRFparser.DebugTest = Config.Debug;
   WRFparser.WRFparser.Time = Config.Time;
   WRFparser.WRFparser.Name = wv.Tag.ToString();
   JArray ja = WRFparser.WRFparser.Parse(html);
   outputs.Add(new JObject(
       new JProperty("name", wv.Tag),
       new JProperty("wind", ja)
       ));
}
*/
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
        
        /*

        private void SaveOutput()
        {
            System.IO.File.WriteAllText(Config.OutputFile, outputs.ToString());
        }
        */
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
