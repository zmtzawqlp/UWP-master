using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridFrozenColumns : ListView
    {
        //
        // Summary:
        //     Occurs when an item in the list view receives an interaction, and the IsItemClickEnabled
        //     property is true.


        public FlexGridFrozenColumns()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
        }


        internal ItemsControl AnotherListViewer { get; set; }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlexGridItem;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlexGridItem();
            return base.GetContainerForItemOverride();
            //var container= base.GetContainerForItemOverride();
            //return container;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
           
            (element as FlexGridItem).AnotherListViewer = AnotherListViewer;
            (element as FlexGridItem).ParentListViewer = this;
            (element as FlexGridItem).DataItem = item;
            base.PrepareContainerForItemOverride(element, item);
            //(element as FlexGridItem).Style = this.ItemContainerStyle;
            //Debug.WriteLine("PrepareContainerForItemOverride : " + IndexFromContainer(element) + "," + ItemsPanelRoot?.Children.Count);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            //Style style = (element as FlexGridItem).Style;
            (element as FlexGridItem).AnotherListViewer = null;
            (element as FlexGridItem).ParentListViewer = null;
            (element as FlexGridItem).DataItem = null;
            base.ClearContainerForItemOverride(element, item);
            //(element as FlexGridItem).Style = style;
            Debug.WriteLine("ClearContainerForItemOverride : " + IndexFromContainer(element) + "," + ItemsPanelRoot?.Children.Count);
        }
        //public void OnItemClick()
        //{
        //    if (ItemClick != null)
        //    {
        //        ItemClick(this, new FlexGridItemClickEventArgs() { });
        //    }
        //}
    }

}
