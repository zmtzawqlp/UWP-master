using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    /// <summary>
    /// note: just support in touch mode.
    /// </summary>
    [TemplatePart(Name = PanelHeader, Type = typeof(ContentControl))]
    [TemplatePart(Name = PanelContent, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = InnerCustomPanel, Type = typeof(ScrollPanel))]
    public class PullToRefreshPanel : ContentControl
    {
        #region Fields
        private const string PanelHeader = "PanelHeader";
        private const string PanelContent = "PanelContent";
        private const string ScrollViewer = "ScrollViewer";
        private const string InnerCustomPanel = "InnerCustomPanel";
        private ContentControl _panelHeader;
        private ContentPresenter _panelContent;
        private ScrollViewer _scrollViewer;
        private ScrollPanel _innerCustomPanel;
        #endregion

        #region Property

        /// <summary>
        /// The threshold for release to refresh，defautl value is 2/5 of PullToRefreshPanel's height.
        /// </summary>
        public double RefreshThreshold
        {
            get { return (double)GetValue(RefreshThresholdProperty); }
            set { SetValue(RefreshThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshThresholdProperty =
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(PullToRefreshPanel), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRefreshThresholdChanged)));

        private static void OnRefreshThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pullToRefreshPanel = d as PullToRefreshPanel;
            pullToRefreshPanel.UpdateVerticalOffset();
        }

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
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(PullToRefreshPanel), new PropertyMetadata(null));

        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(PullToRefreshPanel), new PropertyMetadata(false));

        public DateTime LastRefreshTime
        {
            get { return (DateTime)GetValue(LastRefreshTimeProperty); }
            set { SetValue(LastRefreshTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastRefreshTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastRefreshTimeProperty =
            DependencyProperty.Register("LastRefreshTime", typeof(DateTime), typeof(PullToRefreshPanel), new PropertyMetadata(DateTime.Now));



        //public bool IsRefreshing
        //{
        //    get { return (bool)GetValue(IsRefreshingProperty); }
        //    set { SetValue(IsRefreshingProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsRefreshing.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsRefreshingProperty =
        //    DependencyProperty.Register("IsRefreshing", typeof(bool), typeof(PullToRefreshPanel), new PropertyMetadata(false));



        #endregion

        protected override void OnApplyTemplate()
        {
            _panelHeader = GetTemplateChild(PanelHeader) as ContentControl;
            _panelContent = GetTemplateChild(PanelContent) as ContentPresenter;
            _scrollViewer = GetTemplateChild(ScrollViewer) as ScrollViewer;
            _innerCustomPanel = GetTemplateChild(InnerCustomPanel) as ScrollPanel;

            base.OnApplyTemplate();
            _panelHeader.DataContext = this;
            _innerCustomPanel.SizeChanged += _innerCustomPanel_SizeChanged;
            _scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
            _scrollViewer.SizeChanged += _scrollViewer_SizeChanged;
            _scrollViewer.DirectManipulationStarted += _scrollViewer_DirectManipulationStarted;
        }

        private void _scrollViewer_DirectManipulationStarted(object sender, object e)
        {
            _scrollViewer.DirectManipulationStarted -= _scrollViewer_DirectManipulationStarted;
            _panelHeader.Opacity = 1;
        }

        public PullToRefreshPanel()
        {
            this.DefaultStyleKey = typeof(PullToRefreshPanel);

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVerticalOffset();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (RefreshThreshold == 0.0)
            {
                RefreshThreshold = availableSize.Height * 2 / 5.0;
            }
            return base.MeasureOverride(availableSize);
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
            //if (_panelHeader != null && _panelHeader.Height == sv.VerticalOffset)
            //{
            //    IsRefreshing = false;
            //}
        }

        private void _scrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            //IsRefreshing = true;
            _scrollViewer.DirectManipulationCompleted -= _scrollViewer_DirectManipulationCompleted;

            _panelHeader.Height = RefreshThreshold > _panelHeader.ActualHeight ? RefreshThreshold : _panelHeader.ActualHeight;
            _scrollViewer.ChangeView(null, _panelHeader.Height, null);

            if (PullToRefresh != null)
            {
                LastRefreshTime = DateTime.Now;
                PullToRefresh(this, null);
            }

        }

        private void UpdateVerticalOffset()
        {
            if (_scrollViewer != null && _panelHeader != null)
            //&& _panelContent != null && _panelHeader != null)
            {
                _panelHeader.Height = RefreshThreshold > _panelHeader.ActualHeight ? RefreshThreshold : _panelHeader.ActualHeight;

                _scrollViewer.ChangeView(null, _panelHeader.Height, null, true);
            }
        }
    }
}
