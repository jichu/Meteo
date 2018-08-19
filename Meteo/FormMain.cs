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

namespace Meteo
{
    public partial class FormMain : System.Windows.Forms.Form
    {

        public FormMain()
        {
            InitializeComponent();
            View.FormMain = this;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Util.ShowLoading("Načítání aplikace...");
            new Controller();
            this.menuItemExplore.PerformClick();

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
            DialogResult dialogResult = MessageBox.Show("Jste si jistí, že chcete přepat data aktuálníma maskama ORP a adresářovou strukturou /models/?", "Načíst data do databáze", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                PreImage mapORP = new PreImage();
                mapORP.LoadORPfromModels();
            }
            else if (dialogResult == DialogResult.No)
            {
            }

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

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
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
    }
}
