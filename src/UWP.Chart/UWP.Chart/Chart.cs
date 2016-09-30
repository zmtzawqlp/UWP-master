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
            _series.CollectionChanged += _series_CollectionChanged;
            //_axis = new Axis();
            //_legend = new Legend();
            //_marker = new Marker();
        }

        private void _series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Series item in e.NewItems)
                    {
                        item.Chart = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (Series item in e.OldItems)
                    {
                        item.Chart = null;
                        //todo
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
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
                using (CanvasDrawingSession cds = cl.CreateDrawingSession())
                {
                    OnDraw(cds);
                }
                args.DrawingSession.DrawImage(cl);
            }
            if (ForceRedrawn)
            {
                _view.Invalidate();
            }
        }

        private void OnDraw(CanvasDrawingSession cds)
        {
            if (Axis != null && Axis.CanDraw)
            {
                Render.OnDrawAxis(this, cds);
            }

            if (Series != null)
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
