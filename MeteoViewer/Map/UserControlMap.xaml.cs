using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeteoViewer.Map
{
    /// <summary>
    /// Interakční logika pro UserControlMap.xaml
    /// </summary>
    public partial class UserControlMap : UserControl
    {
        public UserControlMap()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(1);
        }
    }
}
