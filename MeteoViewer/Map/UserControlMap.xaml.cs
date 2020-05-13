using MeteoViewer.JSONparser;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace MeteoViewer.Map
{
    /// <summary>
    /// Interakční logika pro UserControlMap.xaml
    /// </summary>
    public partial class UserControlMap : UserControl
    {
        public static UserControlMap Instance { get; set; }

        internal Bitmap MapOutput { get; set; }

        public UserControlMap()
        {
            InitializeComponent();
        }
        
        internal void Open(string name)
        {
            LoadRootAsync();
            LoadDataAsync(name);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;

            LoadRootAsync();
            if (Data.Resources.Load())
            {
                new MaskORP();
                CreateCanvas();
                Task.Run(() => RenderLoop());
            }
        }


        private async void LoadRootAsync()
        {
            Data.Stream.JRoot = await JSONreader.LoadJsonRoot();
        }

        private async void LoadDataAsync(string name)
        {
            Data.Stream.JData = await JSONreader.LoadJson(name);
        }

        private void RenderLoop()
        {
            while(true)
            {
                Render();
                Thread.Sleep(250);
            }
        }

        private void CreateCanvas()
        {
            BitmapImage bg = Data.Resources.MapOutputBackground;
            MapOutput = Data.Resources.BitmapMapOutputBackground;
            Canvas.Source = bg;
            Canvas.Width = MapOutput.Width;
            Canvas.Height = MapOutput.Height;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
        }

        internal void Render()
        {
            if (Data.Stream.JRoot != null && Data.Cache.Redraw)
            {
                RerfreshComboOutputList();
                RefreshSliderHour();
                RefreshDrawMap();
                Data.Cache.Redraw = false;
            }
        }

        private void RerfreshComboOutputList()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                ComboOutputList.Items.Clear();
                foreach (string item in Data.Stream.GetJRoot("outputlist"))
                    ComboOutputList.Items.Add(item);
                if (ComboOutputList.Items.Count > 0)
                    ComboOutputList.SelectedIndex = 0;
            }));
        }

        private void RefreshSliderHour()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                if (Data.Stream.GetJData("samplename").Count > 0)
                {
                    SliderHour.Maximum = Data.Stream.GetJData("samplename").Count - 1;
                    LabelHour.Content = $"Hodina {Data.Stream.GetJData("samplename")[0]}:";
                }
            }));
        }

        internal void RefreshDrawMap()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                JArray values = Data.Stream.GetJDataValues();
                if(values.Count!= Data.Stream.GetJRoot("orplist").Count)
                    return;
                int i = 0;
                Data.Region.CurrentORP.Clear();
                foreach (string name in Data.Stream.GetJRoot("orplist"))
                {
                    DrawRegion(Data.Region.GetCoodsByRegionName(name), (int)values[i]);
                    Data.Region.CurrentORP.Add(name, (int)values[i]);
                    i++;
                }
                Canvas.Source = ImageSourceFromBitmap(MapOutput);
                Table.UserControlTableView.Instance.Refresh();
            }));
        }

        internal void Refresh()
        {
            Data.Cache.Redraw = true;
        }

        private void DrawRegion(JArray coods, int value)
        {
            foreach (JArray point in coods)
                MapOutput.SetPixel((int)point[0], (int)point[1], GetOutputColor(value));
        }

        private System.Drawing.Color GetOutputColor(int v)
        {
            JArray colors = Data.Stream.GetJRoot("outputresultcolor");
            string colorStr = colors.Count>v?colors[v+1].ToString():null;
            if (colorStr == null)
                return System.Drawing.Color.White;
            return ColorTranslator.FromHtml(colorStr);
        }
        
        private void ComboOutputList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.Cache.indexOutputlist = (sender as ComboBox).SelectedIndex;
            RefreshDrawMap();
        }

        private void SliderHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = (int)(sender as Slider).Value;
                Debug.WriteLine((sender as Slider).Value);
                LabelHour.Content = $"Hodina {Data.Stream.GetJData("samplename")[val]}:";
                Table.UserControlTableView.Instance.MenuItemExport.Header = $"Exportovat {Data.Stream.GetJData("samplename")[val]}h do Excelu...";
                Data.Cache.indexHour = val;
                RefreshDrawMap();
            }
            catch 
            {

            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = new Point
            {
                X = e.GetPosition(Canvas).X,
                Y = e.GetPosition(Canvas).Y
            };
            if (!TooltipRegion.IsOpen) 
                TooltipRegion.IsOpen = true; 
            TooltipRegion.HorizontalOffset = position.X;
            TooltipRegion.VerticalOffset = position.Y+20;
            string name = Data.Region.GetRegionNameByCoods(position);
            if(name==string.Empty)
                TooltipRegion.IsOpen = false;
            if (LabelRegion.Content.ToString() != name)
            {
                int val = -1;
                if (!Data.Region.CurrentORP.TryGetValue(name, out val)) val = -1;
                string koef = val == -1 ? "" : $"\nkoeficient: {val}";
                LabelRegion.Content = $"{name}{koef}";
                TooltipImage.Source = Data.Resources.LoadSymbol("riziko tornád 3");
                //Console.WriteLine($"{ComboOutputList.SelectedItem} {val}");
                //TooltipImage.Source = Data.Resources.LoadSymbol($"{ComboOutputList.SelectedItem} {val}");
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            TooltipRegion.IsOpen = false;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Images files (*.png)|*.png";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Exportovat výstup do Excelu";

            Nullable<bool> result = saveFileDialog.ShowDialog();

            if (result == true)
                SaveToImageFile(saveFileDialog.FileName);
        }

        private void SaveToImageFile(string fileName)
        {
            try
            {
                MapOutput.Save(fileName, ImageFormat.Png);
            }
            catch (Exception e)
            {
                Utils.Log.Error(e);
            }
        }
    }
}
