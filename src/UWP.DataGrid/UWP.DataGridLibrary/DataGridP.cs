using UWP.DataGrid.CollectionView;
using UWP.DataGrid.Common;
using UWP.DataGrid.Model.Cell;
using UWP.DataGrid.Model.RowCol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWP.DataGrid
{
    public partial class DataGrid : Control
    {
        #region fields
        //ScrollViewer _scrollViewer;
        Grid _contentGrid;
        ScrollBar _horizontalScrollBar;
        ScrollBar _verticalScrollBar;
        DataGridPanel _columnHeaderPanel;
        DataGridPanel _cellPanel;
        ContentControl _pullToRefreshHeader;
        ContentPresenter _header;
        ContentPresenter _footer;

        ICollectionView _view;
        ICellFactory _cellFactory;
        Dictionary<string, PropertyInfo> _props;
        Type _itemType;
        ICellFactory _defautlCellFactory = new CellFactory();
        private bool _isLoadingMoreItems;
        ObservableCollection<SortDescription> _manualSort = new ObservableCollection<SortDescription>();
        Canvas _canvas;
        Line _lnFX, _lnFY;
        CultureInfo _ci;
        string _lang;
        bool startingPullToRefresh = false;
        bool startingCrossSlideLeft = false;
        bool startingCrossSlideRight = false;
        double preDeltaTranslationX;
        double preDeltaTranslationY;
        ManipulationStatus manipulationStatus;
        ScollingDirection scollingDirection;
        Point? pointerOverPoint;
        double _headerHeight = 0;
        double _footerHeight = 0;
        #endregion

        #region Internal Properties

        internal IList<SortDescription> ManualSortDescriptions { get { return _manualSort; } }

        bool firstTimeTrytoFindPivotItem = true;
        bool firstTimeTrytoFindScrollViewer = true;
        PivotItem _pivotItem;
        internal PivotItem PivotItem
        {
            get
            {
                if (_pivotItem == null && firstTimeTrytoFindPivotItem)
                {
                    firstTimeTrytoFindPivotItem = false;
                    var parent = this.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent is Page)
                        {
                            break;
                        }
                        _pivotItem = parent as PivotItem;
                        if (_pivotItem != null)
                        {
                            break;
                        }
                        parent = parent.Parent as FrameworkElement;
                    }

                }
                return _pivotItem;
            }
        }

        ScrollViewer _outerScrollViewer;
        internal ScrollViewer OuterScrollViewer
        {
            get
            {
                if (_outerScrollViewer == null && firstTimeTrytoFindScrollViewer)
                {
                    firstTimeTrytoFindScrollViewer = false;
                    var parent = this.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent is Page)
                        {
                            break;
                        }
                        _outerScrollViewer = parent as ScrollViewer;
                        if (_outerScrollViewer != null)
                        {
                            break;
                        }
                        parent = parent.Parent as FrameworkElement;
                    }

                }
                return _outerScrollViewer;
            }

        }

        internal TranslateTransform PivotItemTT
        {
            get
            {
                if (PivotItem != null)
                {
                    var tt = PivotItem.RenderTransform as TranslateTransform;
                    if (tt == null)
                    {
                        tt = new TranslateTransform();
                    }
                    return tt;
                }
                else
                {
                    return new TranslateTransform();
                }
            }
        }
        #endregion

        #region Public Properties

        public SortMode SortMode { get; set; }

        public Rows Rows
        {
            get { return _cellPanel.Rows; }
        }
        public Columns Columns
        {
            get { return _cellPanel.Columns; }
        }
        public ICellFactory CellFactory
        {
            get { return _cellFactory; }
            set
            {
                if (_cellFactory != value)
                {
                    _cellFactory = value;
                    Invalidate();
                }
            }
        }

        public int FrozenColumns
        {
            get { return Columns.Frozen; }
            set { Columns.Frozen = value; }
        }

        public int FrozenRows
        {
            get { return Rows.Frozen; }
            set { Rows.Frozen = value; }
        }

        public DataGridPanel Cells
        {
            get { return _cellPanel; }
        }

        public DataGridPanel ColumnHeaders
        {
            get { return _columnHeaderPanel; }
        }
        private Point _scrollPosition;

        public Point ScrollPosition
        {
            get
            {
                return _scrollPosition;
                //return _cellPanel.ScrollPosition;
            }
            set
            {
                if (_contentGrid != null)
                {
                    var wid = _contentGrid.ActualWidth;
                    var hei = _contentGrid.ActualHeight;

                    //if (!double.IsPositiveInfinity(wid) && !double.IsPositiveInfinity(hei))
                    {
                        //viewPort
                        var sz = new Size(wid, hei);
                        sz = _contentGrid.DesiredSize;
                        //total size
                        var totalRowsSize = Rows.GetTotalSize();
                        var totalColumnsSize = Columns.GetTotalSize();
                        var totalHeight = totalRowsSize + _headerHeight + _footerHeight + _columnHeaderPanel.DesiredSize.Height;

                        var maxV = totalHeight - sz.Height;
                        maxV = maxV >= 0 ? maxV : 0;
                        var maxH = totalColumnsSize - sz.Width;

                        var totalScrollPosition = new Point();
                        totalScrollPosition.X = Math.Max(-maxH, Math.Min(value.X, 0));
                        totalScrollPosition.Y = Math.Max(-maxV, Math.Min(value.Y, 0));

                        if (_scrollPosition != totalScrollPosition)
                        {
                            _scrollPosition = totalScrollPosition;
                            if (_horizontalScrollBar != null && _verticalScrollBar != null)
                            {
                                _horizontalScrollBar.Value = -totalScrollPosition.X;
                                _verticalScrollBar.Value = -totalScrollPosition.Y;
                            }

                            //_header.Measure(_contentGrid.DesiredSize);
                            if (totalScrollPosition.Y <= 0)
                            {
                                var headerHeight = _headerHeight + totalScrollPosition.Y;
                                if (headerHeight > 0)
                                {
                                    _contentGrid.RowDefinitions[1].Height = new GridLength(_headerHeight + totalScrollPosition.Y);
                                    _header.Margin = new Thickness(0, totalScrollPosition.Y, 0, 0);
                                    _header.Clip = new RectangleGeometry() { Rect = new Rect(0, -totalScrollPosition.Y, _contentGrid.DesiredSize.Width, _headerHeight) };
                                }
                                else if (_contentGrid.RowDefinitions[1].Height.Value != 0)
                                {
                                    _contentGrid.RowDefinitions[1].Height = new GridLength(0);
                                    _header.Margin = new Thickness(0, _headerHeight, 0, 0);
                                    _header.Clip = new RectangleGeometry() { Rect = new Rect(0, _headerHeight, _contentGrid.DesiredSize.Width, _headerHeight) };
                                }
                            }
                        }
                        else if (OuterScrollViewer != null && value != _scrollPosition)
                        {
                            var horizontalOffset = OuterScrollViewer.HorizontalOffset + ScrollPosition.X - _scrollPosition.X;
                            var verticalOffset = OuterScrollViewer.VerticalOffset + ScrollPosition.Y - _scrollPosition.Y;
                            OuterScrollViewer.ChangeView(horizontalOffset, verticalOffset, null);
                        }
                        #region Cell ScrollPosition
                        //Cell ScrollPosition


                        // validate range
                        var cellScrollPosition = value;
                        cellScrollPosition.Y += _headerHeight;

                        maxV = totalRowsSize - sz.Height;

                        cellScrollPosition.X = Math.Max(-maxH, Math.Min(cellScrollPosition.X, 0));
                        cellScrollPosition.Y = Math.Max(-maxV, Math.Min(cellScrollPosition.Y, 0));

                        if (cellScrollPosition != _cellPanel.ScrollPosition)
                        {
                            _cellPanel.ScrollPosition = cellScrollPosition;
                            _columnHeaderPanel.ScrollPosition = new Point(cellScrollPosition.X, 0);

                            if (pointerOverPoint != null)
                            {
                                var pt = pointerOverPoint.Value;
                                pt = this.TransformToVisual(_cellPanel).TransformPoint(pt);
                                var fy = _cellPanel.Rows.GetFrozenSize();
                                pt.Y += _columnHeaderPanel.ActualHeight;
                                var sp = _cellPanel.ScrollPosition;
                                if (pt.Y < 0 || pt.Y > fy) pt.Y -= sp.Y;
                                // get row and column at given coordinates
                                var row = _cellPanel.Rows.GetItemAt(pt.Y);
                                _cellPanel.HandlePointerOver(row);
                            }
                        }
                        else
                        {

                        }
    
                        maxV = totalHeight - sz.Height;

                        if (!HasMoreItems(value) && (maxV + value.Y) <= 0)
                        {
                            if (value.Y < 0)
                            {
                                var footHeight = (value.Y - _scrollPosition.Y);
                                if (footHeight > 0 && footHeight <= _footerHeight)
                                {
                                    _contentGrid.RowDefinitions[5].Height = new GridLength(footHeight);
                                    _footer.Margin = new Thickness(0, 0, 0, -(value.Y - _scrollPosition.Y));
                                    _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, _contentGrid.DesiredSize.Width, _footerHeight) };
                                }
                                else
                                {
                                    _contentGrid.RowDefinitions[5].Height = new GridLength(0);
                                    _footer.Margin = new Thickness(0, 0, 0, _footerHeight);
                                    _footer.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, _contentGrid.DesiredSize.Width, 0) };
                                }


                                //_header.RenderTransform = new TranslateTransform() { Y = value.Y };
                            }
                        }
                        #endregion

                        //HasMoreItems(value);





                        // apply new value
                        if (cellScrollPosition != _cellPanel.ScrollPosition)
                        {
                            //_cellPanel.ScrollPosition = cellScrollPosition;
                            //_columnHeaderPanel.ScrollPosition = new Point(cellScrollPosition.X, 0);

                            ////HorizontalOffset = -value.X;
                            ////VerticalOffset = -value.Y;

                            //if (_horizontalScrollBar != null && _verticalScrollBar != null)
                            //{
                            //    _horizontalScrollBar.Value = -value.X;
                            //    _verticalScrollBar.Value = -value.Y;
                            //}

                            //if (pointerOverPoint != null)
                            //{
                            //    var pt = pointerOverPoint.Value;
                            //    pt = this.TransformToVisual(_cellPanel).TransformPoint(pt);
                            //    var fy = _cellPanel.Rows.GetFrozenSize();
                            //    pt.Y += _columnHeaderPanel.ActualHeight;
                            //    var sp = _cellPanel.ScrollPosition;
                            //    if (pt.Y < 0 || pt.Y > fy) pt.Y -= sp.Y;
                            //    // get row and column at given coordinates
                            //    var row = _cellPanel.Rows.GetItemAt(pt.Y);
                            //    _cellPanel.HandlePointerOver(row);
                            //}
                        }
                        //handle outer scrollviewer


                    }

                }

            }
        }
        /// <summary>
        /// fire when tap in column header
        /// </summary>
        public event EventHandler<SortingColumnEventArgs> SortingColumn;


        /// <summary>
        /// fire when tap in column header
        /// </summary>
        public event EventHandler<ItemClickEventArgs> ItemClick;

        /// <summary>
        /// occur when reach threshold.
        /// </summary>
        public event EventHandler PullToRefresh;

        #endregion

        #region Dependency Properties
        public object ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(DataGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).OnItemsSourceChanged();
        }

        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenerateColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(DataGrid), new PropertyMetadata(false, OnAutoGenerateColumnsChanged));


        private static void OnAutoGenerateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).OnItemsSourceChanged();
        }

        public Brush RowBackground
        {
            get { return (Brush)GetValue(RowBackgroundProperty); }
            set { SetValue(RowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register("RowBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnRowBackgroundChanged));

        private static void OnRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public Brush AlternatingRowBackground
        {
            get { return (Brush)GetValue(AlternatingRowBackgroundProperty); }
            set { SetValue(AlternatingRowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlternatingRowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlternatingRowBackgroundProperty =
            DependencyProperty.Register("AlternatingRowBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnAlternatingRowBackgroundChanged));

        private static void OnAlternatingRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnGridLinesBrushChanged));

        private static void OnGridLinesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public Brush HeaderGridLinesBrush
        {
            get { return (Brush)GetValue(HeaderGridLinesBrushProperty); }
            set { SetValue(HeaderGridLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderGridLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderGridLinesBrushProperty =
            DependencyProperty.Register("HeaderGridLinesBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnHeaderGridLinesBrushChanged));

        private static void OnHeaderGridLinesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public Brush ColumnHeaderBackground
        {
            get { return (Brush)GetValue(ColumnHeaderBackgroundProperty); }
            set { SetValue(ColumnHeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderBackgroundProperty =
            DependencyProperty.Register("ColumnHeaderBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnColumnHeaderBackgroundChanged));

        private static void OnColumnHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }


        public GridLinesVisibility GridLinesVisibility
        {
            get { return (GridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLinesVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(GridLinesVisibility), typeof(DataGrid), new PropertyMetadata(GridLinesVisibility.None, OnGridLinesVisibilityChanged));

        private static void OnGridLinesVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public Brush ColumnHeaderForeground
        {
            get { return (Brush)GetValue(ColumnHeaderForegroundProperty); }
            set { SetValue(ColumnHeaderForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderForegroundProperty =
            DependencyProperty.Register("ColumnHeaderForeground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnColumnHeaderForegroundChanged));

        private static void OnColumnHeaderForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }


        public bool ShowSort
        {
            get { return (bool)GetValue(ShowSortProperty); }
            set { SetValue(ShowSortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSortProperty =
            DependencyProperty.Register("ShowSort", typeof(bool), typeof(DataGrid), new PropertyMetadata(true, OnShowSortChanged));

        private static void OnShowSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public bool AllowSorting
        {
            get { return (bool)GetValue(AllowSortingProperty); }
            set { SetValue(AllowSortingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowSorting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowSortingProperty =
            DependencyProperty.Register("AllowSorting", typeof(bool), typeof(DataGrid), new PropertyMetadata(true));

        public Brush FrozenLinesBrush
        {
            get { return (Brush)GetValue(FrozenLinesBrushProperty); }
            set { SetValue(FrozenLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenLinesBrushProperty =
            DependencyProperty.Register("FrozenLinesBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null, OnFrozenLinesBrushChanged));

        private static void OnFrozenLinesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).Invalidate();
        }

        public bool ShowFrozenLines
        {
            get { return (bool)GetValue(ShowFrozenLinesProperty); }
            set { SetValue(ShowFrozenLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFrozenLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFrozenLinesProperty =
            DependencyProperty.Register("ShowFrozenLines", typeof(bool), typeof(DataGrid), new PropertyMetadata(true));



        #region PullToRefresh

        public bool AllowPullToRefresh
        {
            get { return (bool)GetValue(AllowPullToRefreshProperty); }
            set { SetValue(AllowPullToRefreshProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowPullToRefresh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowPullToRefreshProperty =
            DependencyProperty.Register("AllowPullToRefresh", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));
        /// <summary>
        /// The threshold for release to refresh，defautl value is 1/5 of PullToRefreshPanel's height.
        /// </summary>
        public double RefreshThreshold
        {
            get { return (double)GetValue(RefreshThresholdProperty); }
            set { SetValue(RefreshThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshThresholdProperty =
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(DataGrid), new PropertyMetadata(0.0));



        public DataTemplate PullToRefreshHeaderTemplate
        {
            get { return (DataTemplate)GetValue(PullToRefreshHeaderTemplateProperty); }
            set { SetValue(PullToRefreshHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PullToRefreshHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullToRefreshHeaderTemplateProperty =
            DependencyProperty.Register("PullToRefreshHeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null));

        public object PullToRefreshHeader
        {
            get { return (object)GetValue(PullToRefreshHeaderProperty); }
            set { SetValue(PullToRefreshHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PullToRefreshHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullToRefreshHeaderProperty =
            DependencyProperty.Register("PullToRefreshHeader", typeof(object), typeof(DataGrid), new PropertyMetadata(null));


        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));

        public DateTime LastRefreshTime
        {
            get { return (DateTime)GetValue(LastRefreshTimeProperty); }
            set { SetValue(LastRefreshTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastRefreshTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastRefreshTimeProperty =
            DependencyProperty.Register("LastRefreshTime", typeof(DateTime), typeof(DataGrid), new PropertyMetadata(DateTime.Now));

        #endregion

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DataGrid), new PropertyMetadata(ScrollBarVisibility.Auto));


        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DataGrid), new PropertyMetadata(ScrollBarVisibility.Auto));

        public ScollingDirectionMode ScollingDirectionMode
        {
            get { return (ScollingDirectionMode)GetValue(ScollingDirectionModeProperty); }
            set { SetValue(ScollingDirectionModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScollingDirectionMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScollingDirectionModeProperty =
            DependencyProperty.Register("ScollingDirectionMode", typeof(ScollingDirectionMode), typeof(DataGrid), new PropertyMetadata(ScollingDirectionMode.TwoDirection));


        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null));

        public Brush PointerOverBackground
        {
            get { return (Brush)GetValue(PointerOverBackgroundProperty); }
            set { SetValue(PointerOverBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerOverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerOverBackgroundProperty =
            DependencyProperty.Register("PointerOverBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(null));


        #region Header
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(DataGrid), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null));


        #endregion

        #region Footer
        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Footer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(DataGrid), new PropertyMetadata(null));

        public DataTemplate FooterTemplate
        {
            get { return (DataTemplate)GetValue(FooterTemplateProperty); }
            set { SetValue(FooterTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FooterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null));

        #endregion

        #endregion


    }

    public class SortingColumnEventArgs : CancelEventArgs
    {
        public Column Column { get; private set; }
        public SortingColumnEventArgs(Column col)
        {
            Column = col;
        }
    }

    public class ItemClickEventArgs : EventArgs
    {
        public Row Row { get; private set; }
        public object ClickedItem { get; private set; }

        public ItemClickEventArgs(Row row)
        {
            Row = row;
            ClickedItem = row.DataItem;
        }
    }

    public class CrossSlideEventArgs
    {
        public CrossSlideMode Mode { get; private set; }
        public CrossSlideEventArgs(CrossSlideMode mode)
        {
            Mode = mode;
        }
    }

}
