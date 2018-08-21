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
        private int symbolsRainCount = 0;
        private string curColorRegion="";

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

            ShowModels();
            //treeViewModel.ExpandAll();
        }

        public void ShowModels()
        {
            string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";

            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory + @"models";
                int nodeModel = 0;
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
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
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        treeViewModel.Nodes[nodeModel].Nodes.Add(submodel);
                        var files = Directory.GetFiles(dirPath+"\\"+model+"\\"+submodel, "*.*",SearchOption.TopDirectoryOnly)
                            .Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
                        int nodeFile = 0;
                        foreach (var file in files)
                        {
                            treeViewModel.Nodes[nodeModel].Nodes[nodeSubModel].Nodes.Add(Path.GetFileName(file));
                            treeViewModel.Nodes[nodeModel].Nodes[nodeSubModel].Nodes[nodeFile].Name=file;
                            nodeFile++;
                        }
                        nodeSubModel++;
                    }
                    nodeModel++;
                }
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
                        symbol.BackgroundImage = (Image)bmp1;
                        symbol.BackgroundImageLayout = ImageLayout.Stretch;
                        this.Controls.Add(symbol);
                        this.Controls["pictureBoxMask"].Location = new Point(pictureBoxMap.Location.X, pictureBoxMap.Location.Y);
                        this.Controls["pictureBoxMask"].BringToFront();
                        this.Controls["pictureBoxMask"].MouseMove+=new MouseEventHandler(this.pictureBoxMask_MouseMove);
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
            Bitmap b = new Bitmap((sender as PictureBox).BackgroundImage);
            string c = "#"+b.GetPixel(e.X, e.Y).Name.Substring(2,6);
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
                string path = Util.pathSource["symbol_rain"];
                if (File.Exists(path))
                {
                    Util.ShowLoading("Načítání symbolů...");
                    foreach (var point in Util.rainRegion)
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
                        this.Controls["rain" + symbolsRainCount].Location = new Point((pictureBoxMap.Location.X + point.Value.X) - size / 2, (pictureBoxMap.Location.Y + point.Value.Y) - size / 2);
                        this.Controls["rain" + symbolsRainCount].BackColor = Color.Transparent;
                        this.Controls["rain" + symbolsRainCount].BringToFront();
                        this.Controls["rain" + symbolsRainCount].MouseHover += new EventHandler(this.pictureBoxSymbol_MouseHover);
                        symbolsRainCount++;
                    }
                }
                else
                {
                    Util.l($"Program nemůže načíst obrázek {path}.|Chybí symbol srážek");
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
            }
            else
            {
                richTextBoxOutput.Visible = false;
            }
        }

    }
}
