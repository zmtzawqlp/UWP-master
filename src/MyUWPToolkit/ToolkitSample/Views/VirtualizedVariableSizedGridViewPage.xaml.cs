using MyUWPToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolkitSample.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VirtualizedVariableSizedGridViewPage : Page
    {
        private MyIncrementalLoading<Thing> _things;
        public VirtualizedVariableSizedGridViewPage()
        {
            this.InitializeComponent();
            Loaded += VirtualizedVariableSizedGridView_Loaded;
        }

        private async void VirtualizedVariableSizedGridView_Loaded(object sender, RoutedEventArgs e)
        {

            //demoList.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            //demoList.DataFetchSize = 2.0;
            //demoList.IncrementalLoadingThreshold = 1.0;
            _things = new MyIncrementalLoading<Thing>(1000, (startIndex, count) =>
            {
                //lblLog.Text += string.Format("从索引 {0} 处开始获取 {1} 条数据", startIndex, count);
                //lblLog.Text += Environment.NewLine;

                return new MainPageViewModel().Things;
            });
            //await _things.LoadMoreItemsAsync(10);
            var a= new RowAdapter<Thing>(_things, 2);
            this.demoList.ItemsSource = a;
            //_things.LoadMoreItemsAsync(10);
            a.CollectionChanged += A_CollectionChanged;
        }

        private void A_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.demoList.ItemsSource = null;
            this.demoList.ItemsSource = sender;
        }
    }

    public class MyListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            
            base.PrepareContainerForItemOverride(element, item);
        }



        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

       
    }

    public class DataGen
    {
        static ObservableCollection<string> data;

        public static ObservableCollection<string> Gen()
        {
            if (data == null)
            {
                data = new ObservableCollection<string>();
                for (int i = 0; i < 1000; i++)
                {
                    data.Add("block-" + i.ToString());
                }
            }

            return data;
        }
    }
}
