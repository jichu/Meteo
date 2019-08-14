using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WRFdll
{
    public static class WRF
    {
        internal static Dictionary<string, string> pathSource = new Dictionary<string, string>
        {
            { "source", @".\img1\test.png" },
            { "mask", @".\img1\mask.png" },
            { "mask_orp", @".\img1\Model_WRF_NMM_FLYMET.bmp" }
        };

        internal static Bitmap MapSource;
        internal static Bitmap MapMask;
        internal static Bitmap MapMaskORP;
        internal static Point Size { get; set; }
        internal static bool Ready { get; set; }

        internal static PictureBox CreatePicture()
        {
            Bitmap bmp = LoadImage(pathSource["source"]);
            Bitmap mask = LoadImage(pathSource["mask"]);
            Bitmap maskorp = LoadImage(pathSource["mask_orp"]);
            PictureBox pb = new PictureBox();
            if (bmp != null && mask!=null && maskorp!=null) {
                pb.Image = bmp;
                pb.Width = bmp.Width;
                pb.Height = bmp.Height;
                MapSource = bmp;
                Size = new Point(bmp.Width,bmp.Height);
                MapMask = mask;
                MapMaskORP = maskorp;
                Ready = true;
            }
            return pb;
        }

        public static void Process(Dictionary<string, string> dic)
        {
            pathSource = dic;
            PictureBox pb = CreatePicture();
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
