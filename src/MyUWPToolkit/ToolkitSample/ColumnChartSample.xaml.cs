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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColumnChartSample : Page
    {
        public ColumnChartSample()
        {
            this.InitializeComponent();
            List<KeyValuePair<string, long>> valueList = new List<KeyValuePair<string, long>>();
            valueList.Add(new KeyValuePair<string, long>("净超大", -60));
            valueList.Add(new KeyValuePair<string, long>("净大单", 20));
            valueList.Add(new KeyValuePair<string, long>("净中单", -50));
            valueList.Add(new KeyValuePair<string, long>("净小单", 30));
            //valueList.Add(new KeyValuePair<string, int>("Project Manager", 40));

            cc.DataContext = valueList;
        }
    }
}
