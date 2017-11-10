using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyUWPToolkit.RadialMenu
{
    public class RadialMenuBase : DependencyObject
    {
        public static RadialMenu GetAttachedMenu(DependencyObject obj)
        {
            return (RadialMenu)obj.GetValue(AttachedMenuProperty);
        }

        public static void SetAttachedMenu(DependencyObject obj, RadialMenu value)
        {
            obj.SetValue(AttachedMenuProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachedMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedMenuProperty =
            DependencyProperty.RegisterAttached("AttachedMenu", typeof(RadialMenu), typeof(RadialMenuBase), new PropertyMetadata(null));
    }
}
