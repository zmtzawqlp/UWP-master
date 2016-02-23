using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public class VariableSizedGridView : GridView
    {
        private const int INCREMENTAL_THRESHOLD = 100;
        private ScrollViewer _scrollViewer;

        VariableSizedWrapGridDataContext _variableSizedWrapGridDataContext;



        #region DP
        public DataTemplate VariableSizedItemTemplate
        {
            get { return (DataTemplate)GetValue(VariableSizedItemTemplateProperty); }
            set { SetValue(VariableSizedItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VariableSizedItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VariableSizedItemTemplateProperty =
            DependencyProperty.Register("VariableSizedItemTemplate", typeof(DataTemplate), typeof(VariableSizedGridView), new PropertyMetadata(null));
        #endregion
        public VariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VariableSizedGridView);
            this.Loaded += VariableSizedGridView_Loaded;
            this.Unloaded += VariableSizedGridView_Unloaded;
        }

        private void VariableSizedGridView_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void VariableSizedGridView_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            _variableSizedWrapGridDataContext = new VariableSizedWrapGridDataContext();
            if (this.ItemsPanelRoot != null)
            {
                this.ItemsPanelRoot.DataContext = _variableSizedWrapGridDataContext;

            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _variableSizedWrapGridDataContext.ItemHeight = e.Size.Height / 8.0;
            _variableSizedWrapGridDataContext.ItemWidth = e.Size.Width / 4.0;
        }

        protected override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            base.OnApplyTemplate();
            //_scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
            //_scrollViewer.Loaded += _scrollViewer_Loaded;
        }

        private async void _scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset < INCREMENTAL_THRESHOLD)
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    await (ItemsSource as ISupportIncrementalLoading).LoadMoreItemsAsync(20);
                }
            }
        }

        private async void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Debug.WriteLine(this.ItemsPanelRoot.Children.Count);

            if (_scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset < INCREMENTAL_THRESHOLD)
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    await (ItemsSource as ISupportIncrementalLoading).LoadMoreItemsAsync(20);
                }
            }
        }
        protected override DependencyObject GetContainerForItemOverride()
        {


            var a= base.GetContainerForItemOverride();
            return a;
        }

        VariableSizedItemDataContext preitemDataContext = null;
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var viewModel = item as IResizable;

            var gridviewItem = element as GridViewItem;


            //element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, viewModel.Width);
            //element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.Height);
            //var gridviewItem = element as GridViewItem;
            //gridviewItem.Width = 100;
            //gridviewItem.Height = 200;

            if (preitemDataContext == null)
            {
                //base.PrepareContainerForItemOverride(element, item);
                var itemDataContext = new VariableSizedItemDataContext();
                gridviewItem.ContentTemplate = this.ItemTemplate;
                gridviewItem.DataContext = itemDataContext;
                gridviewItem.Content = new TextBlock { Text = "213123" };
                gridviewItem.Width = 300;
                gridviewItem.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
                //gridviewItem.DataContext = itemDataContext;
                itemDataContext.Items = new ObservableCollection<object>();
                itemDataContext.VariableSizedItemTemplate = this.VariableSizedItemTemplate;
                preitemDataContext = itemDataContext;
                gridviewItem.DataContextChanged += GridviewItem_DataContextChanged;
                //preGridview = gridviewItem.ContentTemplate.LoadContent() as GridView;
                //preGridview.ItemTemplate = VariableSizedItemTemplate;
                //if (preGridview.ItemsSource == null)
                //{
                //    preGridview.ItemsSource = new ObservableCollection<object>() { item };
                //}
                //else
                //{
                //    (preGridview.ItemsSource as ObservableCollection<object>).Add(item);
                //}
            }
            preitemDataContext.Items.Add(item);
            element = null;
            if (preitemDataContext.Items.Count == 5)
            {
              
                preitemDataContext = null;
            }
            //if ((preGridview.ItemsSource as ObservableCollection<object>).Count==5)
            //{
            //    preGridview = null;
            //}
           // base.PrepareContainerForItemOverride(element, item);
        }

        private void GridviewItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
           
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            return base.MeasureOverride(availableSize);
        }

        protected override void OnDisconnectVisualChildren()
        {
            base.OnDisconnectVisualChildren();
        }



        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }



}
