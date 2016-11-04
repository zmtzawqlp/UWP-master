using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{
    public class ColorButton : ContentControl
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorButton), new PropertyMetadata(Colors.Transparent));



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ColorButton), new PropertyMetadata(false,new PropertyChangedCallback(OnIsSelectedChanged)));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cb = (d as ColorButton);
            if (cb.IsSelected)
            {
                cb.GoToState("IsSelected");
            }
            else
            {
                cb.GoToState("Normal");
            }
        }


        public ColorButton()
        {
            this.DefaultStyleKey = typeof(ColorButton);
            IsTabStop = true;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            IsSelected = true;
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            IsSelected = false;
            base.OnLostFocus(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            IsSelected = true;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            IsSelected = false;
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
      
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
        }

        internal void GoToState(string stateName)
        {
            VisualStateManager.GoToState(this, stateName, false);
            //switch (stateName)
            //{
            //    case "PointerOver":
            //        if (!IsPointerOver)
            //        {
            //            IsPointerOver = true;
            //        }
            //        break;
            //    case "Normal":
            //    case "Pressed":
            //        IsPointerOver = false;
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
