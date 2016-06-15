using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.DataGridLibrary.CollectionView;
using UWP.DataGridLibrary.DataGrid;
using UWP.DataGridLibrary.DataGrid.Model.Cell;
using UWP.DataGridSample.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.DataGridSample.Views
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

        private async void datagrid_ItemClick(object sender, DataGridLibrary.DataGrid.ItemClickEventArgs e)
        {
            Employee ee = e.ClickedItem as Employee;
            await new MessageDialog("Click on " + ee.Name).ShowAsync();
        }

        private void datagrid_SortingColumn(object sender, SortingColumnEventArgs e)
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

    public class MyCellFactory :CellFactory
    {
        public override FrameworkElement GetGlyphSort(ListSortDirection dir, Brush brush)
        {
            TextBlock tb = new TextBlock() { FontSize = 20, Foreground = brush, Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center };
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

    public class MyIncrementalLoading<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        // 是否正在异步加载中
        private bool _isBusy = false;

        // 提供数据的 Func
        // 第一个参数：增量加载的起始索引；第二个参数：需要获取的数据量；第三个参数：获取到的数据集合
        private Func<int, int, List<T>> _funcGetData;
        // 最大可显示的数据量
        private uint _totalCount = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalCount">最大可显示的数据量</param>
        /// <param name="getDataFunc">提供数据的 Func</param>
        public MyIncrementalLoading(uint totalCount, Func<int, int, List<T>> getDataFunc)
        {
            _funcGetData = getDataFunc;
            _totalCount = totalCount;
        }

        /// <summary>
        /// 是否还有更多的数据
        /// </summary>
        public bool HasMoreItems
        {
            get { return this.Count < _totalCount; }
        }

        /// <summary>
        /// 异步加载数据（增量加载）
        /// </summary>
        /// <param name="count">需要加载的数据量</param>
        /// <returns></returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_isBusy)
            {
                // throw new InvalidOperationException("忙着呢，先不搭理你");
            }
            _isBusy = true;

            var dispatcher = Window.Current.Dispatcher;


            //var items = _funcGetData(0, (int)count);
            //foreach (var item in items)
            //{
            //    this.Add(item);
            //}
            //return new LoadMoreItemsResult { Count = (uint)this.Count };


            return AsyncInfo.Run(
                (token) =>
                    Task.Run<LoadMoreItemsResult>(
                       async () =>
                       {
                           try
                           {
                               // 模拟长时任务
                               //await Task.Delay(1000);

                               // 增量加载的起始索引
                               var startIndex = this.Count;

                               await dispatcher.RunAsync(
                                    CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        //count = 10;
                                        // 通过 Func 获取增量数据
                                        var items = _funcGetData(startIndex, (int)count);
                                        foreach (var item in items)
                                        {
                                            this.Add(item);
                                        }
                                    });

                               // Count - 实际已加载的数据量
                               return new LoadMoreItemsResult { Count = (uint)this.Count };
                           }
                           finally
                           {
                               _isBusy = false;
                           }
                       },
                       token));
        }

        public void OrderBy()
        {

        }
    }

}
