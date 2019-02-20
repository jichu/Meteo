using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WRF
{
    public partial class FormTemplate : Form
    {
        private PictureBox pb;
        public FormTemplate(string title, Bitmap picture)
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Text= title;
            this.Width = picture.Width;
            this.Height = picture.Height+20;
            pb = new PictureBox();
            pb.Image = picture;
            pb.Width = picture.Width;
            pb.Height = picture.Height;
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Uložit jako...", new EventHandler(PictureSave_Click));
            pb.ContextMenu = cm;
            this.Controls.Add(pb);
        }

        private void PictureSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                pb.Image.Save(sfd.FileName, format);
            }
        }
    }
}
