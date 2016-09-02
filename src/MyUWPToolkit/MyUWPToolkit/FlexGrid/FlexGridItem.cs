using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridItem : ListViewItem
    {
        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        Compositor _compositor;
        Visual _frozenContentVisual;
        ContentPresenter _frozenContent;
        ScrollViewer _sv;

        public DataTemplate FrozenColumnsItemTemplate
        {
            get { return (DataTemplate)GetValue(FrozenColumnsItemTemplateProperty); }
            set { SetValue(FrozenColumnsItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsItemTemplateProperty =
            DependencyProperty.Register("FrozenColumnsItemTemplate", typeof(DataTemplate), typeof(ListViewItem), new PropertyMetadata(null));

        public Visibility FrozenColumnsVisibility
        {
            get { return (Visibility)GetValue(FrozenColumnsVisibilityProperty); }
            set { SetValue(FrozenColumnsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrozenColumnsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnsVisibilityProperty =
            DependencyProperty.Register("FrozenColumnsVisibility", typeof(Visibility), typeof(ListViewItem), new PropertyMetadata(Visibility.Visible));


        public FlexGridItem()
        {
            this.DefaultStyleKey = typeof(FlexGridItem);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _frozenContent = this.GetTemplateChild("frozenContent") as ContentPresenter;
            StartAnimation(_sv);
        }

        internal void StartAnimation(ScrollViewer sv)
        {
            _sv = sv;
            if (_frozenContent == null || _sv == null || _frozenContentVisual != null)
            {
                return;
            }
            _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sv);
            _compositor = _scrollerViewerManipulation.Compositor;
            _offsetAnimation = _compositor.CreateExpressionAnimation("-min(0,ScrollManipulation.Translation.X)");
            _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

            _frozenContentVisual = ElementCompositionPreview.GetElementVisual(_frozenContent);
            _frozenContentVisual.StartAnimation("Offset.X", _offsetAnimation);
        }

        internal void StopAnimation()
        {
            if (_frozenContentVisual != null)
            {
                _frozenContentVisual.StopAnimation("Offset.X");
                _frozenContentVisual.Dispose();
                _frozenContentVisual = null;
                _compositor.Dispose();
                _compositor = null;
                _offsetAnimation.Dispose();
                _offsetAnimation = null;
                _scrollerViewerManipulation.Dispose();
                _scrollerViewerManipulation = null;
            }
        }

    }
}
