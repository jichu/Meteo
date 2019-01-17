using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public partial class FormPromt : Form
    {
        internal string Value { get; set; }

        public FormPromt(PromtData pd)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = pd.Width;
            labelTitle.Text = pd.Title + ":";
            textBoxValue.Text = pd.Value;
            buttonOK.Text = pd.OK;
        }
        
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Value = textBoxValue.Text;
            Close();
        }
    }

    public class PromtData
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string OK { get; set; }
        public int Width { get; set; }
    }

}
