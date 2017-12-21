using BodyNamed.Models;
using BodyNamed.Utils;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BodyNamed.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public BodyName AddItem = new BodyName();
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ts.IsOn = AppSettings.BodyGender == Gender.Male;
            InitializeAddItem();
            Group();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //NameList.ItemsSource = null;
            BodyNamesHelper.Instance.Save();
            base.OnNavigatedFrom(e);
        }

        public void InitializeAddItem()
        {
            AddItem.Name = "周天";
            AddItem.Gender = Gender.Male;
            AddItem.Chance = 100;
        }

        public void Group()
        {
            cvs.Source = BodyNamesHelper.Instance.BodyNames.OrderBy(x => x.Gender).GroupBy(x => x.Gender);
        }
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            BodyNamesHelper.Instance.BodyNames.Remove((sender as Button).DataContext as BodyName);
            Group();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            BodyNamesHelper.Instance.BodyNames.Add(new BodyName() { Name = AddItem.Name, Gender = AddItem.Gender, Chance = AddItem.Chance });
            Group();
            InitializeAddItem();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Tag == null)
            {
                (sender as ComboBox).Tag = 1;
            }
            else
            {
                Group();
            }
        }

        private void ts_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.BodyGender = ts.IsOn ? Gender.Male : Gender.Female;
        }
    }
}
