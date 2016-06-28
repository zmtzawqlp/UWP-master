using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{
    internal class ExpressionAnimationItem
    {
        private Visual visual;
        //private float min;
        //private float max;
        private CompositionPropertySet scrollViewerManipProps;
        private ExpressionAnimation expression;

        //animation active
        public bool IsActive { get; set; }

        //VisualElement for ExpressionAnimation
        public ContentControl VisualElement { get; set; }

        public ContentControl TempElement { get; set; }

        public ScrollViewer ScrollViewer { get; set; }

        public Visibility Visibility
        {
            get
            {

                return VisualElement.Visibility;

            }
            set
            {
                VisualElement.Visibility = value;
            }
        }
        double VerticalOffset = -1;
        public void StartAnimation(bool update = false)
        {

            if (update || expression == null || visual == null)
            {
                visual = ElementCompositionPreview.GetElementVisual(VisualElement);
                //if (0 <= VisualElement.Margin.Top && VisualElement.Margin.Top <= ScrollViewer.ActualHeight)
                //{
                //    min = (float)-VisualElement.Margin.Top;
                //    max = (float)ScrollViewer.ActualHeight + min;
                //}
                //else if (VisualElement.Margin.Top < 0)
                //{

                //}
                //else if (VisualElement.Margin.Top > ScrollViewer.ActualHeight)
                //{

                //}
                if (scrollViewerManipProps == null)
                {
                    scrollViewerManipProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(ScrollViewer);
                }
                Compositor compositor = scrollViewerManipProps.Compositor;

                // Create the expression
                //expression = compositor.CreateExpressionAnimation("min(max((ScrollViewerManipProps.Translation.Y + VerticalOffset), MinValue), MaxValue)");
                ////Expression = compositor.CreateExpressionAnimation("ScrollViewerManipProps.Translation.Y +VerticalOffset");

                //expression.SetScalarParameter("MinValue", min);
                //expression.SetScalarParameter("MaxValue", max);
                //expression.SetScalarParameter("VerticalOffset", (float)ScrollViewer.VerticalOffset);

                expression = compositor.CreateExpressionAnimation("ScrollViewerManipProps.Translation.Y + VerticalOffset");
                ////Expression = compositor.CreateExpressionAnimation("ScrollViewerManipProps.Translation.Y +VerticalOffset");

                //expression.SetScalarParameter("MinValue", min);
                //expression.SetScalarParameter("MaxValue", max);
                VerticalOffset = ScrollViewer.VerticalOffset;
                expression.SetScalarParameter("VerticalOffset", (float)ScrollViewer.VerticalOffset);

                // set "dynamic" reference parameter that will be used to evaluate the current position of the scrollbar every frame
                expression.SetReferenceParameter("ScrollViewerManipProps", scrollViewerManipProps);

            }



            visual.StartAnimation("Offset.Y", expression);

            IsActive = true;
            //Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            //Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {
            //Update();
        }

        public void StopAnimation()
        {
            if (visual != null)
            {
                visual.StopAnimation("Offset.Y");
            }
            IsActive = false;
            //Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;
        }

        public void Update()
        {
            if (IsActive)
            {
                StopAnimation();
                Debug.WriteLine(visual.Offset.Y + "," + VerticalOffset);
                //if (min == visual.Offset.Y || max == visual.Offset.Y)
                //{

                //}
                //else
                {
                    StartAnimation(false);
                }
            }
        }
    }
}
