using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP.DataGridSample.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.DataGridSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        private ObservableCollection<Employee> _employees;
        public BlankPage1()
        {
            this.InitializeComponent();
            Loaded += BlankPage1_Loaded;
            listview.Loaded += Listview_Loaded;
            //listview1.Loaded += Listview1_Loaded;
        }


        private void BlankPage1_Loaded(object sender, RoutedEventArgs e)
        {
            //_employees = new ObservableCollection<Employee>(TestData.GetEmployees().Take(200).ToList());
            _employees = new MyIncrementalLoading<Employee>(1000, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });


            //_employees = TestData.GetEmployees();

            listview.ItemsSource = _employees;
            //ItemsControl a = new ItemsControl();

            //listview1.ItemsSource = _employees;

        }

        private void Listview_Loaded(object sender, RoutedEventArgs e)
        {

            sv1 = GetFirstChildOfType<ScrollViewer>(this.listview) as ScrollViewer;
            var a = GetFirstChildOfType<ScrollBar>(sv1, 1);
            a.ValueChanged += A_ValueChanged;
            //sv1.Loaded += Sv1_Loaded;
            //sv1.ViewChanging += Sv1_ViewChanging;
            sv1.ViewChanged += Sv1_ViewChanged;
            a.Scroll += A_Scroll;
        }

        private void A_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void A_LayoutUpdated(object sender, object e)
        {
            //sv2.ChangeView(0, sv1.VerticalOffset, null, true);
            Debug.WriteLine("A_LayoutUpdated");
        }

        private void Sv1_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.IsInertial)
            {
                //sv2.ChangeView(0, e.FinalView.VerticalOffset, null, false);
                Debug.WriteLine("IsInertial:true : " + e.FinalView.VerticalOffset + "," + e.NextView.VerticalOffset);
            }
            else
            {

                Debug.WriteLine("IsInertial:false : " + e.FinalView.VerticalOffset + "," + e.NextView.VerticalOffset);
            }
            //sv2.ChangeView(0, e.NextView.VerticalOffset, null, true);
        }

        private void Sv1_Loaded(object sender, RoutedEventArgs e)
        {
            bar1 = GetFirstChildOfType<ScrollBar>(sv1);
            bar1.ValueChanged += Bar1_ValueChanged;
        }

        private void Listview1_Loaded(object sender, RoutedEventArgs e)
        {
            //sv2 = GetFirstChildOfType<ScrollViewer>(this.listview1) as ScrollViewer;
            sv2.Loaded += Sv2_Loaded;
            sv2.ViewChanging += Sv2_ViewChanging;

        }

        private void Sv2_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //sv1.ChangeView(0, e.NextView.VerticalOffset, null, true);
        }

        private void Sv2_Loaded(object sender, RoutedEventArgs e)
        {
            bar2 = GetFirstChildOfType<ScrollBar>(sv2);

            bar2.ValueChanged += Bar2_ValueChanged;
        }

        ScrollBar bar1, bar2;
        ScrollViewer sv1, sv2;


        private void Bar2_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //sv1.ChangeView(0, e.NewValue, null, true);
        }

        private void Bar1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //sv2.ChangeView(0,e.NewValue,null,true);
        }



        private void ItemsPresenter_LayoutUpdated(object sender, object e)
        {
            var items = ItemsPresenter;
        }

        ItemsPresenter ItemsPresenter;
        private void ItemsPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsPresenter = (sender as ItemsPresenter);
            var count = VisualTreeHelper.GetChildrenCount(ItemsPresenter);
            for (int i = 0; i < count; i++)
            {
                var item = VisualTreeHelper.GetChild(ItemsPresenter, i);
                if (i == 1)
                {
                    //(item as ItemsStackPanel).ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
                }
            }
        }

        private void ListView_Loaded_1(object sender, RoutedEventArgs e)
        {

            //(sender as ListView).IsSynchronizedWithCurrentItem = true;

        }

        double? horizontalOffset;
        DispatcherTimer a = new DispatcherTimer();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // sv1.HorizontalScrollMode = ScrollMode.Disabled;
            horizontalOffset = sv1.HorizontalOffset;
            //var a= GetFirstChildOfType<ScrollBar>(sv1, 1);
            //a.ClearValue(ScrollBar.ValueProperty);
            ItemsPresenter.SizeChanged += ItemsPresenter_SizeChanged;
            _employees.Clear();

            //a.Interval = new TimeSpan(1000);
            //a.Tick += A_Tick;
            //a.Start();
        }

        private void ItemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (horizontalOffset != null && e.NewSize!=new Size() && e.NewSize!=new Size(88,44))
            {
                sv1.ChangeView(horizontalOffset, 0, null);
                horizontalOffset = null;
            }
            
        }

        private void A_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }
        private void A_Tick(object sender, object e)
        {
            a.Stop();
            sv1.ChangeView(horizontalOffset, null, null);
            a.Tick -= A_Tick;
        }

        private void Sv1_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Debug.WriteLine("Sv1_ViewChanged");
            //if (horizontalOffset != null)
            //{
            //    //sv1.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { sv1.ChangeView(horizontalOffset, 0, null); });
            //    sv1.InvalidateScrollInfo();
            //    sv1.ChangeView(horizontalOffset, 0, null);
            //    //sv1.ScrollToVerticalOffset(horizontalOffset.Value);
            //    horizontalOffset = null;
            //}
        }

        /// <summary>
        /// Gets an element's children of a given type.
        /// </summary>
        /// <typeparam name="T">Type to look for.</typeparam>
        /// <param name="e">Parent element.</param>
        public static IEnumerable<T> GetChildrenOfType<T>(DependencyObject e) where T : DependencyObject
        {
            if (e != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(e); i++)
                {
                    var child = VisualTreeHelper.GetChild(e, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T grandChild in GetChildrenOfType<T>(child))
                    {
                        yield return grandChild;
                    }
                }
            }
        }
        /// <summary>
        /// Gets an element's first child of a given type.
        /// </summary>
        /// <typeparam name="T">Type to look for.</typeparam>
        /// <param name="e">Parent element.</param>
        /// <returns>Element's first child of type T (or the element itself if it is of type T).</returns>
        public static T GetFirstChildOfType<T>(FrameworkElement e, int index = 0) where T : DependencyObject
        {
            var t = e as T;
            if (t != null)
            {
                return t;
            }
            int i = 0;
            foreach (var child in GetChildrenOfType<T>(e))
            {
                if (i == index)
                {
                    return child;
                }
                i++;
            }
            return null;
        }
    }


    public class MyItemsControl : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListViewItem;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItem();
            return base.GetContainerForItemOverride();
        }
        protected override void OnDisconnectVisualChildren()
        {
            base.OnDisconnectVisualChildren();
        }
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }
    }
}
