using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            if (Data.Resources.Load())
            {
                new MaskORP();
                CreateCanvas();
                Debug.WriteLine(Data.Stream.JRoot);
                Debug.WriteLine(JsonConvert.SerializeObject(Data.Region.GetCoodsByRegionName("Aš")));
            }
        }
        private void CreateCanvas()
        {
            BitmapImage bg = Data.Resources.MapOutputBackground;
            Canvas.Source = bg;
            Canvas.Width = bg.Width;
            Canvas.Height = bg.Height;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }
    }
}
