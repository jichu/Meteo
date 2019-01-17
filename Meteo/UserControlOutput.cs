using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Meteo
{
    public partial class UserControlOutput : UserControl
    {
        private static UserControlOutput uc;
        private PictureBox canvas;
        private List<string> settingOutputVarriableNames = new List<string>();

        public static UserControlOutput Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlOutput();
                return uc;
            }
        }

        public UserControlOutput()
        {
            InitializeComponent();
            CreateCanvas();
            CreateTable();
            Render();
            //Util.SetSettings("pokus", "kk");
        }

        private Color GetOutputColor(int v)
        {
            string colorStr = Util.GetSettings($"output_result{v}_color");
            if (colorStr ==null)
                return Color.White;
            return ColorTranslator.FromHtml(colorStr);
        }

        private void CreateCanvas()
        {
            PictureBox pb = new PictureBox();
            pb.Width = 600;
            pb.Height = 370;
            pb.BackColor = Color.Khaki;
            pb.Name = "canvas";
            this.Controls.Add(pb);
            canvas = (this.Controls["canvas"] as PictureBox);
            canvas.Image = new Bitmap(canvas.Width, canvas.Height);
            canvas.Paint += Canvas_Paint;
        }

        private void CreateTable()
        {
            dgv.Columns.Add("Id", "");
            dgv.Columns["Id"].Width = 0;
            dgv.Columns.Add("Color", "");
            dgv.Columns["Color"].Width = 15;
            dgv.Columns.Add("Region", "Region");
            dgv.Columns.Add("Value", "Hodnota");
            dgv.Rows.Clear();
            //dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            dgv.ClearSelection();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FormPromt promt = new FormPromt(new PromtData() {
                Title="Barva výstupní hodnoty pro "+ (sender as DataGridView).Rows[e.RowIndex].Cells["Value"].Value,
                Value= Color.FromName((sender as DataGridView).Rows[e.RowIndex].Cells["Color"].Style.BackColor.Name).ToArgb().ToString(),
                OK="Uložit",
                Width=200
            });
            promt.ShowDialog();
            Util.l(promt.Value);
        }

        private void AddTable(DataOutput data)
        {
            DataGridViewRow row;
            DataGridViewCell CellByName(string columnName)
            {
                var column = dgv.Columns[columnName];
                if (column == null)
                    throw new InvalidOperationException("Unknown column name: " + columnName);
                return row.Cells[column.Index];
            }
            row = new DataGridViewRow();
            row.CreateCells(dgv);
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.SelectionBackColor = Color.Purple;
            cellStyle.SelectionForeColor = Color.White;
            CellByName("Color").Style.BackColor = data.Color;
            CellByName("Region").Value = data.RegionName;
            CellByName("Value").Value = data.Value;
            row.ReadOnly = true;
            dgv.Rows.Add(row);

        }

        internal void Render()
        {
            Draw(new Dictionary<string, float>()
            {
                { "Aš", 0 },
                { "Tachov",1 },
                { "Kraslice",2 },
                { "Ostrov",3}
            });
            DataGridViewSelectionMode oldmode = dgv.SelectionMode;
            canvas.Invalidate();
        }

        private void Draw(Dictionary<string, float> output)
        {
            foreach (var ll in output)
            {
                AddTable(new DataOutput()
                {
                    Color = GetOutputColor((int)ll.Value),
                    RegionName = ll.Key,
                    Value = ll.Value
                });
                DrawRegion(Model.Cloud.ORPNameToColor(ll.Key), GetOutputColor((int)ll.Value));
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DrawRegion(string hex, Color value)
        {
            Util.ShowLoading("Vykreslování");
            List<CloudMaskSpectrum> cms = Model.Cloud.MaskSpectrumGetCoodsByColor(hex, "Model_ALADIN_CZ");
            string coods = cms.Count > 0 ? cms.First().coods : "";
            if (coods != "")
            {
                foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                {
                    ((Bitmap)canvas.Image).SetPixel((int)point[0], (int)point[1], value);
                }
            }
        }


    }
}
