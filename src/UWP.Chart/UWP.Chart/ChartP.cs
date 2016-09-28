using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using UWP.Chart.Render;

namespace UWP.Chart
{
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

            set { _axis = value; }
        }

        public Legend Legend
        {
            get { return _legend; }
            set { _legend = value; }
        }

        public Marker Marker
        {
            get { return _marker; }
            set { _marker = value; }
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

        #endregion
    }
}
