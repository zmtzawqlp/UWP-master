using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
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
    [ContentProperty(Name = nameof(Series))]
    public partial class Chart : Control, IDisposable
    {
        public Chart()
        {
            this.DefaultStyleKey = typeof(Chart);
            InitializeComponent();
            Loaded += Chart_Loaded;
            Unloaded += Chart_Unloaded;
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
            _defaultRender = new ChartRender();
            _series = new SeriesCollection();
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
        #endregion

        #region View
        private void _view_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            using (CanvasCommandList cl = new CanvasCommandList(sender))
            {
                using (CanvasDrawingSession clds = cl.CreateDrawingSession())
                {
                    OnDraw(clds);
                }
                args.DrawingSession.DrawImage(cl);
            }
            if (ForceRedrawn)
            {
                _view.Invalidate();
            }
        }

        private void OnDraw(CanvasDrawingSession clds)
        {
            if (Axis != null)
            {
                Render.OnDrawAxis(this, clds);
            }

            if (Series.Count != 0)
            {
                Render.OnDrawSeries(this, clds);
            }

            if (Marker != null)
            {
                Render.OnDrawMarker(this, clds);
            }

            if (Legend != null)
            {
                Render.OnDrawLegend(this, clds);
            }
        }

        private void _view_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }
        #endregion

        #region Public Methods
        //Indicates that the contents of the Chart need to be redrawn. 
        public void Invalidate()
        {
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
