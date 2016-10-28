using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyUWPToolkit
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "ToggleButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "Flyout", Type = typeof(Flyout))]
    [TemplatePart(Name = "BasicColorItems", Type = typeof(ItemsControl))]
    public partial class ColorPicker : Control
    {

        public ColorPicker()
        {
            this.DefaultStyleKey = typeof(ColorPicker);
        }

        protected override void OnApplyTemplate()
        {
            _toggleButton = GetTemplateChild("ToggleButton") as ToggleButton;
            _toggleButton.Click += _toggleButton_Click;
            _flyout = GetTemplateChild("Flyout") as Flyout;
            _basicColorItems = GetTemplateChild("BasicColorItems") as ItemsControl;
            _basicColorItems.ItemsSource = ColorPickerColorHelper.BasicColors;
            base.OnApplyTemplate();
        }

        private async void _toggleButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (_toggleButton.IsChecked != null)
            {
                if (_toggleButton.IsChecked.Value)
                {
                    var customColors = await ColorPickerColorHelper.GetCustomColorsAsync();

                    _flyout.ShowAt(this);
                }
                else
                {
                    _flyout.Hide();
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_toggleButton!=null)
            {
                (_toggleButton.Content as Grid).Width = finalSize.Width;
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
