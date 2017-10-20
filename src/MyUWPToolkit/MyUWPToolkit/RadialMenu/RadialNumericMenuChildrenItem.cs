using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit.RadialMenu
{
    /// <summary>
    /// This is internal class only for RadialNumericMenuItem
    /// </summary>

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public class RadialNumericMenuChildrenItem : RadialMenuItem
    {
        private Path _colorElement;

        internal Path ColorElement
        {
            get
            {
                return _colorElement;
            }
        }

        //internal Border ColorElementClipBorder;

        //private double isFirstOrLastOneOpacity;

        //public double IsFirstOrLastOneOpacity
        //{
        //    get { return isFirstOrLastOneOpacity; }
        //    set
        //    {
        //        isFirstOrLastOneOpacity = value;
        //        OnPropertyChanged(nameof(IsFirstOrLastOneOpacity));
        //    }
        //}


        private Line line1;

        public Line Line1
        {
            get { return line1; }
            set
            {
                line1 = value;
                OnPropertyChanged(nameof(Line1));
            }
        }

        private Line line2;

        public Line Line2
        {
            get { return line2; }
            set
            {
                line2 = value;
                OnPropertyChanged(nameof(Line2));
            }
        }


        public double MarkThick
        {
            get { return (double)GetValue(MarkThickProperty); }
            set { SetValue(MarkThickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkThick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkThickProperty =
            DependencyProperty.Register("MarkThick", typeof(double), typeof(RadialNumericMenuChildrenItem), new PropertyMetadata(2.0));

        public double CircularThickness
        {
            get { return (double)GetValue(CircularThicknessProperty); }
            set { SetValue(CircularThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircularThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircularThicknessProperty =
            DependencyProperty.Register("CircularThickness", typeof(double), typeof(RadialNumericMenuChildrenItem), new PropertyMetadata(1.0));

        public Brush CircularBrush
        {
            get { return (Brush)GetValue(CircularBrushProperty); }
            set { SetValue(CircularBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircularBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircularBrushProperty =
            DependencyProperty.Register("CircularBrush", typeof(Brush), typeof(RadialNumericMenuChildrenItem), new PropertyMetadata(null));



        public RadialNumericMenuChildrenItem()
        {
            this.DefaultStyleKey = typeof(RadialNumericMenuChildrenItem);
            IsSelectedEnable = true;
            ManipulationMode = ManipulationModes.None;
            PrepareElements();
            Loaded += RadialNumericMenuChildrenItem_Loaded;

        }

        protected override void OnApplyTemplate()
        {
            _colorElement = GetTemplateChild("ColorElement") as Path;
            //ColorElementClipBorder = GetTemplateChild("ColorElementClipBorder") as Border;
            base.OnApplyTemplate();
        }

        private void RadialNumericMenuChildrenItem_Loaded(object sender, RoutedEventArgs e)
        {
            OnIsSelectedChanged();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (!IsSelected)
                VisualStateManager.GoToState(this, "PointerOver", false);
            e.Handled = true;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (!IsSelected)
                VisualStateManager.GoToState(this, "Normal", false);

            e.Handled = true;
            base.OnPointerExited(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                UpdateIsSelectedState();
            }
            base.OnPointerReleased(e);
        }
    }
}
