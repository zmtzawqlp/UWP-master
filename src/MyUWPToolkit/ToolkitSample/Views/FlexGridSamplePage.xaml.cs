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
    public sealed partial class FlexGridSamplePage : Page
    {
        private MyIncrementalLoading<Employee> _employees;

        public FlexGridSamplePage()
        {
            this.InitializeComponent();
            Loaded += FlexGridSamplePage_Loaded;
            
        }

        private void FlexGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;
            flexgrid.FrozenColumnsHeaderItemsSource = new List<string>() { "test1" };
            flexgrid.ColumnsHeaderItemsSource = new List<string>() { "Name1", "Name2", "Name3", "Name4", "Name5", "Name6", "Name7"};
            flexgrid.ItemsSource = _employees;
        }

        private void flexgrid_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void flexgrid_PullToRefresh(object sender, EventArgs e)
        {

        }

        private void flexgrid_SortingColumn(object sender, MyUWPToolkit.FlexGrid.SortingColumnEventArgs e)
        {

        }
    }
}
