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
using MyUWPToolkit.Util;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlexGridSamplePage : Page
    {
        private ObservableCollection<Employee> _employees;
        System.Collections.ObjectModel.ObservableCollection<MyColumn> columns;
        public FlexGridSamplePage()
        {
            this.InitializeComponent();
            Loaded += FlexGridSamplePage_Loaded;
            
        }



        private void Flexgrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width!=e.PreviousSize.Width)
            {
                UpdateColumns(e.NewSize.Width);
            }
        }

        private void FlexGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            _employees = new ObservableCollection<Employee>(1000, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;
            
            columns = new System.Collections.ObjectModel.ObservableCollection<MyColumn>();
            for (int i = 1; i < 8; i++)
            {
                columns.Add(new MyColumn() { ColumnName = "Name" + i });
            }
            flexgrid.FrozenColumnsHeaderItemsSource = columns.Take(1).ToObservableCollection<MyColumn>();
            flexgrid.ColumnsHeaderItemsSource = columns;
            flexgrid.ItemsSource = _employees;
            UpdateColumns(flexgrid.ActualWidth);
            flexgrid.SizeChanged += Flexgrid_SizeChanged;
        }


        private void UpdateColumns(double width)
        {
            if (width == 0)
            {
                return;
            }
            var columnsCount = columns.Count;
            //aleady start columns
            if (columnsCount == 0)
            {
                return;
            }

            var currentColumnsCount = (flexgrid.ColumnsHeaderItemsSource as ObservableCollection<MyColumn>)?.Count;
            //var verticalScrollBarWidth = PlatformIndependent.IsWindowsPhoneDevice ? 0 : 12;
            var verticalScrollBarWidth = 0;

            if (DeviceInfo.IsNarrowSrceen)
            {
                var narrowScreenItemTemplate = this.Resources["NarrowScreenItemTemplate"] as DataTemplate;

                if (flexgrid.ItemTemplate != narrowScreenItemTemplate)
                {
                    flexgrid.ItemTemplate = narrowScreenItemTemplate;
                }


                var newcolumns = columns.Take(4).ToObservableCollection<MyColumn>();
                if (currentColumnsCount != 4)
                {
                    flexgrid.ColumnsHeaderItemsSource = newcolumns;
                }
                flexgrid.FrozenColumnsHeaderItemsSource = null;
                flexgrid.FrozenColumnsItemTemplate = null;
                flexgrid.FrozenColumnsVisibility = Visibility.Collapsed;
                var w = (width - 100 - verticalScrollBarWidth) / (newcolumns.Count - 1);
                foreach (var item in newcolumns)
                {
                    item.ColumnWidth = w;
                }
                columns[0].ColumnWidth = 100;
                flexgrid.UpdateWidth(width - verticalScrollBarWidth);
            }
            else
            {
                double columnsSize = 100 + 110 * (columns.Count - 1);

                var wideScreenItemTemplate = this.Resources["WideScreenItemTemplate"] as DataTemplate;

                if (flexgrid.ItemTemplate != wideScreenItemTemplate)
                {
                    flexgrid.ItemTemplate = wideScreenItemTemplate;
                }

                //update column
                if (currentColumnsCount != columns.Count)
                {
                    flexgrid.ColumnsHeaderItemsSource = columns;
                }

                //update list
                if (columnsSize < width - verticalScrollBarWidth)
                {
                    flexgrid.FrozenColumnsHeaderItemsSource = null;
                    flexgrid.FrozenColumnsItemTemplate = null;
                    flexgrid.FrozenColumnsVisibility = Visibility.Collapsed;
                    var w = (width - 100 - verticalScrollBarWidth) / (columns.Count - 1);
                    foreach (var item in columns)
                    {
                        item.ColumnWidth = w;
                    }
                    columns[0].ColumnWidth = 100;
                    flexgrid.UpdateWidth(width - verticalScrollBarWidth);
                }
                else
                {
                    if (flexgrid.FrozenColumnsHeaderItemsSource == null)
                    {
                        flexgrid.FrozenColumnsHeaderItemsSource = columns.Take(1).ToObservableCollection<MyColumn>();
                    }
                    if (flexgrid.FrozenColumnsItemTemplate == null)
                    {
                        flexgrid.FrozenColumnsItemTemplate = this.Resources["FrozenColumnsItemTemplate"] as DataTemplate;
                    }
                    flexgrid.FrozenColumnsVisibility = Visibility.Visible;
                    foreach (var item in columns)
                    {
                        item.ColumnWidth = 110;
                    }
                    columns[0].ColumnWidth = 100;
                    flexgrid.UpdateWidth(double.NaN);
                }

            }
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


        private void flexgrid_ItemClick(object sender, ItemClickEventArgs e)
        {
          
        }

        private void PullToRefreshGrid_PullToRefresh(object sender, EventArgs e)
        {
            _employees.Clear();
        }
    }


    public class MyColumn:Column,INotifyPropertyChanged
    {
        private double columnWidth = 110;

        public double ColumnWidth
        {
            get { return columnWidth; }
            set
            {
                if (columnWidth != value)
                {
                    columnWidth = value;
                    OnPropertyChanged("ColumnWidth");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        //private double _frozenColumnWidth = 100;

      
        //public double FrozenColumnWidth
        //{
        //    get { return _frozenColumnWidth; }
        //    set
        //    {
        //        if (_frozenColumnWidth != value)
        //        {
        //            _frozenColumnWidth = value;
        //            OnPropertyChanged("FrozenColumnWidth");
        //        }
        //    }
        //}

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
