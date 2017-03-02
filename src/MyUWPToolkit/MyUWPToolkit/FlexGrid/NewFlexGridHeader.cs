using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace MyUWPToolkit.FlexGrid
{
    public class NewFlexGridColumnHeader : ListView
    {
        public int FrozenCount
        {
            get { return (int)GetValue(FrozenCountProperty); }
            set { SetValue(FrozenCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenCountProperty =
            DependencyProperty.Register("FrozenCount", typeof(int), typeof(NewFlexGridColumnHeader), new PropertyMetadata(0));


        internal ExpressionAnimation _offsetXAnimation;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            int index = this.IndexFromContainer(element);
            if (index > -1 && index < FrozenCount && _offsetXAnimation != null)
            {
                Canvas.SetZIndex((element as UIElement), 10);
                var _frozenContentVisual = ElementCompositionPreview.GetElementVisual(element as UIElement);

                _frozenContentVisual.StartAnimation("Offset.X", _offsetXAnimation);
            }
        }


    }
}
