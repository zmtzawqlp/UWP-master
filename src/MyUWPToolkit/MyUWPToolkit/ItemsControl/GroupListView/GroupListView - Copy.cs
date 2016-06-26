using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{
    ///// <summary>
    ///// Group ListView to support each group ISupportIncrementalLoading and UI Virtualized
    ///// </summary>
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "ProgressRing", Type = typeof(ProgressRing))]
    public class GroupListView : ListView
    {
        private ContentControl currentTopGroupHeader;
        private ScrollViewer scrollViewer;
        private Grid groupHeadersGrid;
        private ProgressRing progressRing;
        Dictionary<IGroupHeader, ContentControl> visibleGroupHeaders;
        private double groupHeaderDelta = 30;
        private Thickness defaultListViewItemMargin = new Thickness(0);
        private bool isGotoGrouping = false;
        #region Property

        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(GroupListView), new PropertyMetadata(0));



        #endregion
        private IGroupCollection groupCollection;

        public new object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(GroupListView), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GroupListView).OnItemsSourceChanged();
        }


        internal void OnItemsSourceChanged()
        {
            if (this.ItemsSource != null && ItemsSource is IGroupCollection)
            {
                groupCollection = (ItemsSource as IGroupCollection);
            }

            if (groupHeadersGrid != null)
            {
                groupHeadersGrid.Children.Clear();
            }
            visibleGroupHeaders.Clear();
            base.ItemsSource = this.ItemsSource;
        }


        public GroupListView()
        {
            this.DefaultStyleKey = typeof(GroupListView);
            visibleGroupHeaders = new Dictionary<IGroupHeader, ContentControl>();
        }

        protected override void OnApplyTemplate()
        {
            scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            progressRing = GetTemplateChild("ProgressRing") as ProgressRing;
            scrollViewer.Loaded += scrollViewer_Loaded;
            base.OnApplyTemplate();
        }

        private void scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer.Loaded -= scrollViewer_Loaded;
            groupHeadersGrid = this.scrollViewer.FindDescendantByName("GroupHeadersGrid") as Grid;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            groupHeadersGrid.Loaded += GroupHeadersGrid_Loaded;
        }

        private void GroupHeadersGrid_Loaded(object sender, RoutedEventArgs e)
        {
            groupHeadersGrid.Loaded -= GroupHeadersGrid_Loaded;
            foreach (var item in visibleGroupHeaders)
            {
                if (item.Value.Parent == null)
                {
                    var groupheader = item.Value;

                    groupHeadersGrid.Children.Add(groupheader);

                    groupheader.Measure(new Windows.Foundation.Size(this.ActualWidth, this.ActualHeight));

                    item.Key.Height = groupheader.DesiredSize.Height;

                    var listViewItem = ContainerFromIndex(item.Key.FirstIndex) as ListViewItem;
                    listViewItem.Tag = listViewItem.Margin;
                    listViewItem.Margin = GetItemMarginBaseOnDeafult(item.Key.Height);
                    groupheader.Visibility = Visibility.Collapsed;
                    UpdateGroupHeaders();
                }
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateGroupHeaders();
            isGotoGrouping = false;
            this.IsHitTestVisible = true;
        }

        private void UpdateGroupHeaders()
        {
            var firstVisibleItemIndex = this.GetFirstVisibleIndex();
            foreach (var item in visibleGroupHeaders)
            {
                //top header
                if (item.Key.FirstIndex <= firstVisibleItemIndex && (firstVisibleItemIndex <= item.Key.LastIndex || item.Key.LastIndex == -1))
                {
                    item.Value.Visibility = Visibility.Visible;
                    item.Value.Margin = new Thickness(0);
                    item.Value.Clip = null;
                    currentTopGroupHeader = item.Value;
                }
                else
                {
                    ListViewItem listViewItem = ContainerFromIndex(item.Key.FirstIndex) as ListViewItem;

                    if (listViewItem == null && item.Key.LastIndex != -1)
                    {
                        listViewItem = ContainerFromIndex(item.Key.LastIndex) as ListViewItem;
                    }
                    if (listViewItem != null)
                    {
                        //handle moving header
                        {
                            //unloaded
                            if (listViewItem.ActualHeight == 0 || listViewItem.ActualWidth == 0)
                            {
                                listViewItem.Loaded += ListViewItem_Loaded;
                            }
                            else
                            {
                                GeneralTransform gt = listViewItem.TransformToVisual(this);
                                var rect = gt.TransformBounds(new Rect(0, 0, listViewItem.ActualWidth, listViewItem.ActualHeight));
                                groupHeaderDelta = item.Key.Height;
                                //add delta,so that it does not look like suddenly
                                if (rect.Bottom + groupHeaderDelta < 0 || rect.Top > this.ActualHeight + groupHeaderDelta)
                                {
                                    item.Value.Visibility = Visibility.Collapsed;
                                    item.Value.Margin = new Thickness(0);
                                    item.Value.Clip = null;
                                }
                                //in view port
                                else
                                {
                                    item.Value.Visibility = Visibility.Visible;
                                    item.Value.Margin = new Thickness(0, rect.Top - groupHeaderDelta - defaultListViewItemMargin.Top, 0, 0);
                                    if (item.Value.Margin.Top < 0)
                                    {
                                        var clipHeight = groupHeaderDelta + item.Value.Margin.Top;
                                        //moving header has part in viewport
                                        if (clipHeight > 0)
                                        {
                                            item.Value.Clip = new RectangleGeometry() { Rect = new Rect(0, -item.Value.Margin.Top, this.ActualWidth, clipHeight) };
                                        }
                                        //moving header not in viewport
                                        else
                                        {
                                            item.Value.Visibility = Visibility.Collapsed;
                                            item.Value.Clip = null;
                                        }
                                    }
                                    else if (item.Value.Margin.Top + groupHeaderDelta > this.ActualHeight)
                                    {

                                        var clipHeight = groupHeaderDelta - (groupHeaderDelta + item.Value.Margin.Top - this.ActualHeight);
                                        //moving header has part in viewport
                                        if (clipHeight > 0)
                                        {
                                            item.Value.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, clipHeight) };
                                        }
                                        //moving header not in viewport
                                        else
                                        {
                                            item.Value.Visibility = Visibility.Collapsed;
                                            item.Value.Clip = null;
                                        }
                                    }
                                    //moving header all in viewport
                                    else
                                    {
                                        item.Value.Clip = null;
                                    }

                                    if (currentTopGroupHeader != null)
                                    {
                                        var delta = currentTopGroupHeader.ActualHeight - (item.Value.Margin.Top);
                                        if (delta > 0)
                                        {
                                            currentTopGroupHeader.Margin = new Thickness(0, -delta, 0, 0);
                                            currentTopGroupHeader.Clip = new RectangleGeometry() { Rect = new Rect(0, delta, currentTopGroupHeader.ActualWidth, currentTopGroupHeader.ActualHeight) };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item.Value != currentTopGroupHeader)
                        {
                            item.Value.Visibility = Visibility.Collapsed;
                            item.Value.Margin = new Thickness(0);
                            item.Value.Clip = null;
                        }

                    }
                }
            }
        }

        private void ListViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ListViewItem).Loaded -= ListViewItem_Loaded;
            UpdateGroupHeaders();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            base.PrepareContainerForItemOverride(element, item);
            ListViewItem listViewItem = element as ListViewItem;
            if (listViewItem.Tag == null)
            {
                defaultListViewItemMargin = listViewItem.Margin;
            }

            if (groupCollection != null && listViewItem != null)
            {
                var index = IndexFromContainer(element);
                var group = groupCollection.GroupHeaders.FirstOrDefault(x => x.FirstIndex == index || x.LastIndex == index);
                if (group != null)
                {

                    //if has exist,remove from groupHeadersGrid first.
                    if (visibleGroupHeaders.ContainsKey(group) && visibleGroupHeaders[group].Parent != null)
                    {
                        //remove binding
                        visibleGroupHeaders[group].ClearValue(ContentControl.ContentTemplateProperty);
                        (visibleGroupHeaders[group].Parent as Grid).Children.Remove(visibleGroupHeaders[group]);
                        visibleGroupHeaders[group] = null;
                    }

                    ContentControl groupheader = new ContentControl();
                    Binding binding = new Binding();
                    binding.Source = this;
                    binding.Mode = BindingMode.OneWay;
                    binding.Path = new PropertyPath("GroupHeaderTemplate");
                    groupheader.SetBinding(ContentControl.ContentTemplateProperty, binding);
                    groupheader.DataContext = group;
                    groupheader.HorizontalAlignment = HorizontalAlignment.Stretch;
                    groupheader.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    groupheader.VerticalAlignment = VerticalAlignment.Top;
                    groupheader.VerticalContentAlignment = VerticalAlignment.Stretch;
                    visibleGroupHeaders[group] = groupheader;
                    //top header alway at ahead, so that we should order by firstIndex,
                    //so that in UpdateGroupHeaders() method, we can find TopHeader correctly.                    
                    var temp = new Dictionary<IGroupHeader, ContentControl>();
                    foreach (var keyValue in visibleGroupHeaders.OrderBy(x => x.Key.FirstIndex))
                    {
                        temp[keyValue.Key] = keyValue.Value;
                    }
                    visibleGroupHeaders = temp;
                    if (groupHeadersGrid != null)
                    {
                        groupHeadersGrid.Children.Add(groupheader);

                        groupheader.Measure(new Windows.Foundation.Size(this.ActualWidth, this.ActualHeight));

                        group.Height = groupheader.DesiredSize.Height;

                        if (group.FirstIndex == index)
                        {
                            listViewItem.Tag = listViewItem.Margin;
                            listViewItem.Margin = GetItemMarginBaseOnDeafult(groupheader.DesiredSize.Height);
                        }

                        groupheader.Visibility = Visibility.Collapsed;

                        UpdateGroupHeaders();
                    }
                }
                else
                {
                    listViewItem.Margin = defaultListViewItemMargin;
                }
            }
            else
            {
                listViewItem.Margin = defaultListViewItemMargin;
            }
        }

        Thickness GetItemMarginBaseOnDeafult(double heightdelta)
        {
            return new Thickness(defaultListViewItemMargin.Left, defaultListViewItemMargin.Top + heightdelta, defaultListViewItemMargin.Right, defaultListViewItemMargin.Bottom);
        }

        private int GetCurrentVisibleGroupIndex()
        {
            var currentGroupIndex = 0;
            if (groupCollection != null)
            {
                if (currentTopGroupHeader != null)
                {
                    foreach (var item in visibleGroupHeaders)
                    {
                        if (currentTopGroupHeader == item.Value)
                        {
                            currentGroupIndex = groupCollection.GroupHeaders.IndexOf(item.Key);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Assert(false, "why not has top Group");
                    var firstVisibleItemIndex = this.GetFirstVisibleIndex();
                    foreach (var item in visibleGroupHeaders)
                    {
                        if (item.Key.FirstIndex <= firstVisibleItemIndex && (firstVisibleItemIndex <= item.Key.LastIndex || item.Key.LastIndex == -1))
                        {
                            currentGroupIndex = groupCollection.GroupHeaders.IndexOf(item.Key);
                            break;
                        }
                    }
                }
            }
            return currentGroupIndex;
        }


        public async Task GoToNextGroupAsync(ScrollIntoViewAlignment scrollIntoViewAlignment = ScrollIntoViewAlignment.Leading)
        {
            if (groupCollection != null)
            {
                var gc = groupCollection;

                var currentGroupIndex = GetCurrentVisibleGroupIndex();

                if (currentGroupIndex + 1 < gc.GroupHeaders.Count)
                {
                    currentGroupIndex++;
                }
                else
                {
                    currentGroupIndex = 0;
                }

                await GoToGroupAsync(currentGroupIndex, scrollIntoViewAlignment);
            }
        }

        public async Task GoToPreviousGroupAsync(ScrollIntoViewAlignment scrollIntoViewAlignment = ScrollIntoViewAlignment.Leading)
        {
            if (groupCollection != null)
            {
                var gc = groupCollection;

                var currentGroupIndex = GetCurrentVisibleGroupIndex();

                if (currentGroupIndex - 1 < 0)
                {
                    currentGroupIndex = gc.GroupHeaders.Count - 1;
                }
                else
                {
                    currentGroupIndex--;
                }
                await GoToGroupAsync(currentGroupIndex, scrollIntoViewAlignment);
            }
        }

        public async Task GoToGroupAsync(int groupIndex, ScrollIntoViewAlignment scrollIntoViewAlignment = ScrollIntoViewAlignment.Leading)
        {
            if (groupCollection != null)
            {
                var gc = groupCollection;
                if (groupIndex < gc.GroupHeaders.Count && groupIndex >= 0 && !isGotoGrouping)
                {
                    isGotoGrouping = true;
                    //load more so that ScrollIntoViewAlignment.Leading can go to top
                    var loadcount = this.GetVisibleItemsCount() + 1;

                    progressRing.IsActive = true;
                    progressRing.Visibility = Visibility.Visible;
                    //make sure user don't do any other thing at the time.
                    this.IsHitTestVisible = false;
                    //await Task.Delay(3000);
                    while (gc.GroupHeaders[groupIndex].FirstIndex == -1)
                    {
                        if (gc.HasMoreItems)
                        {
                            await gc.LoadMoreItemsAsync(loadcount);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (gc.GroupHeaders[groupIndex].FirstIndex != -1)
                    {
                        //make sure there are enought items to go ScrollIntoViewAlignment.Leading
                        //this.count > (firstIndex + loadcount)
                        if (scrollIntoViewAlignment == ScrollIntoViewAlignment.Leading)
                        {
                            var more = this.Items.Count - (gc.GroupHeaders[groupIndex].FirstIndex + loadcount);
                            if (gc.HasMoreItems && more < 0)
                            {
                                await gc.LoadMoreItemsAsync((uint)Math.Abs(more));
                            }
                        }
                        progressRing.IsActive = false;
                        progressRing.Visibility = Visibility.Collapsed;
                        var groupFirstIndex = gc.GroupHeaders[groupIndex].FirstIndex;
                        ScrollIntoView(this.Items[groupFirstIndex], scrollIntoViewAlignment);
                        //already in viewport, maybe it will not change view 
                        if (visibleGroupHeaders.ContainsKey(gc.GroupHeaders[groupIndex]) && visibleGroupHeaders[gc.GroupHeaders[groupIndex]].Visibility == Visibility.Visible)
                        {
                            this.IsHitTestVisible = true;
                            isGotoGrouping = false;
                        }
                    }
                    else
                    {
                        this.IsHitTestVisible = true;
                        isGotoGrouping = false;
                        progressRing.IsActive = false;
                        progressRing.Visibility = Visibility.Collapsed;
                    }

                }
            }
        }

    }

}
