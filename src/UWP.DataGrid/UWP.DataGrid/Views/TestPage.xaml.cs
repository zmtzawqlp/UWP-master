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
    public sealed partial class TestPage : Page
    {
        //private MyIncrementalLoading<Employee> _employees;
        private ObservableCollection<Employee> _employees;
        private DispatcherTimer _timer;
        ScrollViewer ScrollViewer;
        ScrollBar ScrollBar;
        public TestPage()
        {
            this.InitializeComponent();
            Loaded += TestPage_Loaded;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 3);
            //listView.Loaded += ListView_Loaded;
            //listView1.Loaded += ListView1_Loaded;

        }

        private void ListView1_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer = GetFirstChildOfType<ScrollViewer>(listView);
            //ScrollViewer.RegisterPropertyChangedCallback(ScrollViewer.VerticalOffsetProperty, new DependencyPropertyChangedCallback(OnScrollViewerVerticalOffsetPropertyChanged));
            ScrollBar = GetFirstChildOfType<ScrollBar>(ScrollViewer);
           // ScrollViewer1 = GetFirstChildOfType<ScrollViewer>(listView1);
            var ScrollBar1 = GetFirstChildOfType<ScrollBar>(ScrollViewer1);
     
            Binding b = new Binding();
            b.Source = ScrollBar;
            b.Path = new PropertyPath("Value");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            ScrollBar1.SetBinding(ScrollBar.ValueProperty, b);
        }

        private void OnScrollViewerVerticalOffsetPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (ScrollViewer.VerticalOffset == ScrollBar.Maximum)
            {
                int count = _employees.Count - 1;

                for (int i = 0; i < 100; i++)
                {
                    _employees.RemoveAt(0);
                }


                for (int i = 0; i < 100; i++)
                {
                    //_employees.Insert(0, new Employee() { Name = "Add" + i });
                    //_employees.Insert(count, new Employee() { Name = "Add" + i });
                    _employees.Add(new Employee() { Name = "Add" + i });
                }


                //for (int i = 0; i < 100; i++)
                //{
                //    _employees.RemoveAt(0);
                //}
            }
            //else if(ScrollViewer.VerticalOffset==0)
            //{
            //    int count = _employees.Count - 1;
            //    for (int i = 0; i < 100; i++)
            //    {
            //        //_employees.Add(new Employee() { Name = "Add" + i });
            //        _employees.Insert(0, new Employee() { Name = "Add" + i });
            //    }
            //    for (int i = _employees.Count - 1; i > count; i--)
            //    {
            //        _employees.RemoveAt(i);
            //    }
            //    //for (int i = 0; i < 100; i++)
            //    //{
            //    //    _employees.RemoveAt(0);
            //    //}
            //}
            Debug.WriteLine("AfterVerticalOffset : " + ScrollViewer.VerticalOffset);
        }

        private void TestPage_Loaded(object sender, RoutedEventArgs e)
        {

            _employees = new ObservableCollection<Employee>(TestData.GetEmployees().Take(200).ToList());
            //_employees = new MyIncrementalLoading<Employee>(200, (startIndex, count) =>
            //{
            //    return TestData.GetEmployees().Take(200).ToList();
            //});


            //_employees = TestData.GetEmployees();
            listView.ItemsSource = _employees;

            //listView1.ItemsSource = _employees;
            //listView2.ItemsSource = _employees;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("BeforeVerticalOffset : " + ScrollViewer.VerticalOffset);
            istrue = true;
            int count = _employees.Count - 1;

            //_employees.Add(new Employee() { Name = "Add" + 0 });
            ////_employees.Add(new Employee() { Name = "Add" + 0 });
            //_employees.RemoveAt(0);

            //_employees.Insert(0, new Employee() { Name = "Add" + 0 });
            //_employees.RemoveAt(0);

            for (int i = 0; i < 90; i++)
            {
                _employees.Insert(0, new Employee() { Name = "Add" + i });
            }

            for (int i = 0; i < 80; i++)
            {
                _employees.RemoveAt(0);
            }

            //for (int i = _employees.Count - 1; i > count; i--)
            //{
            //    _employees.RemoveAt(i);
            //}
            //if (!_timer.IsEnabled)
            //{
            //    _timer.Start();
            //}
            //else
            //{
            //    _timer.Stop();
            //}
        }

        bool istrue = false;
        private void _timer_Tick(object sender, object e)
        {
            if (!istrue)
            {
                Debug.WriteLine("BeforeVerticalOffset : " + ScrollViewer.VerticalOffset);
                istrue = true;
                int count = _employees.Count - 1;

                //_employees.Add(new Employee() { Name = "Add" + 0 });
                ////_employees.Add(new Employee() { Name = "Add" + 0 });
                //_employees.RemoveAt(0);

                //_employees.Insert(0, new Employee() { Name = "Add" + 0 });
                //_employees.RemoveAt(0);

                for (int i = 0; i < 100; i++)
                {
                    _employees.Insert(0, new Employee() { Name = "Add" + i });
                }
                for (int i = _employees.Count - 1; i > count; i--)
                {
                    _employees.RemoveAt(i);
                }
                //istrue = false;
            }

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
        public static T GetFirstChildOfType<T>(FrameworkElement e) where T : DependencyObject
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

        private void ItemsPresenter_VerticalSnapPointsChanged(object sender, object e)
        {
            
        }
        ScrollViewer sv;

        public ScrollViewer ScrollViewer1 { get; private set; }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
           
            //sv.RegisterPropertyChangedCallback(ScrollViewer.VerticalOffsetProperty, new DependencyPropertyChangedCallback(Onchanged));
        }

        private void Onchanged(DependencyObject sender, DependencyProperty dp)
        {
            //sv = sender as ScrollViewer;
            //Debug.WriteLine(sv.VerticalOffset);
            //ScrollViewer1.ChangeView(0, sv.VerticalOffset,null);
        }

        private void ScrollContentPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            var a = (sender as ScrollContentPresenter);
            a.ManipulationDelta += A_ManipulationDelta;
        }

        private void A_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            
        }
    }
}
