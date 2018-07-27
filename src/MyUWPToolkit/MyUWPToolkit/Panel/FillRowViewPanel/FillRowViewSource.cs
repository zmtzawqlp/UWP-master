using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public class FillRowViewSource<T> : ObservableCollection<ObservableCollection<T>>, ISupportIncrementalLoading
    {
        public int RowItemsCount { get; private set; }

        private readonly IList<T> sourceList;

        public FillRowViewSource(IList<T> sourceList, int rowItemsCount)
        {
            this.sourceList = sourceList;
            this.RowItemsCount =Math.Min(1, rowItemsCount);
            if (this.sourceList != null && this.sourceList is INotifyCollectionChanged icc)
            {
                icc.CollectionChanged += SourceList_CollectionChanged;
            }
            this.CollectionChanged += FillRowViewSource_CollectionChanged;
        }

        private void SourceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    if (this.Count > 0)
                    {
                        this.Clear();
                    }
                    break;
                default:
                    break;
            }
        }

        private void FillRowViewSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    if (this.sourceList != null && this.sourceList.Count > 0)
                    {
                        this.sourceList.Clear();
                    }
                    break;
                default:
                    break;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            IAsyncOperation<LoadMoreItemsResult> result = null;
            if (this.sourceList is ISupportIncrementalLoading iil)
            {
                result = iil.LoadMoreItemsAsync(count);
            }
            {
                UpdateRowItems();
            }

            return result;
        }

        public void UpdateRowItemsCount(int rowItemsCount)
        {
            if (rowItemsCount != this.RowItemsCount)
            {
                this.RowItemsCount = rowItemsCount;
                UpdateRowItems();
            }
        }

        private void UpdateRowItems()
        {
            if (sourceList == null)
            {
                return;
            }
            int i = 0;
            var rowItems = sourceList.Skip(i * RowItemsCount).Take(RowItemsCount);
            while (rowItems != null && rowItems.Count() != 0)
            {
                var rowItemsCount = rowItems.Count();
                var item = this.ElementAtOrDefault(i);
                if (item == null)
                {
                    item = new ObservableCollection<T>();
                    this.Insert(i, item);
                }

                for (int j = 0; j < rowItemsCount; j++)
                {
                    var rowItem = rowItems.ElementAt(j);
                    var temp = item.ElementAtOrDefault(j);
                    if (temp==null || !temp.Equals(rowItem))
                    {
                        item.Insert(j, rowItem);
                    }
                }

                while (item.Count > rowItemsCount)
                {
                    item.RemoveAt(item.Count - 1);
                }
                i++;
                rowItems = sourceList.Skip(i * RowItemsCount).Take(RowItemsCount);
            }

            var rowCount = sourceList.Count / RowItemsCount + 1;
            while (this.Count > rowCount)
            {
                this.RemoveAt(this.Count - 1);
            }

        }

        public bool HasMoreItems
        {
            get
            {
                if (sourceList == null)
                {
                    return false;
                }

                if (this.sourceList is ISupportIncrementalLoading iil)
                {
                    return iil.HasMoreItems;
                }

                int count = 0;
                foreach (var item in this)
                {
                    count += item.Count;
                }

                return count == sourceList.Count;
            }
        }
    }
}
