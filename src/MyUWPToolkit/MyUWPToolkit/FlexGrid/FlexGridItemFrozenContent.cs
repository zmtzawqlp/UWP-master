using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridItemFrozenContent
    {
        public static bool GetIsFrozenContent(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFrozenContentProperty);
        }

        public static void SetIsFrozenContent(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFrozenContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsFrozenContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFrozenContentProperty =
            DependencyProperty.RegisterAttached("IsFrozenContent", typeof(bool), typeof(FlexGridItemFrozenContent), new PropertyMetadata(false,new PropertyChangedCallback(IsFrozenContentChanged)));

        private static void IsFrozenContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d is UIElement)
            {
                Canvas.SetZIndex(d as UIElement, 10);
            }
        }
    }
}
