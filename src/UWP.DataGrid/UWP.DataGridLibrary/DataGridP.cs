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
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Diagnostics;
using Windows.UI.Xaml.Input;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        #region fields
        //ScrollViewer _scrollViewer;
        Grid _contentGrid;
        internal ScrollBar _horizontalScrollBar;
        internal ScrollBar _verticalScrollBar;
        DataGridPanel _columnHeaderPanel;
        DataGridPanel _cellPanel;
        ContentControl _pullToRefreshHeader;
        DataGridContentPresenter _header;
        DataGridContentPresenter _footer;

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
        bool manipulationOnHeaderOrFooter;
        Point? pointerOverPoint;
        AddRemoveItemHanlder addRemoveItemHanlder;
        #endregion

        #region Internal Properties

        internal double HeaderMeasureHeight
        {

            get
            {
                if (_header != null)
                {
                    return _header.ContentHeight;
                }
                return 0.0;
            }
        }

        internal double FooterMeasureHeight
        {

            get
            {
                if (_footer != null)
                {
                    return _footer.ContentHeight;
                }
                return 0.0;
            }
        }

        internal IList<SortDescription> ManualSortDescriptions { get { return _manualSort; } }

        bool firstTimeTrytoFindPivotItem = true;
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

        internal double topToOuterScrollViewer = -1;
        ScrollViewerView preview;
        ScrollViewer _outerScrollViewer;

        internal ScrollViewer OuterScrollViewer
        {
            get
            {
                if (_outerScrollViewer == null)
                {
                    var parent =this.Parent as FrameworkElement;
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

                //hanlde verticalScroll by outer ScrollViewer
                if (_outerScrollViewer != null && (_outerScrollViewer.VerticalScrollMode != ScrollMode.Disabled || _outerScrollViewer.HorizontalScrollMode != ScrollMode.Disabled) && topToOuterScrollViewer == -1)
                {
                    var _outerScrollViewerContent = (_outerScrollViewer.Content as FrameworkElement);
                    if (_outerScrollViewerContent != null)
                    {
                        _outerScrollViewerContent.SizeChanged -= _outerScrollViewerContent_SizeChanged;
                        _outerScrollViewerContent.SizeChanged += _outerScrollViewerContent_SizeChanged; ;
                    }
                    _outerScrollViewer.ViewChanging -= _outerScrollViewer_ViewChanging;
                    _outerScrollViewer.ViewChanging += _outerScrollViewer_ViewChanging;
                   
                }
                return _outerScrollViewer;
            }

        }



        internal bool OuterScrollViewerVerticalScrollEnable
        {
            get
            {
                if (OuterScrollViewer != null)
                {
                    if (OuterScrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled || OuterScrollViewer.VerticalScrollMode == ScrollMode.Disabled)
                    {
                        return false;
                    }
                    return true;

                    //return !(OuterScrollViewer.VerticalScrollMode == ScrollMode.Disabled && OuterScrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);

                    ////return (OuterScrollViewer.VerticalScrollMode != ScrollMode.Disabled 
                    ////    //|| OuterScrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled
                    ////    );
                }
                return false;
            }
        }

        internal bool OuterScrollViewerHorizontalScrollEnable
        {
            get
            {
                if (OuterScrollViewer != null)
                {
                    if (OuterScrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled || OuterScrollViewer.HorizontalScrollMode == ScrollMode.Disabled)
                    {
                        return false;
                    }
                    return true;
                    //return !(OuterScrollViewer.HorizontalScrollMode == ScrollMode.Disabled && OuterScrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled);

                    //return (OuterScrollViewer.HorizontalScrollMode != ScrollMode.Disabled 
                    //    //|| OuterScrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled
                    //    );
                }
                return false;
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

        internal double HeaderActualHeight
        {
            get
            {
                if (_contentGrid == null)
                {
                    return 0;
                }
                return _contentGrid.RowDefinitions[1].ActualHeight;
            }
        }

        internal double FooterActualHeight
        {
            get
            {
                if (_contentGrid == null)
                {
                    return 0;
                }
                return _contentGrid.RowDefinitions[5].ActualHeight;
            }
        }


        internal GridLength HeaderHeight
        {
            get
            {
                if (_contentGrid == null)
                {
                    return GridLength.Auto;
                }
                return _contentGrid.RowDefinitions[1].Height;
            }
            set
            {
                if (_contentGrid != null)
                {
                    _contentGrid.RowDefinitions[1].Height = value;
                }
            }
        }

        internal GridLength FooterHeight
        {
            get
            {
                if (_contentGrid == null)
                {
                    return GridLength.Auto;
                }
                return _contentGrid.RowDefinitions[5].Height;
            }
            set
            {
                if (_contentGrid != null)
                {
                    _contentGrid.RowDefinitions[5].Height = value;
                }
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Refresh ScrollPosition when view changed.
        /// default value true
        /// Sometime if you don't want refresh scrollPosition 
        /// when you add or remove items
        /// you can set it to false before your changes.
        /// </summary>
        public bool RefreshScrollPosition { get; set; }

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
                    if (!double.IsPositiveInfinity(wid) && !double.IsPositiveInfinity(hei))
                    {
                        if (VerticalScrollMode == ScrollMode.Disabled || (OuterScrollViewer != null && OuterScrollViewer.VerticalScrollMode == ScrollMode.Disabled))
                        {
                            value.Y = 0;
                        }

                        if (HorizontalScrollMode == ScrollMode.Disabled || (OuterScrollViewer != null && OuterScrollViewer.HorizontalScrollMode == ScrollMode.Disabled))
                        {
                            value.X = 0;
                        }

                        //viewPort
                        //var sz = _contentGrid.DesiredSize;
                        var sz = new Size(wid, hei);
                        //total size
                        var totalRowsSize = Rows.GetTotalSize();
                        //Debug.WriteLine("addRemoveCount : " + addRemoveItemHanlder.Count);
                        //Debug.WriteLine("addCount : " + addRemoveItemHanlder.AddTotalCount + ", removeCount : " + addRemoveItemHanlder.RemoveTotalCount);
                        if (RefreshScrollPosition)
                        {
                            value.Y -= (addRemoveItemHanlder.Count) * _cellPanel.Rows.DefaultSize;
                        }
                        addRemoveItemHanlder.Reset();

                        var totalColumnsSize = Columns.GetTotalSize();
                        var totalHeight = totalRowsSize + HeaderMeasureHeight + _columnHeaderPanel.DesiredSize.Height + FooterMeasureHeight;

                        bool reachingFirstRow = false;
                        if (_view != null && _view.Count != 0)
                        {
                            if (_scrollPosition.Y < value.Y)
                            {
                                if (HeaderMeasureHeight != 0)
                                {
                                    if (Math.Abs(value.Y) <= HeaderMeasureHeight || value.Y > 0)
                                    {
                                        reachingFirstRow = true;
                                        if (ReachingFirstRow != null)
                                        {
                                            var eventArgs = new ReachingFirstRowEventArgs();
                                            ReachingFirstRow(this, eventArgs);
                                            if (eventArgs.Cancel)
                                            {
                                                //value.Y = _scrollPosition.Y;
                                                value.Y = -HeaderMeasureHeight;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var maxV = totalHeight - sz.Height;
                        maxV = maxV >= 0 ? maxV : 0;
                        var maxH = totalColumnsSize - sz.Width;

                        var totalScrollPosition = new Point();
                        totalScrollPosition.X = Math.Max(-maxH, Math.Min(value.X, 0));
                        totalScrollPosition.Y = Math.Max(-maxV, Math.Min(value.Y, 0));

                        if (_scrollPosition != totalScrollPosition)
                        {
                            //if (_verticalScrollBar.Maximum == -totalScrollPosition.Y && HasMoreItems(value))
                            //{
                            //    _scrollPosition = totalScrollPosition;
                            //    return;
                            //}
                            _scrollPosition = totalScrollPosition;
                            if (_horizontalScrollBar != null && _verticalScrollBar != null)
                            {
                                _horizontalScrollBar.Value = -totalScrollPosition.X;
                                _verticalScrollBar.Value = -totalScrollPosition.Y;
                            }

                            HandleHeader(totalScrollPosition);
                        }
                        //Handle outerScrollViewer
                        //else if (OuterScrollViewer != null && value != _scrollPosition)
                        //{
                        //    var horizontalOffset = OuterScrollViewer.HorizontalOffset + _scrollPosition.X - value.X;
                        //    var verticalOffset = OuterScrollViewer.VerticalOffset + _scrollPosition.Y - value.Y;
                        //    OuterScrollViewer.ChangeView(horizontalOffset, verticalOffset, null);
                        //}
                        HandleCellAndColumnScrollPosition(value, sz, totalRowsSize, maxH);


                        if (_view != null && _view.Count != 0)
                        {
                            if (HeaderMeasureHeight != 0 && reachingFirstRow && ReachedFirstRow != null)
                            {
                                ReachedFirstRow(this, EventArgs.Empty);
                            }
                        }

                        var hasMoreItems = HasMoreItems(value);
                        if (_view != null && _view.Count != 0)
                        {
                            if (!hasMoreItems && ReachingLastRow != null)
                            {
                                var eventArgs = new ReachingLastRowEventArgs();
                                ReachingLastRow(this, eventArgs);
                                if (eventArgs.Cancel)
                                {
                                    return;
                                }
                            }
                        }
                        //if maxV <0 and value.Y==0, it means, rows height +_headerHeight +column height is less than this control height.
                        //we should handle footer also
                        HandleFooter(value, sz, totalRowsSize, hasMoreItems);

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

        /// <summary>
        /// occur when reaching first row.
        /// if cancel is true, it won't go to header if it has.
        /// </summary>
        public event EventHandler<ReachingFirstRowEventArgs> ReachingFirstRow;

        /// <summary>
        /// occur when reaching last row.
        /// if cancel is true, it won't go to footer if it has.
        /// </summary>
        public event EventHandler<ReachingLastRowEventArgs> ReachingLastRow;


        /// <summary>
        /// occur when reached first row.
        /// </summary>
        public event EventHandler ReachedFirstRow;



        /// <summary>
        /// Occurs when the grid starts loading the rows with items from the data source.
        /// </summary>
        public event EventHandler LoadingRows;

        /// <summary>
        /// Occurs when the grid finishes loading the rows with items from the data source.
        /// </summary>
        public event EventHandler LoadedRows;
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
        /// The threshold for release to refresh，defautl value is 1/5 of PullToRefreshGrid's height.
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



        public ScrollMode HorizontalScrollMode
        {
            get { return (ScrollMode)GetValue(HorizontalScrollModeProperty); }
            set { SetValue(HorizontalScrollModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollModeProperty =
            DependencyProperty.Register("HorizontalScrollMode", typeof(ScrollMode), typeof(DataGrid), new PropertyMetadata(ScrollMode.Auto, new PropertyChangedCallback(OnScrollModeChanged)));

        private static void OnScrollModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataGrid).OnScrollModeChanged();
        }

        public ScrollMode VerticalScrollMode
        {
            get { return (ScrollMode)GetValue(VerticalScrollModeProperty); }
            set { SetValue(VerticalScrollModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollModeProperty =
            DependencyProperty.Register("VerticalScrollMode", typeof(ScrollMode), typeof(DataGrid), new PropertyMetadata(ScrollMode.Auto, new PropertyChangedCallback(OnScrollModeChanged)));

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

    public class ReachingFirstRowEventArgs : CancelEventArgs
    {

    }

    public class ReachingLastRowEventArgs : CancelEventArgs
    {

    }

    internal class AddRemoveItemHanlder
    {
        public int AddTotalCount { get; set; }

        public int RemoveTotalCount { get; set; }

        public int AddItemLessThanTopCount { get; set; }

        public int RemoveItemLessThanTopCount { get; set; }

        public int CurrentTopRow { get; set; }

        public AddRemoveItemHanlder()
        {
            CurrentTopRow = -1;
        }

        public void Reset()
        {
            AddTotalCount = RemoveTotalCount =
             AddItemLessThanTopCount = RemoveItemLessThanTopCount = 0;
            CurrentTopRow = -1;
        }

        /// <summary>
        /// if count is positive，we should remove row height, otherwise add.
        /// </summary>
        public int Count
        {
            get
            {
                //if the count is not change,return 0
                //if (AddTotalCount == RemoveTotalCount)
                //{
                //    return 0;
                //}
                //else
                {
                    return AddItemLessThanTopCount - RemoveItemLessThanTopCount;
                }
            }
        }
    }
}
