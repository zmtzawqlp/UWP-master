using MyUWPToolkit.Common;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace MyUWPToolkit.FlexGrid
{
    [TemplatePart(Name = "ContentGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "FrozenColumnsHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "FrozenColumnsCell", Type = typeof(ListView))]
    [TemplatePart(Name = "ColumnsHeader", Type = typeof(ListView))]
    [TemplatePart(Name = "Cell", Type = typeof(ListView))]
    [TemplatePart(Name = "PullToRefreshHeader", Type = typeof(ContentControl))]
    public partial class FlexGrid : Control
    {
        #region ctor
        public FlexGrid()
        {
            this.DefaultStyleKey = typeof(FlexGrid);

        }
        #endregion

        #region override method
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitializeContentGrid();
            InitializeFrozenColumnsHeader();
            InitializeFrozenColumnsCell();
            InitializeColumnsHeader();
            InitializeCell();
            InitializePullToRefreshHeader();
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            if (RefreshThreshold==0.0)
            {
                RefreshThreshold= RefreshThreshold = availableSize.Height * 1 / 5;
            }
            return base.MeasureOverride(availableSize);
        }

        #endregion


        #region private method

        #region ContentGrid
        private void InitializeContentGrid()
        {
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            _contentGrid.ManipulationDelta += _contentGrid_ManipulationDelta;
            _contentGrid.ManipulationCompleted += _contentGrid_ManipulationCompleted;
            _contentGrid.ManipulationStarting += _contentGrid_ManipulationStarting;
        }

        private void _contentGrid_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            startingPullToRefresh = false;
            startingCrossSlideLeft = false;
            startingCrossSlideRight = false;

            if (_cellSV.VerticalOffset == 0)
            {
                startingPullToRefresh = true;
            }

            //Cross Slide left
            if (_cellSV.HorizontalOffset == 0)
            {
                startingCrossSlideLeft = true;
            }
            //double
            if (Math.Abs(_cellSV.HorizontalOffset - _cellSV.ScrollableWidth) < 0.0001)
            {
                startingCrossSlideRight = true;
            }
        }

        private void _contentGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
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
            else if (((_pullToRefreshHeader.Height == 0 && y > 0) || _pullToRefreshHeader.Height > 0) && Math.Abs(x) < Math.Abs(y) && _cellSV.VerticalOffset == 0 && startingPullToRefresh)
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
            //var point = new Point() { X = ScrollPosition.X + x, Y = ScrollPosition.Y + y };
            var horizontalOffset = _cellSV.HorizontalOffset - x;
            var verticalOffset = _cellSV.VerticalOffset - y;

            _cellSV.ChangeView(horizontalOffset, verticalOffset, null);
            _frozenColumnsCellSV.ChangeView(null, verticalOffset, null);
            _columnsHeaderSV.ChangeView(horizontalOffset, null, null);
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

        #region Cell
        private void InitializeCell()
        {
            _cell = GetTemplateChild("Cell") as ListView;
            _cell.Loaded += _cell_Loaded;
            _cell.ItemClick += OnItemClick;
        }

        private void _cell_Loaded(object sender, RoutedEventArgs e)
        {
            _cell.Loaded -= _cell_Loaded;
            _cellSV = _cell.GetFirstChildOfType<ScrollViewer>();
        }


        #endregion

        #region ColumnsHeader

        private void InitializeColumnsHeader()
        {
            _columnsHeader = GetTemplateChild("ColumnsHeader") as ListView;
            _columnsHeader.Loaded += _columnsHeader_Loaded;
            _columnsHeader.ItemClick += OnColumnSorting;    
        }

      

        private void _columnsHeader_Loaded(object sender, RoutedEventArgs e)
        {
            _columnsHeader.Loaded -= _columnsHeader_Loaded;
            _columnsHeaderSV = _columnsHeader.GetFirstChildOfType<ScrollViewer>();
        }

        #endregion

        #region FrozenColumnsCell
        private void InitializeFrozenColumnsCell()
        {
            _frozenColumnsCell = GetTemplateChild("FrozenColumnsCell") as ListView;
            _frozenColumnsCell.Loaded += _frozenColumnsCell_Loaded;
            _frozenColumnsCell.ItemClick += OnItemClick;
        }

        private void _frozenColumnsCell_Loaded(object sender, RoutedEventArgs e)
        {
            _frozenColumnsCell.Loaded -= _frozenColumnsCell_Loaded;
            _frozenColumnsCellSV = _frozenColumnsCell.GetFirstChildOfType<ScrollViewer>();
        }

        #endregion

        #region FrozenColumnsHeader
        private void InitializeFrozenColumnsHeader()
        {
            _frozenColumnsHeader = GetTemplateChild("FrozenColumnsHeader") as ListView;
            _frozenColumnsHeader.ItemClick += OnColumnSorting;
        }

        #endregion

        #region PullToRefreshHeader

        private void InitializePullToRefreshHeader()
        {
            _pullToRefreshHeader = GetTemplateChild("PullToRefreshHeader") as ContentControl;
            _pullToRefreshHeader.DataContext = this;
        }
        #endregion

        #region DP
        private void OnItemsSourceChanged()
        {
            //_manualSort.Clear();
            //ScrollPosition = new Point(0, 0);
            //if (_view != null)
            //{
            //    _view.VectorChanged -= _view_VectorChanged;
            //}
            //_view = ItemsSource as ICollectionView;

            //_props = null;
            //_itemType = null;
            //if (_view == null && ItemsSource != null)
            //{
            //    _view = new UWPCollectionView(ItemsSource);
            //}

            //// remove old rows, auto-generated columns
            //Rows.Clear();
            //ClearAutoGeneratedColumns();

            //// bind grid to new data source
            //if (_view != null)
            //{
            //    // connect event handlers
            //    _view.VectorChanged += _view_VectorChanged;
            //    // get list of properties available for binding
            //    _props = GetItemProperties();

            //    // just in case GetItemProperties changed something
            //    ClearAutoGeneratedColumns();

            //    //auto - generate columns
            //    if (AutoGenerateColumns)
            //    {
            //        using (Columns.DeferNotifications())
            //        {
            //            GenerateColumns(_props);
            //        }
            //    }

            //    // initialize non-auto-generated column bindings
            //    foreach (var col in Columns)
            //    {
            //        if (!col.AutoGenerated)
            //        {
            //            BindColumn(col);
            //        }
            //    }

            //    // load rows
            //    LoadRows();

            //}
        }
        #endregion

        #region Common
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.ItemClick!=null)
            {
                this.ItemClick(this, e);
            }
        }
        private void OnColumnSorting(object sender, ItemClickEventArgs e)
        {
            if (this.SortingColumn!=null)
            {
                this.SortingColumn(this, new SortingColumnEventArgs(e.ClickedItem));
            }
        }
        #endregion
        #endregion

    }
}
