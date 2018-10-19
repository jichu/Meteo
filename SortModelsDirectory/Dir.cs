using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortModelsDirectory
{
    class Dir
    {

        string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string dirPath = AppDomain.CurrentDomain.BaseDirectory + @"models";
        string dirPathSource = AppDomain.CurrentDomain.BaseDirectory + @"source";
        int copied = 0;

        public Dir(Form1 form)
        {
            dirPath = AppDomain.CurrentDomain.BaseDirectory + form.textBoxTarget.Text;
            dirPathSource = AppDomain.CurrentDomain.BaseDirectory + form.textBoxSource.Text;
            form.buttonRunSearch.Enabled = false;
            string textButton = form.buttonRunSearch.Text;
            Log.clear();
            form.buttonRunSearch.Text = "zpracovávám...";

            var thread = new Thread(
              () =>
              {
                  try
                  {
                      RunSearch();
                  }
                  finally
                  {
                      Log.add("===hotovo===");
                      Log.add($"zkopirováno souborů: {copied}");
                      form.buttonRunSearch.BeginInvoke((Action)(() =>
                      {
                          form.buttonRunSearch.Enabled = true;
                          form.buttonRunSearch.Text = textButton;
                      }));
                  }
              });
            thread.Start();
        }

        private void RunSearch()
        {
            try
            {
                int nodeModel = 0;
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                foreach (var dir in dirs)
                {
                    string model = dir.Substring(dir.LastIndexOf("\\") + 1);
                    List<string> subdirs = new List<string>(Directory.EnumerateDirectories(dirPath + "\\" + model));
                    foreach (var subdir in subdirs)
                    {
                        string submodel = subdir.Substring(subdir.LastIndexOf("\\") + 1);
                        Log.add($"Hledám {model}/{submodel} :");
                        DirSearch(dirPathSource, model, submodel, subdir.Replace(baseDir,""));
                    }
                    nodeModel++;
                }
            }
            catch (Exception e)
            {
                Log.add(e.Message);
            }

        }

        private void DirSearch(string sDir, string model, string submodel, string target)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    var dir = new DirectoryInfo(d);
                    if (dir.Name == submodel)
                    {
                        Log.add($"\tnalezeno {dir.FullName.Replace(baseDir,"")}");
                        FilesSearch(dir.FullName, target);
                        continue;
                    }
                    DirSearch(d,model,submodel,target);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void FilesSearch(string sDir,string target)
        {
            try
            {
                var files = Directory.GetFiles(sDir, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                    Log.add($"\t\tkopíruji {Path.GetFileName(file)} do {target}");
                    System.IO.File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
                    copied++;
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    FilesSearch(d,target);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

    }
}
