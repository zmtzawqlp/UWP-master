using MyUWPToolkit.Common;
using MyUWPToolkit.DataGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlDemo.Model;
using MyUWPToolkit.CollectionView;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataGridSamplePage : Page
    {
        private MyIncrementalLoading<Employee> _employees;
        //private ObservableCollection<Employee> _employees;
        public DataGridSamplePage()
        {
            this.InitializeComponent();
            Loaded += DataGridSamplePage_Loaded;
        }

        private void DataGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {

            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                if (count == -1)
                {
                    count = 5;
                }

                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees = TestData.GetEmployees();
            datagrid.ItemsSource = _employees;
            //you can custom cell if you want 
            datagrid.CellFactory = new MyCellFactory();
        }

        private async void datagrid_ItemClick(object sender, MyUWPToolkit.DataGrid.ItemClickEventArgs e)
        {
            Employee ee = e.ClickedItem as Employee;
            await new MessageDialog("Click on " + ee.Name).ShowAsync();
        }

        private void datagrid_SortingColumn(object sender, MyUWPToolkit.DataGrid.SortingColumnEventArgs e)
        {

            //if you want to handle sort by youself
            //please set SortMode to Manual and set e.Cancel=true;
            //e.Cancel = true;
            //_employees.Clear();

            //_employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            //{


            //    return TestData.GetEmployees().Skip(startIndex).Take(count).OrderByDescending(x => x.IsMale).ToList();
            //});
            //datagrid.ItemsSource = _employees;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var test = datagrid.GetVisibleItems().ToList();
            //foreach (var item in test)
            //{

            //}
        }

        private void PullToRefreshPanel_PullToRefresh(object sender, EventArgs e)
        {
            datagrid.ItemsSource = null;
            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                if (count == -1)
                {
                    count = 5;
                }

                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;

            datagrid.ItemsSource = _employees;
        }
    }

    public class People
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class MyCellFactory : MyUWPToolkit.DataGrid.Model.Cell.CellFactory
    {
        public override FrameworkElement GetGlyphSort(ListSortDirection dir, Brush brush)
        {
            TextBlock tb= new TextBlock() { FontSize=20, Foreground = brush, Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center };
            if (dir == ListSortDirection.Ascending)
            {
                tb.Text = "↑"; 
            }
            else
            {
                tb.Text = "↓";         
            }
            return tb;
            //return base.GetGlyphSort(dir, brush);
        }
    }
}
