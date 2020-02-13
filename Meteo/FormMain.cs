using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WRFdll;

namespace Meteo
{
    public partial class FormMain : System.Windows.Forms.Form
    {

        public FormMain()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            View.FormMain = this;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Util.ShowLoading("Načítání aplikace...");

            var json = new JSONparser.JSONwriter();
            _ = json.Do();

            //CsvHelper.CSVexport.Write();

            /*  
             Dictionary<string,string> wrf =WRF.Process(new Dictionary<string, string>
             {
                 { "source", @".\img1\test.png" },
                 { "mask", Util.pathSource["wrf_mask"] },
                 { "mask_orp", @".\img1\Model_WRF_NMM_FLYMET.bmp" }
             }
             );



             foreach (var r in wrf)
             {
                 Console.WriteLine(r.Key+"  "+r.Value);
             }*/
            
            new Controller();
            this.menuItemOutput.PerformClick();
            
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
            Model.Cloud.DBClear();
            new LoadData();
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
            FormSetModelsDir dlg = new FormSetModelsDir();
            dlg.ShowDialog();
            if (Util.curModelDir == null)
                return;

            WRF.Init(new Dictionary<string, string>{
                 { "mask", Util.pathSource["wrf_mask"] },
                 { "mask_orp", Util.pathSource["masks"]+"Model_WRF_NMM_FLYMET.bmp" }
                }, false);

            Task.Run(() => UserControlModel.Instance.EnumerationModels());

          //  backgroundWorkerEnumerationModels.RunWorkerAsync(); //INPUT_DATA
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
        }

        private void backgroundWorkerEnumerationModels_DoWork(object sender, DoWorkEventArgs e)
        {
            UserControlModel.Instance.EnumerationModels();
        }

        private void MenuItemAlgorithm_Click(object sender, EventArgs e)
        {
            Util.ShowLoading("Algoritmus počítá přepověď....");
            try
            {
                new StormEngine();
            }
            catch (Exception ex) {
                Util.l("Chyba při výpočtu");
            }
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
        }

        private void FormMain_MouseClick(object sender, MouseEventArgs e)
        {
            Util.l(0);
        }
    }
}
