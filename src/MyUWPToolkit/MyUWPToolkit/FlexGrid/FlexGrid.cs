using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    [TemplatePart(Name = "FrozenColumnsHeader", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "FrozenColumnsCell", Type = typeof(ListView))]
    [TemplatePart(Name = "ColumnsHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "Cell", Type = typeof(ListView))]
    public partial class FlexGrid : Control
    {
        #region Ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);
           
        }
        #endregion
       
    }
}
