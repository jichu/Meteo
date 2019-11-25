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

            WRF.Init(new Dictionary<string, string>{
                 { "mask", pathSource["wrf_mask"] },
                 { "mask_orp", pathSource["masks"]+"Model_WRF_NMM_FLYMET.bmp" }
                });


            List<string> paths = new List<string>()
            {
                 @".\models\03.png", //@".\models\06.png"
            };

            List<Task> tasks = new List<Task>();
                foreach (string si in paths)
            {
                EnumerationModel(si);
               // tasks.Add(Task.Run(() => EnumerationModel(si)));
                }
            
            //Task.WaitAll(tasks.ToArray());
        }

        private void EnumerationModel(string p)
        {
            WRF.Process(new Dictionary<string, string>{
                 { "source", p }
                });
        }
    }
}
