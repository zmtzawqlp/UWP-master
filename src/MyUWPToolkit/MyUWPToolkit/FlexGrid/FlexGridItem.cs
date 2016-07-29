using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridItem : ListViewItem
    {

        internal object DataItem { get; set; }
        internal ItemsControl AnotherListViewer { get; set; }
        internal ItemsControl ParentListViewer { get; set; }

        public FlexGridItem()
        {
            this.DefaultStyleKey = typeof(FlexGridItem);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {

            base.OnPointerEntered(e);
            GoToState("PointerOver");

        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {

            base.OnPointerExited(e);
            GoToState("Normal");
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {

            base.OnPointerPressed(e);
            GoToState("Pressed");
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {

            base.OnPointerReleased(e);
            GoToState("PointerOver");

            //if (ParentListViewer != null)
            //{
            //    if (ParentListViewer is FlexGridFrozenColumns)
            //    {
            //        (ParentListViewer as FlexGridFrozenColumns).OnItemClick(DataItem, this);
            //    }
            //    if (ParentListViewer is FlexGrid)
            //    {
            //        (ParentListViewer as FlexGrid).OnItemClick(DataItem, this);
            //    }
            //}
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        public void GoToState(string stateName)
        {
            if (ParentListViewer == null || AnotherListViewer == null)
            {
                Debug.WriteLine("ParentListViewer or AnotherListViewer is null");
                return;
            }


            int index = ParentListViewer.IndexFromContainer(this);
            //Debug.WriteLine( stateName + ": " + index);
            var item = AnotherListViewer.ContainerFromIndex(index);
            if (item != null && item is FlexGridItem)
            {
                VisualStateManager.GoToState((item as FlexGridItem), stateName, false);
            }

            //VisualStateManager.GoToState(this, stateName, true);
        }
    }
}
