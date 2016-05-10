using MyUWPToolkit.DataGrid.Model.Cell;
using MyUWPToolkit.DataGrid.Model.RowCol;
using MyUWPToolkit.DataGrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.DataGrid
{
    public class Row : RowCol
    {

        UnboundRowStorage _urd;
        static FrameworkElement _tbh = new TextBlock();
        private object _item;

        public object DataItem
        {
            get { return _item; }
            set { _item = value; }
        }

        public override DataGrid Grid
        {
            get { return Rows != null ? Rows.Grid : null; }
        }

        public override DataGridPanel GridPanel
        {
            get
            {
                return Rows != null ? Rows.GridPanel : null;
            }
        }

        public int Index
        {
            get
            {
                this.Rows.Update();
                return this.ItemIndex;
            }
        }
        internal Rows Rows
        {
            get { return List as Rows; }
        }

        public object this[Column col]
        {
            get { return GetDataRaw(col); }
            set { SetData(col, value); }
        }

        public object this[string colName]
        {
            get { return GetData(Grid.Columns[colName]); }
            set { SetData(Grid.Columns[colName], value); }
        }

        public object this[int colIndex]
        {
            get { return GetData(Grid.Columns[colIndex]); }
            set { SetData(Grid.Columns[colIndex], value); }
        }

        public double Height
        {
            get { return Size; }
            set { Size = value; }
        }

        protected virtual object GetData(Column col)
        {
            // get bound values
            if (DataItem != null)
            {
                var b = col.Binding;

                if (b != null)
                {
                    return b.Execute<object>(DataItem);
                }
            }

            // get unbound value
            return GetUnboundValue(col);
        }
        /// <summary>
        /// Gets the raw (unformatted) value stored in this row at a given column.
        /// </summary>
        /// <param name="col"><see cref="Column"/> that contains the value.</param>
        /// <returns>Raw value stored in the cell.</returns>
        public virtual object GetDataRaw(Column col)
        {
            // get value from Binding/PropertyInfo
            var value = GetData(col);

            // handle bound values with StringFormat (they become strings)
            if (value is string && DataItem != null &&
                col.Binding != null && !string.IsNullOrEmpty(col.Format))
            {
                col.TryChangeType(ref value);
            }

            return value;
        }
        /// <summary>
        /// Gets the display (formatted) value stored in this row at a given column.
        /// </summary>
        /// <param name="col"><see cref="Column"/> that contains the value.</param>
        /// <returns>Display value stored in the cell.</returns>
        public virtual string GetDataFormatted(Column col)
        {
            var value = GetData(col);
            var ifmt = value as IFormattable;
            if (ifmt != null && !string.IsNullOrEmpty(col.Format))
            {
                try
                {
                    value = ifmt.ToString(col.Format, Grid.GetCultureInfo());
                }
                catch
                {
                    // formatting failed. for example: 
                    // group row counting dates, count can't be formatted as DateTime.
                }
            }
            return value != null ? value.ToString() : string.Empty;
        }
        /// <summary>
        /// Sets the value stored in this row at a given column.
        /// </summary>
        /// <param name="col"><see cref="Column"/> that contains the value.</param>
        /// <param name="value">Value to store in the cell.</param>
        /// <returns>True if a new value was successfully stored in the cell.</returns>
        protected virtual bool SetData(Column col, object value)
        {
            // same value, no work...
            if (object.Equals(GetData(col), value))
            {
                return false;
            }

            var b = col.Binding;
            // set bound data
            if (DataItem != null && b != null)
            {
                try
                {

                    DataItem.SetPropertyValue(b.Path.Path, value, b.Converter, b.ConverterParameter, b.ConverterLanguage);

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    InvalidateCell(col);
                }
            }

            // set (unbound) data directly on row
            return SetUnboundValue(col, value);
        }
        /// <summary>
        /// Gets the unbound value stored in this row at a given column.
        /// </summary>
        /// <param name="col">Column that contains the value.</param>
        /// <returns>The unbound value stored at the given column.</returns>
        protected object GetUnboundValue(Column col)
        {
            // get unbound value from storage, if any
            object value = null;
            if (_urd != null)
            {
                _urd.TryGetValue(col, out value);
            }

            // compute aggregate, if necessary
            
            // return value
            return value;
        }
        /// <summary>
        /// Assigns an unbound value to this row at a given column.
        /// </summary>
        /// <param name="col">Column that contains the value.</param>
        /// <param name="value">Unbound value to be stored at the given column.</param>
        /// <returns>True if the value was stored successfully.</returns>
        protected bool SetUnboundValue(Column col, object value)
        {
            // create storage if necessary
            if (_urd == null)
            {
                _urd = new UnboundRowStorage(this);
            }
            if (!_urd.SetValue(col, value))
            {
                return false;
            }

            // invalidate the cell and return success
            InvalidateCell(col);
            return true;
        }

        /// <summary>
        /// Assigns a bound value to this row at a given column.
        /// </summary>
        /// <param name="col">Column that contains the value.</param>
        /// <param name="value">Value to be stored at the given column.</param>
        internal void SetBoundValue(Column col, object value)
        {
            if (this.DataItem != null && col.Binding != null)
            {
                _tbh.Language = Grid != null ? Grid.Language : null;
                _tbh.DataContext = this.DataItem;
                _tbh.SetBinding(TextBlock.TagProperty, col.Binding);
                _tbh.SetValue(TextBlock.TagProperty, value);
                _tbh.DataContext = null;
            }
        }

        void InvalidateCell(Column c)
        {
            if (GridPanel != null)
            {
                // invalidate if the range is in view (big perf impact!)
                var rng = new CellRange(this.Index, c.Index);
                GridPanel.Invalidate(rng);
            }
        }
        protected override void OnPropertyChanged(string name)
        {
            if (Rows != null)
            {
                Rows.OnPropertyChanged(this, name);
            }
        }
    }
}
