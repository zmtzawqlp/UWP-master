using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyUWPToolkit
{
    /// <summary>
    /// VirtualizedVariableSizedGridView for Windows,
    /// it change variable item size for different control size
    /// you can define it in ObservableRowAapter's ResizeableItems property.
    /// for example, windowMinwidth is 500, windowMaxwidth is 1920
    /// default is (1920-500)/4=355
    /// that means 
    /// 1- 501 to 501+355 is one column style
    /// 2- 501 + 355*2+1 to 501 + 355*3 is two column style
    /// 3- 501 + 355*3+1 to 501 + 355*4 is three column style
    /// 4- 501 + 355*4+1 to double.PositiveInfinity is four column style
    /// you can define the style(the same as using MS VariableSizedWrapGrid) in ObservableRowAapter
    /// and in windows phone,it's the same as ListView.
    /// </summary>
    public class VirtualizedVariableSizedGridView : ListView
    {
        #region Fields
        private List<VariableSizedGridView> variableSizedGridViews = new List<VariableSizedGridView>();
        #endregion

        #region Property

        public new event ItemClickEventHandler ItemClick;

        private ResizeableItems _resizeableItems = null;

        public ResizeableItems ResizeableItems
        {
            get
            {
                if (_resizeableItems == null && ItemsSource != null)
                {
                    _resizeableItems = (ItemsSource as IResizeableItems).ResizeableItems;
                }
                if (_resizeableItems == null)
                {
                    //if null, will use default
                    Debug.Assert(false, "ItemsSource must be IResizeableItems");
                    InitializeResizeableItems();
                }
                return _resizeableItems;
            }
        }

        public DataTemplate VirtualizedVariableSizedGridViewItemTemplate
        {
            get { return (DataTemplate)GetValue(VirtualizedVariableSizedGridViewItemTemplateProperty); }
            set { SetValue(VirtualizedVariableSizedGridViewItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizedVariableSizedGridViewItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizedVariableSizedGridViewItemTemplateProperty =
            DependencyProperty.Register("VirtualizedVariableSizedGridViewItemTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null, OnVirtualizedVariableSizedGridViewItemTemplateChanged));

        private static void OnVirtualizedVariableSizedGridViewItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                var gridview = d as VirtualizedVariableSizedGridView;

                gridview.ItemTemplate = gridview.VirtualizedVariableSizedGridViewItemTemplate;
            }
        }

        #endregion

        public VirtualizedVariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VirtualizedVariableSizedGridView);
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                this.SizeChanged += VirtualizedVariableSizedGridView_SizeChanged;
            }
            else
            {
                 base.ItemClick += (s, e) =>
                 {
                     if (this.ItemClick != null)
                     {
                         ItemClick(this, e);
                     }
                 };
            }
        }

        private void InitializeResizeableItems()
        {
            if (_resizeableItems == null)
            {
                _resizeableItems = new ResizeableItems();

                // ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(0, 0));

                double windowMinwidth = 500;
                double windowMaxwidth = 1920;
                double rangwidth = (windowMaxwidth - windowMinwidth) / 4.0;

                #region 4
                var list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c4 = new ResizeableItem() { Columns = 4, Items = list, Min = windowMinwidth + rangwidth * 3 + 1, Max = double.PositiveInfinity };
                _resizeableItems.Add(c4);
                #endregion

                #region 3
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c3 = new ResizeableItem() { Columns = 3, Items = list, Min = windowMinwidth + rangwidth * 2 + 1, Max = windowMinwidth + rangwidth * 3 };
                _resizeableItems.Add(c3);
                #endregion

                #region 2
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c2 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + rangwidth * 1 + 1, Max = windowMinwidth + rangwidth * 2 };
                _resizeableItems.Add(c2);
                #endregion

                #region 1
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c1 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + +1, Max = windowMinwidth + rangwidth * 1 };
                _resizeableItems.Add(c1);
                #endregion
            }
        }

        private void VirtualizedVariableSizedGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ItemsSource != null)
            {
                var resizeableItem = ResizeableItems.GetItem(e.NewSize.Width);
                resizeableItem.ItemWidth = (int)(e.NewSize.Width / resizeableItem.Columns - 7);

                foreach (var item in this.Items)
                {
                    var gridviewItem = this.ContainerFromItem(item) as ListViewItem;
                    //not null, it's in viewport, so it need to update.
                    if (gridviewItem != null && gridviewItem.ContentTemplateRoot != null)
                    {
                        var gridview = gridviewItem.ContentTemplateRoot as VariableSizedGridView;
                        gridview.ResizeableItem = null;
                        gridview.ResizeableItem = resizeableItem;

                    }
                }
            }
        }

        #region Method

        #endregion

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                var gridviewItem = element as ListViewItem;
                //Container Recycling, so ContentTemplateRoot maybe not null.
                if (gridviewItem.ContentTemplateRoot != null)
                {
                    var gridview = gridviewItem.ContentTemplateRoot as VariableSizedGridView;
                    //variableSizedGridViews.Add(gridview);

                    var resizeableItem = ResizeableItems.GetItem(this.ActualWidth);
                    resizeableItem.ItemWidth = (int)(this.ActualWidth / resizeableItem.Columns - 7);
                    gridview.ResizeableItem = resizeableItem;
                    gridview.ItemClick += Gridview_ItemClick;
                    //gridview.ItemTemplate = VirtualizedVariableSizedGridViewItemTemplate;
                }
                else
                {
                    gridviewItem.Loaded += GridviewItem_Loaded;
                }
            }

        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                var gridview = (element as ListViewItem).ContentTemplateRoot as VariableSizedGridView;
                gridview.ItemClick -= Gridview_ItemClick;
                //variableSizedGridViews.Remove(gridview);
            }

            base.ClearContainerForItemOverride(element, item);
        }

        private void GridviewItem_Loaded(object sender, RoutedEventArgs e)
        {
            var gridview = (sender as ListViewItem).ContentTemplateRoot as VariableSizedGridView;
            variableSizedGridViews.Add(gridview);

            var resizeableItem = ResizeableItems.GetItem(this.ActualWidth);
            resizeableItem.ItemWidth = (int)(this.ActualWidth / resizeableItem.Columns - 7);
            gridview.ResizeableItem = resizeableItem;
            gridview.ItemClick += Gridview_ItemClick;
            gridview.ItemTemplate = VirtualizedVariableSizedGridViewItemTemplate;

            (sender as ListViewItem).Loaded -= GridviewItem_Loaded;
        }

        private void Gridview_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.ItemClick != null)
            {
                ItemClick(this, e);
            }
        }
    }
}
