using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit.FlexGrid
{
    public class NewFlexGridFrozenRows:ListView
    {
        internal ExpressionAnimation _offsetXAnimation;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            (element as ListViewItem).Loaded += NewFlexGridFrozenRows_Loaded;
        }

        private void NewFlexGridFrozenRows_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ListViewItem).Loaded -= NewFlexGridFrozenRows_Loaded;

            var templateRoot = (sender as ListViewItem).ContentTemplateRoot;

            var child = templateRoot.GetAllChildren();
            var _frozenContent = child.Where(x => FlexGridItemFrozenContent.GetIsFrozenContent(x));
            if (_frozenContent != null && _offsetXAnimation != null)
            {
                foreach (var item in _frozenContent)
                {
                    var _frozenContentVisual = ElementCompositionPreview.GetElementVisual(item);

                    _frozenContentVisual.StartAnimation("Offset.X", _offsetXAnimation);

                }
            }
        }
    }
}
