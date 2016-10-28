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
    public class ColorButton : Control
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
                cb.GoToState("PointerOver");
            }
            else
            {
                cb.GoToState("Normal");
            }
        }

        public bool IsPointerOver
        {
            get { return (bool)GetValue(IsPointerOverProperty); }
            set { SetValue(IsPointerOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPointerOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPointerOverProperty =
            DependencyProperty.Register("IsPointerOver", typeof(bool), typeof(ColorButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsPointerOverChanged)));

        private static void OnIsPointerOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cb = (d as ColorButton);
            if (cb.IsPointerOver)
            {
                cb.GoToState("PointerOver");
            }
            else
            {
                cb.GoToState("Normal");
            }
        }

        public ColorButton()
        {
            this.DefaultStyleKey = typeof(ColorButton);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            IsPointerOver = true;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            IsPointerOver = false;
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            GoToState("Pressed");
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            GoToState("Normal");
            base.OnPointerReleased(e);
        }

        internal void GoToState(string stateName)
        {
            VisualStateManager.GoToState(this, stateName, true);
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
