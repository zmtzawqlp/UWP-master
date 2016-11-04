using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyUWPToolkit.Common;
using Windows.Foundation;
using MyUWPToolkit.Util;

namespace MyUWPToolkit
{
    public class ColorPickerItemsControl : ItemsControl
    {
        public ColorPicker ColorPicker { get; internal set; }

        public ColorPickerItemsControl()
        {
            this.DefaultStyleKey = typeof(ColorPickerItemsControl);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FrameworkElement).Tapped += ColorPickerItemsControl_Tapped;
            base.PrepareContainerForItemOverride(element, item);
        }

        private void ColorPickerItemsControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (ColorPicker != null)
            {
                ColorPicker.SelectedColor = ColorConverter.GetColor((sender as FrameworkElement).DataContext?.ToString());
                ColorPicker.Hide();
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            (element as FrameworkElement).Tapped -= ColorPickerItemsControl_Tapped;
            base.ClearContainerForItemOverride(element, item);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
