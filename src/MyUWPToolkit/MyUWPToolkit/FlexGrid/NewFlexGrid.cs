using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.FlexGrid
{
    /// <summary>
    /// to fix bug in 14393, re-creat one new FlexGrid
    /// </summary>
    [TemplatePart(Name = "ColumnsHeader", Type = typeof(NewFlexGridColumnHeader))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public class NewFlexGrid : ListView
    {
        #region Filed
        ScrollViewer _scrollViewer;
        ItemsPresenter _itemsPresenter;
        NewFlexGridColumnHeader _columnsHeader;
        NewFlexGridFrozenRows _frozenRows;
        #endregion

        #region Property
        ScrollViewer _outerScrollViewer;

        internal ScrollViewer OuterScrollViewer
        {
            get
            {
                if (_outerScrollViewer == null)
                {
                    var parent = this.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent is Page)
                        {
                            break;
                        }
                        _outerScrollViewer = parent as ScrollViewer;
                        if (_outerScrollViewer != null)
                        {
                            break;
                        }
                        parent = parent.Parent as FrameworkElement;
                    }

                }
                return _outerScrollViewer;
            }

        }

        public event EventHandler<SortingColumnEventArgs> SortingColumn;

        public ScrollViewer ScrollViewer
        {
            get
            {
                return _scrollViewer;
            }
        }
        #endregion

        public object ColumnsHeaderItemsSource
        {
            get { return (object)GetValue(ColumnsHeaderItemsSourceProperty); }
            set { SetValue(ColumnsHeaderItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsHeaderItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsHeaderItemsSourceProperty =
            DependencyProperty.Register("ColumnsHeaderItemsSource", typeof(object), typeof(NewFlexGrid), new PropertyMetadata(null));



        public object FrozenRowsItemsSource
        {
            get { return (object)GetValue(FrozenRowsItemsSourceProperty); }
            set { SetValue(FrozenRowsItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenRowsItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenRowsItemsSourceProperty =
            DependencyProperty.Register("FrozenRowsItemsSource", typeof(object), typeof(NewFlexGrid), new PropertyMetadata(null));



        public DataTemplate ColumnsHeaderItemTemplate
        {
            get { return (DataTemplate)GetValue(ColumnsHeaderItemTemplateProperty); }
            set { SetValue(ColumnsHeaderItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsHeaderItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsHeaderItemTemplateProperty =
            DependencyProperty.Register("ColumnsHeaderItemTemplate", typeof(DataTemplate), typeof(NewFlexGrid), new PropertyMetadata(null));


        public int ColumnHeaderFrozenCount
        {
            get { return (int)GetValue(ColumnHeaderFrozenCountProperty); }
            set { SetValue(ColumnHeaderFrozenCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderFrozenCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderFrozenCountProperty =
            DependencyProperty.Register("ColumnHeaderFrozenCount", typeof(int), typeof(NewFlexGrid), new PropertyMetadata(0));

        public new event ItemClickEventHandler ItemClick;

        public NewFlexGrid()
        {
            this.DefaultStyleKey = typeof(NewFlexGrid);
            this.AddHandler(UIElement.PointerWheelChangedEvent, new PointerEventHandler(FlexGrid_PointerWheelChanged), true);
            this.ContainerContentChanging += FlexGrid_ContainerContentChanging;
            this.Loaded += NewFlexGrid_Loaded;
            this.Unloaded += NewFlexGrid_Unloaded;
            base.ItemClick += NewFlexGrid_ItemClick; ;
        }

        private void NewFlexGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        ExpressionAnimation _offsetXAnimation;
        CompositionPropertySet _scrollerViewerManipulation;

        private void NewFlexGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_offsetXAnimation != null)
            {
                _offsetXAnimation.Dispose();
                _offsetXAnimation = null;
            }

            //if (_scrollerViewerManipulation != null)
            //{
            //    _scrollerViewerManipulation.Dispose();
            //    _scrollerViewerManipulation = null;
            //}
        }

        private void NewFlexGrid_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareCompositionAnimation();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InitializeScrollViewer();
            Initialize();
            PrepareCompositionAnimation();
        }


        private void FlexGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot;

            var child = templateRoot.GetAllChildren();
            var _frozenContent = child.Where(x => FlexGridItemFrozenContent.GetIsFrozenContent(x));
            if (_frozenContent != null && _offsetXAnimation != null)
            {
                foreach (var item in _frozenContent)
                {
                    var _frozenContentVisual = ElementCompositionPreview.GetElementVisual(item);

                    _frozenContentVisual.StartAnimation("Offset.X", _offsetXAnimation);

                }
            }

        }

        private void FlexGrid_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (_scrollViewer != null && _scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as UIElement);
                var delta = mousePosition.Properties.MouseWheelDelta;
                _scrollViewer.ChangeView(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset - delta, null);
            }

            if (OuterScrollViewer != null && OuterScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as UIElement);
                var delta = mousePosition.Properties.MouseWheelDelta;
                OuterScrollViewer.ChangeView(OuterScrollViewer.HorizontalOffset, OuterScrollViewer.VerticalOffset - delta, null);
            }
        }


        #region private method
        private void Initialize()
        {
            _columnsHeader = GetTemplateChild("ColumnHeader") as NewFlexGridColumnHeader;
            _frozenRows = GetTemplateChild("FrozenRows") as NewFlexGridFrozenRows;
            if (_frozenRows != null)
            {
                _frozenRows.ItemClick += NewFlexGrid_ItemClick;
            }
        }

        private void InitializeScrollViewer()
        {
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
        }

        private void PrepareCompositionAnimation()
        {
            if (_scrollViewer != null)
            {
                if (_scrollerViewerManipulation == null)
                {
                    _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);

                }
                if (_offsetXAnimation == null)
                {
                    _offsetXAnimation = _scrollerViewerManipulation.Compositor.CreateExpressionAnimation("-min(0,ScrollManipulation.Translation.X)");
                    _offsetXAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);
                    _columnsHeader._offsetXAnimation = _offsetXAnimation;
                    _frozenRows._offsetXAnimation = _offsetXAnimation;
                }
            }
        }

        private void _columnHeader_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnColumnSorting(this, e);
        }


        private void OnColumnSorting(object sender, ItemClickEventArgs e)
        {
            if (this.SortingColumn != null)
            {
                this.SortingColumn(this, new SortingColumnEventArgs(e.ClickedItem));
            }
        }

        #endregion


        public void UpdateWidth(double columnsWidth)
        {
            if (_scrollViewer != null)
            {
                _itemsPresenter = _scrollViewer.Content as ItemsPresenter;
            }
            if (_itemsPresenter != null)
            {
                _itemsPresenter.Width = columnsWidth;
            }
        }
    }
}
