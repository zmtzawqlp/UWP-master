using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    public class GroupListViewItem : ListViewItem
    {
        ContentPresenter headerPresenter;
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(GroupListViewItem), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupListViewItem), new PropertyMetadata(null));

        public GroupListViewItem()
        {
            this.DefaultStyleKey = typeof(GroupListViewItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            headerPresenter = GetTemplateChild("headerPresenter") as ContentPresenter;
            headerPresenter.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, new DependencyPropertyChangedCallback(OnHeaderPresenterContentChanged));
        }

        private void OnHeaderPresenterContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            headerPresenter.Content = Header;
        }
    }
}
