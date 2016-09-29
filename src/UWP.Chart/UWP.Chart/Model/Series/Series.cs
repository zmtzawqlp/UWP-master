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

namespace UWP.Chart
{
    public class Series : ModelBase, ISeries
    {

        #region Public Property
        private Binding _valueBinding;
        /// <summary>
        /// The binding used to identify the value binding.
        /// </summary>
        public Binding ValueBinding
        {
            get
            {
                return _valueBinding;
            }
            set
            {
                if (_valueBinding != value)
                {
                    _valueBinding = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the value.
        /// </summary>
        public string ValuePath
        {
            get
            {
                return (null != ValueBinding) ? ValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    ValueBinding = null;
                }
                else
                {
                    ValueBinding = new Binding() { Path = new PropertyPath(value) };
                }
            }
        }


        #endregion

        #region DependencyProperty
        /// <summary>
        /// Gets or sets the values collection. 
        /// </summary>
        public DoubleCollection Values
        {
            get { return (DoubleCollection)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(DoubleCollection), typeof(Series), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));


        /// <summary>
        /// Gets or sets the dependent values source. use with DependentValueBinding
        /// </summary>
        public IEnumerable ValuesSource
        {
            get { return (IEnumerable)GetValue(ValuesSourceProperty); }
            set { SetValue(ValuesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuesSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesSourceProperty =
            DependencyProperty.Register("ValuesSource", typeof(IEnumerable), typeof(Series), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));



        public ChartType? ChartType
        {
            get { return (ChartType?)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartTypeProperty =
            DependencyProperty.Register("ChartType", typeof(ChartType?), typeof(Series), new PropertyMetadata(null));




        #endregion


        #region ISeries

        public string Label { get; set; }

        string[] ISeries.GetItemNames()
        {
            return GetItemNames();
        }

        double[,] ISeries.GetValues()
        {
            return GetValues();
        }

        virtual internal string[] GetItemNames()
        {
            throw new NotImplementedException();
        }

        virtual internal double[,] GetValues()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
