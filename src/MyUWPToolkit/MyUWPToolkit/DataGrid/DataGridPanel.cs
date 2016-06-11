using MyUWPToolkit.DataGrid.Model.Cell;
using MyUWPToolkit.DataGrid.Model.RowCol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Collections.Specialized;
using System.Threading;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Devices.Input;

namespace MyUWPToolkit.DataGrid
{
    public class DataGridPanel : Panel
    {
        #region ** fields

        DataGrid _grid;
        Rows _rows;
        Columns _cols;
        CellRange _viewRange;
        CellType _cellType;
        Point _scrollPos;
        CellRangeDictionary _cells;
        #endregion


        #region ctor
        internal DataGridPanel(DataGrid grid, CellType cellType, int rowHeight, int colWidth)
        {
            _grid = grid;
            _cellType = cellType;
            _cells = new CellRangeDictionary();
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
        /// Gets the collection of rows in this <see cref="DataGridPanel"/>.
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
                Children.Clear();
            }
        }


        /// <summary>
        /// Gets the collection of columns in this <see cref="DataGridPanel"/>.
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

        public DataGrid Grid
        {
            get { return _grid; }
        }

        public object this[int row, Column col]
        {
            get { return Rows[row][col]; }
            set
            {
                // this setter invalidates the Cells panel
                Rows[row][col] = value;

                // if this is not the Cells panel, then invalidate ourselves
                if (this.CellType != CellType.Cell)
                {
                    Invalidate(new CellRange(row, col.Index));
                }
            }
        }
        /// <summary>
        /// Gets or sets the value of a specific cell.
        /// </summary>
        /// <param name="row">Index of the row that contains the cell.</param>
        /// <param name="col">Index of the column that contains the cell.</param>
        /// <returns>The value of the cell.</returns>
        public object this[int row, int col]
        {
            get { return this[row, Columns[col]]; }
            set { this[row, Columns[col]] = value; }
        }
        /// <summary>
        /// Gets or sets the value of a specific cell.
        /// </summary>
        /// <param name="row">Index of the row that contains the cell.</param>
        /// <param name="colName"><see cref="Column.ColumnName"/> of the column that contains the cell.</param>
        /// <returns>The value of the cell.</returns>
        public object this[int row, string colName]
        {
            get { return this[row, Columns[colName]]; }
            set { this[row, Columns[colName]] = value; }
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

            // REVIEW: not sure why this is needed
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
            for (int i = 0; i < removeCells.Count; i++)
            {
                RemoveCell(removeCells[i]);
            }

            // create cells that should be shown but aren't
            foreach (var rng in showCells.Keys)
            {
                CreateCell(rng);
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
            if (currentPressedRow > -1)
            {
                HandlePointerPressed(currentPressedRow);
            }
            else
            {
                HandlePointerOver(currentpointerOverRow);
            }

            // measure content
            return new Size(_cols.GetTotalSize(), _rows.GetTotalSize());
        }

        void CreateCell(CellRange rng)
        {
            if (!_cells.ContainsKey(rng))
            {
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

            // find top/bottom rows
            var r = new CellRange(Rows.Frozen, Columns.Frozen);
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

            // done
            return r;
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

        CellRangeDictionary _pressedCells = new CellRangeDictionary();
        internal int currentPressedRow = -1;
        internal void HandlePointerPressed(int row)
        {
            _pressedCells.Clear();
            if (row > -1)
            {
                currentPressedRow = row;
                foreach (var item in _cells)
                {
                    if (item.Key.Row == row)
                    {
                        var cell = (item.Value as Border);

                        FrameworkElement element = null;
                        if (cell.Child != null)
                        {
                            element = cell.Child as FrameworkElement;
                        }
                        else
                        {
                            element = cell;
                        }

                        if (item.Key.Column == 0)
                        {
                            element.RenderTransform = new ScaleTransform() { ScaleX = 0.9, ScaleY = 0.9, CenterX = element.ActualWidth, CenterY = element.ActualHeight / 2 };
                        }
                        else if (item.Key.Column == this.Columns.Count - 1)
                        {
                            element.RenderTransform = new ScaleTransform() { ScaleX = 0.9, ScaleY = 0.9, CenterX = 0, CenterY = element.ActualHeight / 2 };
                        }
                        else
                        {
                            element.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 0.9, CenterX = 0, CenterY = element.ActualHeight / 2 };
                        }
                        cell.Background = Grid.PressedBackground;
                        _pressedCells.Add(item.Key, item.Value);
                    }
                }
            }

        }

        internal void ClearPointerPressedAnimation()
        {
            currentPressedRow = -1;
            foreach (var item in _pressedCells)
            {
                var cell = (item.Value as Border);

                FrameworkElement element = null;
                if (cell.Child != null)
                {
                    element = cell.Child as FrameworkElement;
                }
                else
                {
                    element = cell;
                }

                element.RenderTransform = null;

                var even = true;
                if (Grid.Rows.Count > item.Key.Row)
                {
                    even = Grid.Rows[item.Key.Row].VisibleIndex % 2 == 0;
                }
                cell.Background = even || Grid.AlternatingRowBackground == null
                       ? Grid.RowBackground
                       : Grid.AlternatingRowBackground;


            }
            _pressedCells.Clear();
        }

        CellRangeDictionary _pointerOverCells = new CellRangeDictionary();
        internal int currentpointerOverRow = -1;
        internal void HandlePointerOver(int row)
        {
            if (Grid.Rows.Count <= row)
            {
                _pointerOverCells.Clear();
                return;
            }
            //clear all
            {
                currentpointerOverRow = -1;
                foreach (var item in _pointerOverCells)
                {
                    var element = item.Value as Border;
                    var even = true;
                    if (Grid.Rows.Count > item.Key.Row)
                    {
                        even = Grid.Rows[item.Key.Row].VisibleIndex % 2 == 0;      
                    }
                    element.Background = even || Grid.AlternatingRowBackground == null
                           ? Grid.RowBackground
                           : Grid.AlternatingRowBackground;
                }
                _pointerOverCells.Clear();
            }

            if (row > -1)
            {
                currentpointerOverRow = row;
                foreach (var item in _cells)
                {
                    var element = item.Value as Border;
                    if (item.Key.Row == row)
                    {
                        element.Background = Grid.PointerOverBackground;
                        _pointerOverCells.Add(item.Key, item.Value);
                    }

                }
            }


        }
        #endregion

    }
}
