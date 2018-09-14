using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class FormSetOptions : Form
    {
        private JProperty p;

        public string model { get; set; }
        public string submodel { get; set; }
        public string options { get; set; }

        public FormSetOptions(string model, string submodel,string options)
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            labelModel.Text = $"{model}:{submodel}";
            this.model = model;
            this.submodel = submodel;
            this.options = options;
        }
        
        private void FormSetOptions_Load(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string sel = GetCheckedRadio();
            if (sel == string.Empty)
            {
                Util.l("Zvolte metodu.|Nelze uložit");
                return; 
            }
            else
            {
                JObject jo = JObject.Parse(options);
                jo.Add("countMethod", sel);
                options = JsonConvert.SerializeObject(jo);
            }
            this.Close();
        }

        private string GetCheckedRadio()
        {
            foreach (Control control in groupBoxCountMethod.Controls)
            {
                RadioButton radio = control as RadioButton;
                if (radio != null && radio.Checked)
                    return radio.Tag.ToString();
            }
            return "";
        }
    }
}
