using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridFrozenColumnsPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine("FlexGridFrozenColumnsPanel");
            return base.ArrangeOverride(finalSize);
        }
    }
}
