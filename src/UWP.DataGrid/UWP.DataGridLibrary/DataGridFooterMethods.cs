using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        private void HandleFooter(Point value, Size sz, double totalRowsSize, bool hasMoreItems)
        {
            if (Footer == null && FooterTemplate == null)
            {
                return;
            }
            var maxV = totalRowsSize + HeaderMeasureHeight + _columnHeaderPanel.DesiredSize.Height - sz.Height;
            if ((maxV + value.Y) <= 0)
            {
                //if (value.Y < 0)
                {
                    if (!hasMoreItems && FooterMeasureHeight == 0)
                    {
                        _footer.Measure(_contentGrid.DesiredSize);
                        if (_footer.DesiredSize != Size.Empty && _footer.DesiredSize.Height != 0 && _footer.DesiredSize.Width != 0)
                        {
                            _verticalScrollBar.Maximum += _footer.DesiredSize.Height;
                        }
                    }

                    var footHeight = -(maxV + value.Y);

                    if (footHeight > 0 && FooterMeasureHeight > 0)
                    {
                        if (footHeight <= FooterMeasureHeight)
                        {
                            FooterHeight = new GridLength(footHeight);
                            _footer.Margin = new Thickness(0, 0, 0, footHeight - FooterMeasureHeight);
                            _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, footHeight) };
                            if (!HasHeader())
                            {
                                _cellPanel.footerHeight = footHeight;
                            }
                        }
                        else
                        {
                            FooterHeight = new GridLength(FooterMeasureHeight);
                            if (!HasHeader())
                            {
                                _cellPanel.footerHeight = FooterMeasureHeight;
                            }
                        } 
                    }
                    else if (HasFooter())
                    {
                        FooterHeight = new GridLength(0);
                        _footer.Margin = new Thickness(0, 0, 0, -FooterMeasureHeight);
                        _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, 0) };
                        if (!HasHeader())
                        {
                            _cellPanel.footerHeight = 0;
                        }
                    }
                }
            }
            else
            {
                if (HasFooter())
                {
                    FooterHeight = new GridLength(0);
                    _footer.Margin = new Thickness(0, 0, 0, -FooterMeasureHeight);
                    _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, 0) };
                    if (!HasHeader())
                    {
                        _cellPanel.footerHeight = 0;
                    }
                }
            }
        }

    }
}
