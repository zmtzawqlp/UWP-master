using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace ToolkitSample.Model
{
    public abstract class IncrementalLoadingBase<T> : System.Collections.ObjectModel.ObservableCollection<T>, ISupportIncrementalLoading
    {
        #region ISupportIncrementalLoading

        public bool HasMoreItems => HasMoreItemsOverride();

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => AsyncInfo.Run(c => LoadMoreItemsAsync(count, c));

        #endregion

        #region Private methods

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken ct)
        {
            uint resultCount = 0;

            try
            {
                ct.ThrowIfCancellationRequested();

                IsLoading = true;
                IsFaulted = false;

                var items = await LoadMoreItemsOverrideAsync(count, ct);

                int baseIndex = Count;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Add(item);
                    }
                }
                resultCount = (uint)(Count - baseIndex);
            }
            catch (OperationCanceledException)
            { }
            catch
            {
                IsFaulted = Count == 0;
            }
            finally
            {
                IsLoading = false;
            }

            return new LoadMoreItemsResult { Count = resultCount };
        }

        #endregion

        #region Overridable methods

        protected abstract bool HasMoreItemsOverride();

        protected abstract Task<IEnumerable<T>> LoadMoreItemsOverrideAsync(uint count, CancellationToken ct);

        #endregion

        #region State

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            protected set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
                }
            }
        }

        private bool _isFaulted;

        public bool IsFaulted
        {
            get => _isFaulted;
            protected set
            {
                if (_isFaulted != value)
                {
                    _isFaulted = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsFaulted)));
                }
            }
        }

        #endregion
    }
}
