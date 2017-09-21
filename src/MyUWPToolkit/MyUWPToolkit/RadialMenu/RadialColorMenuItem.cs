using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace MyUWPToolkit.RadialMenu
{
    public class RadialColorMenuItem : RadialMenuItem
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RadialColorMenuItem), new PropertyMetadata(Colors.Transparent));

        //public RadialColorMenuItem()
        //{
        //    this.DefaultStyleKey = typeof(RadialColorMenuItem);
        //    PrepareElements();
        //}
    }
}
