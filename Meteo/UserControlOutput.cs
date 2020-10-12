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
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing.Imaging;

namespace Meteo
{
    public partial class UserControlOutput : UserControl
    {
        private static UserControlOutput uc;
        private PictureBox canvas;
        private PictureBox legend;
        private ComboBox comboAlgorithmOutput;
        private TrackBar trackBarHourMove;
        private Label labelHourMove;
        private int stepHour=3;
        private int curHour;
        private int minTrackBar;
        private int maxTrackBar;
        private List<string> settingOutputVarriableNames = new List<string>();
        private string output_text_value_blank;
        private string usedMask = "Model_ALADIN_CZ";
        private Bitmap mask;
        List<CloudSamples> listSamples;
        private Dictionary<string, float> output;

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
            CreateTable();
            CreateComboAlgorithmOutput();
            CreateTrackBarHourMove();
            Render();
        }
        
        private void CreateComboAlgorithmOutput()
        {
            ComboBox cb = new ComboBox();
            cb.Name = "comboAlgorithmOutput";
            cb.Width = 350;
            this.Controls.Add(cb);
            comboAlgorithmOutput = (this.Controls["comboAlgorithmOutput"] as ComboBox);
            comboAlgorithmOutput.DisplayMember = "Text";
            comboAlgorithmOutput.ValueMember = "Value";
            foreach (var item in Util.algorithmOutput)
            {
                comboAlgorithmOutput.Items.Add(new {Text = item.Key.ToString(), Value = item.Value });
            }
            comboAlgorithmOutput.SelectedIndex = 0;
            comboAlgorithmOutput.SelectedIndexChanged += new EventHandler(ComboAlgorithmOutput_SelectedIndexChanged);
        }
        private void IntitializeMinMax() {
            listSamples = Model.Cloud.InputDataGetSamples();
            if (listSamples.Count() > 0)
            {
                if (Int32.TryParse(listSamples.First().sample_name, out int first))
                {
                    minTrackBar = first;
                }
                else
                {
                    minTrackBar = 0;
                }

                if (Int32.TryParse(listSamples.Last().sample_name, out int last))
                {
                    maxTrackBar = last;
                }
                else
                {
                    maxTrackBar = 0;
                }
            }
            else
            {
                minTrackBar = 0;
                maxTrackBar = 0;
            }
        }

        private void CreateTrackBarHourMove()
        {
            //Util.l($"Počet samplů: {listSamples.Count()}");
            IntitializeMinMax();
            curHour = minTrackBar;
            int space = 10;
            labelHourMove = new Label();
            labelHourMove.Location = new Point(comboAlgorithmOutput.Width + space, 0);
            labelHourMoveTextRefresh();
            this.Controls.Add(labelHourMove);
            trackBarHourMove = new TrackBar();
            trackBarHourMove.Minimum = minTrackBar / stepHour;
            trackBarHourMove.Maximum = maxTrackBar / stepHour;
            trackBarHourMove.SmallChange = 1;
            trackBarHourMove.LargeChange = 1;
            trackBarHourMove.Location = new Point(comboAlgorithmOutput.Width+labelHourMove.Width+space,0);
            trackBarHourMove.Width = 200;
            trackBarHourMove.Value = curHour/stepHour;
            trackBarHourMove.Scroll += new EventHandler(trackBarHourMove_Scroll);
            trackBarHourMove.MouseUp += new MouseEventHandler(trackBarHour_Render);
            trackBarHourMove.KeyUp += new KeyEventHandler(trackBarHour_Render);
            this.Controls.Add(trackBarHourMove);
        }

        private void RefreshTrackBarHourMove(object sender)
        {
            IntitializeMinMax();
            (sender as TrackBar).Minimum = minTrackBar / stepHour;
            (sender as TrackBar).Maximum = maxTrackBar / stepHour;
            (sender as TrackBar).Value=((sender as TrackBar).Value > (sender as TrackBar).Maximum)? (sender as TrackBar).Maximum: (sender as TrackBar).Value;
            (sender as TrackBar).Value=((sender as TrackBar).Value < (sender as TrackBar).Minimum)? (sender as TrackBar).Minimum: (sender as TrackBar).Value;
        }

        private void labelHourMoveTextRefresh(object sender=null)
        {
            labelHourMove.Text = "Vybrána "+SelectedHour() + ". hodina";
            if(sender!=null) RefreshTrackBarHourMove(sender);
        }

        private void trackBarHourMove_Scroll(object sender, EventArgs e)
        {
            curHour = (sender as TrackBar).Value*stepHour;
            labelHourMoveTextRefresh(sender);
            //Render();
        }
        private void trackBarHour_Render(object sender, EventArgs e)
        {
            curHour = (sender as TrackBar).Value*stepHour;
            labelHourMoveTextRefresh(sender);
            Render();
        }

        private void ComboAlgorithmOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private Color GetOutputColor(int v)
        {
            string colorStr = Util.GetSettings($"output_result{v}_color");
            if (colorStr ==null)
                return Color.White;
            return ColorTranslator.FromHtml(colorStr);
        }

        private bool SetOutputColor(string color, int v)
        {
            if (CheckValidFormatHexColor(color))
            {
                Util.SetSettings($"output_result{v}_color", color);
                return true;
            }
            return false;
        }

        private void CreateCanvas()
        {
            ClearControl("canvas");
            PictureBox pb = new PictureBox();
            pb.Width = 600;
            pb.Height = 370;
            pb.BackColor = Color.White;
            pb.Name = "canvas";
            pb.Location = new Point(0, comboAlgorithmOutput.Height+20);
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Uložit jako...", new EventHandler(PictureSave_Click));
            pb.ContextMenu = cm;
            this.Controls.Add(pb);
            canvas = (this.Controls["canvas"] as PictureBox);
            canvas.Image = new Bitmap(canvas.Width, canvas.Height,PixelFormat.Format32bppPArgb);
            if(File.Exists(Util.pathSource["map_output_background"]))
                canvas.Image = Image.FromFile(Util.pathSource["map_output_background"]);
            canvas.Paint += Canvas_Paint;
            string orpMask = Util.pathSource["masks"] + usedMask + ".bmp";
            if (File.Exists(orpMask))
            {
                mask = (Bitmap)Image.FromFile(orpMask);
            }
            canvas.MouseMove += Canvas_MouseMove;
        }
        
        private void PictureSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                canvas.Image.Save(sfd.FileName, format);
            }
        }

        private void CreateLegend()
        {
            string path = Util.pathSource["output_legend"].Replace("[ID]", comboAlgorithmOutput.SelectedIndex.ToString());
            ClearControl("legend");
            if (File.Exists(path))
            {
                PictureBox pb = new PictureBox();
                pb.Name = "legend";
                Image img = Image.FromFile(path);
                pb.Image = img;
                pb.Width = img.Width;
                pb.Height = img.Height;
                pb.Location = new Point(0,canvas.Height+comboAlgorithmOutput.Height+20);
                this.Controls.Add(pb);
            }
        }

        private void ClearControl(string v)
        {
            foreach (Control item in this.Controls.OfType<Control>())
            {
                if (item.Name == v)
                    this.Controls.Remove(item);
            }

        }

        private void CreateTable()
        {
            output_text_value_blank = Util.GetSettings("output_text_value_-1");
            dgv.Columns.Add("Id", "id");
            dgv.Columns["Id"].Width = 15;
            dgv.Columns.Add("Color", "");
            dgv.Columns["Color"].Width = 20;
            dgv.Columns.Add("Region", "Region");
            dgv.Columns.Add("Value", "Hodnota");
            //dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            dgv.ClearSelection();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FormPromt promt = new FormPromt(new PromtData() {
                Title="Barva pro výstupní hodnoty "+ (sender as DataGridView).Rows[e.RowIndex].Cells["Value"].Value,
                Value= Color2hex((sender as DataGridView).Rows[e.RowIndex].Cells["Color"].Style.BackColor),
                OK="Uložit",
                Width=200
            });
            promt.ShowDialog();
            if(SetOutputColor(promt.Value, int.Parse((sender as DataGridView).Rows[e.RowIndex].Cells["Value"].Value.ToString())))
                Render();
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
            CellByName("Id").Value = dgv.Rows.Count+1;
            CellByName("Id").Style.BackColor = Color.Gray;
            CellByName("Color").Style.BackColor = data.Color;
            CellByName("Region").Value = data.RegionName;
            CellByName("Value").Value = data.Value==-1? output_text_value_blank : data.Value.ToString();
            row.ReadOnly = true;
            dgv.Rows.Add(row);
        }

        internal void Render()
        {
            dgv.Rows.Clear();
            CreateCanvas();
            CreateLegend();
            /*Draw(new Dictionary<string, float>()
            {
                { "Aš", 0 },
                { "Tachov",1 },
                { "Kraslice",2 },
                { "Ostrov",3}
            });*/
            Draw(Model.Cloud.OUTPUTDATAGetDataForSample(SelectedHour(), comboAlgorithmOutput.SelectedIndex));
            //Draw(Model.Cloud.OUTPUTDATAGetDataForSample("18", comboAlgorithmOutput.SelectedIndex));
            canvas.Invalidate();
        }

        private string SelectedHour()
        {
            return curHour < 10 ? "0" + curHour.ToString() : curHour.ToString();
        }

        private void Draw(Dictionary<string, float> output)
        {
            this.output = output;
            int rows = 0;
            int filter = checkBoxTableShow.Checked?-1:1;
            foreach (var ll in output)
            {
                if (ll.Value >=filter)
                {
                    AddTable(new DataOutput()
                    {
                        Color = GetOutputColor((int)ll.Value),
                        RegionName = ll.Key,
                        Value = ll.Value
                    });
                    CsvHelper.CSVexport.AddRow(new CsvHelper.CSVdataOutputTemplate()
                    {
                        NameORP = ll.Key,
                        Value = ll.Value.ToString()
                    });
                    rows++;
                }
                DrawRegion(Model.Cloud.ORPNameToColor(ll.Key), GetOutputColor((int)ll.Value));
            }
            if (rows == 0) dgv.Hide();
            else dgv.Show();

            //CsvHelper.CSVexport.Write(comboAlgorithmOutput.Text.ToString()+"_" + curHour.ToString() + "h"); 
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {

        }

        private string curColorRegion = "";
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (usedMask != null)
            {
                Util.curModelName = usedMask;
                Bitmap img = (Bitmap)(sender as PictureBox).Image;
                float sX = img.Width / (float)(sender as PictureBox).Width;
                float sY = img.Height / (float)(sender as PictureBox).Height;
                string c = "";
                if (e.X < mask.Width && e.Y < mask.Height)
                    c = "#" + mask.GetPixel((int)(e.X), (int)(e.Y)).Name.Substring(2, 6);
                if (!(curColorRegion == c || c == "#ffffff" || c == "#000000"))
                {
                    curColorRegion = c;
                    ShowToolTip(Util.GetRegionNameByColor(curColorRegion));
                }
            }
        }

        private ToolTip tt;
        private int symbolSize =75;
        private int textShift =75;
        private void ShowToolTip(string regionName)
        {
            tt?.Dispose();
            tt = new ToolTip();
            tt.BackColor = Color.LightYellow;
            string value = "";
            foreach (var row in output)
            {
                
                if(row.Key==regionName)
                {
                    value = (row.Value==-1)? output_text_value_blank:row.Value.ToString();
                    break;
                }
            }
            int tag = -1;
            tt.Tag = tag;
            string k = "";
            if (int.TryParse(value, out tag))
            {
                tt.Tag = tag;
                k = "Koeficient: ";
            }
            tt.SetToolTip(canvas, regionName.ToUpper()+Environment.NewLine+k+value);
            tt.OwnerDraw = true;
            tt.Popup += new PopupEventHandler(tooltip_Popup);
            tt.Draw += new DrawToolTipEventHandler(toolTip_Draw);
        }

        private void tooltip_Popup(object sender, PopupEventArgs e)
        {
            Size size = TextRenderer.MeasureText((sender as ToolTip).GetToolTip(e.AssociatedControl), new Font("Arial", 16.0f));
            textShift = (sender as ToolTip).Tag.ToString()=="-1"?0:symbolSize;
            e.ToolTipSize = new Size(size.Width+ textShift, symbolSize);
        }
        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font f = new Font("Arial", 16.0f);
            e.DrawBackground();
            var symbol = Util.pathSymbolsAlgorithmOutput[comboAlgorithmOutput.SelectedIndex].ToString().Replace("[ID]", (sender as ToolTip).Tag.ToString());
            if (File.Exists(symbol))
            {
                e.Graphics.DrawImage(Image.FromFile(symbol), 0, 0);
            }
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.Black, new PointF(textShift, 15));
        }

        private void DrawRegion(string hex, Color value)
        {
            List<CloudMaskSpectrum> cms = Model.Cloud.MaskSpectrumGetCoodsByColor(hex, usedMask);
            string coods = cms.Count > 0 ? cms.First().coods : "";
            if (coods != "")
            {
                foreach (JArray point in JsonConvert.DeserializeObject<JArray>(coods))
                {
                    if (((Bitmap)canvas.Image).GetPixel((int)point[0], (int)point[1]).Name != "ff000000")
                        ((Bitmap)canvas.Image).SetPixel((int)point[0], (int)point[1], value);
                }
            }
        }

        private bool CheckValidFormatHexColor(string inputColor)
        {

            if (Regex.Match(inputColor, "^#(?:[0-9a-fA-F]{3}){1,2}$").Success)
                return true;
            return false;
            /*
            var result = System.Drawing.Color.FromName(inputColor);
            return result.IsKnownColor;
            */
        }

        private String Color2hex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void checkBoxTableShow_CheckedChanged(object sender, EventArgs e)
        {
            Render();
        }
    }
}
