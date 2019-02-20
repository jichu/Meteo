using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WRF
{
    internal static class Map
    {
        private static Dictionary<string, string> pathSource = new Dictionary<string, string>
        {
            { "source", @".\img\test.png" },
            { "mask", @".\img\mask.png" }
        };

        public static Bitmap MapSource;
        public static Bitmap MapMask;
        public static Point Size { get; set; }
        public static bool Ready { get; set; }

        public static PictureBox CreatePicture()
        {
            Bitmap bmp = LoadImage(pathSource["source"]);
            Bitmap mask = LoadImage(pathSource["mask"]);
            PictureBox pb = new PictureBox();
            if (bmp != null && mask!=null) {
                pb.Image = bmp;
                pb.Width = bmp.Width;
                pb.Height = bmp.Height;
                MapSource = bmp;
                Size = new Point(bmp.Width,bmp.Height);
                MapMask = mask;
                Ready = true;
            }
            return pb;
        }

        internal static void Process()
        {
            MagicWandForF0ckWRFmodel magic = new MagicWandForF0ckWRFmodel();
        }

        public static void ShowPicture()
        {

        }

        private static Bitmap LoadImage(string path)
        {
            try
            {
                Bitmap bmp1 = (Bitmap)Image.FromFile(path);
                return new Bitmap(bmp1, new Size(bmp1.Width, bmp1.Height));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

    }


}
