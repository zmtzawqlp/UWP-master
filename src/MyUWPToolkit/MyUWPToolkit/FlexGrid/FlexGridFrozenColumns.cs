using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridFrozenColumns:ListView
    {

        // Summary:
        //     Occurs after a control changes into a different state.
        public event VisualStateChangedEventHandler CurrentStateChanged;
        //
        // Summary:
        //     Occurs when a control begins changing into a different state.
        public event VisualStateChangedEventHandler CurrentStateChanging;


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlexGridItem;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlexGridItem();
            return base.GetContainerForItemOverride();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FlexGridItem).CurrentStateChanging += FlexGridFrozenColumns_CurrentStateChanging;
            (element as FlexGridItem).CurrentStateChanged += FlexGridFrozenColumns_CurrentStateChanged;
            base.PrepareContainerForItemOverride(element, item);
        }

        private void FlexGridFrozenColumns_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (CurrentStateChanged != null)
            {
                CurrentStateChanged(sender, e);
            }
        }

        private void FlexGridFrozenColumns_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (CurrentStateChanging != null)
            {
                CurrentStateChanging(sender, e);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FlexGridItem).CurrentStateChanging -= FlexGridFrozenColumns_CurrentStateChanging;
            (element as FlexGridItem).CurrentStateChanged -= FlexGridFrozenColumns_CurrentStateChanged;
            base.ClearContainerForItemOverride(element, item);
        }
    }
}
