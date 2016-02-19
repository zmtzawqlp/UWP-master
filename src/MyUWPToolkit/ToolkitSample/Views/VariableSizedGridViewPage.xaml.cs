using System;
using System.Collections.Generic;
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
    public sealed partial class VariableSizedGridViewPage : Page
    {
        private MyIncrementalLoading<Thing> _things;

        public VariableSizedGridViewPage()
        {
            this.InitializeComponent();
            this.gridview.Loaded += Gridview_Loaded;
        }

        private async void Gridview_Loaded(object sender, RoutedEventArgs e)
        {
            //gridview.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            //gridview.DataFetchSize = 2.0;
            //gridview.IncrementalLoadingThreshold = 1.0;

            _things = new MyIncrementalLoading<Thing>(1000, (startIndex, count) =>
            {
                //lblLog.Text += string.Format("从索引 {0} 处开始获取 {1} 条数据", startIndex, count);
                //lblLog.Text += Environment.NewLine;

                return new MainPageViewModel().Things;
            });

            //_employees.CollectionChanged += _employees_CollectionChanged;

            gridview.ItemsSource = _things;
           //await _things.LoadMoreItemsAsync(100);
            //await _things.LoadMoreItemsAsync(100);
            //await _things.LoadMoreItemsAsync(100);
            //await _things.LoadMoreItemsAsync(100);
            //await _things.LoadMoreItemsAsync(100);
            //await _things.LoadMoreItemsAsync(100);
        }

        private void gridview_DataRequested(object sender, EventArgs e)
        {

        }

        int i = 0;
        //int j = 0;
        Random r = new Random();
        private void gridview_OnCalculatingItemSize(MyUWPToolkit.VirtualizedVariableSizedWrapGrid.ItemContainer item)
        {
          
            item.ColumnSpan = r.Next(1, 3);
            item.RowSpan = r.Next(1, 3);
            //if (i++%3==0)
            //{
            //    item.ColumnSpan = 1;
            //    item.RowSpan = 2;
            //}
            //else 
            //{
            //    item.ColumnSpan = 1;
            //    item.RowSpan = 1;
            //}
        }

        private void gridview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    //public class ItemsWrapGrid1: ItemsWrapGrid
    //{

    //}
}
