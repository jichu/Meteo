using Meteo.JSONparser;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WRFdll;

namespace Meteo
{
    public partial class FormMain : System.Windows.Forms.Form
    {
        dynamic ControlActive = null;
        public bool Preloader { get; set; } = false;

        public FormMain()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            View.FormMain = this;
            menuSettingWind.Checked = false;
            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
         //   Util.ShowLoading("Načítání aplikace...");
            new Controller();
            //ApplyWRF();
            //this.menuItemOutput.PerformClick();            
        }

        public void ShowControlLoader(string message="Zpracování...")
        {
            if (panelLayout.InvokeRequired)
                panelLayout.BeginInvoke((Action)(() =>
                {
                    LoaderOn(message);
                }));
            else
                LoaderOn(message);
        }

        private void LoaderOn(string message)
        {
            UserControlLoader.Instance.Visible = true;
            if (!panelLayout.Controls.Contains(UserControlLoader.Instance))
            {
                panelLayout.Controls.Add(UserControlLoader.Instance);
                UserControlLoader.Instance.Dock = DockStyle.Fill;
            }
            UserControlLoader.Instance.BringToFront();
            UserControlLoader.Instance.UpdateMessage(message);
            Preloader = true;
            modelyToolStripMenuItem.Enabled = false;
        }

        public void HideControlLoader()
        {
            /*if (ControlActive.InvokeRequired)
                ControlActive.BeginInvoke((Action)(() =>
                {
                    LoaderOff();
                }));
            else
                LoaderOff();*/
            LoaderOff();
        }

        private void LoaderOff()
        {
            //ControlActive.BringToFront();
            UserControlLoader.Instance.Visible = false;
            Preloader = false;
            modelyToolStripMenuItem.Enabled = true;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName))
                {
                    process.Kill();
                }
                foreach (var process in Process.GetProcessesByName("Meteo.vshost"))
                {
                    process.Kill();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // menu MODELY
        private void menuItemLoadModels_Click(object sender, EventArgs e)
        {
            LoadModels();
            /*
            DialogResult dialogResult = MessageBox.Show("Jste si jistí, že chcete přepat data aktuálníma maskama ORP a adresářovou strukturou?", "Načíst data do databáze", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                FormSetModelsDir f = new FormSetModelsDir();
                f.ShowDialog();
                PreImage mapORP = new PreImage();
                mapORP.LoadORPfromModels();
            }
            */
        }

        private void LoadModels()
        {
            Model.Cloud.DBClear();
            new LoadData();
            Util.HideLoading();
        }

        private void menuItemExplore_Click(object sender, EventArgs e)
        {
            if (!panelLayout.Controls.Contains(UserControlModel.Instance))
            {
                panelLayout.Controls.Add(UserControlModel.Instance);
                UserControlModel.Instance.Dock = DockStyle.Fill;
                UserControlModel.Instance.BringToFront();
            }
            else
                UserControlModel.Instance.BringToFront();

        }

        private void menuItemLoadInputs_Click(object sender, EventArgs e)
        {
            Util.l(Util.curModelDir);
            if (Util.curModelDir == null)
            {
                FormSetModelsDir dlg = new FormSetModelsDir();
                dlg.ShowDialog();
            }
            LoadInputs();

            //  backgroundWorkerEnumerationModels.RunWorkerAsync(); //INPUT_DATA
        }

        private void LoadInputs()
        {
            if (Util.curModelDir == null)
                    return;
            Util.ShowLoading("Načítání vstupů...", "", false);
            List<Task> tasks = new List<Task>();
            WRF.Init(new Dictionary<string, string>{
                 { "mask", Util.pathSource["wrf_mask"] },
                 { "mask_orp", Util.pathSource["masks"]+"Model_WRF_NMM_FLYMET.bmp" }
                }, false);

            tasks.Add(Task.Run(() => UserControlModel.Instance.EnumerationModels()));
            Task.WaitAll(tasks.ToArray());
            Util.HideLoading();
        }

        private void menuItemOutput_Click(object sender, EventArgs e)
        {
            if (!panelLayout.Controls.Contains(UserControlOutput.Instance))
            {
                panelLayout.Controls.Add(UserControlOutput.Instance);
                UserControlOutput.Instance.Dock = DockStyle.Fill;
                UserControlOutput.Instance.BringToFront();
            }
            else
                UserControlOutput.Instance.BringToFront();

            ControlActive = UserControlOutput.Instance;
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                PreImage mapORP = new PreImage();
                mapORP.LoadORPfromModels();
            }

            if (e.KeyCode == Keys.P)
            {
            }
        }

        private void menuItemPalette_Click(object sender, EventArgs e)
        {
            if (!panelLayout.Controls.Contains(UserControlPaletteForMask.Instance))
            {
                panelLayout.Controls.Add(UserControlPaletteForMask.Instance);
                UserControlPaletteForMask.Instance.Dock = DockStyle.Fill;
                UserControlPaletteForMask.Instance.BringToFront();
            }
            else
                UserControlPaletteForMask.Instance.BringToFront();

            ControlActive = UserControlPaletteForMask.Instance;
        }

        private void backgroundWorkerEnumerationModels_DoWork(object sender, DoWorkEventArgs e)
        {
            UserControlModel.Instance.EnumerationModels();
        }

        private void MenuItemAlgorithm_Click(object sender, EventArgs e)
        {
            LoadAlgorithm();
        }

        private void LoadAlgorithm()
        {
            Util.ShowLoading("Algoritmus počítá předpověď....");
            try
            {
                new StormEngine(algorithms.statistic_forecast);
                //UserControlOutput.Instance.Render();
            }
            catch (Exception ex)
            {
                Util.l("Chyba při výpočtu");
            }
            Util.HideLoading();
        }

        private void menuItemForecast_Click(object sender, EventArgs e)
        {
            if (!panelLayout.Controls.Contains(UserControlForecast.Instance))
            {
                panelLayout.Controls.Add(UserControlForecast.Instance);
                UserControlForecast.Instance.Dock = DockStyle.Fill;
                UserControlForecast.Instance.BringToFront();
            }
            else
                UserControlForecast.Instance.BringToFront();

            ControlActive = UserControlForecast.Instance;
        }

        private void FormMain_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void menuDoAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAll();
            /*LoadModels();
            LoadInputs();
            LoadAlgorithm();*/
        }

        public void DoAll() {
            LoadModels();
            LoadInputs();
            LoadAlgorithm();
            closeMeteo();
        }

        private void menuSettingWind_Click(object sender, EventArgs e)
        {
            menuSettingWind.Checked = !menuSettingWind.Checked;
        }

        private void menuItemStatForecasting_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Začátek počítání statistické předpovědi");
            LoadAlgorithm();
            closeMeteo();
        }

        //WRF Apply smery vetru
        private JArray Outputs = new JArray();

        public void ApplyWRF()
        {
            WRFparser.WRFparser.Init("config/SmeryVetru.json");

            foreach (var orp in WRFparser.WRFparser.Config.ORP)
            {
                RunWeb(orp["url"].ToString(), orp["name"].ToString());
                Console.WriteLine(orp["name"]);
            }
            //_=Completed
        }

        private void RunWeb(string url, string name)
        {
            using (Microsoft.Web.WebView2.WinForms.WebView2 wv = new Microsoft.Web.WebView2.WinForms.WebView2())
            {
                wv.Source = new Uri(url);
                wv.Tag = name;
                wv.NavigationCompleted += webView_NavigationCompleted;
                Thread.Sleep(WRFparser.WRFparser.Config.Delay*2);
            }
        }


        private async void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await LoadHtmlAsync((sender as Microsoft.Web.WebView2.WinForms.WebView2));
        }


        private async Task LoadHtmlAsync(Microsoft.Web.WebView2.WinForms.WebView2 wv)
        {
            string click = "document.querySelector(\"[data-name = '2d_w']\").click();";
            Thread.Sleep(WRFparser.WRFparser.Config.Delay);
            string html = "";
            var o = await wv.CoreWebView2.ExecuteScriptAsync(click);
            o = await wv.CoreWebView2.ExecuteScriptAsync("let g =document.querySelectorAll('#tabid_1_content_div svg g g');let result = '';for (let x of g) { result += x.getAttribute('transform') + ';'; };result");
            html = o.ToString();

            WRFparser.WRFparser.DebugTest = WRFparser.WRFparser.Config.Debug;
            WRFparser.WRFparser.Time = WRFparser.WRFparser.Config.Time;
            WRFparser.WRFparser.Name = wv.Tag.ToString();
            JArray ja = WRFparser.WRFparser.Parse(html);
            Outputs.Add(new JObject(
                new JProperty("name", wv.Tag),
                new JProperty("wind", ja)
                ));
            Console.WriteLine(ja);

        }
        public async Task Completed()
        {
            while (true)
            {
                Console.WriteLine(Outputs.Count);
                if (Outputs.Count == WRFparser.WRFparser.Config.ORP.Count)
                {
                    Console.WriteLine(Outputs);
                    break;
                }
                Thread.Sleep(50);
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            //DoAll();
        }

        private void closeMeteo() {
            Application.Exit();
        }
    }
}

