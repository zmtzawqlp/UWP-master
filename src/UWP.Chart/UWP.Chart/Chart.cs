using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Render;
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
            if (Axis != null && Axis.CanDraw)
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


        private void CreateDataResources()
        {
            if (Data!=null)
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

    }
}
