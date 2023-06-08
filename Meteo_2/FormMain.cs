using Meteo.JSONparser;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WRFparser;

namespace Meteo
{
    public partial class FormMain : System.Windows.Forms.Form
    {
        dynamic ControlActive = null;
        public bool Preloader { get; set; } = false;
        public static Config Config { get; set; }
        private NotifyIcon notifyIcon = new NotifyIcon();

        public FormMain()
        {
            InitializeComponent();
            Log.Clear();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Config.Load();
            NotifyIcon();
            this.Hide();
            View.FormMain = this;
            menuSettingWind.Checked = false;
            Application.Idle += Application_Idle;
        }

        private void NotifyIcon()
        {
            notifyIcon.Text = "Meteo";
            notifyIcon.Icon = (Icon)Properties.Resources.icon;
            notifyIcon.Visible = true;
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem exitApplication = new ToolStripMenuItem();
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            exitApplication.Text = "Zavřít";
            exitApplication.Click += new EventHandler(ExitApplication_Click);
            contextMenu.Items.Add(exitApplication);
        }

        public void ShowBalloonTip(string text, int duration)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                notifyIcon.BalloonTipText = text;
                notifyIcon.ShowBalloonTip(duration * 1000);
            }));
        }

        private void ExitApplication_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if(!Config.Debug) return;
            if (!this.Visible)
            {
                this.TopMost = true;
                this.Show();
            }
            else
                this.Hide();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();

            }));
            ShowBalloonTip(this.Text, 2);

            new Controller();

            //Automatické spuštění celého výpočtu
            LaunchAll();

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

            notifyIcon = null;
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
            Util.l($"Spuštění předpovědního algoritmu...");
            Util.ShowLoading("Algoritmus počítá předpověď....");
            try
            {
                new StormEngine(algorithms.statistic_forecast);
                //UserControlOutput.Instance.Render();
            }
            catch (Exception ex)
            {
                Util.l("Chyba při výpočtu");
                Util.l(ex.Message);
                Util.validData = false;
                new StormEngine(algorithms.statistic_forecast);
            }
            Util.HideLoading();
            Util.l($"Výpočet ukončen");
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
        }

        public void DoAll() {

            LoadModels();

            if (Util.validData)
            {
                LoadInputs();
            }
            LoadAlgorithm();
            closeMeteo();
        }

        private void menuSettingWind_Click(object sender, EventArgs e)
        {
            menuSettingWind.Checked = !menuSettingWind.Checked;
        }

        private void menuItemStatForecasting_Click(object sender, EventArgs e)
        {
            Util.l("Začátek počítání statistické předpovědi");

            LoadAlgorithm();
            closeMeteo();
        }

        private CancellationTokenSource ts = new CancellationTokenSource();

        public void LaunchAll() {
            ApplyWRF();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {


        }

        private void closeMeteo()
        {
            Close();
            Application.Exit();
        }

        ApplyWRF wrf;

        public void ApplyWRF(List<string> missed = null)
        {
            Util.ShowLoading("Načítání a zpracování směrů větru...", "", false);
            wrf = new WRFparser.ApplyWRF(missed);
            wrf.OnCompleted += WRF_completed;
        }

        private int WRFattempt = 10;
        public Dictionary<string, List<string>> WRFDict { get; set; } = new Dictionary<string, List<string>>();
        private void WRF_completed(object sender, EventArgs e)
        {
            var output = (sender as WRFparser.ApplyWRF).Output;
            Log.Add($"WRF completed: In time: {(double)output.ExecutionTime / 1000}");

            if (output.ErrorTimeout)
            {
                Log.Add("WRF: ErrorTimeout");
                return;
            }

            foreach (var i in output.DicData)
                if (!WRFDict.ContainsKey(i.Key) && i.Value.Count!=0) WRFDict.Add(i.Key, i.Value);

            if (WRFattempt > 0)
                if (output.ErrorDataNull.Count > 0)
                {
                    foreach (var i in output.ErrorDataNull)
                        Log.Add($"WRF: Data {i} not found");

                    WRFattempt--;
                    
                    this.Invoke(new Action(() =>
                    {
                        wrf = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        ApplyWRF(output.ErrorDataNull);
                    }));
                    //ApplyWRF(output.ErrorDataNull);
                    return;
                }
            output.DicData = WRFDict;
            foreach (var i in output.DicData) 
                Log.Add($"WRF: {i.Key} : {i.Value.Count}");            
            Util.l($"Pokusů:{10-WRFattempt+1}");
            File.WriteAllText("wrf.tmp", output.JsonData.ToString());

            List<string> tempArr = new List<string>{ };
            List<string> majorWind = new List<string> { };
            List<string> typeWind = new List<string> { "S", "SV", "V", "JV", "J", "JZ", "Z", "SZ"};
            for (int i = 0; i < 8; i++)
            {//i < output.DicData.First().Value.Count
                foreach (var item in output.DicData) {
                    try
                    {
                        tempArr.Add(item.Value[i]);
                    }
                    catch (ArgumentOutOfRangeException ex) {
                        //Util.l(ex);
                        continue;
                    }
                }
                int count = 0;
                string temporaryType = "";

                foreach (var item in typeWind)
                {
                    if (tempArr.Count(j => j == item) > count)
                    {
                        count = tempArr.Count(j => j == item);
                        temporaryType = item;
                    }
                }

                majorWind.Add(temporaryType);
                count = 0;
                tempArr.Clear();
            }

            Util.majorWinds = majorWind;

            //Application.DoEvents();
            this.Invoke(new Action(() =>
            {
                Kill();
                Util.ShowLoading($"WRF completed: In time: {(double)output.ExecutionTime / 1000}");
                DoAll();
            }));
            
        }
        public void Kill()
        {

            /*Process[] workers = Process.GetProcessesByName("msedgewebview2");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }*/
            /*string strCmdText = "taskkill /im msedgewebview2.exe /t /F";
            System.Diagnostics.Process.Start("CMD.exe",strCmdText);*/
        }
    }
}

