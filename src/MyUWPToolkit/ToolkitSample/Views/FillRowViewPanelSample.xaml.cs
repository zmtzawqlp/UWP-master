using MyUWPToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolkitSample.Model;
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

namespace ToolkitSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FillRowViewPanelSample : Page
    {
        FillRowViewSource<TuchongImage> source;
        TuchongImageSource ttSource;
        public FillRowViewPanelSample()
        {
            this.InitializeComponent();
            ttSource= new TuchongImageSource();
            source = new FillRowViewSource<TuchongImage>(ttSource, 1);
            FillRowView.ItemsSource = source;
        }

        private void FillRowView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 600)
            {
                source.UpdateRowItemsCount(2);
            }
            else if (e.NewSize.Width >= 600 && e.NewSize.Width < 900)
            {
                source.UpdateRowItemsCount(3);
            }
            else
            {
                source.UpdateRowItemsCount(4);
            }
        }

        private void PullToRefreshGrid_PullToRefresh(object sender, EventArgs e)
        {
            ttSource.Reset();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ttSource.Reset();
        }
    }
}
