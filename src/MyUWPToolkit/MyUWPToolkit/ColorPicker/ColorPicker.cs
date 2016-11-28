using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "ToggleButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "Flyout", Type = typeof(AdvancedFlyout))]
    [TemplatePart(Name = "BasicColorItems", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "RecentColorItems", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "Hue", Type = typeof(Slider))]
    [TemplatePart(Name = "Alpha", Type = typeof(Slider))]
    [TemplatePart(Name = "ChoiceGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "CustomColorRectangle", Type = typeof(Rectangle))]
    [TemplatePart(Name = "AColor", Type = typeof(NumericTextBox))]
    [TemplatePart(Name = "RColor", Type = typeof(NumericTextBox))]
    [TemplatePart(Name = "GColor", Type = typeof(NumericTextBox))]
    [TemplatePart(Name = "BColor", Type = typeof(NumericTextBox))]
    [TemplatePart(Name = "Indicator", Type = typeof(Canvas))]
    [TemplatePart(Name = "Pivot", Type = typeof(Pivot))]
    [TemplatePart(Name = "ChoiceGridParent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "CustomColorOkButton", Type = typeof(Button))]
    [TemplatePart(Name = "CloseButton", Type = typeof(Button))]

    public partial class ColorPicker : Control
    {

        public ColorPicker()
        {
            this.DefaultStyleKey = typeof(ColorPicker);
            InitializeDefaultBasicColor();
        }

        protected override void OnApplyTemplate()
        {
            _pivot = GetTemplateChild("Pivot") as Pivot;

            _toggleButton = GetTemplateChild("ToggleButton") as Button;
            _toggleButton.Click += _toggleButton_Click;
            _flyout = GetTemplateChild("Flyout") as AdvancedFlyout;
            _flyout.Opening += _flyout_Opening;
            _flyout.Closed += _flyout_Closed;
            _flyout.Opened += _flyout_Opened;
            _basicColorItemsControl = GetTemplateChild("BasicColorItems") as ColorPickerItemsControl;
            _recentColorItemsControl = GetTemplateChild("RecentColorItems") as ColorPickerItemsControl;
            _basicColorItemsControl.ColorPicker = this;
            _recentColorItemsControl.ColorPicker = this;
            _basicColorItemsControl.ItemsSource = _defaultBasicColors;

            _hue = GetTemplateChild("Hue") as Slider;
            AttachEventForHue();
            _alpha = GetTemplateChild("Alpha") as Slider;
            AttachEventForAlpha();

            _choiceGrid = GetTemplateChild("ChoiceGrid") as Grid;
            _choiceGridParent = GetTemplateChild("ChoiceGridParent") as ContentControl;
            _choiceGrid.SizeChanged += _choiceGrid_SizeChanged;
            _choiceGrid.PointerPressed += _choiceGrid_PointerPressed;

            _customColorRectangle = GetTemplateChild("CustomColorRectangle") as Rectangle;

            _aColor = GetTemplateChild("AColor") as NumericTextBox;
            _rColor = GetTemplateChild("RColor") as NumericTextBox;
            _gColor = GetTemplateChild("GColor") as NumericTextBox;
            _bColor = GetTemplateChild("BColor") as NumericTextBox;
            AttachEventForColor();

            _indicator = GetTemplateChild("Indicator") as Canvas;
            _indicator.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            _indicator.ManipulationDelta += _indicator_ManipulationDelta;
            _indicator.ManipulationStarted += _indicator_ManipulationStarted;
            _indicator.ManipulationCompleted += _indicator_ManipulationCompleted;

            _closeButton = GetTemplateChild("CloseButton") as Button;
            _closeButton.Click += _closeButton_Click;
            _customColorOkButton = GetTemplateChild("CustomColorOkButton") as Button;
            _customColorOkButton.Click += _customColorOkButton_Click;

            base.OnApplyTemplate();
        }

        private void _customColorOkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = CurrentCustomColor;
            Hide();
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void _choiceGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _choiceGridParent.Focus(FocusState.Programmatic);
            hueValueChanging = true;
            var pointer = e.GetCurrentPoint(_choiceGrid);
            UpdateIndicator(pointer.Position.X, pointer.Position.Y);
            hueValueChanging = false;
        }


        private void AttachEventForColor()
        {
            _aColor.ValueChanged += ValueChanged;
            _rColor.ValueChanged += ValueChanged;
            _gColor.ValueChanged += ValueChanged;
            _bColor.ValueChanged += ValueChanged;
        }

        private void RemoveEventForColor()
        {
            _aColor.ValueChanged -= ValueChanged;
            _rColor.ValueChanged -= ValueChanged;
            _gColor.ValueChanged -= ValueChanged;
            _bColor.ValueChanged -= ValueChanged;
        }

        private void AttachEventForHue()
        {
            _hue.ValueChanged += _hue_ValueChanged;
        }

        private void RemoveEventForHue()
        {
            _hue.ValueChanged -= _hue_ValueChanged;
        }

        private void AttachEventForAlpha()
        {
            _alpha.ValueChanged += _alpha_ValueChanged;
        }

        private void RemoveEventForAlpha()
        {
            _alpha.ValueChanged -= _alpha_ValueChanged;
        }

        private void _indicator_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            hueValueChanging = false;
            _choiceGridParent.Focus(FocusState.Programmatic);
        }

        private void _indicator_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            hueValueChanging = true;
        }

        private void _indicator_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            hueValueChanging = true;
            var x = Canvas.GetLeft(_indicator) + e.Delta.Translation.X;
            var y = Canvas.GetTop(_indicator) + e.Delta.Translation.Y;

            UpdateIndicator(x, y);
        }

        private void UpdateIndicator(double x, double y)
        {
            var eheight = _choiceGrid.ActualHeight;
            var ewidth = _choiceGrid.ActualWidth;

            var width = Math.Max(x, 0);
            var height = Math.Max(y, 0);
            height = height < eheight ? height : eheight;
            width = width < ewidth ? width : ewidth;
            Canvas.SetLeft(_indicator, width);
            Canvas.SetTop(_indicator, height);
            UpdateActSpectre();
        }

        private void _choiceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize != e.NewSize)
            {
                _choiceGrid.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
                if (_pivot.SelectedIndex == 1)
                {
                    SetDefaultCustomColor();
                }
            }
        }

        private void OnCurrentCustomColorChanged()
        {

        }

        private bool valueChanging = false;
        private void ValueChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("ValueChanged");
            if (alphaValueChanging || hueValueChanging)
            {
                return;
            }
            valueChanging = true;

            CurrentCustomColor = Color.FromArgb((byte)(_aColor.Value), (byte)(_rColor.Value), (byte)(_gColor.Value), (byte)(_bColor.Value));

            double[] hsl = RgbToHsl(CurrentCustomColor);
            var h = _choiceGrid.ActualHeight * (1 - 2 * hsl[2]);
            h = h >= 0 ? h : 0;
            h = h <= _choiceGrid.ActualHeight ? h : _choiceGrid.ActualHeight;
            var w = _choiceGrid.ActualWidth * hsl[1];
            w = w >= 0 ? w : 0;
            w = w <= _choiceGrid.ActualWidth ? w : _choiceGrid.ActualWidth;
            //Debug.WriteLine("H" + hsl[0]);
            _hue.Value = hsl[0] / 360f * _hue.Maximum;
            _alpha.Value = CurrentCustomColor.A;
            _actSpectre = HslToRgb(_hue.Value / _hue.Maximum * 360f, 1, 0.5);
            _choiceGrid.Background = new SolidColorBrush(_actSpectre);
            UpdatePosition(h, w);

            valueChanging = false;
            //UpdateActSpectre();
        }

        bool alphaValueChanging = false;
        private void _alpha_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //Debug.WriteLine("_alpha_ValueChanged");
            if (valueChanging || hueValueChanging)
            {
                return;
            }
            alphaValueChanging = true;


            CurrentCustomColor = Color.FromArgb((byte)(_alpha.Value * 255), (byte)(CurrentCustomColor.R), (byte)(CurrentCustomColor.G), (byte)(CurrentCustomColor.B));
            _aColor.Value = CurrentCustomColor.A;

            alphaValueChanging = false;
        }

        bool hueValueChanging = false;
        private void _hue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //Debug.WriteLine("_hue_ValueChanged");
            if (valueChanging || alphaValueChanging)
            {
                return;
            }

            hueValueChanging = true;
            UpdateActSpectre();

            hueValueChanging = false;
        }

        private void UpdateActSpectre()
        {
            _actSpectre = HslToRgb(_hue.Value / _hue.Maximum * 360f, 1, 0.5);
            _choiceGrid.Background = new SolidColorBrush(_actSpectre);

            var eheight = _choiceGrid.ActualHeight;
            var ewidth = _choiceGrid.ActualWidth;
            var x = Canvas.GetLeft(_indicator);
            var y = Canvas.GetTop(_indicator);
            var width = Math.Max(x, 0);
            var height = Math.Max(y, 0);
            height = height < eheight ? height : eheight;
            width = width < ewidth ? width : ewidth;
            //UpdatePosition(height, width);
            var ratiox = 1 - width / ewidth;
            var ratioy = 1 - height / eheight;
            var newr = (byte)((_actSpectre.R + (255 - _actSpectre.R) * ratiox) * ratioy);
            var newb = (byte)((_actSpectre.B + (255 - _actSpectre.B) * ratiox) * ratioy);
            var newg = (byte)((_actSpectre.G + (255 - _actSpectre.G) * ratiox) * ratioy);

            CurrentCustomColor = Color.FromArgb((byte)(CurrentCustomColor.A), newr, newg, newb);
            _rColor.Value = CurrentCustomColor.R;
            _gColor.Value = CurrentCustomColor.G;
            _bColor.Value = CurrentCustomColor.B;
        }

        private void InitializeDefaultBasicColor()
        {
            _defaultBasicColors = new List<Color>();
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 128, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 255, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 255, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 255, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 255, 255));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 128, 255));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 128, 192));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 128, 255));

            _defaultBasicColors.Add(Color.FromArgb(255, 255, 0, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 255, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 255, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 255, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 255, 255));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 128, 192));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 128, 192));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 0, 255));

            _defaultBasicColors.Add(Color.FromArgb(255, 128, 64, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 128, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 255, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 128, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 64, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 128, 255));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 0, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 0, 128));

            _defaultBasicColors.Add(Color.FromArgb(255, 128, 0, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 128, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 128, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 128, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 0, 255));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 0, 160));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 0, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 0, 255));

            _defaultBasicColors.Add(Color.FromArgb(255, 64, 0, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 64, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 64, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 64, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 0, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 0, 0, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 64, 0, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 64, 0, 128));

            _defaultBasicColors.Add(Color.FromArgb(255, 0, 0, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 128, 0));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 128, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 128, 128, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 64, 128, 128));
            _defaultBasicColors.Add(Color.FromArgb(255, 192, 192, 192));
            _defaultBasicColors.Add(Color.FromArgb(255, 64, 0, 64));
            _defaultBasicColors.Add(Color.FromArgb(255, 255, 255, 255));
        }

        private void _toggleButton_Click(object sender, RoutedEventArgs e)
        {
            _flyout.Placement = this.Placement;
            _flyout.ShowAt(this.PlacementTarget == null ? this : this.PlacementTarget);
        }


        private void _flyout_Opened(object sender, object e)
        {
            //((_flyout.Content as Grid).Parent as FlyoutPresenter).MaxHeight = FlyoutHeight;
            //((_flyout.Content as Grid).Parent as FlyoutPresenter).MaxWidth = FlyoutWidth;
            IsOpen = true;
            _pivot.SelectedIndex = 0;
            SetDefaultCustomColor();
            _choiceGridParent.Focus(FocusState.Programmatic);
            if (Opened != null)
            {
                Opened(sender, e);
            }
        }

        private async void _flyout_Opening(object sender, object e)
        {
            if (_recentColorItemsControl.ItemsSource == null)
            {
                _recentColorItemsControl.ItemsSource = await ColorPickerColorHelper.GetRecentColorsAsync();
            }
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void SetDefaultCustomColor()
        {
            valueChanging = false;
            hueValueChanging = false;
            alphaValueChanging = false;

            Canvas.SetLeft(_indicator, _choiceGrid.ActualWidth);
            Canvas.SetTop(_indicator, 0);
            _hue.Value = 0;
            _alpha.Value = 1;
            _aColor.Value = 255;
            hueValueChanging = true;
            UpdateActSpectre();
            hueValueChanging = false;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (_pivot.SelectedIndex != 1)
            {
                return;
            }

            if (args.VirtualKey == VirtualKey.Enter)
            {
                _customColorOkButton_Click(null, null);
                return;
            }

            var focusedElement = FocusManager.GetFocusedElement();

            if (focusedElement is TextBox || focusedElement is Slider)
            {
                return;
            }
            //if (focusedElement is ContentControl && (focusedElement as ContentControl).Content == _choiceGrid)
            {
                var x = Canvas.GetLeft(_indicator);
                var y = Canvas.GetTop(_indicator);
                switch (args.VirtualKey)
                {
                    case VirtualKey.Left:
                        //(_basicColorItemsControl.ContainerFromIndex(0) as ContentPresenter).
                        hueValueChanging = true;
                        x -= 1;
                        UpdateIndicator(x, y);
                        hueValueChanging = false;
                        break;
                    case VirtualKey.Right:
                        hueValueChanging = true;
                        x += 1;
                        UpdateIndicator(x, y);
                        hueValueChanging = false;
                        break;
                    case VirtualKey.Up:
                        hueValueChanging = true;
                        y -= 1;
                        UpdateIndicator(x, y);
                        hueValueChanging = false;
                        break;
                    case VirtualKey.Down:
                        hueValueChanging = true;
                        y += 1;
                        UpdateIndicator(x, y);
                        hueValueChanging = false;
                        break;
                    case VirtualKey.Tab:
                        break;
                    case VirtualKey.Enter:

                        break;
                    default:
                        break;
                }
            }

        }

        private void _flyout_Closed(object sender, object e)
        {
            IsOpen = false;
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            if (Closed != null)
            {
                Closed(sender, e);
            }
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_toggleButton != null)
            {
                var grid = (_toggleButton.Content as Grid);
                grid.Width = finalSize.Width;
                grid.Height = finalSize.Height;
            }
            return base.ArrangeOverride(finalSize);
        }

        private async Task OnSelectedColorChanged()
        {
            await ColorPickerColorHelper.SetRecentColorsAsync(SelectedColor);
            if (SelectedColorChanged != null)
            {
                SelectedColorChanged(this, new EventArgs());
            }
        }

        public void Show()
        {
            if (_flyout != null)
            {
                _flyout.Placement = this.Placement;
                _flyout.ShowAt(this.PlacementTarget == null ? this : this.PlacementTarget);
            }
        }

        internal void Hide()
        {
            if (_flyout != null)
            {
                _flyout.Hide();

            }
        }

        public static Color HslToRgb(double h, double s, double l)
        {
            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
            var m = l - c / 2;
            double r, g, b;
            if (h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }
            return Color.FromArgb(255, (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
        }

        /// <summary>
        /// Convert an rgb color to hsl
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>0 : hue / 1 : saturation / 2 : lightness</returns>
        public static double[] RgbToHsl(double r, double g, double b)
        {
            var m1 = Math.Max(Math.Max(r, g), b);
            var m2 = Math.Min(Math.Min(r, g), b);
            var c = m1 - m2;
            double h2;
            if (c == 0)
            {
                h2 = 0;
            }
            else if (m1 == r)
            {
                h2 = ((g - b) / c + 6) % 6;
            }
            else if (m1 == g)
            {
                h2 = (b - r) / c + 2;
            }
            else
            {
                h2 = (r - g) / c + 4;
            }
            var h = 60f * h2;
            var l = 0.5f * (m1 + m2);
            var s = l == 1 ? 0 : c / m1;
            return new[] { h, s, l / 255f };

        }

        /// <summary>
        /// Convert an rgb color to hsl
        /// </summary>
        /// <param name="value">Rgb Color <see cref="Color"/></param>
        /// <returns>0 : hue / 1 : saturation / 2 : lightness</returns>
        public static double[] RgbToHsl(Color value)
        {
            return RgbToHsl(value.R, value.G, value.B);
        }

        private void UpdatePosition(double h, double w)
        {
            Canvas.SetTop(_indicator, h);
            Canvas.SetLeft(_indicator, w);
        }
    }
}
