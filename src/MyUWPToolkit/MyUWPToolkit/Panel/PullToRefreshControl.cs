using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class PullToRefreshControl : ContentControl
    {
        #region Fields
        private const string PanelHeader = "PanelHeader";
        private const string PanelContent = "PanelContent";
        private const string ScrollViewer = "ScrollViewer";
        private ContentControl _panelHeader;
        private ContentPresenter _panelContent;
        private ScrollViewer _scrollViewer;
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
            DependencyProperty.Register("RefreshThreshold", typeof(double), typeof(PullToRefreshControl), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRefreshThresholdChanged)));

        private static void OnRefreshThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pullToRefreshControl = d as PullToRefreshControl;
            pullToRefreshControl.UpdateContentGrid();
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
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(PullToRefreshControl), new PropertyMetadata(null));

        public bool IsReachThreshold
        {
            get { return (bool)GetValue(IsReachThresholdProperty); }
            set { SetValue(IsReachThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReachThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReachThresholdProperty =
            DependencyProperty.Register("IsReachThreshold", typeof(bool), typeof(PullToRefreshControl), new PropertyMetadata(false));


        #endregion

        protected override void OnApplyTemplate()
        {
            _panelHeader = GetTemplateChild(PanelHeader) as ContentControl;
            _panelHeader.DataContext = this;
            _panelContent = GetTemplateChild(PanelContent) as ContentPresenter;
            _scrollViewer = GetTemplateChild(ScrollViewer) as ScrollViewer;
            _scrollViewer.ViewChanged += _scrollViewer_ViewChanged;
            base.OnApplyTemplate();
        }

        private void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //Sometime we can't make it to 0.0.
            IsReachThreshold = _scrollViewer.VerticalOffset <= 5.0;
            if (e.IsIntermediate)
            {
                return;
            }

            _panelHeader.Height = RefreshThreshold > _panelHeader.ActualHeight ? RefreshThreshold : _panelHeader.ActualHeight;
            _scrollViewer.ChangeView(null, _panelHeader.Height, null);
            if (IsReachThreshold)
            {
                if (PullToRefresh != null)
                {
                    PullToRefresh(this, null);
                }
            }
        }

        public PullToRefreshControl()
        {
            this.DefaultStyleKey = typeof(PullToRefreshControl);
            this.Loaded += (s, e) =>
            {
                if (RefreshThreshold == 0.0)
                {
                    RefreshThreshold = this.ActualHeight * 2 / 5.0;
                }
                UpdateContentGrid();
            };
            this.SizeChanged += (s, e) =>
            {
                if (RefreshThreshold == 0.0)
                {
                    RefreshThreshold = this.ActualHeight * 2 / 5.0;
                }
                UpdateContentGrid();
            };
        }

        #region Method
        private void UpdateContentGrid()
        {
            if (_scrollViewer != null && _panelContent != null && _panelHeader != null)
            {
                _panelHeader.Height = RefreshThreshold > _panelHeader.ActualHeight ? RefreshThreshold : _panelHeader.ActualHeight;
                //disable animation when load control.
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _scrollViewer.ChangeView(null, _panelHeader.Height, null,true);
                });
                _panelContent.Width = this.ActualWidth;
                _panelContent.Height = this.ActualHeight;
            }
        }
        #endregion
    }
}
