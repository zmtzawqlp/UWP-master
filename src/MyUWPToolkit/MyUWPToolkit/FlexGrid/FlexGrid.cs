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
    [TemplatePart(Name = "FrozenColumnsHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public partial class FlexGrid : ListView
    {

        #region ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);
            this.AddHandler(UIElement.PointerWheelChangedEvent, new PointerEventHandler(FlexGrid_PointerWheelChanged), true);
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

            base.PrepareContainerForItemOverride(element, item);
            (element as FlexGridItem).StartAnimation(_scrollViewer);
            Binding b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath("FrozenColumnsItemTemplate");
            (element as FlexGridItem).SetBinding(FlexGridItem.FrozenColumnsItemTemplateProperty, b);

            Binding b1 = new Binding();
            b1.Source = this;
            b1.Path = new PropertyPath("FrozenColumnsVisibility");
            (element as FlexGridItem).SetBinding(FlexGridItem.FrozenColumnsVisibilityProperty, b1);
        }



        int preCount;
        bool itemsCleared;
        double offset = -1;
        protected override void OnItemsChanged(object e)
        {
            //don't set itemsource null,please clear and add immediately
            if (KeepHorizontalOffsetWhenItemsSourceChanged)
            {
                if (preCount != 0 && this.Items.Count == 0)
                {
                    itemsCleared = true;
                    //make it to int to prevent double 
                    offset = (int)_scrollViewer.HorizontalOffset;
                }
                else if (itemsCleared && this.Items.Count != 0)
                {
                    itemsCleared = false;
                    _scrollViewer.ChangeView(offset, 0, null);
                }
                else
                {
                    offset = -1;
                }
            }
            else
            {
                offset = -1;
            }

            preCount = this.Items.Count;
            base.OnItemsChanged(e);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            (element as FlexGridItem).ClearValue(FlexGridItem.FrozenColumnsItemTemplateProperty);
            (element as FlexGridItem).ClearValue(FlexGridItem.FrozenColumnsVisibilityProperty);
        }

        #endregion

        #region ScrollViewer
        private void InitializeScrollViewer()
        {

            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;

            _scrollViewer.Loaded += _scrollViewer_Loaded;
        }

        ItemsPresenter _itemsPresenter;
        private void _scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            _itemsPresenter = _scrollViewer.Content as ItemsPresenter;
            //_header = _scrollViewer.FindDescendantByName("Header") as ContentControl;
            //Binding headerbinding = new Binding();
            //headerbinding.Source = this;
            //headerbinding.Mode = BindingMode.OneWay;
            //headerbinding.Path = new PropertyPath("Header");
            //_header.SetBinding(ContentControl.ContentProperty, headerbinding);

            //Binding headerTemplatebinding = new Binding();
            //headerTemplatebinding.Source = this;
            //headerTemplatebinding.Mode = BindingMode.OneWay;
            //headerTemplatebinding.Path = new PropertyPath("HeaderTemplate");
            //_header.SetBinding(ContentControl.ContentTemplateProperty, headerTemplatebinding);
            _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerViewerManipulation.Compositor;
            if (_frozenColumnsHeader != null)
            {
                _offsetAnimation = _compositor.CreateExpressionAnimation("-min(0,ScrollManipulation.Translation.X)");

                _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

                _scrollContentVisual = ElementCompositionPreview.GetElementVisual(_frozenColumnsHeader);

                _scrollContentVisual.StartAnimation("Offset.X", _offsetAnimation);

            }

        }
        #endregion

        #region private method
        private void Initialize()
        {
            _columnHeader = GetTemplateChild("ColumnHeader") as ListView;
            _frozenColumnsHeader = GetTemplateChild("FrozenColumnsHeader") as ListView;
            _columnHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumnsHeader.ItemClick += _columnHeader_ItemClick;
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
