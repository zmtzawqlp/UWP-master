using MyUWPToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvancedFlyoutPage : Page
    {

        public AdvancedFlyoutPage()
        {
            this.InitializeComponent();
            combobox.ItemsSource = Enum.GetNames(typeof(AdvancedFlyoutPlacementMode));
            combobox.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Flyout flyout1 = new Flyout();

            //Border b = new Border();
            //b.Width = 600;
            //b.Height = 600;
            //b.Background = new SolidColorBrush(Colors.Red);
            //Button c = new Button();
            //c.Click += C_Click;
            //c.Content = "Click me";
            //b.Child = c;
            //flyout1.Content = b;
            //flyout.Placement = AdvancedFlyoutPlacementMode.Full;
            //flyout.FlyoutPresenterStyle = this.Resources["FlyoutPresenterStyle"] as Style;
            flyout.ShowAt((sender as FrameworkElement));
            //flyout.ShowAt((sender as FrameworkElement));
            // flyout.Content=
            //flyout1.Opened += Flyout1_Opened;
        }

        private void Flyout1_Opened(object sender, object e)
        {
            var a = ((((sender as Flyout).Content as Border).Parent as FlyoutPresenter).Parent as Popup);
            foreach (var item in a.ChildTransitions)
            {

            }

        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            AdvancedFlyout flyout1 = new AdvancedFlyout();

            Border b = new Border();
            b.Width = 300;
            b.Height = 300;
            b.Background = new SolidColorBrush(Colors.Red);
            Button c = new Button();
            c.Click += C_Click;
            c.Content = "Click me";
            b.Child = c;
            flyout1.Content = b;

            flyout1.ShowAt((sender as FrameworkElement));
        }

        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            flyout.Placement = (AdvancedFlyoutPlacementMode)combobox.SelectedIndex;
        }

        private void HorizontalOffsetTB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            flyout.HorizontalOffset = e.NewValue;
        }

        private void VerticalOffsetTB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            flyout.VerticalOffset = e.NewValue;
        }
    }
}
