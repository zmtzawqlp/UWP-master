using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public class GroupObservableCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading, IGroupCollection
    {
        private List<IList<T>> souresList;
        private int currentGroupIndex = 0;
        private List<int> firstIndexInEachGroup = new List<int>();
        private List<GroupHeader> groupHeaders;

        public GroupObservableCollection(List<IList<T>> souresList, List<GroupHeader> groupHeaders)
        {
            this.souresList = souresList;
            this.groupHeaders = groupHeaders;
        }

        public bool HasMoreItems
        {
            get
            {
                if (currentGroupIndex < souresList.Count)
                {
                    var source = souresList[currentGroupIndex];
                    if (source is ISupportIncrementalLoading)
                    {
                        if (!(source as ISupportIncrementalLoading).HasMoreItems)
                        {
                            currentGroupIndex++;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (currentGroupIndex == source.Count - 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public List<int> FirstIndexInEachGroup
        {
            get
            {
                return firstIndexInEachGroup;
            }

            set
            {
                firstIndexInEachGroup = value;
            }
        }

        public List<GroupHeader> GroupHeaders
        {
            get
            {
                return groupHeaders;
            }

            set
            {
                groupHeaders = value;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return FetchItems(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> FetchItems(uint count)
        {
            var source = souresList[currentGroupIndex];
            if (source is ISupportIncrementalLoading)
            {
                int firstIndex = source.Count;
                var result = await (source as ISupportIncrementalLoading).LoadMoreItemsAsync(count);
                for (int i = firstIndex; i < source.Count; i++)
                {
                    this.Add(source[i]);
                    if (i == 0)
                    {
                        groupHeaders[currentGroupIndex].FirstIndex = this.Count-1;
                    }
                }
                return result;
            }
            else
            {
                for (int i = 0; i < source.Count; i++)
                {
                    this.Add(source[i]);
                    if (i == 0)
                    {
                        groupHeaders[currentGroupIndex].FirstIndex = this.Count-1;
                    }
                }
                currentGroupIndex++;

                return new LoadMoreItemsResult() { Count = (uint)source.Count };
            }
        }
    }
}
