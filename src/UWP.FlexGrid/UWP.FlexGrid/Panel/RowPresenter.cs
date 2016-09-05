using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace UWP.FlexGrid
{
    public class RowPresenter : Panel
    {

        #region fields
        FlexGrid _grid;
        Rows _rows;
        Columns _cols;
        CellRange _viewRange;
        CellType _cellType;
        Point _scrollPos;
        CellRangeDictionary _cells;
        #endregion


        //public RowContainer(Row row, Columns columns)
        //{
        //    _cells = new CellRangeDictionary();
        //}

        public RowPresenter()
        {
            UseLayoutRounding = false;
            _cells = new CellRangeDictionary();
            _viewRange = CellRange.Empty;
        }

        //internal RowPresenter(FlexGrid grid, int rowHeight, int colWidth, CellType cellType)
        //{
        //    _grid = grid;
        //    _cellType = cellType;
        //    _cells = new CellRangeDictionary();
        //    _viewRange = CellRange.Empty;

        //    //Rows = new Rows(this, rowHeight);
        //    //Columns = new Columns(this, colWidth);
        //    //HorizontalAlignment = HorizontalAlignment.Left;
        //    //VerticalAlignment = VerticalAlignment.Top;
        //    UseLayoutRounding = false;
        //}
        #region override

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(_cols.GetTotalSize(), _rows.GetTotalSize());
            return size;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            //UpdateViewRange();
            //CellRangeDictionary showCells = new CellRangeDictionary();
            //foreach (int r in Rows.EnumerateVisibleElements(_viewRange.TopRow, _viewRange.BottomRow))
            //{

            //    var left = _viewRange.LeftColumn;

            //    // enumerate visible columns
            //    foreach (int c in Columns.EnumerateVisibleElements(left, _viewRange.RightColumn))
            //    {
            //        var rng = new CellRange(r, c);
            //        showCells[rng] = null;
            //    }
            //}

            //// remove cells that are shown but shouldn't be
            //List<CellRange> removeCells = new List<CellRange>();
            //foreach (var rng in _cells.Keys)
            //{
            //    if (!showCells.ContainsKey(rng))
            //        removeCells.Add(rng);
            //}


            //// create cells that should be shown but aren't
            //foreach (var rng in showCells.Keys)
            //{
            //    CreateCell(rng, removeCells);
            //}

            //for (int i = 0; i < removeCells.Count; i++)
            //{
            //    RemoveCell(removeCells[i]);
            //}

            //foreach (KeyValuePair<CellRange, FrameworkElement> kvp in _cells)
            //{
            //    // avoid re-creating the range based on element (for performance)
            //    var rng = kvp.Key;
            //    var cell = kvp.Value;

            //    if (rng.Row < 0 || rng.Row >= Rows.Count ||
            //        rng.Column < 0 || rng.Column >= Columns.Count)
            //    {
            //        cell.Visibility = Visibility.Collapsed;
            //    }
            //    else
            //    {
            //        // get rectangle to arrange cell
            //        var rc = GetCellRect(rng);
            //        //if we has footerHeight 
            //        //for example, footerHeight is 30, and row height is 30.
            //        //at the begining, show rows are 38-61,
            //        //after we has footerHeight, we should get 39-61
            //        //and rect is not right at this time, we should rc.Y -= footerHeight;
            //        if (!Rows.IsFrozen(rng.Row))
            //        {
            //            rc.Y -= footerHeight;
            //        }

            //        //fix auto  column,if Width is Auto and AdaptUISize is true, the width will the adapt to UI width.
            //        if (Columns[rng.Column].AdaptUISize)
            //        {
            //            cell.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //            if (cell.DesiredSize.Width > rc.Width)
            //            {
            //                rc.Width = cell.DesiredSize.Width + 5;
            //                Columns[rng.Column].SetSize(rc.Width);
            //                Columns.SetIsDirty(true);
            //                //if Cell has dirty data, we should also update Column
            //                if (CellType == CellType.Cell)
            //                {
            //                    Grid.ColumnHeaders.Invalidate();
            //                }
            //                //Column always render before cell, so we don't need invalidate cell right now.
            //                ////if column has dirty data, we should also update cell
            //                //else
            //                //{
            //                //    Grid.Cells.Invalidate();
            //                //}
            //            }
            //        }

            //        // set clipping to account for frozen rows/columns
            //        if (Rows.Frozen > 0 || Columns.Frozen > 0)
            //        {
            //            cell.Clip = null;
            //            if (!Columns.IsFrozen(rng.Column) || !Rows.IsFrozen(rng.Row))
            //            {
            //                // no clipping yet
            //                var rcClip = new Rect(0, 0, rc.Width, rc.Height);

            //                // clip unfrozen columns
            //                if (!Columns.IsFrozen(rng.Column))
            //                {
            //                    var fx = Columns.GetFrozenSize();
            //                    if (fx > 0 && rc.X < fx)
            //                    {
            //                        rcClip.X = fx - rc.X;
            //                    }
            //                }

            //                // clip unfrozen rows
            //                if (!Rows.IsFrozen(rng.Row))
            //                {
            //                    var fy = Rows.GetFrozenSize();
            //                    if (fy > 0 && rc.Y < fy)
            //                    {
            //                        rcClip.Y = fy - rc.Y;
            //                    }
            //                }

            //                // activate clip
            //                if (rcClip.X > 0 || rcClip.Y > 0)
            //                {
            //                    cell.Clip = new RectangleGeometry() { Rect = rcClip };
            //                }
            //            }
            //        }

            //        // limit editor size to grid cell (so trimming will work)
            //        cell.Width = rc.Width;
            //        cell.Height = rc.Height;
            //        // to finish the layout
            //        cell.Measure(new Size(rc.Width, rc.Height));
            //        cell.Arrange(rc);
            //    }
            //}
            //if (currentPressedRow > -1)
            //{
            //    HandlePointerPressed(currentPressedRow);
            //}
            //else
            //{
            //    HandlePointerOver(currentpointerOverRow);
            //}

            // measure content
            return new Size(_cols.GetTotalSize(), _rows.GetTotalSize());
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

        public FlexGrid Grid
        {
            get { return _grid; }
        }

        #endregion


        #region Method
        private void rowsCols_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //InvalidateMeasure(); // total row/col size may have changed
            ////When a row is added at the end of the collection it is not necessary to remove all the cells. 
            ////This is specially important in incremental loading scenarios.
            //if (e.Action != NotifyCollectionChangedAction.Add ||
            //    e.NewStartingIndex < ViewRange.Row2)
            //{
            //    Invalidate(); // need to re-create and re-arrange cells
            //}
            //else
            //{
            //    Invalidate(new CellRange(e.NewStartingIndex, -1));
            //}
            //UpdateViewRange();
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
            //List<CellRange> removeCells = new List<CellRange>();
            //foreach (var rg in _cells.Keys)
            //{
            //    if (!rg.IsValid || !rng.IsValid || rg.Intersects(rng))
            //        removeCells.Add(rg);
            //}
            //for (int i = 0; i < removeCells.Count; i++)
            //{
            //    RemoveCell(removeCells[i]);
            //}

            //if (removeCells.Count == 0)
            //{
            //    InvalidateArrange();
            //    UpdateViewRange();
            //}
        }

        #endregion
    }
}
