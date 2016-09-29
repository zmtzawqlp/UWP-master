using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP.Chart
{
    public class BubbleSeries : XYSeries
    {
        #region Public Property
        /// <summary>
        /// The binding used to identify the size value.
        /// </summary>
        private Binding _sizeValueBinding;

        /// <summary>
        /// Gets or sets the Binding to use for identifying the size of the bubble.
        /// </summary>
        public Binding SizeValueBinding
        {
            get
            {
                return _sizeValueBinding;
            }
            set
            {
                if (_sizeValueBinding != value)
                {
                    _sizeValueBinding = value;
                    OnPropertyChanged("SizeValueBinding");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the size of the bubble.
        /// </summary>
        public string SizeValuePath
        {
            get
            {
                return (null != SizeValueBinding) ? SizeValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    SizeValueBinding = null;
                }
                else
                {
                    SizeValueBinding = new Binding { Path = new PropertyPath(value) };
                }
            }
        }
        #endregion

        #region DependencyProperty


        public DoubleCollection SizeValues
        {
            get { return (DoubleCollection)GetValue(SizeValuesProperty); }
            set { SetValue(SizeValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeValuesProperty =
            DependencyProperty.Register("SizeValues", typeof(DoubleCollection), typeof(BubbleSeries), new PropertyMetadata(null, OnSizeValuesChanged));

        private static void OnSizeValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }





        public IEnumerable SizeValuesSource
        {
            get { return (IEnumerable)GetValue(SizeValuesSourceProperty); }
            set { SetValue(SizeValuesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeValuesSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeValuesSourceProperty =
            DependencyProperty.Register("SizeValuesSource", typeof(IEnumerable), typeof(BubbleSeries), new PropertyMetadata(null, OnSizeValuesSourceChanged));

        private static void OnSizeValuesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion
    }
}
