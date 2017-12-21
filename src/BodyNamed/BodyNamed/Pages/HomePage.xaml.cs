using BodyNamed.Models;
using BodyNamed.Utils;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BodyNamed.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.03) };
        List<BodyName> Names = new List<BodyName>();
        Random rd = new Random();
        int Total = 0;
        public HomePage()
        {
            this.InitializeComponent();
            dt.Tick += Dt_Tick;
        }

        private void Dt_Tick(object sender, object e)
        {
            var index = new Random(Guid.NewGuid().GetHashCode()).Next(1, Total);
            //Debug.WriteLine(index);
            foreach (var item in Names)
            {
                if (item.Min <= index && index <= item.Max)
                {
                    tb.Text = item.Name;
                    break;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            tb.Text = "点击屏幕以开始";
            Names.Clear();
            var names = BodyNamesHelper.Instance.BodyNames.Where(x => x.Gender == AppSettings.BodyGender);
            Total = 1;
            foreach (var item in names)
            {
                item.Min = Total;
                item.Max = Total + (int)item.Chance.Value - 1;
                Total = item.Max + 1;
                Names.Add(item);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dt.Stop();
            base.OnNavigatedFrom(e);
        }

        bool start;
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (start)
            {
                dt.Stop();
            }
            else
            {
                dt.Start();
            }
            start = !start;
        }
    }
}
