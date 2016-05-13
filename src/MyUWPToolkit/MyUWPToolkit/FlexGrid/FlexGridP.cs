using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit.FlexGrid
{
    public partial class FlexGrid : Control
    {
        #region Filed
        Grid _contentGrid;
        ListView _frozenColumnsHeader;
        ListView _frozenColumnsCell;
        ListView _columnsHeader;
        ListView _cell;
        ContentControl _pullToRefreshHeader;
        //ScrollViewer _frozenColumnsHeader;
        ScrollViewer _frozenColumnsCellSV;
        ScrollViewer _columnsHeaderSV;
        ScrollViewer _cellSV;

        ////ItemsPresenter _frozenColumnsHeader;
        //ItemsPresenter _frozenColumnsCellIP;
        //ItemsPresenter _columnsHeaderIP;
        //ItemsPresenter _cellIP;
        #endregion

        #region Internal property
        ScrollViewer CellSV
        {
            get
            {
                if (_cellSV==null && _cell!=null)
                {
                 
                }
                return null;
            }
        }
        #endregion
        #region DependencyProperty
        public object ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(FlexGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FlexGrid).OnItemsSourceChanged();
        }

        public object FrozenColumnsHeaderItemsSource
        {
            get { return (IEnumerable)GetValue(FrozenColumnsHeaderItemsSourceProperty); }
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

        public DataTemplate FrozenColumnsCellItemTemplate
        {
            get { return (DataTemplate)GetValue(FrozenColumnsCellItemTemplateProperty); }
            set { SetValue(FrozenColumnsCellItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsCellItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsCellItemTemplateProperty =
            DependencyProperty.Register("FrozenColumnsCellItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));

        public DataTemplate CellItemTemplate
        {
            get { return (DataTemplate)GetValue(CellItemTemplateProperty); }
            set { SetValue(CellItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellItemTemplateProperty =
            DependencyProperty.Register("CellItemTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));

        #region PullToRefresh
        /// <summary>
        /// The threshold for release to refresh，defautl value is 1/5 of PullToRefreshPanel's height.
        /// </summary>
        public double RefreshThreshold
        {
            get { return (double)GetValue(RefreshThresholdProperty); }
            set { SetValue(RefreshThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshThresholdProperty =
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(FlexGrid), new PropertyMetadata(0.0));



        public DataTemplate PullToRefreshHeaderTemplate
        {
            get { return (DataTemplate)GetValue(PullToRefreshHeaderTemplateProperty); }
            set { SetValue(PullToRefreshHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PullToRefreshHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullToRefreshHeaderTemplateProperty =
            DependencyProperty.Register("PullToRefreshHeaderTemplate", typeof(DataTemplate), typeof(FlexGrid), new PropertyMetadata(null));


        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(FlexGrid), new PropertyMetadata(false));

        public DateTime LastRefreshTime
        {
            get { return (DateTime)GetValue(LastRefreshTimeProperty); }
            set { SetValue(LastRefreshTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastRefreshTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastRefreshTimeProperty =
            DependencyProperty.Register("LastRefreshTime", typeof(DateTime), typeof(FlexGrid), new PropertyMetadata(DateTime.Now));

        #endregion


        #endregion
    }
}
