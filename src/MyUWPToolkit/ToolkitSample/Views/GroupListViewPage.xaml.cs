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
            _employees = new System.Collections.ObjectModel.ObservableCollection<Employee>(TestData.GetEmployees().Take(50).ToList());


            GroupObservableCollection<Employee> list1 = new GroupObservableCollection<Employee>
                (
                new List<IList<Employee>>() { _employees, _employees1 },
                new List<GroupHeader>()
                {
                    new DefaultGroupHeader() { Name = "a" },
                    new DefaultGroupHeader() { Name = "b" }
                }
                );

            listView.ItemsSource = list1;
        }
    }


}
