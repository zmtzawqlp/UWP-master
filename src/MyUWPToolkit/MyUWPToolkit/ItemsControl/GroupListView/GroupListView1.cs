using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
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

    public class GroupListView1 : ListView
    {
        private IGroupCollection groupCollection;
        private ContentControl currentTopGroupHeader;
        private ScrollViewer scrollViewer;
        private ProgressRing progressRing;
        private List<IGroupHeader> visibleGroupHeaders;
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
            visibleGroupHeaders = new List<IGroupHeader>();
        }


        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            groupCollection = null;
            if (this.ItemsSource != null && ItemsSource is IGroupCollection)
            {
                groupCollection = (ItemsSource as IGroupCollection);
            }
            visibleGroupHeaders.Clear();
            //if (groupHeadersCanvas != null)
            //{
            //    groupHeadersCanvas.Children.Clear();
            //}
            //foreach (var item in groupDic)
            //{
            //    item.Value.VisualElement.ClearValue(ContentControl.ContentTemplateProperty);
            //    item.Value.TempElement.ClearValue(ContentControl.ContentTemplateProperty);
            //}
            //groupDic.Clear();

            if (currentTopGroupHeader != null)
            {
                currentTopGroupHeader.DataContext = null;
                currentTopGroupHeader.Visibility = Visibility.Collapsed;
            }
            //groupHeaderDelta = 30;
            //defaultListViewItemMargin = new Thickness(0);
            //isGotoGrouping = false;
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
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
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
                groupListViewItem.Header = group;
                Binding binding = new Binding();
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.Path = new PropertyPath("GroupHeaderTemplate");
                groupListViewItem.SetBinding(GroupListViewItem.HeaderTemplateProperty, binding);
                visibleGroupHeaders.Add(group);
                visibleGroupHeaders= visibleGroupHeaders.OrderBy(x => x.FirstIndex).ToList();
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
                groupListViewItem.Header = null;
                groupListViewItem.ClearValue(GroupListViewItem.HeaderTemplateProperty);
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

        public void UpdateGroupHeaders()
        {
            if (groupCollection != null)
            {
                var firstVisibleItemIndex = this.GetFirstVisibleIndex();
                foreach (var item in groupCollection.GroupHeaders)
                {
                    if ((-1 < item.FirstIndex && item.FirstIndex <= firstVisibleItemIndex) && (firstVisibleItemIndex <= item.LastIndex || item.LastIndex == -1))
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
                                    var groupHeaderDelta = item.Height;
                                    //add delta,so that it does not look like suddenly
                                    if (rect.Bottom + groupHeaderDelta < 0 || rect.Top > this.ActualHeight + groupHeaderDelta)
                                    {

                                    }
                                    //in view port
                                    else
                                    {
                                        var itemMargin = new Thickness(0, rect.Top - groupHeaderDelta, 0, 0);
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
                                                    currentTopGroupHeader.DataContext = item;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item != currentTopGroupHeader.DataContext)
                            {
                                //item.Value.Visibility = Visibility.Collapsed;
                                //item.Value.Margin = new Thickness(0);
                                //item.Value.Clip = null;
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
    }
}
