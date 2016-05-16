using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace MyUWPToolkit.Util
{
    internal static class Util
    {

        public static bool IsPrimitive(Type t)
        {
            return
                t == null ? false :
                t == typeof(string) ? true :
                 t.GetTypeInfo().IsPrimitive;
        }

        public static T GetFirstChildOfType<T>(this FrameworkElement e) where T : DependencyObject
        {
            var t = e as T;
            if (t != null)
            {
                return t;
            }
            foreach (var child in GetChildrenOfType<T>(e))
            {
                return child;
            }
            return null;
        }

        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject e) where T : DependencyObject
        {
            if (e != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(e); i++)
                {
                    var child = VisualTreeHelper.GetChild(e, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T grandChild in GetChildrenOfType<T>(child))
                    {
                        yield return grandChild;
                    }
                }
            }
        }
        public static IEnumerable<object> GetVisibleItems(ItemsControl itemsControl)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                var obj = itemsControl.ContainerFromIndex(i) as FrameworkElement;
                if (obj != null)
                {
                    GeneralTransform gt = obj.TransformToVisual(itemsControl);
                    var rect = gt.TransformBounds(new Rect(0, 0, obj.ActualWidth, obj.ActualHeight));

                    if (rect.Bottom < 0 || rect.Top > itemsControl.ActualHeight)
                    {
                        continue;
                    }

                    yield return itemsControl.Items[i];
                }
            }
        }

    }
}
