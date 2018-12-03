using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Meteo
{
    public partial class UserControlModel : UserControl
    {
        private static UserControlModel uc;
        private string curImage;
        private List<PictureBox> symbols = new List<PictureBox>();
        public static List<SourceImage> sourceImages = new List<SourceImage>();
        private int symbolsRainCount = 0;
        private string curColorRegion="";
        private string map;

        public static UserControlModel Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlModel();
                return uc;
            }
        }

        public UserControlModel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            dgv.Columns.Add("Color", "Maska");
            dgv.Columns["Color"].Width = 10;////
            dgv.Columns.Add("Region", "Region");
            dgv.Columns.Add("Value", "Hodnota");

            Util.curModelName = "Model_ALADIN_CZ";
            Util.curSubmodelName = "Teplota";
            //new Images(@"D:\_PROGRAMOVANI\workspace\csharp\Repos\Meteo\Meteo\bin\Debug\models\Model_ALADIN_CZ\Teplota\15.png",true);

            ShowModels();

            /*
            Thread t = new Thread(() => EnumerationModels());
            t.Start();
            */
            //treeViewModel.ExpandAll();
        }
        
        public void EnumerationModels()
        {
            if (sourceImages.Count > 0)
            {
                foreach (var si in sourceImages)
                {
                    Util.ShowLoading($"Předzpracovávám ...", $"Model: {si.Model} / {si.Submodel} > Data z obrázku {Path.GetFileName(si.Path)}",false);
                    new Images(si, true);
                    Util.HideLoading();
                }
            }

        }

        public void ShowModels()
        {
            string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";
            JObject jOptionTemp = JObject.Parse(@"{'option': {}}");
            sourceImages.Clear();

            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory + @"models";
                int nodeModel = 0;
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                JObject jModels = new JObject();
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    treeViewModel.Nodes.Add(model);

                    string orpMask = dir + @"\" + model + ".bmp";

                    if (!File.Exists(orpMask))
                    {
                        treeViewModel.Nodes[nodeModel].ForeColor = Color.Red;
                        treeViewModel.Nodes[nodeModel].ToolTipText = "V adresáři chybí maska";
                    }
                    List<string> subdirs = new List<string>(Directory.EnumerateDirectories(dirPath+"\\"+model));
                    int nodeSubModel = 0;
                    JObject jSubmodels = new JObject();
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        jSubmodels.Add(submodel, jOptionTemp);
                        treeViewModel.Nodes[nodeModel].Nodes.Add(submodel);
                        var files = Directory.GetFiles(dirPath+"\\"+model+"\\"+submodel, "*.*",SearchOption.TopDirectoryOnly)
                            .Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
                        int nodeFile = 0;
                        foreach (var file in files)
                        {
                            treeViewModel.Nodes[nodeModel].Nodes[nodeSubModel].Nodes.Add(Path.GetFileName(file));
                            treeViewModel.Nodes[nodeModel].Nodes[nodeSubModel].Nodes[nodeFile].Name=file;
                            sourceImages.Add(new SourceImage()
                            {
                                Path = file,
                                Model = model,
                                Submodel=submodel
                            });
                            nodeFile++;
                        }
                        nodeSubModel++;
                    }
                    nodeModel++;
                    jModels.Add(model, jSubmodels);
                }
                if(Directory.Exists(Util.pathSource["config"]))
                    File.WriteAllText(Util.pathSource["config"]+Util.pathSource["model_cfg"]+".tmp", JsonConvert.SerializeObject(jModels));
                this.treeViewModel.AfterSelect += new TreeViewEventHandler(treeViewModel_AfterSelect);
            }
            catch (Exception e)
            {
                Util.l(e);
            }
        }

        private void treeViewModel_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                Util.curModelName = e.Node.Parent.Parent.Text;
                Util.curSubmodelName = e.Node.Parent.Text;
                Thread t = new Thread(() => LoadMap(e.Node.Name));
                t.Start();
            }
        }

        private void LoadMap(string map)
        {
            Util.ShowLoading("Načítání mapy...");
            curImage = map;
            BeginInvoke(new MethodInvoker(delegate
            {
                pictureBoxMap.Image = (Bitmap)Image.FromFile(map);
            }));
            new Images(map);
            ResizeMap(pictureBoxMap);
        }

        private void ResizeMap(PictureBox pb)
        {
            if (pb != null)
            {
                float ratio = (float)pb.Image.Height / (float)pb.Image.Width;
                int size = this.ClientSize.Width - (treeViewModel.Width + (dgv.Visible ? dgv.Width : 0) + 15);
                BeginInvoke(new MethodInvoker(delegate
                {
                    pb.Width = size;
                    pb.Height = (int)((float)size * ratio);
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                }));
            }
        }

        private void checkBoxShowORP_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                try
                {
                    string orpMask = Util.pathSource["models"] + Util.curModelName+@"\"+Util.curModelName+".bmp";
                    if (File.Exists(orpMask))
                    {
                        Util.ShowLoading("Načítání masky...");
                        PictureBox symbol = new PictureBox();
                        Bitmap bmp1 = (Bitmap)Image.FromFile(orpMask);
                        symbol.Width = bmp1.Width;
                        symbol.Height = bmp1.Height;
                        symbol.Name = "pictureBoxMask";
                        symbol.BackColor = Color.Transparent;
                        symbol.Image = (Image)bmp1;
                        symbol.SizeMode = PictureBoxSizeMode.StretchImage;
                        this.Controls.Add(symbol);
                        this.Controls["pictureBoxMask"].Location = new Point(pictureBoxMap.Location.X, pictureBoxMap.Location.Y);
                        this.Controls["pictureBoxMask"].BringToFront();
                        this.Controls["pictureBoxMask"].MouseMove += new MouseEventHandler(this.pictureBoxMask_MouseMove);
                        ResizeMap(this.Controls["pictureBoxMask"] as PictureBox);
                    }
                    else Util.l($"V adresáři chybí maska {orpMask}.|Chybí maska");
                }
                catch (Exception ex)
                {
                    Util.l(ex);
                }
            }
            else
            {
                this.Controls.Remove(this.Controls["pictureBoxMask"]);
                labelRegionName.Text = "";
            }
        }

        private void pictureBoxMask_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap img = (Bitmap)(sender as PictureBox).Image;
            float sX = img.Width / (float)(sender as PictureBox).Width;
            float sY = img.Height / (float)(sender as PictureBox).Height;
            string c = "#"+img.GetPixel((int)(e.X*sX), (int)(e.Y*sY)).Name.Substring(2,6);
            if (!(curColorRegion == c||c=="#ffffff"||c=="#000000"))
            {
                curColorRegion = c;
                labelRegionName.Text="Název regionu: "+(Util.GetRegionNameByColor(curColorRegion));
            }
            if (c == "#ffffff" || c == "#000000")
                labelRegionName.Text = "";
        }

        private void checkBoxShoweRain_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                LoadSymbols();
            }
            else
            {
                if(symbolsRainCount>0)
                    for (int i = 0; i < symbolsRainCount; i++)
                    {
                        this.Controls.Remove(this.Controls["rain" + i]);
                    }
            }
        }

        private void pictureBoxSymbol_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.BackColor = Color.Yellow;
            //tt.Tag = (sender as PictureBox).Tag.ToString();
            tt.SetToolTip((sender as PictureBox), (sender as PictureBox).Tag.ToString());
            tt.OwnerDraw = true;
            tt.Popup += new PopupEventHandler(tooltip_Popup);
            tt.Draw += new DrawToolTipEventHandler(toolTip_Draw);
            //labelRegionName.Text = "Název regionu: " + (sender as PictureBox).Tag.ToString();
        }

        private void tooltip_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText((sender as ToolTip).GetToolTip(e.AssociatedControl), new Font("Arial", 18.0f));
        }
        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font f = new Font("Arial", 18.0f);
            e.DrawBackground();
            //e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.Black, new PointF(2, 2));
        }

        private void LoadSymbols()
        {
            try
            {
                int size = 24;
                symbolsRainCount = 0;
                Util.ShowLoading("Načítání symbolů...");
                Bitmap symbols;
                foreach (var point in Util.rainRegion)
                {
                    string path = "";
                    float value = Util.rainRegionValue[symbolsRainCount];
                    if(value>=1&&value<2)
                        path = Util.pathSource["symbol_cloud"];
                    else if (value >= 2 && value < 3)
                        path = Util.pathSource["symbol_rain"];
                    else if (value >= 3)
                        path = Util.pathSource["symbol_storm"];
                    if (path == "")
                        continue;
                    if (File.Exists(path))
                    {
                        PictureBox symbol = new PictureBox();
                        symbol.Width = size;
                        symbol.Height = size;
                        symbol.Name = "rain" + symbolsRainCount;
                        symbol.Tag = point.Key;
                        symbol.BackColor = Color.Transparent;
                        symbol.BackgroundImage = Image.FromFile(path);
                        symbol.BackgroundImageLayout = ImageLayout.Stretch;
                        this.Controls.Add(symbol);
                        Bitmap img = (Bitmap)pictureBoxMap.Image;
                        float sX = img.Width / (float)pictureBoxMap.Width;
                        float sY = img.Height / (float)pictureBoxMap.Height;
                        this.Controls["rain" + symbolsRainCount].Location = new Point((pictureBoxMap.Location.X + point.Value.X) - size / 2, (pictureBoxMap.Location.Y + (int)(point.Value.Y*sY)) - size / 2);
                        this.Controls["rain" + symbolsRainCount].BackColor = Color.Transparent;
                        this.Controls["rain" + symbolsRainCount].BringToFront();
                        this.Controls["rain" + symbolsRainCount].MouseHover += new EventHandler(this.pictureBoxSymbol_MouseHover);
                    }
                    else
                    {
                        Util.l($"Program nemůže načíst obrázek {path}.|Chybí symbol srážek");
                    }
                    symbolsRainCount++;
                }
            }
            catch (Exception ex)
            {
                Util.l(ex);
            }
        }

        private void checkBoxShowOutput_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                richTextBoxOutput.Visible = true;
                richTextBoxOutput.Text = Util.curModelOutput;
                dgv.Visible = true;
                DataGridViewRow row;
                DataGridViewCell CellByName(string columnName)
                {
                    var column = dgv.Columns[columnName];
                    if (column == null)
                        throw new InvalidOperationException("Unknown column name: " + columnName);
                    return row.Cells[column.Index];
                }
                dgv.Rows.Clear(); 
                foreach (DataOutput data in Util.curDataOutputs)
                {
                    row = new DataGridViewRow();
                    row.CreateCells(dgv);
                    CellByName("Color").Style.BackColor = data.Color;
                    CellByName("Region").Value = data.RegionName;
                    CellByName("Value").Value = data.Value;
                    dgv.Rows.Add(row);    
                }
                dgv.BringToFront();
                ResizeMap(pictureBoxMap);
                ResizeMap(this.Controls["pictureBoxMask"] as PictureBox);
            }
            else
            {
                dgv.Visible = false;
                richTextBoxOutput.Visible = false;
                ResizeMap(pictureBoxMap);
                ResizeMap(this.Controls["pictureBoxMask"] as PictureBox);
            }
        }

        private void labelCountMethod_DoubleClick(object sender, EventArgs e)
        {
            string options = Model.Cloud.MODELSGetModelOptions(Util.curModelName, Util.curSubmodelName);
            FormSetOptions f = new FormSetOptions(Util.curModelName, Util.curSubmodelName, options);
            f.ShowDialog(View.FormMain);
            options = f.options;
            Util.l(options);
            Model.Cloud.MODELSInsertOrUpdate(new CloudModels(Util.curSubmodelName, Util.curModelName, options));
            Thread t = new Thread(() => LoadMap(curImage));
            t.Start();
        }

    }
}
