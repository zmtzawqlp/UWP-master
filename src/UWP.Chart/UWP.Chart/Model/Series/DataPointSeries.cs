using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using UWP.Chart.Util;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP.Chart
{
    /// <summary>
    /// 
    /// </summary>
    public class DataPointSeries : Series
    {
        #region Fields
        protected List<object> xValues = null;
        #endregion
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



        public IEnumerable XValuesSource
        {
            get { return (IEnumerable)GetValue(XValuesSourceProperty); }
            set { SetValue(XValuesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XValuesSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XValuesSourceProperty =
            DependencyProperty.Register("XValuesSource", typeof(IEnumerable), typeof(DataPointSeries), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));



        public DoubleCollection XValues
        {
            get { return (DoubleCollection)GetValue(XValuesProperty); }
            set { SetValue(XValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.Register("XValues", typeof(DoubleCollection), typeof(DataPointSeries), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));



        #endregion

        #region override
        internal override double[,] GetValues(bool clearValues = false)
        {
            if (clearValues)
            {
                dValues = null;
                values = null;
                xValues = null;
            }

            if (dValues != null)
            {
                return dValues;
            }

            values = GetValues(ValueBinding, ValuesSource, Values);

            xValues = GetValues(XValueBinding, XValuesSource, XValues);

            dValues = CreateValues(new IList[] { xValues, values });

            if (isTimeValues == null)
                isTimeValues = new bool[2];
            isTimeValues[0] = IsTimeData(xValues);
            isTimeValues[1] = IsTimeData(values);

            return dValues;
        }



        #endregion

    }
}
