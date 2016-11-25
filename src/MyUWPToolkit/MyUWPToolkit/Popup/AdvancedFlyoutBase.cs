using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace MyUWPToolkit
{
    public class AdvancedFlyoutBase : DependencyObject
    {

        #region Filed
        protected Popup _popup;
        #endregion

        #region DP

        public bool IsLightDismissEnabled
        {
            get { return (bool)GetValue(IsLightDismissEnabledProperty); }
            set { SetValue(IsLightDismissEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLightDismissEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightDismissEnabledProperty =
            DependencyProperty.Register("IsLightDismissEnabled", typeof(bool), typeof(AdvancedFlyoutBase), new PropertyMetadata(true));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(AdvancedFlyoutBase), new PropertyMetadata(0.0));


        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(AdvancedFlyoutBase), new PropertyMetadata(0.0));

        public AdvancedFlyoutPlacementMode Placement
        {
            get { return (AdvancedFlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(AdvancedFlyoutPlacementMode), typeof(AdvancedFlyoutBase), new PropertyMetadata(AdvancedFlyoutPlacementMode.TopCenter));

        #endregion
        public static AdvancedFlyoutBase GetAttachedFlyout(FrameworkElement element)
        {
            return element.GetValue(AttachedFlyoutProperty) as AdvancedFlyoutBase;
        }
        public static void SetAttachedFlyout(FrameworkElement element, AdvancedFlyoutBase value)
        {
            element.SetValue(AttachedFlyoutProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachedFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedFlyoutProperty =
            DependencyProperty.RegisterAttached("AttachedFlyout", typeof(AdvancedFlyoutBase), typeof(AdvancedFlyoutBase), new PropertyMetadata(null));


        public event EventHandler<System.Object> Closed;
        public event EventHandler<System.Object> Opened;
        public event EventHandler<System.Object> Opening;



        internal FlyoutPresenter FlyoutPresenter
        {
            get
            {
                return _popup.Child as FlyoutPresenter;
            }
        }

        public static void ShowAttachedFlyout(FrameworkElement flyoutOwner)
        {
            var flyout = GetAttachedFlyout(flyoutOwner);
            if (flyout != null)
            {
                flyout.ShowAt(flyoutOwner);
            }

        }
        public void Hide()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
            }
        }

        bool reCalculatePopupPosition = false;
        FrameworkElement placementTarget = null;
        public void ShowAt(FrameworkElement placementTarget)
        {
            if (Opening != null)
            {
                Opening(this, null);
            }

            if (_popup == null)
            {
                _popup = new Popup();
                _popup.ChildTransitions = new TransitionCollection() { new PopupThemeTransition() };
                _popup.Opened += _popup_Opened;
                _popup.Closed += _popup_Closed;
                _popup.Child = CreatePresenter();
            }
            reCalculatePopupPosition = !CalculatePopupPosition(placementTarget);
            _popup.IsLightDismissEnabled = IsLightDismissEnabled;
            this.placementTarget = placementTarget;
            if (reCalculatePopupPosition || FlyoutPresenter.Style == null)
            {
                _popup.Opacity = 0;
            }

            _popup.HorizontalOffset += HorizontalOffset;
            _popup.VerticalOffset += VerticalOffset;

            _popup.IsOpen = true;
        }

        private void _popup_Closed(object sender, object e)
        {
            placementTarget = null;
            reCalculatePopupPosition = false;
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        private void _popup_Opened(object sender, object e)
        {
            //DesiredSize was not right when style was null before opened
            //we should re-calcuatePopupPosition after FlyoutPresenter get default values from default style or app resource style
            if (FlyoutPresenter.Style == null || reCalculatePopupPosition)
            {
                CalculatePopupPosition(placementTarget);
                _popup.HorizontalOffset += HorizontalOffset;
                _popup.VerticalOffset += VerticalOffset;
                _popup.Opacity = 1;
            }

            if (Opened != null)
            {
                Opened(this, e);
            }

        }

        private bool CalculatePopupPosition(FrameworkElement placementTarget)
        {
            try
            {
                var windowSize = new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

                var fpSize = new Size(0, 0);
                var fp = FlyoutPresenter;
                fp.Width = double.NaN;
                fp.Height = double.NaN;
                if (fp.DesiredSize == fpSize)
                {
                    fp.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }

                fpSize = fp.DesiredSize;

                if (placementTarget != null)
                {
                    var placementTargetRect = placementTarget.TransformToVisual(Window.Current.Content as FrameworkElement).TransformBounds(new Rect(0, 0, placementTarget.ActualWidth, placementTarget.ActualHeight));
                    switch (Placement)
                    {
                        case AdvancedFlyoutPlacementMode.TopLeft:
                        case AdvancedFlyoutPlacementMode.TopCenter:
                        case AdvancedFlyoutPlacementMode.TopRight:
                            if (!TryHandlePlacementTop(placementTargetRect, fpSize, windowSize))
                            {
                                if (!TryHandlePlacementBottom(placementTargetRect, fpSize, windowSize))
                                {
                                    if (!TryHandlePlacementLeft(placementTargetRect, fpSize, windowSize))
                                    {
                                        if (!TryHandlePlacementRight(placementTargetRect, fpSize, windowSize))
                                        {
                                            TryHandlePlacementCenterScreen(fpSize, windowSize);
                                        }
                                    }
                                }
                            }

                            break;
                        case AdvancedFlyoutPlacementMode.BottomLeft:
                        case AdvancedFlyoutPlacementMode.BottomCenter:
                        case AdvancedFlyoutPlacementMode.BottomRight:
                            if (!TryHandlePlacementBottom(placementTargetRect, fpSize, windowSize))
                            {
                                if (!TryHandlePlacementTop(placementTargetRect, fpSize, windowSize))
                                {
                                    if (!TryHandlePlacementLeft(placementTargetRect, fpSize, windowSize))
                                    {
                                        if (!TryHandlePlacementRight(placementTargetRect, fpSize, windowSize))
                                        {
                                            TryHandlePlacementCenterScreen(fpSize, windowSize);
                                        }
                                    }
                                }
                            }
                            break;
                        case AdvancedFlyoutPlacementMode.LeftTop:
                        case AdvancedFlyoutPlacementMode.LeftCenter:
                        case AdvancedFlyoutPlacementMode.LeftBottom:
                            if (!TryHandlePlacementLeft(placementTargetRect, fpSize, windowSize))
                            {
                                if (!TryHandlePlacementRight(placementTargetRect, fpSize, windowSize))
                                {
                                    if (!TryHandlePlacementTop(placementTargetRect, fpSize, windowSize))
                                    {
                                        if (!TryHandlePlacementBottom(placementTargetRect, fpSize, windowSize))
                                        {
                                            TryHandlePlacementCenterScreen(fpSize, windowSize);
                                        }
                                    }
                                }
                            }
                            break;
                        case AdvancedFlyoutPlacementMode.RightTop:
                        case AdvancedFlyoutPlacementMode.RightCenter:
                        case AdvancedFlyoutPlacementMode.RightBottom:
                            if (!TryHandlePlacementRight(placementTargetRect, fpSize, windowSize))
                            {
                                if (!TryHandlePlacementLeft(placementTargetRect, fpSize, windowSize))
                                {
                                    if (!TryHandlePlacementTop(placementTargetRect, fpSize, windowSize))
                                    {
                                        if (!TryHandlePlacementBottom(placementTargetRect, fpSize, windowSize))
                                        {
                                            TryHandlePlacementCenterScreen(fpSize, windowSize);
                                        }
                                    }
                                }
                            }
                            break;
                        case AdvancedFlyoutPlacementMode.FullScreen:
                            TryHandlePlacementFullScreen(windowSize);
                            break;
                        case AdvancedFlyoutPlacementMode.CenterScreen:
                            TryHandlePlacementCenterScreen(fpSize, windowSize);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    TryHandlePlacementCenterScreen(fpSize, windowSize);
                }

                return true;
            }
            catch (Exception ex)
            {
                // it's not the right time to CalculatePopupPosition
            }
            return false;
        }

        private bool TryHandlePlacementLeft(Rect placementTargetRect, Size fpSize, Size windowSize)
        {

            if (placementTargetRect.X - fpSize.Width < 0)
            {
                return false;
            }

            double y = 0;

            _popup.HorizontalOffset = placementTargetRect.X - fpSize.Width;

            if (fpSize.Height > windowSize.Height)
            {
                _popup.VerticalOffset = 0;
                return true;
            }

            switch (Placement)
            {
                case AdvancedFlyoutPlacementMode.LeftTop:
                    y = placementTargetRect.Y;
                    break;
                case AdvancedFlyoutPlacementMode.LeftCenter:
                    y = placementTargetRect.Y + placementTargetRect.Height / 2 - fpSize.Height / 2;
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
                case AdvancedFlyoutPlacementMode.LeftBottom:
                    y = placementTargetRect.Y + placementTargetRect.Height - fpSize.Height;
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
                default:
                    goto case AdvancedFlyoutPlacementMode.LeftCenter;
            }

            if (y + fpSize.Height > windowSize.Height)
            {
                y = windowSize.Height - fpSize.Height;
            }

            _popup.VerticalOffset = y;
            return true;
        }

        private bool TryHandlePlacementRight(Rect placementTargetRect, Size fpSize, Size windowSize)
        {

            if (placementTargetRect.Right + fpSize.Width > windowSize.Width)
            {
                return false;
            }

            double y = 0;

            _popup.HorizontalOffset = placementTargetRect.Right;

            if (fpSize.Height > windowSize.Height)
            {
                _popup.VerticalOffset = 0;
                return true;
            }


            switch (Placement)
            {
                case AdvancedFlyoutPlacementMode.RightTop:
                    y = placementTargetRect.Y;
                    break;
                case AdvancedFlyoutPlacementMode.RightCenter:
                    y = placementTargetRect.Y + placementTargetRect.Height / 2 - fpSize.Height / 2;
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
                case AdvancedFlyoutPlacementMode.RightBottom:
                    y = placementTargetRect.Y + placementTargetRect.Height - fpSize.Height;
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
                default:
                    goto case AdvancedFlyoutPlacementMode.RightCenter;
            }

            if (y + fpSize.Height > windowSize.Height)
            {
                y = windowSize.Height - fpSize.Height;
            }

            _popup.VerticalOffset = y;
            return true;
        }

        private bool TryHandlePlacementTop(Rect placementTargetRect, Size fpSize, Size windowSize)
        {
            if (placementTargetRect.Y - fpSize.Height < 0)
            {
                return false;
            }

            double x = 0;

            _popup.VerticalOffset = placementTargetRect.Y - fpSize.Height;

            if (fpSize.Width > windowSize.Width)
            {
                _popup.HorizontalOffset = 0;
                return true;
            }

            switch (Placement)
            {
                case AdvancedFlyoutPlacementMode.TopLeft:
                    x = placementTargetRect.X;
                    break;
                case AdvancedFlyoutPlacementMode.TopCenter:
                    x = placementTargetRect.X + placementTargetRect.Width / 2 - fpSize.Width / 2;
                    if (x < 0)
                    {
                        x = 0;
                    }
                    break;
                case AdvancedFlyoutPlacementMode.TopRight:
                    x = placementTargetRect.X + placementTargetRect.Width - fpSize.Width;
                    if (x < 0)
                    {
                        x = 0;
                    }
                    break;
                default:
                    goto case AdvancedFlyoutPlacementMode.TopCenter;
            }


            if (x + fpSize.Width > windowSize.Width)
            {
                x = windowSize.Width - fpSize.Width;
            }


            _popup.HorizontalOffset = x;
            return true;
        }

        private bool TryHandlePlacementBottom(Rect placementTargetRect, Size fpSize, Size windowSize)
        {
            if (placementTargetRect.Bottom + fpSize.Height > windowSize.Height)
            {
                return false;
            }

            double x = 0;
            _popup.VerticalOffset = placementTargetRect.Bottom;

            if (fpSize.Width > windowSize.Width)
            {
                _popup.HorizontalOffset = 0;
                return true;
            }

            switch (Placement)
            {
                case AdvancedFlyoutPlacementMode.BottomLeft:
                    x = placementTargetRect.X;
                    break;
                case AdvancedFlyoutPlacementMode.BottomCenter:
                    x = placementTargetRect.X + placementTargetRect.Width / 2 - fpSize.Width / 2;
                    if (x < 0)
                    {
                        x = 0;
                    }
                    break;
                case AdvancedFlyoutPlacementMode.BottomRight:
                    x = placementTargetRect.X + placementTargetRect.Width - fpSize.Width;
                    if (x < 0)
                    {
                        x = 0;
                    }
                    break;
                default:
                    goto case AdvancedFlyoutPlacementMode.BottomCenter;
            }

            if (x + fpSize.Width > windowSize.Width)
            {
                x = windowSize.Width - fpSize.Width;
            }

            _popup.HorizontalOffset = x;
            return true;
        }

        private bool TryHandlePlacementFullScreen(Size windowSize)
        {
            _popup.HorizontalOffset = 0;
            _popup.VerticalOffset = 0;
            FlyoutPresenter.Width = windowSize.Width;
            FlyoutPresenter.Height = windowSize.Height;

            return true;
        }

        private bool TryHandlePlacementCenterScreen(Size fpSize, Size windowSize)
        {

            if (fpSize.Width > windowSize.Width || fpSize.Height > windowSize.Height)
            {
                TryHandlePlacementFullScreen(windowSize);
            }
            else
            {
                _popup.HorizontalOffset = (windowSize.Width - fpSize.Width) / 2;
                _popup.VerticalOffset = (windowSize.Height - fpSize.Height) / 2;
            }
            return true;
        }

        protected virtual Control CreatePresenter()
        {
            return new FlyoutPresenter();
        }
    }
}
