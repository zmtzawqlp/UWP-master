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
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Markup;

namespace MyUWPToolkit
{
    /// <summary>
    /// use Composition API in 10586 or later
    /// PullToRefreshGrid for Content's first child is not ScrollViewer
    /// </summary>
    [TemplatePart(Name = "Header", Type = typeof(ContentControl))]
    [TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public class PullToRefreshGrid : ContentControl
    {
        #region Common
        private ContentControl _header;
        private ScrollViewer _scrollViewer;
        private ContentPresenter _content;
        #endregion

        #region CompositionAPI
        private Border _scrollViewerBorder;
        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        ExpressionAnimation _opacityAnimation;
        Compositor _compositor;
        Visual _headerVisual;
        Visual _contentVisual;
        bool _refresh = false;
        DateTime _pulledDownTime, _releaseTime;
        #endregion

        #region No-CompositionAPI
        private const string PanelHeader = "PanelHeader";
        private const string PanelContent = "PanelContent";
        private const string ScrollViewer = "ScrollViewer";
        private const string InnerCustomPanel = "InnerCustomPanel";
        private ScrollPanel _innerCustomPanel;
        string NoUseCompositionAPIControlTemplateString = @"
      <ControlTemplate   xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                         xmlns:local='using:MyUWPToolkit' TargetType='local:PullToRefreshGrid'>
        <local:PullToRefreshBorder x:Name='OuterCustomPanel' HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>
            <ScrollViewer x:Name='ScrollViewer' VerticalSnapPointsType='MandatorySingle'  VerticalSnapPointsAlignment='Near'
                          VerticalScrollMode='Enabled' VerticalScrollBarVisibility='Hidden' VerticalContentAlignment='Stretch' VerticalAlignment='Stretch'>
                <local:ScrollPanel x:Name='InnerCustomPanel'>
                    <ContentControl x:Name='PanelHeader' Opacity='0' ContentTemplate='{TemplateBinding HeaderTemplate}' 
                                          HorizontalContentAlignment='Center' VerticalContentAlignment='Bottom'/>
                    <ContentPresenter x:Name='PanelContent'  ContentTemplate='{TemplateBinding ContentTemplate}' ContentTransitions='{TemplateBinding ContentTransitions}' Content='{TemplateBinding Content}' HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}' Margin='{TemplateBinding Padding}' VerticalAlignment='{TemplateBinding VerticalContentAlignment}'/>
                </local:ScrollPanel>
            </ScrollViewer>
        </local:PullToRefreshBorder>
      </ControlTemplate>";

        private ControlTemplate _noUseCompositionAPIControlTemplate;

        internal ControlTemplate NoUseCompositionAPIControlTemplate
        {
            get
            {
                if (_noUseCompositionAPIControlTemplate == null)
                {
                    _noUseCompositionAPIControlTemplate = XamlReader.Load(NoUseCompositionAPIControlTemplateString) as ControlTemplate;
                }
                return _noUseCompositionAPIControlTemplate;

            }
        }
        #endregion

        internal bool UseCompositionAPI { get; set; }
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
            UseCompositionAPI = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 2);
            //UseCompositionAPI = false;
            if (!UseCompositionAPI && NoUseCompositionAPIControlTemplate != null)
            {
                Template = NoUseCompositionAPIControlTemplate;
            }
            if (UseCompositionAPI)
            {
                Unloaded += PullToRefreshGrid_Unloaded;
            }
            else
            {
                this.Loaded += OnLoaded;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (UseCompositionAPI)
            {
                _header = GetTemplateChild("Header") as ContentControl;
                _header.DataContext = this;
                _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
                _scrollViewer.Loaded += ScrollViewer_Loaded;
                _scrollViewer.DirectManipulationStarted += ScrollViewer_DirectManipulationStarted;
                _scrollViewer.DirectManipulationCompleted += ScrollViewer_DirectManipulationCompleted;
                _content = GetTemplateChild("Content") as ContentPresenter;
            }
            else
            {
                _header = GetTemplateChild(PanelHeader) as ContentControl;
                _content = GetTemplateChild(PanelContent) as ContentPresenter;
                _scrollViewer = GetTemplateChild(ScrollViewer) as ScrollViewer;
                _innerCustomPanel = GetTemplateChild(InnerCustomPanel) as ScrollPanel;

                _header.DataContext = this;
                _innerCustomPanel.SizeChanged += _innerCustomPanel_SizeChanged;
                _scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
                _scrollViewer.SizeChanged += _scrollViewer_SizeChanged;
                _scrollViewer.DirectManipulationStarted += _scrollViewer_DirectManipulationStarted;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (UseCompositionAPI)
            {
                _content.Width = finalSize.Width;
                _content.Height = finalSize.Height;
            }
            else
            {
                if (RefreshThreshold == 0.0)
                {
                    RefreshThreshold = finalSize.Height * 2 / 5.0;
                }
            }

            if (_header != null)
            {
                _header.Width = finalSize.Width;
            }
            return base.ArrangeOverride(finalSize);
        }

        #region Non-CompositionAPI

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVerticalOffset();
        }

        private void _scrollViewer_DirectManipulationStarted(object sender, object e)
        {
            _scrollViewer.DirectManipulationStarted -= _scrollViewer_DirectManipulationStarted;
            if (_header != null)
            {
                _header.Opacity = 1;
            }
        }

        private void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _innerCustomPanel?.InvalidateMeasure();
        }

        private void _innerCustomPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVerticalOffset();
        }

        private void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            IsReachThreshold = sv.VerticalOffset == 0;

            //修复当scroller滚动之后，从别的页面回来的时候，会触发refresh
            if (IsReachThreshold)
            {
                _scrollViewer.DirectManipulationCompleted -= _scrollViewer_DirectManipulationCompleted;
                _scrollViewer.DirectManipulationCompleted += _scrollViewer_DirectManipulationCompleted;
            }
        }

        private void _scrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            _scrollViewer.DirectManipulationCompleted -= _scrollViewer_DirectManipulationCompleted;

            _header.Height = RefreshThreshold > _header.ActualHeight ? RefreshThreshold : _header.ActualHeight;
            _scrollViewer.ChangeView(null, _header.Height, null);

            if (PullToRefresh != null)
            {
                LastRefreshTime = DateTime.Now;
                PullToRefresh(this, null);
            }

        }

        private void UpdateVerticalOffset()
        {
            if (_scrollViewer != null && _header != null)
            //&& _panelContent != null && _panelHeader != null)
            {
                _header.Height = RefreshThreshold > _header.ActualHeight ? RefreshThreshold : _header.ActualHeight;

                _scrollViewer.ChangeView(null, _header.Height, null, true);
            }
        }
        #endregion

        #region CompositionAPI

        private void PullToRefreshGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_header != null)
            {
                _header.Opacity = 0;
            }
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
        #endregion
    }


    /// <summary>
    /// use Composition API in 10586 or later
    /// PullToRefreshGrid for Content's first child is ScrollViewer
    /// </summary>
    [TemplatePart(Name = "Header", Type = typeof(ContentControl))]
    [TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
    public class PullToRefreshGrid1 : ContentControl
    {
        #region Common
        private ContentControl _header;
        private ScrollViewer _scrollViewer;
        private ContentPresenter _content;
        #endregion

        #region CompositionAPI
        private Border _scrollViewerBorder;
        CompositionPropertySet _scrollerViewerManipulation;
        ExpressionAnimation _offsetAnimation;
        ExpressionAnimation _opacityAnimation;
        Compositor _compositor;
        Visual _headerVisual;
        Visual _contentVisual;
        bool _refresh = false;
        DateTime _pulledDownTime, _releaseTime;
        #endregion

        #region No-CompositionAPI
        private const string PanelHeader = "PanelHeader";
        private const string PanelContent = "PanelContent";
        private const string ScrollViewer = "ScrollViewer";
        private const string InnerCustomPanel = "InnerCustomPanel";
        private ScrollPanel _innerCustomPanel;
        string NoUseCompositionAPIControlTemplateString = @"
      <ControlTemplate   xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                         xmlns:local='using:MyUWPToolkit' TargetType='local:PullToRefreshGrid1'>
        <local:PullToRefreshBorder x:Name='OuterCustomPanel' HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>
            <ScrollViewer x:Name='ScrollViewer' VerticalSnapPointsType='MandatorySingle'  VerticalSnapPointsAlignment='Near'
                          VerticalScrollMode='Enabled' VerticalScrollBarVisibility='Hidden' VerticalContentAlignment='Stretch' VerticalAlignment='Stretch'>
                <local:ScrollPanel x:Name='InnerCustomPanel'>
                    <ContentControl x:Name='PanelHeader' Opacity='0' ContentTemplate='{TemplateBinding HeaderTemplate}' 
                                                HorizontalContentAlignment='Center' VerticalContentAlignment='Bottom' 
                                          />
                    <ContentPresenter x:Name='PanelContent'  ContentTemplate='{TemplateBinding ContentTemplate}' ContentTransitions='{TemplateBinding ContentTransitions}' Content='{TemplateBinding Content}' HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}' Margin='{TemplateBinding Padding}' VerticalAlignment='{TemplateBinding VerticalContentAlignment}'/>
                </local:ScrollPanel>
            </ScrollViewer>
        </local:PullToRefreshBorder>
      </ControlTemplate>";

        private ControlTemplate _noUseCompositionAPIControlTemplate;

        internal ControlTemplate NoUseCompositionAPIControlTemplate
        {
            get
            {
                if (_noUseCompositionAPIControlTemplate == null)
                {
                    _noUseCompositionAPIControlTemplate = XamlReader.Load(NoUseCompositionAPIControlTemplateString) as ControlTemplate;
                }
                return _noUseCompositionAPIControlTemplate;

            }
        }
        #endregion

        internal bool UseCompositionAPI { get; set; }
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

            UseCompositionAPI = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 2);
            //UseCompositionAPI = false;
            if (!UseCompositionAPI && NoUseCompositionAPIControlTemplate != null)
            {
                Template = NoUseCompositionAPIControlTemplate;
            }
            if (UseCompositionAPI)
            {
                Unloaded += PullToRefreshGrid1_Loaded;
            }
            else
            {
                this.Loaded += OnLoaded;
            }

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (UseCompositionAPI)
            {
                _header = GetTemplateChild("Header") as ContentControl;
                _header.DataContext = this;
                _content = GetTemplateChild("Content") as ContentPresenter;
                _content.Loaded += _content_Loaded;
            }
            else
            {
                _header = GetTemplateChild(PanelHeader) as ContentControl;
                _content = GetTemplateChild(PanelContent) as ContentPresenter;
                _scrollViewer = GetTemplateChild(ScrollViewer) as ScrollViewer;
                _innerCustomPanel = GetTemplateChild(InnerCustomPanel) as ScrollPanel;

                _header.DataContext = this;
                _innerCustomPanel.SizeChanged += _innerCustomPanel_SizeChanged;
                _scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
                _scrollViewer.SizeChanged += _scrollViewer_SizeChanged;
                _scrollViewer.DirectManipulationStarted += _scrollViewer_DirectManipulationStarted;
            }

        }

        private void _content_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = _content.GetScrollViewer();
            if (_scrollViewer != null)
            {
                _scrollViewer.DirectManipulationStarted += ScrollViewer_DirectManipulationStarted;
                _scrollViewer.DirectManipulationCompleted += ScrollViewer_DirectManipulationCompleted;
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
            if (!UseCompositionAPI && RefreshThreshold == 0.0)
            {
                RefreshThreshold = finalSize.Height * 2 / 5.0;
            }
            if (_header != null)
            {
                _header.Width = finalSize.Width;
            }
            return base.ArrangeOverride(finalSize);
        }

        #region Non-CompositionAPI

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVerticalOffset();
        }

        private void _scrollViewer_DirectManipulationStarted(object sender, object e)
        {
            _scrollViewer.DirectManipulationStarted -= _scrollViewer_DirectManipulationStarted;
            if (_header != null)
            {
                _header.Opacity = 1;
            }
        }

        private void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _innerCustomPanel?.InvalidateMeasure();
        }

        private void _innerCustomPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVerticalOffset();
        }

        private void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            IsReachThreshold = sv.VerticalOffset == 0;

            //修复当scroller滚动之后，从别的页面回来的时候，会触发refresh
            if (IsReachThreshold)
            {
                _scrollViewer.DirectManipulationCompleted -= _scrollViewer_DirectManipulationCompleted;
                _scrollViewer.DirectManipulationCompleted += _scrollViewer_DirectManipulationCompleted;
            }
        }

        private void _scrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            _scrollViewer.DirectManipulationCompleted -= _scrollViewer_DirectManipulationCompleted;

            _header.Height = RefreshThreshold > _header.ActualHeight ? RefreshThreshold : _header.ActualHeight;
            _scrollViewer.ChangeView(null, _header.Height, null);

            if (PullToRefresh != null)
            {
                LastRefreshTime = DateTime.Now;
                PullToRefresh(this, null);
            }

        }

        private void UpdateVerticalOffset()
        {
            if (_scrollViewer != null && _header != null)
            //&& _panelContent != null && _panelHeader != null)
            {
                _header.Height = RefreshThreshold > _header.ActualHeight ? RefreshThreshold : _header.ActualHeight;

                _scrollViewer.ChangeView(null, _header.Height, null, true);
            }
        }
        #endregion

        #region CompositionAPI

        private void PullToRefreshGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            if (_header != null)
            {
                _header.Opacity = 0;
            }
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
        #endregion
    }

}
