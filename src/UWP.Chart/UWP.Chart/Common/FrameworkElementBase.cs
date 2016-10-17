using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace UWP.Chart.Common
{
    public class FrameworkElementBase : ModelBase
    {
        private Rect _cropRect;

        /// <summary>
        /// Gets the crop rectangle that defines model.
        /// </summary>
        public virtual Rect CropRect
        {
            get { return _cropRect; }
            internal set { _cropRect = value; }
        }

        internal double _actualHeight = double.NaN;
        internal double _actualWidth = double.NaN;

        public double ActualHeight
        {
            get
            {
                return _actualHeight;
            }
        }

        public double ActualWidth
        {
            get
            {
                return _actualWidth;
            }
        }

        #region Dependency Property
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(FrameworkElementBase), new PropertyMetadata(new GridLength(50), OnDependencyPropertyChangedToInvalidate));

        public GridLength Height
        {
            get { return (GridLength)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(GridLength), typeof(FrameworkElementBase), new PropertyMetadata(new GridLength(50), OnDependencyPropertyChangedToInvalidate));

        #endregion

    }
}
