using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{
    /// <summary>
    /// Group ListView to support each group ISupportIncrementalLoading and UI Virtualized
    /// </summary>
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "ProgressRing", Type = typeof(ProgressRing))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "ProgressRing", Type = typeof(ProgressRing))]
    public class GroupListView : ListView
    {
        private ContentControl currentTopGroupHeader;
        private ScrollViewer scrollViewer;
        private Canvas groupHeadersCanvas;
        private ProgressRing progressRing;
        //Dictionary<IGroupHeader, ContentControl> visibleGroupHeaders;
        private double groupHeaderDelta = 30;
        private Thickness defaultListViewItemMargin = new Thickness(0);
        //Dictionary<ContentControl, ExpressionAnimationItem> expressionAnimationDic;
        Dictionary<IGroupHeader, ExpressionAnimationItem> groupDic;
        private Dictionary<IGroupHeader, ListViewItem> groupheaders;
        private bool isGotoGrouping = false;
        private IGroupCollection groupCollection;
        private Visual visual;
        private ExpressionAnimation expression;
        #region Property

        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(GroupListView), new PropertyMetadata(null));



        #endregion

        public GroupListView()
        {
            this.DefaultStyleKey = typeof(GroupListView);
            groupDic = new Dictionary<IGroupHeader, ExpressionAnimationItem>();
            this.RegisterPropertyChangedCallback(ListView.ItemsSourceProperty, new DependencyPropertyChangedCallback(OnItemsSourceChanged));
            groupheaders = new Dictionary<IGroupHeader, ListViewItem>();
        }


        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            groupCollection = null;
            if (this.ItemsSource != null && ItemsSource is IGroupCollection)
            {
                groupCollection = (ItemsSource as IGroupCollection);
            }

            if (groupHeadersCanvas != null)
            {
                groupHeadersCanvas.Children.Clear();
            }
            foreach (var item in groupDic)
            {
                item.Value.VisualElement.ClearValue(ContentControl.ContentTemplateProperty);
                item.Value.TempElement.ClearValue(ContentControl.ContentTemplateProperty);
            }
            groupDic.Clear();

            if (currentTopGroupHeader != null)
            {
                currentTopGroupHeader.DataContext = null;
                currentTopGroupHeader.Visibility = Visibility.Collapsed;
            }
            groupHeaderDelta = 30;
            defaultListViewItemMargin = new Thickness(0);
            isGotoGrouping = false;
            groupheaders.Clear();
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
            groupHeadersCanvas = this.scrollViewer.FindDescendantByName("GroupHeadersCanvas") as Canvas;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            groupHeadersCanvas.Loaded += GroupHeadersGrid_Loaded;

            currentTopGroupHeader = this.scrollViewer.FindDescendantByName("TopGroupHeader") as ContentControl;
            Binding binding = new Binding();
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            binding.Path = new PropertyPath("GroupHeaderTemplate");
            currentTopGroupHeader.SetBinding(ContentControl.ContentTemplateProperty, binding);
            currentTopGroupHeader.Visibility = Visibility.Collapsed;
            if (scrollViewer.VerticalOffset == 0)
            {
                CreateVisual();
            }
            else
            {
                scrollViewer.RegisterPropertyChangedCallback(ScrollViewer.VerticalOffsetProperty, new DependencyPropertyChangedCallback(OnScrollViewerVerticalOffsetChanged));
            }
        }

        private void OnScrollViewerVerticalOffsetChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (scrollViewer.VerticalOffset == 0 && visual == null)
            {
                CreateVisual();
            }
        }

        private void CreateVisual()
        {
            visual = ElementCompositionPreview.GetElementVisual(currentTopGroupHeader);

            var scrollViewerManipProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            Compositor compositor = scrollViewerManipProps.Compositor;

            expression = compositor.CreateExpressionAnimation("max(0,ScrollViewerManipProps.Translation.Y)");


            // set "dynamic" reference parameter that will be used to evaluate the current position of the scrollbar every frame
            expression.SetReferenceParameter("ScrollViewerManipProps", scrollViewerManipProps);
            visual.StartAnimation("Offset.Y", expression);
            //Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void GroupHeadersGrid_Loaded(object sender, RoutedEventArgs e)
        {
            groupHeadersCanvas.Loaded -= GroupHeadersGrid_Loaded;

            foreach (var item in groupDic)
            {
                if (item.Value.VisualElement.Parent == null)
                {
                    var groupheader = item.Key;

                    groupHeadersCanvas.Children.Add(item.Value.VisualElement);
                    groupHeadersCanvas.Children.Add(item.Value.TempElement);

                    item.Value.VisualElement.Measure(new Windows.Foundation.Size(this.ActualWidth, this.ActualHeight));

                    item.Key.Height = item.Value.VisualElement.DesiredSize.Height;

                    item.Value.VisualElement.Height = item.Value.TempElement.Height= item.Key.Height;
                    item.Value.VisualElement.Width = item.Value.TempElement.Width = this.ActualWidth;

                    var listViewItem = ContainerFromIndex(item.Key.FirstIndex) as ListViewItem;
                    listViewItem.Tag = listViewItem.Margin;
                    listViewItem.Margin = GetItemMarginBaseOnDeafult(item.Key.Height);
                    listViewItem.SizeChanged -= ListViewItem_SizeChanged;
                    listViewItem.SizeChanged += ListViewItem_SizeChanged;

                    item.Value.VisualElement.Visibility = Visibility.Collapsed;
                    item.Value.TempElement.Visibility = Visibility.Collapsed;
                    UpdateGroupHeaders();
                }
            }
        }

        bool isViewChanging = false;
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            isViewChanging = e.IsIntermediate;

            UpdateGroupHeaders(e.IsIntermediate);

            if (!e.IsIntermediate)
            {
                isGotoGrouping = false;
                this.IsHitTestVisible = true;
            }
        }

        internal void UpdateGroupHeaders(bool isIntermediate = true)
        {
            var firstVisibleItemIndex = this.GetFirstVisibleIndex();
            foreach (var item in groupDic)
            {
                //top header
                if (item.Key.FirstIndex <= firstVisibleItemIndex && (firstVisibleItemIndex <= item.Key.LastIndex || item.Key.LastIndex == -1))
                {
                    currentTopGroupHeader.Visibility = Visibility.Visible;
                    currentTopGroupHeader.Margin = new Thickness(0);
                    currentTopGroupHeader.Clip = null;
                    currentTopGroupHeader.DataContext = item.Key;

                    if (item.Key.FirstIndex == firstVisibleItemIndex)
                    {
                        if (item.Value.ScrollViewer == null)
                        {
                            item.Value.ScrollViewer = scrollViewer;
                        }

                        var isActive = item.Value.IsActive;


                        item.Value.StopAnimation();
                        item.Value.VisualElement.Clip = null;
                        item.Value.VisualElement.Visibility = Visibility.Collapsed;

                        if (!isActive)
                        {
                            if (!isIntermediate)
                            {
                                item.Value.VisualElement.Margin = new Thickness(0);
                                item.Value.StartAnimation(true);
                            }
                        }
                        else
                        {
                            item.Value.StartAnimation(false);
                        }

                    }
                    ClearTempElement(item);
                }
                //moving header
                else
                {
                    HandleGroupHeader(isIntermediate, item);
                }
            }
        }

        private void ClearTempElement(KeyValuePair<IGroupHeader, ExpressionAnimationItem> item)
        {
            item.Value.TempElement.Margin = new Thickness(0);
            item.Value.TempElement.Clip = null;
            item.Value.TempElement.Visibility = Visibility.Collapsed;
        }

        internal void ForceUpdateGroupHeader(ListViewItem listViewItem, KeyValuePair<IGroupHeader, ExpressionAnimationItem> item)
        {
            GeneralTransform gt = listViewItem.TransformToVisual(this);
            var rect = gt.TransformBounds(new Rect(0, 0, listViewItem.ActualWidth, listViewItem.ActualHeight));
            groupHeaderDelta = item.Key.Height;
            //add delta,so that it does not look like suddenly

            var itemMargin = new Thickness(0, rect.Top - groupHeaderDelta - defaultListViewItemMargin.Top, 0, 0);
            RectangleGeometry itemClip = null;
            Visibility itemVisibility = Visibility.Collapsed;
            if (itemMargin.Top < 0)
            {
                var clipHeight = groupHeaderDelta + itemMargin.Top;
                //moving header has part in viewport
                if (clipHeight > 0)
                {
                    itemVisibility = Visibility.Visible;
                    itemClip = new RectangleGeometry() { Rect = new Rect(0, -itemMargin.Top, this.ActualWidth, clipHeight) };
                }
                //moving header not in viewport
                else
                {
                    itemVisibility = Visibility.Collapsed;
                    itemClip = null;
                }
            }
            else if (itemMargin.Top + groupHeaderDelta > this.ActualHeight)
            {
                var clipHeight = groupHeaderDelta - (groupHeaderDelta + itemMargin.Top - this.ActualHeight);
                //moving header has part in viewport
                if (clipHeight > 0)
                {
                    itemVisibility = Visibility.Visible;
                    itemClip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, clipHeight) };
                }
                //moving header not in viewport
                else
                {
                    itemVisibility = Visibility.Collapsed;
                    itemClip = null;
                }
            }
            //moving header all in viewport
            else
            {
                itemVisibility = Visibility.Visible;
                itemClip = null;
            }

            if (currentTopGroupHeader != null)
            {
                var delta = currentTopGroupHeader.ActualHeight - (itemMargin.Top);
                if (delta > 0)
                {
                    currentTopGroupHeader.Margin = new Thickness(0, -delta, 0, 0);
                    currentTopGroupHeader.Clip = new RectangleGeometry() { Rect = new Rect(0, delta, currentTopGroupHeader.ActualWidth, currentTopGroupHeader.ActualHeight) };
                    if (delta >= groupHeaderDelta)
                    {
                        currentTopGroupHeader.Visibility = Visibility.Visible;
                        currentTopGroupHeader.Margin = new Thickness(0);
                        currentTopGroupHeader.Clip = null;
                        currentTopGroupHeader.DataContext = item.Key;
                    }
                }

            }
            ExpressionAnimationItem expressionItem = item.Value;
            expressionItem.StopAnimation();

            item.Value.VisualElement.Margin = itemMargin;
            expressionItem.VisualElement.Clip = itemClip;
            expressionItem.VisualElement.Visibility = itemVisibility;


            if (expressionItem.ScrollViewer == null)
            {
                expressionItem.ScrollViewer = scrollViewer;
            }

            expressionItem.StartAnimation(true);

        }

        private void HandleGroupHeader(bool isIntermediate, KeyValuePair<IGroupHeader, ExpressionAnimationItem> item)
        {
            ListViewItem listViewItem = ContainerFromIndex(item.Key.FirstIndex) as ListViewItem;

            //if (listViewItem == null && item.Key.LastIndex != -1)
            //{
            //    listViewItem = ContainerFromIndex(item.Key.LastIndex) as ListViewItem;
            //}
            if (listViewItem != null)
            {
                //unloaded
                if (listViewItem.ActualHeight == 0 || listViewItem.ActualWidth == 0)
                {
                    listViewItem.Loaded += ListViewItem_Loaded;
                }
                else
                {
                    HandleItemsInItemsPanel(isIntermediate, item, listViewItem);
                }
            }
            //not in items panel
            else
            {
                if (item.Key != currentTopGroupHeader.DataContext)
                {
                    //stop
                    if (item.Value.IsActive)
                    {
                        item.Value.StopAnimation();
                        item.Value.VisualElement.Margin = new Thickness(0);
                        item.Value.VisualElement.Clip = null;
                        item.Value.VisualElement.Visibility = Visibility.Collapsed;
                        //item.Value.StartAnimation(false);
                    }
                    ClearTempElement(item);
                }

            }
        }

        private void HandleItemsInItemsPanel(bool isIntermediate, KeyValuePair<IGroupHeader, ExpressionAnimationItem> item, ListViewItem listViewItem)
        {
            GeneralTransform gt = listViewItem.TransformToVisual(this);
            var rect = gt.TransformBounds(new Rect(0, 0, listViewItem.ActualWidth, listViewItem.ActualHeight));
            groupHeaderDelta = item.Key.Height;
            //add delta,so that it does not look like suddenly

            var itemMargin = new Thickness(0, rect.Top - groupHeaderDelta - defaultListViewItemMargin.Top, 0, 0);
            RectangleGeometry itemClip = null;
            Visibility itemVisibility = Visibility.Collapsed;
            if (itemMargin.Top < 0)
            {
                var clipHeight = groupHeaderDelta + itemMargin.Top;
                //moving header has part in viewport
                if (clipHeight > 0)
                {
                    itemVisibility = Visibility.Visible;
                    itemClip = new RectangleGeometry() { Rect = new Rect(0, -itemMargin.Top, this.ActualWidth, clipHeight) };
                }
                //moving header not in viewport
                else
                {
                    itemVisibility = Visibility.Collapsed;
                    itemClip = null;
                }
            }
            else if (itemMargin.Top + groupHeaderDelta > this.ActualHeight)
            {
                var clipHeight = groupHeaderDelta - (groupHeaderDelta + itemMargin.Top - this.ActualHeight);
                //moving header has part in viewport
                if (clipHeight > 0)
                {
                    itemVisibility = Visibility.Visible;
                    itemClip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, clipHeight) };
                }
                //moving header not in viewport
                else
                {
                    itemVisibility = Visibility.Collapsed;
                    itemClip = null;
                }
            }
            //moving header all in viewport
            else
            {
                itemVisibility = Visibility.Visible;
                itemClip = null;
            }

            if (currentTopGroupHeader != null)
            {
                var delta = currentTopGroupHeader.ActualHeight - (itemMargin.Top);
                if (delta > 0)
                {
                    currentTopGroupHeader.Margin = new Thickness(0, -delta, 0, 0);
                    currentTopGroupHeader.Clip = new RectangleGeometry() { Rect = new Rect(0, delta, currentTopGroupHeader.ActualWidth, currentTopGroupHeader.ActualHeight) };
                    if (delta >= groupHeaderDelta)
                    {
                        currentTopGroupHeader.Visibility = Visibility.Visible;
                        currentTopGroupHeader.Margin = new Thickness(0);
                        currentTopGroupHeader.Clip = null;
                        currentTopGroupHeader.DataContext = item.Key;
                    }
                }

            }
            bool isInViewport = false;
            if (itemMargin.Top > 0)
            {
                var groupheaderRect = new Rect(0, itemMargin.Top, this.ActualWidth, groupHeaderDelta);
                isInViewport = (groupheaderRect.Top < this.ActualHeight && groupheaderRect.Bottom > 0);
            }
            if (!isInViewport)
            {
                ClearTempElement(item);
            }

            if (!item.Value.IsActive)
            {
                // isIntermediate is false or groupheaderRect has into view port and view not changing
                if (!isIntermediate || (isInViewport && !isViewChanging))
                {
                    ExpressionAnimationItem expressionItem = item.Value;
                    expressionItem.StopAnimation();

                    item.Value.TempElement.Margin = new Thickness(0);
                    item.Value.TempElement.Clip = null;
                    item.Value.TempElement.Visibility = Visibility.Collapsed;

                    item.Value.VisualElement.Margin = itemMargin;
                    item.Value.VisualElement.Clip = itemClip;
                    item.Value.VisualElement.Visibility = itemVisibility;

                    if (expressionItem.ScrollViewer == null)
                    {
                        expressionItem.ScrollViewer = scrollViewer;
                    }
                    expressionItem.StartAnimation(true);
                }
                //temp use
                else if (isInViewport)
                {
                    item.Value.TempElement.Margin = itemMargin;
                    item.Value.TempElement.Clip = itemClip;
                    item.Value.TempElement.Visibility = itemVisibility;
                }
            }
            //use active animation
            else
            {
                if (item.Value.VisualElement.Clip != itemClip || item.Value.VisualElement.Visibility != itemVisibility)
                {
                    ExpressionAnimationItem expressionItem = item.Value;
                    expressionItem.StopAnimation();

                    if (!isIntermediate)
                    {
                        item.Value.VisualElement.Margin = itemMargin;
                    }
                    //item.Value.VisualElement.Margin = itemMargin;
                    expressionItem.VisualElement.Clip = itemClip;
                    expressionItem.VisualElement.Visibility = itemVisibility;


                    if (expressionItem.ScrollViewer == null)
                    {
                        expressionItem.ScrollViewer = scrollViewer;
                    }

                    expressionItem.StartAnimation(!isIntermediate);
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
            listViewItem.SizeChanged -= ListViewItem_SizeChanged;
            if (listViewItem.Tag == null)
            {
                defaultListViewItemMargin = listViewItem.Margin;
            }

            if (groupCollection != null)
            {
                var index = IndexFromContainer(element);
                var group = groupCollection.GroupHeaders.FirstOrDefault(x => x.FirstIndex == index || x.LastIndex == index);
                if (group != null)
                {
                    if (groupheaders.ContainsKey(group) && groupheaders[group] != listViewItem)
                    {
                        groupheaders[group].Margin=defaultListViewItemMargin;
                    }
                    groupheaders[group] = listViewItem;

                    if (!groupDic.ContainsKey(group))
                    {
                        ContentControl groupheader = CreateGroupHeader(group);
                        ContentControl tempGroupheader = CreateGroupHeader(group);

                        ExpressionAnimationItem expressionAnimationItem = new ExpressionAnimationItem();
                        expressionAnimationItem.VisualElement = groupheader;
                        expressionAnimationItem.TempElement = tempGroupheader;

                        groupDic[group] = expressionAnimationItem;

                        var temp = new Dictionary<IGroupHeader, ExpressionAnimationItem>();
                        foreach (var keyValue in groupDic.OrderBy(x => x.Key.FirstIndex))
                        {
                            temp[keyValue.Key] = keyValue.Value;
                        }
                        groupDic = temp;
                        if (groupHeadersCanvas != null)
                        {
                            groupHeadersCanvas.Children.Add(groupheader);
                            groupHeadersCanvas.Children.Add(tempGroupheader);

                            groupheader.Measure(new Windows.Foundation.Size(this.ActualWidth, this.ActualHeight));

                            group.Height = groupheader.DesiredSize.Height;

                            groupheader.Height = tempGroupheader.Height = group.Height;
                            groupheader.Width = tempGroupheader.Width = this.ActualWidth;

                            if (group.FirstIndex == index)
                            {
                                listViewItem.Tag = listViewItem.Margin;
                                listViewItem.Margin = GetItemMarginBaseOnDeafult(groupheader.DesiredSize.Height);
                                listViewItem.SizeChanged += ListViewItem_SizeChanged;
                            }

                            groupheader.Visibility = Visibility.Collapsed;
                            tempGroupheader.Visibility = Visibility.Collapsed;
                            UpdateGroupHeaders();
                        }

                    }
                    else
                    {
                        if (group.FirstIndex == index)
                        {
                            listViewItem.Tag = listViewItem.Margin;
                            listViewItem.Margin = GetItemMarginBaseOnDeafult(group.Height);
                            listViewItem.SizeChanged += ListViewItem_SizeChanged;
                        }
                        else
                        {
                            listViewItem.Margin = defaultListViewItemMargin;
                        }
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


        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            ListViewItem listViewItem = element as ListViewItem;
            listViewItem.SizeChanged -= ListViewItem_SizeChanged;
            base.ClearContainerForItemOverride(element, item);
        }
        private void ListViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize != e.NewSize)
            {
                int index = IndexFromContainer((sender as ListViewItem));

                var item = groupDic.FirstOrDefault(x => x.Key.FirstIndex == index);
                if (item.Key != null)
                {
                    if (item.Value.VisualElement.Width != this.ActualWidth)
                    {
                        item.Value.VisualElement.Width = this.ActualWidth;
                        item.Value.TempElement.Width = this.ActualWidth;
                    }
                    if (item.Value.IsActive)
                    {
                        ForceUpdateGroupHeader((sender as ListViewItem), item);
                    }

                }
            }
        }

        private ContentControl CreateGroupHeader(IGroupHeader group)
        {
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
            return groupheader;
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
                    foreach (var item in groupDic)
                    {
                        if (currentTopGroupHeader.DataContext == item.Key)
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
                    foreach (var item in groupDic)
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
                        if (groupDic.ContainsKey(gc.GroupHeaders[groupIndex]) && groupDic[gc.GroupHeaders[groupIndex]].Visibility == Visibility.Visible)
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
