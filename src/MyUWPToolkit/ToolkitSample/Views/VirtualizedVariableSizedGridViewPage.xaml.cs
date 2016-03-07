using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolkitSample.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class VirtualizedVariableSizedGridViewPage : Page
    {
        private MyIncrementalLoading<Thing> _things;
        public VirtualizedVariableSizedGridViewPage()
        {
            this.InitializeComponent();
          
            Loaded += VirtualizedVariableSizedGridView_Loaded;
        }

        private async void VirtualizedVariableSizedGridView_Loaded(object sender, RoutedEventArgs e)
        {

            //demoList.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            //demoList.DataFetchSize = 2.0;
            //demoList.IncrementalLoadingThreshold = 1.0;
            _things = new MyIncrementalLoading<Thing>(14, (startIndex, count) =>
            {

                return new MainPageViewModel().Things;
            });

            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                this.demoList.ItemsSource = _things;
            }
            else
            {
                var a = new ObservableRowAapter<Thing>(_things);

                this.demoList.ItemsSource = a;
            }

        }
    }
    
}
