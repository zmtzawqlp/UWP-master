using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        private void HandleHeader(Point totalScrollPosition)
        {
            if (Header == null && HeaderTemplate == null)
            {
                return;
            }
            var headerHeight = _headerHeight + totalScrollPosition.Y;
            if (headerHeight > 0)
            {
                HeaderHeight = new GridLength(_headerHeight + totalScrollPosition.Y);
                _header.Margin = new Thickness(0, totalScrollPosition.Y, 0, 0);
                _header.Clip = new RectangleGeometry() { Rect = new Rect(0, -totalScrollPosition.Y, this.ActualWidth, _headerHeight) };
            }
            else if (HasHeader())
            {
                HeaderHeight = new GridLength(0);
                _header.Margin = new Thickness(0, _headerHeight, 0, 0);
                _header.Clip = new RectangleGeometry() { Rect = new Rect(0, _headerHeight, this.ActualWidth, _headerHeight) };
            }
        }

    }
}
