using MeteoViewer.Data;
using MeteoViewer.Map;
//using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

namespace MeteoViewer.Table
{
    /// <summary>
    /// Interakční logika pro UserControlTableView.xaml
    /// </summary>
    public partial class UserControlTableView : UserControl
    {

        public static UserControlTableView Instance { get; set; }

        private struct MyData
        {
            public string orp { set; get; }
            public int value { set; get; }
        }

        public UserControlTableView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;

            CreateTable();
        }

        private void CreateTable()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            DataGridTextColumn col2 = new DataGridTextColumn();
            outputDataGrid.Columns.Add(col1);
            outputDataGrid.Columns.Add(col2);
            col1.Binding = new Binding("orp");
            col2.Binding = new Binding("value");
            col1.Header = "ORP";
            col2.Header = "Hodnota";
            outputDataGrid.IsReadOnly = true;
            MenuItemExport.IsEnabled = false;
            MenuItemExportAll.IsEnabled = false;
        }

        internal void Refresh()
        {
            outputDataGrid.Items.Clear();
            foreach (var item in Data.Region.CurrentORP)
                outputDataGrid.Items.Add(new MyData { orp = item.Key, value = item.Value });

            MenuItemExport.IsEnabled = outputDataGrid.Items.Count > 0 ? true : false;
            MenuItemExportAll.IsEnabled = outputDataGrid.Items.Count > 0 ? true : false;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execel files (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = UserControlMap.Instance.ComboOutputList.SelectedItem.ToString();
            saveFileDialog.Title = "Exportovat výstup do Excelu";

            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
                SaveToExcel(saveFileDialog.FileName);
        }

        private void ExportAll_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execel files (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = UserControlMap.Instance.ComboOutputList.SelectedItem.ToString();
            saveFileDialog.Title = "Exportovat všechny hodiny do Excelu";

            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
                SaveAllToExcel(saveFileDialog.FileName);
        }
        
        private void SaveToExcel(string fileName)
        {
            try
            {
                /*
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = true;
                app.WindowState = XlWindowState.xlMaximized;

                Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet ws = wb.Worksheets[1];
                
                //ws.Range["A1"].Value = "ORP";
                //ws.Cells[1, 1].EntireRow.Font.Bold = true;

                //ws.Range["B1"].Value = Data.Stream.GetJData("samplename")[Cache.indexHour];
                //ws.Cells[1, 2].EntireRow.Font.Bold = true;
                
                for (int i = 1; i <= Data.Region.CurrentORP.Count; i++)
                {
                    ws.Range["A" + i].Value = Data.Region.CurrentORP.ElementAt(i - 1).Key;
                    ws.Range["B" + i].Value = Data.Region.CurrentORP.ElementAt(i - 1).Value;
                }

                ws.Columns.AutoFit();
                wb.SaveAs(fileName);
                */
            }
            catch 
            {

            }
        
        }

        private void SaveAllToExcel(string fileName)
        {
            try
            {/*
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = true;
                app.WindowState = XlWindowState.xlMaximized;

                Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet ws = wb.Worksheets[1];

                ws.Range["A1"].Value = "ORP";
                ws.Cells[1, 1].EntireRow.Font.Bold = true;
                JArray arrSample = Data.Stream.GetJData("samplename");
                int c = 2;
                foreach (var hour in arrSample)
                {
                    ws.Cells[1, c].Value = hour;
                    ws.Cells[1, c].EntireRow.Font.Bold = true;
                    c++;
                }

                for (int i = 1; i <= Data.Region.CurrentORP.Count; i++)
                {
                    ws.Range["A" + (i+1)].Value = Data.Region.CurrentORP.ElementAt(i - 1).Key;
                    for (int j = 1; j <=arrSample.Count; j++)
                    {
                        ws.Cells[i+1, j+1].Value = Data.Stream.GetJDataValue(j-1, Cache.indexOutputlist, j - 1);
                    }
                }
                ws.Columns.AutoFit();
                wb.SaveAs(fileName);*/
                //wb.Close();
            }
            catch
            {
            }
        }


    }
}
