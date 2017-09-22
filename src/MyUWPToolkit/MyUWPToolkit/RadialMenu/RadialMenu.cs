using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit.RadialMenu
{

    [ContentProperty(Name = "Items")]
    [Bindable]
    [TemplatePart(Name = "NavigationButton", Type = typeof(RadialMenuNavigationButton))]
    [TemplatePart(Name = "CurrentItemPresenter", Type = typeof(RadialMenuItemsPresenter))]
    [TemplatePart(Name = "ContentGrid", Type = typeof(Grid))]
    public partial class RadialMenu : Control, IRadialMenuItemsControl
    {
        public RadialMenu()
        {
            this.DefaultStyleKey = typeof(RadialMenu);
            _items = new RadialMenuItemCollection();
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            Loaded += RadialMenu_Loaded;
            Unloaded += RadialMenu_Unloaded;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        private void RadialMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateOffset();
        }

        private void RadialMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            xPositive = 1;
            yPositive = 1;
            UpdateOffset();
        }

        #region override
        protected override void OnApplyTemplate()
        {

            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            _currentItemPresenter = GetTemplateChild("CurrentItemPresenter") as RadialMenuItemsPresenter;
            _currentItemPresenter.Menu = this;
            _navigationButton = GetTemplateChild("NavigationButton") as RadialMenuNavigationButton;
            _navigationButton.Click += _navigationButton_Click;
            if (!DesignMode.DesignModeEnabled)
                PrepareAnimation();
            CurrentItem = this;
            base.OnApplyTemplate();
        }

        private void IsExpandedChanged()
        {
            if (_contentGridVisual != null)
            {
                if (IsExpanded)
                {
                    Expand();
                }
                else
                {
                    Collapse();
                }
            }
        }

        private void _navigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem == this)
            {
                IsExpanded = !IsExpanded;
            }
            else
            {
                SetCurrentItem((CurrentItem as RadialMenuItem).ParentItem);
            }
        }
        #endregion

        int xPositive = 1;
        int yPositive = 1;
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            UpdateOffset(e.Delta.Translation.X, e.Delta.Translation.Y, e.IsInertial);
            //Debug.WriteLine(Offset);
            base.OnManipulationDelta(e);
        }

        private void UpdateOffset(double x = 0, double y = 0, bool isInertial = false)
        {
            Vector3 newOffset = Vector3.Zero;
            var windowRect = Window.Current.Bounds;

            var minX = IsExpanded ? 0 : -(this.ActualWidth - _navigationButton.ActualWidth) / 2.0;
            var minY = IsExpanded ? 0 : -(this.ActualHeight - _navigationButton.ActualHeight) / 2.0;

            var maxX = windowRect.Width - (IsExpanded ? this.ActualWidth : (this.ActualWidth + _navigationButton.ActualWidth) / 2.0);
            var maxY = windowRect.Height - (IsExpanded ? this.ActualHeight : (this.ActualHeight + _navigationButton.ActualHeight) / 2.0);

            var newX = Offset.X + (xPositive * x);
            var newY = Offset.Y + (yPositive * y);
            if (isInertial)
                BounceOffset(maxX, maxY, minX, minY, ref newX, ref newY);
            else
                ClipOffset(maxX, maxY, minX, minY, ref newX, ref newY);

            Offset = new Vector3((float)newX, (float)newY, 0);
        }

        private void ClipOffset(double maxX, double maxY, double minX, double minY, ref double newX, ref double newY)
        {
            newX = Math.Max(minX, Math.Min(maxX, newX));
            newY = Math.Max(minY, Math.Min(maxY, newY));
        }

        private void BounceOffset(double maxX, double maxY, double minX, double minY, ref double newX, ref double newY)
        {
            if (newX < minX)
            {
                xPositive = -xPositive;
                newX = -newX;
            }
            else if (newX > maxX)
            {
                xPositive = -xPositive;
                newX = maxX - (newX % maxX);
            }

            if (newY < minY)
            {
                yPositive = -yPositive;
                newY = -newY;
            }
            else if (newY > maxY)
            {
                yPositive = -yPositive;
                newY = maxY - (newY % maxY);
            }
        }
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            xPositive = 1;
            yPositive = 1;
            base.OnManipulationStarted(e);
        }
        private void OnOffsetChanged()
        {
            if (_radialMenuVisual != null)
            {
                _radialMenuVisual.Offset = Offset;
            }
        }

        Visual _radialMenuVisual;
        Visual _contentGridVisual;
        Compositor _compositor;
        ScalarKeyFrameAnimation rotationAnimation;
        Vector3KeyFrameAnimation scaleAnimation;
        //ScalarKeyFrameAnimation opacityAnimation;
        void PrepareAnimation()
        {
            _radialMenuVisual = ElementCompositionPreview.GetElementVisual(this);
            _radialMenuVisual.Offset = Offset;
            _contentGridVisual = ElementCompositionPreview.GetElementVisual(_contentGrid);
            _compositor = _contentGridVisual.Compositor;

            rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            //opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();

            var easing = _compositor.CreateLinearEasingFunction();

            _contentGrid.SizeChanged += (s, e) =>
            {
                _contentGridVisual.CenterPoint = new Vector3((float)_contentGrid.ActualWidth / 2.0f, (float)_contentGrid.ActualHeight / 2.0f, 0);
            };

            scaleAnimation.InsertKeyFrame(0.0f, new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f });
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3() { X = 1.0f, Y = 1.0f, Z = 0.0f }, easing);

            rotationAnimation.InsertKeyFrame(0.0f, -90.0f);
            rotationAnimation.InsertKeyFrame(1.0f, 0.0f, easing);

            //opacityAnimation.InsertKeyFrame(0.0f, 0.0f);
            //opacityAnimation.InsertKeyFrame(1.0f, 1.0f, easing);

            _contentGridVisual.Scale = new Vector3(0, 0, 0);

        }
        public void Expand()
        {
            UpdateOffset();
            scaleAnimation.Direction = AnimationDirection.Normal;
            rotationAnimation.Direction = AnimationDirection.Normal;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
            //opacityAnimation.Duration = TimeSpan.FromSeconds(0.2);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
            //_contentGridVisual.StartAnimation(nameof(_contentGridVisual.Opacity), opacityAnimation);
            _navigationButton.GoToStateExpand();
        }

        public void Collapse()
        {
            UpdateOffset();
            scaleAnimation.Direction = AnimationDirection.Reverse;
            rotationAnimation.Direction = AnimationDirection.Reverse;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
            //opacityAnimation.Duration = TimeSpan.FromSeconds(0.2);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
            //_contentGridVisual.StartAnimation(nameof(_contentGridVisual.Opacity), opacityAnimation);
            _navigationButton.GoToStateCollapse();
        }

        internal void SetCurrentItem(IRadialMenuItemsControl currentItem)
        {
            var batch = _compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            batch.Completed += (s, e) =>
            {
                CurrentItem = currentItem;
                if (CurrentItem == this)
                {
                    _navigationButton.Content = this.NavigationButtonIcon ?? (char)0xE115;
                }
                else
                {
                    if (CurrentItem is RadialNumericMenuItem)
                    {
                        _navigationButton.GoToStateNumeric();
                    }
                    else
                    {
                        _navigationButton.GoToStateExpand();
                    }
                    _navigationButton.Content = this.NavigationButtonBackIcon ?? (char)0xE2A6;
                }
                scaleAnimation.Direction = AnimationDirection.Normal;
                scaleAnimation.Duration = TimeSpan.FromSeconds(0.15);
                _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            };
            scaleAnimation.Direction = AnimationDirection.Reverse;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.1);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
        }

        internal void OnItemTapped(RadialMenuItem sender, TappedRoutedEventArgs e)
        {
            ItemTapped?.Invoke(sender, e);
        }
    }
}
