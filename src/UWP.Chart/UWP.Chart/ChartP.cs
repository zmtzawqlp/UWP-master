using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using UWP.Chart.Render;
using Windows.UI.Xaml.Markup;
using System.Collections;

namespace UWP.Chart
{
    [ContentProperty(Name = nameof(Data))]
    public partial class Chart
    {
        #region Fields
        private CanvasControl _view;
        private Grid _rootGrid;

        #region Model
        private Axis _axis;
        private Legend _legend;
        private Marker _marker;

        #endregion

        #region Render
        private IChartRender _render;
        private IChartRender _defaultRender;
        #endregion

        #endregion

        #region Internal properties
        internal CanvasControl View
        {
            get
            {
                if (_view == null)
                {
                    _view = new CanvasControl();
                    _view.CreateResources += _view_CreateResources;
                    _view.Draw += _view_Draw;
                }
                return _view;
            }
        }

        internal bool forceReCreateResources;

        #endregion

        #region Public properties

        public Axis Axis
        {
            get { return _axis; }

            set
            {
                if (value != null)
                {
                    value.Chart = this;
                }
                _axis = value;
            }
        }

        public Legend Legend
        {
            get { return _legend; }
            set
            {
                if (value != null)
                {
                    value.Chart = this;
                }
                _legend = value;

            }
        }

        public Marker Marker
        {
            get { return _marker; }
            set
            {
                if (value != null)
                {
                    value.Chart = this;
                }
                _marker = value;

            }
        }

        public IChartRender Render
        {
            get
            {
                if (_render == null)
                {
                    return _defaultRender;
                }
                return _render;
            }
            set
            {
                _render = value;
            }
        }

        private bool _autoGenerateSeries;

        public bool AutoGenerateSeries
        {
            get { return _autoGenerateSeries; }
            set { _autoGenerateSeries = value; }
        }


        #endregion

        #region DP

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Chart), new PropertyMetadata(null));



        public IEnumerable ItemNames
        {
            get { return (IEnumerable)GetValue(ItemNamesProperty); }
            set { SetValue(ItemNamesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemNames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNamesProperty =
            DependencyProperty.Register("ItemNames", typeof(IEnumerable), typeof(Chart), new PropertyMetadata(null));



        /// <summary>
        /// It works when AutoItemSize is false.
        /// every series item size(for example,ColumnSeries item default width is 20.)
        /// </summary>
        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(Chart), new PropertyMetadata(20.0));



        /// <summary>
        /// default value is true.
        /// ItemSize = Actual Chart Size(Series area)(width/height) / Series data count
        /// </summary>
        public bool AutoItemSize
        {
            get { return (bool)GetValue(AutoItemSizeProperty); }
            set { SetValue(AutoItemSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoItemSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoItemSizeProperty =
            DependencyProperty.Register("AutoItemSize", typeof(bool), typeof(Chart), new PropertyMetadata(true));


        public SeriesData Data
        {
            get { return (SeriesData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(SeriesData), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = d as Chart;
            if (chart.Data != null)
            {
                chart.Data.Chart = chart;
            }
        }



        #endregion
    }
}
