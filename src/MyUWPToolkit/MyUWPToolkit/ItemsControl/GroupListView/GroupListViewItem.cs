using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    [TemplatePart(Name = "headerPresenter", Type = typeof(ContentPresenter))]
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
            DependencyProperty.Register("Header", typeof(object), typeof(GroupListViewItem), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderChanged)));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GroupListViewItem).SetHeader();

        }

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
            if (headerPresenter != null)
            {
                headerPresenter.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, new DependencyPropertyChangedCallback(OnHeaderPresenterContentChanged));
            }
            else
            {
                Debug.Assert(false, "headerpresenter is missing.");
            }
        }

        private void OnHeaderPresenterContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (headerPresenter.Content != Header)
            {
                headerPresenter.Content = Header;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (headerPresenter != null)
            {
                headerPresenter.Margin = new Thickness(-this.Margin.Left, -this.Margin.Top, -this.Margin.Right, this.Margin.Bottom);
            }
            return base.ArrangeOverride(finalSize);
        }

        public void ClearHeader()
        {
            Header = null;
            ClearValue(GroupListViewItem.HeaderTemplateProperty);
        }

        public void SetHeader()
        {
            if (headerPresenter != null)
            {
                headerPresenter.Content = Header;
            }
        }
    }
}
