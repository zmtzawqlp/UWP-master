using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit.FlexGrid
{
    public class NewFlexGridFrozenRows : ListView
    {
        internal ExpressionAnimation _offsetXAnimation;

        internal NewFlexGrid FlexGrid;
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var flexGridItem = element as ListViewItem;
            flexGridItem.RightTapped -= FlexGridItem_RightTapped;
            flexGridItem.Holding -= FlexGridItem_Holding;
            flexGridItem.RightTapped += FlexGridItem_RightTapped;
            flexGridItem.Holding += FlexGridItem_Holding;
            flexGridItem.Loaded += NewFlexGridFrozenRows_Loaded;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            var flexGridItem = element as ListViewItem;
            flexGridItem.RightTapped -= FlexGridItem_RightTapped;
            flexGridItem.Holding -= FlexGridItem_Holding;
        }

        private void FlexGridItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                if (FlexGrid != null)
                {
                    FlexGrid.OnItemRightTapped(sender, e.GetPosition(FlexGrid));
                }
            }
        }

        private void FlexGridItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                if (FlexGrid != null)
                {
                    FlexGrid.OnItemRightTapped(sender, e.GetPosition(FlexGrid));
                }
            }
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
