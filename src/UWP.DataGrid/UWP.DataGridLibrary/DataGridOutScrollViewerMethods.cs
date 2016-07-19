using System;
using UWP.DataGrid.Model.Cell;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        private void _outerScrollViewerContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GeneralTransform gt = this.TransformToVisual(sender as UIElement);
            var point = gt.TransformPoint(new Point(0, 0));
            topToOuterScrollViewer = point.Y;
        }

        private void _outerScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            
            GeneralTransform gt = this.TransformToVisual(OuterScrollViewer);
            var rect = gt.TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            //add delta,so that it does not look like suddenly
            if (rect.Bottom < 0 || rect.Top > OuterScrollViewer.ActualHeight)
            {
                //r.Row = Rows.GetItemAt(fy - scrollPosition.Y);
                //r.Row2 = Math.Min(Rows.GetItemAt(sz.Height - scrollPosition.Y), Rows.Count - 1);
            }
            //in view port
            else
            {
                var update = false;
                if (preview != null)
                {

                    if (OuterScrollViewerVerticalScrollEnable && OuterScrollViewerHorizontalScrollEnable)
                    {
                        update = (preview.VerticalOffset != e.NextView.VerticalOffset || preview.HorizontalOffset != e.NextView.HorizontalOffset);
                    }
                    else if (OuterScrollViewerVerticalScrollEnable)
                    {
                        update = preview.VerticalOffset != e.NextView.VerticalOffset;

                    }
                    else if (OuterScrollViewerHorizontalScrollEnable)
                    {
                        update = preview.HorizontalOffset != e.NextView.HorizontalOffset;
                    }

                    if (update)
                    {
                        var sz = _cellPanel.DesiredSize;
                        // find top/bottom rows
                        var r = new CellRange(Rows.Frozen, Columns.Frozen);
                        if (OuterScrollViewerVerticalScrollEnable)
                        {
                            sz.Height = OuterScrollViewer.ActualHeight * 1.5;
                            var y = HeaderActualHeight - OuterScrollViewer.VerticalOffset;
                            y += topToOuterScrollViewer;
                            r.Row = Rows.GetItemAt(-y - OuterScrollViewer.ActualHeight);
                            r.Row2 = Math.Min(Rows.GetItemAt(sz.Height - y), Rows.Count - 1);
                        }

                        if (OuterScrollViewerHorizontalScrollEnable)
                        {
                            sz.Width = OuterScrollViewer.ActualWidth * 1.5;
                            var x = -OuterScrollViewer.HorizontalOffset;
                            r.Column = Columns.GetItemAt(-x - OuterScrollViewer.ActualWidth);
                            r.Column2 = Math.Min(Columns.GetItemAt(sz.Width - x), Columns.Count - 1);
                        }


                        if (_cellPanel.ViewRange != r)
                        {
                            _cellPanel.UpdateViewRange(r);
                        }
                        if (_columnHeaderPanel.ViewRange.Column != r.Column || _columnHeaderPanel.ViewRange.Column2 != r.Column2)
                        {
                            _columnHeaderPanel.UpdateViewRange(r);
                        }
                    }
                }
                preview = e.NextView;
            }

        }

    }
}
