using MyUWPToolkit.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit
{
    public class NumericTextBox : TextBox
    {

        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;
        private const double Epsilon = .00001;

        public event EventHandler ValueChanged;
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericTextBox), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericTextBox).UpdateValueText();
            (d as NumericTextBox).OnValueChanged();
        }

        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(NumericTextBox), new PropertyMetadata("F0"));


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MinValue));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MaxValue));



        public NumericTextBox()
        {
            Text = this.Value.ToString(CultureInfo.CurrentCulture);
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
            this.Text = this.Value.ToString(this.ValueFormat);
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
                    this.Value = Minimum;
                }
                UpdateValueText();

            }

            return false;
        }


        private bool SetValueAndUpdateValidDirections(double value)
        {
            // Range coercion is handled by base class.
            var oldValue = this.Value;
            if (value < Minimum)
            {
                value = Minimum;
            }
            if (value > Maximum)
            {
                value = Maximum;
            }
            this.Value = value;
            if (value < Minimum || value > Maximum)
            {
                UpdateValueText();
            }
            //this.SetValidIncrementDirection();

            return Math.Abs(this.Value - oldValue) > Epsilon;
        }
    }
}