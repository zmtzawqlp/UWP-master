using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWP.FlexGrid
{
    public partial class FlexGrid
    {
        #region fields
        AddRemoveItemHanlder addRemoveItemHanlder;
        FlexGridPanel _columnHeaderPanel;
        FlexGridPanel _cellPanel;

        ICollectionView _view;
        ICellFactory _cellFactory;
        Dictionary<string, PropertyInfo> _props;
        Type _itemType;
        ICellFactory _defautlCellFactory = new CellFactory();
        private bool _isLoadingMoreItems;
        #endregion


        #region public properties
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

        public FlexGridPanel Cells
        {
            get { return _cellPanel; }
        }

        public FlexGridPanel ColumnHeaders
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
            //    if (_contentGrid != null)
            //    {
            //        var wid = _contentGrid.ActualWidth;
            //        var hei = _contentGrid.ActualHeight;
            //        if (!double.IsPositiveInfinity(wid) && !double.IsPositiveInfinity(hei))
            //        {
            //            if (VerticalScrollMode == ScrollMode.Disabled || (OuterScrollViewer != null && OuterScrollViewer.VerticalScrollMode == ScrollMode.Disabled))
            //            {
            //                value.Y = 0;
            //            }

            //            if (HorizontalScrollMode == ScrollMode.Disabled || (OuterScrollViewer != null && OuterScrollViewer.HorizontalScrollMode == ScrollMode.Disabled))
            //            {
            //                value.X = 0;
            //            }

            //            //viewPort
            //            //var sz = _contentGrid.DesiredSize;
            //            var sz = new Size(wid, hei);
            //            //total size
            //            var totalRowsSize = Rows.GetTotalSize();
            //            //Debug.WriteLine("addRemoveCount : " + addRemoveItemHanlder.Count);
            //            //Debug.WriteLine("addCount : " + addRemoveItemHanlder.AddTotalCount + ", removeCount : " + addRemoveItemHanlder.RemoveTotalCount);
            //            if (ItemsUpdatingScrollMode == ItemsUpdatingScrollMode.KeepItemsInView)
            //            {
            //                value.Y -= (addRemoveItemHanlder.Count) * _cellPanel.Rows.DefaultSize;
            //                addRemoveItemHanlder.Reset();
            //            }

            //            var totalColumnsSize = Columns.GetTotalSize();
            //            var totalHeight = totalRowsSize + HeaderMeasureHeight + _columnHeaderPanel.DesiredSize.Height + FooterMeasureHeight;

            //            bool reachingFirstRow = false;
            //            if (_view != null && _view.Count != 0)
            //            {
            //                if (_scrollPosition.Y < value.Y)
            //                {
            //                    if (HeaderMeasureHeight != 0)
            //                    {
            //                        if (Math.Abs(value.Y) <= HeaderMeasureHeight || value.Y > 0)
            //                        {
            //                            reachingFirstRow = true;
            //                            if (ReachingFirstRow != null)
            //                            {
            //                                var eventArgs = new ReachingFirstRowEventArgs();
            //                                ReachingFirstRow(this, eventArgs);
            //                                if (eventArgs.Cancel)
            //                                {
            //                                    //value.Y = _scrollPosition.Y;
            //                                    value.Y = -HeaderMeasureHeight;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            var maxV = totalHeight - sz.Height;
            //            maxV = maxV >= 0 ? maxV : 0;
            //            var maxH = totalColumnsSize - sz.Width;

            //            var totalScrollPosition = new Point();
            //            totalScrollPosition.X = Math.Max(-maxH, Math.Min(value.X, 0));
            //            totalScrollPosition.Y = Math.Max(-maxV, Math.Min(value.Y, 0));

            //            if (_scrollPosition != totalScrollPosition)
            //            {
            //                //if (_verticalScrollBar.Maximum == -totalScrollPosition.Y && HasMoreItems(value))
            //                //{
            //                //    _scrollPosition = totalScrollPosition;
            //                //    return;
            //                //}
            //                _scrollPosition = totalScrollPosition;
            //                if (_horizontalScrollBar != null && _verticalScrollBar != null)
            //                {
            //                    _horizontalScrollBar.Value = -totalScrollPosition.X;
            //                    _verticalScrollBar.Value = -totalScrollPosition.Y;
            //                }

            //                HandleHeader(totalScrollPosition);
            //            }
            //            //Handle outerScrollViewer
            //            //else if (OuterScrollViewer != null && value != _scrollPosition)
            //            //{
            //            //    var horizontalOffset = OuterScrollViewer.HorizontalOffset + _scrollPosition.X - value.X;
            //            //    var verticalOffset = OuterScrollViewer.VerticalOffset + _scrollPosition.Y - value.Y;
            //            //    OuterScrollViewer.ChangeView(horizontalOffset, verticalOffset, null);
            //            //}
            //            HandleCellAndColumnScrollPosition(value, sz, totalRowsSize, maxH);


            //            if (_view != null && _view.Count != 0)
            //            {
            //                if (HeaderMeasureHeight != 0 && reachingFirstRow && ReachedFirstRow != null)
            //                {
            //                    ReachedFirstRow(this, EventArgs.Empty);
            //                }
            //            }

            //            var hasMoreItems = HasMoreItems(value);
            //            if (_view != null && _view.Count != 0)
            //            {
            //                if (!hasMoreItems && ReachingLastRow != null)
            //                {
            //                    var eventArgs = new ReachingLastRowEventArgs();
            //                    ReachingLastRow(this, eventArgs);
            //                    if (eventArgs.Cancel)
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //            //if maxV <0 and value.Y==0, it means, rows height +_headerHeight +column height is less than this control height.
            //            //we should handle footer also
            //            HandleFooter(value, sz, totalRowsSize, hasMoreItems);

            //        }

            //    }

            }
        }
        #endregion


        #region Dependency Properties
        public object ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(FlexGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FlexGrid).OnItemsSourceChanged();
        }

        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenerateColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(FlexGrid), new PropertyMetadata(false, OnAutoGenerateColumnsChanged));


        private static void OnAutoGenerateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FlexGrid).OnItemsSourceChanged();
        }

        /// <summary>
        /// default value is ItemsUpdatingScrollMode.KeepScrollOffset
        /// </summary>
        public ItemsUpdatingScrollMode ItemsUpdatingScrollMode
        {
            get { return (ItemsUpdatingScrollMode)GetValue(ItemsUpdatingScrollModeProperty); }
            set { SetValue(ItemsUpdatingScrollModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsUpdatingScrollMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsUpdatingScrollModeProperty =
            DependencyProperty.Register("ItemsUpdatingScrollMode", typeof(ItemsUpdatingScrollMode), typeof(FlexGrid), new PropertyMetadata(ItemsUpdatingScrollMode.KeepScrollOffset));
        #endregion
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
