using MyUWPToolkit;
using System;
using System.Collections.Generic;
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
    public class VirtualizedVariableSizedGridView:ListView
    {
        #region Fields
        
        #endregion

        #region Property


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
            var gridview = d as VirtualizedVariableSizedGridView;
            if (gridview!=null)
            {
                gridview.OnDataItemsSourceChanged();
            }
        }



        public DataTemplate VirtualizedVariableSizedGridViewItemTemplate
        {
            get { return (DataTemplate)GetValue(VirtualizedVariableSizedGridViewItemTemplateProperty); }
            set { SetValue(VirtualizedVariableSizedGridViewItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizedVariableSizedGridViewItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizedVariableSizedGridViewItemTemplateProperty =
            DependencyProperty.Register("VirtualizedVariableSizedGridViewItemTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));


        public static DataTemplate GetGridViewItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(GridViewItemTemplateProperty);
        }

        public static void SetGridViewItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(GridViewItemTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for GridViewItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridViewItemTemplateProperty =
            DependencyProperty.RegisterAttached("GridViewItemTemplate", typeof(DataTemplate), typeof(VirtualizedVariableSizedGridView), new PropertyMetadata(null));



        #endregion

        public VirtualizedVariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VirtualizedVariableSizedGridView);
        }


        #region Method
        void OnDataItemsSourceChanged()
        {
            //var list = DataItemsSource as IList;
            //Type type = new Type();
            //List<Type> a = new List<Type>();
            //RowAdapter<object> a = DataItemsSource as RowAdapter<object>;
            //DataTemplate a = new DataTemplate();
            //a.
        }
        #endregion

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            base.PrepareContainerForItemOverride(element, item);
            var gridviewItem = element as ListViewItem;
            //var gridview = gridviewItem.ContentTemplate.LoadContent() as GridView;
            //gridviewItem.ApplyTemplate();
            //var gridview = gridviewItem.ContentTemplateRoot as GridView;
           
            //gridviewItem.Loaded += GridviewItem_Loaded;
        }

        private void GridviewItem_Loaded(object sender, RoutedEventArgs e)
        {
            var gridview = (sender as ListViewItem).ContentTemplateRoot as GridView;
            gridview.ItemTemplate = VirtualizedVariableSizedGridViewItemTemplate;
            (sender as ListViewItem).Loaded -= GridviewItem_Loaded;
        }
    }
}
