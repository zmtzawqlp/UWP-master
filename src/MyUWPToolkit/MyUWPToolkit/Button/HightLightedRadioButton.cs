using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{
    public class HightLightedRadioButton: RadioButton
    {

        public Brush HightLightedBackground
        {
            get { return (Brush)GetValue(HightLightedBackgroundProperty); }
            set { SetValue(HightLightedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HightLightedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightLightedBackgroundProperty =
            DependencyProperty.Register("HightLightedBackground", typeof(Brush), typeof(HightLightedRadioButton), new PropertyMetadata(null));



        public Brush HightLightedForeground
        {
            get { return (Brush)GetValue(HightLightedForegroundProperty); }
            set { SetValue(HightLightedForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HightLightedForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightLightedForegroundProperty =
            DependencyProperty.Register("HightLightedForeground", typeof(Brush), typeof(HightLightedRadioButton), new PropertyMetadata(null));

        public HightLightedRadioButton()
        {
            this.DefaultStyleKey = typeof(HightLightedRadioButton);
        }
    }
}
