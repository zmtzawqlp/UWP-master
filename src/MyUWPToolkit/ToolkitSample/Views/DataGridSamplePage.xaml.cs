using MyUWPToolkit.DataGrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlDemo.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataGridSamplePage : Page
    {
        private MyIncrementalLoading<Employee> _employees;

        public DataGridSamplePage()
        {
            this.InitializeComponent();
            Loaded += DataGridSamplePage_Loaded;
        }

        private void DataGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            //listView.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            //listView.DataFetchSize = 2.0;
            //listView.IncrementalLoadingThreshold = 1.0;

            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                if (count==-1)
                {
                    count = 5;
                }

                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;

            datagrid.ItemsSource =_employees;
            datagrid.CrossSlide += Datagrid_CrossSlide;
        }

        private void Datagrid_CrossSlide(object sender, MyUWPToolkit.DataGrid.CrossSlideEventArgs e)
        {
            //Debug.WriteLine("Datagrid_CrossSlide");
            return;
            var index = this.Pivot.SelectedIndex;
            if (e.Mode== CrossSlideMode.Left)
            {
                index= index - 1;
                if (index<0)
                {
                    index = this.Pivot.Items.Count - 1;
                }
            }
            else
            {
                index = index + 1;
                if (index > this.Pivot.Items.Count - 1)
                {
                    index = 0;
                }
            }

            this.Pivot.SelectedIndex = index;
        }

        private void datagrid_ItemClick(object sender, MyUWPToolkit.DataGrid.ItemClickEventArgs e)
        {

        }

        private void datagrid_SortingColumn(object sender, MyUWPToolkit.DataGrid.SortingColumnEventArgs e)
        {
            e.Cancel = true;
            //_employees=_employees.OrderBy(x => x.IsMale).ToList();
            //_employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            //{


            //    return TestData.GetEmployees().Skip(startIndex).Take(count).OrderByDescending(x=>x.IsMale).ToList();
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
}
