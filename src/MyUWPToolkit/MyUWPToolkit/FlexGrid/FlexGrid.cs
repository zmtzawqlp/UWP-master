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

    [TemplatePart(Name = "ColumnHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "FrozenColumnsHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "FrozenColumns", Type = typeof(FlexGridFrozenColumns))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public partial class FlexGrid : ListView
    {

        #region ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);
            base.ItemClick += (s, e) => { OnItemClick(s, e); };
            PointerMoved += FlexGrid_PointerMoved;
        }

        private void FlexGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_frozenColumns != null)
            {
                var point = e.GetCurrentPoint(_frozenColumns).Position;
                var rect = new Rect(0, 0, _frozenColumns.ActualWidth, _frozenColumns.ActualWidth);
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
            (element as FlexGridItem).CurrentStateChanging += FlexGrid_CurrentStateChanging;
            base.PrepareContainerForItemOverride(element, item);
        }


        private void FlexGrid_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (_frozenColumns != null && _frozenColumns.ItemsPanelRoot != null)
            {
                var index = this.IndexFromContainer((e.Control as FlexGridItem));


                //if (currentVisualState.Index != -1)
                //{
                //    if (currentVisualState.Index != index)
                //    {
                //        if (currentVisualState.NewState != null)
                //        {
                //            (this.ContainerFromIndex(currentVisualState.Index) as FlexGridItem)?.GoToState(currentVisualState.NewState.Name);
                //            (_frozenColumns.ContainerFromIndex(currentVisualState.Index) as FlexGridItem)?.GoToState(currentVisualState.NewState.Name);
                //        }

                //        var item = _frozenColumns.ContainerFromIndex(index) as FlexGridItem;
                //        if (item != null)
                //        {
                //            item.GoToState(e.NewState.Name);
                //        }
                //        currentVisualState.Index = index;
                //        currentVisualState.OldState = e.OldState;
                //        currentVisualState.NewState = e.NewState;
                //    }
                //    else
                //    {
                //        if (currentVisualState.NewState.Name != e.NewState.Name)
                //        {
                //            (e.Control as FlexGridItem).GoToState(currentVisualState.NewState.Name);
                //        }
                //        else
                //        {
                //            (e.Control as FlexGridItem).GoToState(e.NewState.Name);
                //        }
                        
                //    }

                //}
                //else
                {
                    var item = _frozenColumns.ContainerFromIndex(index) as FlexGridItem;
                    if (item != null)
                    {
                        item.GoToState(e.NewState.Name);
                    }
                    currentVisualState.Index = index;
                    currentVisualState.OldState = e.OldState;
                    currentVisualState.NewState = e.NewState;
                }


            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FlexGridItem).CurrentStateChanging -= FlexGrid_CurrentStateChanging;
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
            _frozenColumns.CurrentStateChanging += _frozenColumns_CurrentStateChanging;
            _columnHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumnsHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumns.ItemClick += (s, e) => { OnItemClick(s, e); };
        }

        private void _frozenColumns_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {

            if (this.ItemsPanelRoot != null)
            {
                var index = _frozenColumns.IndexFromContainer((e.Control as FlexGridItem));
                Debug.WriteLine("_frozenColumns_CurrentStateChanging : " + e.NewState.Name + " index: " + index);

                //if (currentVisualState.Index != -1)
                //{
                //    if (currentVisualState.Index != index)
                //    {
                //        if (currentVisualState.NewState != null)
                //        {
                //            (this.ContainerFromIndex(currentVisualState.Index) as FlexGridItem)?.GoToState(currentVisualState.NewState.Name);
                //            (_frozenColumns.ContainerFromIndex(currentVisualState.Index) as FlexGridItem)?.GoToState(currentVisualState.NewState.Name);
                //        }

                //        var item = this.ContainerFromIndex(index) as FlexGridItem;
                //        if (item != null)
                //        {
                //            item.GoToState(e.NewState.Name);
                //        }
                //        currentVisualState.Index = index;
                //        currentVisualState.OldState = e.OldState;
                //        currentVisualState.NewState = e.NewState;
                //    }
                //    else
                //    {
                //        if (currentVisualState.NewState.Name != e.NewState.Name)
                //        {
                //            (e.Control as FlexGridItem).GoToState(currentVisualState.NewState.Name);
                //        }
                //        else
                //        {
                //            (e.Control as FlexGridItem).GoToState(e.NewState.Name);
                //        }
                //    }

                //}
                //else
                {
                    var item = this.ContainerFromIndex(index) as FlexGridItem;
                    if (item != null)
                    {
                        item.GoToState(e.NewState.Name);
                    }
                    currentVisualState.Index = index;
                    currentVisualState.OldState = e.OldState;
                    currentVisualState.NewState = e.NewState;
                }


                //else if (currentVisualState != e.NewState && currentVisualState != null && currentVisualState.Name != e.NewState.Name)
                //{
                //    (e.Control as FlexGridItem).GoToState(currentVisualState.Name);
                //}
            }
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
        private void OnItemsSourceChanged()
        {

        }



        #endregion
        public IEnumerable<object> GetVisibleItems()
        {
            return Util.Util.GetVisibleItems(this);
        }
    }
}
