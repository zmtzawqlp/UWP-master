using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit.RadialMenu
{
    [TemplatePart(Name = "NavigationButton", Type = typeof(Button))]
    [TemplatePart(Name = "CurrentItemPresenter", Type = typeof(RadialMenuItemsPresenter))]
    [TemplatePart(Name = "ContentGrid", Type = typeof(Grid))]
    [ContentProperty(Name = "Items")]
    [Bindable]
    public partial class RadialMenu : Control, IRadialMenuItemsControl
    {
        public RadialMenu()
        {
            this.DefaultStyleKey = typeof(RadialMenu);
            _items = new ObservableCollection<RadialMenuItem>();
        }
        #region override
        protected override void OnApplyTemplate()
        {
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            _currentItemPresenter = GetTemplateChild("CurrentItemPresenter") as RadialMenuItemsPresenter;
            _currentItemPresenter.Menu = this;
            _navigationButton = GetTemplateChild("NavigationButton") as Button;
            _navigationButton.Click += _navigationButton_Click;
            CurrentItem = this;
            PrepareAnimation();
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

        Visual _contentGridVisual;
        Compositor _compositor;
        ScalarKeyFrameAnimation rotationAnimation;
        Vector3KeyFrameAnimation scaleAnimation;
        void PrepareAnimation()
        {
            _contentGridVisual = ElementCompositionPreview.GetElementVisual(_contentGrid);
            _compositor = _contentGridVisual.Compositor;

            rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            var easing = _compositor.CreateLinearEasingFunction();
   
            _contentGrid.SizeChanged += (s, e) =>
            {
                _contentGridVisual.CenterPoint = new Vector3((float)_contentGrid.ActualWidth / 2.0f, (float)_contentGrid.ActualHeight / 2.0f, 0);
            };

            scaleAnimation.InsertKeyFrame(0.0f, new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f });
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3() { X = 1.0f, Y = 1.0f, Z = 0.0f }, easing);

            rotationAnimation.InsertKeyFrame(0.0f, -90.0f);
            rotationAnimation.InsertKeyFrame(1.0f, 0.0f, easing);
            _contentGridVisual.Scale = new Vector3(0,0,0);
        }
        public void Expand()
        {
            //_navigationButton.RequestedTheme = ElementTheme.Dark;
            scaleAnimation.Direction = AnimationDirection.Normal;
            rotationAnimation.Direction = AnimationDirection.Normal;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
        }

        public void Collapse()
        {
            //_navigationButton.RequestedTheme = ElementTheme.Light;
           
            scaleAnimation.Direction = AnimationDirection.Reverse;
            rotationAnimation.Direction = AnimationDirection.Reverse;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
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
    }
}
