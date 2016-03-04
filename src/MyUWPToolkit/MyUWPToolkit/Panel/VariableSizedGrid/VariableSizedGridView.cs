using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public class VariableSizedGridView : GridView
    {
        public ResizeableItem ResizeableItem
        {
            get { return (ResizeableItem)GetValue(ResizeableItemProperty); }
            set { SetValue(ResizeableItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResizeableItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeableItemProperty =
            DependencyProperty.Register("ResizeableItem", typeof(ResizeableItem), typeof(VariableSizedGridView), new PropertyMetadata(null, OnResizeableItemChanged));

        private static void OnResizeableItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridview = d as VariableSizedGridView;
            if (gridview.ItemsPanelRoot!=null)
            {
                VariableSizedWrapGrid wrapgrid=gridview.ItemsPanelRoot as VariableSizedWrapGrid;
                wrapgrid.MaximumRowsOrColumns = gridview.ResizeableItem.Columns;
                wrapgrid.ItemHeight = wrapgrid.ItemWidth = gridview.ResizeableItem.ItemWidth;

                foreach (var element in gridview.Items)
                {
                    var gridviewItem = gridview.ContainerFromItem(element) as GridViewItem;
                    if (gridviewItem != null)
                    {
                        gridviewItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, gridview.ResizeableItem.Items[gridview.index].Width);
                        gridviewItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, gridview.ResizeableItem.Items[gridview.index].Height);
                        gridview.index++;
                        if (gridview.index == gridview.ResizeableItem.Items.Count)
                        {
                            gridview.index = 0;
                        }
                    }
                }

            }
        }

        internal int index = 0;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //var viewModel = item as IResizable;

            var gridviewItem = element as GridViewItem;
            if (ResizeableItem != null)
            {
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, ResizeableItem.Items[index].Width);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, ResizeableItem.Items[index].Height);
                index++;
                if (index == ResizeableItem.Items.Count)
                {
                    index = 0;
                }
            }

            base.PrepareContainerForItemOverride(element, item);


        }
    }

}
