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
using Windows.UI.Popups;
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
        private ObservableCollection<Thing> _things;
        private ResizeableItems _resizeableItems;
        public VirtualizedVariableSizedGridViewPage()
        {
            this.InitializeComponent();
          
            Loaded += VirtualizedVariableSizedGridView_Loaded;
        }

        private async void VirtualizedVariableSizedGridView_Loaded(object sender, RoutedEventArgs e)
        {

            _things = new ObservableCollection<Thing>(1000, (startIndex, count) =>
            {

                return new MainPageViewModel().Things;
            });

            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                this.demoList.ItemsSource = _things;
            }
            else
            {
                var rowAapter = new ObservableRowAapter<Thing>(_things);
                //you can define your ResizeableItems as you wish
                InitializeResizeableItems();
                rowAapter.ResizeableItems = _resizeableItems;
                this.demoList.ItemsSource = rowAapter;
            }

        }

        /// you can define it in ObservableRowAapter's ResizeableItems property.
        /// for example, windowMinwidth is 500, windowMaxwidth is 1920
        /// default is (1920-500)/4=355
        /// that means 
        /// 1- 501 to 501+355 is two column style
        /// 2- 501 + 355*2+1 to 501 + 355*3 is two column style
        /// 3- 501 + 355*3+1 to 501 + 355*4 is three column style
        /// 4- 501 + 355*4+1 to double.PositiveInfinity is four column style
        /// Notice:make sure each group has the same items. 
        /// for example, each ResizeableItem has 15 Resizable.
        private void InitializeResizeableItems()
        {
            if (_resizeableItems == null)
            {
                _resizeableItems = new ResizeableItems();

                //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(200, 200));

                double windowMinwidth = 500;
                double windowMaxwidth = DeviceInfo.DeviceScreenSize.Width;
                double rangwidth = (windowMaxwidth - windowMinwidth) / 4.0;

                #region 4
                var list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c4 = new ResizeableItem() { Columns = 4, Items = list, Min = windowMinwidth + rangwidth * 3 + 1, Max = double.PositiveInfinity };
                _resizeableItems.Add(c4);
                #endregion

                #region 3
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c3 = new ResizeableItem() { Columns = 3, Items = list, Min = windowMinwidth + rangwidth * 2 + 1, Max = windowMinwidth + rangwidth * 3 };
                _resizeableItems.Add(c3);
                #endregion

                #region 2
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 2 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });
                list.Add(new Resizable() { Width = 1, Height = 1 });

                var c2 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + rangwidth * 1 + 1, Max = windowMinwidth + rangwidth * 2 };
                _resizeableItems.Add(c2);
                #endregion

                #region 1
                list = new List<Resizable>();
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });
                list.Add(new Resizable() { Width = 2, Height = 1 });

                var c1 = new ResizeableItem() { Columns = 2, Items = list, Min = windowMinwidth + +1, Max = windowMinwidth + rangwidth * 1 };
                _resizeableItems.Add(c1);
                #endregion
            }
        }


        private async void demoList_ItemClick(object sender, ItemClickEventArgs e)
        {
           await  new MessageDialog((e.ClickedItem as Thing).Name).ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_things!=null)
            {
                _things.Clear();
            }
        }

        private void PullToRefreshPanel_PullToRefresh(object sender, EventArgs e)
        {
            if (_things != null)
            {
                _things.Clear();
            }
        }
    }
    
}
