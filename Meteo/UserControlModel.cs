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
            ShowModels();
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
                Thread t = new Thread(() => LoadMap(e.Node.Name));
                t.Start();
            }
        }

        private void LoadMap(string map)
        {
            Util.ShowLoading("Načítání mapy...");
            BeginInvoke(new MethodInvoker(delegate
            {
                pictureBoxMap.Image = (Bitmap)Image.FromFile(map);
            }));
        }
    }
}
