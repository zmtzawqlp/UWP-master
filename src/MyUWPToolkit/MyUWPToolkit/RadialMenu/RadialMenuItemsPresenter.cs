using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.RadialMenu
{
    public class RadialMenuItemsPresenter : ItemsControl
    {
        public RadialMenu Menu { get; internal set; }

        #region override
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is RadialMenuItem menuItem)
            {
                menuItem.SetMenu(Menu);
            }
            else
            {

            }
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }
        #endregion
    }
}
