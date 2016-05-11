using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using XamlDemo.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlexGridSamplePage : Page
    {
        public FlexGridSamplePage()
        {
            this.InitializeComponent();

            listview.ItemsSource = TestData.GetEmployees();
            listview1.ItemsSource = TestData.GetEmployees();
            Loaded += FlexGridSamplePage_Loaded;
        }
        ScrollViewer s1;
        ScrollViewer s2;
        ScrollBar sb1;
        ScrollBar sb2;
        private void FlexGridSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            s1 = GetFirstChildOfType<ScrollViewer>(listview);
            s2 = GetFirstChildOfType<ScrollViewer>(listview1);
            //s1.ManipulationMode = ManipulationModes.TranslateInertia | ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            if (s1 != null && s2 != null)
            {
                //sb1 = GetChildrenOfType<ScrollBar>(s1).FirstOrDefault(x => x.Name == "VerticalScrollBar");

                //sb2 = GetChildrenOfType<ScrollBar>(s2).FirstOrDefault(x => x.Name == "VerticalScrollBar");
                //sb1.ValueChanged += Sb1_ValueChanged;
                //sb2.ValueChanged += Sb2_ValueChanged;
                s1.ViewChanged += S1_ViewChanged;
                s2.ViewChanged += S2_ViewChanged;
                //s1.ViewChanging += S1_ViewChanging;
                s1.DirectManipulationStarted += S1_DirectManipulationStarted;
                s1.DirectManipulationCompleted += S1_DirectManipulationCompleted;
                
                //s2.ViewChanging += S2_ViewChanging;
            }

        }

        private void S1_DirectManipulationCompleted(object sender, object e)
        {
            
        }

        private void S1_DirectManipulationStarted(object sender, object e)
        {
            
        }

        private void Sb2_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            s1.ScrollToVerticalOffset(s2.VerticalOffset);
        }

        private void Sb1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            s2.ScrollToVerticalOffset(s1.VerticalOffset);
        }

        private void S2_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            s1.ViewChanging -= S1_ViewChanging;
            s1.ScrollToVerticalOffset(s2.VerticalOffset);
        }

        private void S1_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            s2.ViewChanging -= S2_ViewChanging;

            s2.ScrollToVerticalOffset(e.NextView.VerticalOffset);
            //s2.ScrollToVerticalOffset(s1.VerticalOffset);
        }

        private void S2_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate)
            {
                s1.ScrollToVerticalOffset(s2.VerticalOffset);
            }
        }

        private void S1_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate)
            {
                //this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { s2.ScrollToVerticalOffset(s1.VerticalOffset); });
                //s1.ViewChanging += S1_ViewChanging;
                s2.ScrollToVerticalOffset(s1.VerticalOffset);
            }
        }


        public T GetFirstChildOfType<T>(FrameworkElement e) where T : DependencyObject
        {
            var t = e as T;
            if (t != null)
            {
                return t;
            }
            foreach (var child in GetChildrenOfType<T>(e))
            {
                return child;
            }
            return null;
        }

        public IEnumerable<T> GetChildrenOfType<T>(DependencyObject e) where T : DependencyObject
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

        private void listview_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //s1.ChangeView(s1.HorizontalOffset + e.Delta.Translation.X, s1.VerticalOffset + e.Delta.Translation.Y,null);
            //s2.ChangeView(null,s2.VerticalOffset + e.Delta.Translation.Y,null);
        }

        private void ItemsPresenter_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (flag && (e.Delta.Translation.Y > 0 || PullToRefreshPanel.IsPulling()))
            {
                (sender as ItemsPresenter).ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateY;
                PullToRefreshPanel.UpdateVerticalOffset(e.Delta.Translation.Y);
                //if (PullToRefreshPanel.IsReachThreshold)
                //{

                //}
            }
            else
            {
                (sender as ItemsPresenter).ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
                //Debug.WriteLine(s1.VerticalOffset);
                s1.ChangeView(s1.HorizontalOffset - e.Delta.Translation.X, s1.VerticalOffset - e.Delta.Translation.Y, null, true);
                s2.ChangeView(null, s2.VerticalOffset - e.Delta.Translation.Y, null, true);
            }


        }

        private void ItemsPresenter_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var s = (sender as ItemsPresenter).Parent as ScrollViewer;
            if (s != null)
            {
                if (s.VerticalOffset == 0)
                {
                    e.Handled = true;
                }
            }
        }

        private void listview_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //this.ManipulationMode = ManipulationModes.System;
        }

        bool flag = false;
        private void ItemsPresenter_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            var s = (sender as ItemsPresenter).Parent as ScrollViewer;
            if (s != null)
            {
                if (s.VerticalOffset == 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
        }

        private void ItemsPresenter_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {

            if (flag)
            {
                (sender as ItemsPresenter).ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateY;
            }
            else
            {
                (sender as ItemsPresenter).ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            }
        }

        private void ItemsPresenter_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }
    }
}
