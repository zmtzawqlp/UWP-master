using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public class ObservableRowAapter<T> : ObservableCollection<IEnumerable<T>>, ISupportIncrementalLoading
    {
        private RowAdapter<T> rowAdapter = null;

        public ObservableRowAapter(IList<T> sourceList, int columns)
        {
            rowAdapter = new RowAdapter<T>(sourceList, columns);
        }

        public bool HasMoreItems
        {
            get
            {
                return rowAdapter.HasMoreItems;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            IAsyncOperation<LoadMoreItemsResult> result = rowAdapter.LoadMoreItemsAsync(count);
            if (rowAdapter.Count > 0)
            {
                
                for (int i = 0; i < rowAdapter.Count; i++)
                {

                    if (rowAdapter.SourceList.Count / rowAdapter.rowItemsCount > i)
                    {
                        var item = this.ElementAtOrDefault(i);
                        if (item == null)
                        {
                            this.Insert(i, rowAdapter[i]);
                        }
                    }
                }
            }

            return result;
        }
    }

    internal class RowObject<T> : IEnumerable<T>
    {
        internal readonly RowAdapter<T> Parent;
        internal readonly int StartIndex;

        public RowObject(RowAdapter<T> parent, int startIndex)
        {
            Parent = parent;
            StartIndex = startIndex;
        }

        #region IEnumerable<T> Members
        public int CurrentCount = 0;

        public IEnumerator<T> GetEnumerator()
        {
            int limit = Parent.SourceList.Count;
            int end = Math.Min(StartIndex + Parent.rowItemsCount, limit);
            CurrentCount = end-StartIndex;
            for (int pos = StartIndex; pos < end; ++pos)
            {
                yield return Parent.SourceList[pos];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    internal class RowAdapter<T> : IList<IEnumerable<T>>, ISupportIncrementalLoading
    {
        private readonly IList<T> items;
        public readonly int rowItemsCount;

        public IList<T> SourceList
        {
            get { return items; }
        }

        public RowAdapter(IList<T> sourceList, int columns)
        {
            if (null == sourceList)
                throw new ArgumentNullException("sourceList", "Resource.RowAdapter_RowAdapter_sourceList_is_null");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns", "Resource.RowAdapter_RowAdapter_ColumnsGreaterOne");

            // We require the source list to implement IList because we
            // need to know how many item there are
            items = sourceList;
            rowItemsCount = columns;

        }

        #region IList<IEnumerable<T>> Members

        public int IndexOf(IEnumerable<T> item)
        {
            var realItem = item as RowObject<T>;
            if (null == realItem || !ReferenceEquals(realItem.Parent, this))
                return -1;          // It does not belong to this collection

            Debug.Assert(0 == realItem.StartIndex % rowItemsCount, "RowObject item has a wierd index");
            return realItem.StartIndex / rowItemsCount;
        }

        public void Insert(int index, IEnumerable<T> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<T> this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                    return null;

                return InternalGetRow(index);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<IEnumerable<T>> Members

        public void Add(IEnumerable<T> item)
        {

        }

        public bool Contains(IEnumerable<T> item)
        {
            var realItem = item as RowObject<T>;
            return null != realItem && object.ReferenceEquals(realItem.Parent, this);
        }

        public void CopyTo(IEnumerable<T>[] array, int arrayIndex)
        {
            // I haven't implemented this. It is easy to implement if you need it
            throw new NotImplementedException();
        }

        public bool Remove(IEnumerable<T> item)
        {
            throw new NotSupportedException();
        }
        public void Clear()
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get
            {
                return (items.Count + (rowItemsCount - 1)) / rowItemsCount;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool HasMoreItems
        {
            get
            {
                if (items is ISupportIncrementalLoading)
                {
                    return (items as ISupportIncrementalLoading).HasMoreItems;
                }
                return false;
            }
        }

        #endregion

        #region IEnumerable<IEnumerable<T>> Members

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return InternalGetRow(i);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private RowObject<T> InternalGetRow(int index)
        {
            return new RowObject<T>(this, index * rowItemsCount);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (items is ISupportIncrementalLoading)
            {
                IAsyncOperation<LoadMoreItemsResult> result = (items as ISupportIncrementalLoading).LoadMoreItemsAsync(count);
                return result;
            }

            return null;
        }
    }

}
