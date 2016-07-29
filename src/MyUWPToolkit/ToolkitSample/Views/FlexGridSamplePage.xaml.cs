using MyUWPToolkit.FlexGrid;
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
        private ObservableCollection<Employee> _employees;

        public FlexGridSamplePage()
        {
            this.InitializeComponent();
            Loaded += FlexGridSamplePage_Loaded;
            flexgrid.SizeChanged += Flexgrid_SizeChanged;
        }



        private void Flexgrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 800)
            {

                flexgrid.SetValue(ScrollViewer.HorizontalScrollModeProperty, ScrollMode.Disabled);
            }
        }

        private void FlexGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            _employees = new ObservableCollection<Employee>(1000, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;
            flexgrid.FrozenColumnsHeaderItemsSource = new System.Collections.ObjectModel.ObservableCollection<Column>() { new Column() { ColumnName = "test1" } };

            var columns = new System.Collections.ObjectModel.ObservableCollection<Column>();
            for (int i = 1; i < 8; i++)
            {
                columns.Add(new Column() { ColumnName = "Name" + i });
            }

            flexgrid.ColumnsHeaderItemsSource = columns;
            flexgrid.ItemsSource = _employees;
        }

 
        private void flexgrid_PullToRefresh(object sender, EventArgs e)
        {
            flexgrid.ItemsSource = null;
            flexgrid.ItemsSource = _employees;
        }

        private void flexgrid_SortingColumn(object sender, MyUWPToolkit.FlexGrid.SortingColumnEventArgs e)
        {
            flexgrid.ItemsSource = null;
            //foreach (var item in _employees.)
            //{

            //}
            //_employees[0].Name = "order";
            flexgrid.ItemsSource = _employees;
            //_employees.Insert(0,new Employee() { Name = "djkajdla" });
            //_employees.Clear();
            //flexgrid.ItemsSource = _employees.OrderBy(x=>x.Age);

            //flexgrid.ItemsSource = _employees;
            //flexgrid.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    break;
            }
        }

        private void flexgrid_ItemClick(object sender, ItemClickEventArgs e)
        {
          
        }
    }

}
