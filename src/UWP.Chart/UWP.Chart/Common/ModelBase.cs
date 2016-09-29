using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UWP.Chart.Common
{
    public abstract class ModelBase : BindableBase
    {
        public bool CanDraw
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
        }

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ModelBase), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnDependencyPropertyChangedToInvalidate)));


        private Chart _chart;

        internal Chart Chart
        {
            get { return _chart; }
            set
            {
                _chart = value;
                //OnPropertyChanged();
            }
        }

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
