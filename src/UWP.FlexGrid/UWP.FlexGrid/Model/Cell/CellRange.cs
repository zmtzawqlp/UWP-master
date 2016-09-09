using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UWP.FlexGrid
{
    public struct CellRange
    {
        int _row, _col, _row2, _col2;
        static CellRange _empty = new CellRange(-1, -1);

        /// <summary>
        /// Initializes a new instance of a <see cref="CellRange"/>.
        /// </summary>
        /// <param name="row1">Index of the first row in the range.</param>
        /// <param name="col1">Index of the last row in the range.</param>
        /// <param name="row2">Index of the first column in the range.</param>
        /// <param name="col2">Index of the last column in the range.</param>
        public CellRange(int row1, int col1, int row2, int col2)
        {
            _row = row1;
            _col = col1;
            _row2 = row2;
            _col2 = col2;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="CellRange"/>. containing a single cell.
        /// </summary>
        public CellRange(int row, int col) : this(row, col, row, col) { }


        #region Properties
        /// <summary>
        /// Gets or sets the index of the first row in this <see cref="CellRange"/>.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        /// <summary>
        /// Gets or sets the index of the first column in this <see cref="CellRange"/>.
        /// </summary>
        public int Column
        {
            get { return _col; }
            set { _col = value; }
        }
        /// <summary>
        /// Gets or sets the index of the last row in this <see cref="CellRange"/>.
        /// </summary>
        public int Row2
        {
            get { return _row2; }
            set { _row2 = value; }
        }
        /// <summary>
        /// Gets or sets the index of the last column in this <see cref="CellRange"/>.
        /// </summary>
        public int Column2
        {
            get { return _col2; }
            set { _col2 = value; }
        }
        /// <summary>
        /// Gets or sets the number of rows in <see cref="CellRange"/>.
        /// </summary>
        public int RowSpan
        {
            get { return Math.Abs(_row2 - _row) + 1; }
            set { _row2 = value + _row - 1; }
        }
        /// <summary>
        /// Gets or sets the number of columns in <see cref="CellRange"/>.
        /// </summary>
        public int ColumnSpan
        {
            get { return Math.Abs(_col2 - _col) + 1; }
            set { _col2 = value + _col - 1; }
        }
        /// <summary>
        /// Gets the index of the top row in this <see cref="CellRange"/>.
        /// </summary>
        /// <remarks>
        /// This value is the lower of <see cref="Row"/> or <see cref="Row2"/>. 
        /// </remarks>
        public int TopRow
        {
            get { return Math.Min(_row, _row2); }
        }
        /// <summary>
        /// Gets the index of the bottom row in this <see cref="CellRange"/>.
        /// </summary>
        /// <remarks>
        /// This value is the higher of <see cref="Row"/> or <see cref="Row2"/>. 
        /// </remarks>
        public int BottomRow
        {
            get { return Math.Max(_row, _row2); }
        }
        /// <summary>
        /// Gets the index of the left column in this <see cref="CellRange"/>.
        /// </summary>
        /// <remarks>
        /// This value is the lower of <see cref="Column"/> or <see cref="Column2"/>. 
        /// </remarks>
        public int LeftColumn
        {
            get { return Math.Min(_col, _col2); }
        }
        /// <summary>
        /// Gets the index of the right column in this <see cref="CellRange"/>.
        /// </summary>
        /// <remarks>
        /// This value is the higher of <see cref="Column"/> or <see cref="Column2"/>. 
        /// </remarks>
        public int RightColumn
        {
            get { return Math.Max(_col, _col2); }
        }

        public static CellRange Empty
        {
            get { return _empty; }
        }

        public bool IsValid
        {
            get { return _row > -1 && _col > -1 && _row2 > -1 && _col2 > -1; }
        }

        public bool Intersects(CellRange rng)
        {
            return Intersection(rng).IsValid;
        }

        public CellRange Intersection(CellRange rng)
        {
            int r1 = Math.Max(TopRow, rng.TopRow);
            int r2 = Math.Min(BottomRow, rng.BottomRow);
            if (r1 > r2)
            {
                r1 = r2 = -1;
            }
            int c1 = Math.Max(LeftColumn, rng.LeftColumn);
            int c2 = Math.Min(RightColumn, rng.RightColumn);
            if (c1 > c2)
            {
                c1 = c2 = -1;
            }
            return new CellRange(r1, c1, r2, c2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// Equality operator for <see cref="CellRange"/> objects.
        /// </summary>
        public static bool operator ==(CellRange r1, CellRange r2)
        {
            return r1.Equals(r2);
        }
        /// <summary>
        /// Inequality operator for <see cref="CellRange"/> objects.
        /// </summary>
        public static bool operator !=(CellRange r1, CellRange r2)
        {
            return !r1.Equals(r2);
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            // copied from System.Drawing.Rectangle:
            return (
                ((_col ^ ((_row << 13) | (_row >> 0x13))) ^
                ((_col2 << 0x1a) | (_col2 >> 6))) ^
                ((_row2 << 7) | (_row2 >> 0x19)));
        }

        public bool IsSingleCell
        {
            get { return _row == _row2 && _col == _col2; }
        }
        #endregion
    }

    internal class CellRangeDictionary : Dictionary<CellRange, FrameworkElement>
    {
    }

    internal class RowsDictionary : Dictionary<Row, FlexGridRow>
    {
    }
}
