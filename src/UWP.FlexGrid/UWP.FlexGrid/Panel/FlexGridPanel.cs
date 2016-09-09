using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP.FlexGrid
{
    public class FlexGridPanel : Panel
    {
        #region fields

        FlexGrid _grid;
        Rows _rows;
        Columns _cols;
        CellRange _viewRange;
        CellType _cellType;
        Point _scrollPos;
        CellRangeDictionary _cells;
        RowsDictionary _rowsDict;
        #endregion


        #region ctor
        internal FlexGridPanel(FlexGrid grid, CellType cellType, int rowHeight, int colWidth)
        {
            _grid = grid;
            _cellType = cellType;
            _cells = new CellRangeDictionary();
            _rowsDict = new RowsDictionary();
            _viewRange = CellRange.Empty;

            Rows = new Rows(this, rowHeight);
            Columns = new Columns(this, colWidth);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            UseLayoutRounding = false;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the collection of rows in this <see cref="FlexGridPanel"/>.
        /// </summary>
        public Rows Rows
        {
            get { return _rows; }
            set
            {
                if (_rows != null)
                {
                    _rows.CollectionChanged -= rowsCols_CollectionChanged;
                }
                _rows = value;
                if (_rows != null)
                {
                    _rows.CollectionChanged += rowsCols_CollectionChanged;
                }
                _cells.Clear();
                _rowsDict.Clear();
                Children.Clear();
            }
        }


        /// <summary>
        /// Gets the collection of columns in this <see cref="FlexGridPanel"/>.
        /// </summary>
        public Columns Columns
        {
            get { return _cols; }
            set
            {
                if (_cols != null)
                {
                    _cols.CollectionChanged -= rowsCols_CollectionChanged;
                }
                _cols = value;
                if (_cols != null)
                {
                    _cols.CollectionChanged += rowsCols_CollectionChanged;
                }
                _rowsDict.Clear();
                _cells.Clear();
                Children.Clear();
            }
        }
        /// <summary>
        /// Gets the type of cell that this panel contains.
        /// </summary>
        public CellType CellType
        {
            get { return _cellType; }
        }
        internal CellRange ViewRange
        {
            get { return _viewRange; }
        }

        public FlexGrid Grid
        {
            get { return _grid; }
        }

       
        #endregion

        #region Method
        private void rowsCols_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure(); // total row/col size may have changed
            //When a row is added at the end of the collection it is not necessary to remove all the cells. 
            //This is specially important in incremental loading scenarios.
            if (e.Action != NotifyCollectionChangedAction.Add ||
                e.NewStartingIndex < ViewRange.Row2)
            {
                Invalidate(); // need to re-create and re-arrange cells
            }
            else
            {
                Invalidate(new CellRange(e.NewStartingIndex, -1));
            }
            UpdateViewRange();
        }


        internal void Invalidate()
        {
            Invalidate(CellRange.Empty);
        }

        public void Invalidate(bool force = false)
        {
            if (force)
            {
                foreach (var item in _cells)
                {
                    Children.Remove(item.Value);
                    _grid.DisposeCell(this, item.Value);
                }

                _cells.Clear();
            }
            Invalidate(CellRange.Empty);
        }

        internal void Invalidate(CellRange rng)
        {
            // negative parameters mean 'whole row' or 'whole column'
            if (rng.Row > -1 || rng.Column > -1)// Keep the range as invalid when rng.IsEmpty so that it can remove all the cells after a row was removed, otherwise it can leave non used cells because Rows.Count is less than there was in previous layout.
            {
                if (rng.Row < 0)
                {
                    rng.Row = 0;
                    rng.Row2 = Rows.Count - 1;
                }
                if (rng.Column < 0)
                {
                    rng.Column = 0;
                    rng.Column2 = Columns.Count - 1;
                }
            }

            // optimization (especially important when setting unbound values)
            if (rng.IsValid &&
                !ViewRange.Intersects(rng) &&
                !Columns.IsFrozen(rng.LeftColumn) &&
                !Rows.IsFrozen(rng.TopRow))
            {
                return;
            }

            // remove cells so they can be re-created
            List<CellRange> removeCells = new List<CellRange>();
            foreach (var rg in _cells.Keys)
            {
                if (!rg.IsValid || !rng.IsValid || rg.Intersects(rng))
                    removeCells.Add(rg);
            }
            for (int i = 0; i < removeCells.Count; i++)
            {
                RemoveCell(removeCells[i]);
            }

            if (removeCells.Count == 0)
            {
                InvalidateArrange();
                UpdateViewRange();
            }
        }

        void RemoveCell(CellRange rng)
        {
            FrameworkElement cell;
            if (_cells.TryGetValue(rng, out cell))
            {
                _cells.Remove(rng);
                Children.Remove(cell);
                _grid.DisposeCell(this, cell);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(_cols.GetTotalSize(), _rows.GetTotalSize());
            return size;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateViewRange();
            CellRangeDictionary showCells = new CellRangeDictionary();
            foreach (int r in Rows.EnumerateVisibleElements(_viewRange.TopRow, _viewRange.BottomRow))
            {

                var left = _viewRange.LeftColumn;

                // enumerate visible columns
                foreach (int c in Columns.EnumerateVisibleElements(left, _viewRange.RightColumn))
                {
                    var rng = new CellRange(r, c);
                    showCells[rng] = null;
                }
            }

            // remove cells that are shown but shouldn't be
            List<CellRange> removeCells = new List<CellRange>();
            foreach (var rng in _cells.Keys)
            {
                if (!showCells.ContainsKey(rng))
                    removeCells.Add(rng);
            }


            // create cells that should be shown but aren't
            foreach (var rng in showCells.Keys)
            {
                CreateCell(rng, removeCells);
            }

            for (int i = 0; i < removeCells.Count; i++)
            {
                RemoveCell(removeCells[i]);
            }

            foreach (KeyValuePair<CellRange, FrameworkElement> kvp in _cells)
            {
                // avoid re-creating the range based on element (for performance)
                var rng = kvp.Key;
                var cell = kvp.Value;

                if (rng.Row < 0 || rng.Row >= Rows.Count ||
                    rng.Column < 0 || rng.Column >= Columns.Count)
                {
                    cell.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // get rectangle to arrange cell
                    var rc = GetCellRect(rng);

                    //fix auto  column,if Width is Auto and AdaptUISize is true, the width will the adapt to UI width.
                    if (Columns[rng.Column].AdaptUISize)
                    {
                        cell.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        if (cell.DesiredSize.Width > rc.Width)
                        {
                            rc.Width = cell.DesiredSize.Width + 5;
                            Columns[rng.Column].SetSize(rc.Width);
                            Columns.SetIsDirty(true);
                            //if Cell has dirty data, we should also update Column
                            if (CellType == CellType.Cell)
                            {
                                //Grid.ColumnHeaders.Invalidate();
                            }
                            //Column always render before cell, so we don't need invalidate cell right now.
                            ////if column has dirty data, we should also update cell
                            //else
                            //{
                            //    Grid.Cells.Invalidate();
                            //}
                        }
                    }

                    // set clipping to account for frozen rows/columns
                    if (Rows.Frozen > 0 || Columns.Frozen > 0)
                    {
                        cell.Clip = null;
                        if (!Columns.IsFrozen(rng.Column) || !Rows.IsFrozen(rng.Row))
                        {
                            // no clipping yet
                            var rcClip = new Rect(0, 0, rc.Width, rc.Height);

                            // clip unfrozen columns
                            if (!Columns.IsFrozen(rng.Column))
                            {
                                var fx = Columns.GetFrozenSize();
                                if (fx > 0 && rc.X < fx)
                                {
                                    rcClip.X = fx - rc.X;
                                }
                            }

                            // clip unfrozen rows
                            if (!Rows.IsFrozen(rng.Row))
                            {
                                var fy = Rows.GetFrozenSize();
                                if (fy > 0 && rc.Y < fy)
                                {
                                    rcClip.Y = fy - rc.Y;
                                }
                            }

                            // activate clip
                            if (rcClip.X > 0 || rcClip.Y > 0)
                            {
                                cell.Clip = new RectangleGeometry() { Rect = rcClip };
                            }
                        }
                    }

                    // limit editor size to grid cell (so trimming will work)
                    cell.Width = rc.Width;
                    cell.Height = rc.Height;
                    // to finish the layout
                    cell.Measure(new Size(rc.Width, rc.Height));
                    cell.Arrange(rc);
                }
            }

            // measure content
            return new Size(_cols.GetTotalSize(), _rows.GetTotalSize());
        }

        void CreateCell(CellRange rng, List<CellRange> removeCells = null)
        {
            if (!_cells.ContainsKey(rng))
            {
                if (removeCells != null && removeCells.Count > 0)
                {
                    var cell1 = removeCells.FirstOrDefault(x => x.Column == rng.Column);
                    if (cell1 != rng && cell1.Column == rng.Column && _cells.ContainsKey(cell1))
                    {
                        var frame = _cells[cell1];

                        frame.SetValue(Windows.UI.Xaml.Controls.Grid.RowProperty, rng.TopRow);
                        frame.SetValue(Windows.UI.Xaml.Controls.Grid.ColumnProperty, rng.LeftColumn);
                        frame.SetValue(Windows.UI.Xaml.Controls.Grid.RowSpanProperty, Math.Abs(rng.RowSpan));
                        frame.SetValue(Windows.UI.Xaml.Controls.Grid.ColumnSpanProperty, Math.Abs(rng.ColumnSpan));
                        frame.DataContext = Rows[rng.Row].DataItem;
                        _cells[rng] = frame;
                        _cells.Remove(cell1);
                        removeCells.Remove(cell1);
                        return;
                    }
                }

                var cell = _grid.CreateCell(this, rng);
                AddCell(cell, rng);
            }
        }

        void AddCell(FrameworkElement cell, CellRange rng)
        {
            cell.SetValue(Windows.UI.Xaml.Controls.Grid.RowProperty, rng.TopRow);
            cell.SetValue(Windows.UI.Xaml.Controls.Grid.ColumnProperty, rng.LeftColumn);
            cell.SetValue(Windows.UI.Xaml.Controls.Grid.RowSpanProperty, Math.Abs(rng.RowSpan));
            cell.SetValue(Windows.UI.Xaml.Controls.Grid.ColumnSpanProperty, Math.Abs(rng.ColumnSpan));

            Children.Add(cell);
            _cells[rng] = cell;
        }

        public Rect GetCellRect(CellRange rng)
        {
            return GetCellRect(rng, false, true);
        }

        internal Rect GetCellRect(CellRange rng, bool clipToView, bool honorGroups)
        {
            // get cell rectangle, expand to handle merges
            var rc = GetCellRect(rng.TopRow, rng.LeftColumn, clipToView, honorGroups);
            if (!rng.IsSingleCell)
            {
                var rc2 = GetCellRect(rng.BottomRow, rng.RightColumn, clipToView, honorGroups);
                rc.Union(rc2);
            }

            // done
            return rc;
        }

        internal Rect GetCellRect(int row, int col, bool clipToView, bool honorGroups)
        {
            // no negative indices
            if (col < 0) col = 0;
            if (row < 0) row = 0;
            if (row >= Rows.Count || col >= Columns.Count)
            {
                return Rect.Empty;
            }

            // adjust for frozen cells
            var offsetX = Columns.IsFrozen(col) ? 0 : _scrollPos.X;
            var offsetY = Rows.IsFrozen(row) ? 0 : _scrollPos.Y;

            // build rectangle
            var rc = new Rect(
                _cols.GetItemPosition(col) + offsetX,
                _rows.GetItemPosition(row) + offsetY,
                _cols.GetItemSize(col),
                _rows.GetItemSize(row));

            // honor frozen areas
            if (clipToView)
            {
                if (!Rows.IsFrozen(row))
                {
                    var fy = Rows.GetFrozenSize();
                    if (rc.Y < fy)
                    {
                        var b = rc.Bottom;
                        rc.Y = fy;
                        rc.Height = Math.Max(0, b - rc.Y);
                    }
                }
                if (!Columns.IsFrozen(col))
                {
                    var fx = Columns.GetFrozenSize();
                    if (rc.X < fx)
                    {
                        var r = rc.Right;
                        rc.X = fx;
                        rc.Width = Math.Max(0, r - rc.X);
                    }
                }
            }

            // done
            return rc;
        }

        internal CellRange GetViewRange(Point scrollPosition)
        {
            // get size, position, range
            var sz = DesiredSize;
            // account for frozen rows/columns
            var fx = Columns.GetFrozenSize();
            var fy = Rows.GetFrozenSize();
            var r = new CellRange(Rows.Frozen, Columns.Frozen);
            //var sv = _grid.OuterScrollViewer;
            //if (sv != null && (_grid.OuterScrollViewerVerticalScrollEnable || _grid.OuterScrollViewerHorizontalScrollEnable))
            //{
            //    if (_grid.OuterScrollViewerVerticalScrollEnable)
            //    {
            //        sz.Height = sv.ActualHeight * 1.5;
            //        var y = _grid.HeaderActualHeight - sv.VerticalOffset;
            //        y += _grid.topToOuterScrollViewer;
            //        r.Row = Rows.GetItemAt(-y - sv.ActualHeight);
            //        r.Row2 = Math.Min(Rows.GetItemAt(sz.Height - y), Rows.Count - 1);
            //    }
            //    else
            //    {
            //        if (fy < sz.Height)
            //        {
            //            sz.Height = sv.ActualHeight;
            //            r.Row = Rows.GetItemAt(fy - scrollPosition.Y);
            //            r.Row2 = Math.Min(Rows.GetItemAt(sz.Height - scrollPosition.Y), Rows.Count - 1);
            //        }
            //    }


            //    if (_grid.OuterScrollViewerHorizontalScrollEnable)
            //    {
            //        // find left/right columns
            //        sz.Width = sv.ActualWidth * 1.5;
            //        var x = -sv.HorizontalOffset;
            //        r.Column = Columns.GetItemAt(-x - sv.ActualWidth);
            //        r.Column2 = Math.Min(Columns.GetItemAt(sz.Width - x), Columns.Count - 1);
            //    }
            //    else
            //    {
            //        if (fx < sz.Width)
            //        {
            //            sz.Width = sv.ActualWidth;
            //            r.Column = Columns.GetItemAt(fx - scrollPosition.X);
            //            r.Column2 = Math.Min(Columns.GetItemAt(sz.Width - scrollPosition.X), Columns.Count - 1);
            //        }
            //    }
            //}
            //else
            {

                // find top/bottom rows
                if (fy < sz.Height)
                {
                    r.Row = Rows.GetItemAt(fy - scrollPosition.Y);
                    r.Row2 = Math.Min(Rows.GetItemAt(sz.Height - scrollPosition.Y), Rows.Count - 1);
                }

                // find left/right columns
                if (fx < sz.Width)
                {
                    r.Column = Columns.GetItemAt(fx - scrollPosition.X);
                    r.Column2 = Math.Min(Columns.GetItemAt(sz.Width - scrollPosition.X), Columns.Count - 1);
                }
            }
            if (r.Row < 0)
            {
                r.Row = 0;
            }
            if (r.Row2 < 0)
            {
                r.Row2 = 0;
            }
            if (r.Column < 0)
            {
                r.Column = 0;
            }
            if (r.Column2 < 0)
            {
                r.Column2 = 0;
            }
            // done
            return r;
        }

        internal bool UpdateViewRange(CellRange viewRange)
        {
            // get old, new ranges
            var oldRange = _viewRange;
            _viewRange = viewRange;

            // re-arrange if they're different
            if (_viewRange != oldRange)
            {
                InvalidateArrange();
                return true;
            }

            // same range
            return false;
        }

        internal bool UpdateViewRange()
        {
            // get old, new ranges
            var oldRange = _viewRange;

            _viewRange = GetViewRange(ScrollPosition);

            // re-arrange if they're different
            if (_viewRange != oldRange)
            {
                InvalidateArrange();
                return true;
            }

            // same range
            return false;
        }

        internal Point ScrollPosition
        {
            get { return _scrollPos; }
            set
            {
                // ScrollPosition values must be <= 0
                if (value.X > 0) value.X = 0;
                if (value.Y > 0) value.Y = 0;

                // apply new value
                if (value != _scrollPos)
                {
                    _scrollPos = value;
                    InvalidateArrange();
                    UpdateViewRange();
                }
            }
        }
        #endregion

    }
}
