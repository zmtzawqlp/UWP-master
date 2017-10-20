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
    public class RadialMenuItem : Control, IRadialMenuItemsControl, INotifyPropertyChanged
    {
        //private Path _expandButton;
        private FontIcon _expandIcon;
        private Grid _expandButtonArea;
        private Path _hitTestElement;
        internal FontIcon ExpandIcon
        {
            get
            {
                return _expandIcon;
            }
        }
        public event EventHandler IsSelectedChanged;

        public string GroupName { get; set; }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(RadialMenuItem), new PropertyMetadata(null));



        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(object), typeof(RadialMenuItem), new PropertyMetadata(null));



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

        public bool IsSelectedEnable
        {
            get { return (bool)GetValue(IsSelectedEnableProperty); }
            set { SetValue(IsSelectedEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectedEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedEnableProperty =
            DependencyProperty.Register("IsSelectedEnable", typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(true));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadialMenuItem).OnIsSelectedChanged();
        }

        public RadialMenuSelectionMode SelectionMode
        {
            get { return (RadialMenuSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(RadialMenuSelectionMode), typeof(RadialMenuItem), new PropertyMetadata(RadialMenuSelectionMode.None));

        public bool ExpandButtonEnabled
        {
            get { return (bool)GetValue(ExpandButtonEnabledProperty); }
            set { SetValue(ExpandButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandButtonEnabledProperty =
            DependencyProperty.Register("ExpandButtonEnabled", typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(true));

        protected void OnIsSelectedChanged()
        {
            if (!(IsSelectedEnable && !HasItems))
            {
                return;
            }
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, "IsSelected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            IsSelectedChanged?.Invoke(this, null);
        }

        public virtual bool HasItems
        {
            get
            {
                return _items.Where(x => x.Visibility == Visibility.Visible).Count() > 0;
            }
        }

        //public RadialMenuItem SelectedItem
        //{
        //    get
        //    {
        //        return Items.FirstOrDefault(x => x.IsSelected);
        //    }
        //}

        public virtual IEnumerable<RadialMenuItem> SelectedItems
        {
            get
            {
                return Items.Where(x => x.IsSelected);
            }
        }

        internal bool IsEmpty { get; set; }

        public event TappedEventHandler ItemTapped;

        public RadialMenuItem()
        {
            this.DefaultStyleKey = typeof(RadialMenuItem);
            PrepareElements();
            Loaded += RadialMenuItem_Loaded;
            IsEnabledChanged += RadialMenuItem_IsEnabledChanged;
        }

        private void RadialMenuItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateHitTestElementStroke();
        }

        private void UpdateHitTestElementStroke()
        {
            if (_hitTestElement != null)
            {
                var color = IsEnabled ? Color.FromArgb(1, 1, 1, 1) : Color.FromArgb(33, 0, 0, 0);
                _hitTestElement.Stroke = new SolidColorBrush(color);
            }
        }

        private void RadialMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(IsSelectedEnable && !HasItems))
            {
                return;
            }
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, "IsSelected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        protected void PrepareElements()
        {
            _items = new RadialMenuItemCollection();
            _items.CollectionChanged += _items_CollectionChanged;
            ArcSegments = new ArcSegments();
            ArcSegments.SelectedElement = new ArcSegmentItem();
            ArcSegments.PointerOverElement = new ArcSegmentItem();
            ArcSegments.ExpandArea = new ArcSegmentItem();
            if (this is RadialColorMenuItem || this is RadialNumericMenuChildrenItem)
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
            if (this is RadialNumericMenuChildrenItem)
            {
                return;
            }
            _expandIcon = GetTemplateChild("ExpandIcon") as FontIcon;
            //_expandButton = GetTemplateChild("ExpandButton") as Path;
            _expandButtonArea = GetTemplateChild("ExpandButtonArea") as Grid;
            _hitTestElement = GetTemplateChild("HitTestElement") as Path;
            UpdateHitTestElementStroke();

            _expandButtonArea.PointerEntered += _expandButtonArea_PointerEntered;
            _expandButtonArea.PointerExited += _expandButtonArea_PointerExited;
            _expandButtonArea.Tapped += _expandButtonArea_Tapped;

        }

        #region ExpandButtonArea

        private void _expandButtonArea_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (HasItems && ExpandButtonEnabled)
            {
                e.Handled = true;
                Menu?.SetCurrentItem(this);
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            //if (this is RadialNumericMenuChildrenItem)
            //{
            //    return;
            //}
            UpdateIsSelectedState();

            Menu?.OnItemTapped(this, e);
            if (ParentItem is RadialMenuItem item)
            {
                item.OnItemTapped(this, e);
            }

            base.OnTapped(e);
        }

        internal void OnItemTapped(RadialMenuItem sender, TappedRoutedEventArgs e)
        {
            ItemTapped?.Invoke(sender, e);
        }

        internal void UpdateIsSelectedState()
        {
            if (IsSelectedEnable && !HasItems)
            {
                switch (ParentItem.SelectionMode)
                {
                    case RadialMenuSelectionMode.None:
                        break;
                    case RadialMenuSelectionMode.Single:
                    case RadialMenuSelectionMode.SingleExtended:
                        foreach (var item in ParentItem.SelectedItems)
                        {
                            if (item != this && item.GroupName == this.GroupName)
                            {
                                item.IsSelected = false;
                            }
                        }
                        if (ParentItem.SelectionMode == RadialMenuSelectionMode.Single)
                        {
                            var selectedEnableItems = ParentItem.Items.Where(x => x.IsSelectedEnable && x.GroupName == this.GroupName);
                            if (selectedEnableItems.Count() == 1)
                            {
                                IsSelected = !IsSelected;
                            }
                            else
                            {
                                IsSelected = true;
                            }
                        }
                        else
                        {
                            IsSelected = !IsSelected;
                        }

                        if (IsSelected && this is RadialNumericMenuChildrenItem radialNumericMenuChildrenItem)
                        {
                            (radialNumericMenuChildrenItem.ParentItem as RadialNumericMenuItem).Value = (double)radialNumericMenuChildrenItem.Content;
                        }
                        break;
                    case RadialMenuSelectionMode.Multiple:
                        IsSelected = !IsSelected;
                        break;
                    default:
                        break;
                }
            }
        }

        private void _expandButtonArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (HasItems && ExpandButtonEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        private void _expandButtonArea_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (HasItems && ExpandButtonEnabled)
            {
                VisualStateManager.GoToState(this, "ExpandButtonPointerOver", false);
            }
        }

        #endregion

        #region IRadialMenuItemsControl
        private RadialMenuItemCollection _items;
        public RadialMenuItemCollection Items => _items;

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

        internal void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion

    }
}
