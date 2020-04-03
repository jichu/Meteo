using MeteoViewer.JSONparser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Path = System.IO.Path;

namespace MeteoViewer.TreeView
{
    /// <summary>
    /// Interakční logika pro UserControlTree.xaml
    /// </summary>
    public partial class UserControlTree : UserControl
    {
        internal static UserControlTree Instance { get; private set; }

        public UserControlTree()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            //_ = RunPeriodicAsync(OnTick, TimeSpan.FromMilliseconds(500), CancellationToken.None);
            RefreshTreeView();
            MonitoringFileSystem();
        }

        private async void LoadRootAsync()
        {
            Data.Stream.JRoot = await JSONreader.LoadJsonRoot();
        }

        private async void LoadDataAsync(string name)
        {
            Data.Stream.JData = await JSONreader.LoadJson(System.IO.Path.GetFileNameWithoutExtension(name));
        }

        private void MonitoringFileSystem()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            fsw.Path = Data.Cache.PathJson;
            fsw.Filter = "*.json";
            fsw.Changed += new FileSystemEventHandler(Dir_Changed);
            fsw.Deleted += new FileSystemEventHandler(Dir_Changed);
            fsw.EnableRaisingEvents = true;
        }

        private void Dir_Changed(object sender, FileSystemEventArgs e)
        {
            if (Data.Cache.JsonDataFormat.Length <= e.Name.Length)                    
                RefreshTreeView();
        }

        private static async Task RunPeriodicAsync(Action onTick, TimeSpan interval, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                onTick?.Invoke();
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        private void RefreshTreeView()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                TreeJson.Items.Clear();
                var ld = new LoadDir();
                List<string> nodes = new List<string>();
                if (ld.Files == null) return;
                foreach (var file in ld.Files)
                {
                    if (Data.Cache.JsonDataFormat.Length <= file.Name.Length)
                    {
                        if (file.Name.Length >= 6)
                        {
                            string date = $"{file.Name.Substring(4, 2)}. {file.Name.Substring(2, 2)}. 20{file.Name.Substring(0, 2)}";
                            string f = $"{file.Name.Substring(14, 3)} ({file.Name.Substring(7, 2)}:{file.Name.Substring(9, 2)})";
                            if (!nodes.Contains(date))
                            {
                                TreeViewItem nodeDate = new TreeViewItem() { Header = date, IsExpanded=true };
                                TreeViewItem item = new TreeViewItem() { Header = f, ToolTip=file.Name, Tag = file.Name };
                                nodeDate.Items.Add(item);
                                TreeJson.Items.Add(nodeDate);
                                nodes.Add(date);
                            }
                            else
                            {
                                foreach (TreeViewItem node in TreeJson.Items) {
                                    if (node.Header.ToString()==date)
                                    {
                                        TreeViewItem item = new TreeViewItem() { Header = f, ToolTip = file.Name, Tag = file.Name };
                                        node.Items.Add(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }


            }));
        }

        private void TreeJson_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((TreeViewItem)e.NewValue == null) return;
            LoadDataAsync(((TreeViewItem)e.NewValue).Tag?.ToString());
            Data.Cache.Redraw = true;
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                (sender as Label).Content = Path.GetFileName(dialog.SelectedPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)); ;
                (sender as Label).ToolTip = System.IO.Path.GetDirectoryName(dialog.SelectedPath);
                Data.Cache.PathJson = dialog.SelectedPath;
                LoadRootAsync();
                RefreshTreeView();
            }
        }
    }
}
