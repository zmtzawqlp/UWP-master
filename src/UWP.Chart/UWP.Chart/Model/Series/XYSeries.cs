using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Chart
{
    public class XYSeries : Series
    {
        #region Public Property
        /// <summary>
        /// Gets or sets the Binding to use for identifying the independent value.
        /// </summary>
        public Binding IndependentValueBinding
        {
            get
            {
                return _independentValueBinding;
            }
            set
            {
                if (_independentValueBinding != value)
                {
                    _independentValueBinding = value;
                    OnPropertyChanged("IndependentValueBinding");
                }
            }
        }

        /// <summary>
        /// The binding used to identify the independent value binding.
        /// </summary>
        private Binding _independentValueBinding;

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the independent value.
        /// </summary>
        public string IndependentValuePath
        {
            get
            {
                return (null != IndependentValueBinding) ? IndependentValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    IndependentValueBinding = null;
                }
                else
                {
                    IndependentValueBinding = new Binding() { Path = new PropertyPath(value) };
                }
            }
        }
        #endregion
    }
}
