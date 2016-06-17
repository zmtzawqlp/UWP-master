using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

        private ObservableCollection<Employee> _employees;

        public PullToRefreshControl()
        {
            this.InitializeComponent();

            listView.Loaded += IncrementalLoading_Loaded;
        }

        void IncrementalLoading_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            listView.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            listView.DataFetchSize = 1.0;
            listView.IncrementalLoadingThreshold = 1.0;

            _employees = new ObservableCollection<Employee>(1000, (startIndex, count) =>
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

    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, ISupportIncrementalLoading
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
        public ObservableCollection(uint totalCount, Func<int, int, List<T>> getDataFunc)
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


    public abstract class IncrementalLoadingBase : IList, ISupportIncrementalLoading, INotifyCollectionChanged
    {
        #region IList

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _storage.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _storage.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                return _storage[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_storage).CopyTo(array, index);
        }

        public int Count
        {
            get { return _storage.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        #endregion

        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get { return HasMoreItemsOverride(); }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        #endregion 

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion 

        #region Private methods

        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var items = await LoadMoreItemsOverrideAsync(c, count);
                var baseIndex = _storage.Count;

                _storage.AddRange(items);

                // Now notify of the new items
                NotifyOfInsertedItems(baseIndex, items.Count);

                return new LoadMoreItemsResult { Count = (uint)items.Count };
            }
            finally
            {
                _busy = false;
            }
        }

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _storage[i + baseIndex], i + baseIndex);
                CollectionChanged(this, args);
            }
        }

        #endregion

        #region Overridable methods

        protected abstract Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count);
        protected abstract bool HasMoreItemsOverride();

        #endregion 

        #region State

        List<object> _storage = new List<object>();
        bool _busy = false;

        #endregion 
    }


    public class ProductList : IncrementalLoadingBase
    {
        protected override bool HasMoreItemsOverride()
        {
            return this.Count < 11;
        }

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {

            IList<object> list = new List<object>();

            foreach (var item in Product.GetProductList(10))
            {
                list.Add(item);
            }

            return list;

        }
    }


    public class Product : INotifyPropertyChanged, IEditableObject
    {
        static string[] _lines = "Computers|Washers|Stoves".Split('|');
        static string[] _colors = "Red|Green|Blue|White|Yellow|Orange|Black|Gray".Split('|');

        // object model
        [Display(Name = "Line")]
        public string Line { get { return (string)GetValue("Line"); } set { SetValue("Line", value); } }

        [Display(Name = "Color")]
        public string Color { get { return (string)GetValue("Color"); } set { SetValue("Color", value); } }

        [Display(Name = "Name")]
        public string Name { get { return (string)GetValue("Name"); } set { SetValue("Name", value); } }

        [Display(Name = "Price")]
        public double Price { get { return (double)GetValue("Price"); } set { SetValue("Price", value); } }

        [Display(Name = "Weight")]
        public double Weight { get { return (double)GetValue("Weight"); } set { SetValue("Weight", value); } }

        [Display(Name = "Cost")]
        public double Cost { get { return (double)GetValue("Cost"); } set { SetValue("Cost", value); } }

        [Display(Name = "Volume")]
        public double Volume { get { return (double)GetValue("Volume"); } set { SetValue("Volume", value); } }

        [Display(Name = "Discontinued")]
        public bool Discontinued { get { return (bool)GetValue("Discontinued"); } set { SetValue("Discontinued", value); } }

        [Display(Name = "Rating")]
        public int Rating { get { return (int)GetValue("Rating"); } set { SetValue("Rating", value); } }

        // get/set values
        Dictionary<string, object> _values = new Dictionary<string, object>();
        object GetValue(string p)
        {
            object value;
            _values.TryGetValue(p, out value);
            return value;
        }
        void SetValue(string p, object value)
        {
            if (!object.Equals(value, GetValue(p)))
            {
                _values[p] = value;
                OnPropertyChanged(p);
            }
        }
        protected virtual void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        // factory
        public static ICollectionView GetProducts(int count)
        {
            var list = new System.Collections.ObjectModel.ObservableCollection<Product>();
            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                var p = new Product();
                p.Line = _lines[rnd.Next() % _lines.Length];
                p.Color = _colors[rnd.Next() % _colors.Length];
                p.Name = string.Format("{0} {1}{2}", p.Line.Substring(0, p.Line.Length - 1), p.Line[0], i);
                p.Price = rnd.Next(1, 1000);
                p.Weight = rnd.Next(1, 100);
                p.Cost = rnd.Next(1, 600);
                p.Volume = rnd.Next(500, 5000);
                p.Discontinued = rnd.NextDouble() < .1;
                p.Rating = rnd.Next(0, 5);
                list.Add(p);
            }
            return new CollectionViewSource() { Source = list }.View;
        }

        public static IList<Product> GetProductList(int count)
        {
            var list = new List<Product>();
            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                var p = new Product();
                p.Line = _lines[rnd.Next() % _lines.Length];
                p.Color = _colors[rnd.Next() % _colors.Length];
                p.Name = string.Format("{0} {1}{2}", p.Line.Substring(0, p.Line.Length - 1), p.Line[0], i);
                p.Price = rnd.Next(1, 1000);
                p.Weight = rnd.Next(1, 100);
                p.Cost = rnd.Next(1, 600);
                p.Volume = rnd.Next(500, 5000);
                p.Discontinued = rnd.NextDouble() < .1;
                p.Rating = rnd.Next(0, 5);
                list.Add(p);
            }
            return list;
            //return new CollectionViewSource() { Source = list }.View;
        }
        public static string[] GetLines()
        {
            return _lines;
        }
        public static string[] GetColors()
        {
            return _colors;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IEditableObject Members

        static Dictionary<string, object> _clone;
        public void BeginEdit()
        {
            if (_clone == null)
            {
                _clone = new Dictionary<string, object>();
            }
            _clone.Clear();
            foreach (var key in _values.Keys)
            {
                _clone[key] = _values[key];
            }
        }
        public void CancelEdit()
        {
            _values.Clear();
            foreach (var key in _clone.Keys)
            {
                _values[key] = _clone[key];
            }
        }
        public void EndEdit()
        {
        }

        #endregion
    }
}
