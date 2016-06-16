using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public TestPage()
        {
            this.InitializeComponent();
            Loaded += TestPage_Loaded;
        }

        private void TestPage_Loaded(object sender, RoutedEventArgs e)
        {
            _employees = new ObservableCollection<Employee>(TestData.GetEmployees().Take(1).ToList());
            //_employees = new MyIncrementalLoading<Employee>(200, (startIndex, count) =>
            //{
            //    return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            //});

            //_employees = TestData.GetEmployees();
            listView.ItemsSource = _employees;
        }
    }
}
