using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [TemplatePart(Name = "Flyout", Type = typeof(Flyout))]
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

    public partial class ColorPicker : Control
    {

        public ColorPicker()
        {
            this.DefaultStyleKey = typeof(ColorPicker);
            InitializeDefaultBasicColor();
        }

        protected override void OnApplyTemplate()
        {
            _toggleButton = GetTemplateChild("ToggleButton") as Button;
            _toggleButton.Click += _toggleButton_Click;
            _flyout = GetTemplateChild("Flyout") as Flyout;
            _flyout.Opening += _flyout_Opening;
            _flyout.Closed += _flyout_Closed;
            _flyout.Opened += _flyout_Opened;
            _basicColorItemsControl = GetTemplateChild("BasicColorItems") as ColorPickerItemsControl;
            _recentColorItemsControl = GetTemplateChild("RecentColorItems") as ColorPickerItemsControl;
            _basicColorItemsControl.ColorPicker = this;
            _recentColorItemsControl.ColorPicker = this;
            _basicColorItemsControl.ItemsSource = _defaultBasicColors;

            _hue = GetTemplateChild("Hue") as Slider;
            _hue.ValueChanged += _hue_ValueChanged;
            _alpha = GetTemplateChild("Alpha") as Slider;
            _alpha.ValueChanged += _alpha_ValueChanged;

            _choiceGrid = GetTemplateChild("ChoiceGrid") as Grid;

            _customColorRectangle = GetTemplateChild("CustomColorRectangle") as Rectangle;

            _aColor = GetTemplateChild("AColor") as NumericTextBox;
            _rColor = GetTemplateChild("RColor") as NumericTextBox;
            _gColor = GetTemplateChild("GColor") as NumericTextBox;
            _bColor = GetTemplateChild("BColor") as NumericTextBox;
            _aColor.ValueChanged += ValueChanged;
            _rColor.ValueChanged += ValueChanged;
            _gColor.ValueChanged += ValueChanged;
            _bColor.ValueChanged += ValueChanged;
            base.OnApplyTemplate();
        }

        private void OnCurrentCustomColorChanged()
        {
            _aColor.Value = CurrentCustomColor.A;
            _rColor.Value = CurrentCustomColor.R;
            _gColor.Value = CurrentCustomColor.G;
            _bColor.Value = CurrentCustomColor.B;
            _alpha.Value = CurrentCustomColor.A / 255.0;

        }

        private void ValueChanged(object sender, EventArgs e)
        {
            CurrentCustomColor = Color.FromArgb((byte)(_aColor.Value), (byte)(_rColor.Value), (byte)(_gColor.Value), (byte)(_bColor.Value));
        }

        private void _alpha_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateActSpectre();
        }

        private void _hue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateActSpectre();
        }

        private void UpdateActSpectre()
        {
            _actSpectre = HslToRgb(_hue.Value / _hue.Maximum * 360f, 1, 0.5);
            _choiceGrid.Background = new SolidColorBrush(_actSpectre);
            CurrentCustomColor = Color.FromArgb((byte)(255 * _alpha.Value), _actSpectre.R, _actSpectre.G, _actSpectre.B);
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

        }

        private async void _flyout_Opening(object sender, object e)
        {
            if (_recentColorItemsControl.ItemsSource == null)
            {
                _recentColorItemsControl.ItemsSource = await ColorPickerColorHelper.GetRecentColorsAsync();
            }
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            _hue.Value = 0;
            _alpha.Value = 1;
            UpdateActSpectre();
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    //(_basicColorItemsControl.ContainerFromIndex(0) as ContentPresenter).
                    var focusedElement = FocusManager.GetFocusedElement();
                    break;
                case VirtualKey.Right:
                    break;
                case VirtualKey.Up:
                    break;
                case VirtualKey.Down:
                    break;
                case VirtualKey.Tab:
                    break;
                default:
                    break;
            }
        }

        private void _flyout_Closed(object sender, object e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
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
    }
}
