
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyUWPToolkit
{
    [TemplatePart(Name="Scroll", Type=typeof(ScrollViewer))]
    [TemplatePart(Name="LayoutArea", Type=typeof(Grid))]
    public class VirtualizedVariableSizedWrapGrid : Control
    {
        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(VirtualizedVariableSizedWrapGrid), new PropertyMetadata(null, ItemsSourceChanged));
        public static DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedWrapGrid), new PropertyMetadata(null, ItemTemplateChanged));
        public static DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(VirtualizedVariableSizedWrapGrid), new PropertyMetadata(0, ItemHeightPropertyChanged));
        public static DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(VirtualizedVariableSizedWrapGrid), new PropertyMetadata(0, ItemWidthPropertyChanged));

        private List<ItemContainer> _itemContainer;
        private Panel _panel;
        private ScrollViewer _scrollViewer;
        private const double SCREENS_PRELOADED = 0.5;
        private const double CARD_MARGIN = 6.0;
        private int _maxRows = 0;
        private object _selectedItem;
        private bool _moreDataRequested = false;

        public event SelectionChangedEventHandler SelectionChanged;
        public event EventHandler DataRequested;
        public delegate void CalculatingItemSizeEventHandler(ItemContainer item);
        public event CalculatingItemSizeEventHandler OnCalculatingItemSize;

        public VirtualizedVariableSizedWrapGrid()
        {
            DefaultStyleKey = typeof(VirtualizedVariableSizedWrapGrid);
            this.Style = (Style) XamlReader.Load(
                @"<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:controls=""using:MyUWPToolkit"" TargetType=""controls:VirtualizedVariableSizedWrapGrid"">
                    <Setter Property=""Template"">
                        <Setter.Value>
                            <ControlTemplate TargetType=""controls:VirtualizedVariableSizedWrapGrid"">
                            <ScrollViewer x:Name=""Scroll"" ZoomMode=""Disabled"" HorizontalScrollMode=""Auto"" VerticalScrollMode=""Disabled"" HorizontalScrollBarVisibility=""Hidden"" VerticalScrollBarVisibility=""Hidden"">
                                <Grid x:Name=""LayoutArea"" />
                            </ScrollViewer>
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
            </Style>");
            this.Loaded += VirtualizedVariableSizedWrapGrid_Loaded;
        }

        #region Dependency Property Changed
        private static void ItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VirtualizedVariableSizedWrapGrid varSizeGrid = sender as VirtualizedVariableSizedWrapGrid;
            if (varSizeGrid == null)
                return;

            varSizeGrid.OnItemsSourceChanged(e);
        }

        private static void ItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VirtualizedVariableSizedWrapGrid varSizeGrid = sender as VirtualizedVariableSizedWrapGrid;
            if (varSizeGrid == null)
                return;

            varSizeGrid.OnItemTemplateChanged(e); 
        }

        private static void ItemHeightPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void ItemWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region Virtual Voids
        protected virtual void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateView();
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                if (e.OldValue is INotifyCollectionChanged)
                {
                    INotifyCollectionChanged collection = (INotifyCollectionChanged)this.ItemsSource;
                    collection.CollectionChanged -= collection_CollectionChanged;
                }

                ClearItems();
            }
            if (e.NewValue != null)
            {
                GenerateItemContainers((IEnumerable)e.NewValue);

                if (e.NewValue is INotifyCollectionChanged)
                {
                    INotifyCollectionChanged collection = (INotifyCollectionChanged)this.ItemsSource;
                    collection.CollectionChanged += collection_CollectionChanged;
                }
            }
        }
        #endregion

        #region Properties
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public object ItemsSource 
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public object SelectedValue
        {
            get { return _selectedItem; }
            set
            {
                if (value == _selectedItem)
                    return;

                if (value == null)
                {
                    _selectedItem = null;
                    return;
                }

                var oldValue = _selectedItem;

                if (BringIntoView(value))
                {
                    _selectedItem = value;
                    if (SelectionChanged != null)
                        SelectionChanged(this, new SelectionChangedEventArgs(new List<object>() { oldValue }, new List<object>() { value }));
                }
            }
        }
        #endregion

        #region Overrides
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = this.GetTemplateChild("LayoutArea") as Panel;
            _scrollViewer = this.GetTemplateChild("Scroll") as ScrollViewer;
            if (ItemsSource != null)
                GenerateItemContainers(ItemsSource);
        }
        #endregion

        #region Private voids
        private void ClearItems()
        {
            if (_panel == null)
                return;

            _panel.Children.Clear();
        }

        private void GenerateItemContainers(object items)
        {
            if (_panel == null || _panel.Visibility == Windows.UI.Xaml.Visibility.Collapsed || _panel.ActualHeight == 0 || _panel.ActualWidth == 0)
                return;

            if (_itemContainer == null)
                _itemContainer = new List<ItemContainer>();
            _itemContainer.Clear();
            
            if (items is IEnumerable)
            {
                int _currentRow = 0, _currentColumn = 0;
                int _currentColWidth = 1;
                _maxRows = (int)(_panel.ActualHeight / (this.ItemHeight + (2* CARD_MARGIN)));
                foreach (var item in items as IEnumerable)
                {
                    ItemContainer container = new ItemContainer();
                    container.DataItem = item;

                    if (OnCalculatingItemSize != null)
                        OnCalculatingItemSize(container);

                    if (container.RowSpan <= 0)
                        container.RowSpan = 1;
                    if (container.ColumnSpan <= 0)
                        container.ColumnSpan = 1;

                    container.Column = _currentColumn;
                    container.Row = _currentRow;

                    if (container.RowSpan + _currentRow > _maxRows) //Not enough rows --> new column
                    { 
                        _itemContainer.Last().RowSpan = _maxRows - _itemContainer.Last().Row;

                        _currentColumn++;
                        container.Column = _currentColumn;
                        container.Row = 0;
                        _currentRow = container.RowSpan;
                        _currentColWidth = container.ColumnSpan;
                        _itemContainer.Add(container);
                        continue;
                    }

                    //Make column as wide as widest element
                    if (container.ColumnSpan > _currentColWidth)
                    { _currentColWidth = container.ColumnSpan; }
                    if (_currentColWidth > container.ColumnSpan)
                    { container.ColumnSpan = _currentColWidth; }

                    _currentRow += container.RowSpan;
                    if (_currentRow >= _maxRows)
                    {
                        //Make sure all elements in column are as wide as widest element
                        foreach (var wider in _itemContainer.Where(x => x.ColumnSpan < _currentColWidth && x.Column == _currentColumn))
                        { wider.ColumnSpan = _currentColWidth; }

                        _currentRow = 0;
                        _currentColumn += _currentColWidth;
                        _currentColWidth = 1;
                    }

                    _itemContainer.Add(container);
                }
            }

            UpdateView();
        }

        private void UpdateView()
        {
            if (_panel == null || _scrollViewer == null || _itemContainer == null || _itemContainer.Count == 0)
                return;

            _panel.Width = ((_itemContainer.Last().Column + 1) * this.ItemWidth) + this.Padding.Left + this.Padding.Right;

            var margin = _scrollViewer.ActualWidth * SCREENS_PRELOADED;
            double minLeft = (_scrollViewer.HorizontalOffset - margin);
            double maxLeft = (_scrollViewer.HorizontalOffset + _scrollViewer.ActualWidth + margin);
            int minColumn = (int)(minLeft / this.ItemWidth);
            int maxColumn = (int)(maxLeft / this.ItemWidth);

            var shown = _itemContainer.Where(x => x.Column >= minColumn && x.Column <= maxColumn).ToList();
            var toRecycle = _panel.Children.Where(x => x is FrameworkElement && (((FrameworkElement)x).Margin.Left < minLeft || ((FrameworkElement)x).Margin.Left > maxLeft)).Cast<FrameworkElement>().ToList();

            foreach (var item in shown)
            {
                if (item.IsRealized)
                    continue;

                bool _wasRecycled = true;
                FrameworkElement obj;
                obj = toRecycle.FirstOrDefault();
                if (obj == null)
                {
                    obj = ItemTemplate.LoadContent() as FrameworkElement;
                    obj.Tapped += Item_Tapped;
                    _wasRecycled = false;
                }
                else
                {
                    //Recycle
                    foreach (var recycled in _itemContainer.Where(x => x.DataItem == obj.DataContext))
                        recycled.IsRealized = false;
                    toRecycle.Remove(obj);
                }

                obj.DataContext = item.DataItem;
                obj.Height = (this.ItemHeight * item.RowSpan) - (2 * CARD_MARGIN);
                obj.Width = (this.ItemWidth * item.ColumnSpan) -  (2 * CARD_MARGIN);
                obj.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                obj.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                obj.Visibility = Windows.UI.Xaml.Visibility.Visible;
                double left = (item.Column * this.ItemWidth) + this.Padding.Left; 
                double top = (item.Row * this.ItemHeight) + this.Padding.Top;
                obj.Margin = new Thickness(left, top, 0, 0);
                
                if (!_wasRecycled)
                    _panel.Children.Add(obj);

                item.IsRealized = true;
            }
        }

        private void Item_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
            object previousObject = _selectedItem;
            object dataObject = ((FrameworkElement)sender).DataContext;

            if (dataObject != _selectedItem)
            {
                _selectedItem = dataObject;
                if (SelectionChanged != null)
                    SelectionChanged(this, new SelectionChangedEventArgs(new List<object>() { previousObject }, new List<object>() { dataObject }));
            }
        }

        void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
                _moreDataRequested = false;

            ClearItems();
            GenerateItemContainers(this.ItemsSource);
            UpdateView();
        }

        void VirtualizedVariableSizedWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._scrollViewer != null)
            {
                this._scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
            }
            _panel.SizeChanged += _panel_SizeChanged;
        }

        void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateView();

            if (!_moreDataRequested && this._scrollViewer.HorizontalOffset + this._scrollViewer.ActualWidth >= (this._panel.ActualWidth))
            {
                _moreDataRequested = true;
                if (DataRequested != null)
                    DataRequested(this, EventArgs.Empty);
            }
        }

        void _panel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int currentMaxRows = _maxRows;
            _maxRows = (int)(_panel.ActualHeight / (this.ItemHeight + (2 * CARD_MARGIN)));
            if (_maxRows != currentMaxRows) //Orientation probably changed, control got higher
            {
                ClearItems();
                GenerateItemContainers(this.ItemsSource);
                UpdateView();
                return;
            }

            if (_itemContainer != null && _itemContainer.Count > 0 && _panel.ActualWidth > 0)
                UpdateView();
        }
        #endregion

        #region Public voids
        public bool BringIntoView(object obj)
        {
            if (_itemContainer == null || _itemContainer.Count == 0)
                return false;

            var item = (from a in _itemContainer
                        where a.DataItem == obj
                        select a).FirstOrDefault();
            if (item == null)
                return false;

            //Calculate position
            double offset = (item.Column * this.ItemWidth) + CARD_MARGIN;
            this._scrollViewer.ChangeView(offset, null, null);
            UpdateView();

            return true;
        }
        #endregion

        public class ItemContainer
        {
            public ItemContainer() { }

            public object DataItem { get; set; }
            public int ColumnSpan { get; set; }
            public int RowSpan { get; set; }
            public int Column { get; internal set; }
            public int Row { get; internal set; }
            public bool IsRealized { get; internal set; }
        }
    }
}
