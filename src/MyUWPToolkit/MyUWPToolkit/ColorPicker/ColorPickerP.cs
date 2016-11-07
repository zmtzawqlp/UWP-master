using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit
{
    public partial class ColorPicker
    {
        #region Fileds
        Button _toggleButton;
        Flyout _flyout;
        ColorPickerItemsControl _basicColorItemsControl;
        ColorPickerItemsControl _recentColorItemsControl;
        private List<Color> _defaultBasicColors;
        private Color _actSpectre;
        private Slider _hue;
        private Slider _alpha;
        private Grid _choiceGrid;
        private Rectangle _customColorRectangle;
        private NumericTextBox _aColor;
        private NumericTextBox _rColor;
        private NumericTextBox _gColor;
        private NumericTextBox _bColor;
        private Canvas _indicator;
        private Pivot _pivot;
        private ContentControl _choiceGridParent;
        private Button _customColorOkButton;
        private Button _closeButton;
        //private double _x;
        //private double _y;
        #endregion

        #region Property
        public event ColorChangedEventHandler SelectedColorChanged;

        #endregion

        #region DependencyProperty
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent, new PropertyChangedCallback(OnSelectedColorChanged)));

        private static async void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await (d as ColorPicker).OnSelectedColorChanged();
        }

        public FrameworkElement PlacementTarget
        {
            get { return (FrameworkElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlacementTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(FrameworkElement), typeof(ColorPicker), new PropertyMetadata(null));



        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(FlyoutPlacementMode), typeof(ColorPicker), new PropertyMetadata(FlyoutPlacementMode.Top));



        public Visibility ArrowVisibility
        {
            get { return (Visibility)GetValue(ArrowVisibilityProperty); }
            set { SetValue(ArrowVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ArrowVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrowVisibilityProperty =
            DependencyProperty.Register("ArrowVisibility", typeof(Visibility), typeof(ColorPicker), new PropertyMetadata(Visibility.Visible));



        public object Owner
        {
            get { return (object)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Owner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(object), typeof(ColorPicker), new PropertyMetadata(null));



        public Color CurrentCustomColor
        {
            get { return (Color)GetValue(CurrentCustomColorProperty); }
            internal set { SetValue(CurrentCustomColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCustomColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCustomColorProperty =
            DependencyProperty.Register("CurrentCustomColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent, new PropertyChangedCallback(OnCurrentCustomColorChanged)));

        private static void OnCurrentCustomColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColorPicker).OnCurrentCustomColorChanged();
        }


        #endregion

    }
}
