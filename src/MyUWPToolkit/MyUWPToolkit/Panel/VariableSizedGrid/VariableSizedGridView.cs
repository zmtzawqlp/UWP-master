using MyUWPToolkit.Util;
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
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                var gridview = d as VariableSizedGridView;
                if (gridview.ItemsPanelRoot != null && gridview.ResizeableItem != null)
                {
                    VariableSizedWrapGrid wrapgrid = gridview.ItemsPanelRoot as VariableSizedWrapGrid;
                    wrapgrid.MaximumRowsOrColumns = gridview.ResizeableItem.Columns;
                    wrapgrid.ItemHeight = wrapgrid.ItemWidth = gridview.ResizeableItem.ItemWidth;
                    wrapgrid.Width = gridview.ResizeableItem.ItemWidth * gridview.ResizeableItem.Columns;
                    for (int i = 0; i < gridview.Items.Count; i++)
                    {
                        var gridviewItem = gridview.ContainerFromItem(gridview.Items[i]) as GridViewItem;
                        if (gridviewItem != null)
                        {
                            gridviewItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, gridview.ResizeableItem.Items[i].Width);
                            gridviewItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, gridview.ResizeableItem.Items[i].Height);
                        }
                    }
                    // wrapgrid.UpdateLayout();
                }
            }
            
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //var viewModel = item as IResizable;
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                var gridviewItem = element as GridViewItem;
                if (ResizeableItem != null)
                {
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, ResizeableItem.Items[this.Items.IndexOf(item)].Width);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, ResizeableItem.Items[this.Items.IndexOf(item)].Height);
                    if (this.ItemsPanelRoot != null)
                    {
                        VariableSizedWrapGrid wrapgrid = this.ItemsPanelRoot as VariableSizedWrapGrid;
                        wrapgrid.MaximumRowsOrColumns = this.ResizeableItem.Columns;
                        wrapgrid.ItemHeight = wrapgrid.ItemWidth = this.ResizeableItem.ItemWidth;
                        wrapgrid.Width = this.ResizeableItem.ItemWidth * this.ResizeableItem.Columns;
                    }
                }
            }

            base.PrepareContainerForItemOverride(element, item);
        }
    }

}
