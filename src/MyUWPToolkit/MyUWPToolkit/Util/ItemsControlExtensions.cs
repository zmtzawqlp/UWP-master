using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.Util
{
    public static class ItemsControlExtensions
    {
        public static int GetFirstVisibleIndex(this ItemsControl itemsControl)
        {
            // First checking if no items source or an empty one is used
            if (itemsControl.ItemsSource == null)
            {
                return -1;
            }

            var enumItemsSource = itemsControl.ItemsSource as IEnumerable;

            if (enumItemsSource != null && !enumItemsSource.GetEnumerator().MoveNext())
            {
                return -1;
            }

            // Check if a modern panel is used as an items panel
            var sourcePanel = itemsControl.ItemsPanelRoot;

            if (sourcePanel == null)
            {
                throw new InvalidOperationException("Can't get first visible index from an ItemsControl with no ItemsPanel.");
            }

            var isp = sourcePanel as ItemsStackPanel;

            if (isp != null)
            {
                return isp.FirstVisibleIndex;
            }

            var iwg = sourcePanel as ItemsWrapGrid;

            if (iwg != null)
            {
                return iwg.FirstVisibleIndex;
            }

            // Check containers for first one in view
            if (sourcePanel.Children.Count == 0)
            {
                return -1;
            }

            if (itemsControl.ActualWidth == 0 || itemsControl.ActualHeight == 0)
            {
                throw new InvalidOperationException("Can't get first visible index from an ItemsControl that is not loaded or has zero size.");
            }

            for (int i = 0; i < sourcePanel.Children.Count; i++)
            {
                var container = (FrameworkElement)sourcePanel.Children[i];
                var bounds = container.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                if (bounds.Left < itemsControl.ActualWidth &&
                    bounds.Top < itemsControl.ActualHeight &&
                    bounds.Right > 0 &&
                    bounds.Bottom > 0)
                {
                    return itemsControl.IndexFromContainer(container);
                }
            }

            throw new InvalidOperationException();
        }

        public static IEnumerable<object> GetVisibleItems(this ItemsControl itemsControl)
        {
            List<object> list = new List<object>();
            // First checking if no items source or an empty one is used
            if (itemsControl.ItemsSource == null)
            {
                return list;
            }

            var enumItemsSource = itemsControl.ItemsSource as IEnumerable;

            if (enumItemsSource != null && !enumItemsSource.GetEnumerator().MoveNext())
            {
                return list;
            }

            // Check if a modern panel is used as an items panel
            var sourcePanel = itemsControl.ItemsPanelRoot;

            if (sourcePanel == null)
            {
                throw new InvalidOperationException("Can't get items from an ItemsControl with no ItemsPanel.");
            }

            // Check containers for first one in view
            if (sourcePanel.Children.Count == 0)
            {
                return list;
            }

            if (itemsControl.ActualWidth == 0 || itemsControl.ActualHeight == 0)
            {
                throw new InvalidOperationException("Can't get items from an ItemsControl that is not loaded or has zero size.");
            }

            for (int i = 0; i < sourcePanel.Children.Count; i++)
            {
                var container = (FrameworkElement)sourcePanel.Children[i];
                var bounds = container.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                if (bounds.Left < itemsControl.ActualWidth &&
                    bounds.Top < itemsControl.ActualHeight &&
                    bounds.Right > 0 &&
                    bounds.Bottom > 0)
                {
                    list.Add(itemsControl.ItemFromContainer(container));
                }
            }
            return list;
            //throw new InvalidOperationException();
        }

        public static bool IsVisibleIndex(this ItemsControl itemsControl, int index)
        {
            // First checking if no items source or an empty one is used
            if (itemsControl.ItemsSource == null)
            {
                return false;
            }

            var enumItemsSource = itemsControl.ItemsSource as IEnumerable;

            if (enumItemsSource != null && !enumItemsSource.GetEnumerator().MoveNext())
            {
                return false;
            }

            // Check if a modern panel is used as an items panel
            var sourcePanel = itemsControl.ItemsPanelRoot;

            if (sourcePanel == null)
            {
                throw new InvalidOperationException("Can't get index from an ItemsControl with no ItemsPanel.");
            }

            // Check containers for first one in view
            if (sourcePanel.Children.Count == 0)
            {
                return false;
            }

            if (itemsControl.ActualWidth == 0 || itemsControl.ActualHeight == 0)
            {
                throw new InvalidOperationException("Can't get index from an ItemsControl that is not loaded or has zero size.");
            }

            for (int i = 0; i < sourcePanel.Children.Count; i++)
            {
                var container = (FrameworkElement)sourcePanel.Children[i];
                var bounds = container.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                if (bounds.Left < itemsControl.ActualWidth &&
                    bounds.Top < itemsControl.ActualHeight &&
                    bounds.Right > 0 &&
                    bounds.Bottom > 0)
                {
                    if (index == itemsControl.IndexFromContainer(container))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsVisibleItem(this ItemsControl itemsControl, object item)
        {
            // First checking if no items source or an empty one is used
            if (itemsControl.ItemsSource == null)
            {
                return false;
            }

            var enumItemsSource = itemsControl.ItemsSource as IEnumerable;

            if (enumItemsSource != null && !enumItemsSource.GetEnumerator().MoveNext())
            {
                return false;
            }

            // Check if a modern panel is used as an items panel
            var sourcePanel = itemsControl.ItemsPanelRoot;

            if (sourcePanel == null)
            {
                throw new InvalidOperationException("Can't get index from an ItemsControl with no ItemsPanel.");
            }

            // Check containers for first one in view
            if (sourcePanel.Children.Count == 0)
            {
                return false;
            }

            if (itemsControl.ActualWidth == 0 || itemsControl.ActualHeight == 0)
            {
                throw new InvalidOperationException("Can't get index from an ItemsControl that is not loaded or has zero size.");
            }

            for (int i = 0; i < sourcePanel.Children.Count; i++)
            {
                var container = (FrameworkElement)sourcePanel.Children[i];
                var bounds = container.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                if (bounds.Left < itemsControl.ActualWidth &&
                    bounds.Top < itemsControl.ActualHeight &&
                    bounds.Right > 0 &&
                    bounds.Bottom > 0)
                {
                    if (item == itemsControl.ItemFromContainer(container))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static uint GetVisibleItemsCount(this ItemsControl itemsControl)
        {
            uint count = 0;
            // First checking if no items source or an empty one is used
            if (itemsControl.ItemsSource == null)
            {
                return count;
            }

            var enumItemsSource = itemsControl.ItemsSource as IEnumerable;

            if (enumItemsSource != null && !enumItemsSource.GetEnumerator().MoveNext())
            {
                return count;
            }

            // Check if a modern panel is used as an items panel
            var sourcePanel = itemsControl.ItemsPanelRoot;

            if (sourcePanel == null)
            {
                throw new InvalidOperationException("Can't get items from an ItemsControl with no ItemsPanel.");
            }

            // Check containers for first one in view
            if (sourcePanel.Children.Count == 0)
            {
                return count;
            }

            if (itemsControl.ActualWidth == 0 || itemsControl.ActualHeight == 0)
            {
                throw new InvalidOperationException("Can't get items from an ItemsControl that is not loaded or has zero size.");
            }

            for (int i = 0; i < sourcePanel.Children.Count; i++)
            {
                var container = (FrameworkElement)sourcePanel.Children[i];
                var bounds = container.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                if (bounds.Left < itemsControl.ActualWidth &&
                    bounds.Top < itemsControl.ActualHeight &&
                    bounds.Right > 0 &&
                    bounds.Bottom > 0)
                {
                    count++;
                }
            }
            return count;
            //throw new InvalidOperationException();
        }
    }
}
