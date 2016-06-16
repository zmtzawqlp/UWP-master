using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.DataGrid.Common;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {

        private void _contentGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            if (PivotItem != null && manipulationStatus == ManipulationStatus.None)
            {
                if (FrozenColumns > 0)
                {
                    var pt = e.Position;
                    pt = this.TransformToVisual(_columnHeaderPanel).TransformPoint(pt);

                    var fx = _columnHeaderPanel.Columns.GetFrozenSize();
                    // adjust for scroll position
                    var sp = _columnHeaderPanel.ScrollPosition;
                    if (pt.X < 0 || pt.X > fx) pt.X -= sp.X;
                    var column = _columnHeaderPanel.Columns.GetItemAt(pt.X);
                    if (FrozenColumns > column)
                    {
                        startingCrossSlideLeft = startingCrossSlideRight = true;
                    }
                }
            }
        }

        private void _contentGrid_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            startingPullToRefresh = false;
            startingCrossSlideLeft = false;
            startingCrossSlideRight = false;
            if (_verticalScrollBar.Value == 0 && AllowPullToRefresh)
            {
                startingPullToRefresh = true;
            }

            //Cross Slide left
            if (_horizontalScrollBar.Value == 0)
            {
                startingCrossSlideLeft = true;
            }
            //double
            if (Math.Abs(_horizontalScrollBar.Value - _horizontalScrollBar.Maximum) < 0.0001)
            {
                startingCrossSlideRight = true;
            }

        }

        private void _contentGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

            if (e != null && e.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            VisualStateManager.GoToState(this, "NoIndicator", true);

            //int pullToRefreshHeaderRow = 0;
            //if (_headerHeight == 0)
            //{
            //    pullToRefreshHeaderRow = 3;
            //}
            //Grid.SetRow(_pullToRefreshHeader, pullToRefreshHeaderRow);
            _pullToRefreshHeader.Height = 0;
            _contentGrid.RowDefinitions[Grid.GetRow(_pullToRefreshHeader)].Height = new GridLength(0);

            if (PivotItem != null)
            {
                PivotItem.RenderTransform = new TranslateTransform();
            }

            if (IsReachThreshold && manipulationStatus == ManipulationStatus.PullToRefresh)
            {
                if (PullToRefresh != null)
                {
                    LastRefreshTime = DateTime.Now;
                    PullToRefresh(this, null);
                }
            }
            IsReachThreshold = false;
            startingPullToRefresh = false;
            startingCrossSlideLeft = false;
            startingCrossSlideRight = false;
            manipulationStatus = ManipulationStatus.None;
            scollingDirection = ScollingDirection.None;
            preDeltaTranslationX = 0;
            preDeltaTranslationY = 0;

            _contentGrid.ManipulationDelta -= _contentGrid_ManipulationDelta;
            _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;

        }

        private void _contentGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            var x = e.Delta.Translation.X;
            var y = e.Delta.Translation.Y;

            switch (manipulationStatus)
            {
                case ManipulationStatus.None:
                    HandleManipulationDelta(e, x, y);
                    break;
                case ManipulationStatus.CrossSlideLeft:
                    HandleCrossSlideLeft(x, y);
                    break;
                case ManipulationStatus.CrossSlideRight:
                    HandleCrossSlideRight(x, y);
                    break;
                case ManipulationStatus.PullToRefresh:
                    HandlePullToRefresh(x, y, e);
                    break;
                case ManipulationStatus.Scrolling:
                    HandleScrolling(x, y, e);
                    break;
                default:
                    HandleManipulationDelta(e, x, y);
                    break;
            }



        }

        private void HandleManipulationDelta(ManipulationDeltaRoutedEventArgs e, double x, double y)
        {
            //Cross Slide left
            if (PivotItem != null && ((x > 0) || PivotItemTT.X != 0) && Math.Abs(x) > Math.Abs(y) && startingCrossSlideLeft)
            {
                HandleCrossSlideLeft(x, y);
            }
            //Cross Slide right
            else if (PivotItem != null && ((x < 0) || PivotItemTT.X != 0) && Math.Abs(x) > Math.Abs(y) && startingCrossSlideRight)
            {
                HandleCrossSlideRight(x, y);
            }
            //support pull to refresh
            else if (((_pullToRefreshHeader.Height == 0 && y > 0) || _pullToRefreshHeader.Height > 0) && Math.Abs(x) < Math.Abs(y) && _verticalScrollBar.Value == 0 && startingPullToRefresh && OuterScrollViewer == null)
            {
                HandlePullToRefresh(x, y, e);
            }
            else if (_pullToRefreshHeader.Height == 0)
            {
                HandleScrolling(x, y, e);
            }
        }

        private void HandleCrossSlideLeft(double x, double y)
        {
            manipulationStatus = ManipulationStatus.CrossSlideLeft;
            VisualStateManager.GoToState(this, "NoIndicator", true);
            var maxThreshold = (PivotItem.Parent as Pivot).ActualWidth * 4 / 5.0;
            var tt = PivotItemTT;
            if (Math.Abs(tt.X) <= maxThreshold && preDeltaTranslationX != x)
            {
                preDeltaTranslationX = x;
                _contentGrid.ManipulationDelta -= _contentGrid_ManipulationDelta;
                _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;
                if (tt.X >= maxThreshold)
                {
                    HandleCrossSlide(CrossSlideMode.Left);
                }
                else
                {
                    var width = tt.X + x;

                    if (width > maxThreshold)
                    {
                        width = maxThreshold;
                        tt.X = width >= 0 ? width : 0;
                        PivotItem.RenderTransform = tt;
                        HandleCrossSlide(CrossSlideMode.Left);
                        return;
                    }

                    tt.X = width >= 0 ? width : 0;
                    PivotItem.RenderTransform = tt;
                }
            }
        }

        private void HandleCrossSlideRight(double x, double y)
        {
            manipulationStatus = ManipulationStatus.CrossSlideRight;
            VisualStateManager.GoToState(this, "NoIndicator", true);
            var maxThreshold = (PivotItem.Parent as Pivot).ActualWidth * 4 / 5.0;
            var tt = PivotItemTT;
            if (Math.Abs(tt.X) <= maxThreshold && preDeltaTranslationX != x)
            {
                preDeltaTranslationX = x;
                _contentGrid.ManipulationDelta -= _contentGrid_ManipulationDelta;
                _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;

                if (Math.Abs(tt.X) >= maxThreshold)
                {
                    HandleCrossSlide(CrossSlideMode.Right);
                }
                else
                {
                    var width = tt.X + x;
                    if (Math.Abs(width) > maxThreshold)
                    {
                        width = -maxThreshold;
                        tt.X = width <= 0 ? width : 0;
                        PivotItem.RenderTransform = tt;
                        HandleCrossSlide(CrossSlideMode.Right);
                        return;
                    }

                    tt.X = width <= 0 ? width : 0;
                    PivotItem.RenderTransform = tt;
                }
            }
        }

        private void HandlePullToRefresh(double x, double y, ManipulationDeltaRoutedEventArgs e)
        {
            if (!AllowPullToRefresh)
            {
                return;
            }
            manipulationStatus = ManipulationStatus.PullToRefresh;
            var maxThreshold = RefreshThreshold * 4 / 3.0;
            //Y not support inertial
            if (e.IsInertial)
            {
                _contentGrid.ManipulationDelta -= _contentGrid_ManipulationDelta;
                _contentGrid_ManipulationCompleted(null, null);
                return;
            }
            if (_pullToRefreshHeader.Height <= maxThreshold && preDeltaTranslationY != y)
            {
                preDeltaTranslationY = y;
                var height = _pullToRefreshHeader.Height + y;
                if (height > maxThreshold)
                {
                    height = maxThreshold;
                }
                _pullToRefreshHeader.Height = height >= 0 ? height : 0;

                _contentGrid.RowDefinitions[Grid.GetRow(_pullToRefreshHeader)].Height = new GridLength(_pullToRefreshHeader.Height);
                //Debug.WriteLine(_pullToRefreshHeader.Height);
            }

            if (_pullToRefreshHeader.Height >= RefreshThreshold)
            {
                this.IsReachThreshold = true;
            }
            else
            {
                this.IsReachThreshold = false;
            }
            VisualStateManager.GoToState(this, "NoIndicator", true);
        }

        private void HandleScrolling(double x, double y, ManipulationDeltaRoutedEventArgs e)
        {
            if (preDeltaTranslationX == x && preDeltaTranslationY == y)
            {
                return;
            }
            manipulationStatus = ManipulationStatus.Scrolling;
            preDeltaTranslationX = x;
            preDeltaTranslationY = y;
            Point point = new Point();
            switch (ScollingDirectionMode)
            {
                case ScollingDirectionMode.TwoDirection:
                    point = new Point() { X = ScrollPosition.X + x, Y = ScrollPosition.Y + y };
                    break;
                case ScollingDirectionMode.OneDirection:
                    switch (scollingDirection)
                    {
                        case ScollingDirection.None:
                            if (Math.Abs(x) <= Math.Abs(y))
                            {
                                point = new Point() { X = ScrollPosition.X, Y = ScrollPosition.Y + y };
                                scollingDirection = ScollingDirection.Vertical;
                            }
                            else
                            {
                                point = new Point() { X = ScrollPosition.X + x, Y = ScrollPosition.Y };
                                scollingDirection = ScollingDirection.Horizontal;
                            }
                            break;
                        case ScollingDirection.Horizontal:
                            point = new Point() { X = ScrollPosition.X + x, Y = ScrollPosition.Y };
                            break;
                        case ScollingDirection.Vertical:
                            point = new Point() { X = ScrollPosition.X, Y = ScrollPosition.Y + y };
                            break;
                    }

                    break;
            }

            ScrollPosition = point;
            //HandleLoadMore(point);

            this.IsReachThreshold = false;
            if (e.PointerDeviceType == PointerDeviceType.Touch)
            {
                VisualStateManager.GoToState(this, "TouchIndicator", true);
            }
            else if (e.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(this, "MouseIndicator", true);
            }
        }

        private void DataGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Mouse)
            {
                if (e.HoldingState == HoldingState.Started)
                {
                    var pt = e.GetPosition(_cellPanel);
                    int row = GetRowFromPoint(pt);
                    _cellPanel.HandlePointerPressed(row);
                }
                //Completed when pointer release

                //Canceled when pointer move
                else
                {
                    _cellPanel.ClearPointerPressedAnimation();
                }

            }
        }
    }
}
