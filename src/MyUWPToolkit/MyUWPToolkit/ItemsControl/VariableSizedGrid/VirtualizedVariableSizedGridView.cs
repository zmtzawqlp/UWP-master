using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;

namespace MyUWPToolkit
{
    /// <summary>
    /// VirtualizedVariableSizedGridView for Windows,
    /// it change variable item size for different control size
    /// you can define it in ObservableRowAapter's ResizeableItems property.
    /// for example, windowMinwidth is 500, windowMaxwidth is 1920
    /// default is (1920-500)/4=355
    /// that means 
    /// 1- 501 to 501+355 is two column style
    /// 2- 501 + 355*2+1 to 501 + 355*3 is two column style
    /// 3- 501 + 355*3+1 to 501 + 355*4 is three column style
    /// 4- 501 + 355*4+1 to double.PositiveInfinity is four column style
    /// you can define the style(the same as using MS VariableSizedWrapGrid) in ObservableRowAapter
    /// and in windows phone,it's the same as ListView.
    /// </summary>
    public class VirtualizedVariableSizedGridView : ListView
    {

        #region EmptyContent
        private ContentPresenter _emptyContent;

        public object EmptyContent
        {
            get { return (object)GetValue(EmptyContentProperty); }
            set { SetValue(EmptyContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EmptyContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register("EmptyContent", typeof(object), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));

        public DataTemplate EmptyContentTemplate
        {
            get { return GetValue(EmptyContentTemplateProperty) as DataTemplate; }
            set { SetValue(EmptyContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty EmptyContentTemplateProperty =
            DependencyProperty.Register("EmptyContentTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));

        private void CustomListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEmptyContentVisibility();
            Loaded -= CustomListView_Loaded;
        }

        protected override void OnApplyTemplate()
        {
            _emptyContent = GetTemplateChild("EmptyContent") as ContentPresenter;
            base.OnApplyTemplate();
        }


        protected override void OnItemsChanged(object e)
        {
            if (Items.Count == 1)
            {
                UpdateEmptyContentVisibility();
            }
            base.OnItemsChanged(e);
        }

        public void UpdateEmptyContentVisibility()
        {
            if (_emptyContent == null)
            {
                Loaded += CustomListView_Loaded;
                return;
            }

            int count = 0;
            if (ItemsSource != null && ItemsSource is IObservableRowAapter)
            {
                var resizeableItems = ItemsSource as IObservableRowAapter;
                count = resizeableItems.SourceCount;
            }
            else
            {
                count = this.Items.Count;
            }

            if (count == 0)
            {
                _emptyContent.Visibility = Visibility.Visible;
            }
            else
            {
                _emptyContent.Visibility = Visibility.Collapsed;
            }

        }

        public void VisibleEmptyContent()
        {
            if (_emptyContent == null)
            {
                return;
            }

            _emptyContent.Visibility = Visibility.Visible;

        }

        public void CollapseEmptyContent()
        {
            if (_emptyContent == null)
            {
                return;
            }

            _emptyContent.Visibility = Visibility.Collapsed;

        }
        #endregion

        #region Property

        public new event ItemClickEventHandler ItemClick;

        private ResizeableItems _resizeableItems = null;

        public ResizeableItems ResizeableItems
        {
            get
            {

                if (_resizeableItems == null && ItemsSource != null && ItemsSource is IResizeableItems)
                {
                    _resizeableItems = (ItemsSource as IResizeableItems).ResizeableItems;
                }
                //if (_resizeableItems == null)
                //{
                //    //if null, will use default
                //    Debug.Assert(false, "ItemsSource must be IResizeableItems");
                //    InitializeResizeableItems();
                //}
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
        /// <summary>
        /// when ItemsSource is IResizeableItems,we should to disable point down animation and so on.
        /// </summary>
        public Style VirtualizedVariableSizedGridViewItemContainerStyle
        {
            get { return (Style)GetValue(VirtualizedVariableSizedGridViewItemContainerStyleProperty); }
            set { SetValue(VirtualizedVariableSizedGridViewItemContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizedVariableSizedGridViewItemContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizedVariableSizedGridViewItemContainerStyleProperty =
            DependencyProperty.Register("VirtualizedVariableSizedGridViewItemContainerStyle", typeof(Style), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));

        /// <summary>
        /// GridViewItem
        /// </summary>
        public Style VirtualizedVariableSizedInternalGridViewContainerStyle
        {
            get { return (Style)GetValue(VirtualizedVariableSizedInternalGridViewContainerStyleProperty); }
            set { SetValue(VirtualizedVariableSizedInternalGridViewContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizedVariableSizedInternalGridViewContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizedVariableSizedInternalGridViewContainerStyleProperty =
            DependencyProperty.Register("VirtualizedVariableSizedInternalGridViewContainerStyle", typeof(Style), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));



        #endregion

        public VirtualizedVariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VirtualizedVariableSizedGridView);

            base.ItemClick += VirtualizedVariableSizedGridView_ItemClick;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                OnMeasureOverride(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        private void VirtualizedVariableSizedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        private void InitializeResizeableItems()
        {
            if (_resizeableItems == null)
            {
                _resizeableItems = new ResizeableItems();

                // ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(0, 0));

                //minwindow 500
                double windowMinwidth = 500;
                double windowMaxwidth = DeviceInfo.DeviceScreenSize.Width;
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

        private void OnMeasureOverride(Size availableSize)
        {
            if (ItemsSource != null && ItemsSource is IResizeableItems && availableSize != Size.Empty)
            {
                var resizeableItem = ResizeableItems.GetItem(availableSize.Width);

                if (resizeableItem != null)
                {
                    resizeableItem.ItemWidth = (int)(availableSize.Width / resizeableItem.Columns - 7);

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
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (!PlatformIndependent.IsWindowsPhoneDevice && ItemsSource != null && ItemsSource is IResizeableItems)
            {
                base.ItemClick -= VirtualizedVariableSizedGridView_ItemClick;
                var gridviewItem = element as ListViewItem;
                if (VirtualizedVariableSizedGridViewItemContainerStyle != null)
                {
                    gridviewItem.Style = VirtualizedVariableSizedGridViewItemContainerStyle;
                }
                //Container Recycling, so ContentTemplateRoot maybe not null.
                if (gridviewItem.ContentTemplateRoot != null)
                {
                    var gridview = gridviewItem.ContentTemplateRoot as VariableSizedGridView;
                    //variableSizedGridViews.Add(gridview);
                    var resizeableItem = ResizeableItems.GetItem(this.ActualWidth);
                    if (resizeableItem != null)
                    {
                        resizeableItem.ItemWidth = (int)(this.ActualWidth / resizeableItem.Columns - 7);
                        gridview.ResizeableItem = resizeableItem;
                        gridview.ItemClick -= Gridview_ItemClick;
                        gridview.ItemClick += Gridview_ItemClick;
                    }

                    if (gridview.ItemContainerStyle == null)
                    {
                        Binding binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Mode = BindingMode.OneWay;
                        binding1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        binding1.Path = new PropertyPath("VirtualizedVariableSizedInternalGridViewContainerStyle");
                        gridview.SetBinding(GridView.ItemContainerStyleProperty, binding1);
                    }

                }
                else
                {
                    gridviewItem.Loaded -= GridviewItem_Loaded;
                    gridviewItem.Loaded += GridviewItem_Loaded;
                }
            }

        }

        //protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        //{
        //    if (!PlatformIndependent.IsWindowsPhoneDevice && ItemsSource != null && ItemsSource is IResizeableItems)
        //    {
        //        var gridview = (element as ListViewItem).ContentTemplateRoot as VariableSizedGridView;
        //        gridview.ItemClick -= Gridview_ItemClick;
        //        //variableSizedGridViews.Remove(gridview);
        //    }

        //    base.ClearContainerForItemOverride(element, item);
        //}

        private void GridviewItem_Loaded(object sender, RoutedEventArgs e)
        {
            var gridview = (sender as ListViewItem).ContentTemplateRoot as VariableSizedGridView;

            var resizeableItem = ResizeableItems.GetItem(this.ActualWidth);
            if (resizeableItem != null)
            {
                resizeableItem.ItemWidth = (int)(this.ActualWidth / resizeableItem.Columns - 7);
                gridview.ResizeableItem = resizeableItem;

            }
            gridview.ItemClick -= Gridview_ItemClick;
            gridview.ItemClick += Gridview_ItemClick;
            //gridview.ItemTemplate = VirtualizedVariableSizedGridViewItemTemplate;
            Binding binding = new Binding();
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Path = new PropertyPath("VirtualizedVariableSizedGridViewItemTemplate");
            gridview.SetBinding(GridView.ItemTemplateProperty, binding);

            Binding binding1 = new Binding();
            binding1.Source = this;
            binding1.Mode = BindingMode.OneWay;
            binding1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding1.Path = new PropertyPath("VirtualizedVariableSizedInternalGridViewContainerStyle");
            gridview.SetBinding(GridView.ItemContainerStyleProperty, binding1);
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
