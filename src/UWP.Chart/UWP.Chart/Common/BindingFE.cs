using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Chart.Common
{
    class BindingFE : FrameworkElement
    {
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(BindingFE), new PropertyMetadata(null));

        public object GetValue(Binding binding)
        {
            SetBinding(ValueProperty, binding);
            object obj = Value;
            ClearValue(ValueProperty);
            return obj;
        }
    }
}
