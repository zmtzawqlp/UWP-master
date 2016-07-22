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
    [TemplatePart(Name = "FrozenColumns", Type = typeof(ListView))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public partial class FlexGrid : ListView
    {
        #region ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);
            base.ItemClick += (s, e) => { OnItemClick(s, e); };
        }
        #endregion

        #region override method
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Initialize();
            InitializeScrollViewer();

        }

        private void Initialize()
        {
            _columnHeader = GetTemplateChild("ColumnHeader") as ListView;
            _frozenColumnsHeader = GetTemplateChild("FrozenColumnsHeader") as ListView;
            _frozenColumns = GetTemplateChild("FrozenColumns") as ListView;
            _columnHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumnsHeader.ItemClick += _columnHeader_ItemClick;
            _frozenColumns.ItemClick += (s, e) => { OnItemClick(s, e); };
        }

        private void _columnHeader_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnColumnSorting(this, e);
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

        #region Common
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
            //reset scrollviewer

        }

        public IEnumerable<object> GetVisibleItems()
        {
            return Util.Util.GetVisibleItems(this);
        }
        #endregion
        #endregion

    }
}
