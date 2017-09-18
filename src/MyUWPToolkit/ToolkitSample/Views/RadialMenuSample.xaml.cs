using MyUWPToolkit.RadialMenu;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RadialMenuSample : Page
    {
        public RadialMenuSample()
        {
            this.InitializeComponent();
        }

        private void OnTapped1(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as RadialMenuItem);
            if (item.IsChecked)
            {
                item.Content = "阳线(实)";
            }
            else
            {
                item.Content = "阳线(空)";
            }
        }

        private void OnTapped2(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as RadialMenuItem);
            if (item.IsChecked)
            {
               tb.Text = "显示缺口";
            }
            else
            {
                tb.Text = "隐藏缺口";
            }
        }
    }
}
