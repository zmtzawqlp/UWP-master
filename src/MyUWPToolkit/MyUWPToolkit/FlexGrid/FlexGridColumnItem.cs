using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridColumnItem : ListViewItem
    {
        internal object DataItem { get; set; }
        internal FlexGridColumnHeader ParentListViewer { get; set; }

        public FlexGridColumnItem()
        {
            this.DefaultStyleKey = typeof(FlexGridColumnItem);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            //if (ParentListViewer != null )
            //{
            //    ParentListViewer.OnItemClick(DataItem, this); 
            //}
        }
    }
}
