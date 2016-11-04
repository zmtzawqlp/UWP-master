using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColorPickerPage : Page
    {
        public ColorPickerPage()
        {
            this.InitializeComponent();
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            colorPicker.Placement = FlyoutPlacementMode.Right;
            colorPicker.PlacementTarget = (sender as FrameworkElement);
            colorPicker.Owner = sender;
            colorPicker.Show();
            
        }

        private void colorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            if (colorPicker.Owner!=null)
            {
                (colorPicker.Owner as Rectangle).Fill = new SolidColorBrush(colorPicker.SelectedColor);
                colorPicker.Owner = null;
            }
        }
    }
}
