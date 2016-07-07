using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace UWP.Chart
{
    public partial class Chart
    {
        #region Fields
        private CanvasControl _view;
        #endregion

        #region Internal properties
        internal CanvasControl View
        {
            get
            {
                if (_view == null)
                {
                    _view = new CanvasControl();
                    _view.Draw += _view_Draw;
                }
                return _view;
            }
        }

        #endregion

        #region Public properties

        #endregion

        #region DP

        #endregion
    }
}
