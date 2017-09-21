using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.RadialMenu
{
    public enum RadialMenuSelectionMode
    {
        //
        // Summary:
        //     A user can't select items.
        None = 0,
        //
        // Summary:
        //     A user can select a single item.
        Single = 1,
        //
        // Summary:
        //     The user can select multiple items without entering a special mode.
        Multiple = 2,
        ////
        //// Summary:
        ////     The user can select multiple items by entering a special mode, for example when
        ////     depressing a modifier key.
        //Extended = 3
    }
}
