using MyUWPToolkit;
using MyUWPToolkit.RadialMenu;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolkitSample.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
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
            
           
        }



        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                listView.Items.Add("CustomKeyboardPage");
            }
            else
            {
                listView.Items.Add("VirtualizedVariableSizedGridView");   
            }
            //WebView webView = new WebView();
            //string width = await webView.InvokeScriptAsync("eval", new string[] { "window.screen.width.toString()" });
            var a = contentControl.GetScrollViewer();
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
                case "CustomKeyboardPage":
                    Frame.Navigate(typeof(CustomKeyboardPage));
                    break;
                case "WIN2DPage":
                    Frame.Navigate(typeof(WIN2DPage));
                    break;
                case "FlexGrid":
                    Frame.Navigate(typeof(FlexGridSamplePage));
                    break;
                case "GroupListView":
                    Frame.Navigate(typeof(GroupListViewPage));
                    break;
                case "Test":
                    Frame.Navigate(typeof(BlankPage1));
                    break;
                case "ColorPicker":
                    Frame.Navigate(typeof(ColorPickerPage));
                    break;
                case "AdvancedFlyout":
                    Frame.Navigate(typeof(AdvancedFlyoutPage));
                    break;
                case "HightLightedRadioButton":
                    Frame.Navigate(typeof(HightLightedRadioButtonSamplePage));
                    break;
                case "RadialMenu":
                    Frame.Navigate(typeof(RadialMenuSample));
                    break;
                default:
                    break;
            }
        }
    }

   
}

public static class Extensions
{
    public static ScrollViewer GetScrollViewer(this DependencyObject element)
    {
        if (element is ScrollViewer)
        {
            return (ScrollViewer)element;
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);

            var result = GetScrollViewer(child);
            if (result == null)
            {
                continue;
            }
            else
            {
                return result;
            }
        }

        return null;
    }
}
