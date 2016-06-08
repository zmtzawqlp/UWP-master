using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    /// <summary>
    /// Only for VirtualizedVariableSizedGridView control and sourceList must be ISupportIncrementalLoading
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableRowAapter<T> : ObservableCollection<IEnumerable<T>>, ISupportIncrementalLoading, IResizeableItems, IObservableRowAapter
    {
        private RowAdapter<T> rowAdapter = null;
        private readonly IList<T> sourceList;

        public int SourceCount
        {
            get
            {
                return sourceList.Count;
            }
        }
        public int RowItemsCount
        {
            get { return _resizeableItems.RowItemsCount; }
        }

        public ObservableRowAapter(IList<T> sourceList)
        {
            //default
            InitializeResizeableItems();
            rowAdapter = new RowAdapter<T>(sourceList, RowItemsCount);
            this.sourceList = sourceList;
            if (this.sourceList!=null && this.sourceList is INotifyCollectionChanged)
            {
                (this.sourceList as INotifyCollectionChanged).CollectionChanged += ObservableRowAapter_CollectionChanged;
            }
            this.CollectionChanged += ObservableRowAapter_CollectionChanged1;
        }

        private void ObservableRowAapter_CollectionChanged1(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (this.sourceList != null && this.sourceList.Count>0)
                    {
                        this.sourceList.Clear();
                    }
                    break;
                default:
                    break;
            }
        }

        private void ObservableRowAapter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (this.Count>0)
                    {
                        this.Clear();
                    }
                    break;
                default:
                    break;
            }
        }

        private void InitializeResizeableItems()
        {
            if (_resizeableItems == null)
            {
                _resizeableItems = new ResizeableItems();

                //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(200, 200));
               
                double windowMinwidth = 500;
                double windowMaxwidth = DeviceInfo.DeviceScreenSize.Width;
                double rangwidth = (windowMaxwidth - windowMinwidth) / 4.0;

                #region 4
                var list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c4 = new ResizeableItem() { Columns = 4, Items = list, Min = windowMinwidth + rangwidth * 3 + 1, Max = double.PositiveInfinity };
                _resizeableItems.Add(c4);
                #endregion

                #region 3
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c3 = new ResizeableItem() { Columns = 3, Items = list, Min = windowMinwidth + rangwidth * 2 + 1, Max = windowMinwidth + rangwidth * 3 };
                _resizeableItems.Add(c3);
                #endregion

                #region 2
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c2 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + rangwidth * 1 + 1, Max = windowMinwidth + rangwidth * 2 };
                _resizeableItems.Add(c2);
                #endregion

                #region 1
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c1 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + +1, Max = windowMinwidth + rangwidth * 1 };
                _resizeableItems.Add(c1);
                #endregion
            }
        }

        public bool HasMoreItems
        {
            get
            {
                var hasMoreItems = rowAdapter.HasMoreItems;
                //fix Known issue
                //Known issue: if ISupportIncrementalLoading is not Infinitely.
                //if the ISupportIncrementalLoading vestigial items(it means MaxCount % RowItemsCount) less than RowItemsCount, 
                //it will miss the item.
                //has no good solution now.
                if (!hasMoreItems)
                {
                    if (rowAdapter.Count > 0 && this.Count < rowAdapter.Count)
                    {
                        for (int i = this.Count; i < rowAdapter.Count; i++)
                        {
                            //sometime it will miss some indexs in LoadMoreItemsAsync method,
                            //if hasMoreItems is false, that means not more items,
                            //so at that monment we should add the missed items.
                            //if (rowAdapter.SourceList.Count / rowAdapter.rowItemsCount <= i)
                            {
                                var item = this.ElementAtOrDefault(i);
                                if (item == null)
                                {
                                    this.Insert(i, rowAdapter[i]);
                                }
                            }
                        }
                    }
                }
                return hasMoreItems;
            }
        }
        ResizeableItems _resizeableItems;

        public ResizeableItems ResizeableItems
        {
            get
            {
                return _resizeableItems;
            }

            set
            {
                if (_resizeableItems != value)
                {
                    _resizeableItems = value;
                    rowAdapter = new RowAdapter<T>(sourceList, RowItemsCount);
                }
            }
        }


        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            IAsyncOperation<LoadMoreItemsResult> result = rowAdapter.LoadMoreItemsAsync(count);
            if (rowAdapter.Count > 0)
            {
                for (int i = this.Count; i < rowAdapter.Count; i++)
                {
                    if (rowAdapter.SourceList.Count / rowAdapter.rowItemsCount > i)
                    {
                        var item = this.ElementAtOrDefault(i);
                        if (item == null)
                        {
                            this.Insert(i, rowAdapter[i]);
                        }
                    }
                    //has fix in HasMoreItems property
                    //else
                    //{
                    //    this will make UI updating very uncomfortable
                    //    but with out this, it will casue an issue that
                    //    Known issue: if ISupportIncrementalLoading is not Infinitely.
                    //    if the ISupportIncrementalLoading vestigial items(it means MaxCount % RowItemsCount) less than RowItemsCount, 
                    //    it will miss the item.
                    //    has no good solution now.
                    //    var item = this.ElementAtOrDefault(i);
                    //    if (item != null)
                    //    {
                    //        this[i] = rowAdapter[i];
                    //    }
                    //}
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
            CurrentCount = end - StartIndex;
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

        public RowAdapter(IList<T> sourceList, int rowItemsCount)
        {
            if (null == sourceList)
                throw new ArgumentNullException("sourceList", "sourceList can not be null");
            if (rowItemsCount <= 0)
                throw new ArgumentOutOfRangeException("rowItemsCount", "rowItemsCount should be more than one");

            // We require the source list to implement IList because we
            // need to know how many item there are
            items = sourceList;
            this.rowItemsCount = rowItemsCount;

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
            throw new NotSupportedException();
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

    public interface IObservableRowAapter
    {
        int SourceCount { get; }
    }
}
