using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.RadialMenu
{

    public partial class RadialMenu
    {
        #region fileds
        private ObservableCollection<RadialMenuItem> _items;
        //private static SymbolIcon defaulNavigationButtonIcon = new SymbolIcon(Symbol.Setting);
        //private static SymbolIcon defaulNavigationButtonBackIcon = new SymbolIcon(Symbol.Back);
        private RadialMenuItemsPresenter _currentItemPresenter;
        internal RadialMenuNavigationButton _navigationButton;
        private Grid _contentGrid;
        #endregion

        #region DP


        public bool FillEmptyPlaces
        {
            get { return (bool)GetValue(FillEmptyPlacesProperty); }
            set { SetValue(FillEmptyPlacesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillEmptyPlaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillEmptyPlacesProperty =
            DependencyProperty.Register("FillEmptyPlaces", typeof(bool), typeof(RadialMenu), new PropertyMetadata(true));


        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadialMenu), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedChanged)));

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


        public double SelectedElementThickness
        {
            get { return (double)GetValue(SelectedElementThicknessProperty); }
            set { SetValue(SelectedElementThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedElementThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedElementThicknessProperty =
            DependencyProperty.Register("SelectedElementThickness", typeof(double), typeof(RadialMenu), new PropertyMetadata(8.0));


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



        //public Style NavigationButtonStyle
        //{
        //    get { return (Style)GetValue(NavigationButtonStyleProperty); }
        //    set { SetValue(NavigationButtonStyleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NavigationButtonStyle.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NavigationButtonStyleProperty =
        //    DependencyProperty.Register("NavigationButtonStyle", typeof(Style), typeof(RadialMenu), new PropertyMetadata(null));


        #endregion


        #region ExpandButton

        public Brush ExpandButtonBackground
        {
            get { return (Brush)GetValue(ExpandButtonBackgroundProperty); }
            set { SetValue(ExpandButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandButtonBackgroundProperty =
            DependencyProperty.Register("ExpandButtonBackground", typeof(Brush), typeof(RadialMenu), new PropertyMetadata(null));

        public Brush ExpandButtonPointerOverBackground
        {
            get { return (Brush)GetValue(ExpandButtonPointerOverBackgroundProperty); }
            set { SetValue(ExpandButtonPointerOverBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandButtonPointerOverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandButtonPointerOverBackgroundProperty =
            DependencyProperty.Register("ExpandButtonPointerOverBackground", typeof(Brush), typeof(RadialMenu), new PropertyMetadata(null));


        #endregion

        public Brush SelectedElementBackground
        {
            get { return (Brush)GetValue(SelectedElementBackgroundProperty); }
            set { SetValue(SelectedElementBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedElementBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedElementBackgroundProperty =
            DependencyProperty.Register("SelectedElementBackground", typeof(Brush), typeof(RadialMenu), new PropertyMetadata(null));



        public RadialMenuSelectionMode SelectionMode
        {
            get { return (RadialMenuSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(RadialMenuSelectionMode), typeof(RadialMenu), new PropertyMetadata(RadialMenuSelectionMode.Single));

        public Vector3 Offset
        {
            get { return (Vector3)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Vector3), typeof(RadialMenu), new PropertyMetadata(Vector3.Zero, OnOffsetChanged));

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadialMenu).OnOffsetChanged();
        }



        public bool IsSupportInertial
        {
            get { return (bool)GetValue(IsSupportInertialProperty); }
            set { SetValue(IsSupportInertialProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSupportInertial.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSupportInertialProperty =
            DependencyProperty.Register("IsSupportInertial", typeof(bool), typeof(RadialMenu), new PropertyMetadata(true, OnIsSupportInertialChanged));

        private static void OnIsSupportInertialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadialMenu rm = d as RadialMenu;
            if (rm.IsSupportInertial)
            {
                rm.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            }
            else
            {
                rm.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            }
        }

        #endregion

        #region Prop
        public ObservableCollection<RadialMenuItem> Items
        {
            get
            {
                return _items;
            }
        }

        //public RadialMenuItem SelectedItem
        //{
        //    get
        //    {
        //        return Items.FirstOrDefault(x => x.IsSelected);
        //    }
        //}

        public IEnumerable<RadialMenuItem> SelectedItems
        {
            get
            {
                return Items.Where(x => x.IsSelected);
            }
        }

        public event TappedEventHandler ItemTapped;
        #endregion
    }

    //public struct RadialMenuOffset : IEquatable<RadialMenuOffset>
    //{
    //    //
    //    // Summary:
    //    //     The X component of the vector.
    //    public float X;
    //    //
    //    // Summary:
    //    //     The Y component of the vector.
    //    public float Y;

    //    public static RadialMenuOffset Zero
    //    {
    //        get
    //        {
    //            return  new RadialMenuOffset(0, 0);
    //        }
    //    }

    //    public RadialMenuOffset(float x, float y)
    //    {
    //        X = x;
    //        Y = y;
    //    }

    //    public bool Equals(RadialMenuOffset other)
    //    {
    //        return X == other.X && Y == other.Y;
    //    }

    //}

    //public class RadialMenuConverter : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return base.CanConvertFrom(context, sourceType);
    //    }
    //}

}
