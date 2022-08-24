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

        }

        private void Do()
        {
            var wrf = new WRFparser.ApplyWRF();
            wrf.OnCompleted += WRF_completed;
        }

        private void WRF_completed(object sender, EventArgs e)
        {
            Console.WriteLine("Hotovo WRF");
            Console.WriteLine((sender as WRFparser.ApplyWRF).Outputs);
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
