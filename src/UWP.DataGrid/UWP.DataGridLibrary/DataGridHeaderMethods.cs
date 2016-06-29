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
            var headerHeight = HeaderMeasureHeight + totalScrollPosition.Y;
            if (headerHeight > 0)
            {
                if (totalScrollPosition.Y < 0)
                {
                    HeaderHeight = new GridLength(HeaderMeasureHeight + totalScrollPosition.Y);
                    _header.Margin = new Thickness(0, totalScrollPosition.Y, 0, 0);
                    _header.Clip = new RectangleGeometry() { Rect = new Rect(0, -totalScrollPosition.Y, this.ActualWidth, HeaderMeasureHeight) };
                }
                else
                {
                    if (HeaderHeight != GridLength.Auto)
                    {
                        HeaderHeight = new GridLength(HeaderMeasureHeight);
                        _header.Margin = new Thickness(0);
                        _header.Clip = null;
                    }
                }
            }
            else if (HasHeader())
            {
                HeaderHeight = new GridLength(0);
                _header.Margin = new Thickness(0, -HeaderMeasureHeight, 0, 0);
                _header.Clip = new RectangleGeometry() { Rect = new Rect(0, HeaderMeasureHeight, this.ActualWidth, HeaderMeasureHeight) };
            }
        }

    }
}
