using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public partial class FormSetModelsDir : Form
    {
        public FormSetModelsDir()
        {
            InitializeComponent();
        }

        private void FormSetModelsDir_Load(object sender, EventArgs e)
        {
            ScanModelsDir();
        }

        private void ScanModelsDir()
        {
            Util.modelsDir.Clear();
            bool change = false;
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(Util.pathSource["models"]));
            foreach (var dir in dirs)
            {
                string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                if (!Util.modelsDir.Contains(model))
                {
                    Util.modelsDir.Add(model);
                    change = true;
                }
            }
            if (change)
                ShowComboBoxModels();
        }

        private void ShowComboBoxModels()
        {
            //comboBoxModels.Items.Clear();
            foreach (var model in Util.modelsDir)
            {
                comboBoxModels.Items.Add(model);
            }
            if (comboBoxModels.Items.Count > 0)
            {
                comboBoxModels.SelectedIndex = 0;
                comboBoxModels.SelectedIndexChanged += new EventHandler(comboBoxModels_SelectedIndexChanged);
            }
            else
                comboBoxModels.SelectedIndexChanged -= new EventHandler(comboBoxModels_SelectedIndexChanged);
        }

        private void comboBoxModels_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxModels.SelectedIndex != -1)
                Util.curModelDir = comboBoxModels.SelectedItem.ToString();

            Close();
        }
    }
}
