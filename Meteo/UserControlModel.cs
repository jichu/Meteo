using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public partial class UserControlModel : UserControl
    {
        private static UserControlModel uc;

        public static UserControlModel Instance
        {
            get
            {
                if (uc == null)
                    uc = new UserControlModel();
                return uc;
            }
        }

        public UserControlModel()
        {
            InitializeComponent();
        }
    }
}
