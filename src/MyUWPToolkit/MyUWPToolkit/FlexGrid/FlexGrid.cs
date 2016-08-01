using MyUWPToolkit.Common;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls.Primitives;
using System.Diagnostics;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;

namespace MyUWPToolkit.FlexGrid
{

    [TemplatePart(Name = "ColumnHeader", Type = typeof(FlexGridFrozenColumns))]
    [TemplatePart(Name = "FrozenColumnsHeader", Type = typeof(FlexGridFrozenColumns))]
    [TemplatePart(Name = "FrozenColumns", Type = typeof(FlexGridFrozenColumns))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public partial class FlexGrid : ListView
    {


        public bool KeepHorizontalOffsetWhenItemsSourceChanged
        {
            get { return (bool)GetValue(KeepHorizontalOffsetWhenItemsSourceChangedProperty); }
            set { SetValue(KeepHorizontalOffsetWhenItemsSourceChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeepHorizontalOffsetWhenItemsSourceChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeepHorizontalOffsetWhenItemsSourceChangedProperty =
            DependencyProperty.Register("KeepHorizontalOffsetWhenItemsSourceChanged", typeof(bool), typeof(FlexGrid), new PropertyMetadata(true));



        #region ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);
            base.ItemClick += (s, e) => { OnItemClick(s, e); };
            //KeepHorizontalOffsetWhenItemsSourceChanged = false;
            //this.RegisterPropertyChangedCallback(ListView.ItemsSourceProperty, new DependencyPropertyChangedCallback(OnItemsSourceChanged));
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {

            //if (_frozenColumns != null && _scrollViewer != null)
            //{
            //    _scrollViewer.LeftHeader = null;
            //    _scrollViewer.LeftHeader = _frozenColumns;
            //}
        }

        #endregion

        #region override method
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Initialize();
            InitializeScrollViewer();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlexGridItem;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlexGridItem();
            return base.GetContainerForItemOverride();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            
            (element as FlexGridItem).AnotherListViewer = _frozenColumns;
            (element as FlexGridItem).ParentListViewer = this;
            (element as FlexGridItem).DataItem = item;
            base.PrepareContainerForItemOverride(element, item);
            
            //Debug.WriteLine("FlexGrid_PrepareContainerForItemOverride : " + IndexFromContainer(element) + "," + ItemsPanelRoot?.Children.Count);
        }

        int preCount;
        bool itemsCleared;
        bool itemsAddedAfterCleared;
        bool updatelayoutAfteritemsAddedAfterCleared;
        protected override void OnItemsChanged(object e)
        {
            if (KeepHorizontalOffsetWhenItemsSourceChanged)
            {
                if (preCount != 0 && this.Items.Count == 0)
                {
                    itemsCleared = true;
                }
                if (itemsCleared && this.Items.Count != 0)
                {
                    itemsCleared = false;
                    itemsAddedAfterCleared = true;
                }

                preCount = this.Items.Count;
            }

            base.OnItemsChanged(e);
        }
        double offset = 0;
        protected override Size MeasureOverride(Size availableSize)
        {

            if (KeepHorizontalOffsetWhenItemsSourceChanged && itemsAddedAfterCleared && _scrollViewer != null)
            {
                offset = _scrollViewer.HorizontalOffset;
            }

            var size = base.MeasureOverride(availableSize);

            if (KeepHorizontalOffsetWhenItemsSourceChanged && itemsAddedAfterCleared && _scrollViewer != null)
            {
                //if (offset >= _frozenColumns.ActualWidth + 35)
                {
                    updatelayoutAfteritemsAddedAfterCleared = true;
                    Debug.WriteLine("offset > _frozenColumns.ActualWidth");
                    _scrollViewer.ChangeView(offset, 0, null);
                }
                itemsAddedAfterCleared = false;
                //_scrollViewer.ChangeView(offset, 0, null);

            }
            return size;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ItemsPanelRoot != null && _frozenColumns != null && _columnHeader != null && _columnHeader.Items.Count > 0)
            {
                var width = finalSize.Width - _frozenColumns.ActualWidth;
                if (width > 100 * _columnHeader.Items.Count)
                {
                    //foreach (var item in ItemsPanelRoot.Children)
                    //{
                    //    (item as ListViewItem).Width = width;
                    //}

                    ////_columnHeader.Width = double.NaN;
                    //foreach (var item in _columnHeader.ItemsPanelRoot.Children)
                    //{
                    //    (item as ListViewItem).Width = width / _columnHeader.Items.Count;
                    //}
                    ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Disabled);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
                }
                else
                {
                    //foreach (var item in ItemsPanelRoot.Children)
                    //{
                    //    (item as ListViewItem).Width = 100 * _columnHeader.Items.Count;
                    //}

                    ////_columnHeader.Width= 100 * _columnHeader.Items.Count;
                    //foreach (var item in _columnHeader.ItemsPanelRoot.Children)
                    //{
                    //    (item as ListViewItem).Width = double.NaN;
                    //}
                    ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Auto);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Auto);
                }

            }


            var a = base.ArrangeOverride(finalSize);

            if (KeepHorizontalOffsetWhenItemsSourceChanged && updatelayoutAfteritemsAddedAfterCleared)
            {
                updatelayoutAfteritemsAddedAfterCleared = false;
                //_scrollViewer.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                //{
                //_scrollViewer.ChangeView(offset, 0, null);
                offset = 0;

                //});

                //this.UpdateLayout();
            }

            return a;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            //Debug.WriteLine("FlexGrid_ClearContainerForItemOverride : " + IndexFromContainer(element) + "," + ItemsPanelRoot?.Children.Count);
            (element as FlexGridItem).AnotherListViewer = null;
            (element as FlexGridItem).ParentListViewer = null;
            (element as FlexGridItem).DataItem = null;
            base.ClearContainerForItemOverride(element, item);
        }

        #endregion
        #region ScrollViewer
        private void InitializeScrollViewer()
        {
          
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            _scrollViewer.Loaded += _scrollViewer_Loaded;
        }

        private void _scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            _header = _scrollViewer.FindDescendantByName("Header") as ContentControl;
            Binding headerbinding = new Binding();
            headerbinding.Source = this;
            headerbinding.Mode = BindingMode.OneWay;
            headerbinding.Path = new PropertyPath("Header");
            _header.SetBinding(ContentControl.ContentProperty, headerbinding);

            Binding headerTemplatebinding = new Binding();
            headerTemplatebinding.Source = this;
            headerTemplatebinding.Mode = BindingMode.OneWay;
            headerTemplatebinding.Path = new PropertyPath("HeaderTemplate");
            _header.SetBinding(ContentControl.ContentTemplateProperty, headerTemplatebinding);

            _scrollContent = _scrollViewer.FindDescendantByName("ScrollContent") as Grid;

            return;
            _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerViewerManipulation.Compositor;

            double ratio = 1.0;

            _header.Measure(new Size(this.ActualWidth, this.ActualHeight));
            var headerHeight = _header.DesiredSize.Height;
            if (headerHeight == 0)
            {
                headerHeight = 50;
            }

            //_offsetAnimation = _compositor.CreateExpressionAnimation("(min(max(0, ScrollManipulation.Translation.Y * ratio) / Divider, 1)) * MaxOffsetY");
            _offsetAnimation = _compositor.CreateExpressionAnimation("ScrollManipulation.Translation.Y");
            _offsetAnimation.SetScalarParameter("Divider", (float)headerHeight);
            _offsetAnimation.SetScalarParameter("MaxOffsetY", (float)headerHeight);
            _offsetAnimation.SetScalarParameter("ratio", (float)ratio);
            _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);


            _scrollContentVisual = ElementCompositionPreview.GetElementVisual(_scrollContent);

            _scrollContentVisual.StartAnimation("Offset.Y", _offsetAnimation);
        }
        #endregion

        #region private method
        private void Initialize()
        {
            _columnHeader = GetTemplateChild("ColumnHeader") as ListView;
            _frozenColumnsHeader = GetTemplateChild("FrozenColumnsHeader") as ListView;
            _frozenColumns = GetTemplateChild("FrozenColumns") as FlexGridFrozenColumns;
            _frozenColumns.AnotherListViewer = this;
            _columnHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumnsHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumns.ItemClick += (s, e) => { OnItemClick(s, e); };
        }

        private void _columnHeader_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnColumnSorting(this, e);
        }


        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.ItemClick != null)
            {
                this.ItemClick(this, e);
            }
        }
        private void OnColumnSorting(object sender, ItemClickEventArgs e)
        {
            if (this.SortingColumn != null)
            {
                this.SortingColumn(this, new SortingColumnEventArgs(e.ClickedItem));
            }
        }

        #endregion
        public IEnumerable<object> GetVisibleItems()
        {
            return Util.Util.GetVisibleItems(this);
        }
    }
}
