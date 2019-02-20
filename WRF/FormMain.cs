using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WRF
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controls.Add(Map.CreatePicture());
        }

        private void SetWinForm()
        {
            this.Width = Map.Size.X;
            this.Height = Map.Size.Y;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (Map.Ready)
            {
                SetWinForm();
                new FormTemplate("mask", Map.MapMask).Show();
                Map.Process();
            }
        }
    }
}
