using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    /// <summary>
    /// 
    /// </summary>
    public class VirtualizedVariableSizedGridView : ListView
    {
        #region Fields
        private List<VariableSizedGridView> variableSizedGridViews = new List<VariableSizedGridView>();
        #endregion

        #region Property
        private ResizeableItems _resizeableItems = new ResizeableItems();

        public ResizeableItems ResizeableItems
        {
            get { return _resizeableItems; }
            set { _resizeableItems = value; }
        }


        public object DataItemsSource
        {
            get { return (object)GetValue(DataItemsSourceProperty); }
            set { SetValue(DataItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataItemsSourceProperty =
            DependencyProperty.Register("DataItemsSource", typeof(object), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null, OnDataItemsSourceChanged));

        private static void OnDataItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            //var gridview = d as VirtualizedVariableSizedGridView;
            //if (gridview!=null)
            //{
            //    gridview.OnDataItemsSourceChanged();
            //}
        }



        public DataTemplate VirtualizedVariableSizedGridViewItemTemplate
        {
            get { return (DataTemplate)GetValue(VirtualizedVariableSizedGridViewItemTemplateProperty); }
            set { SetValue(VirtualizedVariableSizedGridViewItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizedVariableSizedGridViewItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizedVariableSizedGridViewItemTemplateProperty =
            DependencyProperty.Register("VirtualizedVariableSizedGridViewItemTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));

        #endregion

        public VirtualizedVariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VirtualizedVariableSizedGridView);
            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                this.IsItemClickEnabled = true;
            }
            else
            {
                this.IsItemClickEnabled = false;
            }
            this.SizeChanged += VirtualizedVariableSizedGridView_SizeChanged;

            _resizeableItems = new ResizeableItems();

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

            var c4 = new ResizeableItem() { Columns = 4, Items = list, Min = 1920 / 4 * 3, Max = double.PositiveInfinity };
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
            list.Add(new Resizable() { Width = 1, Height = 2 });

            var c3 = new ResizeableItem() { Columns = 3, Items = list, Min = 1920 / 4 * 2, Max = 1920 / 4 * 3 };
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

            var c2 = new ResizeableItem() { Columns = 2, Items = list, Min = 1920 / 4 * 0, Max = 1920 / 4 * 2 };
            _resizeableItems.Add(c2);
            #endregion

        }

        private void VirtualizedVariableSizedGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in variableSizedGridViews)
            {
                //var list = new List<Resizable>();
                //list.Add(new Resizable() { Width = 2, Height = 2 });
                //list.Add(new Resizable() { Width = 1, Height = 2 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 2 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                //list.Add(new Resizable() { Width = 1, Height = 1 });
                var resizeableItem = ResizeableItems.GetItem(e.NewSize.Width);
                resizeableItem.ItemWidth = (int)(e.NewSize.Width / resizeableItem.Columns - 7);
                item.ResizeableItem = resizeableItem;
            }
        }

        #region Method
        void OnDataItemsSourceChanged()
        {


        }
        #endregion

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            base.PrepareContainerForItemOverride(element, item);
            var gridviewItem = element as ListViewItem;

            gridviewItem.Loaded += GridviewItem_Loaded;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            var gridview = (element as ListViewItem).ContentTemplateRoot as VariableSizedGridView;
            variableSizedGridViews.Remove(gridview);

            base.ClearContainerForItemOverride(element, item);
        }

        private void GridviewItem_Loaded(object sender, RoutedEventArgs e)
        {
            var gridview = (sender as ListViewItem).ContentTemplateRoot as VariableSizedGridView;
            variableSizedGridViews.Add(gridview);
           
            var resizeableItem = ResizeableItems.GetItem(this.ActualWidth);
            resizeableItem.ItemWidth = (int)(this.ActualWidth / resizeableItem.Columns - 7);
            gridview.ResizeableItem = resizeableItem;

            gridview.ItemTemplate = VirtualizedVariableSizedGridViewItemTemplate;

            (sender as ListViewItem).Loaded -= GridviewItem_Loaded;
        }
    }
}
