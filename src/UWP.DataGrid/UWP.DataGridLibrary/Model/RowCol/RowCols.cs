using UWP.DataGrid;
using UWP.DataGrid.Model.Cell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.DataGrid.Model.RowCol
{
    public abstract class RowCols<T>
       : ObservableCollection<T> where T : RowCol
    {
        #region Fileds
        DataGridPanel _panel = null;
        
        private int _updating;
        private bool _dirty;
        private int _frozen;
        private int _firstVisible;
        private double _minSize;
        private double _defSize;
        private double _maxSize;
        private double _indent;
        private double _size;
        #endregion

        internal RowCols(DataGridPanel panel, int defaultSize)
        {
            _panel = panel;
            _defSize = defaultSize;
        }
        // deferred notifications
        internal int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                {
                    _dirty = true;
                    OnCollectionChanged();
                }
            }
        }

        internal bool IsDirty
        {
            get
            {
                return _dirty;
            }
        }

        internal void SetIsDirty(bool dirty)
        {
            _dirty = dirty;
        }

        public int Frozen
        {
            get { return _frozen; }
            set
            {
                if (value != _frozen)
                {
                    _frozen = value;
                    OnCollectionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the size (width or height) in pixels
        /// for row and column objects in this collection.
        /// </summary>
        public double DefaultSize
        {
            get { return _defSize; }
            set
            {
                if (value != _defSize)
                {
                    if (value < 0)
                    {
                        throw new Exception("Default size cannot be negative.");
                    }
                    _defSize = value;
                    OnCollectionChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value that indicates the minimum size (width or height) 
        /// in pixels for row and column objects in this collection.
        /// </summary>
        public double MinSize
        {
            get { return _minSize; }
            set
            {
                if (value != _minSize)
                {
                    _minSize = value;
                    OnCollectionChanged();
                }
            }
        }

        public double MaxSize
        {
            get { return _maxSize; }
            set
            {
                if (value != _maxSize)
                {
                    _maxSize = value;
                    OnCollectionChanged();
                }
            }
        }

        internal double Indent
        {
            get { return _indent; }
            set
            {
                if (value != _indent)
                {
                    _indent = Math.Max(value, 0);
                    OnCollectionChanged();
                }
            }
        }

        internal bool IsFrozen(int index)
        {
            return index < Frozen;
        }
        internal double GetFrozenSize()
        {
            var index = Math.Min(Frozen, Count - 1);
            return index > 0 ? GetItemPosition(index) : 0;
        }


        public IDisposable DeferNotifications()
        {
            return new DeferNotification(this);
        }
        internal void OnCollectionChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // connect new items
            if (e.NewItems != null)
            {
                foreach (RowCol rc in e.NewItems)
                {
                    rc.List = this;
                }
            }

            // remember we're dirty
            _dirty = true;

            // fire event unless suspended
            if (_updating <= 0)
            {
                base.OnCollectionChanged(e);
            }
        }

        internal int GetItemAt(double position)
        {
            Update();
            int index = BinarySearch(position);
            index = index < 0 ? ~index - 1 : index;
            while (index > -1 && !this[index].IsVisible)
            {
                index--;
            }
            return index;
        }

        int BinarySearch(double position)
        {
            // binary search
            int low = 0;
            int hi = Count - 1;
            while (low <= hi)
            {
                int mid = (low + hi) / 2;
                int cmp = Math.Sign(this[mid].Position - position);
                if (cmp == 0)
                {
                    return mid;
                }
                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return ~low;
        }

        internal virtual void Update()
        {
            if (_dirty)
            {
                _firstVisible = -1;
                int dataIndex = 0, visIndex = 0;
                double pos = 0;
                for (int index = 0; index < Count; index++)
                {
                    // get the row
                    var item = this[index];
                    var row = item as Row;

                    // update index into rows collection
                    item.ItemIndex = index;
                    item.VisibleIndex = visIndex;

                    // update index into view
                    if (row != null)
                    {
                            item.DataIndex = row is BoundRow
                                ? dataIndex++ // bound rows count
                                : -1; 

                    }

                    // update first visible row
                    if (_firstVisible < 0 && item.IsVisible)
                    {
                        _firstVisible = index;
                    }

                    // update position/visible index (after updating outline info)
                    item.Position = pos;
                    var sz = GetItemSize(index);
                    pos += sz;
                    if (sz > 0) visIndex++;
                }
                _dirty = false;
            }
        }

        internal double GetItemSize(int index)
        {
            return GetItemSize(index, true);
        }

        internal double GetItemSize(int index, bool includeIndent)
        {
            // handle invisible row/col
            var item = this[index];
            if (!item.IsVisible)
            {
                return 0;
            }

            // handle default size
            var sz = item.Size;
            if (sz < 0) sz = _defSize;

            // handle limits
            if (sz < _minSize) sz = _minSize;
            if (_maxSize > 0 && sz > _maxSize)
            {
                sz = _maxSize;
            }

            // handle indent
            if (index == _firstVisible && includeIndent)
            {
                sz += _indent;
            }

            // done
            return sz;
        }


        internal double GetTotalSize()
        {
            var cnt = Count;
            return cnt > 0
                ? GetItemPosition(cnt - 1) + GetItemSize(cnt - 1)
                : 0;
        }
        internal double GetItemPosition(int index)
        {
            Update();
            return this[index].Position;
        }

        internal IEnumerable<int> EnumerateVisibleElements(int first, int last)
        {
            for (int i = 0; i < Frozen && i < Count; i++)
            {
                if (GetItemSize(i) > 0)
                {
                    yield return i;
                }
            }
            if (Frozen > first)
            {
                first = Frozen;
            }
            for (int i = first; i <= last && i < Count; i++)
            {
                if (GetItemSize(i) > 0)
                {
                    yield return i;
                }
            }
        }

       

        internal DataGridPanel GridPanel
        {
            get { return _panel; }
        }
        internal DataGrid Grid
        {
            get { return _panel.Grid; }
        }
        internal CellType CellType
        {
            get { return _panel.CellType; }
        }

        internal int FirstVisibleIndex
        {
            get { return _firstVisible; }
        }
        class DeferNotification : IDisposable
        {
            RowCols<T> _parent;
            public DeferNotification(RowCols<T> parent)
            {
                _parent = parent;
                _parent.Updating++;
            }
            public void Dispose()
            {
                _parent.Updating--;
            }
        }

        internal void OnPropertyChanged(object sender, string propName)
        {
            if (_updating > 0)
            {
                _dirty = true;
                return;
            }

            // optimized handling for some properties
            switch (propName)
            {

                // invalidate arrange only (faster than firing CollectionChanged)
                case "Size":
                    if (Grid != null && CellType == CellType.Cell)
                    {
                        _dirty = true;
                        Grid.InvalidateMeasure();
                        Grid.ColumnHeaders.InvalidateMeasure();
                        Grid.Cells.InvalidateMeasure();
                        return;
                    }
                    break;
            }

            // other properties require OnCollectionChanged (slower, safer)
            OnCollectionChanged();
        }
    }
}
