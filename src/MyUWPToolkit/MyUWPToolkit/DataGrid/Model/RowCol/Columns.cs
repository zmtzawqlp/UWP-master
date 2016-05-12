using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.DataGrid.Model.RowCol
{
    public class Columns : RowCols<Column>
    {
        #region ** fields

        double _starCount;
        double _availableSize;

        #endregion
        public Column this[string colName]
        {
            get { return this[this.IndexOf(colName)]; }
        }
        public int IndexOf(string colName)
        {
            // look by name
            for (int i = 0; i < Count; i++)
            {
                if (string.Equals(colName, this[i].ColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            // not found, look by binding
            for (int i = 0; i < Count; i++)
            {
                if (string.Equals(colName, this[i].BoundPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            // not found
            return -1;
        }
        internal Columns(DataGridPanel panel, int defaultSize) : base(panel, defaultSize)
        {
        }

        internal override void Update()
        {
            if (IsDirty)
            {
                // convert lengths into sizes
                _starCount = 0;
                double requiredSize = 0;
                Column lastCol = null;
                for (int index = 0; index < Count; index++)
                {
                    var c = this[index];
                    if (c.IsVisible)
                    {
                        if (c.Width.IsStar)
                        {
                            _starCount += c.Width.Value;
                            lastCol = c;
                        }
                        else if (c.Width.IsAuto)
                        {
                            var cw = Math.Max(c.MinWidth, Math.Min(c.MaxWidth, this.DefaultSize));
                            c.SetSize(cw);
                            requiredSize += GetItemSize(index);
                        }
                        else // explicit size
                        {
                            // save new size
                            var cw = Math.Max(c.MinWidth, Math.Min(c.MaxWidth, c.Width.Value));
                            c.SetSize(cw);
                            requiredSize += GetItemSize(index);
                        }
                    }
                }

                if (_starCount > 0 && Grid != null && _availableSize > 0)
                {
                    var totalSize = _availableSize - requiredSize;
                    if (FirstVisibleIndex > -1 && this[FirstVisibleIndex].Width.IsStar)
                    {
                        totalSize -= Indent;
                    }

                    // compute star value
                    var starValue = totalSize > 0 ? totalSize / _starCount : 0;

                    // apply star value
                    var szLast = totalSize;
                    foreach (var c in this)
                    {
                        if (c.IsVisible && c.Width.IsStar)
                        {
                            if (c == lastCol) // to avoid round-off errors
                            {
                                c.SetSize(Math.Max(c.MinWidth, Math.Min(c.MaxWidth, szLast)));
                            }
                            else
                            {
                                var cw = c.Width.Value * starValue;
                                cw = Math.Max(c.MinWidth, Math.Min(c.MaxWidth, cw));
                                cw = Math.Round(cw);
                                c.SetSize(cw);
                                szLast -= cw;
                            }
                        }
                    }
                }

                // let base do its stuff
                base.Update();
            }
        }

        internal void UpdateStarSizes(double sz)
        {
            if (_starCount > 0 && sz != _availableSize)
            {
                _availableSize = sz;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
