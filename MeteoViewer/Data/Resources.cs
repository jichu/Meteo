using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MeteoViewer.Data
{
    internal static class Resources
    {
        private static string PathImages { get; set; } = "images";
        private static string map_output_background { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "map_output_background.png");
        private static string Model_ALADIN_CZ { get; set; } = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathImages, "Model_ALADIN_CZ.bmp");
        internal static BitmapImage MapOutputBackground { get; private set; }
        internal static BitmapImage MapMaskORP { get; private set; }
        
        internal static bool Load()
        {
            if (Check())
            {
                MapOutputBackground = new BitmapImage(new Uri(map_output_background, UriKind.RelativeOrAbsolute));
                MapMaskORP = new BitmapImage(new Uri(Model_ALADIN_CZ, UriKind.RelativeOrAbsolute));
                return true;
            }
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
    }
}
