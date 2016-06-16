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
        private void HandleFooter(Point value, Size sz, double totalRowsSize, bool hasMoreItems)
        {
            var maxV = totalRowsSize + _headerHeight + _columnHeaderPanel.DesiredSize.Height - sz.Height;
            if ((maxV + value.Y) <= 0)
            {
                //if (value.Y < 0)
                {
                    if (!hasMoreItems && _footerHeight == 0)
                    {
                        _footer.Measure(_contentGrid.DesiredSize);
                        if (_footer.DesiredSize != Size.Empty && _footer.DesiredSize.Height != 0 && _footer.DesiredSize.Width != 0)
                        {
                            _verticalScrollBar.Maximum += _footer.DesiredSize.Height;
                            _footerHeight = _footer.DesiredSize.Height;
                        }
                    }

                    var footHeight = -(maxV + value.Y);

                    if (footHeight > 0 && _footerHeight > 0)
                    {
                        if (footHeight <= _footerHeight)
                        {
                            FooterHeight = new GridLength(footHeight);
                            _footer.Margin = new Thickness(0, 0, 0, footHeight - _footerHeight);
                            _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, footHeight) };
                            if (!HasHeader())
                            {
                                _cellPanel.footerHeight = footHeight;
                            }
                        }
                        else
                        {
                            FooterHeight = new GridLength(_footerHeight);
                            if (!HasHeader())
                            {
                                _cellPanel.footerHeight = _footerHeight;
                            }
                        } 
                    }
                    else if (HasFooter())
                    {
                        FooterHeight = new GridLength(0);
                        _footer.Margin = new Thickness(0, 0, 0, -_footerHeight);
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
                    _footer.Margin = new Thickness(0, 0, 0, -_footerHeight);
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
