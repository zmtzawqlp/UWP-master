using MyUWPToolkit.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using System.Diagnostics;
using Windows.UI.Xaml.Input;
using System.ComponentModel;
using Windows.UI;

namespace MyUWPToolkit.RadialMenu
{
    //[TemplatePart(Name = "ExpandIcon", Type = typeof(FontIcon))]
    //[TemplatePart(Name = "ExpandButton", Type = typeof(Path))]
    [ContentProperty(Name = "Items")]
    [TemplatePart(Name = "ExpandButtonArea", Type = typeof(Grid))]
    [Bindable]
    public class RadialMenuItem : ContentControl, IRadialMenuItemsControl, INotifyPropertyChanged
    {
        //private Path _expandButton;
        private FontIcon _expandIcon;
        private Grid _expandButtonArea;
        internal FontIcon ExpandIcon
        {
            get
            {
                return _expandIcon;
            }
        }
        public int SectorCount
        {
            get { return (int)GetValue(SectorCountProperty); }
            set { SetValue(SectorCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectorCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectorCountProperty =
            DependencyProperty.Register("SectorCount", typeof(int), typeof(RadialMenuItem), new PropertyMetadata(0));

        public ArcSegments ArcSegments
        {
            get { return (ArcSegments)GetValue(ArcSegmentsProperty); }
            internal set { SetValue(ArcSegmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ArcSegments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArcSegmentsProperty =
            DependencyProperty.Register("ArcSegments", typeof(ArcSegments), typeof(RadialMenuItem), new PropertyMetadata(null));

        public double ContentAngle
        {
            get { return (double)GetValue(ContentAngleProperty); }
            set { SetValue(ContentAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentAngleProperty =
            DependencyProperty.Register("ContentAngle", typeof(double), typeof(RadialMenuItem), new PropertyMetadata(0.0));



        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheckable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(false));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(false,OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadialMenuItem).OnIsCheckedChanged();
        }

        private void OnIsCheckedChanged()
        {
            if (!IsCheckable)
            {
                return;
            }
            if (IsChecked)
            {
                VisualStateManager.GoToState(this, "IsChecked", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        public bool HasItems
        {
            get { return _items.Count > 0; }
        }

        public RadialMenuItem()
        {
            this.DefaultStyleKey = typeof(RadialMenuItem);
            PrepareElements();
        }

        protected void PrepareElements()
        {
            _items = new ObservableCollection<RadialMenuItem>();
            _items.CollectionChanged += _items_CollectionChanged;
            ArcSegments = new ArcSegments();
            ArcSegments.CheckElement = new ArcSegmentItem();
            ArcSegments.PointerOverElement = new ArcSegmentItem();
            ArcSegments.ExpandArea = new ArcSegmentItem();
            if (this is RadialColorMenuItem)
            {
                ArcSegments.ColorElement = new ArcSegmentItem();
            }
            ArcSegments.HitTestElement = new ArcSegmentItem();
        }

        private void _items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HasItems");
        }

        protected override void OnApplyTemplate()
        {
            //base.OnApplyTemplate();
            _expandIcon = GetTemplateChild("ExpandIcon") as FontIcon;
            //_expandButton = GetTemplateChild("ExpandButton") as Path;
            _expandButtonArea = GetTemplateChild("ExpandButtonArea") as Grid;

            _expandButtonArea.PointerEntered += _expandButtonArea_PointerEntered;
            _expandButtonArea.PointerExited += _expandButtonArea_PointerExited;
            _expandButtonArea.Tapped += _expandButtonArea_Tapped;

        }

        #region ExpandButtonArea

        private void _expandButtonArea_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (HasItems)
            {
                e.Handled = true;
                Menu?.SetCurrentItem(this);
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (IsCheckable)
            {
                IsChecked = !IsChecked;
            }
            Menu?.OnItemTapped(this, e);
            base.OnTapped(e);
        }

        private void _expandButtonArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (HasItems)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        private void _expandButtonArea_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (HasItems)
            {
                VisualStateManager.GoToState(this, "ExpandButtonPointerOver", false);
            }
        }



        #endregion

        #region IRadialMenuItemsControl
        private ObservableCollection<RadialMenuItem> _items;
        public ObservableCollection<RadialMenuItem> Items => _items;

        #endregion


        private RadialMenu _menu;

        private IRadialMenuItemsControl _parentItem;
        public RadialMenu Menu => _menu;
        public IRadialMenuItemsControl ParentItem => _parentItem;

        internal void SetMenu(RadialMenu menu)
        {
            _menu = menu;
            _parentItem = menu?.CurrentItem;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion

    }
}
