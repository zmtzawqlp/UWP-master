using MyUWPToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    /// <summary>
    /// Group ListView to support each group ISupportIncrementalLoading and UI Virtualized
    /// </summary>
    public class GroupListView : ListView
    {
        private ContentControl topGroupHeader;
        private ContentControl movingGroupHeader;
        #region Property
        public DataTemplate GroupHeaderDataTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderDataTemplateProperty); }
            set { SetValue(GroupHeaderDataTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupHeaderDataTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupHeaderDataTemplateProperty =
            DependencyProperty.Register("GroupHeaderDataTemplate", typeof(DataTemplate), typeof(GroupListView), new PropertyMetadata(null));



        #endregion
        private IGroupCollection groupCollection;

        public GroupListView()
        {
            this.DefaultStyleKey = typeof(GroupListView);
        }

        protected override void OnApplyTemplate()
        {
            topGroupHeader = GetTemplateChild("TopGroupHeader") as ContentControl;
            movingGroupHeader = GetTemplateChild("MovingGroupHeader") as ContentControl;

            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (groupCollection == null && this.ItemsSource != null && ItemsSource is IGroupCollection)
            {
                groupCollection = (ItemsSource as IGroupCollection);
            }
            base.PrepareContainerForItemOverride(element, item);

            if (groupCollection != null)
            {
                var index = IndexFromContainer(element);
                var group = groupCollection.GroupHeaders.FirstOrDefault(x => x.FirstIndex == index);
                if (group != null)
                {
                    (element as ListViewItem).Margin = new Thickness(0, 50, 0, 0);
                    
                    topGroupHeader.DataContext = group;
                }
                else
                {
                    (element as ListViewItem).Margin = new Thickness(0);
                }
            }
        }
    }
}
