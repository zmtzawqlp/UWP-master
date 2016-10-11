using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using UWP.Chart.Render;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UWP.Chart
{
    /// <summary>
    /// 
    /// </summary>

    public partial class Chart : Control, IDisposable
    {
        public Chart()
        {
            this.DefaultStyleKey = typeof(Chart);
            InitializeComponent();
            Loaded += Chart_Loaded;
            Unloaded += Chart_Unloaded;
            SizeChanged += Chart_SizeChanged;
        }

        #region Override
        protected override void OnApplyTemplate()
        {
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            base.OnApplyTemplate();
            CreateCanvas();

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeChilden(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void ArrangeChilden(Size finalSize)
        {
            Rect axesRectX = Rect.Empty;
            Rect axesRectY = Rect.Empty;
            Rect seriesRect = Rect.Empty;
            Rect legendRect = Rect.Empty;

            List<FrameworkElementBase> listX = new List<FrameworkElementBase>();
            List<FrameworkElementBase> listY = new List<FrameworkElementBase>();

            if (Legend != null && Legend.CanDraw)
            {
                switch (Legend.Position)
                {
                    case LegendPosition.Left:
                    case LegendPosition.Right:
                        listX.Add(Legend);
                        break;
                    case LegendPosition.Top:
                    case LegendPosition.Bottom:
                        listY.Add(Legend);
                        break;
                    default:
                        break;
                }
            }

            if (Axes != null && Axes.CanDraw)
            {
                if (Axes.AxisX.Count > 0)
                {
                    listX.AddRange(Axes.AxisX);
                }

                if (Axes.AxisY.Count > 0)
                {
                    listY.AddRange(Axes.AxisY);
                }
            }

            if (Data != null)
            {
                listX.Add(Data);
                listY.Add(Data);
            }

            double startCount = 0;

            double requiredSize = 0;

            foreach (var item in listX)
            {
                if (item.Width.IsStar)
                {
                    startCount += item.Width.Value;
                }
                else
                {
                    requiredSize += item.Width.Value;
                    item._actualWidth = item.Width.Value;
                }
            }

            if (startCount > 0)
            {
                var totalSize = finalSize.Width - requiredSize;
                var starValue = totalSize > 0 ? totalSize / startCount : 0;

                foreach (var item in listX)
                {
                    if (item.Width.IsStar)
                    {
                        item._actualWidth = starValue * item.Width.Value;
                    }
                }
            }

            startCount = 0;

            requiredSize = 0;

            foreach (var item in listY)
            {
                if (item.Height.IsStar)
                {
                    startCount += item.Height.Value;
                }
                else
                {
                    requiredSize += item.Height.Value;
                    item._actualHeight = item.Height.Value;
                }
            }

            if (startCount > 0)
            {
                var totalSize = finalSize.Height - requiredSize;
                var starValue = totalSize > 0 ? totalSize / startCount : 0;

                foreach (var item in listY)
                {
                    if (item.Height.IsStar)
                    {
                        item._actualHeight = starValue * item.Height.Value;
                    }
                }
            }

        }

        #endregion

        #region Private Method
        private void InitializeComponent()
        {
            forceReCreateResources = true;
            _defaultRender = new ChartRender();
            Data = new SeriesData();
            //_data.CollectionChanged += _series_CollectionChanged;
            //_axis = new Axis();
            //_legend = new Legend();
            //_marker = new Marker();
        }

        private void Chart_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Dispose();
        }

        private void Chart_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void Chart_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {

        }

        #endregion

        #region View
        private void _view_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            using (CanvasCommandList cl = new CanvasCommandList(sender))
            {
                using (CanvasDrawingSession cds = cl.CreateDrawingSession())
                {
                    OnDraw(cds);
                }
                args.DrawingSession.DrawImage(cl);
            }
        }

        private void OnDraw(CanvasDrawingSession cds)
        {
            if (Axes != null && Axes.CanDraw)
            {
                Render.OnDrawAxis(this, cds);
            }

            if (Data != null)
            {
                Render.OnDrawSeries(this, cds);
            }

            if (Marker != null && Marker.CanDraw)
            {
                Render.OnDrawMarker(this, cds);
            }

            if (Legend != null && Legend.CanDraw)
            {
                Render.OnDrawLegend(this, cds);
            }

        }

        private void _view_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CreateDataResources();
        }


        /// <summary>
        /// Create data mode for render
        /// </summary>
        private void CreateDataResources()
        {
            if (Data != null)
            {
                foreach (ISeries item in Data.Children)
                {
                    item.CreateDataResources();
                }
            }

        }
        #endregion

        #region Internal Methods


        #endregion

        #region Public Methods
        //Indicates that the contents of the Chart need to be redrawn. 
        public void Invalidate()
        {
            CreateDataResources();
            if (_view != null)
            {
                _view.Invalidate();
            }
        }
        #endregion

        #region Canvas

        public void CreateCanvas()
        {
            if (_view == null)
            {
                var view = View;
            }

            if (_view.Parent == null && _rootGrid != null)
            {
                _rootGrid.Children.Add(_view);
            }
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.Draw -= _view_Draw;
                _view.CreateResources -= _view_CreateResources;
                _view.RemoveFromVisualTree();
                _view = null;
            }
        }
        #endregion

        #region PropertyChanged
        internal static void OnDependencyPropertyChangedToInvalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (e.Property.ToString() == "VisibilityProperty")
            {

                var chart = d as Chart;
                if (e.Property == DataProperty)
                {
                    chart.Data.Chart = chart;
                }

                chart.Invalidate();
            }
        }

        internal void OnPropertyChangedToInvalidate()
        {
            Invalidate();
        }
        #endregion

    }
}
