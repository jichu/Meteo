using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MeteoViewerSmery.Data
{
    internal static class Resources
    {
        private static string PathImages { get; set; } = "images";
        private static string PathSymbols { get; set; } = "images/symbols";
        private static string map_output_background { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "map_output_background.png");
        private static string mask { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "mask.bmp");
        internal static Bitmap BitmapMapOutputBackground { get; private set; }
        internal static Bitmap BitmapMapMaskORP { get; private set; }
        
        internal static bool Load()
        {
            if (Check())
            {
                BitmapMapOutputBackground = new Bitmap(map_output_background);
                BitmapMapMaskORP = new Bitmap(mask);
                return true;
            }
            MessageBox.Show("Chybí obrázkové zdroje!");
            return false;
        }
        internal static Bitmap LoadSymbol(string name)
        {
            string path = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathSymbols, name+".png");
            if (File.Exists(path))
                return new Bitmap(path);
            return null;
        }
        private static bool Check()
        {
            if (File.Exists(map_output_background)&&
                File.Exists(mask))
                return true;
            Utils.Log.Error(new FileNotFoundException());
            return false;
        }

    }
}
