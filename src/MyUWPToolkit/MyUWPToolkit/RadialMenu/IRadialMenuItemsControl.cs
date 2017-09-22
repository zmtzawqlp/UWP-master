using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.RadialMenu
{
    public interface IRadialMenuItemsControl
    {
        RadialMenuItemCollection Items { get; }
        //RadialMenuItem SelectedItem { get; }
        IEnumerable<RadialMenuItem> SelectedItems { get; }

        RadialMenuSelectionMode SelectionMode { get; set; }
        //RadialMenu Menu { get; }

        //IRadialMenuItemsControl ParentItem { get;}

        //void SetMenuAndParentItem(RadialMenu menu, IRadialMenuItemsControl parentItem);
    }
}
