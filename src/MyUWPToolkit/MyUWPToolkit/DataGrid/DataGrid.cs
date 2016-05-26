using MyUWPToolkit.DataGrid.Model.Cell;
using MyUWPToolkit.DataGrid.Util;
using MyUWPToolkit.CollectionView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using System.Reflection;
using MyUWPToolkit.DataGrid.Model.RowCol;
using MyUWPToolkit.Util;
using Windows.Foundation;
using System.Globalization;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation.Collections;
using Windows.Devices.Input;
using System.Diagnostics;
using Windows.UI;
using MyUWPToolkit.Common;
using Windows.UI.Input;

namespace MyUWPToolkit.DataGrid
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "ContentGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "VerticalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "HorizontalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PullToRefreshHeader", Type = typeof(ContentControl))]
    public partial class DataGrid : Control
    {

        #region Ctor
        public DataGrid()
        {
            this.DefaultStyleKey = typeof(DataGrid);
            this.InitializePanel();
        }


        #endregion

        #region override method
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InitializeContentGrid();
            this.InitializeScrollBar();
            UpdateScrollBars();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (RefreshThreshold == 0.0)
            {
                RefreshThreshold = availableSize.Height * 1 / 5;
            }
            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region Private Methods

        #region ScrollBar

        private void InitializeScrollBar()
        {
            _verticalScrollBar = GetTemplateChild("VerticalScrollBar") as ScrollBar;
            _horizontalScrollBar = GetTemplateChild("HorizontalScrollBar") as ScrollBar;
            _verticalScrollBar.Scroll += ScrollBar_Scroll;
            _horizontalScrollBar.Scroll += ScrollBar_Scroll;
            _verticalScrollBar.PointerWheelChanged += _verticalScrollBar_PointerWheelChanged;
            _horizontalScrollBar.PointerWheelChanged += _horizontalScrollBar_PointerWheelChanged;
        }

        private void _horizontalScrollBar_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as ScrollBar);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var horizontalOffset = ScrollPosition.X + delta;
                ScrollPosition = new Point(horizontalOffset, ScrollPosition.Y);
            }
        }

        private void _verticalScrollBar_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as ScrollBar);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var verticalOffset = ScrollPosition.Y + delta;
                ScrollPosition = new Point(ScrollPosition.X, verticalOffset);
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // get new scroll position from bars
            var sp = new Point(-_horizontalScrollBar.Value, -_verticalScrollBar.Value);

            // scroll now
            ScrollPosition = sp;
        }

        void UpdateScrollBars()
        {
            if (_contentGrid != null && _horizontalScrollBar != null && _verticalScrollBar != null)
            {
                // calculate size available (client and scrollbars)
                var wid = _contentGrid.ActualWidth;
                var hei = _contentGrid.ActualHeight;
                var columnHeaderHeight = _columnHeaderPanel.ActualHeight;
                var frozenColumnsSize = Columns.GetFrozenSize();
                var frozenRowsSize = Rows.GetFrozenSize();

                if (!double.IsPositiveInfinity(wid) && !double.IsPositiveInfinity(hei))
                {
                    hei -= columnHeaderHeight;
                    _verticalScrollBar.Margin = new Thickness(0, columnHeaderHeight + frozenRowsSize, 0, 0);
                    _horizontalScrollBar.Margin = new Thickness(frozenColumnsSize, 0, 0, 0);
                    UpdateStarSizes();

                    // update scrollbar parameters
                    _verticalScrollBar.SmallChange = _horizontalScrollBar.SmallChange = Rows.DefaultSize;
                    _verticalScrollBar.LargeChange = _verticalScrollBar.ViewportSize = hei - columnHeaderHeight;
                    _horizontalScrollBar.LargeChange = _horizontalScrollBar.ViewportSize = wid;
                    _verticalScrollBar.Maximum = Rows.GetTotalSize() - hei;
                    _horizontalScrollBar.Maximum = Columns.GetTotalSize() - wid;

                    // update scrollbar visibility
                    if (_verticalScrollBar != null)
                    {

                        switch (VerticalScrollBarVisibility)
                        {

                            case ScrollBarVisibility.Auto:
                                _verticalScrollBar.Visibility = _verticalScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;
                                break;
                            case ScrollBarVisibility.Disabled:
                            case ScrollBarVisibility.Hidden:
                                _verticalScrollBar.Visibility = Visibility.Collapsed;
                                break;
                            case ScrollBarVisibility.Visible:
                                _verticalScrollBar.Visibility = Visibility.Visible;
                                break;
                            default:
                                break;
                        }

                    }
                    if (_horizontalScrollBar != null)
                    {
                        switch (HorizontalScrollBarVisibility)
                        {

                            case ScrollBarVisibility.Auto:
                                _horizontalScrollBar.Visibility = _horizontalScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;
                                break;
                            case ScrollBarVisibility.Disabled:
                            case ScrollBarVisibility.Hidden:
                                _horizontalScrollBar.Visibility = Visibility.Collapsed;
                                break;
                            case ScrollBarVisibility.Visible:
                                _horizontalScrollBar.Visibility = Visibility.Visible;
                                break;
                            default:
                                break;
                        }

                    }

                    // make sure current scroll position is valid
                    ScrollPosition = ScrollPosition;
                    if (_view != null && _verticalScrollBar != null)
                    {
                        if (-ScrollPosition.Y >= _verticalScrollBar.Maximum && _verticalScrollBar.Maximum >= 0)
                        {
                            if (_view.HasMoreItems && !_isLoadingMoreItems)
                            {
                                _isLoadingMoreItems = true;
                                var firstRow = Math.Max(0, Math.Min(Rows.Count - 1, Rows.GetItemAt(_verticalScrollBar.Value)));
                                var lastRow = Math.Max(-1, Math.Min(Rows.Count - 1, Rows.GetItemAt(_verticalScrollBar.Value + _cellPanel.ActualHeight)));
                                uint count = Math.Max(1, (uint)(lastRow - firstRow));
                                //uint count = Math.Max(1, (uint)(10));
                                if (count == uint.MaxValue)
                                {
                                    count = (uint)((this.ActualHeight - this._columnHeaderPanel.ActualHeight) / Rows.DefaultSize + 0.5);
                                }
                                _view.LoadMoreItemsAsync(count).AsTask().ContinueWith(t =>
                                {
                                    _isLoadingMoreItems = false;
                                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

                            }
                        }
                    }
                }
            }

        }
        #endregion

        #region ContentGrid
        private void InitializeContentGrid()
        {
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            _contentGrid.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;
            _contentGrid.ManipulationCompleted += _contentGrid_ManipulationCompleted;
            _contentGrid.ManipulationStarting += _contentGrid_ManipulationStarting;
            if (!PlatformIndependent.IsWindowsPhoneDevice)
            {
                _contentGrid.PointerWheelChanged += _contentGrid_PointerWheelChanged;
                this.PointerEntered += OnPointerEntered;
                this.PointerExited += OnPointerExited;
            }

            _pullToRefreshHeader = GetTemplateChild("PullToRefreshHeader") as ContentControl;
            _pullToRefreshHeader.DataContext = this;

            _cellPanel.SetValue(Grid.RowProperty, 2);
            _columnHeaderPanel.SetValue(Grid.RowProperty, 0);
            _cellPanel.LayoutUpdated += _cellPanel_LayoutUpdated;
            _columnHeaderPanel.Tapped += _columnHeaderPanel_Tapped;
            _cellPanel.Tapped += _cellPanel_Tapped;

            _canvas.SetValue(Grid.RowSpanProperty, 3);
            _contentGrid.Children.Add(_cellPanel);
            _contentGrid.Children.Add(_columnHeaderPanel);
            _contentGrid.Children.Add(_canvas);

            int sz = (int)(FontSize * 1.6 + 4);
            if (Rows.DefaultSize == 0)
            {
                Rows.DefaultSize = sz;
            }
            if (ColumnHeaders.Rows.DefaultSize == 0)
            {
                ColumnHeaders.Rows.DefaultSize = sz;
            }

        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "NoIndicator", true);
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "MouseIndicator", true);
        }

        private void _contentGrid_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as Grid);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var verticalOffset = ScrollPosition.Y + delta;
                ScrollPosition = new Point(ScrollPosition.X, verticalOffset);
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

            VisualStateManager.GoToState(this, "NoIndicator", true);
            _pullToRefreshHeader.Height = 0;
            _contentGrid.RowDefinitions[1].Height = new GridLength(0);

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
            preDeltaTranslationX = 0;
            preDeltaTranslationY = 0;

            _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;

        }

        private void _contentGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
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
            else if (((_pullToRefreshHeader.Height == 0 && y > 0) || _pullToRefreshHeader.Height > 0) && Math.Abs(x) < Math.Abs(y) && _verticalScrollBar.Value == 0 && startingPullToRefresh)
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
                _contentGrid.RowDefinitions[1].Height = new GridLength(_pullToRefreshHeader.Height);
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
            var point = new Point() { X = ScrollPosition.X + x, Y = ScrollPosition.Y + y };

            ScrollPosition = point;

            if (_view != null && _verticalScrollBar != null)
            {
                if (-point.Y >= _verticalScrollBar.Maximum && _verticalScrollBar.Maximum > 0)
                {
                    if (_view.HasMoreItems && !_isLoadingMoreItems)
                    {
                        _isLoadingMoreItems = true;

                        var firstRow = Math.Max(0, Math.Min(Rows.Count - 1, Rows.GetItemAt(_verticalScrollBar.Value)));
                        var lastRow = Math.Max(-1, Math.Min(Rows.Count - 1, Rows.GetItemAt(_verticalScrollBar.Value + _cellPanel.ActualHeight)));
                        uint count = Math.Max(1, (uint)(lastRow - firstRow));

                        _view.LoadMoreItemsAsync(count).AsTask().ContinueWith(t =>
                        {
                            _isLoadingMoreItems = false;
                        }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

                    }
                }
            }

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

        private void HandleCrossSlide(CrossSlideMode mode)
        {

            _contentGrid.ManipulationDelta -= _contentGrid_ManipulationDelta;
            if (PivotItem != null)
            {
                //PivotItem.Margin = _defaultPivotItemMargin;
                PivotItem.RenderTransform = new TranslateTransform();
                var pivot = PivotItem.Parent as Pivot;
                //Manipulation Is Inertial
                //when swip quickly, may it will change index manytime, we only handle the selectindex is 
                //PivotItem index
                if (pivot != null && pivot.SelectedIndex == pivot.IndexFromContainer(PivotItem))
                {
                    this.Visibility = Visibility.Collapsed;
                    var index = pivot.SelectedIndex;
                    if (mode == CrossSlideMode.Left)
                    {
                        index = index - 1;
                        if (index < 0)
                        {
                            index = pivot.Items.Count - 1;
                        }
                    }
                    else
                    {
                        index = index + 1;
                        if (index > pivot.Items.Count - 1)
                        {
                            index = 0;
                        }
                    }
                    pivot.SelectedIndex = index;
                    this.Visibility = Visibility.Visible;
                }
            }

        }

        #endregion

        #region GridPanel
        private void InitializePanel()
        {
            _cellPanel = new DataGridPanel(this, CellType.Cell, Consts.ROWHEIGHT, Consts.COLUMNWIDTH);

            _columnHeaderPanel = new DataGridPanel(this, CellType.ColumnHeader, Consts.ROWHEIGHT, Consts.COLUMNWIDTH);
            _columnHeaderPanel.Columns = _cellPanel.Columns;

            _columnHeaderPanel.Rows.Add(new Row());
            #region frozen
            _canvas = new Canvas();
            _lnFX = new Line();
            _lnFX.Visibility = Visibility.Collapsed;
            _lnFX.StrokeThickness = 1;
            _canvas.Children.Add(_lnFX);

            _lnFY = new Line();
            _lnFY.Visibility = Visibility.Collapsed;
            _lnFY.StrokeThickness = 1;
            _canvas.Children.Add(_lnFY);
            #endregion

            #region More Columns

            #endregion

        }

        private void _cellPanel_LayoutUpdated(object sender, object e)
        {
            UpdateScrollBars();

            // clip canvas
            var g = new RectangleGeometry();
            _canvas.Clip = g;
            g.Rect = new Rect(0, 0, _canvas.ActualWidth, _canvas.ActualHeight);

            // update frozen row, column indicators
            var hdrX = 0;
            var hdrY = ColumnHeaders.Visibility == Visibility.Visible ? ColumnHeaders.ActualHeight : 0;
            if (Columns.Frozen > 0 && ShowFrozenLines)
            {
                var fx = Columns.GetFrozenSize();
                _lnFX.X1 = _lnFX.X2 = fx;
                _lnFX.Y2 = Math.Min(10000, Cells.ActualHeight + hdrY);
                _lnFX.Stroke = FrozenLinesBrush;
                _lnFX.Visibility = Visibility.Visible;
            }
            else
            {
                _lnFX.Visibility = Visibility.Collapsed;
            }
            if (Rows.Frozen > 0 && ShowFrozenLines)
            {
                var fy = Rows.GetFrozenSize() + hdrY;
                _lnFY.Y1 = _lnFY.Y2 = fy;
                _lnFY.X2 = Math.Min(10000, Cells.ActualWidth + hdrX);
                _lnFY.Stroke = FrozenLinesBrush;
                _lnFY.Visibility = Visibility.Visible;
            }
            else
            {
                _lnFY.Visibility = Visibility.Collapsed;
            }
        }
        private void _cellPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pt = e.GetPosition(_cellPanel);
            pt = this.TransformToVisual(_cellPanel).TransformPoint(pt);
            var fy = _cellPanel.Rows.GetFrozenSize();
            pt.Y += _columnHeaderPanel.ActualHeight;
            var sp = _cellPanel.ScrollPosition;
            if (pt.Y < 0 || pt.Y > fy) pt.Y -= sp.Y;
            // get row and column at given coordinates
            var row = _cellPanel.Rows.GetItemAt(pt.Y);
            if (ItemClick != null && row > -1)
            {
                var args = new ItemClickEventArgs(this.Rows[row]);
                ItemClick(this, args);
            }
        }

        private void _columnHeaderPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pt = e.GetPosition(_columnHeaderPanel);
            pt = this.TransformToVisual(_columnHeaderPanel).TransformPoint(pt);

            var fx = _columnHeaderPanel.Columns.GetFrozenSize();
            // adjust for scroll position
            var sp = _columnHeaderPanel.ScrollPosition;
            if (pt.X < 0 || pt.X > fx) pt.X -= sp.X;
            var column = _columnHeaderPanel.Columns.GetItemAt(pt.X);
            HandleSort(column);
        }

        void HandleSort(int column)
        {
            if (column > -1 && this.AllowSorting && this.Columns[column].AllowSorting)
            {

                // get view
                var view = _view as ICollectionViewEx;

                // get column to sort
                var col = this.Columns[column];
                var direction = ListSortDirection.Ascending;
                var sds = view.SortDescriptions;
                var args = new SortingColumnEventArgs(this.Columns[column]);
                if (this.SortingColumn != null)
                {
                    SortingColumn(this, args);
                }
                // get property to sort on
                var pn = col.BoundPropertyName;
                if (!string.IsNullOrEmpty(pn))
                {
                    if (!args.Cancel)
                    {
                        // apply new sort
                        try
                        {
                            // if already sorted, reverse direction
                            foreach (var sd in sds)
                            {
                                if (sd.PropertyName == pn && sd.Direction == ListSortDirection.Ascending)
                                {
                                    direction = ListSortDirection.Descending;
                                    break;
                                }
                            }
                            using (view.DeferRefresh())
                            {
                                sds.Clear();
                                sds.Add(new SortDescription(pn, direction));
                            }
                        }
                        catch
                        {
                        }

                    }
                    else
                    {
                        if (this.SortMode == SortMode.Manual)
                        {
                            // if already sorted, reverse direction
                            foreach (var sd in ManualSortDescriptions)
                            {
                                if (sd.PropertyName == pn && sd.Direction == ListSortDirection.Ascending)
                                {
                                    direction = ListSortDirection.Descending;
                                    break;
                                }
                            }
                            ManualSortDescriptions.Clear();

                            ManualSortDescriptions.Add(new SortDescription(pn, direction));
                            this.Invalidate();
                        }
                    }
                }
                else
                {
                    if (this.SortMode == SortMode.Manual)
                    {
                        var name = this.Columns[column].ColumnName + this.Columns[column].ItemIndex;
                        // if already sorted, reverse direction
                        foreach (var sd in ManualSortDescriptions)
                        {
                            if (sd.PropertyName == name && sd.Direction == ListSortDirection.Ascending)
                            {
                                direction = ListSortDirection.Descending;
                                break;
                            }
                        }
                        ManualSortDescriptions.Clear();

                        ManualSortDescriptions.Add(new SortDescription(name, direction));
                        this.Invalidate();
                    }
                }
            }
        }
        #endregion

        #region DP
        private void OnItemsSourceChanged()
        {
            _manualSort.Clear();
            ScrollPosition = new Point(ScrollPosition.X, 0);
            if (_view != null)
            {
                _view.VectorChanged -= _view_VectorChanged;
            }
            _view = ItemsSource as ICollectionView;

            _props = null;
            _itemType = null;
            if (_view == null && ItemsSource != null)
            {
                _view = new UWPCollectionView(ItemsSource);
            }

            // remove old rows, auto-generated columns
            Rows.Clear();
            ClearAutoGeneratedColumns();

            // bind grid to new data source
            if (_view != null)
            {
                // connect event handlers
                _view.VectorChanged += _view_VectorChanged;
                // get list of properties available for binding
                _props = GetItemProperties();

                // just in case GetItemProperties changed something
                ClearAutoGeneratedColumns();

                //auto - generate columns
                if (AutoGenerateColumns)
                {
                    using (Columns.DeferNotifications())
                    {
                        GenerateColumns(_props);
                    }
                }

                // initialize non-auto-generated column bindings
                foreach (var col in Columns)
                {
                    if (!col.AutoGenerated)
                    {
                        BindColumn(col);
                    }
                }

                // load rows
                LoadRows();

            }
        }


        #endregion

        #region View
        private void _view_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs e)
        {
            if (_props == null || _props.Count == 0 || _itemType != GetItemType(_view))
            {
                if (_itemType == null || !MyUWPToolkit.Util.Util.IsPrimitive(_itemType))
                {
                    if (GetItemType(_view) != typeof(object))
                    {
                        OnItemsSourceChanged();
                        return;
                    }
                }
            }

            // handle the collection change
            OnViewChanged(sender, e);

        }

        internal void OnViewChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            // handle action
            var rows = Rows;
            var index = (int)e.Index;
            var rowIndex = GetRowIndex(index);
            switch (e.CollectionChange)
            {
                case CollectionChange.ItemInserted:

                    // create the new bound row
                    var r = CreateBoundRow(sender[index]);

                    if (rowIndex < 0)
                    {
                        rowIndex = rows.Count;
                    }

                    // add the new bound row to the rows collection
                    if (rowIndex > -1)
                    {
                        rows.Insert(rowIndex, r);
                    }
                    else
                    {
                        LoadRows();
                    }
                    break;

                case CollectionChange.ItemRemoved:
                    if (rowIndex > -1)
                    {
                        rows.RemoveAt(rowIndex);
                    }
                    else
                    {
                        LoadRows();
                    }
                    break;

                case CollectionChange.ItemChanged:
                    rows[rowIndex].DataItem = sender[index];
                    _cellPanel.Invalidate(new CellRange(rowIndex, 0, rowIndex, Columns.Count - 1));
                    break;

                default: // Reset, Move
                    LoadRows();
                    break;
            }

            // ensure scrollbars are in sync
            InvalidateArrange();
        }

        internal int GetRowIndex(int dataIndex)
        {
            if (dataIndex > -1)
            {
                // update DataIndex members
                Rows.Update();

                // look for the row with the right DataIndex
                for (int rowIndex = dataIndex; rowIndex < Rows.Count; rowIndex++)
                {
                    if (Rows[rowIndex].DataIndex == dataIndex)
                        return rowIndex;
                }
            }

            // not found
            return -1;
        }
        #endregion

        #region Columns
        private void ClearAutoGeneratedColumns()
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].AutoGenerated)
                {
                    Columns.RemoveAt(i);
                    i--;
                }
            }
        }

        void GenerateColumns(Dictionary<string, PropertyInfo> props)
        {
            if (props.Count == 0 && _view != null)
            {
                // special case: binding directly to primitive types (int, string, etc)
                var type = GetItemType(_view);
                if (MyUWPToolkit.Util.Util.IsPrimitive(type))
                {
                    var col = new Column();
                    BindAutoColumn(col, type);
                    col.AutoGenerated = true;
                    Columns.Add(col);
                }
            }
            else
            {
                // generate one column for each property
                foreach (var cpi in props.Values)
                {
                    var col = new Column();
                    BindAutoColumn(col, cpi);
                    col.AutoGenerated = true;
                    Columns.Add(col);
                }
            }
        }

        void BindAutoColumn(Column c, PropertyInfo cpi)
        {
            // create automatic binding (property name may be enclosed in square brackets)
            var name = cpi.Name;
            List<char> specialChar = new List<char>() { '/', '.' };
            bool containsSpecialChar = specialChar.Any(letter => name.Contains(letter));
            if (name != null && name.StartsWith("[") && name.EndsWith("]") && !containsSpecialChar)
            {
                name = name.Substring(1, name.Length - 2);
            }
            var b = new Binding { Path = new PropertyPath(name) };
            b.Mode = !cpi.CanWrite ? BindingMode.OneWay : BindingMode.TwoWay;

            // assign name, binding, property info to column (pi after b!!!!)
            c.ColumnName = cpi.Name;
            c.Binding = b;
            c.PropertyInfo = cpi;

            // initialize alignment, format, etc
            InitializeAutoColumn(c, cpi.PropertyType);
        }

        void BindAutoColumn(Column c, Type type)
        {
            c.Header = type != null ? type.Name : null;
            var b = new Binding();
            if (c.BoundPropertyName == null)
            {
                b.Mode = BindingMode.OneWay;
            }
            c.Binding = b;

            InitializeAutoColumn(c, type);
        }

        // initialize auto column properties based on data type
        void InitializeAutoColumn(Column c, Type type)
        {
            // save column type
            c.DataType = type;

            // handle nullable types
            type = type.GetNonNullableType();

            // initialize column properties based on type
            if (type == typeof(string))
            {
                c.Width = new GridLength(180);
            }
            else if (type.IsNumericIntegral())
            {
                c.Width = new GridLength(80);
                c.Format = "n0";
            }
            else if (type.IsNumericNonIntegral())
            {
                c.Width = new GridLength(80);
                c.Format = "n2";
            }
            else if (type == typeof(bool))
            {
                c.Width = new GridLength(60);
            }
            else if (type == typeof(DateTime))
            {
                c.Format = "d";
            }
        }

        // bind custom column (non-auto generated)
        internal void BindColumn(Column c)
        {
            var b = c.Binding;
            if (b != null)
            {
                // get path from binding (may be null)
                var path = b.Path != null ? b.Path.Path : string.Empty;

                // get PropertyInfo from binding
                PropertyInfo cpi = null;
                if (_props != null && b.Path != null && _props.TryGetValue(path, out cpi))
                {
                    c.PropertyInfo = cpi;
                    if (c.PropertyInfo == null && (c.DataType == null || c.DataType == typeof(object)))
                    {
                        c.DataType = cpi.PropertyType;
                    }
                }

                // set column name if empty
                if (string.IsNullOrEmpty(c.ColumnName))
                {
                    c.ColumnName = cpi != null ? cpi.Name : path;
                }
            }
        }
        #endregion

        #region Rows
        void LoadRows()
        {
            if (_view != null)
            {
                using (Rows.DeferNotifications())
                {
                    // add all data items
                    Rows.Clear();
                    CreateBoundRows();
                }
                // show new data and sorting order
                Invalidate();
            }
        }

        private void CreateBoundRows()
        {
            if (_view != null)
            {

                int count = _view.Count;
                for (int i = 0; i < _view.Count; i++)
                {
                    var item = _view[i];

                    var r = CreateBoundRow(item);
                    Rows.Add(r);
                }

            }
        }

        private Row CreateBoundRow(object dataItem)
        {
            return new BoundRow(dataItem);
        }
        #endregion

        #region Common
        private Dictionary<string, PropertyInfo> GetItemProperties()
        {
            var props = new Dictionary<string, PropertyInfo>();
            if (_view != null)
            {
                // get item type
                _itemType = GetItemType(_view);

                if (_itemType != null && !MyUWPToolkit.Util.Util.IsPrimitive(_itemType))
                {

                    foreach (var pi in _itemType.GetRuntimeProperties())
                    {
                        // skip indexed properties
                        var ix = pi.GetIndexParameters();
                        if (ix != null && ix.Length > 0)
                        {
                            continue;
                        }

                        // keep this one
                        props[pi.Name] = pi;
                    }
                }
            }

            return props;
        }

        private Type GetItemType(ICollectionView view)
        {
            if (view != null)
            {
                // get type from current item
                if (view.CurrentItem != null)
                {
                    return view.CurrentItem.GetType();
                }

                // get type from *any* item
                foreach (var item in view)
                {
                    if (item != null)
                    {
                        return item.GetType();
                    }
                }
            }

            return null;
        }


        internal void Invalidate()
        {
            if (_contentGrid != null)
            {
                // invalidate cells
                _columnHeaderPanel.Invalidate();
                _cellPanel.Invalidate();
            }
        }

        internal void DisposeCell(DataGridPanel panel, FrameworkElement cell)
        {
            var cf = GetCellFactory();
            cf.DisposeCell(this, panel.CellType, cell);
        }

        private ICellFactory GetCellFactory()
        {
            return _cellFactory == null ? _defautlCellFactory : _cellFactory;
        }

        internal FrameworkElement CreateCell(DataGridPanel panel, CellRange rng)
        {
            var cf = GetCellFactory();
            return cf.CreateCell(this, panel.CellType, rng);
        }

        internal bool TryChangeType(ref object value, Type type)
        {
            if (type != null && type != typeof(object))
            {
                // handle nullable types
                if (type.IsNullableType())
                {
                    // if value is null, we're done
                    if (value == null || object.Equals(value, string.Empty))
                    {
                        value = null;
                        return true;
                    }

                    // get actual type for parsing
                    type = Nullable.GetUnderlyingType(type);
                }
                else if (type.GetTypeInfo().IsValueType && value == null)
                {
                    // not nullable, can't assign null value to this
                    return false;
                }

                // handle special numeric formatting
                var ci = GetCultureInfo();
                var str = value as string;
                if (!string.IsNullOrEmpty(str) && type.IsNumeric())
                {
                    // handle percentages (ci.NumberFormat.PercentSymbol? overkill...)
                    bool pct = str[0] == '%' || str[str.Length - 1] == '%';
                    if (pct)
                    {
                        str = str.Trim('%');
                    }
                    decimal d;
                    if (decimal.TryParse(str, NumberStyles.Any, ci, out d))
                    {
                        if (pct)
                        {
                            value = d / 100;
                        }
                        else
                        {
                            // <<IP>> for currencies Convert.ChangeType will always give exception if we do without parsing,
                            // so change value here to the parsed one
                            value = d;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // ready to change type
                try
                {
                    value = Convert.ChangeType(value, type, ci);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        internal CultureInfo GetCultureInfo()
        {
            // refresh culture info based on this.Language (as in Bindings)
            if (_ci == null || _lang != this.Language)
            {
                _lang = this.Language;
                _ci = CultureInfo.CurrentCulture;
            }

            // done
            return _ci;
        }

        internal void UpdateStarSizes()
        {
            if (_contentGrid != null && Columns != null && Columns.Grid == this)
            {
                var width = _contentGrid.ActualWidth;
                Columns.UpdateStarSizes(width);
            }
        }

        internal ListSortDirection? GetColumnSort(int col)
        {
            var view = _view as ICollectionViewEx;
            if (view != null)
            {
                var colName = Columns[col].BoundPropertyName;
                foreach (var sd in view.SortDescriptions)
                {
                    if (sd.PropertyName == colName)
                    {
                        return sd.Direction;
                    }
                }
            }
            if (SortMode == SortMode.Manual)
            {
                var colName = Columns[col].BoundPropertyName;
                var name = Columns[col].ColumnName + this.Columns[col].ItemIndex;
                foreach (var sd in ManualSortDescriptions)
                {
                    if (sd.PropertyName == colName || sd.PropertyName == name)
                    {
                        return sd.Direction;
                    }
                }
            }
            return null;
        }
        #endregion

        #endregion

        #region public Methods
        public IEnumerable<object> GetVisibleItems()
        {
            if (FrozenRows > 0)
            {
                for (int i = 0; i < FrozenRows; i++)
                {
                    yield return _view[i];
                }
            }

            if (_cellPanel != null && _view != null)
            {
                var viewRange = _cellPanel.ViewRange;

                for (int i = viewRange.Row; i <= viewRange.Row2; i++)
                {
                    yield return _view[i];
                }
            }
            else
            {
                yield return null;
            }
        }
        #endregion
    }

}