using MyUWPToolkit.DataGrid.Model.Cell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.DataGrid.Model.RowCol
{
    internal class UnboundRowStorage : Dictionary<Column, object>
    {
        // ** fields
        Row _row;

        // ** ctor

        /// <summary>
        /// Initializes a new instance of a <see cref="UnboundRowStorage"/>.
        /// </summary>
        /// <param name="row">Row that owns this storage object.</param>
        public UnboundRowStorage(Row row)
        {
            _row = row;
        }

        // ** object model

        // coerce and store value in dictionary
        public bool SetValue(Column col, object value)
        {
            // no change?
            object current;
            if (this.TryGetValue(col, out current) && object.Equals(current, value))
            {
                return false;
            }

            // coerce data if neither row not column are headers
            bool coerce = _row.Rows.CellType == CellType.Cell && col.Columns.CellType == CellType.Cell;

            // do not coerce if this is an unbound row in a bound grid
            if (_row.Rows.Grid.ItemsSource != null && _row.DataItem == null)
            {
                coerce = false;
            }

            // coerce
            if (coerce && value != null)
            {
                // data type provided, honor it
                var type = col.DataType;
                if (type != null && type != typeof(object))
                {
                    if (col.TryChangeType(ref value))
                    {
                        this[col] = value;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // store as is
            this[col] = value;
            return true;
        }
    }
}
