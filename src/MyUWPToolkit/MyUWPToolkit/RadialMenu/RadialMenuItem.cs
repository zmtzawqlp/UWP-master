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

namespace MyUWPToolkit.RadialMenu
{
    [TemplatePart(Name = "ExpandIcon", Type = typeof(FontIcon))]
    [TemplatePart(Name = "ExpandButton", Type = typeof(Path))]
    [TemplatePart(Name = "ExpandButtonArea", Type = typeof(Grid))]
    [ContentProperty(Name = "Items")]
    [Bindable]
    public class RadialMenuItem : ContentControl, IRadialMenuItemsControl, INotifyPropertyChanged
    {
        private Path _expandButton;
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

        public bool HasItems
        {
            get { return _items.Count > 0; }
        }

        public RadialMenuItem()
        {
            this.DefaultStyleKey = typeof(RadialMenuItem);
            _items = new ObservableCollection<RadialMenuItem>();
            _items.CollectionChanged += _items_CollectionChanged;
            ArcSegments = new ArcSegments();
            ArcSegments.CheckElement = new ArcSegmentItem();
            ArcSegments.PointerOverElement = new ArcSegmentItem();
            ArcSegments.ExpandArea = new ArcSegmentItem();
        }

        private void _items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HasItems");
        }

        protected override void OnApplyTemplate()
        {
            _expandIcon = GetTemplateChild("ExpandIcon") as FontIcon;
            _expandButton = GetTemplateChild("ExpandButton") as Path;
            _expandButtonArea = GetTemplateChild("ExpandButtonArea") as Grid;
            _expandButtonArea.PointerEntered += _expandButtonArea_PointerEntered;
            _expandButtonArea.PointerExited += _expandButtonArea_PointerExited;
            _expandButtonArea.Tapped += _expandButtonArea_Tapped;
            base.OnApplyTemplate();
        }

        #region ExpandButtonArea

        private void _expandButtonArea_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Menu.SetCurrentItem(this);
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
