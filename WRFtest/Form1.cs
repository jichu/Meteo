using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WRFdll;

namespace WRFtest
{
    public partial class Form1 : Form
    {
        public Dictionary<string, string> pathSource = new Dictionary<string, string>
        {
            { "masks", @".\config\masks\" },
            { "wrf_mask", @".\images\wrf_mask.png" },
        };
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            List<Task<int>> tasks = new List<Task<int>>();
            try
            {
                int i = 0;
                    tasks.Add(Task.Run(() => Run()));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            };
        }

        private int Run()
        {
            WRF.Init(new Dictionary<string, string>{
                 { "mask", pathSource["wrf_mask"] },
                 { "mask_orp", pathSource["masks"]+"Model_WRF_NMM_FLYMET.bmp" }
                });



            WRF.Process(new Dictionary<string, string>{
                 { "source", @".\models\18.png" }
                });
            return 1;
        }
    }
}
