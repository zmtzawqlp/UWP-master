using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit.RadialMenu
{
    [TemplatePart(Name = "BackgroundElement", Type = typeof(Ellipse))]
    public class RadialMenuNavigationButton : ContentControl
    {
        private Ellipse _backgroundElement;
        public event RoutedEventHandler Click;
        public RadialMenuNavigationButton()
        {
            this.DefaultStyleKey = typeof(RadialMenuNavigationButton);
        }

        protected override void OnApplyTemplate()
        {
            _backgroundElement = GetTemplateChild("BackgroundElement") as Ellipse;
            if (!DesignMode.DesignModeEnabled)
            {
                GoToStateCollapse();
            }
           
            base.OnApplyTemplate();
        }

        public void GoToStateExpand()
        {
            VisualStateManager.GoToState(this, "Expand", false);
        }

        public void GoToStateCollapse()
        {
            VisualStateManager.GoToState(this, "Collapse", false);
        }

        public void GoToStateNumeric()
        {
            VisualStateManager.GoToState(this, "Numeric", false);
        }

        //Visual _backgroundElementVisual;
        //Compositor _compositor;
        //ScalarKeyFrameAnimation rotationAnimation;
        //Vector3KeyFrameAnimation scaleAnimation;
        //ScalarKeyFrameAnimation opacityAnimation;
        //void PrepareAnimation()
        //{
        //    _backgroundElementVisual = ElementCompositionPreview.GetElementVisual(_backgroundElement);
        //    _compositor = _backgroundElementVisual.Compositor;

        //    rotationAnimation = _compositor.CreateColorKeyFrameAnimation();
        //    scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
        //    opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();

        //    var easing = _compositor.CreateLinearEasingFunction();

        //    //_contentGrid.SizeChanged += (s, e) =>
        //    //{
        //    //    _contentGridVisual.CenterPoint = new Vector3((float)_contentGrid.ActualWidth / 2.0f, (float)_contentGrid.ActualHeight / 2.0f, 0);
        //    //};

        //    //scaleAnimation.InsertKeyFrame(0.0f, new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f });
        //    //scaleAnimation.InsertKeyFrame(1.0f, new Vector3() { X = 1.0f, Y = 1.0f, Z = 0.0f }, easing);

        //    //rotationAnimation.InsertKeyFrame(0.0f, -90.0f);
        //    //rotationAnimation.InsertKeyFrame(1.0f, 0.0f, easing);

        //    //opacityAnimation.InsertKeyFrame(0.0f, 0.0f);
        //    //opacityAnimation.InsertKeyFrame(1.0f, 1.0f, easing);

        //    //_contentGridVisual.Scale = new Vector3(0, 0, 0);

        //}



        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Click?.Invoke(this, new RoutedEventArgs());
            base.OnTapped(e);
        }
    }
}
