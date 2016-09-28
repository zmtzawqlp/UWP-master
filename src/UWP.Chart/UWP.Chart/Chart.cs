using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Render;
using Windows.UI.Xaml.Controls;

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
        }
        #endregion

        #region Private Method
        private void InitializeComponent()
        {
            _defaultRender = new ChartWin2DRender();
            _series = new SeriesCollection();
            _axis = new Axis();
            _legend = new Legend();
            _marker = new Marker();
          
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

        }

        private void _view_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }
        #endregion

        #region Public Methods

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
