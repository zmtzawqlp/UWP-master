using BodyNamed.Pages;
using BodyNamed.Utils;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BodyNamed
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationService.RegisterCustomFrame(innerFrame, nameof(innerFrame));
        }

        private void ToggleSplitPane(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
            hamburgerButton.Width = splitView.IsPaneOpen ? 100 : 48;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (innerFrame.Content == null)
            {
                line1.Opacity = 1;
                line2.Opacity= line3.Opacity = 0;
                NavigationService.Navigate(typeof(HomePage));
            }
        }

        private void homeSP_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line1.Opacity = 1;
            line2.Opacity = line3.Opacity = 0;
            NavigationService.GoBackToRootPage();
        }

        private void settingsSP_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line2.Opacity = 1;
            line1.Opacity = line3.Opacity = 0;
            NavigationService.CleanNavigate(typeof(SettingsPage));
        }

        private void aboutSP_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line3.Opacity = 1;
            line1.Opacity = line2.Opacity = 0;
            NavigationService.CleanNavigate(typeof(AboutPage));
        }
    }
}
