using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.DataGrid.Common;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (manipulationStatus != ManipulationStatus.None)
            {
                return;
            }
            switch (e.Key)
            {
                case Windows.System.VirtualKey.PageDown:
                    ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y- _verticalScrollBar.LargeChange);
                    break;
                case Windows.System.VirtualKey.PageUp:
                    ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y + _verticalScrollBar.LargeChange);
                    break;
                case Windows.System.VirtualKey.Home:
                    ScrollPosition = new Point(ScrollPosition.X, 0);
                    break;
                case Windows.System.VirtualKey.End:
                    ScrollPosition = new Point(ScrollPosition.X, -_verticalScrollBar.Maximum);
                    break;
                case Windows.System.VirtualKey.Left:
                    ScrollPosition = new Point(ScrollPosition.X + _horizontalScrollBar.SmallChange, ScrollPosition.Y);
                    break;
                case Windows.System.VirtualKey.Right:
                    ScrollPosition = new Point(ScrollPosition.X - _horizontalScrollBar.SmallChange, ScrollPosition.Y);
                    break;
                case Windows.System.VirtualKey.Up:
                    ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y + _verticalScrollBar.SmallChange);
                    break;
                case Windows.System.VirtualKey.Down:
                    ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y - _verticalScrollBar.SmallChange);
                    break;
                default:
                    break;
            }
        }
    }
}
