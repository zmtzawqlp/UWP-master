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
        ContentControl _header;
        Grid _scrollContent;

        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        Compositor _compositor;
        Visual _scrollContentVisual;
        #endregion


        #region Internal property

        #endregion

        #region Public property
        /// <summary>
        /// fire when tap in column header
        /// </summary>
        public new event EventHandler<ItemClickEventArgs> ItemClick;

        /// <summary>
        /// occur when reach threshold.
        /// </summary>
        public event EventHandler PullToRefresh;

        /// <summary>
        /// fire when tap in column header
        /// </summary>
        public event EventHandler<SortingColumnEventArgs> SortingColumn;
        #endregion

        #region DependencyProperty

        public ObservableCollection<Column> FrozenColumnsHeaderItemsSource
        {
            get { return (ObservableCollection<Column>)GetValue(FrozenColumnsHeaderItemsSourceProperty); }
            set { SetValue(FrozenColumnsHeaderItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsHeaderItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsHeaderItemsSourceProperty =
            DependencyProperty.Register("FrozenColumnsHeaderItemsSource", typeof(ObservableCollection<Column>), typeof(FlexGrid), new PropertyMetadata(null));


        public ObservableCollection<Column> ColumnsHeaderItemsSource
        {
            get { return (ObservableCollection<Column>)GetValue(ColumnsHeaderItemsSourceProperty); }
            set { SetValue(ColumnsHeaderItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsHeaderItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsHeaderItemsSourceProperty =
            DependencyProperty.Register("ColumnsHeaderItemsSource", typeof(ObservableCollection<Column>), typeof(FlexGrid), new PropertyMetadata(null));


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


        //public DataTemplate CellItemTemplate
        //{
        //    get { return (DataTemplate)GetValue(CellItemTemplateProperty); }
        //    set { SetValue(CellItemTemplateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CellItemTemplate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CellItemTemplateProperty =
        //    DependencyProperty.Register("CellItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));
        #endregion
    }

    public class SortingColumnEventArgs:EventArgs
    {
        public object Column { get; private set; }
        public SortingColumnEventArgs(object column)
        {
            Column = column;
        }
    }
}
