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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
        }

        private void CoreWindow_SizeChanged(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
        {
            
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            WebView webView = new WebView();
            string width = await webView.InvokeScriptAsync("eval", new string[] { "window.screen.width.toString()" });
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            switch (e.ClickedItem.ToString())
            {

                case "ImageTool":
                    Frame.Navigate(typeof(ImageToolPage));
                    break;
                case "PullToRefreshControl":
                    Frame.Navigate(typeof(PullToRefreshControl));
                    break;
                case "CropImageControl":
                    Frame.Navigate(typeof(CropImageControlPage));
                    break;
                case "ColumnChart":
                    Frame.Navigate(typeof(ColumnChartSample));
                    break;
                case "VirtualizedVariableSizedGridView":
                    Frame.Navigate(typeof(VirtualizedVariableSizedGridViewPage));
                    break;
                default:
                    break;
            }
        }
    }


   
}
