using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using MyUWPToolkit.Util;

namespace MyUWPToolkit
{
    /// <summary>
    /// use Composition API
    /// PullToRefreshGrid for Content's first child is not ScrollViewer
    /// </summary>
    [TemplatePart(Name = "Header", Type = typeof(ContentControl))]
    [TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public class PullToRefreshGrid : ContentControl
    {
        private ContentControl _header;
        private ScrollViewer _scrollViewer;
        private Border _scrollViewerBorder;
        private ContentPresenter _content;

        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        ExpressionAnimation _opacityAnimation;
        Compositor _compositor;
        Visual _headerVisual;
        Visual _contentVisual;
        bool _refresh = false;
        DateTime _pulledDownTime, _releaseTime;
        #region DP
        /// <summary>
        /// The threshold for release to refresh，default value is header height.
        /// </summary>
        public double RefreshThreshold
        {
            get { return (double)GetValue(RefreshThresholdProperty); }
            set { SetValue(RefreshThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshThresholdProperty =
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(PullToRefreshGrid), new PropertyMetadata(0.0));

        /// <summary>
        /// occur when reach threshold.
        /// </summary>
        public event EventHandler PullToRefresh;

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(PullToRefreshGrid), new PropertyMetadata(null));

        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(PullToRefreshGrid), new PropertyMetadata(false));

        public DateTime LastRefreshTime
        {
            get { return (DateTime)GetValue(LastRefreshTimeProperty); }
            set { SetValue(LastRefreshTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastRefreshTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastRefreshTimeProperty =
            DependencyProperty.Register("LastRefreshTime", typeof(DateTime), typeof(PullToRefreshGrid), new PropertyMetadata(DateTime.Now));


        #endregion


        // Summary:
        //     Occurs when any direct manipulation of the ScrollViewer finishes.
        public event EventHandler<System.Object> DirectManipulationCompleted;
        //
        // Summary:
        //     Occurs when any direct manipulation of the ScrollViewer begins.
        public event EventHandler<System.Object> DirectManipulationStarted;

        public PullToRefreshGrid()
        {
            this.DefaultStyleKey = typeof(PullToRefreshGrid);
            Unloaded += PullToRefreshGrid_Unloaded;
        }

        private void PullToRefreshGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_header != null)
            {
                _header.Opacity = 0;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _header = GetTemplateChild("Header") as ContentControl;
            _header.DataContext = this;
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            _scrollViewer.Loaded += ScrollViewer_Loaded;
            _scrollViewer.DirectManipulationStarted += ScrollViewer_DirectManipulationStarted;
            _scrollViewer.DirectManipulationCompleted += ScrollViewer_DirectManipulationCompleted;
            _content = GetTemplateChild("Content") as ContentPresenter;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _content.Width = finalSize.Width;
            _content.Height = finalSize.Height;
            return base.ArrangeOverride(finalSize);
        }

        private void ScrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            if (DirectManipulationCompleted != null)
            {
                DirectManipulationCompleted(sender, e);
            }
            Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            var cancelled = (_releaseTime - _pulledDownTime) > TimeSpan.FromMilliseconds(250);
            _scrollViewerBorder.Clip = null;
            if (_refresh)
            {
                _refresh = false;
                if (cancelled)
                {
                    Debug.WriteLine("Refresh cancelled...");
                }
                else
                {
                    Debug.WriteLine("Refresh now!!!");
                    if (PullToRefresh != null)
                    {
                        _headerVisual.StopAnimation("Offset.Y");
                        LastRefreshTime = DateTime.Now;
                        _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);
                        PullToRefresh(this, null);
                    }
                }
            }
            if (_header != null)
            {
                _header.Opacity = 0;
            }
        }

        private void ScrollViewer_DirectManipulationStarted(object sender, object e)
        {
            if (DirectManipulationStarted != null)
            {
                DirectManipulationStarted(sender, e);
            }
            Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;
            _refresh = false;
            if (_header != null)
            {
                _header.Opacity = 1;
            }
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {
            _headerVisual.StopAnimation("Offset.Y");

            var offsetY = _headerVisual.Offset.Y;
            IsReachThreshold = offsetY >= RefreshThreshold;
            _scrollViewerBorder.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, _content.Width, _content.Height - offsetY) };
            Debug.WriteLine(IsReachThreshold + "," + _headerVisual.Offset.Y + "," + RefreshThreshold);
            _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);

            if (!_refresh)
            {
                _refresh = IsReachThreshold;
            }

            if (_refresh)
            {
                _pulledDownTime = DateTime.Now;
            }

            if (_refresh && offsetY <= 1)
            {
                _releaseTime = DateTime.Now;
            }

        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(_scrollViewer, 0);
            _scrollViewerBorder = border;
            _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerViewerManipulation.Compositor;

            double ratio = 1.0;

            _header.Measure(new Size(this.ActualWidth, this.ActualHeight));
            var headerHeight = _header.DesiredSize.Height;
            if (headerHeight == 0)
            {
                headerHeight = 50;
            }

            if (RefreshThreshold == 0.0)
            {
                RefreshThreshold = headerHeight;
            }
            ratio = RefreshThreshold / headerHeight;

            _offsetAnimation = _compositor.CreateExpressionAnimation("(min(max(0, ScrollManipulation.Translation.Y * ratio) / Divider, 1)) * MaxOffsetY");
            _offsetAnimation.SetScalarParameter("Divider", (float)RefreshThreshold);
            _offsetAnimation.SetScalarParameter("MaxOffsetY", (float)RefreshThreshold * 5 / 4);
            _offsetAnimation.SetScalarParameter("ratio", (float)ratio);
            _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

            _opacityAnimation = _compositor.CreateExpressionAnimation("min((max(0, ScrollManipulation.Translation.Y * ratio) / Divider), 1)");
            _opacityAnimation.SetScalarParameter("Divider", (float)headerHeight);
            _opacityAnimation.SetScalarParameter("ratio", (float)1);
            _opacityAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

            _headerVisual = ElementCompositionPreview.GetElementVisual(_header);

            _contentVisual = ElementCompositionPreview.GetElementVisual(_scrollViewerBorder);

            _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);
            _headerVisual.StartAnimation("Opacity", _opacityAnimation);
            _contentVisual.StartAnimation("Offset.Y", _offsetAnimation);

        }
    }


    /// <summary>
    /// use Composition API
    /// PullToRefreshGrid for Content's first child is ScrollViewer
    /// </summary>
    [TemplatePart(Name = "Header", Type = typeof(ContentControl))]
    [TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
    public class PullToRefreshGrid1 : ContentControl
    {
        private ContentControl _header;
        private ScrollViewer _scrollViewer;
        private Border _scrollViewerBorder;
        private ContentPresenter _content;

        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        ExpressionAnimation _opacityAnimation;
        Compositor _compositor;
        Visual _headerVisual;
        Visual _contentVisual;
        bool _refresh = false;
        DateTime _pulledDownTime, _releaseTime;
        #region DP
        /// <summary>
        /// The threshold for release to refresh，default value is header height.
        /// </summary>
        public double RefreshThreshold
        {
            get { return (double)GetValue(RefreshThresholdProperty); }
            set { SetValue(RefreshThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshThresholdProperty =
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(PullToRefreshGrid1), new PropertyMetadata(0.0));


        /// <summary>
        /// occur when reach threshold.
        /// </summary>
        public event EventHandler PullToRefresh;

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(PullToRefreshGrid1), new PropertyMetadata(null));

        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(PullToRefreshGrid1), new PropertyMetadata(false));

        public DateTime LastRefreshTime
        {
            get { return (DateTime)GetValue(LastRefreshTimeProperty); }
            set { SetValue(LastRefreshTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastRefreshTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastRefreshTimeProperty =
            DependencyProperty.Register("LastRefreshTime", typeof(DateTime), typeof(PullToRefreshGrid1), new PropertyMetadata(DateTime.Now));


        #endregion

        // Summary:
        //     Occurs when any direct manipulation of the ScrollViewer finishes.
        public event EventHandler<System.Object> DirectManipulationCompleted;
        //
        // Summary:
        //     Occurs when any direct manipulation of the ScrollViewer begins.
        public event EventHandler<System.Object> DirectManipulationStarted;
        public PullToRefreshGrid1()
        {
            this.DefaultStyleKey = typeof(PullToRefreshGrid1);
            Loaded += PullToRefreshGrid1_Loaded;
        }

        private void PullToRefreshGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            if (_header != null)
            {
                _header.Opacity = 0;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _header = GetTemplateChild("Header") as ContentControl;
            _header.DataContext = this;
            _content = GetTemplateChild("Content") as ContentPresenter;
            _content.Loaded += _content_Loaded;
        }

        private void _content_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = _content.GetScrollViewer();
            if (_scrollViewer != null)
            {
                var border = (Border)VisualTreeHelper.GetChild(_scrollViewer, 0);
                if (border == null)
                {
                    _scrollViewer.Loaded += ScrollViewer_Loaded;
                }
                else
                {
                    ScrollViewer_Loaded(this, null);
                }
            }
            else
            {
                throw new InvalidOperationException("content's first child must be ScrollViewer");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        private void ScrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            if (DirectManipulationCompleted != null)
            {
                DirectManipulationCompleted(sender, e);
            }

            Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            var cancelled = (_releaseTime - _pulledDownTime) > TimeSpan.FromMilliseconds(250);
            _scrollViewerBorder.Clip = null;
            if (_refresh)
            {
                _refresh = false;
                if (cancelled)
                {
                    Debug.WriteLine("Refresh cancelled...");
                }
                else
                {
                    Debug.WriteLine("Refresh now!!!");
                    if (PullToRefresh != null)
                    {
                        _headerVisual.StopAnimation("Offset.Y");
                        LastRefreshTime = DateTime.Now;
                        _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);
                        PullToRefresh(this, null);
                    }
                }
            }
            if (_header != null)
            {
                _header.Opacity = 0;
            }
        }

        private void ScrollViewer_DirectManipulationStarted(object sender, object e)
        {
            if (DirectManipulationStarted != null)
            {
                DirectManipulationStarted(sender, e);
            }

            Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;
            _refresh = false;
            if (_header != null)
            {
                _header.Opacity = 1;
            }
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {
            _headerVisual.StopAnimation("Offset.Y");

            var offsetY = _headerVisual.Offset.Y;
            IsReachThreshold = offsetY >= RefreshThreshold;
            _scrollViewerBorder.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, _content.ActualWidth, _content.ActualHeight - offsetY) };
            Debug.WriteLine(IsReachThreshold + "," + _headerVisual.Offset.Y + "," + RefreshThreshold);
            _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);

            if (!_refresh)
            {
                _refresh = IsReachThreshold;
            }

            if (_refresh)
            {
                _pulledDownTime = DateTime.Now;
            }

            if (_refresh && offsetY <= 1)
            {
                _releaseTime = DateTime.Now;
            }

        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer.DirectManipulationStarted += ScrollViewer_DirectManipulationStarted;
            _scrollViewer.DirectManipulationCompleted += ScrollViewer_DirectManipulationCompleted;
            var border = (Border)VisualTreeHelper.GetChild(_scrollViewer, 0);
            _scrollViewerBorder = border;
            _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerViewerManipulation.Compositor;

            double ratio = 1.0;

            _header.Measure(new Size(this.ActualWidth, this.ActualHeight));
            var headerHeight = _header.DesiredSize.Height;
            if (headerHeight == 0)
            {
                headerHeight = 50;
            }

            if (RefreshThreshold == 0.0)
            {
                RefreshThreshold = headerHeight;
            }
            ratio = RefreshThreshold / headerHeight;

            _offsetAnimation = _compositor.CreateExpressionAnimation("(min(max(0, ScrollManipulation.Translation.Y * ratio) / Divider, 1)) * MaxOffsetY");
            _offsetAnimation.SetScalarParameter("Divider", (float)RefreshThreshold);
            _offsetAnimation.SetScalarParameter("MaxOffsetY", (float)RefreshThreshold * 5 / 4);
            _offsetAnimation.SetScalarParameter("ratio", (float)ratio);
            _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

            _opacityAnimation = _compositor.CreateExpressionAnimation("min((max(0, ScrollManipulation.Translation.Y * ratio) / Divider), 1)");
            _opacityAnimation.SetScalarParameter("Divider", (float)headerHeight);
            _opacityAnimation.SetScalarParameter("ratio", (float)1);
            _opacityAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);

            _headerVisual = ElementCompositionPreview.GetElementVisual(_header);

            _contentVisual = ElementCompositionPreview.GetElementVisual(_scrollViewerBorder);

            _headerVisual.StartAnimation("Offset.Y", _offsetAnimation);
            _headerVisual.StartAnimation("Opacity", _opacityAnimation);
            _contentVisual.StartAnimation("Offset.Y", _offsetAnimation);

        }
    }

}
