using MeteoViewer.Data;
using MeteoViewer.JSONparser;
using Newtonsoft.Json.Linq;
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

namespace MeteoViewer
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRootAsync();
            LoadDataAsync();
        }

        private async void LoadRootAsync()
        {
            Stream.JRoot = await JSONreader.LoadJsonRoot();
        }

        private async void LoadDataAsync()
        {
            Stream.JData = await JSONreader.LoadJson("03h_200225_205750");
        }
    }
}
