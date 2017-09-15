using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit.RadialMenu
{

    public partial class RadialMenu
    {
        #region fileds
        private ObservableCollection<RadialMenuItem> _items;
        //private static SymbolIcon defaulNavigationButtonIcon = new SymbolIcon(Symbol.Setting);
        //private static SymbolIcon defaulNavigationButtonBackIcon = new SymbolIcon(Symbol.Back);
        private RadialMenuItemsPresenter _currentItemPresenter;
        private Button _navigationButton;
        private Grid _contentGrid;
        #endregion

        #region DP


        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadialMenu), new PropertyMetadata(false,new PropertyChangedCallback(OnIsExpandedChanged)));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadialMenu).IsExpandedChanged();
        }

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialMenu), new PropertyMetadata(0.0));

        public IRadialMenuItemsControl CurrentItem
        {
            get { return (IRadialMenuItemsControl)GetValue(CurrentItemProperty); }
            set { SetValue(CurrentItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(IRadialMenuItemsControl), typeof(RadialMenu), new PropertyMetadata(null));

        public int SectorCount
        {
            get { return (int)GetValue(SectorCountProperty); }
            set { SetValue(SectorCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectorCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectorCountProperty =
            DependencyProperty.Register("SectorCount", typeof(int), typeof(RadialMenu), new PropertyMetadata(0));


        public double ExpandAreaThickness
        {
            get { return (double)GetValue(ExpandAreaThicknessProperty); }
            set { SetValue(ExpandAreaThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandAreaThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandAreaThicknessProperty =
            DependencyProperty.Register("ExpandAreaThickness", typeof(double), typeof(RadialMenu), new PropertyMetadata(25.0));

        public double PointerOverElementThickness
        {
            get { return (double)GetValue(PointerOverElementThicknessProperty); }
            set { SetValue(PointerOverElementThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerOverElementThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerOverElementThicknessProperty =
            DependencyProperty.Register("PointerOverElementThickness", typeof(double), typeof(RadialMenu), new PropertyMetadata(8.0));


        public double CheckElementThickness
        {
            get { return (double)GetValue(CheckElementThicknessProperty); }
            set { SetValue(CheckElementThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckElementThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckElementThicknessProperty =
            DependencyProperty.Register("CheckElementThickness", typeof(double), typeof(RadialMenu), new PropertyMetadata(2.0));


        #region NavigationButton
        public object NavigationButtonIcon
        {
            get { return (object)GetValue(NavigationButtonIconProperty); }
            set { SetValue(NavigationButtonIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationButtonIconProperty =
            DependencyProperty.Register("NavigationButtonIcon", typeof(object), typeof(RadialMenu), new PropertyMetadata((char)0xE115));

        public object NavigationButtonBackIcon
        {
            get { return (object)GetValue(NavigationButtonBackIconProperty); }
            set { SetValue(NavigationButtonBackIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationButtonBackIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationButtonBackIconProperty =
            DependencyProperty.Register("NavigationButtonBackIcon", typeof(object), typeof(RadialMenu), new PropertyMetadata((char)0xE2A6));



        public Style NavigationButtonStyle
        {
            get { return (Style)GetValue(NavigationButtonStyleProperty); }
            set { SetValue(NavigationButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationButtonStyleProperty =
            DependencyProperty.Register("NavigationButtonStyle", typeof(Style), typeof(RadialMenu), new PropertyMetadata(null));


        #endregion


        #endregion

        #region Prop
        public ObservableCollection<RadialMenuItem> Items => _items;

        #endregion
    }
}
