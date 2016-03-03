using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    
    public class RowAdapter<TItemType> : IList<IEnumerable<TItemType>>, INotifyCollectionChanged, ISupportIncrementalLoading
    {
        private readonly IList<TItemType> _sourceList;
        private readonly int _columns;

        public IList<TItemType> SourceList
        {
            get { return _sourceList; }
        }

        private class RowObject : IEnumerable<TItemType>
        {
            internal readonly RowAdapter<TItemType> Parent;
            internal readonly int StartIndex;

            public RowObject(RowAdapter<TItemType> parent, int startIndex)
            {
                Parent = parent;
                StartIndex = startIndex;
            }

            #region IEnumerable<TItemType> Members

            public IEnumerator<TItemType> GetEnumerator()
            {
                int limit = Parent._sourceList.Count;
                int end = Math.Min(StartIndex + Parent._columns, limit);

                for (int pos = StartIndex; pos < end; ++pos)
                {
                    yield return Parent._sourceList[pos];
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

        public RowAdapter(IList<TItemType> sourceList, int columns)
        {
            if (null == sourceList)
                throw new ArgumentNullException("sourceList", "Resource.RowAdapter_RowAdapter_sourceList_is_null");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns", "Resource.RowAdapter_RowAdapter_ColumnsGreaterOne");

            // We require the source list to implement IList because we
            // need to know how many item there are
            _sourceList = sourceList;
            _columns = columns;

            //if (_sourceList is ISupportIncrementalLoading)
            //{
            //    this.LoadMoreItemsAsync(0);
            //}
            var sourceNotify = sourceList as INotifyCollectionChanged;
            if (null != sourceNotify)
            {
                sourceNotify.CollectionChanged += OnSourceCollectionChanged;
            }
        }

        #region IList<IEnumerable<TItemType>> Members

        public int IndexOf(IEnumerable<TItemType> item)
        {
            var realItem = item as RowObject;
            if (null == realItem || !ReferenceEquals(realItem.Parent, this))
                return -1;          // It does not belong to this collection

            Debug.Assert(0 == realItem.StartIndex % _columns, "RowObject item has a wierd index");
            return realItem.StartIndex / _columns;
        }

        public void Insert(int index, IEnumerable<TItemType> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<TItemType> this[int index]
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

        #region ICollection<IEnumerable<TItemType>> Members

        public void Add(IEnumerable<TItemType> item)
        {
            throw new NotSupportedException();
        }

        public bool Contains(IEnumerable<TItemType> item)
        {
            var realItem = item as RowObject;
            return null != realItem && object.ReferenceEquals(realItem.Parent, this);
        }

        public void CopyTo(IEnumerable<TItemType>[] array, int arrayIndex)
        {
            // I haven't implemented this. It is easy to implement if you need it
            throw new NotImplementedException();
        }

        public bool Remove(IEnumerable<TItemType> item)
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
                return (_sourceList.Count + (_columns - 1)) / _columns;
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
                if (_sourceList is ISupportIncrementalLoading)
                {
                    return (_sourceList as ISupportIncrementalLoading).HasMoreItems;
                }
                return false;
            }
        }

        #endregion

        #region IEnumerable<IEnumerable<TItemType>> Members

        public IEnumerator<IEnumerable<TItemType>> GetEnumerator()
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

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void FireCollectionChanged()
        {

            var handler = CollectionChanged;
            if (null != handler)
            {
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FireCollectionChanged();
        }

        #endregion

        private RowObject InternalGetRow(int index)
        {
            return new RowObject(this, index * _columns);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_sourceList is ISupportIncrementalLoading)
            {
                return (_sourceList as ISupportIncrementalLoading).LoadMoreItemsAsync(count);
            }

            return null;
        }
    }


    /// <summary>
    /// only can be used as IObservableVector<Object>,no idea now.
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    public class MyRowAdapter<TItemType> : IObservableVector<IEnumerable<TItemType>>, ISupportIncrementalLoading
    {

        private readonly IList<TItemType> _sourceList;
        private readonly int _columns;


        public MyRowAdapter(IList<TItemType> sourceList, int columns)
        {
            if (null == sourceList)
                throw new ArgumentNullException("sourceList", "Resource.RowAdapter_RowAdapter_sourceList_is_null");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns", "Resource.RowAdapter_RowAdapter_ColumnsGreaterOne");

            // We require the source list to implement IList because we
            // need to know how many item there are
            _sourceList = sourceList;
            _columns = columns;

            //if (_sourceList is ISupportIncrementalLoading)
            //{
            //    this.LoadMoreItemsAsync(0);
            //}
            //var sourceNotify = sourceList as INotifyCollectionChanged;
            //if (null != sourceNotify)
            //{
            //    sourceNotify.CollectionChanged += OnSourceCollectionChanged;
            //}
        }
        public IList<TItemType> SourceList
        {
            get { return _sourceList; }
        }


        public IEnumerable<TItemType> this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                    return null;

                return InternalGetRow(index);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return (_sourceList.Count + (_columns - 1)) / _columns;
            }
        }

        public bool HasMoreItems
        {
            get
            {
                if (_sourceList is ISupportIncrementalLoading)
                {
                    return (_sourceList as ISupportIncrementalLoading).HasMoreItems;
                }
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event VectorChangedEventHandler<IEnumerable<TItemType>> VectorChanged;

        public void Add(IEnumerable<TItemType> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<TItemType> item)
        {
            var realItem = item as RowObject;
            return null != realItem && object.ReferenceEquals(realItem.Parent, this);
        }

        public void CopyTo(IEnumerable<TItemType>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IEnumerable<TItemType>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return InternalGetRow(i);
            }
        }

        public int IndexOf(IEnumerable<TItemType> item)
        {
            var realItem = item as RowObject;
            if (null == realItem || !ReferenceEquals(realItem.Parent, this))
                return -1;          // It does not belong to this collection

            Debug.Assert(0 == realItem.StartIndex % _columns, "RowObject item has a wierd index");
            return realItem.StartIndex / _columns;
        }

        public void Insert(int index, IEnumerable<TItemType> item)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_sourceList is ISupportIncrementalLoading)
            {
                return (_sourceList as ISupportIncrementalLoading).LoadMoreItemsAsync(count);
            }

            return null;
        }

        public bool Remove(IEnumerable<TItemType> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private RowObject InternalGetRow(int index)
        {
            return new RowObject(this, index * _columns);
        }



        #region RowObject
        private class RowObject : IEnumerable<TItemType>
        {
            internal readonly MyRowAdapter<TItemType> Parent;
            internal readonly int StartIndex;

            public RowObject(MyRowAdapter<TItemType> parent, int startIndex)
            {
                Parent = parent;
                StartIndex = startIndex;
            }

            #region IEnumerable<TItemType> Members

            public IEnumerator<TItemType> GetEnumerator()
            {
                int limit = Parent._sourceList.Count;
                int end = Math.Min(StartIndex + Parent._columns, limit);

                for (int pos = StartIndex; pos < end; ++pos)
                {
                    yield return Parent._sourceList[pos];
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

        #endregion
    }
}
