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
using Windows.Foundation;

namespace UWP.Chart
{
    [ContentProperty(Name = nameof(Data))]
    public partial class Chart
    {
        #region Fields
        private CanvasControl _view;
        private Grid _rootGrid;
        private Size preViewSize = Size.Empty;
       
        #region Model
        private Axes _axes;
        private Legend _legend;
        private Marker _marker;

        #endregion

        #region Render
        private IChartRender _render;
        private IChartRender _defaultRender;
        #endregion

        #endregion

        #region Internal Property
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

        internal bool LegendCanDraw
        {
            get
            {
                if (Legend == null)
                {
                    return false;
                }
                return Legend.CanDraw;
            }
        }

        internal bool DataCanDraw
        {
            get
            {
                if (Data == null)
                {
                    return false;
                }

                var canDraw = Data.Children.FirstOrDefault(x => x.CanDraw);

                return (Data.CanDraw && canDraw != null);
            }
        }

        internal bool AxesCanDraw
        {
            get
            {
                if (Axes == null)
                {
                    return false;
                }
                var canDraw = Axes.Children.FirstOrDefault(x => x.CanDraw);

                return (Axes.CanDraw && canDraw != null);
            }
        }

        internal bool MarkerCanDraw
        {
            get
            {
                if (Marker == null)
                {
                    return false;
                }
                return Marker.CanDraw;
            }
        }

        internal bool forceCreateDataResources;
        internal bool forceArrangeChildren;
        #endregion

        #region Public Property

        public Axes Axes
        {
            get { return _axes; }

            set
            {
                if (value != null)
                {
                    value.Chart = this;
                }
                _axes = value;
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

        #region Dependency Property

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));



        public IEnumerable ItemNames
        {
            get { return (IEnumerable)GetValue(ItemNamesProperty); }
            set { SetValue(ItemNamesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemNames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNamesProperty =
            DependencyProperty.Register("ItemNames", typeof(IEnumerable), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));



        /// <summary>
        /// It works when it is not double.NaN or more than zero.
        /// every series item size(for example,ColumnSeries item default width is double.NaN.)
        /// when It is not a available value, ItemSize = Actual Chart Size(Series area)(width/height) / Series data count
        /// </summary>
        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN, OnDependencyPropertyChangedToInvalidate));



        public SeriesData Data
        {
            get { return (SeriesData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(SeriesData), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnDependencyPropertyChangedToInvalidate)));

        #endregion
    }
}
