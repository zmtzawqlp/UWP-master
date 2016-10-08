using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.UI.Xaml.Markup;

namespace UWP.Chart
{
    [ContentProperty(Name = nameof(Children))]
    public class SeriesData : ModelBase
    {
        #region Fields
        private SeriesCollection _children;
        #endregion
        #region Public Properties

        public SeriesCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new SeriesCollection();
                }
                return _children;
            }
        }
        #endregion

    }
}
