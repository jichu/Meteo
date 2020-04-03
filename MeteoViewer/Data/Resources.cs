using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MeteoViewer.Data
{
    internal static class Resources
    {
        private static string PathImages { get; set; } = "images";
        private static string PathSymbols { get; set; } = "images/symbols";
        private static string map_output_background { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "map_output_background.png");
        private static string Model_ALADIN_CZ { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "Model_ALADIN_CZ.bmp");
        internal static BitmapImage MapOutputBackground { get; private set; }
        internal static BitmapImage MapMaskORP { get; private set; }
        internal static Bitmap BitmapMapOutputBackground { get; private set; }
        internal static Bitmap BitmapMapMaskORP { get; private set; }
        
        internal static bool Load()
        {
            if (Check())
            {
                MapOutputBackground = new BitmapImage(new Uri(map_output_background, UriKind.RelativeOrAbsolute));
                MapMaskORP = new BitmapImage(new Uri(Model_ALADIN_CZ, UriKind.RelativeOrAbsolute));
                BitmapMapOutputBackground = BitmapImage2Bitmap(MapOutputBackground);
                BitmapMapMaskORP = BitmapImage2Bitmap(MapMaskORP);
                return true;
            }
            //MessageBox.Show("Chybí obrázkové zdroje!");
            return false;
        }
        internal static bool LoadSymbols()
        {
            return false;
        }
        private static bool Check()
        {
            if (File.Exists(map_output_background)&&
                File.Exists(Model_ALADIN_CZ))
                return true;
            Utils.Log.Error(new FileNotFoundException());
            return false;
        }
        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

    }
}
