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
    [ContentProperty(Name = nameof(Series))]
    public partial class Chart
    {
        #region Fields
        private CanvasControl _view;
        private Grid _rootGrid;

        #region Model
        private Axis _axis;
        private Legend _legend;
        private Marker _marker;
        private SeriesCollection _series;
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

        public SeriesCollection Series
        {
            get { return _series; }
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

        #endregion

        #region DP


        /// <summary>
        /// set it true if datas are changing frequently, so that don't need to call Chart's Invalidate method whenever datas are changed.
        /// </summary>
        public bool ForceRedrawn
        {
            get { return (bool)GetValue(ForceRedrawnProperty); }
            set { SetValue(ForceRedrawnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceRedrawn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceRedrawnProperty =
            DependencyProperty.Register("ForceRedrawn", typeof(bool), typeof(Chart), new PropertyMetadata(false, new PropertyChangedCallback(OnForceRedrawnChanged)));

        private static void OnForceRedrawnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (d as Chart);
            if (chart.ForceRedrawn)
            {
                (d as Chart).Invalidate();
            }
        }

        public ChartType ChartType
        {
            get { return (ChartType)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartTypeProperty =
            DependencyProperty.Register("ChartType", typeof(ChartType), typeof(Chart), new PropertyMetadata(ChartType.Column));


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




        #endregion
    }
}
