using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Chart
{
    /// <summary>
    /// 
    /// </summary>
    public class DataPointSeries : Series
    {
        #region Public Property
        /// <summary>
        /// Gets or sets the Binding to use for identifying the independent(x) value.
        /// </summary>
        public Binding XValueBinding
        {
            get
            {
                return _xValueBinding;
            }
            set
            {
                if (_xValueBinding != value)
                {
                    _xValueBinding = value;
                    OnPropertyChanged("XValueBinding");
                }
            }
        }

        /// <summary>
        /// The binding used to identify the independent value binding.
        /// </summary>
        private Binding _xValueBinding;

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the independent(x) value.
        /// </summary>
        public string XValuePath
        {
            get
            {
                return (null != XValueBinding) ? XValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    XValueBinding = null;
                }
                else
                {
                    XValueBinding = new Binding() { Path = new PropertyPath(value) };
                }
            }
        }
        #endregion
    }
}
