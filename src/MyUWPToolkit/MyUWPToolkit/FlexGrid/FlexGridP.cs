using MyUWPToolkit.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.FlexGrid
{
    public partial class FlexGrid : ListView
    {
        #region Filed
        ScrollViewer _scrollViewer;
        ListView _columnHeader;
        ListView _frozenColumnsHeader;

        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        Compositor _compositor;
        Visual _scrollContentVisual;
        #endregion

        #region Internal property
        ScrollViewer _outerScrollViewer;

        internal ScrollViewer OuterScrollViewer
        {
            get
            {
                if (_outerScrollViewer == null)
                {
                    var parent = this.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent is Page)
                        {
                            break;
                        }
                        _outerScrollViewer = parent as ScrollViewer;
                        if (_outerScrollViewer != null)
                        {
                            break;
                        }
                        parent = parent.Parent as FrameworkElement;
                    }

                }
                return _outerScrollViewer;
            }

        }
        #endregion

        #region Public property

        /// <summary>
        /// fire when tap in column header
        /// </summary>
        public event EventHandler<SortingColumnEventArgs> SortingColumn;

        public ScrollViewer ScrollViewer
        {
            get
            {
                return _scrollViewer;
            }
        }
        #endregion

        #region DependencyProperty

        public object FrozenColumnsHeaderItemsSource
        {
            get { return (object)GetValue(FrozenColumnsHeaderItemsSourceProperty); }
            set { SetValue(FrozenColumnsHeaderItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsHeaderItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsHeaderItemsSourceProperty =
            DependencyProperty.Register("FrozenColumnsHeaderItemsSource", typeof(object), typeof(FlexGrid), new PropertyMetadata(null));


        public object ColumnsHeaderItemsSource
        {
            get { return (object)GetValue(ColumnsHeaderItemsSourceProperty); }
            set { SetValue(ColumnsHeaderItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsHeaderItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsHeaderItemsSourceProperty =
            DependencyProperty.Register("ColumnsHeaderItemsSource", typeof(object), typeof(FlexGrid), new PropertyMetadata(null));


        public DataTemplate FrozenColumnsHeaderItemTemplate
        {
            get { return (DataTemplate)GetValue(FrozenColumnsHeaderItemTemplateProperty); }
            set { SetValue(FrozenColumnsHeaderItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsHeaderItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsHeaderItemTemplateProperty =
            DependencyProperty.Register("FrozenColumnsHeaderItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));

        public DataTemplate ColumnsHeaderItemTemplate
        {
            get { return (DataTemplate)GetValue(ColumnsHeaderItemTemplateProperty); }
            set { SetValue(ColumnsHeaderItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsHeaderItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsHeaderItemTemplateProperty =
            DependencyProperty.Register("ColumnsHeaderItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));


        public DataTemplate FrozenColumnsItemTemplate
        {
            get { return (DataTemplate)GetValue(FrozenColumnsItemTemplateProperty); }
            set { SetValue(FrozenColumnsItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsItemTemplateProperty =
            DependencyProperty.Register("FrozenColumnsItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));



        public bool KeepHorizontalOffsetWhenItemsSourceChanged
        {
            get { return (bool)GetValue(KeepHorizontalOffsetWhenItemsSourceChangedProperty); }
            set { SetValue(KeepHorizontalOffsetWhenItemsSourceChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeepHorizontalOffsetWhenItemsSourceChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeepHorizontalOffsetWhenItemsSourceChangedProperty =
            DependencyProperty.Register("KeepHorizontalOffsetWhenItemsSourceChanged", typeof(bool), typeof(FlexGrid), new PropertyMetadata(true));



        public Visibility FrozenColumnsVisibility
        {
            get { return (Visibility)GetValue(FrozenColumnsVisibilityProperty); }
            set { SetValue(FrozenColumnsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsVisibilityProperty =
            DependencyProperty.Register("FrozenColumnsVisibility", typeof(Visibility), typeof(FlexGrid), new PropertyMetadata(Visibility.Visible));


        #endregion
    }

    public class SortingColumnEventArgs : EventArgs
    {
        public object Data { get; private set; }
        public SortingColumnEventArgs(object data)
        {
            Data = data;
        }
    }


}
