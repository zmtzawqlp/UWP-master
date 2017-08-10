using MyUWPToolkit.Common;
using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit
{
    public class ColorPickerNumericTextBox : TextBox
    {

        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;
        private const double Epsilon = .00001;

        public event EventHandler ValueChanged;


        public bool AllowNullValue
        {
            get { return (bool)GetValue(AllowNullValueProperty); }
            set { SetValue(AllowNullValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowNullValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowNullValueProperty =
            DependencyProperty.Register("AllowNullValue", typeof(bool), typeof(ColorPickerNumericTextBox), new PropertyMetadata(false));


        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(ColorPickerNumericTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColorPickerNumericTextBox).UpdateValueText();
            (d as ColorPickerNumericTextBox).OnValueChanged();
        }

        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(ColorPickerNumericTextBox), new PropertyMetadata("F0"));


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ColorPickerNumericTextBox), new PropertyMetadata(double.MinValue));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ColorPickerNumericTextBox), new PropertyMetadata(double.MaxValue));



        public ColorPickerNumericTextBox()
        {
            if (this.Value != null)
            {
                Text = this.Value.Value.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                this.Text = "";
            }

            TextChanged += this.OnValueTextBoxTextChanged;
            KeyDown += this.OnValueTextBoxKeyDown;
            PointerExited += this.OnValueTextBoxPointerExited;
        }

        private void OnValueTextBoxPointerExited(object sender, PointerRoutedEventArgs e)
        {

        }

        private void OnValueTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void OnValueTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateValueFromText();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.UpdateValueFromText();
            base.OnLostFocus(e);
        }


        private void UpdateValueText()
        {
            _isChangingTextWithCode = true;
            if (this.Value != null)
            {
                this.Text = this.Value.Value.ToString(this.ValueFormat);
            }
            else
            {
                this.Text = "";
            }
            this.SelectionStart = this.Text.Length;
            _isChangingTextWithCode = false;
        }

        private void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(null, null);
            }
        }

        private bool UpdateValueFromText()
        {
            if (_isChangingTextWithCode)
            {
                return false;
            }

            double val;
            if (double.TryParse(this.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out val) ||
                Calculator.TryCalculate(this.Text, out val))
            {
                _isChangingValueWithCode = true;
                if (val < Minimum)
                {
                    val = Minimum;
                }
                if (val > Maximum)
                {
                    val = Maximum;
                }

                this.Value = val;

                UpdateValueText();

                _isChangingValueWithCode = false;

                return true;
            }
            else
            {
                if (this.Text == "")
                {
                    if (!AllowNullValue)
                    {
                        this.Value = Minimum;
                    }
                    else
                    {
                        this.Value = null;
                    }

                }

                UpdateValueText();

            }

            return false;
        }


        //private bool SetValueAndUpdateValidDirections(double value)
        //{
        //    // Range coercion is handled by base class.
        //    var oldValue = this.Value;
        //    if (value < Minimum)
        //    {
        //        value = Minimum;
        //    }
        //    if (value > Maximum)
        //    {
        //        value = Maximum;
        //    }
        //    this.Value = value;
        //    if (value < Minimum || value > Maximum)
        //    {
        //        UpdateValueText();
        //    }
        //    //this.SetValidIncrementDirection();

        //    return Math.Abs(this.Value - oldValue) > Epsilon;
        //}
    }

}
