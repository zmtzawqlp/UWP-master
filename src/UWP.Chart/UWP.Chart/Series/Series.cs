using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP.Chart.Series
{
    public class Series : BindableBase
    {

        #region Public Property
        private Binding _dependentValueBinding;
        /// <summary>
        /// The binding used to identify the dependent value binding.
        /// </summary>
        public Binding DependentValueBinding
        {
            get
            {
                return _dependentValueBinding;
            }
            set
            {
                if (_dependentValueBinding != value)
                {
                    _dependentValueBinding = value;
                    OnPropertyChanged("DependentValueBinding");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the dependent value.
        /// </summary>
        public string DependentValuePath
        {
            get
            {
                return (null != DependentValueBinding) ? DependentValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    DependentValueBinding = null;
                }
                else
                {
                    DependentValueBinding = new Binding() { Path = new PropertyPath(value) };
                }
            }
        }


        #endregion

        #region DependencyProperty
        /// <summary>
        /// Gets or sets the values(Y) collection.
        /// </summary>
        public DoubleCollection DependentValues
        {
            get { return (DoubleCollection)GetValue(DependentValuesProperty); }
            set { SetValue(DependentValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DependentValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DependentValuesProperty =
            DependencyProperty.Register("DependentValues", typeof(DoubleCollection), typeof(Series), new PropertyMetadata(null, OnDependentValuesChanged));

        private static void OnDependentValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        /// <summary>
        /// Gets or sets the dependent values(Y) source.
        /// </summary>
        public IEnumerable DependentValuesSource
        {
            get { return (IEnumerable)GetValue(DependentValuesSourceProperty); }
            set { SetValue(DependentValuesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DependentValuesSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DependentValuesSourceProperty =
            DependencyProperty.Register("DependentValuesSource", typeof(IEnumerable), typeof(Series), new PropertyMetadata(null, OnDependentValuesSourceChanged));

        private static void OnDependentValuesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion


    }
}
