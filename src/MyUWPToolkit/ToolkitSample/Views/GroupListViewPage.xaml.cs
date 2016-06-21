using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XamlDemo.Model;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Core;
using Windows.Foundation;
using System;
using System.Collections.Generic;
using MyUWPToolkit;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupListViewPage : Page
    {

        private System.Collections.ObjectModel.ObservableCollection<Employee> _employees;
        private ObservableCollection<Employee> _employees1;
        private ObservableCollection<Employee> _employees2;
        private ObservableCollection<Employee> _employees3;
        private ObservableCollection<Employee> _employees4;

        public GroupListViewPage()
        {
            this.InitializeComponent();
            Loaded += GroupListViewPage_Loaded;
        }

        private void GroupListViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            _employees1 = new ObservableCollection<Employee>(50, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });
            _employees2 = new ObservableCollection<Employee>(50, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });
            _employees3 = new ObservableCollection<Employee>(50, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });
            _employees4 = new ObservableCollection<Employee>(50, (startIndex, count) =>
            {
                return TestData.GetEmployees().Skip(startIndex).Take(count).ToList();
            });
            _employees = new System.Collections.ObjectModel.ObservableCollection<Employee>(TestData.GetEmployees().Take(20).ToList());


            GroupObservableCollection<Employee> list1 = new GroupObservableCollection<Employee>
                (
                new List<IList<Employee>>() { _employees, _employees1,_employees2,_employees3,_employees4 },
                new List<IGroupHeader>()
                {
                    new DefaultGroupHeader() { Name = "i'm group1" },
                    new DefaultGroupHeader() { Name = "i'm group2" },
                    new DefaultGroupHeader() { Name = "i'm group3" },
                    new DefaultGroupHeader() { Name = "i'm group4" },
                    new DefaultGroupHeader() { Name = "i'm group5" },
                }
                );

            listView.ItemsSource = list1;
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            await listView.GoToNextGroup((ScrollIntoViewAlignment)comboBox.SelectedIndex);
        }

        private async void previousButton_Click(object sender, RoutedEventArgs e)
        {
            await listView.GoToPreviousGroup((ScrollIntoViewAlignment)comboBox.SelectedIndex);
        }
    }


}
