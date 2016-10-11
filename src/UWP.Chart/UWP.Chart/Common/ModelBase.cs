using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace UWP.Chart.Common
{
    public abstract class ModelBase : BindableBase
    {

        #region Fields
        private Chart _chart;
        #endregion

        #region Properties

        internal virtual Chart Chart
        {
            get { return _chart; }
            set
            {
                _chart = value;
                OnPropertyChanged("CropRect");
            }
        }

        public virtual bool CanDraw
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
        }

      
        #endregion

        #region DP
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ModelBase), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnDependencyPropertyChangedToInvalidate)));
        #endregion

        #region OnPropertyChangedToInvalidate

        internal static void OnDependencyPropertyChangedToInvalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (e.Property.ToString() == "VisibilityProperty")
            {
                var modelBase = d as ModelBase;
                if (modelBase.Chart != null)
                {
                    modelBase.Chart.Invalidate();
                }
            }
        }

        protected void OnPropertyChangedToInvalidate()
        {
            if (Chart != null)
            {
                Chart.Invalidate();
            }
        }
        #endregion

    }
}
