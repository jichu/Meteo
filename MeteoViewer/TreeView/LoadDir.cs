using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoViewer.TreeView
{
    internal class LoadDir
    {
        internal FileInfo[] Files = null;
        public LoadDir()
        {
            //Task.Run(() => ScanDir());
            ScanDir();
        }

        private void ScanDir()
        {
            DirectoryInfo di = new DirectoryInfo(Data.Cache.PathJson);
            try
            {
                Files = di.GetFiles("*.json", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException e)
            {
                Utils.Log.Error(e);
            }
            catch(Exception e)
            {
                Utils.Log.Error(e);
            }
        }
    }
}
