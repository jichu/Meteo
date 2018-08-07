using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Meteo
{
    public partial class UserControlPaletteForMask : UserControl
    {
        private static UserControlPaletteForMask uc;

        int rgbSwitch = 0;
        int colorIntense = 255;
        Bitmap bmp;

        public static UserControlPaletteForMask Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlPaletteForMask();
                return uc;
            }
        }

        public UserControlPaletteForMask()
        {
            InitializeComponent();
            CreatePalette();
        }

        private void CreatePalette()
        {
            int boxSize = 20;
            palette.Width = 6*boxSize;
            palette.Height = 700;
            bmp = new Bitmap(palette.Width, palette.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                richTextBoxOutput.Clear();
                int count = 0;
                for (int x = 0; x < 6; x++)
                    for (int y = 0; y < 35; y++)
                    {
                        if (count > 204) break;
                        Brush brush = GetColor(colorIntense);
                        g.FillRectangle(brush, x * boxSize, y * boxSize, boxSize, boxSize);
                        g.DrawString(count.ToString(), new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular),
                                    new SolidBrush(Color.White), x * boxSize, y * boxSize);
                        Pen pen = new Pen(brush);

                        richTextBoxOutput.Text += $"{count}\t{pen.Color.Name}{Environment.NewLine}";
                        count++;
                    }

                Util.l(count);
            }
            palette.Image = bmp;
        }

        private Brush GetColor(int value)
        {
            Brush brush=null;
            if(rgbSwitch==0) brush = new SolidBrush(Color.FromArgb(255, value, 0, 0));
            if (rgbSwitch == 1) brush = new SolidBrush(Color.FromArgb(255, 0, value, 0));
            if (rgbSwitch == 2) brush = new SolidBrush(Color.FromArgb(255, 0, 0, value));
            if (rgbSwitch == 3) brush = new SolidBrush(Color.FromArgb(255, value, 0, value));
            if (rgbSwitch == 4) brush = new SolidBrush(Color.FromArgb(255, 0, value, value));
            if (rgbSwitch == 5) brush = new SolidBrush(Color.FromArgb(255, value, value, 0));
            rgbSwitch++;
            if (rgbSwitch > 5) {
                colorIntense -= 5;
                rgbSwitch = 0;
            }

            return brush;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "paleta.bmp";
            savefile.Filter = "Obrázek (*.bmp)|*.bmp";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                palette.DrawToBitmap(bmp, palette.ClientRectangle);
                bmp.Save(savefile.FileName, ImageFormat.Bmp);
            }
        }
    }
}
