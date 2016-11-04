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
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "ProgressRing", Type = typeof(ProgressRing))]
    public class GroupListView1 : ListView
    {
        private IGroupCollection groupCollection;
        private ContentControl currentTopGroupHeader;
        private ScrollViewer scrollViewer;
        private ProgressRing progressRing;
        private Visual visual;
        private ExpressionAnimation expression;
        private bool isGotoGrouping;
        private Dictionary<IGroupHeader, GroupListViewItem> groupheaders;
        #region Property

        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(GroupListView1), new PropertyMetadata(null));


        #endregion

        public GroupListView1()
        {
            this.DefaultStyleKey = typeof(GroupListView1);
            this.RegisterPropertyChangedCallback(ListView.ItemsSourceProperty, new DependencyPropertyChangedCallback(OnItemsSourceChanged));
            groupheaders = new Dictionary<IGroupHeader, GroupListViewItem>();
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            groupCollection = null;
            if (this.ItemsSource != null && ItemsSource is IGroupCollection)
            {
                groupCollection = (ItemsSource as IGroupCollection);
            }

            if (currentTopGroupHeader != null)
            {
                currentTopGroupHeader.DataContext = null;
                currentTopGroupHeader.Visibility = Visibility.Collapsed;
            }
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
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;

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

        private void OnCompositionTargetRendering(object sender, object e)
        {
            visual.StopAnimation("Offset.Y");
            Debug.WriteLine(visual.Offset.Y);
            visual.StartAnimation("Offset.Y", expression);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                isGotoGrouping = false;
                this.IsHitTestVisible = true;
            }
            UpdateGroupHeaders();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var index = IndexFromContainer(element);
            var group = groupCollection.GroupHeaders.FirstOrDefault(x => x.FirstIndex == index);
            if (group != null && element is GroupListViewItem)
            {
                var groupListViewItem = (element as GroupListViewItem);
                if (groupheaders.ContainsKey(group) && groupheaders[group] != groupListViewItem)
                {
                    groupheaders[group].ClearHeader();
                }
                groupheaders[group] = groupListViewItem;
                groupListViewItem.Header = group;
                Binding binding = new Binding();
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.Path = new PropertyPath("GroupHeaderTemplate");
                groupListViewItem.SetBinding(GroupListViewItem.HeaderTemplateProperty, binding);
                UpdateGroupHeaders();
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            var index = IndexFromContainer(element);
            var group = groupCollection.GroupHeaders.FirstOrDefault(x => x.FirstIndex == index);
            if (group != null && element is GroupListViewItem)
            {
                var groupListViewItem = (element as GroupListViewItem);
                groupListViewItem.ClearHeader();
                if (groupheaders.ContainsKey(group))
                {
                    groupheaders.Remove(group);
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GroupListViewItem;
            return base.IsItemItsOwnContainerOverride(item);
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GroupListViewItem();
            return base.GetContainerForItemOverride();
        }

        private void UpdateGroupHeaders()
        {
            if (groupCollection != null)
            {
                var firstVisibleItemIndex = this.GetFirstVisibleIndex();
                if (firstVisibleItemIndex < 0)
                {
                    return;
                }
                foreach (var item in groupCollection.GroupHeaders)
                {
                    if (item.FirstIndex == -1)
                    {
                        continue;
                    }
                    if (item.FirstIndex <= firstVisibleItemIndex && (firstVisibleItemIndex <= item.LastIndex || item.LastIndex == -1))
                    {
                        currentTopGroupHeader.Visibility = Visibility.Visible;
                        currentTopGroupHeader.Margin = new Thickness(0);
                        currentTopGroupHeader.Clip = null;
                        currentTopGroupHeader.DataContext = item;
                    }
                    else
                    {
                        ListViewItem listViewItem = ContainerFromIndex(item.FirstIndex) as ListViewItem;

                        if (listViewItem == null && item.LastIndex != -1)
                        {
                            listViewItem = ContainerFromIndex(item.LastIndex) as ListViewItem;
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
                                    //add delta,so that it does not look like suddenly
                                    if (rect.Bottom < 0 || rect.Top > this.ActualHeight)
                                    {

                                    }
                                    //in view port
                                    else
                                    {
                                        if (currentTopGroupHeader != null)
                                        {
                                            var delta = currentTopGroupHeader.ActualHeight - (rect.Top);
                                            if (delta > 0)
                                            {
                                                currentTopGroupHeader.Margin = new Thickness(0, -delta, 0, 0);
                                                currentTopGroupHeader.Clip = new RectangleGeometry() { Rect = new Rect(0, delta, currentTopGroupHeader.ActualWidth, currentTopGroupHeader.ActualHeight) };
                                                if (delta >= currentTopGroupHeader.ActualHeight)
                                                {
                                                    currentTopGroupHeader.Visibility = Visibility.Visible;
                                                    currentTopGroupHeader.Margin = new Thickness(0);
                                                    currentTopGroupHeader.Clip = null;
                                                    currentTopGroupHeader.DataContext = item;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
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

        private int GetCurrentVisibleGroupIndex()
        {
            var currentGroupIndex = 0;
            if (groupCollection != null)
            {
                if (currentTopGroupHeader != null)
                {
                    foreach (var item in groupCollection.GroupHeaders)
                    {
                        if (currentTopGroupHeader.DataContext == item)
                        {
                            currentGroupIndex = groupCollection.GroupHeaders.IndexOf(item);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Assert(false, "why not has top Group");
                    var firstVisibleItemIndex = this.GetFirstVisibleIndex();
                    foreach (var item in groupCollection.GroupHeaders)
                    {
                        if (item.FirstIndex <= firstVisibleItemIndex && (firstVisibleItemIndex <= item.LastIndex || item.LastIndex == -1))
                        {
                            currentGroupIndex = groupCollection.GroupHeaders.IndexOf(item);
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

                        ListViewItem listViewItem = ContainerFromIndex(groupFirstIndex) as ListViewItem;
                        if (listViewItem != null)
                        {
                            GeneralTransform gt = listViewItem.TransformToVisual(this);
                            var rect = gt.TransformBounds(new Rect(0, 0, listViewItem.ActualWidth, listViewItem.ActualHeight));
                            //add delta,so that it does not look like suddenly
                            if (rect.Bottom < 0 || rect.Top > this.ActualHeight)
                            {

                            }
                            //already in viewport, maybe it will not change view 
                            else
                            {
                                this.IsHitTestVisible = true;
                                isGotoGrouping = false;
                            }
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
