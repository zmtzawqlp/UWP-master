using MyUWPToolkit.Util;
using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyUWPToolkit.RadialMenu
{
    /// <summary>
    /// notice: do not put RadialMenu into the virtualtree directly
    /// use RadialMenuBase AttachedMenu instead  
    /// </summary>

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
            Loaded += RadialMenu_Loaded;
            //Unloaded += RadialMenu_Unloaded;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            IsHitTestVisible = false;
            _popup = new Windows.UI.Xaml.Controls.Primitives.Popup();
            _popup.IsLightDismissEnabled = false;
            BindingOperations.SetBinding(_popup, Windows.UI.Xaml.Controls.Primitives.Popup.IsOpenProperty, new Binding() { Mode = BindingMode.TwoWay, Source = this, Path = new PropertyPath("IsOpen") });
            _popup.Child = this;
            _popup.Opened += _popup_Opened;
            _popup.Closed += _popup_Closed;
            //UpdatePopupSize();
            lowerThan14393 = !ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
        }

        private void _popup_Closed(object sender, object e)
        {
            //Debug.WriteLine("_popup_Closed" + this.Tag?.ToString());
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void _popup_Opened(object sender, object e)
        {
            //Debug.WriteLine("_popup_Opened" + this.Tag?.ToString());
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        //private void RadialMenu_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    //Window.Current.SizeChanged -= Current_SizeChanged;
        //    //Debug.WriteLine("RadialMenu_Unloaded");
        //}

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            //UpdatePopupSize();
            UpdateOffset();
        }

        private void RadialMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //UpdatePopupSize();
            //Window.Current.SizeChanged += Current_SizeChanged;
            //Debug.WriteLine("RadialMenu_Loaded");

            xPositive = 1;
            yPositive = 1;
            UpdateOffset();

            if (!IsHitTestVisible)
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { this.IsHitTestVisible = true; });
            }
            InitialIsExpandedStates();
        }

        private void InitialIsExpandedStates()
        {
            if (CurrentItem == this)
            {
                if (lowerThan14393)
                {
                    if (_contentGrid != null)
                    {
                        var ct = (_contentGrid.RenderTransform as CompositeTransform);
                        if (IsExpanded)
                        {

                            ct.ScaleX = 1;
                            ct.ScaleY = 1;
                            _navigationButton?.GoToStateExpand();
                        }
                        else
                        {
                            ct.ScaleX = 0;
                            ct.ScaleY = 0;
                            _navigationButton?.GoToStateCollapse();
                        }
                    }
                }
                else
                {
                    if (_contentGridVisual != null)
                    {
                        if (IsExpanded)
                        {
                            _contentGridVisual.Scale = new Vector3(1, 1, 0);
                            _navigationButton?.GoToStateExpand();
                        }
                        else
                        {
                            _contentGridVisual.Scale = new Vector3(0, 0, 0);
                            _navigationButton?.GoToStateCollapse();
                        }
                    }
                }
            }
        }

        #region override
        protected override void OnApplyTemplate()
        {
            //_popup = GetTemplateChild("Popup") as Windows.UI.Xaml.Controls.Primitives.Popup;

            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            _currentItemPresenter = GetTemplateChild("CurrentItemPresenter") as RadialMenuItemsPresenter;
            _currentItemPresenter.Menu = this;
            _navigationButton = GetTemplateChild("NavigationButton") as RadialMenuNavigationButton;
            _navigationButton.Click += _navigationButton_Click;
            if (!DesignMode.DesignModeEnabled)
                PrepareAnimation();
            CurrentItem = this;
            OnIsSupportInertialChanged();
            InitialIsExpandedStates();
            base.OnApplyTemplate();
        }

        //private void UpdatePopupSize()
        //{
        //    return;
        //    if (_popup == null || lowerThan14393)
        //    {
        //        return;
        //    }
        //    var windowRect = Window.Current.Bounds;
        //    if (DeviceInfo.IsNarrowSrceen)
        //    {
        //        //Gets the visible region of the window (app view). The visible region is the region 
        //        //not occluded by chrome such as the status bar and app bar.   
        //        windowRect = ApplicationView.GetForCurrentView().VisibleBounds;
        //    }
        //    _popup.Width = windowRect.Width;
        //    _popup.Height = windowRect.Height;
        //}

        private void OnCurrentItemChanged(DependencyPropertyChangedEventArgs e)
        {
            CurrentItemChanged?.Invoke(this, e);
        }

        private void OnIsSupportInertialChanged()
        {
            if (IsSupportInertial)
            {
                ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            }
            else
            {
                ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            }
        }

        private void OnOffsetChanged()
        {
            //if (lowerThan14393)
            {
                if (_popup != null)
                {
                    double x = Offset.X;
                    double y = Offset.Y;
                    if (_popup.HorizontalOffset != x || _popup.VerticalOffset != y)
                    {
                        _popup.HorizontalOffset = x;
                        _popup.VerticalOffset = y;
                    }
                }
            }
            //else
            //{
            //    if (_radialMenuVisual != null)
            //    {
            //        if (_radialMenuVisual.Offset != Offset)
            //        {
            //            _radialMenuVisual.Offset = Offset;
            //            AppSettings.TKAppOffset = Offset;
            //        }
            //    }
            //}
        }
        private void IsExpandedChanged()
        {
            //if (_contentGridVisual != null)
            //{
            if (IsExpanded)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
            //}
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
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            xPositive = 1;
            yPositive = 1;
        }

        private void UpdateOffset(double x = 0, double y = 0, bool isInertial = false)
        {
            if (this.ActualWidth == 0 || this.ActualHeight == 0)
            {
                return;
            }
            Vector3 newOffset = Vector3.Zero;

            var windowRect = Window.Current.Bounds;
            if (DeviceInfo.IsNarrowSrceen)
            {
                //Gets the visible region of the window (app view). The visible region is the region 
                //not occluded by chrome such as the status bar and app bar.   
                windowRect = ApplicationView.GetForCurrentView().VisibleBounds;

                //notice
                //higher than 10586 the StatusBar will take height event it seems like hide
                //it will make your radialmenu will never get the top boundary
                //if you are crae about this ,you can use follow code to make StatusBar are really hide.
                //if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                //{
                //    Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
                //}
            }


            var navigationButtonWidth = _navigationButton == null ? 0 : _navigationButton.ActualWidth;
            var navigationButtonHeight = _navigationButton == null ? 0 : _navigationButton.ActualHeight;


            var minX = IsExpanded ? 0 : -(this.ActualWidth - navigationButtonWidth) / 2.0;
            var minY = IsExpanded ? 0 : -(this.ActualHeight - navigationButtonHeight) / 2.0;

            var maxX = windowRect.Width - (IsExpanded ? this.ActualWidth : (this.ActualWidth + navigationButtonWidth) / 2.0);
            var maxY = windowRect.Height - (IsExpanded ? this.ActualHeight : (this.ActualHeight + navigationButtonHeight) / 2.0);

            var newX = Offset.X + (xPositive * x);
            var newY = Offset.Y + (yPositive * y);
            if (isInertial)
                BounceOffset(maxX, maxY, minX, minY, ref newX, ref newY);
            else
                ClipOffset(maxX, maxY, minX, minY, ref newX, ref newY);

            Offset = new Point(newX, newY);
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
                //when minX <0 , newX will be
                newX = minX + minX - newX;
            }
            else if (newX > maxX)
            {
                xPositive = -xPositive;
                newX = maxX - (newX % maxX);
            }

            if (newY < minY)
            {
                yPositive = -yPositive;
                newY = minY + minY - newY;
            }
            else if (newY > maxY)
            {
                yPositive = -yPositive;
                newY = maxY - (newY % maxY);
            }
        }

        //Visual _radialMenuVisual;
        Visual _contentGridVisual;
        Compositor _compositor;
        ScalarKeyFrameAnimation rotationAnimation;
        Vector3KeyFrameAnimation scaleAnimation;

        //use animation for lower than 14393
        Storyboard expand;
        Storyboard collapse;
        Storyboard open;
        Storyboard close;
        void PrepareAnimation()
        {
            if (lowerThan14393)
            {
                _contentGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                _contentGrid.RenderTransform = new CompositeTransform();
                #region expand
                expand = new Storyboard();
                var duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 1, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                expand.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 1, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                expand.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = -90 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                expand.Children.Add(duk);
                #endregion

                #region collapse
                collapse = new Storyboard();
                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 1 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                collapse.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 1 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                collapse.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = -90, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                collapse.Children.Add(duk);

                #endregion

                #region Open
                open = new Storyboard();
                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1)), Value = 1, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                open.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1)), Value = 1, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                open.Children.Add(duk);
                #endregion

                #region Close
                close = new Storyboard();
                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 1 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.07)), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                close.Children.Add(duk);

                duk = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(duk, _contentGrid);
                Storyboard.SetTargetProperty(duk, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 1 });
                duk.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.07)), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                close.Children.Add(duk);
                #endregion
            }
            else
            {
                //_radialMenuVisual = ElementCompositionPreview.GetElementVisual(this);
                //_radialMenuVisual.Offset = Offset;
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

            }
        }
        void Expand()
        {
            UpdateOffset();
            if (lowerThan14393)
            {
                expand?.Begin();
            }
            else
            {
                if (_contentGridVisual != null)
                {
                    scaleAnimation.Direction = AnimationDirection.Normal;
                    rotationAnimation.Direction = AnimationDirection.Normal;
                    scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    //opacityAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
                    _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
                    //_contentGridVisual.StartAnimation(nameof(_contentGridVisual.Opacity), opacityAnimation);

                }
            }

            _navigationButton?.GoToStateExpand();
        }

        void Collapse()
        {
            UpdateOffset();
            if (lowerThan14393)
            {
                collapse?.Begin();
            }
            else
            {
                if (_contentGridVisual != null)
                {
                    scaleAnimation.Direction = AnimationDirection.Reverse;
                    rotationAnimation.Direction = AnimationDirection.Reverse;
                    scaleAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    rotationAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
                    _contentGridVisual.StartAnimation(nameof(_contentGridVisual.RotationAngleInDegrees), rotationAnimation);
                }
            }

            _navigationButton?.GoToStateCollapse();
        }

        internal void SetCurrentItem(IRadialMenuItemsControl currentItem)
        {
            if (lowerThan14393)
            {
                if (close != null)
                {
                    EventHandler<object> handler = null;
                    handler = new EventHandler<object>((s1, e1) =>
                    {
                        close.Completed -= handler;
                        SetCurrentItemIn(currentItem);
                        open?.Begin();
                    });
                    close.Completed += handler;
                    close.Begin();
                }
                else
                {
                    SetCurrentItemIn(currentItem);
                }
            }
            else
            {
                var batch = _compositor.GetCommitBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) =>
                {
                    SetCurrentItemIn(currentItem);

                    scaleAnimation.Duration = TimeSpan.FromSeconds(0.1);
                    if (!lowerThan14393)
                    {
                        scaleAnimation.Direction = AnimationDirection.Normal;
                    }
                    _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
                };

                scaleAnimation.Duration = TimeSpan.FromSeconds(0.07);
                scaleAnimation.Direction = AnimationDirection.Reverse;
                _contentGridVisual.StartAnimation(nameof(_contentGridVisual.Scale), scaleAnimation);
            }
        }

        private void SetCurrentItemIn(IRadialMenuItemsControl currentItem)
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
        }

        internal void OnItemTapped(RadialMenuItem sender, TappedRoutedEventArgs e)
        {
            ItemTapped?.Invoke(sender, e);
        }


        public void CollapseMenu()
        {
            if (CurrentItem == null)
            {
                return;
            }
            if (CurrentItem != this)
            {
                CurrentItem = this;
                if (_navigationButton != null)
                {
                    _navigationButton.Content = this.NavigationButtonIcon ?? (char)0xE115;
                }
            }
            IsExpanded = false;
        }
        //not work, use 
        //_popup.HorizontalOffset = x;
        //_popup.VerticalOffset = y;
        //instead of use Composition API

        ///// <summary>
        ///// when page NavigationCacheMode is not disable, 
        ///// Composition API about offset will not work right
        ///// follow method is a workaround for this
        ///// </summary>
        //public void OffsetWrokaround()
        //{
        //    return;
        //    if (!lowerThan14393)
        //    {
        //        if (_radialMenuVisual != null)
        //        {      
        //            _radialMenuVisual.Offset = new Vector3();
        //            _radialMenuVisual.Offset = Offset;
        //        }
        //    }
        //}
    }
}
