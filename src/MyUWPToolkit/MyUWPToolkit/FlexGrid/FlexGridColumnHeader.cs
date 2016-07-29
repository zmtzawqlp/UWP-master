using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridColumnHeader : ItemsControl
    {
        public event FlexGridItemClickEventHandler ItemClick;

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlexGridColumnItem;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlexGridColumnItem();
            return base.GetContainerForItemOverride();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FlexGridColumnItem).ParentListViewer = this;
            (element as FlexGridColumnItem).DataItem = item;
            base.PrepareContainerForItemOverride(element, item);
            //(element as FlexGridItem).Style = this.ItemContainerStyle;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FlexGridColumnItem).ParentListViewer = null;
            (element as FlexGridColumnItem).DataItem = null;
            base.ClearContainerForItemOverride(element, item);
            //(element as FlexGridItem).Style = style;
        }

        //public void OnItemClick(object clickedItem, object originalSource)
        //{
        //    if (ItemClick != null)
        //    {
        //        ItemClick(this, new FlexGridItemClickEventArgs(clickedItem, originalSource));
        //    }
        //}
    }
}
