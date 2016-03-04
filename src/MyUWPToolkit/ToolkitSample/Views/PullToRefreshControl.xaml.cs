using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class PullToRefreshControl : Page
    {

        private MyIncrementalLoading<Employee> _employees;

        public PullToRefreshControl()
        {
            this.InitializeComponent();

            listView.Loaded += IncrementalLoading_Loaded;
        }

        void IncrementalLoading_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            listView.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            listView.DataFetchSize = 2.0;
            listView.IncrementalLoadingThreshold = 1.0;

            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                lblLog.Text += string.Format("从索引 {0} 处开始获取 {1} 条数据", startIndex, count);
                lblLog.Text += Environment.NewLine;

                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });

            _employees.CollectionChanged += _employees_CollectionChanged;

            listView.ItemsSource = _employees;
        }

        void _employees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lblMsg.Text = "已获取的数据量：" + _employees.Count.ToString();
        }

        private async void PullToRefreshPanel_PullToRefresh(object sender, EventArgs e)
        {
            _employees.LoadMoreItemsAsync(20);
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
    }
}
